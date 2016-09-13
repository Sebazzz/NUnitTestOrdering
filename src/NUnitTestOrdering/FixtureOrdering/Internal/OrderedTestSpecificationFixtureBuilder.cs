namespace NUnitTestOrdering.FixtureOrdering.Internal {
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    using Common;

    using NUnit.Framework.Interfaces;
    using NUnit.Framework.Internal;

    internal sealed class OrderedTestSpecificationFixtureBuilder {
        private readonly TestAssemblyOrderingContext _context;

        public OrderedTestSpecificationFixtureBuilder(TestAssemblyOrderingContext context) {
            this._context = context;
        }

        public OrderedTestSpecificationFixture BuildFrom(Type type) {
            OrderedTestSpecificationFixture orderedTestFixture = new OrderedTestSpecificationFixture(new TypeWrapper(type));

            this.EnsureNotRegistered(type);

            this._context.TestsByType[type] = new TestInfo(orderedTestFixture);

            try {
                this.AddPartsToFixture(orderedTestFixture, type);
            } catch (Exception ex) {
                orderedTestFixture.RunState = RunState.NotRunnable;
                orderedTestFixture.Properties.Set(PropertyNames.SkipReason, ex.ToString());
            }

            return orderedTestFixture;
        }

        private void EnsureNotRegistered(Type type) {
            TestInfo existingRegistration = this._context.TestsByType[type];
            if (existingRegistration != null) {
                throw new TestOrderingException($"Ordered test specification {type} is already registered. It has {existingRegistration.PartOf?.FullName ?? "no parent"} as parent");
            }
        }

        private void AddPartsToFixture(OrderedTestSpecificationFixture orderedTestFixture, Type type) {
            ICollection<IOrderedTestPart> parts = TestOrderingSpecification.RunSpecification(type);

            if (parts.Count == 0) {
                orderedTestFixture.RunState = RunState.NotRunnable;
                orderedTestFixture.Properties.Set(PropertyNames.SkipReason, "The ordered specification doesn't contain any child tests");
                return;
            }

            int order = 0;
            foreach (IOrderedTestPart part in parts) {
                Test test = part.GetTest(this._context);
                
                if (test.TypeInfo != null) {
                    TestInfo registration = this._context.TestsByType[test.TypeInfo.Type];

                    Debug.Assert(registration != null);
                    registration.SetPartOf(orderedTestFixture);
                }

                orderedTestFixture.Add(test);
                test.Properties.Set(PropertyNames.Order, order++);
            }
        }

        public bool IsStillUnregistered(Type type) {
            return !this._context.TestsByType.ContainsKey(type);
        }
    }
}