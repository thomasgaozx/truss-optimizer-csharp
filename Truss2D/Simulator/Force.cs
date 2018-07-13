using System;
using Truss2D.Math;

namespace Truss2D
{
    public class Force
    {
        public decimal? Magnitude { get; set; }
        public UnitVector Direction { get; private set; }

        public Force(decimal? magnitude, UnitVector direction)
        {
            Magnitude = magnitude;
            Direction = direction;
        }

        public Vector ToVector()
        {
            if (IsUnknown())
                throw new Exception("Cannot get the magnitude of unknown force");

            Vector unitVector = new Vector(Direction);
            unitVector.Scale(Magnitude.Value);

            return unitVector;
        }

        public bool IsUnknown()
        {
            return Magnitude==null;
        }
    }
}
