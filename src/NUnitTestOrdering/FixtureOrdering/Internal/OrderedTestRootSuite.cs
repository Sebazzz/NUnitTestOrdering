namespace NUnitTestOrdering.FixtureOrdering.Internal {
    using System;

    using NUnit.Framework.Interfaces;
    using NUnit.Framework.Internal;

    internal sealed class OrderedTestRootSuite : TestSuite {
        public OrderedTestRootSuite(string name) : base(new TypeWrapper(typeof(SentinelType))) {
            this.Name = name;
            this.FullName = name;
        }
    }

    /// <summary>
    /// This type supports the ordered testing infrastructure. Do not use.
    /// </summary>
    /// <remarks>
    /// This type exists so that at the start of ordered testing. TODO: can't figure this out, for now, require users to apply OrderedTestGlobalSetUpFixtureAttribute to global SetupFixture
    /// </remarks>
    [OrderedTestGlobalSetUpFixtureAttribute]
    public sealed class SentinelType {}

    /// <summary>
    /// This attribute need to run once ordered testing begins. This happens for two reasons:
    /// - We don't necessarily need ordered tests to have <see cref="TestExecutionContext.IsSingleThreaded"/> being <c>false</c>
    /// - When the test assembly uses one or more <see cref="SetUpFixture"/>, then when <see cref="IApplyToContext"/> is implemented
    ///   on an assembly-level attribute (<see cref="EnableTestOrderingAttribute"/>), it won't be called.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class OrderedTestGlobalSetUpFixtureAttribute : Attribute, IApplyToContext {
        /// <inheritdoc />
        public void ApplyToContext(TestExecutionContext context) {
            // Note of the comment below: "execute" means in NUnit terms actually "discovering and scheduling
            // work items _and_ execute them"

            // We need to force NUnit to run single threaded. In this way, NUnit will "execute" ordered test
            // fixtures immediately instead of scheduling the "execution" of the work items. By the way, in
            // ordered tests we cannot have any parallelism anyway.

            // The below statement has the consequence the *entire* test hierarchy of ordered tests won't
            // run in parallel, but if you care so much for that you might as well seperate your (integration) tests to a seperate assembly.
            context.IsSingleThreaded = true;
        }
    }
}