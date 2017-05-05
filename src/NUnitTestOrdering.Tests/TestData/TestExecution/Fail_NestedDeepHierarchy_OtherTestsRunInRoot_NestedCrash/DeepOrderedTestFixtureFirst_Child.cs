// ******************************************************************************
//  © 2016 Sebastiaan Dammann - damsteen.nl
// 
//  File:           : RootOrderedTestFixture.cs
//  Project         : NUnitTestOrdering.Tests
// ******************************************************************************

namespace NUnitTestOrdering.Tests.TestData.TestExecution.Fail_NestedDeepHierarchy_OtherTestsRunInRoot_NestedCrash.DeepOrderedTestFixtureFirst.Child {
    using FixtureOrdering;

    using NUnit.Framework;

    public sealed class OrderedTestFixture : TestOrderingSpecification {
        protected override void DefineTestOrdering() {
            this.TestFixture<Nest2TestOne>();
            this.TestFixture<Nest2TestTwo>();
            this.TestFixture<Nest2TestThree>();
        }
        protected override bool ContinueOnError => false;
    }

    [TestFixture]
    public sealed class Nest2TestOne : LoggingTestBase {
        [Test]
        public void DoTest() {
            this.Log();
        }
    }

    [TestFixture]
    public sealed class Nest2TestTwo : LoggingTestBase {
        [Test]
        [Order(2)]
        public void DoTest() {
            this.Log("Crash");
            Assert.Fail("I need to fail, and only the root should continue.");
        }

        [Test]
        [Order(1)]
        public void PreTest() {
            this.Log();
        }
    }

    [TestFixture]
    public sealed class Nest2TestThree : LoggingTestBase {
        [Test]
        public void DoTest() {
            this.Log();
        }
    }
}
