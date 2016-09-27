// ******************************************************************************
//  © 2016 Sebastiaan Dammann - damsteen.nl
// 
//  File:           : TestAssemblyOrderingContext.cs
//  Project         : NUnitTestOrdering
// ******************************************************************************

namespace NUnitTestOrdering.FixtureOrdering.Internal {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Common;

    using NUnit.Framework;
    using NUnit.Framework.Interfaces;
    using NUnit.Framework.Internal;
    using NUnit.Framework.Internal.Builders;


    internal class TestRepository {
        private readonly TestSuite _rootFixture;

        private readonly ISuiteBuilder _suiteBuilder;

        public IDictionary<Type, Test> TestsByType { get; }

        public TestRepository(TestSuite rootFixture) {
            this._rootFixture = rootFixture;

            this._suiteBuilder = new DefaultSuiteBuilder();

            this.TestsByType = new NullableDictionary<Type, Test>();
            InitializeTestsByTypeDictionary(this.TestsByType, this._rootFixture);
        }

        private static void InitializeTestsByTypeDictionary(IDictionary<Type, Test> testsByType, Test test) {
            if (test.TypeInfo != null) {
                testsByType[test.TypeInfo.Type] = test;
            }

            foreach (Test childTest in test.Tests.OfType<Test>()) {
                InitializeTestsByTypeDictionary(testsByType, childTest);
            }
        }

        public Test Get(Type type) {
            Test result = this.Get(this._rootFixture, type) ?? this.BuildNewTest(type);

            return result;
        }

        private Test BuildNewTest(Type type) {
            return this._suiteBuilder.BuildFrom(new TypeWrapper(type));
        }

        private Test Get(Test container, Type type) {
            foreach (Test test in container.Tests.Cast<Test>()) {
                if (test.TypeInfo?.Type == type) {
                    container.Tests.Remove(test);
                    return test;
                }

                Test childTest = this.Get(test, type);
                if (childTest != null) {
                    return childTest;
                }
            }

            return null;
        }

    }

    internal class TestHierarchyContext {
        private readonly HashSet<Type> _types;

        public TestRepository TestRepository { get; }

        public TestHierarchyContext(TestRepository testRepository) {
            this.TestRepository = testRepository;
            this._types = new HashSet<Type>();
        }

        public IDisposable EnterHierarchy(Type type) {
            if (!this._types.Add(type)) {
                string ancestors = String.Join("; ", this._types);

                throw new TestOrderingException($"Unable to enter scope for type {type}: Already one of the ancestors! Ancestor list is as follows: {ancestors}");
            }

            return new TestHierarchyScope(type, this._types);
        }

        private sealed class TestHierarchyScope : IDisposable {
            private readonly HashSet<Type> _parentTypes;
            private readonly Type _currentType;

            public TestHierarchyScope(Type currentType, HashSet<Type> parentTypes) {
                this._currentType = currentType;
                this._parentTypes = parentTypes;
            }

            public void Dispose() {
                this._parentTypes.Remove(this._currentType);
            }
        }
    }

    

    internal class TestFixtureOrderer {
        private readonly Assembly _testAssembly;
        private readonly TestSuite _rootFixture;

        public Test Root => this._rootFixture;

        private readonly TestHierarchyContext _testHierarchyContext;

        private readonly List<OrderedTestSpecificationFixture> _orderedTestSpecificationFixtures;

        public TestFixtureOrderer(Assembly testAssembly, TestSuite rootFixture) {
            this._testAssembly = testAssembly;
            this._rootFixture = rootFixture;

            this._testHierarchyContext = new TestHierarchyContext(new TestRepository(rootFixture));
            this._orderedTestSpecificationFixtures = new List<OrderedTestSpecificationFixture>();
        }
        

        public void OrderTests() {
            this.InitializeTestOrderingSpecs();
            this.FixTestHierarchy();
        }

        private void FixTestHierarchy() {
            this.AddUnorderedTestSuite();
            this.AddOrderedTestSuite();
            this.ShrinkHierarchy();
        }

        private void ShrinkHierarchy() {
            // If the root test is "Unordered" it will remove that test and keep hierarchy as-is
            if (this._rootFixture.Tests.Count == 1 && !(this._rootFixture.Tests[0] is OrderedTestRootSuite)) {
                foreach (ITest test in this._rootFixture.Tests[0].Tests) {
                    this._rootFixture.Tests.Add(test);
                }
                this._rootFixture.Tests.RemoveAt(0);
            }
        }

        private void AddUnorderedTestSuite() {
            TestSuite unorderedTests = new TestSuite("Unordered");
            unorderedTests.Properties.Set(PropertyNames.Order, -1000);

            foreach (Test test in this._rootFixture.Tests.Cast<Test>().ToList()) {
                this._rootFixture.Tests.Remove(test);
                unorderedTests.Add(test);
            }

            TestHierarchyUtility.RemoveEmptyNodes(unorderedTests);
            if (unorderedTests.HasChildren) {
                this._rootFixture.Tests.Add(unorderedTests);
            }
        }

        private void AddOrderedTestSuite() {
            OrderedTestRootSuite orderedTestRootSuite = new OrderedTestRootSuite("Ordered");

            // Can't afford parallelism in an ordered test
            orderedTestRootSuite.Properties.Set(PropertyNames.ParallelScope, ParallelScope.None);
            orderedTestRootSuite.Properties.Set(PropertyNames.Order, 1000);

            foreach (OrderedTestSpecificationFixture testInfo in this._orderedTestSpecificationFixtures) {
                orderedTestRootSuite.Add(testInfo);
            }

            FullNameAssigner.AssignFullName(orderedTestRootSuite);

            if (orderedTestRootSuite.HasChildren) {
                this._rootFixture.Add(orderedTestRootSuite);
            }
        }

        private void InitializeTestOrderingSpecs() {
            OrderedTestSpecificationFixtureBuilder builder = new OrderedTestSpecificationFixtureBuilder(this._testHierarchyContext);
            foreach (Type type in TestOrderingSpecificationDiscoverer.GetFromAssembly(this._testAssembly)) {
                this._orderedTestSpecificationFixtures.Add(builder.BuildFrom(type));
            }
        }
    }
}
