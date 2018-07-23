using static Truss2D.Shell.ConsoleFormat;
using System.Collections.Generic;
using System.Linq;
using Truss2D;
using Truss2D.Math;
using Truss2D.Shell;
using Truss2D.Simulator;

/// <summary>
/// Assume that the force and support does not change and acts
/// on immutable joints
/// </summary>
namespace Truss2D.Optimization
{
    #region Helpers

    static class OptimizationGeometry
    {
        private static readonly decimal sqrt3 = (decimal)System.Math.Sqrt(3);
        private static readonly decimal sqrt2 = (decimal)System.Math.Sqrt(2);

        public const decimal Level1 = 1;
        public const decimal Level2 = 0.5M;
        public const decimal Level3 = 0.2M;
        public const decimal Level4 = 0.1M;
        public const decimal Level5 = 0.05M;
        public const decimal Level6 = 0.02M;
        public const decimal Level7 = 0.01M;

        public static decimal[,] Triangle => new decimal[4, 2] 
        { 
            { 0, 0 }, 
            { 0, 1 }, 
            { sqrt3 / 2, -1.5M }, 
            { -sqrt3, 0 }
        };

        public static decimal[,] Cross => new decimal[5, 2]
        {
            { 0, 0 },
            { 0, 1 }, 
            { sqrt2, -sqrt2 }, 
            { -sqrt2, -sqrt2 }, 
            { -sqrt2, sqrt2 }
        };

        public static decimal[,] Hexagon => new decimal[7, 2] 
        { 
            { 0, 0 }, 
            { 0, 1 }, 
            { sqrt3 / 2, -0.5M }, 
            { 0, -1 }, 
            { -sqrt3 / 2, -0.5M }, 
            { -sqrt3 / 2, 0.5M }, 
            { 0, 1 }
        };

        public static decimal[,] Octagon => new decimal[9, 2] 
        { 
            { 0, 0 }, 
            { 0, 1 }, 
            { sqrt2 / 2, sqrt2 / 2 - 1 }, 
            { 1 - sqrt2 / 2, -sqrt2 / 2 }, 
            { sqrt2 / 2 - 1, -sqrt2 / 2 }, 
            { -sqrt2 / 2, sqrt2 / 2 - 1 }, 
            { -sqrt2 / 2, 1 - sqrt2 / 2 }, 
            { sqrt2 / 2 - 1, sqrt2 / 2 }, 
            { 1 - sqrt2 / 2, sqrt2 / 2 }
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

        private const decimal JointCost = 5;
        private const decimal CostPerMeter = 10;
        private const decimal MinimumMemberLength = 3;
        private const decimal MinForce = -9;
        private const decimal MaxForce = 12;

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

        public void Level1Optimize(decimal[,] cycle)
        {
            if (!(MemberWorks() && Pass()))
                throw new System.Exception("Initial Condition is Not Met");
            for (int i = 0; i < 200 && Iteration(cycle); ++i)
            {
                PrintWarning($"Iteration {i} ... ");
            }
        }

        public void Level2Optimize(decimal[,] cycle)
        {
            ScaleCycle(cycle, OptimizationGeometry.Level2);
            Level1Optimize(cycle);
        }

        public void Level3Optimize(decimal[,] cycle)
        {
            ScaleCycle(cycle, OptimizationGeometry.Level3);
            Level1Optimize(cycle);
        }

        public void Level4Optimize(decimal[,] cycle)
        {
            ScaleCycle(cycle, OptimizationGeometry.Level4);
            Level1Optimize(cycle);
        }

        public void Level5Optimize(decimal[,] cycle)
        {
            ScaleCycle(cycle, OptimizationGeometry.Level5);
            Level1Optimize(cycle);
        }

        public void Level6Optimize(decimal[,] cycle)
        {
            ScaleCycle(cycle, OptimizationGeometry.Level6);
            Level1Optimize(cycle);
        }

        public void Level7Optimize(decimal[,] cycle)
        {
            ScaleCycle(cycle, OptimizationGeometry.Level7);
            Level1Optimize(cycle);
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

            for (int i=0; i<freeJoints.Count; ++i)
            {
                var cur = GetJoint(freeJoints[i]);
                bestCombination[i] = new Vertex(cur.X, cur.Y);
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

                    GetJoint(freeJoints[step]).Shift(cycle[remainder,0], cycle[remainder,1]);
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
                throw new System.Exception("Fucking unsolvable ..");

            return true;
        }

    }
}
