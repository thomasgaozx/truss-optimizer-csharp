using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Truss2D.Math;

namespace Truss2D.UnitTest.MathUnitTest
{
    [TestFixture]
    public class VectorUnitTest
    {
        [TestCase(3,4,5)]
        [TestCase(6,8,10)]
        [TestCase(5,12,13)]
        [TestCase(7,24,25)]
        public void Vector_GetLength(decimal x, decimal y, decimal expected)
        {
            // Arrange
            Vector vec = new Vector(x, y);

            // Assert
            Assert.That(vec.GetLength(), Is.EqualTo(expected));
        }


    }
}
