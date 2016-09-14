namespace NUnitTestOrdering.FixtureOrdering.Internal.ExecutionTracking {
    using NUnit.Framework;
    using NUnit.Framework.Internal;

    internal sealed class TestMethodExecutionTracker : ITestTracker<TestMethod> {
        private readonly TestExecutionTrackingContext _trackingContext;

        public TestMethodExecutionTracker(TestExecutionTrackingContext trackingContext) {
            this._trackingContext = trackingContext;
        }

        public void TrackExecution(TestMethod test, TestContext currentTestContext) {
            // A test fixture will be marked as failed if any of the tests in the test fixture fail,
            // or if any of the tests in the test fixture are inconclusive (but that's a kind-of fail too)
            if (ExecutionUtility.IsFailure(currentTestContext.Result)) {
                this.TestHasFailed(test);
            }
        }

        public void HandleTestStart(TestMethod test, TestExecutionContext executionContext) {
            ExecutionUtility.ThrowPreventExecutionOnParentError(this._trackingContext, test);
        }

        private void TestHasFailed(TestMethod test) {
            ExecutionUtility.MarkOrderedTestSpecificationAsFailed(this._trackingContext, test);
        }
    }
}