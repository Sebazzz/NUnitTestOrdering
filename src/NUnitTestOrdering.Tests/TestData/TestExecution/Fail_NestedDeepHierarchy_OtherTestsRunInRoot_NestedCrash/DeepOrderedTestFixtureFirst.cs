// ******************************************************************************
//  © 2016 Sebastiaan Dammann - damsteen.nl
// 
//  File:           : RootOrderedTestFixture.cs
//  Project         : NUnitTestOrdering.Tests
// ******************************************************************************

namespace NUnitTestOrdering.Tests.TestData.TestExecution.Fail_NestedDeepHierarchy_OtherTestsRunInRoot_NestedCrash.DeepOrderedTestFixtureFirst {
    using FixtureOrdering;

    using NUnit.Framework;

    //[OrderedTestFixture] //Note: delibate no atrribute so we test how everything behaves
    public sealed class OrderedTestFixture : TestOrderingSpecification {
        protected override void DefineTestOrdering() {
            this.TestFixture<Nest1TestOne>();
            this.TestFixture<Nest1TestTwo>();

            this.OrderedTestSpecification<Child.OrderedTestFixture>();

            this.TestFixture<Nest1TestThree>();
        }

        protected override bool ContinueOnError => true;
    }

    [TestFixture]
    public sealed class Nest1TestOne : LoggingTestBase {
        [Test]
        public void DoTest() {
            this.Log();
        }
    }

    [TestFixture]
    public sealed class Nest1TestTwo : LoggingTestBase {
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
    public sealed class Nest1TestThree : LoggingTestBase {
        [Test]
        public void DoTest() {
            this.Log();
        }
    }
}
