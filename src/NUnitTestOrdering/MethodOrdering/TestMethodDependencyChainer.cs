// ******************************************************************************
//  © 2016 Sebastiaan Dammann - damsteen.nl
// 
//  File:           : TestMethodDependencyChainer.cs
//  Project         : NUnitTestOrdering
// ******************************************************************************
namespace NUnitTestOrdering.MethodOrdering {
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using System.Reflection;

    using Common;

    using NUnit.Framework;
    using NUnit.Framework.Internal;

    internal class TestMethodDependencyChainer {
        public static TestMethodDependencyChainer Instance = new TestMethodDependencyChainer();

        private readonly List<TestCaseWrapper> _testCaseDescriptors = new List<TestCaseWrapper>();

        private TestMethodDependencyChainer() {
            var excludedMethodNames = new[] { nameof(this.GetHashCode), nameof(Equals), nameof(this.ToString), nameof(this.GetType) };
            var types =
                from assembly in AppDomain.CurrentDomain.GetAssemblies()
                from type in assembly.GetExportedTypes()
                where type.GetCustomAttribute<TestFixtureAttribute>() != null
                from method in type.GetMethods(BindingFlags.Instance | BindingFlags.Public)
                where Array.IndexOf(excludedMethodNames, method.Name) == -1
                select new TestCaseWrapper(new TestCaseDescriptor(method.Name, type));

            this._testCaseDescriptors.AddRange(DependencySorter.Sort(types));
        }

        public int GetOrder(Test test) {
            TestCaseDescriptor testCaseDescriptor = new TestCaseDescriptor(test.Method.Name, test.Method.TypeInfo.Type);

            return this._testCaseDescriptors.FindIndex(x => x.Descriptor == testCaseDescriptor);
        }

        private sealed class TestCaseDescriptor : IEquatable<TestCaseDescriptor> {
            private readonly Assembly _assembly;
            private readonly string _method;
            private readonly string _type;

            public Assembly Assembly => this._assembly;
            public string Method => this._method;
            public string Type => this._type;

            public TestCaseDescriptor(string method, Type dependendOnType) {
                this._method = method;
                this._type = dependendOnType.FullName;
                this._assembly = dependendOnType.Assembly;
            }

            public bool Equals(TestCaseDescriptor other) {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return string.Equals(this._method, other._method) && string.Equals(this._type, other._type);
            }

            public override bool Equals(object obj) {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                return obj is TestCaseDescriptor && this.Equals((TestCaseDescriptor)obj);
            }

            public override int GetHashCode() {
                unchecked {
                    return ((this._method?.GetHashCode() ?? 0) * 397) ^ (this._type?.GetHashCode() ?? 0);
                }
            }

            public static bool operator ==(TestCaseDescriptor left, TestCaseDescriptor right) {
                if (ReferenceEquals(null, left)) return ReferenceEquals(null, right);
                if (ReferenceEquals(null, right)) return false;

                return left.Equals(right);
            }

            public static bool operator !=(TestCaseDescriptor left, TestCaseDescriptor right) {
                return !(left == right);
            }

            public override string ToString() => $"{this.Type}.{this.Method}";
        }

        private sealed class TestCaseWrapper : IDependencyIndicator<TestCaseWrapper> {
            public TestCaseWrapper(TestCaseDescriptor testCase) {
                this.Descriptor = testCase;

                var type = testCase.Assembly.GetType(testCase.Type, true);

                MethodInfo method = null;
                if (testCase.Method != null) {
                    method = type.GetMethod(testCase.Method, BindingFlags.Public | BindingFlags.Instance);
                    if (method == null) throw new InvalidOperationException($"Unable to find method '{testCase.Method}' on {type}");
                }

                TestMethodDependencyAttribute attribute = method.GetCustomAttribute<TestMethodDependencyAttribute>();

                if (attribute != null) {
                    this.Dependency = new TestCaseDescriptor(attribute.MethodDependency, type);
                }
            }


            public TestCaseDescriptor Descriptor { get; }
            public TestCaseDescriptor Dependency { get; }

            public bool IsDependencyOf(TestCaseWrapper other) {
                if (other.Dependency == null) {
                    return false;
                }

                return this.Descriptor == other.Dependency;
            }

            public bool HasDependencies => this.Dependency != null;

            public bool Equals(TestCaseWrapper other) {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return Equals(this.Descriptor, other.Descriptor);
            }

            public override bool Equals(object obj) {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                return obj is TestCaseWrapper && Equals((TestCaseWrapper)obj);
            }

            public override int GetHashCode() {
                return this.Descriptor?.GetHashCode() ?? 0;
            }

            public override string ToString() => this.Descriptor.ToString();
        }
    }
}
