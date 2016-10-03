// ******************************************************************************
//  © 2016 Sebastiaan Dammann - damsteen.nl
// 
//  File:           : Tests.cs
//  Project         : NUnitTestOrdering.Tests
// ******************************************************************************

namespace NUnitTestOrdering.Tests.TestData.MethodOrdering.UsingTestOrderer_DuplicateTest_MarksFixtureAsNotRunnable {
    using System.Diagnostics.CodeAnalysis;

    using NUnit.Framework;

    using NUnitTestOrdering.MethodOrdering;

    [TestFixture]
    [TestMethodOrderer(typeof(Orderer))]
    public sealed class Tests : LoggingTestBase {
        [Test]
        public void TheFirstTest() {
            this.Log();
        }

        [Test]
        public void ShouldExecuteSecond() {
            this.Log();
        }

        [Test]
        public void LastOne() {
            this.Log();
        }

        [SuppressMessage("ReSharper", "ArrangeThisQualifier")]
        private sealed class Orderer : TestOrderer<Tests> {
            protected override void DefineOrdering() {
                TestMethod(nameof(TheFirstTest));
                TestMethod(x => x.TheFirstTest()); // Alternate syntax, this should throw
            }
        }
    }
}
