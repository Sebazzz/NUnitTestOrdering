// ******************************************************************************
//  © 2016 Sebastiaan Dammann - damsteen.nl
// 
//  File:           : RootOrderedTestFixture.cs
//  Project         : NUnitTestOrdering.Tests
// ******************************************************************************

namespace NUnitTestOrdering.Tests.TestData.TestExecution.Fail_NestedDeepHierarchy_OtherTestsRunInRoot {
    using DeepOrderedTestFixtureFirst;

    using NUnit.Framework;

    using NUnitTestOrdering.FixtureOrdering;

    [OrderedTestFixture]
    public sealed class RootOrderedTestFixture : TestOrderingSpecification {
        protected override void DefineTestOrdering() {
            this.TestFixture<RootTestOne>();

            this.OrderedTestSpecification<OrderedTestFixture>();

            this.TestFixture<RootTestTwo>();
            this.TestFixture<RootTestThree>();
        }

        protected override bool ContinueOnError => true;
    }

    [TestFixture]
    public sealed class RootTestOne : LoggingTestBase {
        [Test]
        public void DoTest() {
            this.Log();
        }
    }

    [TestFixture]
    public sealed class RootTestTwo : LoggingTestBase {
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
    public sealed class RootTestThree : LoggingTestBase {
        [Test]
        public void DoTest() {
            this.Log();
        }
    }
}
