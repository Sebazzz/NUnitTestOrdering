namespace NUnitTestOrdering.FixtureOrdering.Internal {
    using NUnit.Framework.Internal;

    internal sealed class OrderedTestRootSuite : TestSuite {
        public OrderedTestRootSuite(string name) : base(name) {}
    }
}