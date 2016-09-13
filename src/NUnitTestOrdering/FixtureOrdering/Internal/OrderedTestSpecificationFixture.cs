namespace NUnitTestOrdering.FixtureOrdering.Internal {
    using NUnit.Framework.Interfaces;
    using NUnit.Framework.Internal;

    internal sealed class OrderedTestSpecificationFixture : TestFixture {
        public OrderedTestSpecificationFixture(ITypeInfo fixtureType) : base(fixtureType) {
            this.MaintainTestOrder = true;
        }
    }
}