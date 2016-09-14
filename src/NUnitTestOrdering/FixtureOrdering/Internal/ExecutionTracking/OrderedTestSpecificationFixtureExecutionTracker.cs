namespace NUnitTestOrdering.FixtureOrdering.Internal.ExecutionTracking {
    using NUnit.Framework;
    using NUnit.Framework.Internal;

    internal sealed class OrderedTestSpecificationFixtureExecutionTracker : ITestTracker<OrderedTestSpecificationFixture> {
        private readonly TestExecutionTrackingContext _trackingContext;

        public OrderedTestSpecificationFixtureExecutionTracker(TestExecutionTrackingContext trackingContext) {
            this._trackingContext = trackingContext;
        }

        public void TrackExecution(OrderedTestSpecificationFixture test, TestContext currentTestContext) {
            if (ExecutionUtility.IsFailure(currentTestContext.Result)) {
                this.TestHasFailed(test);
            }
        }

        public void HandleTestStart(OrderedTestSpecificationFixture test, TestExecutionContext executionContext) {
            ExecutionUtility.ThrowPreventExecutionOnParentError(this._trackingContext, test);
        }

        private void TestHasFailed(OrderedTestSpecificationFixture test) {
            ExecutionUtility.MarkOrderedTestSpecificationAsFailed(this._trackingContext, test);
        }
    }
}