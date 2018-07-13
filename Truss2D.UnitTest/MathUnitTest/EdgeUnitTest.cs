using NUnit.Framework;
using Truss2D.Math;

namespace Truss2D.UnitTest.Math
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
            UnitVector u = new Edge(new Vertice(), new Vertice(2, 2)).DirectionFrom(new Vertice());
            Assert.That((double)u.X, Is.EqualTo(System.Math.Sqrt(2) / 2.0).Within(0.01));
            Assert.That(u.Y, Is.EqualTo(u.X).Within( (decimal)0.01 ));
        }

        [Test]
        public void Edge_DirectionFrom_AToB()
        {
            var expectedUnitVector = new UnitVector(new Vector(B.X - A.X, B.Y - A.Y));
            Assert.That(AB.DirectionFrom(A),
                Is.EqualTo(expectedUnitVector));
            Assert.That(BA.DirectionFrom(A),
                Is.EqualTo(expectedUnitVector));
        }
        

        [Test]
        public void Edge_DirectionFrom_HToG()
        {
            var expectedUnitVector = new UnitVector(new Vector(G.X - H.X, G.Y - H.Y));
            Assert.That(GH.DirectionFrom(H),
                Is.EqualTo(expectedUnitVector));

        }
    }
}
