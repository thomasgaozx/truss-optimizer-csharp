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
            reactions = new List<Vector>();
        }

        public Joint(Vertice v) : this(v.X, v.Y) { }
    }
}
