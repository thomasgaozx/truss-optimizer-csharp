using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Truss2D.Optimization
{
    public class CharEdge
    {
        public char A { get; private set; }
        public char B { get; private set; }
        
        public CharEdge(char a, char b)
        {
            A = a;
            B = b;
        }

        public override bool Equals(object obj)
        {
            var edge = obj as CharEdge;
            return edge != null &&
                   A == edge.A && B == edge.B;
        }

    }
}
