namespace NUnitTestOrdering.Tests.IntegrationTests {
    using NUnit.Framework;
    using NUnit.Framework.Interfaces;

    using Support;

    partial class MethodOrdering {
        partial void UsingTestOrderer_OnException_MarksFixtureAsNotRunnable_AssertResultsFile(ResultsDocument testResults) {
            const string prefix = "NUnitTestOrdering.Tests.TestData.MethodOrdering.UsingTestOrderer_OnException_MarksFixtureAsNotRunnable";

            Assert.That(testResults.GetRunState($"{prefix}.Tests"), Is.EqualTo(RunState.NotRunnable));
            Assert.That(testResults.GetFailureMessage($"{prefix}.Tests"), Contains.Substring("I like to fail"));
        }

        partial void UsingTestOrderer_DuplicateTest_MarksFixtureAsNotRunnable_AssertResultsFile(ResultsDocument testResults) {
            const string prefix = "NUnitTestOrdering.Tests.TestData.MethodOrdering.UsingTestOrderer_DuplicateTest_MarksFixtureAsNotRunnable";

            Assert.That(testResults.GetRunState($"{prefix}.Tests"), Is.EqualTo(RunState.NotRunnable));
            Assert.That(testResults.GetFailureMessage($"{prefix}.Tests"), Contains.Substring("Test TheFirstTest was already specified in the ordering list"));
        }
    }
}
