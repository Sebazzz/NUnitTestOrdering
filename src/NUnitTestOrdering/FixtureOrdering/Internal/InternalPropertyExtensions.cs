namespace NUnitTestOrdering.FixtureOrdering.Internal {
    using System;

    internal static class InternalPropertyExtensions {
        public static bool HasFailed(this OrderedTestSpecificationFixture spec) {
            if (spec == null) throw new ArgumentNullException(nameof(spec));

            return spec.Properties.ContainsKey(InternalPropertyNames.HasFailed) &&
                   (bool) spec.Properties.Get(InternalPropertyNames.HasFailed);
        }

        public static void MarkAsFailed(this OrderedTestSpecificationFixture spec, string failingTest) {
            if (spec == null) throw new ArgumentNullException(nameof(spec));
            
            spec.Properties.Set(InternalPropertyNames.HasFailed, true);

            if (!spec.Properties.ContainsKey(InternalPropertyNames.FailReason)) {
                spec.Properties.Set(InternalPropertyNames.FailReason, $"Test {failingTest} has failed");
            }
        }

        public static string GetFailReason(this OrderedTestSpecificationFixture spec) {
            return (string) spec.Properties.Get(InternalPropertyNames.FailReason);
        }
    }
}