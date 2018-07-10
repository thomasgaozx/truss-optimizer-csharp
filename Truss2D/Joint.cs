using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Truss2D.Math;

namespace Truss2D
{
    public class Joint : Vertice
    {
        private List<Joint> neighbours; // used to get the internal force information from truss ADT

        // ADT: { Edge : IForce }; ==> get magnitude AND direction
        private List<IForce> reactions;

        public List<Joint> Neightbours { get => neighbours; }

        public List<IForce> Reactions { get => reactions; }
        private int knowns;

        private int unknowns;

        public Joint(decimal x, decimal y) : base(x, y)
        {
            neighbours = new List<Joint>();
            reactions = new List<IForce>();
        }
    }
}
