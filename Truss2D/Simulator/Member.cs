using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Truss2D.Math;

namespace Truss2D.Simulator
{
    public class Member
    {

        protected Joint A { get; private set; }
        protected Joint B { get; private set; }

        public decimal GetDistance() => (decimal)System.Math.Sqrt((double)((A.X - B.X) * (A.X - B.X) + (A.Y - B.Y) * (A.Y - B.Y)));

        public bool Contains(Joint joint) => joint.Equals(A) || joint.Equals(B);

        /// <summary>
        /// if v is a, then return a->b direction
        /// otherwise, return b->a direction
        /// returns the unit vector
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public Vector DirectionFrom(Vertex v)
        {
            decimal newX = 0;
            decimal newY = 0;
            if (A.Equals(v))
            {
                newX = B.X - A.X;
                newY = B.Y - A.Y;
            }
            else if (B.Equals(v))
            {
                newX = A.X - B.X;
                newY = A.Y - B.Y;
            }
            else
                throw new Exception("v is not contained in the edge");

            return new Vector(newX, newY).GetUnitVector();

        }

        #region Overridden

        public override bool Equals(object obj)
        {
            var member = obj as Member;
            return member != null &&
                   (A.Equals(member.A) && B.Equals(member.B)) || (B.Equals(member.A) && A.Equals(member.B));
        }

        public override int GetHashCode()
        {
            var hashCode = -1817952719;
            hashCode = hashCode * -1521134295 + EqualityComparer<Joint>.Default.GetHashCode(A) + EqualityComparer<Joint>.Default.GetHashCode(B);
            return hashCode;
        }

        #endregion

        #region Constructor

        public Member(Joint a, Joint b)
        {
            A = a;
            B = b;
        }

        #endregion


    }
}