namespace Truss2D.Math
{
    public class Vector : Base2D
    {
        public bool IsUnitVector()
        {
            return System.Math.Abs(GetLength() - 1) < (decimal)10E-9;
        }

        public decimal GetLength()
        {
            return (decimal)System.Math.Sqrt((double)(X * X + Y * Y));
        }

        public Vector GetUnitVector()
        {
            decimal len = GetLength();
            return new Vector(X/len, Y/len);
        }

        public void Add(Vector dir)
        {
            X += dir.X;
            Y += dir.Y;
        }

        public void Scale(decimal constant)
        {
            X *= constant;
            Y *= constant;
        }

        public decimal Dot(Vector dir)
        {
            return X * dir.X + Y * dir.Y;
        }

        public decimal Cross(Vector dir)
        {
            return X * dir.Y - Y * dir.X;
        }

        public decimal ProjectionOn(Vector dir)
        {
            return Dot(dir.GetUnitVector());
        }

        public Vector() : base() { }

        public Vector(decimal x, decimal y) : base(x, y) { }

        public Vector(Vector another) : this(another.X, another.Y) { }

    }
}
