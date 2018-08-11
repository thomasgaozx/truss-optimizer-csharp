using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Truss2D
{
    struct ConstraintValues
    {
        public decimal JointCost;
        public decimal MemberCostPerMeter;
        public decimal MinimumLength;
        public decimal MaximumMemberTension;
        public decimal MaximumMemberCompression;
    }
}
