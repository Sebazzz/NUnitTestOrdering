// ******************************************************************************
//  © 2016 Sebastiaan Dammann - damsteen.nl
// 
//  File:           : TestHierarchyUtility.cs
//  Project         : NUnitTestOrdering
// ******************************************************************************

namespace NUnitTestOrdering.FixtureOrdering.Internal {
    using NUnit.Framework.Internal;

    internal static class TestHierarchyUtility {
        public static void RemoveEmptyNodes(Test root) {
            int index = 0;
            while (index < root.Tests.Count) {
                Test current = (Test) root.Tests[index];

                if (current is TestFixture) {
                    // skip
                    index++;
                    continue;
                }

                RemoveEmptyNodes(current);

                if (!current.HasChildren) {
                    root.Tests.RemoveAt(index);
                } else {
                    index++;
                }
            }
        }
    }
}
