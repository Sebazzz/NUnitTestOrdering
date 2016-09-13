namespace NUnitTestOrdering.FixtureOrdering.Internal {
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    internal static class TestOrderingSpecificationDiscoverer {
        public static IEnumerable<Type> GetFromAssembly(Assembly assembly) {
            Type specType = typeof(TestOrderingSpecification);

            foreach (Type type in assembly.GetExportedTypes()) {
                if (!specType.IsAssignableFrom(type) || type.IsAbstract) continue;

                bool hasAttribute = type.GetCustomAttribute<OrderedTestFixtureAttribute>() != null;
                if (hasAttribute) yield return type;
            }
        }
    }
}