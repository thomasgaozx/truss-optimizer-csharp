using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Truss2D.Math
{
    // Order of A, B does not matter
    public class Edge
    {
        protected Vertice A { get; private set; }
        protected Vertice B { get; private set; }

        public bool Contains(Vertice vertice)
        {
            return vertice == A || vertice == B;
        }

        #region Overridden

        public override bool Equals(object obj)
        {
            var edge = obj as Edge;
            return edge != null && (edge.A==A && edge.B==B)||(edge.A==B && edge.B==A); // order doesn't matter
        }

        /// <summary>
        /// The order of A, B does not matter
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            var hashCode = -123;
            hashCode = hashCode * -1521134295 + EqualityComparer<Vertice>.Default.GetHashCode(A) 
                + EqualityComparer<Vertice>.Default.GetHashCode(B);
            return hashCode;
        }

        #endregion

        #region Constructor

        public Edge(Vertice a, Vertice b)
        {
            A = a;
            B = b;
        }

        #endregion
    }
}
