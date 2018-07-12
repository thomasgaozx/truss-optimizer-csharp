using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Truss2D.Heap;
using Truss2D.Math;

namespace Truss2D
{
    public abstract class Truss
    {
        /// <summary>
        /// Flow:
        /// 1. Building the shape of the trusses using add edge
        /// 2. Add external forces
        /// 3. Render
        /// 4. Solve
        /// </summary>
        public Truss()
        {
            internalForces = new Dictionary<Edge, IForce>();
            joints = new MinHeap<Joint>((a, b) => a.Unknowns.CompareTo(b.Unknowns));
            jointStash = new Stack<Joint>();
        }

        private Dictionary<Edge, IForce> internalForces; // data base

        private MinHeap<Joint> joints;
        // when a edge is solved, updates the vertices of both the joint
        // and the, then 'pull' forward the updated vertice

        private Stack<Joint> jointStash = new Stack<Joint>();

        /// <summary>
        /// Putting joints from the stack to the mean heap
        /// </summary>
        public abstract void Render();

        /// <summary>
        /// Throw exception if not rendered
        /// </summary>
        public abstract void Solve();

        public abstract void AddForce(Vertice joint, Force force);

        public abstract void AddEdge(Vertice a, Vertice b);

    }
}