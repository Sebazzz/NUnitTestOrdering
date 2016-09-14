namespace NUnitTestOrdering.FixtureOrdering.Internal.ExecutionTracking {
    using NUnit.Framework;
    using NUnit.Framework.Interfaces;
    using NUnit.Framework.Internal;

    internal interface ITestTracker<in TTest> where TTest : ITest {
        void TrackExecution(TTest test, TestContext currentTestContext);

        void HandleTestStart(TTest test, TestExecutionContext executionContext);
    }
}