﻿namespace Truss2D.Math
{
    public abstract class Base2D
    {
        public decimal X { get; protected set; }
        public decimal Y { get; protected set; }

        public Base2D()
        {
            X = 0;
            Y = 0;
        }
        public Base2D(decimal x, decimal y)
        {
            X = x;
            Y = y;
        }

        #region Override
        
        public override bool Equals(object obj)
        {
            var d = obj as Base2D;
            return d != null &&
                   X == d.X &&
                   Y == d.Y;
        }

        public override int GetHashCode()
        {
            var hashCode = 1861411795;
            hashCode = hashCode * -1521134295 + X.GetHashCode();
            hashCode = hashCode * -1521134295 + Y.GetHashCode();
            return hashCode;
        }

        #endregion
    }
}
