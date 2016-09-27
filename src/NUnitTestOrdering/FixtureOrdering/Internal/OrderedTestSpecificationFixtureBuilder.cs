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

            HashSet<string> names = new HashSet<string>(StringComparer.Ordinal);
                
            int order = 0;
            foreach (IOrderedTestPart part in testInfo.Parts) {
                Test test = part.GetTest(this._context);

                if (!names.Add(test.Name)) {
                    throw new InvalidOperationException($"Test {test.Name} already exists on the same level of tests: {String.Join(";", names)}");
                }

                orderedTestFixture.Add(test);
                test.Properties.Set(PropertyNames.Order, order++);
            }
        }
    }
}