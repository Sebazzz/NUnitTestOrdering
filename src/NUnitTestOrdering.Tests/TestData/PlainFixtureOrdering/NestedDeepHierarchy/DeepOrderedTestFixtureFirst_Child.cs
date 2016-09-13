// ******************************************************************************
//  © 2016 Sebastiaan Dammann - damsteen.nl
// 
//  File:           : RootOrderedTestFixture.cs
//  Project         : NUnitTestOrdering.Tests
// ******************************************************************************

namespace NUnitTestOrdering.Tests.TestData.PlainFixtureOrdering.NestedDeepHierarchy.DeepOrderedTestFixtureFirst.Child {
    using FixtureOrdering;

    using NUnit.Framework;

    [OrderedTestFixture]
    public sealed class OrderedTestFixture : TestOrderingSpecification {
        protected override void DefineTestOrdering() {
            this.TestFixture<Nest2TestOne>();
            this.TestFixture<Nest2TestTwo>();
            this.TestFixture<Nest2TestThree>();
        }
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
            this.Log();
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
