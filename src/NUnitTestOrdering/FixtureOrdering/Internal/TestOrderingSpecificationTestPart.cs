// ******************************************************************************
//  © 2016 Sebastiaan Dammann - damsteen.nl
// 
//  File:           : TestOrderingSpecificationTestPart.cs
//  Project         : NUnitTestOrdering
// ******************************************************************************
namespace NUnitTestOrdering.FixtureOrdering.Internal {
    using System;
    using System.Diagnostics;

    using NUnit.Framework.Internal;

    [DebuggerDisplay("{_type}")]
    internal sealed class TestOrderingSpecificationTestPart : IOrderedTestPart {
        private readonly Type _type;

        public TestOrderingSpecificationTestPart(Type type) {
            this._type = type;
        }

        public Test GetTest(TestHierarchyContext context) {
            return this.UnregisteredOrderedTestFixture(context);
        }

        private Test UnregisteredOrderedTestFixture(TestHierarchyContext context) {
            OrderedTestSpecificationFixtureBuilder builder = new OrderedTestSpecificationFixtureBuilder(context);
            OrderedTestSpecificationFixture test = builder.BuildFrom(this._type);

            return test;
        }
    }
}
