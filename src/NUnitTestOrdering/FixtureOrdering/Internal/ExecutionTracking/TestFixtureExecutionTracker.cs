namespace NUnitTestOrdering.FixtureOrdering.Internal.ExecutionTracking {
    using System.Diagnostics;

    using NUnit.Framework;
    using NUnit.Framework.Internal;

    internal sealed class TestFixtureExecutionTracker : ITestTracker<TestFixture> {
        private readonly TestExecutionTrackingContext _trackingContext;

        public TestFixtureExecutionTracker(TestExecutionTrackingContext trackingContext) {
            this._trackingContext = trackingContext;
        }

        public void TrackExecution(TestFixture test, TestContext currentTestContext) {
            Debug.Assert(!(test is OrderedTestSpecificationFixture));

            // A test fixture will be marked as failed if any of the tests in the test fixture fail,
            // or if any of the tests in the test fixture are inconclusive (but that's a kind-of fail too)
            if (ExecutionUtility.IsFailure(currentTestContext.Result)) {
                this.TestHasFailed(test);
            }
        }

        public void HandleTestStart(TestFixture test, TestExecutionContext executionContext) {
        }

        private void TestHasFailed(TestFixture test) {
            ExecutionUtility.MarkOrderedTestSpecificationAsFailed(this._trackingContext, test);
        }
    }
}