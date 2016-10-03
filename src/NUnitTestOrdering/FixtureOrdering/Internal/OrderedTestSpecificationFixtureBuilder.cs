namespace NUnitTestOrdering.FixtureOrdering.Internal {
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    using Common;

    using NUnit.Framework.Interfaces;
    using NUnit.Framework.Internal;

    internal sealed class OrderedTestSpecificationFixtureBuilder {
        private readonly TestHierarchyContext _context;

        public OrderedTestSpecificationFixtureBuilder(TestHierarchyContext context) {
            this._context = context;
        }

        public OrderedTestSpecificationFixture BuildFrom(Type type) {
            using (this.CreateScope(type)) {
                OrderedTestSpecificationFixture orderedTestFixture = new OrderedTestSpecificationFixture(new TypeWrapper(type));

                try {
                    this.AddPartsToFixture(orderedTestFixture, type);
                } catch (Exception ex) {
                    orderedTestFixture.RunState = RunState.NotRunnable;
                    orderedTestFixture.Properties.Set(PropertyNames.SkipReason, ex.ToString());
                }

                return orderedTestFixture;
            }
        }

        private IDisposable CreateScope(Type type) {
            try {
                return this._context.EnterHierarchy(type);
            }
            catch (Exception ex) {
                throw new TestOrderingException($"Ordered test specification {type} is already registered within the same line of ancestors", ex);
            }
        }

        private void AddPartsToFixture(OrderedTestSpecificationFixture orderedTestFixture, Type type) {
            TestSpecificationInfo testInfo = TestOrderingSpecification.RunSpecification(type);
            orderedTestFixture.ContinueOnError = testInfo.ContinueOnError;

            if (testInfo.Parts.Count == 0) {
                orderedTestFixture.RunState = RunState.NotRunnable;
                orderedTestFixture.Properties.Set(PropertyNames.SkipReason, "The ordered specification doesn't contain any child tests");
                return;
            }

            Dictionary<string, TestInfo> names = new Dictionary<string, TestInfo>(StringComparer.Ordinal);
                
            int order = 0;
            foreach (IOrderedTestPart part in testInfo.Parts) {
                Test test = part.GetTest(this._context);

                TestInfo existingTestInfo;
                if (names.TryGetValue(test.Name, out existingTestInfo)) {
                    Debug.WriteLine($"Test {test.MethodName} already exists on the same level of tests: {String.Join(";", names)}. Renaming tests.");

                    // Rename existing test
                    if (existingTestInfo.TestCount == 1) existingTestInfo.Test.Name += $"[{existingTestInfo.TestCount - 1}]";
                    existingTestInfo.TestCount++;

                    test.Name += $"[{existingTestInfo.TestCount - 1}]";
                } else {
                    names.Add(test.Name, new TestInfo {
                        Test = test,
                        TestCount = 1
                    });
                }
                
                orderedTestFixture.Add(test);
                test.Properties.Set(PropertyNames.Order, order++);
            }
        }

        private sealed class TestInfo {
            public Test Test { get; set; }
            public int TestCount { get; set; }
        }
    }
}