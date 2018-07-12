using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Truss2D.Math;

namespace Truss2D
{
    public class Force : IForce
    {
        private decimal? magnitude;
        private UnitVector direction;

        public Force(decimal? magnitude, UnitVector direction)
        {
            this.magnitude = magnitude;
            this.direction = direction;
        }

        public UnitVector GetDirection()
        {
            return direction;
        }

        public decimal? GetMagnitude()
        {
            return magnitude;
        }

        public bool IsUnknown()
        {
            return magnitude==null;
        }
    }
}
