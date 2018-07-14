﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Truss2D.Math;

namespace Truss2D.Shell
{
    public class TrussBuilder
    {
        private Truss model;

        private List<Vertice> joints;

        public Truss Model => model;

        public int CurrentAlphaPos => joints.Count;

        public Vertice GetJoint(char a)
        {
            int pos = a - 'a';
            if (pos > joints.Count)
                throw new Exception("Bad joint ...");
            return joints[pos];
        }

        public TrussBuilder()
        {
            model = new Truss();
            joints = new List<Vertice>();
        }

        /// <summary>
        /// Create a new truss object.
        /// </summary>
        public void Restart()
        {
            model = new Truss();
            joints.Clear();
        }

        public void AddJoint(Vertice newVertice)
        {
            joints.Add(newVertice);
        }

        /// <summary>
        /// Add a new edge with vertices a and b to the truss.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public void LinkJoints(char a, char b)
        {
            int posA = a - 'a';
            int posB = b - 'a';
            if (posA > joints.Count || posB > joints.Count || posA == posB)
                throw new Exception($"Joint '{a}' or joint '{b}' does not exist, " +
                    $"or illegal connection of joints detected");
            model.AddEdge(joints[posA], joints[posB]);
        }

        /// <summary>
        /// Add a reaction force in a joint of the truss.
        /// </summary>
        /// <param name="joint"></param>
        /// <param name="force"></param>
        /// 
        //[Obsolete("using a vertice to find force is redundant")]
        public void AddForce(Vertice joint, Vector force)
        {
            model.AddForce(joint, force);
        }

        /// <summary>
        /// Returns the success state of the solved truss
        /// </summary>
        /// <returns></returns>
        public bool Render()
        {
            model.Solve(out bool success);
            return success;
        }

    }
}