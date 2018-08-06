using System;
using System.Collections.Generic;
using Truss2D.Math;
using static Truss2D.Shell.ConsoleFormat;

namespace Truss2D.Shell
{
    public class TrussBuilder
    {

        #region Members and Constructor

        private Truss model;
        private List<Vertex> joints;
        public Truss Model => model;

        public List<Tuple<Vertex, Vector>> knownForces;
        private Pin pin;
        private Roller roller;

        public int CurrentAlphaPos => joints.Count;

        public List<Vertex> Joints => joints;

        public TrussBuilder()
        {
            model = new Truss();
            joints = new List<Vertex>();
            knownForces = new List<Tuple<Vertex, Vector>>();
        }

        #endregion

        #region Console Utility

        public void PrintPinForces()
        {
            Print($"[{pin.X}, {pin.Y}]");
        }

        public void PrintRollerForces()
        {
            Print($"[0, {roller.Y}]");
        }

        public void PrintAllReactions()
        {
            model.PrintReactionsInJoints(joints);
        }

        public void PrintAllJoints()
        {
            for (int i = 0; i < joints.Count; ++i)
            {
                var joint = joints[i];
                Print($"Joint {(char)('A' + i)}: ({joint.X.ToString("0.##")}, {joint.Y.ToString("0.##")})");
            }
        }

        public void ResetVertex(char a, decimal newx, decimal newy)
        {
            int pos = a - 'a';
            if (pos > joints.Count)
                throw new Exception("Bad joint ...");
            model.ResetVertexCoord(GetJoint(a), newx, newy);
            joints[pos].ResetCoordinate(newx, newy);
        }

        public Vertex GetJoint(char a)
        {
            int pos = a - 'a';
            if (pos > joints.Count)
                throw new Exception("Bad joint ...");
            return joints[pos];
        }

        #endregion

        /// <summary>
        /// Create a new truss object.
        /// </summary>
        public void Restart()
        {
            model = new Truss();
            joints.Clear();
            pin = null;
            roller = null;
            knownForces.Clear();
        }

        public void AddJoint(Vertex newvertex)
        {
            joints.Add(newvertex);
        }

        #region support reaction handling

        private void CheckValidityWithBackUp(Pin oldPinJustInCase)
        {
            if (pin != null && roller != null && pin.Joint.X.Equals(roller.Joint.X))
            {
                pin = oldPinJustInCase;
                throw new Exception("Fuck you, bad support alignment ...");
            }
        }

        private void CheckValidityWithBackUp(Roller oldRollerJustInCase)
        {
            if (pin != null && roller != null && pin.Joint.X.Equals(roller.Joint.X))
            {
                roller = oldRollerJustInCase;
                throw new Exception("Fuck you, bad support alignment ...");
            }
        }

        public void AddPin(char joint)
        {
            if (pin != null)
                throw new Exception("There is already a pin ...");
            ChangePin(joint);
        }

        public void ChangePin(char to)
        {
            var oldpin = pin;
            pin = new Pin(GetJoint(to));
            CheckValidityWithBackUp(oldpin);
        }

        public void AddRoller(char joint)
        {
            if (roller != null)
                throw new Exception("There is already a roller");
            ChangeRoller(joint);

        }

        public void ChangeRoller(char to)
        {
            var oldrol = roller;
            roller = new Roller(GetJoint(to));
            CheckValidityWithBackUp(oldrol);
        }

        #endregion

        /// <summary>
        /// Add a new edge with vertexs a and b to the truss.
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
        //[Obsolete("using a vertex to find force is redundant")]
        public void AddForce(Vertex joint, Vector force)
        {
            //model.AddForce(joint, force);
            knownForces.Add(new Tuple<Vertex, Vector>(joint, force));
        }

        private void ClearReactions()
        {
            if (pin != null)
                pin.X = pin.Y = null;
            if (roller != null)
                roller.Y = null;
        }

        private void SolveForReactions()
        {
            if (roller!=null && pin != null && knownForces.Count>0)
            {
                decimal sumMomentAboutPin = 0;
                decimal sumX = 0;
                decimal sumY = 0;

                foreach(var tuple in knownForces)
                {
                    var joint = tuple.Item1;
                    sumX += tuple.Item2.X;
                    sumY += tuple.Item2.Y;
                    sumMomentAboutPin += new Vector(joint.X - pin.Joint.X, joint.Y - pin.Joint.Y).Cross(tuple.Item2);
                }

                pin.X = -sumX;
                roller.Y = sumMomentAboutPin / (pin.Joint.X - roller.Joint.X);
                pin.Y = -sumY - roller.Y;
            }
        }

        /// <summary>
        /// Returns the success state of the solved truss
        /// </summary>
        /// <returns></returns>
        public bool Render()
        {
            ClearReactions();
            model.ClearAllReactions();
            SolveForReactions();

            foreach (var tuple in knownForces)
                model.AddForce(tuple.Item1, tuple.Item2);

            if (pin!=null)
                model.AddForce(pin.Joint, new Vector(pin.X.Value, pin.Y.Value));

            if (roller != null)
                model.AddForce(roller.Joint, new Vector(0, roller.Y.Value));

            model.Solve(out bool success);
            return success;
        }

    }
}
