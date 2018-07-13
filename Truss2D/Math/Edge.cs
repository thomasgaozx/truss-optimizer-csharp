using System;
using System.Collections.Generic;

namespace Truss2D.Math
{
    /// <summary>
    /// Order of A, B does not matter
    /// </summary>
    public class Edge
    {
        protected Vertice A { get; private set; }
        protected Vertice B { get; private set; }

        public bool Contains(Vertice vertice)
        {
            return vertice.Equals(A) || vertice.Equals(B);
        }

        /// <summary>
        /// if v is a, then return a->b direction
        /// otherwise, return b->a direction
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public UnitVector DirectionFrom(Vertice v)
        {
            decimal newX=0;
            decimal newY=0;
            if (v.Equals(A))
            {
                newX = B.X - A.X;
                newY = B.Y - A.Y;
            }
            else if (v.Equals(B))
            {
                newX = A.X - B.X;
                newY = A.Y - B.Y;
            }
            else
                throw new Exception("v is not contained in the edge");
            
            return new UnitVector(new Vector(newX, newY));
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
            var hashCode = -12123;
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
