using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Truss2D.Math;

namespace Truss2D.Shell
{
    public class Roller : BaseReaction
    {
        public decimal? Y { get; set; }

        public Roller(Vertex joint) : base(joint)
        {
        }
    }
}
