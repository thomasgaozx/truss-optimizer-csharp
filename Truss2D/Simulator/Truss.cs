using System;
using System.Linq;
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

        public decimal? GetInternalForce(Edge edge)
        {
            return internalForces[edge];
        }

        public decimal? MaxInternalForce =>
            internalForces.Values.Max();
        public decimal? MinInternalForce =>
            internalForces.Values.Min();

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
        /// Should be called before solving new element
        /// </summary>
        private void Reset()
        {
            var keys = internalForces.Keys.ToArray();
            foreach (var key in keys)
            {
                internalForces[key] = null;
            }
        }

        /// <summary>
        /// Returns a hashset of solved joints.
        /// Internal forces are reset before solving.
        /// </summary>
        /// <param name="successStatus">returns whether the solving is complete or not</param>
        /// <returns></returns>
        public HashSet<Joint> Solve(out bool successStatus)
        {
            Reset();

            Solver solver = new Solver(internalForces);
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
                    solver.JointDecomposition(joint);
                    int numSolved = solver.Solve(out bool complete);

                    if (complete)
                        solved.Add(joint);
                    else
                        burndown.Enqueue(joint);

                    progress = numSolved != 0;
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

            if (!jointMap.ContainsKey(a)) jointMap.Add(a, jointA);
            if (!jointMap.ContainsKey(b)) jointMap.Add(b, jointB);
        }

        [Obsolete("using a vertice to find force is redundant")]
        public void AddForce(Vertice point, Vector force)
        {
            if (!jointMap.ContainsKey(point))
                throw new Exception("No joint found on the given point ...");

            jointMap[point].AddReaction(force);
        }

    }
}