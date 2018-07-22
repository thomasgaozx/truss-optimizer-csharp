
using System.Collections.Generic;
using System.Linq;
using Truss2D.Math;
using Truss2D.Simulator;

/// <summary>
/// Assume that the force and support does not change and acts
/// on immutable joints
/// </summary>
namespace Truss2D.Optimization
{
    public class OTruss
    {
        List<Joint> joints;
        List<char> freeJoints;
        private Dictionary<Member, decimal?> members;

        private const decimal JointCost = 5;
        private const decimal CostPerMeter = 10;
        private const decimal MinimumMemberLength = 3;
        private const decimal MinForce = -9;
        private const decimal MaxForce = 12;

        public static bool ForceIsValid(decimal force) => !(force < MinForce || force > MaxForce);

        public static bool MemberLengthIsValid(decimal memberLength) => memberLength >= MinimumMemberLength;

        public Joint GetJoint(char a) => joints[a - 'a'];

        public void ClearMembers()
        {
            var edges = members.Keys.ToArray();
            foreach (var edge in edges)
                members[edge] = null;
        }

        public decimal GetPrice()
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
