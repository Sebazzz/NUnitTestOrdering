// ******************************************************************************
//  © 2016 Sebastiaan Dammann - damsteen.nl
// 
//  File:           : TestMethodOrdererApplier.cs
//  Project         : NUnitTestOrdering
// ******************************************************************************

namespace NUnitTestOrdering.MethodOrdering.Internal {
    using System;

    using FixtureOrdering.Internal;

    using NUnit.Framework.Interfaces;
    using NUnit.Framework.Internal;

    internal static class TestMethodOrdererApplier {
        public static void DoOrderTestMethodsRecursive(ITest rootTest) {
            foreach (ITest test in rootTest.Tests) {
                TestFixture testFixture = test as TestFixture;

                if (testFixture != null) {
                    DoOrderTestMethods(testFixture);
                } else {
                    DoOrderTestMethodsRecursive(test);
                }
            }
        }

        private static void DoOrderTestMethods(TestFixture testFixture) {
            if (!testFixture.Properties.ContainsKey(InternalPropertyNames.MethodOrdererType)) {
                return;
            }

            Type testOrdererType = (Type) testFixture.Properties.Get(InternalPropertyNames.MethodOrdererType);

            try {
                TestOrderer testOrderer = (TestOrderer) Activator.CreateInstance(testOrdererType);

                testOrderer.ApplyOrdering(testFixture);
            }
            catch (Exception ex) {
                testFixture.RunState = RunState.NotRunnable;
                testFixture.Properties.Set(PropertyNames.SkipReason, $"Unable to apply test method ordering: {ex}");
            }
        }
    }
}
