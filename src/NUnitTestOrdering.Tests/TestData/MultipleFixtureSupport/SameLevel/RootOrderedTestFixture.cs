// ******************************************************************************
//  © 2016 Sebastiaan Dammann - damsteen.nl
// 
//  File:           : RootOrderedTestFixture.cs
//  Project         : NUnitTestOrdering.Tests
// ******************************************************************************

namespace NUnitTestOrdering.Tests.TestData.MultipleFixtureSupport.SameLevel {
    using NUnit.Framework;

    using NUnitTestOrdering.FixtureOrdering;

    [OrderedTestFixture]
    public sealed class RootOrderedTestFixture : TestOrderingSpecification {
        protected override void DefineTestOrdering() {
            this.OrderedTestSpecification<OrderedTestFixture1>();
            this.OrderedTestSpecification<OrderedTestFixture2>();
            this.OrderedTestSpecification<OrderedTestFixture1>();
        }
        protected override bool ContinueOnError => false;
    }

    public sealed class OrderedTestFixture1 : TestOrderingSpecification {
        private readonly TestLogger _testLogger;

        public OrderedTestFixture1() {
            this._testLogger = new TestLogger(this);
        }

        protected override void DefineTestOrdering() {
            this.TestFixture<TestOne>();
            this.TestFixture<TestTwo>();
            this.TestFixture<TestThree>();
        }
        protected override bool ContinueOnError => false;

        [OneTimeSetUp]
        public void OneTimeSetUp() {
            this._testLogger.Log();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown() {
            this._testLogger.Log();
        }
    }

    public sealed class OrderedTestFixture2 : TestOrderingSpecification {
        private readonly TestLogger _testLogger;

        public OrderedTestFixture2() {
            this._testLogger = new TestLogger(this);
        }

        protected override void DefineTestOrdering() {
            this.TestFixture<TestOne>();
            this.TestFixture<TestTwo>();
            this.TestFixture<TestThree>();
        }
        protected override bool ContinueOnError => false;

        [OneTimeSetUp]
        public void OneTimeSetUp() {
            this._testLogger.Log();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown() {
            this._testLogger.Log();
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

    [TestFixture]
    public sealed class TestThree : LoggingTestBase {
        [Test]
        public void DoTest() {
            this.Log();
        }
    }
}
