// ******************************************************************************
//  © 2016 Sebastiaan Dammann - damsteen.nl
// 
//  File:           : OrderedTestSpecificationFixture.cs
//  Project         : NUnitTestOrdering
// ******************************************************************************

namespace NUnitTestOrdering.FixtureOrdering.Internal {
    using NUnit.Framework.Interfaces;
    using NUnit.Framework.Internal;

    internal sealed class OrderedTestSpecificationFixture : TestFixture {
        public OrderedTestSpecificationFixture(ITypeInfo fixtureType) : base(fixtureType) {
            this.MaintainTestOrder = true;
        }

        public bool ContinueOnError { get; internal set; }
    }
}
