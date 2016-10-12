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

    /// <summary>
    /// Configures a test fixture of the specified type to run. The full test fixture is run in whatever order specified.
    /// </summary>
    [DebuggerDisplay("{_type}")]
    internal sealed class TestFixtureTestPart : IOrderedTestPart {
        private readonly Type _type;

        public TestFixtureTestPart(Type type) {
            this._type = type;
        }

        public Test GetTest(TestHierarchyContext context) {
            Test theTest = context.TestRepository.Get(this._type);

            if (theTest == null) {
                throw new TestOrderingException($"Unable to find type of fixture type {this._type.FullName}");
            }

            // Do some nice sorting
            TestFixture fixtureTest = (TestFixture) theTest;
            ReflectionUtil.SetMaintainTestOrder(fixtureTest);
            ReflectionUtil.SortByOrder(fixtureTest);

            return theTest;
        }
    }
}