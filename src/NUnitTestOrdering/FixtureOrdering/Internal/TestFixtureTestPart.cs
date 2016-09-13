// ******************************************************************************
//  © 2016 Sebastiaan Dammann - damsteen.nl
// 
//  File:           : TestFixtureTestPart.cs
//  Project         : NUnitTestOrdering
// ******************************************************************************
namespace NUnitTestOrdering.FixtureOrdering.Internal {
    using System;
    using System.Diagnostics;

    using Common;

    using NUnit.Framework.Internal;

    [DebuggerDisplay("{_type}")]
    internal sealed class TestFixtureTestPart : IOrderedTestPart {
        private readonly Type _type;

        public TestFixtureTestPart(Type type) {
            this._type = type;
        }

        public Test GetTest(TestAssemblyOrderingContext context) {
            Test theTest = context.FindAndRemove(this._type);

            if (theTest == null) {
                throw new TestOrderingException($"Unable to find type of fixture type {this._type.FullName}");
            }

            TestInfo testInfo = context.TestsByType[this._type];
            if (testInfo == null) {
                testInfo = new TestInfo(theTest);
                context.TestsByType[this._type] = testInfo;
            }

            return theTest;
        }
    }
}