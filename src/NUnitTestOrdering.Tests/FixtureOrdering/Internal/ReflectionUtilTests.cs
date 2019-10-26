// ******************************************************************************
//  © 2016 Sebastiaan Dammann - damsteen.nl
// 
//  File:           : ReflectionUtilTests.cs
//  Project         : NUnitTestOrdering.Tests
// ******************************************************************************

namespace NUnitTestOrdering.Tests.FixtureOrdering.Internal {
    using System.Collections.Generic;
    using System.Linq;

    using NUnit.Framework;
    using NUnit.Framework.Interfaces;
    using NUnit.Framework.Internal;

    using NUnitTestOrdering.FixtureOrdering.Internal;

    using Test = NUnit.Framework.Internal.Test;

    [TestFixture]
    public sealed class ReflectionUtilTests {
        [Test]
        public void ReflectionUtil_CanSetMaintainTestOrder_OnTestFixture() {
            // Given
            TestFixture testFixture = new TestFixture(new TypeWrapper(typeof(ReflectionUtilTests))) {
                Tests = {
                    new FakeTest("FirstTest"),
                    new FakeTest("SecondTest"),
                    new FakeTest("ThirdTest"),
                    new FakeTest("FourthTest")
                }
            };

            // When
            ReflectionUtil.SetMaintainTestOrder(testFixture);
            testFixture.Sort();

            // Then
            var result = testFixture.Tests.Select(x => x.Name);

            Assert.That(result, Is.EqualTo(new[] { "FirstTest", "SecondTest", "ThirdTest", "FourthTest" }));
        }

        [Test]
        public void ReflectionUtil_SortTestsByOrder_PutsUnorderedTestsFirst() {
            // Given
            TestFixture testFixture = new TestFixture(new TypeWrapper(typeof(ReflectionUtilTests))) {
                Tests = {
                    new FakeTest("1", 1),
                    new FakeTest("2", 2),
                    new FakeTest("Unordered"),
                    new FakeTest("3", 3)
                }
            };

            // When
            ReflectionUtil.SortByOrder(testFixture);

            // Then
            var result = testFixture.Tests.Select(x => x.Name);
            Assert.That(result, Is.EqualTo(new[] { "Unordered", "1", "2", "3" }));
        }

        [Test]
        public void ReflectionUtil_SortTestsByOrder_SortsUnorderedTestsByName() {
            // Given
            TestFixture testFixture = new TestFixture(new TypeWrapper(typeof(ReflectionUtilTests))) {
                Tests = {
                    new FakeTest("1", 1),
                    new FakeTest("2", 2),
                    new FakeTest("CCC"),
                    new FakeTest("AAA"),
                    new FakeTest("BBB"),
                    new FakeTest("3", 3)
                }
            };

            // When
            ReflectionUtil.SortByOrder(testFixture);

            // Then
            var result = testFixture.Tests.Select(x => x.Name);
            Assert.That(result, Is.EqualTo(new[] { "AAA", "BBB", "CCC", "1", "2", "3" }));
        }


        private sealed class FakeTest : Test {
            public FakeTest(string name) : base(name) { }

            public FakeTest(string name, int order) : base(name) {
                this.Properties.Set(PropertyNames.Order, order);
            }
            public override TestResult MakeTestResult() {
                throw new System.NotImplementedException();
            }

            public override TNode AddToXml(TNode parentNode, bool recursive) {
                throw new System.NotImplementedException();
            }

            public override string XmlElementName {
                get { throw new System.NotImplementedException(); }
            }

            public override bool HasChildren {
                get { throw new System.NotImplementedException(); }
            }

            public override IList<ITest> Tests => null;

            /// <inheritdoc />
            public override object[] Arguments {
                get { throw new System.NotImplementedException(); }
            }
        }
    }
}
