// ******************************************************************************
//  © 2016 Sebastiaan Dammann - damsteen.nl
// 
//  File:           : TestAssemblyOrderingContext.cs
//  Project         : NUnitTestOrdering
// ******************************************************************************

namespace NUnitTestOrdering.FixtureOrdering.Internal {
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Common;

    using NUnit.Framework;
    using NUnit.Framework.Internal;

    internal class TestAssemblyOrderingContext {
        private readonly TestAssembly _testAssembly;
        private static TestAssemblyOrderingContext Current;

        public IDictionary<Type, TestInfo> TestsByType { get; }

        public Test Root => this._testAssembly;

        public Test FindAndRemove(Type type) {
            return this.FindAndRemove(this.Root, type);
        }

        private Test FindAndRemove(Test container, Type type) {
            foreach (Test test in container.Tests.Cast<Test>()) {
                if (test.TypeInfo?.Type == type) {
                    container.Tests.Remove(test);
                    return test;
                }

                Test childTest = this.FindAndRemove(test, type);
                if (childTest != null) {
                    return childTest;
                }
            }

            return null;
        }

        private TestAssemblyOrderingContext(TestAssembly testAssembly) {
            this._testAssembly = testAssembly;

            this.TestsByType = new NullableDictionary<Type, TestInfo>();
            InitializeTestsByTypeDictionary(this.TestsByType, this._testAssembly);
        }

        private static void InitializeTestsByTypeDictionary(IDictionary<Type, TestInfo> testsByType, Test rootTest) {
            if (rootTest.TypeInfo != null) {
                testsByType[rootTest.TypeInfo.Type] = new TestInfo(rootTest);
            }

            foreach (Test test in rootTest.Tests.OfType<Test>()) {
                InitializeTestsByTypeDictionary(testsByType, test);
            }
        }

        public static TestAssemblyOrderingContext Create(TestAssembly testAssembly) {
            return Current = new TestAssemblyOrderingContext(testAssembly);
        }

        internal static TestAssemblyOrderingContext GetCurrent() {
            if (Current == null) {
                throw new TestOrderingException("There is no TestAssemblyOrderingContext");
            }

            return Current;
        }

        public void Initialize() {
            this.InitializeTestOrderingSpecs();
            this.FixTestHierarchy();
        }

        private void FixTestHierarchy() {
            this.AddUnorderedTestSuite();
            this.AddOrderedTestSuite();
        }

        private void AddUnorderedTestSuite() {
            TestSuite unorderedTests = new TestSuite("Unordered");

            foreach (Test test in this._testAssembly.Tests.Cast<Test>().ToList()) {
                this._testAssembly.Tests.Remove(test);
                unorderedTests.Add(test);
            }

            TestHierarchyUtility.RemoveEmptyNodes(unorderedTests);
            if (unorderedTests.HasChildren) {
                this._testAssembly.Tests.Add(unorderedTests);
            }
        }

        private void AddOrderedTestSuite() {
            OrderedTestRootSuite orderedTestRootSuite = new OrderedTestRootSuite("Ordered");

            // Can't afford parallelism in an ordered test
            orderedTestRootSuite.Properties.Set(PropertyNames.ParallelScope, ParallelScope.None);

            foreach (TestInfo testInfo in this.TestsByType.Values) {
                if (testInfo.PartOf == null && testInfo.Test is OrderedTestSpecificationFixture) {
                    orderedTestRootSuite.Add(testInfo.Test);
                    testInfo.SetPartOf(orderedTestRootSuite);
                }
            }

            this._testAssembly.Add(orderedTestRootSuite);
        }

        private void InitializeTestOrderingSpecs() {
            OrderedTestSpecificationFixtureBuilder builder = new OrderedTestSpecificationFixtureBuilder(this);
            foreach (Type type in TestOrderingSpecificationDiscoverer.GetFromAssembly(this._testAssembly.Assembly)) {
                // Because building the test specifications itself may register additional
                // test specifications, we must check if those specifications aren't registered
                // already by the time we reach them

                if (builder.IsStillUnregistered(type)) {
                    builder.BuildFrom(type);
                }
            }
        }
    }
}
