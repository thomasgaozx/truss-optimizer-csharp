using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Truss2D.Math;

namespace Truss2D.Shell
{
    
    public abstract class BaseReaction
    { 
        public Vertex Joint { get; private set; }

        public BaseReaction(Vertex joint)
        {
            Joint = joint;
        }

    }
}
