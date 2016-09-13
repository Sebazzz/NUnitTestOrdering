// ******************************************************************************
//  © 2016 Sebastiaan Dammann - damsteen.nl
// 
//  File:           : EnableTestFixtureOrderingAttribute.cs
//  Project         : NUnitTestOrdering
// ******************************************************************************

namespace NUnitTestOrdering.FixtureOrdering {
    using System;

    using Common;

    using Internal;

    using NUnit.Framework;
    using NUnit.Framework.Interfaces;
    using NUnit.Framework.Internal;

    /// <summary>
    /// Apply this attribute to your assembly to enable test ordering
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly)]
    public sealed class EnableTestFixtureOrderingAttribute : NUnitAttribute, IApplyToTest, IApplyToContext, ITestAction {
        private bool _cancelTest;

        /// <inheritdoc />
        public void ApplyToTest(Test test) {
            TestAssembly testAssembly = test as TestAssembly;

            if (testAssembly == null) {
                throw new TestOrderingException($"Expected condition: input object {test} is not a {typeof(TestAssembly).FullName}");
            }

            TestAssemblyOrderer orderer = new TestAssemblyOrderer(testAssembly);
            orderer.ApplyOrdering();
        }

        /// <inheritdoc />
        public void ApplyToContext(TestExecutionContext context) {
            // Note of the comment below: "execute" means in NUnit terms actually "discovering and scheduling
            // work items _and_ execute them"

            // We need to force NUnit to run single threaded. In this way, NUnit will "execute" ordered test
            // fixtures immediately instead of scheduling the "execution" of the work items. By the way, in
            // ordered tests we cannot have any parallelism anyway.

            // The below statement has the unfortunate consequence the *entire* test assembly won't
            // run in parallel, but if you care so much for that you might as well seperate your unordered
            // (integration) tests to a seperate assembly.
            context.IsSingleThreaded = true;
        }

        /// <inheritdoc />
        public void BeforeTest(ITest test) {
            TestExecutionContext currentTestContext = TestExecutionContext.GetTestExecutionContext();
            if (currentTestContext != null) {
                this.BeforeTestCore(currentTestContext);
            }
        }

        private void BeforeTestCore(TestExecutionContext testContext) {
            if (this._cancelTest) {
                throw new InconclusiveException("A previous test has failed to complete");
            }
        }

        /// <inheritdoc />
        public void AfterTest(ITest test) {
            TestContext currentTestContext = TestContext.CurrentContext;
            if (currentTestContext == null) {
                return;
            }

            this.AfterTestCore(test, currentTestContext);
        }

        private void AfterTestCore(ITest test, TestContext testContext) {
            if (!testContext.Result.Outcome.Equals(ResultState.Success)) {
                this._cancelTest = true;

                TestContext.Progress.WriteLine($"Test {test.FullName} has failed. Subsequent tests will be skipped.");
            }
        }

        /// <inheritdoc />
        public ActionTargets Targets => ActionTargets.Suite | ActionTargets.Test;
    }
}
