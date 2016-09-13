namespace NUnitTestOrdering.FixtureOrdering.Internal {
    using Common;

    using NUnit.Framework.Internal;

    internal sealed class TestInfo {
        public Test Test { get; }

        public Test PartOf { get; private set; }

        public TestInfo(Test test) {
            this.Test = test;
        }

        public void SetPartOf(Test parent) {
            this.EnsureNotPartOfTest();

            this.PartOf = parent;
        }

        private void EnsureNotPartOfTest() {
            if (this.PartOf != null) {
                throw new TestOrderingException($"Test {this.Test.FullName} is already part of test {this.PartOf.FullName}");
            }
        }
    }
}