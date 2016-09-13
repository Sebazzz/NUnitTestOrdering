// ******************************************************************************
//  © 2016 Sebastiaan Dammann - damsteen.nl
// 
//  File:           : RootOrderedTestFixture.cs
//  Project         : NUnitTestOrdering.Tests
// ******************************************************************************

namespace NUnitTestOrdering.Tests.TestData.PlainFixtureOrdering.HierarchySetupTeardown {
    using FixtureOrdering;

    using NUnit.Framework;

    [OrderedTestFixture]
    public sealed class RootOrderedTestFixture : TestOrderingSpecification {
        private readonly TestLogger _testLogger;

        public RootOrderedTestFixture() {
            this._testLogger = new TestLogger(this);
        }

        [OneTimeSetUp]
        public void OneTimeSetUp() {
            this._testLogger.Log();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown() {
            this._testLogger.Log();
        }

        protected override void DefineTestOrdering() {
            this.TestFixture<TestOne>();
            this.TestFixture<TestTwo>();
        }
    }

    [TestFixture]
    public sealed class TestOne : LoggingTestBase {
        [Test]
        public void DoTest() {
            this.Log();
        }
    }

    [TestFixture]
    public sealed class TestTwo : LoggingTestBase {
        [Test]
        [Order(2)]
        public void DoTest() {
            this.Log();
        }

        [Test]
        [Order(1)]
        public void PreTest() {
            this.Log();
        }
    }
}
