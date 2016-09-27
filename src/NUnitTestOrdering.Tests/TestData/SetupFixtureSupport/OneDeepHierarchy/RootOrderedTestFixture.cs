// ******************************************************************************
//  © 2016 Sebastiaan Dammann - damsteen.nl
// 
//  File:           : RootOrderedTestFixture.cs
//  Project         : NUnitTestOrdering.Tests
// ******************************************************************************

namespace NUnitTestOrdering.Tests.TestData.SetupFixtureSupport.OneDeepHierarchy {
    using NUnit.Framework;

    using NUnitTestOrdering.FixtureOrdering;

    [OrderedTestFixture]
    public sealed class RootOrderedTestFixture : TestOrderingSpecification {
        protected override void DefineTestOrdering() {
            this.TestFixture<TestOne>();
            this.TestFixture<TestTwo>();
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
