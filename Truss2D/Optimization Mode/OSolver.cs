using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Truss2D.Math;
using Truss2D.Simulator;

namespace Truss2D.Optimization
{
    public class OSolver
    {
        private Vector knownForce;
        private Dictionary<Member, decimal?> internalForces;
        private List<Tuple<Member, Vector>> unknowns;

        public OSolver(Dictionary<Member, decimal?> internalForces)
        {
            unknowns = new List<Tuple<Member, Vector>>();
            knownForce = new Vector();
            this.internalForces = internalForces;
        }

        public void Clear()
        {
            unknowns.Clear();
            knownForce = new Vector();
        }

        public void JointDecomposition(Joint joint)
        {
            Clear();

            // Add all reactions
            foreach (var reaction in joint.Reactions)
            {
                knownForce.Add(reaction);
            }

            // Add all neighbours
            foreach (var neighbour in joint.Neightbours)
            {
                Member newEdge = new Member(joint, neighbour); // clumsiness to be fixed

                decimal? internalForce = internalForces[newEdge];
                Vector direction = newEdge.DirectionFrom(joint);

                if (internalForce == null)
                    unknowns.Add(new Tuple<Member, Vector>(newEdge, direction));
                else
                {
                    Vector knownInternalForce = new Vector(newEdge.DirectionFrom(joint));
                    knownInternalForce.Scale(internalForce.Value);
                    knownForce.Add(knownInternalForce);
                }
            }

        }

        public const int QuitSignal = -1;

        public int Solve_Optimized(out bool complete)
        {
            Vector negativeKnown = new Vector(knownForce);
            negativeKnown.Scale(-1);
            Matrix matrix = new Matrix(unknowns.Select(edgeTuple => edgeTuple.Item2).ToList(), negativeKnown);
            int rank = matrix.ReduceToRREF();

            complete = true;
            int numSolved = 0;
            for (int i = 0; i < rank; ++i)
            {
                int pos = FetchSolvedPosition(matrix, i);
                if (pos > -1)
                {
                    var solvedInternalForce = matrix[i, matrix.N - 1];

                    if (!OTruss.ForceIsValid(solvedInternalForce))
                        return -1;

                    internalForces[unknowns[pos].Item1] = solvedInternalForce;
                    ++numSolved;
                }
            }

            complete = numSolved == unknowns.Count;
            return numSolved;
        }

        /// <summary>
        /// Might through exception
        /// </summary>
        private static int FetchSolvedPosition(Matrix matrix, int row)
        {
            int state = -1;
            int pos = 0;
            int limit = matrix.N - 2;
            for (; matrix[row, pos] == 0; ++pos)
                if (pos == limit)
                    return -1;
            state = pos++;
            ++limit;
            for (; pos < limit && matrix[row, pos] == 0; ++pos) { }
            return pos == limit ? state : -1;
        }

    }
}
