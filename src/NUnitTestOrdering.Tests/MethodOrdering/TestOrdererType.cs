// ******************************************************************************
//  © 2016 Sebastiaan Dammann - damsteen.nl
// 
//  File:           : TestOrdererType.cs
//  Project         : NUnitTestOrdering.Tests
// ******************************************************************************

namespace NUnitTestOrdering.Tests.MethodOrdering {
    using System;
    using System.Collections.Generic;

    using FixtureOrdering.Internal;

    using NUnit.Framework;
    using NUnit.Framework.Interfaces;
    using NUnit.Framework.Internal;

    using NUnitTestOrdering.Common;
    using NUnitTestOrdering.MethodOrdering;

    [TestFixture]
    public sealed class TestOrdererTests {
        [Test]
        public void TestOrderer_ThrowsException_OnDuplicateTest() {
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
            PublicTestOrderer testOrderer = new PublicTestOrderer(
                m => {
                    m("FirstTest");
                    m("SecondTest");
                    m("ThirdTest");
                    m("ThirdTest"); // Oops!
                });
            TestDelegate action = () => testOrderer.ApplyOrdering(testFixture);

            // Then
            Assert.That(action, Throws.TypeOf<TestOrderingException>());
        }

        [Test]
        public void TestOrderer_SetsOrderAttributeOfSelectedTests() {
            // Given
            TestFixture testFixture = new TestFixture(new TypeWrapper(typeof(ReflectionUtilTests))) {
                Tests = {
                    new FakeTest("FirstTest"),
                    new FakeTest("SecondTest"),
                    new FakeTest("ThirdTest"),
                    new FakeTest("UndefinedTest")
                }
            };

            // When
            PublicTestOrderer testOrderer = new PublicTestOrderer(
                m => {
                    m("FirstTest");
                    m("SecondTest");
                    m("ThirdTest");
                });
            testOrderer.ApplyOrdering(testFixture);

            // Then
            Assert.That(testFixture.Tests[0].Properties.Get(PropertyNames.Order), Is.EqualTo(1));
            Assert.That(testFixture.Tests[1].Properties.Get(PropertyNames.Order), Is.EqualTo(2));
            Assert.That(testFixture.Tests[2].Properties.Get(PropertyNames.Order), Is.EqualTo(3));
            Assert.That(testFixture.Tests[3].Properties.Get(PropertyNames.Order), Is.EqualTo(-1));
        }

        private sealed class PublicTestOrderer : TestOrderer {
            private readonly Action<Action<string>> _orderer;

            public PublicTestOrderer(Action<Action<string>> orderer) {
                this._orderer = orderer;
            }

            protected override void DefineOrdering() {
                this._orderer.Invoke(this.TestMethod);
            }
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
                get { throw new NotImplementedException(); }
            }
        }
    }
}
