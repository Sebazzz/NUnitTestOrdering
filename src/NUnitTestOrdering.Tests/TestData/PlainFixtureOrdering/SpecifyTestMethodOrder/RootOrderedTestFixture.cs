// ******************************************************************************
//  © 2016 Sebastiaan Dammann - damsteen.nl
// 
//  File:           : RootOrderedTestFixture.cs
//  Project         : NUnitTestOrdering.Tests
// ******************************************************************************

namespace NUnitTestOrdering.Tests.TestData.PlainFixtureOrdering.SpecifyTestMethodOrder {
    using NUnit.Framework;

    using NUnitTestOrdering.FixtureOrdering;

    [OrderedTestFixture]
    public sealed class RootOrderedTestFixture : TestOrderingSpecification {
        protected override void DefineTestOrdering() {
            this.TestFixture<TestOne>(x => x.TestMethod(t => t.DoTest()));

            this.TestFixture<TestTwo>(x => x.TestMethod(t => t.PreTest())
                                            .TestMethod(t => t.DoTest())
                                            .TestMethod(t => t.PostTest()));

            this.TestFixture<TestThree>();
        }
        protected override bool ContinueOnError => false;
    }

    [TestFixture]
    public sealed class TestOne : LoggingTestBase {
        [Test]
        public void DoTest() {
            this.Log();
        }

        [Test]
        public void I_Will_Not_Run() {
            this.Log();
        }
    }

    [TestFixture]
    public sealed class TestTwo : LoggingTestBase {
        [Test]
        public void DoTest() {
            this.Log();
        }

        [Test]
        public void PostTest() {
            this.Log();
        }

        [Test]
        public void PreTest() {
            this.Log();
        }
    }

    [TestFixture]
    public sealed class TestThree : LoggingTestBase {
        [Test]
        public void DoTest() {
            this.Log();
        }
    }
}
