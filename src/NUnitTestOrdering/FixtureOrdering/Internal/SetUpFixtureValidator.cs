namespace NUnitTestOrdering.FixtureOrdering.Internal {
    using System;
    using System.Diagnostics;
    using System.Linq;

    using Common;

    using NUnit.Framework.Interfaces;
    using NUnit.Framework.Internal;

    internal static class SetUpFixtureValidator {
        public static void ValidateGlobalSetUpFixture(SetUpFixture setUpFixture) {
            if (setUpFixture == null) throw new ArgumentNullException(nameof(setUpFixture));

            ITypeInfo typeInfo = setUpFixture.TypeInfo;
            Debug.Assert(typeInfo != null);
            Debug.Assert(typeInfo.Namespace == null);

            if (!typeInfo.GetCustomAttributes<OrderedTestGlobalSetUpFixtureAttribute>(false).Any()) {
                throw new TestOrderingException($"You are using a global SetUpFixture ({typeInfo.FullName}) in your tests. Please apply the {typeof(OrderedTestGlobalSetUpFixtureAttribute)} to it to ensure your ordered tests will execute in the correct order.");
            }
        }
    }
}
