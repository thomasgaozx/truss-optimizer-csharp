using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Truss2D.Math;

namespace Truss2D.Shell
{
    public class Pin : BaseReaction
    {
        public decimal? X { get; set; }
        public decimal? Y { get; set; }

        public Pin(Vertex joint) : base(joint)
        {
        }
    }
}
