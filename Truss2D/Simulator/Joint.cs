using System;
using System.Collections.Generic;
using Truss2D.Math;

namespace Truss2D
{
    public class Joint : Vertice
    {
        private List<Joint> neighbours; 
        // used to get the internal force information from truss ADT

        // ADT: { Edge : Force }; ==> get magnitude AND direction
        // Actually, unknown force is just null
        private List<Force> reactions;

        public IReadOnlyCollection<Joint> Neightbours { get => neighbours.AsReadOnly(); }
        
        // Must not be unsolved, if not solved, throw an exception while adding
        public IReadOnlyCollection<Force> Reactions { get => reactions; }
        
        public void AddNeighbour(Joint neighbour)
        {
            neighbours.Add(neighbour);
        }

        public void AddReactions(Force force)
        {
            if (force.IsUnknown())
                throw new Exception("Force is unknown");
            reactions.Add(force);
        }

        public override bool Equals(object obj)
        {
            var joint = obj as Joint;
            return joint != null &&
                   base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public Joint(decimal x, decimal y) : base(x, y)
        {
            neighbours = new List<Joint>();
            reactions = new List<Force>();
        }

        public Joint(Vertice v) : this(v.X, v.Y) { }
    }
}
