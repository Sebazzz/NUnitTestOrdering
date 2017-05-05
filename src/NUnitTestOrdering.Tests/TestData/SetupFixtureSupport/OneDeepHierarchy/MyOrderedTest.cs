// ******************************************************************************
//  © 2016 Sebastiaan Dammann - damsteen.nl
// 
//  File:           : MyOrderedTest.cs
//  Project         : NUnitTestOrdering.Tests
// ******************************************************************************

namespace NUnitTestOrdering.Tests.TestData.SetupFixtureSupport.OneDeepHierarchy {
    using NUnit.Framework;

    [TestFixture]
    public sealed class MyUnorderedTest : LoggingTestBase {
        [Test]
        [Order(1)]
        public void TestOne() {
            this.Log();
        }

        [Test]
        [Order(2)]
        public void TestTwo() {
            this.Log();
        }
    }

    [SetUpFixture]
    public sealed class NamespaceSetUpFixture : LoggingTestBase {
        [OneTimeSetUp]
        public void SetUp() {
            this.Log();
        }

        [OneTimeTearDown]
        public void TearDown() {
            this.Log();
        }
    }
}