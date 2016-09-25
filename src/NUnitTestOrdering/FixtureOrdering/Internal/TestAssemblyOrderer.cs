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
    using System.Reflection;

    using Common;

    using NUnit.Framework;
    using NUnit.Framework.Internal;

    internal interface ITestAssemblyOrderingContext {
        IDictionary<Type, TestInfo> TestsByType { get; }
        Test FindAndRemove(Type type);
    }

    internal class TestAssemblyOrderer : ITestAssemblyOrderingContext {
        private readonly Assembly _testAssembly;
        private readonly TestSuite _rootFixture;

        public IDictionary<Type, TestInfo> TestsByType { get; }

        public Test Root => this._rootFixture;

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

        public TestAssemblyOrderer(Assembly testAssembly, TestSuite rootFixture) {
            this._testAssembly = testAssembly;
            this._rootFixture = rootFixture;

            this.TestsByType = new NullableDictionary<Type, TestInfo>();
            InitializeTestsByTypeDictionary(this.TestsByType, this._rootFixture);
        }

        private static void InitializeTestsByTypeDictionary(IDictionary<Type, TestInfo> testsByType, Test rootTest) {
            if (rootTest.TypeInfo != null) {
                testsByType[rootTest.TypeInfo.Type] = new TestInfo(rootTest);
            }

            foreach (Test test in rootTest.Tests.OfType<Test>()) {
                InitializeTestsByTypeDictionary(testsByType, test);
            }
        }

        public void OrderTests() {
            this.InitializeTestOrderingSpecs();
            this.FixTestHierarchy();
        }

        private void FixTestHierarchy() {
            this.AddUnorderedTestSuite();
            this.AddOrderedTestSuite();
        }

        private void AddUnorderedTestSuite() {
            TestSuite unorderedTests = new TestSuite("Unordered");
            unorderedTests.Properties.Set(PropertyNames.Order, -1000);

            foreach (Test test in this._rootFixture.Tests.Cast<Test>().ToList()) {
                this._rootFixture.Tests.Remove(test);
                unorderedTests.Add(test);
            }

            TestHierarchyUtility.RemoveEmptyNodes(unorderedTests);
            if (unorderedTests.HasChildren) {
                this._rootFixture.Tests.Add(unorderedTests);
            }
        }

        private void AddOrderedTestSuite() {
            OrderedTestRootSuite orderedTestRootSuite = new OrderedTestRootSuite("Ordered");

            // Can't afford parallelism in an ordered test
            orderedTestRootSuite.Properties.Set(PropertyNames.ParallelScope, ParallelScope.None);
            orderedTestRootSuite.Properties.Set(PropertyNames.Order, 1000);

            foreach (TestInfo testInfo in this.TestsByType.Values) {
                if (testInfo.PartOf == null && testInfo.Test is OrderedTestSpecificationFixture) {
                    orderedTestRootSuite.Add(testInfo.Test);
                    testInfo.SetPartOf(orderedTestRootSuite);
                }
            }

            FullNameAssigner.AssignFullName(orderedTestRootSuite);

            this._rootFixture.Add(orderedTestRootSuite);
        }

        private void InitializeTestOrderingSpecs() {
            OrderedTestSpecificationFixtureBuilder builder = new OrderedTestSpecificationFixtureBuilder(this);
            foreach (Type type in TestOrderingSpecificationDiscoverer.GetFromAssembly(this._testAssembly)) {
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
