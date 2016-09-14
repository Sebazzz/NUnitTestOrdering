// ******************************************************************************
//  © 2016 Sebastiaan Dammann - damsteen.nl
// 
//  File:           : TestExecutionTracker.cs
//  Project         : NUnitTestOrdering
// ******************************************************************************

namespace NUnitTestOrdering.FixtureOrdering.Internal.ExecutionTracking {
    using System.Diagnostics;

    using NUnit.Framework;
    using NUnit.Framework.Interfaces;
    using NUnit.Framework.Internal;

    /// <summary>
    ///     Helper class to help track test execution
    /// </summary>
    internal class TestExecutionTracker {
        private readonly TestTrackerDispatcher _testTrackerDispatcher;

        public TestExecutionTracker() {
            this._testTrackerDispatcher = new TestTrackerDispatcher(new TestExecutionTrackingContext());
        }

        /// <summary>
        ///     At the start of the test, update test state
        /// </summary>
        /// <param name="test"></param>
        /// <returns></returns>
        public void HandleTestStart(ITest test) {
            TestExecutionContext executionContext = TestExecutionContext.GetTestExecutionContext();

            if (executionContext != null) {
                this.HandleTestStartCore(test, executionContext);
                return;
            }

            Debug.Assert(executionContext != null, "Expect test execution context not to be null: This library relies on proper function by reading the test context");
        }

        private void HandleTestStartCore(ITest test, TestExecutionContext executionContext) {
            this._testTrackerDispatcher.HandleTestStart(test, executionContext);
        }

        /// <summary>
        ///     At the end of the test, tracks the execution of the specified test
        /// </summary>
        /// <param name="test"></param>
        public void TrackExecution(ITest test) {
            TestContext currentTestContext = TestContext.CurrentContext;

            if (currentTestContext != null) {
                this.TraceExecutionCore(test, currentTestContext);
                return;
            }

            Debug.Assert(currentTestContext != null, "Expect test context not to be null: This library relies on proper function by reading the test context");
        }

        private void TraceExecutionCore(ITest test, TestContext currentTestContext) {
            this._testTrackerDispatcher.TrackExecution(test, currentTestContext);
        }
    }
}
