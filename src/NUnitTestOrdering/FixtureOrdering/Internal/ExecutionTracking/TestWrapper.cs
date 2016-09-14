namespace NUnitTestOrdering.FixtureOrdering.Internal.ExecutionTracking {
    using System;
    using System.Runtime.InteropServices;

    using NUnit.Framework.Interfaces;

    /// <summary>
    /// An <see cref="ITest"/> doesn't necessarily implement good hashcode behavior, so
    /// we use this helper struct to stuff it into a dictionary
    /// </summary>
    [StructLayout(LayoutKind.Auto)]
    internal struct TestWrapper : IEquatable<TestWrapper> {
        private readonly ITest _internalTest;
        private readonly string _fullName;

        public TestWrapper(ITest internalTest) : this() {
            this._internalTest = internalTest;
            this._fullName = internalTest?.FullName;
        }

        public bool Equals(TestWrapper other) {
            return Equals(this._internalTest, other._internalTest) && string.Equals(this._fullName, other._fullName, StringComparison.Ordinal);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            return obj is TestWrapper && this.Equals((TestWrapper) obj);
        }

        public override int GetHashCode() {
            return this._fullName?.GetHashCode() ?? 0;
        }
    }
}