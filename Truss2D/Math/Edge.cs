using System;
using System.Collections.Generic;

namespace Truss2D.Math
{
    /// <summary>
    /// Order of A, B does not matter
    /// </summary>
    public class Edge
    {
        protected Vertex A { get; private set; }
        protected Vertex B { get; private set; }

        public decimal GetDistance() => (decimal)System.Math.Sqrt((double)((A.X - B.X) * (A.X - B.X) + (A.Y - B.Y) * (A.Y - B.Y)));

        public bool Contains(Vertex vertex) => vertex.Equals(A) || vertex.Equals(B);

        /// <summary>
        /// if v is a, then return a->b direction
        /// otherwise, return b->a direction
        /// returns the unit vector
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public Vector DirectionFrom(Vertex v)
        {
            decimal newX=0;
            decimal newY=0;
            if (A.Equals(v))
            {
                newX = B.X - A.X;
                newY = B.Y - A.Y;
            }
            else if (B.Equals(v))
            {
                newX = A.X - B.X;
                newY = A.Y - B.Y;
            }
            else
                throw new Exception("v is not contained in the edge");
            
            return new Vector(newX, newY).GetUnitVector();
        }

        #region Overridden

        public override bool Equals(object obj)
        {
            var edge = obj as Edge;
            return edge != null && (edge.A.Equals(A) && edge.B.Equals(B))
                ||(edge.A.Equals(B) && edge.B.Equals(A)); // order doesn't matter
        }

        /// <summary>
        /// The order of A, B does not matter
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            var hashCode = -12123;
            hashCode = hashCode * -1521134295 + EqualityComparer<Vertex>.Default.GetHashCode(A) 
                + EqualityComparer<Vertex>.Default.GetHashCode(B);
            return hashCode;
        }

        #endregion

        #region Constructor

        public Edge(Vertex a, Vertex b)
        {
            A = a;
            B = b;
        }

        #endregion
    }
}