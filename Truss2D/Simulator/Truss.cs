using System;
using System.Linq;
using System.Collections.Generic;
using Truss2D.Math;
using static Truss2D.Shell.ConsoleFormat;
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
        private Dictionary<Vertex, Joint> jointMap;

        /// <summary>
        /// Assume that joints are ordered from a-z
        /// </summary>
        /// <param name="vertexs"></param>
        public void PrintReactionsInJoints(IList<Vertex> vertexs)
        {
            for (int i=0; i<vertexs.Count; ++i)
            {
                var v = vertexs[i];
                if (jointMap.ContainsKey(v))
                {

                    // Print all reaction for that joint
                    var reactions = jointMap[v].Reactions;
                    if (jointMap[v].Reactions.Count>0)
                    {
                        Print(($"Joint {(char)('A' + i)}: "));
                        foreach(var reaction in reactions)
                        {
                            Print($"\t [{reaction.X}, {reaction.Y}]");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// For shell application
        /// </summary>
        public decimal? GetInternalForce(Edge edge)
        {
            return internalForces[edge];
        }

        /// <summary>
        /// For shell application
        /// </summary>
        public void ClearJointForce(Vertex v)
        {
            if (!jointMap.ContainsKey(v))
                throw new Exception($"The joint that you're looking for does not exist at location ({v.X.ToString("0.##")}, {v.Y.ToString("0.##")})...");
            jointMap[v].ClearReactions();
        }

        /// <summary>
        /// For shell application
        /// </summary>
        public void ResetvertexCoord(Vertex v, decimal x, decimal y)
        {
            var newvertex = new Vertex(x, y);
            if (jointMap.ContainsKey(newvertex))
                throw new Exception($"The coordinate ({x}, {y}) is already occupied ...");

            Joint oldJoint = jointMap[v];
            jointMap.Remove(v);
            oldJoint.ResetCoordinate(x, y);
            jointMap.Add(newvertex, oldJoint);
        }

        public decimal? MaxInternalForce =>
            internalForces.Values.Max();
        public decimal? MinInternalForce =>
            internalForces.Values.Min();

        public Truss()
        {
            internalForces = new Dictionary<Edge, decimal?>();
            jointMap = new Dictionary<Vertex,Joint>();
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
                internalForces[key] = null;
        }

        public void ClearAllReactions()
        {
            var joints = jointMap.Keys.ToArray();
            foreach (var joint in joints)
                jointMap[joint].ClearReactions();
        }

        /// <summary>
        /// Returns a hashset of solved joints.
        /// Internal forces are reset before solving.
        /// </summary>
        /// <param name="successStatus">returns whether the solving is complete or not</param>
        /// <returns></returns>
        public HashSet<Joint> Solve(out bool successStatus)
        {
            // Arrange
            Reset();
            Solver solver = new Solver(internalForces);
            Queue<Joint> burndown = new Queue<Joint>();
            HashSet<Joint> solved = new HashSet<Joint>();

            // Burndown Cycle
            foreach (var joint in jointMap.Values)
            {
                burndown.Enqueue(joint);
            }

            bool progress=true;
            while (progress && burndown.Count!=0)
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
                    {
                        burndown.Enqueue(joint);
                    }

                    if (numSolved != 0)
                        progress = true;

                }

            }

            // Conclusions
            if (burndown.Count == 0)
                successStatus = true;
            else
                successStatus = false;

            return solved;
        }

        public void AddEdge(Vertex a, Vertex b)
        {
            Edge newEdge = new Edge(a, b);
            if (internalForces.ContainsKey(newEdge))
                throw new Exception("Edge already exists");

            internalForces.Add(newEdge, null);

            Joint jointA = null;
            Joint jointB = null;

            if (jointMap.ContainsKey(a))
                jointA = jointMap[a];
            else
            {
                jointA = new Joint(a);
                jointMap.Add(a, jointA);
            }

            if (jointMap.ContainsKey(b))
                jointB = jointMap[b];
            else
            {
                jointB = new Joint(b);
                jointMap.Add(b, jointB);
            }

            jointA.AddNeighbour(jointB);
            jointB.AddNeighbour(jointA);

         }

        [Obsolete("using a vertex to find force is redundant")]
        public void AddForce(Vertex point, Vector force)
        {
            if (!jointMap.ContainsKey(point))
                throw new Exception("No joint found on the given point ...");

            jointMap[point].AddReaction(force);
        }

    }
}