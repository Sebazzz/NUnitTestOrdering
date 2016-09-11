namespace NUnitTestOrdering.Tests.Common {
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    using NUnit.Framework;

    using NUnitTestOrdering.Common;

    using Support;

    [TestFixture]
    [SuppressMessage("ReSharper", "ReturnValueOfPureMethodIsNotUsed", Justification = "Dependency Sorter is lazy")]
    public sealed class DependencyValidatorTests {
        [Test]
        public void Throws_UnknownDependency() {
            // Given
            FakeDependency[] input = new[] {
                new FakeDependency(1, 999),
                new FakeDependency(2, 1),
            };

            // When
            TestDelegate action = () => DependencySorter.Sort(input).ToArray();

            // Then
            Assert.That(action, Throws.TypeOf<TestOrderingException>().And.Message.StartWith("Broken dependency"));
        }

        [Test]
        public void Throws_CyclicDependency() {
            // Given
            FakeDependency[] input = new[] {
                new FakeDependency(1, 2),
                new FakeDependency(2, 1),
            };

            // When
            TestDelegate action = () => DependencySorter.Sort(input).ToArray();

            // Then
            Assert.That(action, Throws.TypeOf<TestOrderingException>().And.Message.StartWith("Cyclic dependency"));
        }
    }
}