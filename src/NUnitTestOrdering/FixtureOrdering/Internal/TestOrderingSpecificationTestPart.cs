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

        public Test GetTest(ITestAssemblyOrderingContext context) {
            TestInfo existingTestInfo = context.TestsByType[this._type];

            return existingTestInfo != null ? OrderedTestFixtureExists(existingTestInfo) : this.UnregisteredOrderedTestFixture(context);
        }

        private static Test OrderedTestFixtureExists(TestInfo existingTestInfo) {
            return existingTestInfo.Test;
        }

        private Test UnregisteredOrderedTestFixture(ITestAssemblyOrderingContext context) {
            OrderedTestSpecificationFixtureBuilder builder = new OrderedTestSpecificationFixtureBuilder(context);
            OrderedTestSpecificationFixture test = builder.BuildFrom(this._type);

            return test;
        }
    }
}
