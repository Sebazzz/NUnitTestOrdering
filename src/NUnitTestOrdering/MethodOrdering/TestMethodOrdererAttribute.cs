namespace NUnitTestOrdering.MethodOrdering {
    using System;

    using FixtureOrdering.Internal;

    using NUnit.Framework.Interfaces;
    using NUnit.Framework.Internal;

    /// <summary>
    ///     Registers the specified test method orderer with this fixture
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class TestMethodOrdererAttribute : Attribute, IApplyToTest {
        /// <inheritdoc />
        public TestMethodOrdererAttribute(Type testOrdererType) {
            this.TestOrdererType = testOrdererType;
        }

        /// <summary>
        /// Specifies which type, derived from <see cref="TestMethodOrdererAttribute"/> will apply ordering
        /// </summary>
        public Type TestOrdererType { get; }

        /// <summary>
        /// Applies ordering to the specified test
        /// </summary>
        /// <param name="test"></param>
        public void ApplyToTest(Test test) {
            test.Properties.Set(InternalPropertyNames.MethodOrdererType, this.TestOrdererType);
        }
    }
}