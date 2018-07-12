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
        private List<Joint> neighbours; 
        // used to get the internal force information from truss ADT

        // ADT: { Edge : IForce }; ==> get magnitude AND direction
        // Actually, unknown force is just null
        private List<IForce> reactions;

        public IReadOnlyCollection<Joint> Neightbours { get => neighbours.AsReadOnly(); }
        
        // Must not be unsolved, if not solved, throw an exception while adding
        public IReadOnlyCollection<IForce> Reactions { get => reactions; }
        
        public void AddNeighbour(Joint neighbour)
        {
            neighbours.Add(neighbour);
            Unknowns++;
        }

        public void AddReactions(Force force)
        {
            if (force.IsUnknown())
                throw new Exception("Force is unknown");
            reactions.Add(force);
            ++Knowns;
        }

        public int Knowns { get; private set; }
        public int Unknowns { get; private set; }

        public Joint(decimal x, decimal y) : base(x, y)
        {
            neighbours = new List<Joint>();
            reactions = new List<IForce>();

            Knowns = 0;
            Unknowns = 0;
        }
    }
}
