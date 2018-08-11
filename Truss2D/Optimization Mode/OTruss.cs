using static Truss2D.Shell.ConsoleFormat;
using System.Collections.Generic;
using System.Linq;
using Truss2D;
using Truss2D.Math;
using Truss2D.Shell;
using Truss2D.Simulator;
using Newtonsoft.Json;
using System.IO;

/// <summary>
/// Assume that the force and support does not change and acts
/// on immutable joints
/// </summary>
namespace Truss2D.Optimization
{
    #region Helpers

    static class OptimizationGeometry
    {

        public static decimal[,] GetShiftGeometry(int numOfCorners)
        {
            decimal[,] collection = new decimal[numOfCorners+1, 2];
            collection[0, 0] = collection[0,1] = 0;

            double deltaTheta = 2*System.Math.PI / numOfCorners;
            double theta = 0;

            for (int i=0; i < numOfCorners; theta+=deltaTheta)
            {
                ++i;
                collection[i, 0] = (decimal)System.Math.Cos(theta);
                collection[i, 1] = (decimal)System.Math.Sin(theta);
            }
            return collection;
        }

        public static decimal[,] Grid => new decimal[9, 2]
        {
            { 0, 0 },
            { 0, 1 },
            { 1, 1},
            { 1, 0 },
            { -1, 1},
            { -1, 0},
            { -1, -1 },
            { 0, -1 },
            { 1, -1 }
        };

    }

    struct FreeJoint
    {
        public char name;
        public decimal x;
        public decimal y;

        public FreeJoint(char name, decimal x, decimal y)
        {
            this.name = name;
            this.x = x;
            this.y = y;
        }

        public FreeJoint(FreeJoint other) : this(other.name, other.x, other.y) { }
    }

    public enum ZoomExtent { None, Min, Medium, Max };

    #endregion

    public class OTruss
    {
        private static readonly ConstraintValues constraints // intended as a singleton
            = JsonConvert.DeserializeObject<ConstraintValues>(
                File.ReadAllText("Constraints.json")); 

        private static readonly decimal JointCost = constraints.JointCost;
        private static readonly decimal CostPerMeter = constraints.MemberCostPerMeter;
        private static readonly decimal MinimumMemberLength = constraints.MinimumLength;
        private static readonly decimal MinForce = -1*constraints.MaximumMemberCompression;
        private static readonly decimal MaxForce = constraints.MaximumMemberTension;


        private List<Joint> joints;
        private List<char> freeJoints;
        private Dictionary<Member, decimal?> members;

        public OTruss()
        {
            joints = new List<Joint>();
            freeJoints = new List<char>();
            members = new Dictionary<Member, decimal?>();
        }

        public void PrintJoints()
        {
            for (int i = 0; i < joints.Count; ++i)
            {
                var joint = joints[i];
                PrintWarning($"Joint {(char)('A' + i)}: ({joint.X.ToString("0.####")}, {joint.Y.ToString("0.####")})");
            }
        }

        public void AddJoint(Joint joint, bool free = false)
        {
            if (free)
                freeJoints.Add((char)('a' + joints.Count));
            joints.Add(joint);
        }

        public void AddEdge(char a, char b)
        {
            var jointA = GetJoint(a);
            var jointB = GetJoint(b);

            members.Add(new Member(jointA, jointB), null);
            jointA.AddNeighbour(jointB);
            jointB.AddNeighbour(jointA);
        }

        public void AddForce(char a, decimal x, decimal y)
        {
            GetJoint(a).AddReaction(new Vector(x, y));
        }        



        public int NumOfJoints => joints.Count;

        public static bool ForceIsValid(decimal force) => !(force < MinForce || force > MaxForce);

        public static bool MemberLengthIsValid(decimal memberLength) => memberLength >= MinimumMemberLength;

        public Joint GetJoint(char a) => joints[a - 'a'];

        private void ScaleCycle(decimal[,] cycle, decimal scale)
        {
            int cycleLength = cycle.Length / 2;
            for (int i = 0; i < cycleLength; ++i)
            {
                cycle[i, 0] *= scale;
                cycle[i, 1] *= scale;
            }
        }

        public void BaseOptimization(decimal[,] cycle)
        {
            if (!(MemberWorks() && Pass()))
                throw new System.Exception("Initial Condition is Not Met");
            for (int i = 0; i < 200 && Iteration(cycle); ++i)
            {
                PrintWarning($"Iteration {i} ... ");
            }
        }

        public void Optimize(int geometryNum, decimal scale)
        {
            var cycle = OptimizationGeometry.GetShiftGeometry(geometryNum);

            int cyclen = cycle.Length / 2;
            for (int i=0; i<cyclen; ++i)
            {
                cycle[i, 0] *= scale;
                cycle[i, 1] *= scale;
            }

            BaseOptimization(cycle);
        }

        public void GridOptimize(decimal scale)
        {
            var cycle = OptimizationGeometry.Grid;
            
            int cyclen = cycle.Length / 2;
            for (int i = 0; i < cyclen; ++i)
            {
                cycle[i, 0] *= scale;
                cycle[i, 1] *= scale;
            }

            BaseOptimization(cycle);
        }

        /// <summary>
        /// Returns whether progress has been made in the current iteration
        /// Note that cycle should already be scaled
        /// Assume cycle.Rank == 2
        /// </summary>
        /// <returns></returns>
        private bool Iteration(decimal[,] cycle)
        {
            decimal minCost = GetCost();
            bool progress = false;
            int cycleLength = cycle.Length / 2;
            Vertex[] bestCombination = new Vertex[freeJoints.Count];
            Vertex[] currentCombination = new Vertex[freeJoints.Count];
            int[] currentState = new int[freeJoints.Count];

            for (int i=0; i<freeJoints.Count; ++i)
            {
                var cur = GetJoint(freeJoints[i]);
                bestCombination[i] = new Vertex(cur.X, cur.Y);
                currentCombination[i] = new Vertex(cur.X, cur.Y);
            }

            int total = (int)System.Math.Pow(cycleLength, freeJoints.Count);

            for (int i=1; i<total; ++i)
            {
                int pos = i;
                
                for (int step =0; step<freeJoints.Count; ++step)
                {
                    int posDivCycLen = pos / cycleLength;
                    int remainder = pos - posDivCycLen * cycleLength;
                    pos = posDivCycLen;

                    if (currentState[step] !=remainder)
                    {
                        currentState[step] = remainder;
                        Vertex node = currentCombination[step];
                        Joint joint = GetJoint(freeJoints[step]);
                        joint.ResetCoordinate(node.X, node.Y);
                        joint.Shift(cycle[remainder, 0], cycle[remainder, 1]);
                    }

                }

                decimal money = GetCost();

                if (MemberWorks() && Pass() && money < minCost)
                {
                    progress = true;
                    for (int j = 0; j < freeJoints.Count; ++j)
                    {
                        var freejoint = GetJoint(freeJoints[j]);
                        bestCombination[j].ResetCoordinate(freejoint.X, freejoint.Y);
                    }
                    minCost = money;
                }
            }


            for (int i=0; i<freeJoints.Count; ++i)
            {
                Vertex v = bestCombination[i];
                GetJoint(freeJoints[i]).ResetCoordinate(v.X, v.Y);
            }


            if (progress)
            {
                PrintWarning($"Cost: {minCost}");
                return true;
            }
            
            return false;
        }

        public void ClearMembers()
        {
            var edges = members.Keys.ToArray();
            members.Clear();
            foreach (var edge in edges)
                members[edge] = null;
        }

        public decimal GetCost()
        {
            decimal dist = 0;
            var edges = members.Keys.ToArray();
            foreach (Member edge in edges)
                dist += edge.GetDistance();

            return dist * CostPerMeter + joints.Count * JointCost;
        }

        /// <summary>
        /// Checks member length to make sure it doesn't
        /// break the rules
        /// </summary>
        /// <returns></returns>
        public bool MemberWorks()
        {
            var edges = members.Keys.ToArray();
            foreach (var edge in edges)
            {
                if (edge.GetDistance()<MinimumMemberLength)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// True means the truss passed.
        /// False means the truss failed.
        /// </summary>
        /// <returns></returns>
        public bool Pass()
        {
            // Arrange
            ClearMembers();
            OSolver solver = new OSolver(members);
            Queue<Joint> burndown = new Queue<Joint>();

            // Burndown Cycle
            foreach (var joint in joints)
                burndown.Enqueue(joint);

            bool progress = true;
            while (progress && burndown.Count != 0)
            {
                int cycleLength = burndown.Count;
                progress = false;
                for (int i = 0; i < cycleLength; ++i)
                {
                    Joint joint = burndown.Dequeue();
                    solver.JointDecomposition(joint);
                    int numSolved = solver.Solve_Optimized(out bool complete);

                    if (numSolved == Solver.QuitSignal)
                        return false;

                    if (!complete)
                        burndown.Enqueue(joint);

                    if (numSolved != 0)
                        progress = true;
                }
            }

            if (burndown.Count != 0)
            {
                PrintDanger("One unsolvable case encountered ...");
            }

            return true;
        }

    }
}
