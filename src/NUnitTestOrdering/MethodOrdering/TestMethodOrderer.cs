// ******************************************************************************
//  © 2016 Sebastiaan Dammann - damsteen.nl
// 
//  File:           : TestMethodOrderer.cs
//  Project         : NUnitTestOrdering
// ******************************************************************************

namespace NUnitTestOrdering.MethodOrdering {
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Common;

    using NUnit.Framework.Interfaces;
    using NUnit.Framework.Internal;

    /// <summary>
    ///     Base class to allow influencing the way certain tests within a fixture are executed. 
    /// </summary>
    public abstract class TestOrderer {
        private TestFixture _testFixture;

        private readonly HashSet<string> _testNames;

        /// <inheritdoc />
        protected TestOrderer() {
            this._testNames = new HashSet<string>(StringComparer.Ordinal);
        }

        /// <summary>
        /// When implemented in a derived class, defines the ordering of tests by calling <see cref="TestMethod"/>
        /// </summary>
        protected abstract void DefineOrdering();

        /// <summary>
        /// Applies ordering to the specified test fixture
        /// </summary>
        /// <param name="fixture"></param>
        internal void ApplyOrdering(TestFixture fixture) {
            SetBaseLine(fixture);

            try {
                this._testFixture = fixture;

                this.DefineOrdering();
            }
            finally {
                this._testFixture = null;
                this._testNames.Clear();
            }
        }

        private static void SetBaseLine(TestFixture fixture) {
            // Any tests which won't have an order specified will run first
            foreach (ITest test in fixture.Tests) {
                test.Properties.Set(PropertyNames.Order, -1);
            }
        }

        /// <summary>
        /// Specifies the specified test method should run in the current order opposed to the other test methods
        /// </summary>
        /// <param name="name"></param>
        protected void TestMethod(string name) {
            Test method = this._testFixture.Tests.Cast<Test>().FirstOrDefault(x => x.Name == name);
            if (method == null) {
                throw new InvalidOperationException($"Unable to find test method: {name}");
            }

            if (!this._testNames.Add(name)) {
                throw new TestOrderingException($"Test {name} was already specified in the ordering list");
            }

            method.Properties.Set(PropertyNames.Order, this._testNames.Count);
        }
    }
}
