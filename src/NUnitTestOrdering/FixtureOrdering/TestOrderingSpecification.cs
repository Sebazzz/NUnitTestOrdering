// ******************************************************************************
//  © 2016 Sebastiaan Dammann - damsteen.nl
// 
//  File:           : TestOrderingSpecification.cs
//  Project         : NUnitTestOrdering
// ******************************************************************************
namespace NUnitTestOrdering.FixtureOrdering {
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Runtime.InteropServices;

    using Common;

    using Internal;

    /// <summary>
    /// Defines an order to run tests in
    /// </summary>
    public abstract class TestOrderingSpecification {
        private readonly List<IOrderedTestPart> _parts;

        /// <inheritdoc />
        protected TestOrderingSpecification() {
            this._parts = new List<IOrderedTestPart>();
        }

        /// <summary>
        /// In a derived class, implement this to define the tests to run. Specify this by calling the
        /// <see cref="TestFixture{T}()"/> and <see cref="OrderedTestSpecification{T}"/> method
        /// </summary>
        protected abstract void DefineTestOrdering();

        /// <summary>
        /// Gets whether to continue test execution of this fixture when a child test fixture fails. See remarks.
        /// </summary>
        /// <remarks>
        /// <para>
        ///     When this property returns <c>false</c>: Execution of this test fixture will never stop, though still marked as failed.
        /// </para>
        /// <para>
        ///     When this property returns <c>true</c>:
        ///     - When a child test fixture fails, this ordered fixture will be marked as failed and further test execution will be stopped
        ///     - When a child test ordered test fails, and the child ordered test has <see cref="ContinueOnError"/> set to true,
        ///       this ordered test fixture will be marked as failed and further test execution will be stopped.
        /// </para>
        /// </remarks>
        protected abstract bool ContinueOnError { get; }

        /// <summary>
        /// Run the specified test fixture
        /// </summary>
        /// <typeparam name="T"></typeparam>
        protected void TestFixture<T>() where T : class {
            this._parts.Add(new TestFixtureTestPart(typeof(T)));
        }

        /// <summary>
        /// Run the specified ordered test specification
        /// </summary>
        /// <typeparam name="T"></typeparam>
        protected void OrderedTestSpecification<T>() where T : TestOrderingSpecification {
            this._parts.Add(new TestOrderingSpecificationTestPart(typeof(T)));
        }

        /// <summary>
        /// Runs the specified ordered test fixture using a specific config
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configureTestFixture">Allows configuring which test methods to run. Test methods not specified will not run.</param>
        protected void TestFixture<T>(Action<TestFixtureConfigurator<T>> configureTestFixture) {
            if (configureTestFixture == null) throw new ArgumentNullException(nameof(configureTestFixture));

            ITestFixtureConfigurator configurator = new DelegateTestFixtureConfigurator<T>(configureTestFixture);

            this._parts.Add(new ConfiguredTestFixtureTestPart(typeof(T), configurator));
        }

        internal ICollection<IOrderedTestPart> GetTestParts() {
            this._parts.Clear();

            try {
                this.DefineTestOrdering();
            }
            catch (Exception ex) {
                throw new TestOrderingException($"{this.GetType().FullName}: Unable to define test ordering", ex);
            }

            return this._parts;
        }

        internal static TestSpecificationInfo RunSpecification(Type type) {
            TestOrderingSpecification spec;

            try {
                spec = (TestOrderingSpecification) Activator.CreateInstance(type);
            }
            catch (InvalidCastException) {
                throw new TestOrderingException($"Type {type.FullName} does not inherit from {typeof(TestOrderingSpecification).FullName}");
            }
            catch (TargetInvocationException ex) {
                throw new TestOrderingException($"Unable to instantiate test ordering specification {type.FullName}", ex.InnerException);
            }
            catch (Exception ex) {
                throw new TestOrderingException($"Unable to instantiate test ordering specification {type.FullName}", ex);
            }

            return new TestSpecificationInfo(spec.ContinueOnError, spec.GetTestParts());
        }
    }

    [StructLayout(LayoutKind.Auto)]
    internal struct TestSpecificationInfo {
        public ICollection<IOrderedTestPart> Parts { get; }
        public bool ContinueOnError { get; }

        public TestSpecificationInfo(bool continueOnError, ICollection<IOrderedTestPart> parts) {
            this.ContinueOnError = continueOnError;
            this.Parts = parts;
        }
    }
}