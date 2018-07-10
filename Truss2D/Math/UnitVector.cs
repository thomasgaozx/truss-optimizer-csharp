using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Truss2D.Math
{
    public class UnitVector : Vector
    {
        public static UnitVector I => new UnitVector(1, 0);
        public static UnitVector J => new UnitVector(0, 1);

        public UnitVector(Vector vector)
        {
            decimal len = X * X + Y * Y;
            X = vector.X / len;
            Y = vector.Y / len;
        }

        private UnitVector(decimal x, decimal y) : base(x, y) { }

    }
}
