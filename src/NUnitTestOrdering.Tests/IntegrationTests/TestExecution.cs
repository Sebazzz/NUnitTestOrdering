// ******************************************************************************
//  © 2016 Sebastiaan Dammann - damsteen.nl
// 
//  File:           : TestExecution.cs
//  Project         : NUnitTestOrdering.Tests
// ******************************************************************************

namespace NUnitTestOrdering.Tests.IntegrationTests {
    using System.Xml.Linq;

    using NUnit.Framework;
    using NUnit.Framework.Interfaces;

    using Support;

    partial class TestExecution {
        partial void Fail_OneDeepHierarchy_AssertResultsFile(ResultsDocument testResults) {
            string baseName = "Ordered.RootOrderedTestFixture";

            Assert.That(testResults.GetTestResult(baseName + ".TestOne"), Is.EqualTo(TestStatus.Passed));
            Assert.That(testResults.GetTestResult(baseName + ".TestTwo"), Is.EqualTo(TestStatus.Failed));
            Assert.That(testResults.GetTestResult(baseName + ".TestTwo.DoTest"), Is.EqualTo(TestStatus.Inconclusive));
            Assert.That(testResults.GetTestResult(baseName + ".TestThree.DoTest"), Is.EqualTo(TestStatus.Inconclusive));
        }
    }
}
