// ******************************************************************************
//  © 2016 Sebastiaan Dammann - damsteen.nl
// 
//  File:           : Attributes.cs
//  Project         : NUnitTestOrdering
// ******************************************************************************
namespace NUnitTestOrdering.MethodOrdering {
    using System;
    using NUnit.Framework;
    using NUnit.Framework.Interfaces;
    using NUnit.Framework.Internal;

    /// <summary>
    /// Needs to be applied to method which don't have a dependency, but other methods have a dependency on
    /// </summary>
    public sealed class TestMethodWithoutDependencyAttribute : NUnitAttribute, IApplyToTest {
        /// <inheritdoc />
        public void ApplyToTest(Test test) {
            if (!test.Properties.ContainsKey(PropertyNames.Order)) {
                // NUnit sorts tests without an order set to be executed last and we want those tests to execute first.
                test.Properties.Set(PropertyNames.Order, 0);
            }
        }
    }

    /// <summary>
    ///     Defines the order that the test will run in
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class TestMethodDependencyAttribute : NUnitAttribute, IApplyToTest {
        /// <inheritdoc />
        public TestMethodDependencyAttribute(string methodDependency) {
            this.MethodDependency = methodDependency;
        }

        /// <summary>
        /// Specified the method name the current test method is dependent on
        /// </summary>
        public string MethodDependency { get; set; }

        /// <summary>
        ///     Modifies a test as defined for the specific attribute.
        /// </summary>
        /// <param name="test">The test to modify</param>
        public void ApplyToTest(Test test) {
            if (!test.Properties.ContainsKey(PropertyNames.Order)) {
                test.Properties.Set(PropertyNames.Order, TestMethodDependencyChainer.Instance.GetOrder(test));
            }
        }
    }
}
