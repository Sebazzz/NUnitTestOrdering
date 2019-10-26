namespace NUnitTestOrdering.Tests.Common {
    using System.Linq;

    using NUnit.Framework;
    using NUnit.Framework.Constraints;

    using NUnitTestOrdering.Common;

    using Support;

    [TestFixture]
    public sealed class DependencySorterTests {
        [Test]
        public void Simple_TwoOrdering() {
            // Given
            FakeDependency[] input = {
                new FakeDependency(1, 0),
                new FakeDependency(2, 1),
                new FakeDependency(3, 2),
            };

            // When
            FakeDependency[] result = DependencySorter.Sort(input).ToArray();

            // Then
            VerifyResults(result, 1, 2, 3);
        }

        [Test]
        public void Simple_TwoDependency() {
            // Given
            FakeDependency[] input = {
                new FakeDependency(1, 0),
                new FakeDependency(2, 1),
                new FakeDependency(3, 2),
                new FakeDependency(4, 2),
            };

            // When
            FakeDependency[] result = DependencySorter.Sort(input).ToArray();

            // Then
            VerifyAnyResults(result, new[] { 1, 2, 4, 3 }, new[] { 1, 2, 3, 4 });
        }

        [Test]
        public void NoOrder() {
            // Given
            FakeDependency[] input = {
                new FakeDependency(1, 0),
                new FakeDependency(2, 0),
                new FakeDependency(3, 1),
                new FakeDependency(4, 3),
            };

            // When
            FakeDependency[] result = DependencySorter.Sort(input).ToArray();

            // Then
            VerifyAnyResults(result, new[] { 1, 3, 4, 2 }, new[] { 2, 1, 3, 4 });
        }

        [Test]
        public void SameDependency() {
            // Given
            FakeDependency[] input = {
                new FakeDependency(1, 0),
                new FakeDependency(2, 1),
                new FakeDependency(3, 1),
            };

            // When
            FakeDependency[] result = DependencySorter.Sort(input).ToArray();

            // Then
            VerifyAnyResults(result, new[] { 1, 2, 3 }, new [] { 1, 3, 2 });
        }

        [Test]
        public void RandomOrdered() {
            // Given
            FakeDependency[] input = {
                new FakeDependency(1, 0),
                new FakeDependency(4, 3),
                new FakeDependency(2, 1),
                new FakeDependency(3, 2),
            };

            // When
            FakeDependency[] result = DependencySorter.Sort(input).ToArray();

            // Then
            VerifyResults(result, 1, 2, 3, 4);
        }

        [Test]
        public void ReverseOrdered() {
            // Given
            FakeDependency[] input = {
                new FakeDependency(4, 3),
                new FakeDependency(3, 2),
                new FakeDependency(2, 1),
                new FakeDependency(1, 0),
            };

            // When
            FakeDependency[] result = DependencySorter.Sort(input).ToArray();

            // Then
            VerifyResults(result, 1, 2, 3, 4);
        }

        private static void VerifyResults(FakeDependency[] result, params int[] ids) {
            Assert.That(result.Select(x => x.Id).ToArray(), Is.EqualTo(ids));
        }

        private static void VerifyAnyResults(FakeDependency[] result, params int[][] variants) {
            Constraint constraint = Is.Null;

            foreach (int[] variant in variants) {
                constraint = constraint.Or.EqualTo(variant);
            }

            Assert.That(result.Select(x => x.Id).ToArray(), constraint);
        }
    }
}
