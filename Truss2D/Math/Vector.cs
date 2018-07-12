using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Truss2D.Math
{
    public class Vector : Base2D
    {
        public decimal GetLength()
        {
            return X * X + Y * Y;
        }

        public UnitVector GetUnitVector()
        {
            return new UnitVector(this);
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
