namespace NUnitTestOrdering.FixtureOrdering.Internal.ExecutionTracking {
    using System.Diagnostics;

    using NUnit.Framework;
    using NUnit.Framework.Interfaces;
    using NUnit.Framework.Internal;

    internal static class ExecutionUtility {
        public static bool IsFailure(TestContext.ResultAdapter testResult) {
            TestStatus testStatus = testResult.Outcome.Status;
            switch (testStatus) {
                case TestStatus.Passed:
                    return false;

                case TestStatus.Warning:
                case TestStatus.Failed:
                    goto case TestStatus.Inconclusive;

                case TestStatus.Inconclusive:
                case TestStatus.Skipped:
                    return true;

                default:
                    Debug.Fail($"Unexpected test status: {testStatus} #{testStatus:D}");
                    return false;
            }
        }

        public static void MarkOrderedTestSpecificationAsFailed(TestExecutionTrackingContext trackingContext, Test test) {
            OrderedTestSpecificationFixture containingOrderedSpec = trackingContext.FindOrderedSpec(test);

            // Recursively find all parents which need to fail
            while (true) {
                if (containingOrderedSpec == null) {
                    return;
                }

                if (containingOrderedSpec.ContinueOnError) {
                    return;
                }

                containingOrderedSpec.MarkAsFailed(test.FullName);

                containingOrderedSpec = trackingContext.FindOrderedSpec(containingOrderedSpec);
            }
        }

        public static void ThrowPreventExecutionOnParentError(TestExecutionTrackingContext trackingContext, Test currentTest) {
            OrderedTestSpecificationFixture spec = trackingContext.FindOrderedSpec(currentTest);
            if (spec == null) {
                return;
            }

            if (spec.HasFailed()) {
                ResultStateException resultStateException = new InconclusiveException(spec.GetFailReason());

                throw resultStateException;
            }
        }
    }
}