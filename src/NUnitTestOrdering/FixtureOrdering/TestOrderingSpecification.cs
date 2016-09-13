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

    using Common;

    using Internal;

    /// <summary>
    /// Base class for classes which specify 
    /// </summary>
    public abstract class TestOrderingSpecification {
        private readonly List<IOrderedTestPart> _parts;

        protected TestOrderingSpecification() {
            this._parts = new List<IOrderedTestPart>();
        }

        /// <summary>
        /// In a derived class, implement this to define the tests to run. Specify this by calling the
        /// <see cref="TestFixture{T}"/> and <see cref="OrderedTestSpecification{T}"/> method
        /// </summary>
        protected abstract void DefineTestOrdering();

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

        internal static ICollection<IOrderedTestPart> RunSpecification(Type type) {
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

            return spec.GetTestParts();
        }
    }
}