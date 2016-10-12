// ******************************************************************************
//  © 2016 Sebastiaan Dammann - damsteen.nl
// 
//  File:           : ConfiguredTestFixtureTestPart.cs
//  Project         : NUnitTestOrdering
// ******************************************************************************

namespace NUnitTestOrdering.FixtureOrdering.Internal {
    using System;

    using Common;

    using NUnit.Framework.Interfaces;
    using NUnit.Framework.Internal;

    /// <summary>
    /// Configures a test fixture to run using a specific ordering configuration
    /// </summary>
    internal sealed class ConfiguredTestFixtureTestPart : IOrderedTestPart {
        private readonly Type _type;
        private readonly ITestFixtureConfigurator _testFixtureConfigurator;

        public ConfiguredTestFixtureTestPart(Type type, ITestFixtureConfigurator testFixtureConfigurator) {
            this._type = type;
            this._testFixtureConfigurator = testFixtureConfigurator;
        }

        public Test GetTest(TestHierarchyContext context) {
            Test theTest = context.TestRepository.Get(this._type);

            if (theTest == null) {
                throw new TestOrderingException($"Unable to find type of fixture type {this._type.FullName}");
            }

            // Configure
            TestFixture fixtureTest = (TestFixture)theTest;
            this._testFixtureConfigurator.Configure(fixtureTest);

            // Do some nice sorting
            ReflectionUtil.SetMaintainTestOrder(fixtureTest);
            ReflectionUtil.SortByOrder(fixtureTest);

            return theTest;
        }
    }

    internal interface ITestFixtureConfigurator {
        void Configure(TestFixture test);
    }

    internal sealed class DelegateTestFixtureConfigurator<T> : ITestFixtureConfigurator {
        private readonly Action<TestFixtureConfigurator<T>> _action;

        public DelegateTestFixtureConfigurator(Action<TestFixtureConfigurator<T>> action) {
            this._action = action;
        }

        public void Configure(TestFixture test) {
            TestFixtureConfigurator<T> testFixtureConfigurator = new TestFixtureConfigurator<T>();

            try {
                testFixtureConfigurator.TestFixture = test;

                this._action.Invoke(testFixtureConfigurator);
                testFixtureConfigurator.Configure(test);
            }
            catch (Exception ex) {
                test.RunState = RunState.NotRunnable;
                test.Properties.Set(PropertyNames.SkipReason, "Exception during running of test fixture configurator: " + ex);
            }
            finally {
                testFixtureConfigurator.TestFixture = null;
            }
        }
    }
}