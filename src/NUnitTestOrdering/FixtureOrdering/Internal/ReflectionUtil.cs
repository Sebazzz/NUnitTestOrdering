// ******************************************************************************
//  © 2016 Sebastiaan Dammann - damsteen.nl
// 
//  File:           : ReflectionUtil.cs
//  Project         : NUnitTestOrdering
// ******************************************************************************

namespace NUnitTestOrdering.FixtureOrdering.Internal {
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    using NUnit.Framework.Interfaces;
    using NUnit.Framework.Internal;

    internal static class ReflectionUtil {
        public static void SetMaintainTestOrder(TestSuite testSuite) {
            if (testSuite == null) throw new ArgumentNullException(nameof(testSuite));
            const string propertyName = "MaintainTestOrder";

            PropertyInfo prop = testSuite.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.NonPublic);
            if (prop == null) {
                throw new InvalidOperationException($"Unable to find property {propertyName} in type {testSuite.GetType()}");
            }

            prop.SetValue(testSuite, true);
        }

        public static void SortByOrder(Test test) {
            test.Tests.Sort(new TestByOrderComparer());
        }

        private sealed class TestByOrderComparer : IComparer<ITest> {
            public int Compare(ITest x, ITest y) {
                int? leftOrder = x?.Properties.Get(PropertyNames.Order) as int?;
                int? rightOrder = y?.Properties.Get(PropertyNames.Order) as int?;

                if (leftOrder == null) {
                    if (rightOrder == null) {
                        return StringComparer.Ordinal.Compare(x?.FullName, y?.FullName);
                    }

                    return -1; // x < y
                }

                if (rightOrder == null) {
                    return 1; // x > y
                }

                return leftOrder.Value - rightOrder.Value;
            }
        }
    }

    internal static class ListExtensions {
        public static void Sort<T>(this IList<T> list, IComparer<T> comparison) {
            List<T> listClass = list as List<T>;

            if (listClass != null) {
                listClass.Sort(comparison);
            } else {
                List<T> copy = new List<T>(list);
                copy.Sort(comparison);
                Copy(copy, 0, list, 0, list.Count);
            }
        }

        private static void Copy<T>(IList<T> sourceList, int sourceIndex, IList<T> destinationList, int destinationIndex, int count) {
            for (int i = 0; i < count; i++) {
                destinationList[destinationIndex + i] = sourceList[sourceIndex + i];
            }
        }
    }
}
