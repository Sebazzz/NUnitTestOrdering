// ******************************************************************************
//  © 2016 Sebastiaan Dammann - damsteen.nl
// 
//  File:           : Test.cs
//  Project         : NUnitTestOrdering.Tests
// ******************************************************************************

namespace NUnitTestOrdering.Tests.TestData.MethodOrdering.Simple {
    using NUnit.Framework;

    using NUnitTestOrdering.MethodOrdering;

    [TestFixture]
    public sealed class Test : LoggingTestBase {
        [Test]
        [TestMethodWithoutDependency]
        public void One() {
            this.Log();
        }

        [Test]
        [TestMethodDependency(nameof(Three))]
        public void Four() {
            this.Log();
        }

        [Test]
        [TestMethodDependency(nameof(One))]
        public void Two() {
            this.Log();
        }

        [Test]
        [TestMethodDependency(nameof(Two))]
        public void Three() {
            this.Log();
        }
    }
}
