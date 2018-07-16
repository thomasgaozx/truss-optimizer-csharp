using NUnit.Framework;
using Truss2D.Math;
using Truss2D.Shell;

namespace Truss2D.TrussIntegrationTest
{
    [TestFixture]
    public class TrussBuilderIntegrationTest
    {
        private TrussBuilder builder;

        [OneTimeSetUp]
        public void SetUpFixture()
        {
            builder = new TrussBuilder();
        }

        [SetUp]
        public void SetUpTest()
        {
            builder.Restart();
        }

        /// <summary>
        /// Syntax shortcut
        /// </summary>
        private void PutJoint(int x, int y)
        {
            builder.AddJoint(new Vertice(x, y));
        }

        /// <summary>
        /// Syntax shortcut
        /// </summary>
        private void Link(char a, char b)
        {
            builder.LinkJoints(a, b);
        }

        private void PutForce(char joint, int x, int y)
        {
            builder.AddForce(builder.GetJoint(joint), new Vector(x, y));
        }

        private decimal? LookupInternalForce(char joint1, char joint2)
        {
            return builder.Model.GetInternalForce(new Edge(builder.GetJoint(joint1), builder.GetJoint(joint2)));
        }

        [Test]
        public void RegularTruss1()
        {
            // Arrange joints
            PutJoint(-6, 2);
            PutJoint(-3, 5);
            PutJoint(0, 5);
            PutJoint(3, 5);
            PutJoint(0, 0);
            PutJoint(-3, 2);

            // Arrange edges
            Link('a', 'b');
            Link('b', 'f');
            Link('a', 'f');
            Link('b', 'c');
            Link('c', 'f');
            Link('c', 'd');
            Link('c', 'e');
            Link('e', 'd');
            Link('a', 'e');

            // Arrange Force
            PutForce('a', 0, -5);
            PutForce('b', 0, -4);
            PutForce('e', 0, 23);
            PutForce('d', 0, -14);

            // Act
            builder.Render();

            //Assert
            Assert.That(LookupInternalForce('a', 'b'), Is.EqualTo(3.11).Within(0.01));
            Assert.That(LookupInternalForce('b', 'f'), Is.EqualTo(-6.2).Within(0.01));
            Assert.That(LookupInternalForce('a', 'f'), Is.EqualTo(6.2).Within(0.01));
            Assert.That(LookupInternalForce('c', 'b'), Is.EqualTo(2.2).Within(0.01));
            Assert.That(LookupInternalForce('c', 'f'), Is.EqualTo(0).Within(0.01));
            Assert.That(LookupInternalForce('c', 'd'), Is.EqualTo(8.4).Within(0.01));
            Assert.That(LookupInternalForce('c', 'e'), Is.EqualTo(-6.2).Within(0.01));
            Assert.That(LookupInternalForce('e', 'd'), Is.EqualTo(-16.33).Within(0.01));
            Assert.That(LookupInternalForce('a', 'e'), Is.EqualTo(-8.85).Within(0.01));

        }


    }
}
