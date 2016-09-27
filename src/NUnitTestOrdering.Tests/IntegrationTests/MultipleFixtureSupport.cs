// ******************************************************************************
//  © 2016 Sebastiaan Dammann - damsteen.nl
// 
//  File:           : MultipleFixtureSupport.cs
//  Project         : NUnitTestOrdering.Tests
// ******************************************************************************

namespace NUnitTestOrdering.Tests.IntegrationTests {
    using NUnit.Framework;
    using NUnit.Framework.Interfaces;

    using Support;

    partial class MultipleFixtureSupport {
        partial void Simple_AssertResultsFile(ResultsDocument testResults) {
            const string baseName = "Ordered.RootOrderedTestFixture";

            Assert.That(testResults.GetTestResult(baseName + ".OrderedTestFixture1"), Is.EqualTo(TestStatus.Passed));
            Assert.That(testResults.GetTestResult(baseName + ".OrderedTestFixture1.TestOne"), Is.EqualTo(TestStatus.Passed));
            Assert.That(testResults.GetTestResult(baseName + ".OrderedTestFixture1.TestOne.DoTest"), Is.EqualTo(TestStatus.Passed));
            Assert.That(testResults.GetTestResult(baseName + ".OrderedTestFixture2"), Is.EqualTo(TestStatus.Passed));
            Assert.That(testResults.GetTestResult(baseName + ".OrderedTestFixture2.TestOne"), Is.EqualTo(TestStatus.Passed));
            Assert.That(testResults.GetTestResult(baseName + ".OrderedTestFixture2.TestOne.DoTest"), Is.EqualTo(TestStatus.Passed));
        }
    }
}
