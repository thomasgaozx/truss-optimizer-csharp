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

        [TestCase(1,1,0.70711,0.70711)]
        [TestCase(2,3,0.55471,0.83206)]
        public void Vector_UnitVector(decimal x, decimal y, decimal u_x, decimal u_y)
        {
            // Arrange
            Vector vec = new Vector(x, y);

            // Act
            Vector u = vec.GetUnitVector();

            // Assert
            Assert.That(u.X, Is.EqualTo(u_x).Within(0.01));
            Assert.That(u.Y, Is.EqualTo(u_y).Within(0.01));
        }

        [TestCase(1,1,1,1,2,2)]
        [TestCase(1.1,2.2,3.3,4.4,4.4,6.6)]
        public void Vector_Add(decimal x1, decimal y1, 
            decimal x2, decimal y2, decimal e_x, decimal e_y)
        {
            // Arrange
            Vector v1 = new Vector(x1, y1);
            Vector v2 = new Vector(x2, y2);

            // Act
            v1.Add(v2);

            // Assert
            Assert.That(v1.X, Is.EqualTo(e_x));
            Assert.That(v1.Y, Is.EqualTo(e_y));
        }

        [TestCase(1,1,2,2,2)]
        [TestCase(1.33, 2.11, 3, 3.99, 6.33)]
        public void Vector_Scale(decimal x, decimal y,
            decimal scale, decimal e_x, decimal e_y)
        {
            // Arrange
            Vector vec = new Vector(x, y);

            // Act
            vec.Scale(scale);

            // Assert
            Assert.That(vec.X, Is.EqualTo(e_x));
            Assert.That(vec.Y, Is.EqualTo(e_y));
        }

        [TestCase(1,0,0,1,0)]
        [TestCase(1,0,5,0,5)]
        [TestCase(1,1,2,0,2)]
        public void Vector_Dot(decimal x1, decimal y1,
            decimal x2, decimal y2, decimal expected)
        {
            // Arrange
            Vector v1 = new Vector(x1, y1);
            Vector v2 = new Vector(x2, y2);

            // Act
            decimal result = v1.Dot(v2);

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [TestCase(1,0,0,1,1)]
        [TestCase(1,1,0,1,1)]
        [TestCase(1,0,1, 1.73205, 1.73205)]
        public void Vector_Cross(decimal x1, decimal y1,
            decimal x2, decimal y2, decimal expected)
        {
            // Arrange
            Vector v1 = new Vector(x1, y1);
            Vector v2 = new Vector(x2, y2);

            // Act
            decimal result = v1.Cross(v2);

            // Assert
            Assert.That(result, Is.EqualTo(expected).Within(0.01));
        }

        [TestCase(1,1234,4,0,1)]
        [TestCase(2,99,0,1,99)]
        [TestCase(1,0,1,1,0.7071)]
        public void Vector_ProjectionOn(decimal x1, decimal y1,
            decimal xp, decimal yp, decimal expected)
        {
            // Arrange
            Vector v1 = new Vector(x1, y1);
            Vector v2 = new Vector(xp, yp);

            // Act
            decimal result = v1.ProjectionOn(v2);

            // Assert
            Assert.That(result, Is.EqualTo(expected).Within(0.01));
        }
    }
}
