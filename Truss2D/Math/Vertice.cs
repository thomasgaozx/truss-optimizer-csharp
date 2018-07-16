namespace Truss2D.Math
{
    public class Vertice : Base2D
    {
        public Vertice() : base() { }

        public Vertice(decimal x, decimal y) : base(x, y) { }

        public Vertice(Vertice vertice) : this(vertice.X, vertice.Y) { }

        public void ResetCoordinate(decimal x, decimal y)
        {
            X = x;
            Y = y;
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

    }
}
