using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Truss2D.Math;

namespace Truss2D
{
    /// <summary>
    /// Capable of possibly solving 1 joint.
    /// </summary>
    public class Solver
    {
        // push all the force component to it.
        private List<Force> unknowns;
        private Vector knownForce;

        public void Clear()
        {
            unknowns.Clear();
            knownForce = new Vector();
        }

        public Solver()
        {
            unknowns = new List<Force>();
            knownForce = new Vector();
        }

        public void AddForce(Force force)
        {
            if (force.IsUnknown())
            {
                unknowns.Add(force);
                return;
            }

            knownForce.Add(force.ToVector());
        }

        /// <summary>
        /// returns true if the joint is completely solved,
        /// false otherwise
        /// </summary>
        /// <returns></returns>
        public bool Solve()
        {
            Vector negativeKnown = new Vector(knownForce);
            negativeKnown.Scale(-1);
            Matrix matrix = new Matrix(unknowns.Select(f => f.Direction as Vector).ToList(), negativeKnown);
            int rank= matrix.ReduceToRREF();

            bool solved = true;
            for (int i=0; i<rank; ++i)
            {
                int pos = FetchSolvedPosition(matrix, i);
                if (pos > -1)
                    unknowns[pos].Magnitude = matrix[i, matrix.N - 1];
                else
                    solved = false;
            }
            return solved;
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
            for (; pos < limit && matrix[row, pos] == 0; ++pos) { }
            return pos==limit? state:-1;
        }

    }
}
