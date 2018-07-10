using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Truss2D.Math;

namespace Truss2D
{
    public interface IForce
    {
        Vector GetDirection();
        decimal GetMagnitude();

    }
}
