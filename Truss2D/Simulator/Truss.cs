using System;
using System.Collections.Generic;
using Truss2D.Math;

namespace Truss2D
{
    /// <summary>
    /// Flow:
    /// 1. Building the shape of the trusses using add edge
    /// 2. Add known external forces
    /// 3. Render
    /// 4. Solve
    /// </summary>
    public class Truss
    {

        private Dictionary<Edge, decimal?> internalForces; 
        private Dictionary<Vertice, Joint> jointMap;

        public Truss()
        {
            internalForces = new Dictionary<Edge, decimal?>();
            jointMap = new Dictionary<Vertice,Joint>();
        }

        public void Clear()
        {
            internalForces.Clear();
            jointMap.Clear();
        }

        /// <summary>
        /// Returns a hashset of solved joints.
        /// </summary>
        /// <param name="successStatus">returns whether the solving is complete or not</param>
        /// <returns></returns>
        public HashSet<Joint> Solve(out bool successStatus)
        {
            Solver solver = new Solver();
            Queue<Joint> burndown = new Queue<Joint>();
            HashSet<Joint> solved = new HashSet<Joint>();

            foreach (var joint in jointMap.Values)
            {
                burndown.Enqueue(joint);
            }

            bool progress = true;
            while (progress || burndown.Count==0)
            {
                int cycleLength = burndown.Count;
                progress = false;
                for (int i = 0; i < cycleLength; ++i)
                {
                    Joint joint = burndown.Dequeue();
                    Dictionary<Force, Edge> unknowns = new Dictionary<Force,Edge>();
                    var neighbours = joint.Neightbours;
                    foreach (var neighbour in neighbours)
                    {
                        Edge edge = new Edge(joint, neighbour);
                        Force newForce = new Force(internalForces[edge], edge.DirectionFrom(joint));
                        if (newForce.IsUnknown())
                            unknowns.Add(newForce,edge);
                        solver.AddForce(newForce);
                    }

                    foreach (var reaction in joint.Reactions)
                    {
                        solver.AddForce(reaction);
                    }

                    bool complete = solver.Solve();

                    foreach (var unknown in unknowns.Keys)
                    {
                        if (!unknown.IsUnknown())
                        {
                            progress = true;
                            internalForces[unknowns[unknown]] = unknown.Magnitude;
                        }
                    }

                    if (!complete)
                        burndown.Enqueue(joint);
                    else
                        solved.Add(joint);

                }

            }

            if (burndown.Count == 0)
                successStatus = true;
            else
                successStatus = false;

            return solved;
        }

        public void AddEdge(Vertice a, Vertice b)
        {
            Edge newEdge = new Edge(a, b);
            if (internalForces.ContainsKey(newEdge))
                throw new Exception("Edge already exists");

            internalForces.Add(newEdge, null);

            Joint jointA = new Joint(a);
            Joint jointB = new Joint(b);

            jointA.AddNeighbour(jointB);
            jointB.AddNeighbour(jointB);

            jointMap.Add(a, jointA);
            jointMap.Add(b, jointB);
        }

        public void AddForce(Vertice point, Force force)
        {
            if (!jointMap.ContainsKey(point))
                throw new Exception("No joint found on the given point ...");

            jointMap[point].AddReactions(force);
        }

    }
}