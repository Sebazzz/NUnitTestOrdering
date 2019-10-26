// ******************************************************************************
//  © 2016 Sebastiaan Dammann - damsteen.nl
// 
//  File:           : RootOrderedTestFixture.cs
//  Project         : NUnitTestOrdering.Tests
// ******************************************************************************

namespace NUnitTestOrdering.Tests.TestData.TestCaseSourceAttributeSupport.TestCaseSourceWithMethodOrder {
    using System.Collections.Generic;

    using NUnit.Framework;

    using NUnitTestOrdering.FixtureOrdering;

    [OrderedTestFixture]
    public sealed class RootOrderedTestFixture : TestOrderingSpecification {
        protected override void DefineTestOrdering() {
            this.TestFixture<TestOne>(cfg => cfg.TestMethod(x => x.FirstTest()).TestMethod(x => x.CentralTest(null)).TestMethod(x => x.LastTest()));
        }
        protected override bool ContinueOnError => false;
    }

    [TestFixture]
    public sealed class TestOne : LoggingTestBase {
        [Test]
        public void FirstTest() {
            this.Log();
        }

        [Test]
        [TestCaseSource(typeof(TestCases), nameof(TestCases.Tests))]
        public void CentralTest(string input) {
            this.Log(input);
        }

        [Test]
        public void LastTest() {
            this.Log();
        }

        public static class TestCases {
            public static IEnumerable<string> Tests() {
                yield return "Source 1";
                yield return "Source 2";
                yield return "Source 3";
            }
        }
    }

}
