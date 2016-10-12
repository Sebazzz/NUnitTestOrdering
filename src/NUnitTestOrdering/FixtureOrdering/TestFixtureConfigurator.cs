// ******************************************************************************
//  © 2016 Sebastiaan Dammann - damsteen.nl
// 
//  File:           : TestFixtureConfigurator.cs
//  Project         : NUnitTestOrdering
// ******************************************************************************

namespace NUnitTestOrdering.FixtureOrdering {
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Linq.Expressions;

    using Common;

    using Internal;

    using NUnit.Framework.Interfaces;
    using NUnit.Framework.Internal;

    /// <summary>
    /// Represents a way to configure using specific methods of a test fixture
    /// </summary>
    public sealed class TestFixtureConfigurator<T> : ITestFixtureConfigurator {
        private readonly HashSet<string> _testNames;
        private TestFixture _testFixture;

        internal TestFixtureConfigurator() {
            this._testNames = new HashSet<string>(StringComparer.Ordinal);
        }

        internal TestFixture TestFixture {
            get { return this._testFixture; }
            set {
                this._testFixture = value;

                if (this._testFixture != null) {
                    SetBaseLine(this._testFixture);
                }
            }
        }

        /// <summary>
        /// Specifies the specified test method should run in the current order opposed to the other test methods
        /// </summary>
        /// <param name="name"></param>
        public TestFixtureConfigurator<T> TestMethod(string name) {
            Debug.Assert(this.TestFixture!=null);

            Test method = this.TestFixture.Tests.Cast<Test>().FirstOrDefault(x => x.Name == name);
            if (method == null) {
                throw new InvalidOperationException($"Unable to find test method: {name}");
            }

            if (!this._testNames.Add(name)) {
                throw new TestOrderingException($"Test {name} was already specified in the ordering list");
            }

            method.Properties.Set(PropertyNames.Order, this._testNames.Count);

            return this;
        }

        private static void SetBaseLine(TestFixture fixture) {
            // Any tests which won't have an order specified will run first
            foreach (ITest test in fixture.Tests) {
                test.Properties.Set(PropertyNames.Order, -1);
            }
        }

        internal void Configure(TestFixture test) => ((ITestFixtureConfigurator) this).Configure(test);

        void ITestFixtureConfigurator.Configure(TestFixture test) {
            Debug.Assert(this.TestFixture != null);

            // Remove redundant tests
            foreach (ITest childTest in this.TestFixture.Tests.Where(x => (int) x.Properties.Get(PropertyNames.Order) == -1).ToList()) {
                this.TestFixture.Tests.Remove(childTest);
            }
        }

        /// <summary>
        /// Specifies the specified test method should run in the current order opposed to the other test methods
        /// </summary>
        /// <param name="expression"></param>
        public TestFixtureConfigurator<T> TestMethod(Expression<Action<T>> expression) {
            MethodFinder methodFinder = new MethodFinder(expression.Parameters[0]);

            try {
                methodFinder.Visit(expression.Body);
            } catch (TestOrderingException ex) {
                throw new TestOrderingException($"Unable to find method name in expression {expression}", ex);
            }

            if (String.IsNullOrEmpty(methodFinder.TestMethodName)) {
                throw new TestOrderingException($"Unable to find method name in expression {expression}");
            }

            return this.TestMethod(methodFinder.TestMethodName);
        }

        private sealed class MethodFinder : ExpressionVisitor {
            private readonly ParameterExpression _inputParameter;

            public MethodFinder(ParameterExpression inputParameter) {
                this._inputParameter = inputParameter;
            }

            /// <summary>
            /// Gets the name of the test method
            /// </summary>
            public string TestMethodName { get; private set; }

            protected override Expression VisitMethodCall(MethodCallExpression node) {
                if (node.Object != this._inputParameter) {
                    throw new TestOrderingException($"Method call {node} does not point to input parameter {this._inputParameter}");
                }

                this.TestMethodName = node.Method.Name;
                return node;
            }
        }
    }
}
