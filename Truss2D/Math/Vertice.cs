namespace Truss2D.Math
{
    public class Vertex : Base2D
    {
        public Vertex() : base() { }

        public Vertex(decimal x, decimal y) : base(x, y) { }

        public Vertex(Vertex vertex) : this(vertex.X, vertex.Y) { }

        public void ResetCoordinate(decimal x, decimal y)
        {
            X = x;
            Y = y;
        }

        public void Shift (decimal x, decimal y)
        {
            X += x;
            Y += y;
        }
        #region Overridden

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return base.ToString();
        }
        
        #endregion

        /// <summary>
        /// Does not perform null checking
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static decimal GetDist(Vertex a, Vertex b)
        {
            return (a.X - b.X) * (a.X - b.X)
                + (a.Y - b.Y) * (a.Y - b.Y);
        }

    }
}
