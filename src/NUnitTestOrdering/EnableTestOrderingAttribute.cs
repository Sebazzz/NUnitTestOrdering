// ******************************************************************************
//  © 2016 Sebastiaan Dammann - damsteen.nl
// 
//  File:           : EnableTestFixtureOrderingAttribute.cs
//  Project         : NUnitTestOrdering
// ******************************************************************************

namespace NUnitTestOrdering.FixtureOrdering {
    using System;
    using System.Diagnostics;
    using System.Reflection;

    using Common;

    using Internal;
    using Internal.ExecutionTracking;

    using MethodOrdering.Internal;

    using NUnit.Framework;
    using NUnit.Framework.Interfaces;
    using NUnit.Framework.Internal;

    /// <summary>
    /// Apply this attribute to your assembly to enable test ordering
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly)]
    public sealed class EnableTestOrderingAttribute : OrderedTestGlobalSetUpFixtureAttribute, IApplyToTest, ITestAction {
        private readonly TestExecutionTracker _testExecutionTracker;

        /// <inheritdoc/>
        public EnableTestOrderingAttribute() {
            this._testExecutionTracker = new TestExecutionTracker();
        }

        /// <inheritdoc />
        public void ApplyToTest(Test test) {
            TestAssembly testAssembly = test as TestAssembly;
            Assembly assembly;

            // When NUnit find a SetUpFixture it passes in the SetUpFixture,
            // but the parent of the SetUpFixture is null. This means we really aren't
            // able to determine the test assembly, except to assume that the SetUpFixture's type is in the test assembly
            TestSuite root = testAssembly;
            if (testAssembly == null || testAssembly.Tests.Count == 1 && testAssembly.Tests[0] is SetUpFixture) {
                // As of NUnit 3.8.0 the SetupFixture is not the root but the first and only test of the TestAssembly

                SetUpFixture setUpFixture = testAssembly == null ? test as SetUpFixture : (SetUpFixture) testAssembly.Tests[0];

                if (setUpFixture == null) {
                    throw new TestOrderingException($"Expected condition: input object {test} is not a {typeof(TestAssembly).FullName} nor a {typeof(SetUpFixture).FullName}");
                }

                assembly = setUpFixture.TypeInfo.Assembly;
                root = setUpFixture;

                SetUpFixtureValidator.ValidateGlobalSetUpFixture(setUpFixture);

                Trace.WriteLine("Note: Test assembly is using a SetUpFixture. Assuming SetUpFixture type is the test assembly.");
            } else {
                assembly = testAssembly.Assembly;
            }

            Debug.Assert(root != null);
            Debug.Assert(assembly != null);

            ApplyOrdering(root, assembly);
        }

        private static void ApplyOrdering(TestSuite root, Assembly assembly) {
            // Mark for debugging
            root.Properties.Set(nameof(NUnitTestOrdering), "Applied at: " + Environment.StackTrace);

            // Apply test orderer
            TestMethodOrdererApplier.DoOrderTestMethodsRecursive(root);
            TestFixtureOrderer orderer = new TestFixtureOrderer(assembly, root);
            orderer.OrderTests();
        }

        /// <inheritdoc />
        public void BeforeTest(ITest test) {
            this._testExecutionTracker.HandleTestStart(test);
        }

        /// <inheritdoc />
        public void AfterTest(ITest test) {
            this._testExecutionTracker.TrackExecution(test);
        }

        /// <inheritdoc />
        public ActionTargets Targets => ActionTargets.Suite | ActionTargets.Test;
    }
}
