namespace NUnitTestOrdering.MethodOrdering {
    using System;
    using System.Linq.Expressions;

    using Common;

    /// <summary>
    ///    Base class to allow influencing the way certain tests within a fixture are executed. Allows specifying
    /// the test method using an LINQ expression, useful if you're forced to use an C# 5 compiler
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class TestOrderer<T> : TestOrderer {
        /// <summary>
        /// Specifies the specified test method should run in the current order opposed to the other test methods
        /// </summary>
        /// <param name="expression"></param>
        protected void TestMethod(Expression<Action<T>> expression) {
            MethodFinder methodFinder = new MethodFinder(expression.Parameters[0]);

            try {
                methodFinder.Visit(expression.Body);
            }
            catch (TestOrderingException ex) {
                throw new TestOrderingException($"Unable to find method name in expression {expression}", ex);
            }

            if (String.IsNullOrEmpty(methodFinder.TestMethodName)) {
                throw new TestOrderingException($"Unable to find method name in expression {expression}");
            }

            this.TestMethod(methodFinder.TestMethodName);
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