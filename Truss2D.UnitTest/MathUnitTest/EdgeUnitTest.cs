using NUnit.Framework;
using System.Collections.Generic;
using Truss2D.Math;

namespace Truss2D.UnitTest.MathUnitTest
{
    [TestFixture]
    public class EdgeUnitTest
    {
        const decimal Val1 = (decimal)12.99;
        const decimal Val2 = (decimal)4.1111;
        const decimal Val3 = (decimal)5.982738472;
        const decimal Val4 = (decimal)9.87654321;
        const decimal Val5 = (decimal)101.324;
        const decimal Val6 = (decimal)7.1;

        public Vertice A = new Vertice(Val1, Val2);
        public Vertice B = new Vertice(Val1, Val3);
        public Vertice C = new Vertice(Val3, Val2);
        public Vertice D = new Vertice(Val2, Val2);
        public Vertice E = new Vertice(Val4, Val5);
        public Vertice F = new Vertice(Val3, Val6);
        public Vertice G = new Vertice(Val5, Val2);
        public Vertice H = new Vertice(Val6, Val1);

        public Edge AB;
        public Edge BA;
        public Edge AC;
        public Edge CA;
        public Edge AD;
        public Edge EF;
        public Edge GH;
        public Edge EH;
        public Edge FH;

        [SetUp] 
        public void SetUpTest()
        {
            AB = new Edge(A,B);
            BA = new Edge(B,A);
            AC = new Edge(A,C);
            CA = new Edge(C,A);
            AD = new Edge(A,D);
            EF = new Edge(E,F);
            GH = new Edge(G,H);
            EH = new Edge(E,H);
            FH = new Edge(F,H);
        }

        [TearDown]
        public void TearDownTest()
        {

        }

        [Test]
        public void Edge_Contains()
        {
            Assert.IsTrue(AB.Contains(A));
            Assert.IsTrue(AB.Contains(B));
            Assert.IsTrue(BA.Contains(A));
            Assert.IsTrue(BA.Contains(B));
            Assert.IsTrue(FH.Contains(F));
            Assert.IsTrue(FH.Contains(H));
        }

        [Test]
        public void Edge_DirectionFrom_HardCoded()
        {
            var newEdge = new Edge(new Vertice(), new Vertice(2, 2));
            Vector u = newEdge.DirectionFrom(new Vertice());
            Assert.That((double)u.X, Is.EqualTo(System.Math.Sqrt(2) / 2.0).Within(0.01));
            Assert.That(u.Y, Is.EqualTo(u.X).Within( (decimal)0.01 ));
        }

        [Test]
        public void Edge_DirectionFrom_AToB()
        {
            var expectedUnitVector = new Vector(B.X - A.X, B.Y - A.Y).GetUnitVector();
            Assert.That(AB.DirectionFrom(A),
                Is.EqualTo(expectedUnitVector));
            Assert.That(BA.DirectionFrom(A),
                Is.EqualTo(expectedUnitVector));
        }
        

        [Test]
        public void Edge_DirectionFrom_HToG()
        {
            var expectedUnitVector = new Vector(G.X - H.X, G.Y - H.Y).GetUnitVector();
            Assert.That(GH.DirectionFrom(H),
                Is.EqualTo(expectedUnitVector));
        }

        [Test]
        public void Edge_Equals()
        {
            Assert.IsTrue(AB.Equals(BA));
            Assert.IsTrue(AC.Equals(CA));
            Assert.IsFalse(AB.Equals(AD));
            Assert.IsFalse(GH.Equals(FH));
        }

        [Test]
        public void Edge_SameHashCode()
        {
            Assert.IsTrue(AB.GetHashCode().Equals(BA.GetHashCode()));
            Assert.IsTrue(AB.GetHashCode().Equals(new Edge(A, B).GetHashCode()));
            Assert.IsTrue(AB.GetHashCode().Equals(new Edge(B,A).GetHashCode()));
            Assert.IsTrue(AC.GetHashCode().Equals(CA.GetHashCode()));
            Assert.IsTrue(AC.GetHashCode().Equals(new Edge(C,A).GetHashCode()));
        }

        [Test]
        public void Edge_HashTable()
        {
            // Arrange
            Dictionary<Edge, int> dict = new Dictionary<Edge, int>();
            const int ab_val = 1;
            const int ac_val = 2;
            const int gh_val = 3;
            const int ef_val = 4;
            const int fh_val = 5;

            // Act
            dict.Add(AB, ab_val);
            dict.Add(CA, ac_val);
            dict.Add(GH, gh_val);
            dict.Add(EF, ef_val);
            dict.Add(FH, fh_val);

            // Assert I
            Assert.That(dict.ContainsKey(BA));
            Assert.That(dict[new Edge(B, A)], Is.EqualTo(ab_val));
            Assert.That(dict[AC], Is.EqualTo(ac_val));
            Assert.That(dict[new Edge(C, A)], Is.EqualTo(ac_val));
            Assert.That(dict[new Edge(H, G)], Is.EqualTo(gh_val));
            Assert.That(dict[new Edge(F, E)], Is.EqualTo(ef_val));
            Assert.That(dict[new Edge(F, H)], Is.EqualTo(fh_val));
        }
    }
}
