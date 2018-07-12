using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Truss2D.Math
{
    public class Vertice : Base2D
    {
        public Vertice() : base() { }

        public Vertice(decimal x, decimal y) : base(x, y) { }

        public Vertice(Vertice vertice) : this(vertice.X, vertice.Y) { }

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
