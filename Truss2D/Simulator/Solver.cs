using System;
using System.Collections.Generic;
using System.Linq;
using Truss2D.Math;

namespace Truss2D
{
    /// <summary>
    /// Capable of possibly solving 1 joint.
    /// </summary>
    public class Solver
    {
        // push all the force component to it.
        private List<Tuple<Edge,Vector>> unknowns;
        private Vector knownForce;
        private Dictionary<Edge, decimal?> internalForces;

        public Solver(Dictionary<Edge, decimal?> internalForces)
        {
            unknowns = new List<Tuple<Edge, Vector>>();
            knownForce = new Vector();
            this.internalForces = internalForces;
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
                Edge newEdge = new Edge(new Vertice(joint), new Vertice(neighbour)); // clumsiness to be fixed

                decimal? internalForce = internalForces[newEdge];
                Vector direction = newEdge.DirectionFrom(joint);

                if (internalForce == null)
                    unknowns.Add(new Tuple<Edge, Vector>(newEdge,direction));
                else
                {
                    Vector knownInternalForce = new Vector(newEdge.DirectionFrom(joint));
                    knownInternalForce.Scale(internalForce.Value);
                    knownForce.Add(knownInternalForce);
                }
            }

        }

        public void Clear()
        {
            unknowns.Clear();
            knownForce = new Vector();
        }

        /// <summary>
        /// returns number of joints solved,
        /// false otherwise
        /// </summary>
        /// <returns></returns>
        public int Solve(out bool complete)
        {
            Vector negativeKnown = new Vector(knownForce);
            negativeKnown.Scale(-1);
            Matrix matrix = new Matrix(unknowns.Select(edgeTuple => edgeTuple.Item2).ToList(), negativeKnown);
            int rank= matrix.ReduceToRREF();

            complete = true;
            int numSolved = 0;
            for (int i=0; i<rank; ++i)
            {
                int pos = FetchSolvedPosition(matrix, i);
                if (pos > -1)
                {
                    internalForces[unknowns[pos].Item1] = matrix[i, matrix.N - 1];
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
            return pos==limit? state:-1;
        }

    }
}
