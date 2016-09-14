namespace NUnitTestOrdering.FixtureOrdering.Internal.ExecutionTracking {
    using System.Collections.Generic;

    using NUnit.Framework.Interfaces;
    using NUnit.Framework.Internal;

    internal sealed class TestExecutionTrackingContext {
        private readonly Dictionary<TestWrapper, OrderedTestSpecificationFixture> _fixtureCache;

        public TestExecutionTrackingContext() {
            this._fixtureCache = new Dictionary<TestWrapper, OrderedTestSpecificationFixture>();
        }

        /// <summary>
        /// Find an ordered specification based on the specified test
        /// </summary>
        /// <remarks>
        /// Internally uses a dictionary. This method is called before and after each test, so
        /// it doesn't hurt to have a little performance here. We cache both succesfull and failed lookups.
        /// </remarks>
        /// <param name="test"></param>
        /// <returns></returns>
        public OrderedTestSpecificationFixture FindOrderedSpec(Test test) {
            OrderedTestSpecificationFixture foundFixture;

            ITest parentTest = test.Parent;
            if (parentTest == null) {
                return null;
            }

            if (this._fixtureCache.TryGetValue(new TestWrapper(parentTest), out foundFixture)) {
                return foundFixture;
            }

            return this.FindRecursive(parentTest);
        }

        private OrderedTestSpecificationFixture FindRecursive(ITest test) {
            OrderedTestSpecificationFixture spec = test as OrderedTestSpecificationFixture;

            if (spec == null) {
                ITest parentTest = test.Parent;

                if (parentTest == null) {
                    return null;
                }

                if (this._fixtureCache.TryGetValue(new TestWrapper(parentTest), out spec)) {
                    return spec;
                }

                spec = this.FindRecursive(parentTest);
                this._fixtureCache[new TestWrapper(test)] = spec;
            }

            return spec;
        }
    }
}