// ******************************************************************************
//  © 2016 Sebastiaan Dammann - damsteen.nl
// 
//  File:           : RootOrderedTestFixture.cs
//  Project         : NUnitTestOrdering.Tests
// ******************************************************************************

namespace NUnitTestOrdering.Tests.TestData.TestCaseSourceAttributeSupport.TestCaseSourceOnly {
    using System.Collections.Generic;

    using NUnit.Framework;

    using NUnitTestOrdering.FixtureOrdering;

    [OrderedTestFixture]
    public sealed class RootOrderedTestFixture : TestOrderingSpecification {
        protected override void DefineTestOrdering() {
            this.TestFixture<TestOne>();
        }
        protected override bool ContinueOnError => false;
    }

    [TestFixture]
    public sealed class TestOne : LoggingTestBase {
        [Test]
        [TestCaseSource(typeof(TestCases), nameof(TestCases.Tests))]
        public void DoTest(string input) {
            this.Log(input);
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
