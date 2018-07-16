using System;
using System.Collections.Generic;
using Truss2D.Math;

namespace Truss2D
{
    public class Joint : Vertice
    {
        private List<Joint> neighbours; 

        private List<Vector> reactions;

        public IReadOnlyCollection<Joint> Neightbours { get => neighbours.AsReadOnly(); }
        
        // Must not be unsolved, if not solved, throw an exception while adding
        public IReadOnlyCollection<Vector> Reactions { get => reactions; }
        
        public void AddNeighbour(Joint neighbour)
        {
            if (Equals(neighbour))
                throw new Exception("You add the joint itself as neighbour");
            neighbours.Add(neighbour);
        }

        /// <summary>
        /// The reactions added has to be known
        /// </summary>
        /// <param name="force"></param>
        public void AddReaction(Vector force)
        {
            reactions.Add(force);
        }

        public void ClearReactions()
        {
            reactions.Clear();
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public Joint(decimal x, decimal y) : base(x, y)
        {
            neighbours = new List<Joint>();
            reactions = new List<Vector>();
        }

        public Joint(Vertice v) : this(v.X, v.Y) { }
    }
}
