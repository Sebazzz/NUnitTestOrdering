// ******************************************************************************
//  © 2016 Sebastiaan Dammann - damsteen.nl
// 
//  File:           : FullNameAssigner.cs
//  Project         : NUnitTestOrdering
// ******************************************************************************

namespace NUnitTestOrdering.FixtureOrdering.Internal {
    using System.Linq;

    using NUnit.Framework.Internal;

    /// <summary>
    /// Helper class to assign a unique full name to each child test
    /// </summary>
    internal static class FullNameAssigner {
        public static void AssignFullName(Test test) {
            string fullName = test.FullName;

            AssignFullNameToChildTests(test, fullName);
        }

        private static void AssignFullNameToChildTests(Test test, string parentName) {
            foreach (Test childTest in test.Tests.Cast<Test>()) {
                childTest.FullName = parentName + "." + childTest.Name;

                AssignFullNameToChildTests(childTest, childTest.FullName);
            }
        }
    }
}
