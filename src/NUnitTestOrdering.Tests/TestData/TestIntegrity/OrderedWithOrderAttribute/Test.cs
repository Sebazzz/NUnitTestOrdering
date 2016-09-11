// ******************************************************************************
//  © 2016 Sebastiaan Dammann - damsteen.nl
// 
//  File:           : Test.cs
//  Project         : NUnitTestOrdering.Tests
// ******************************************************************************
namespace NUnitTestOrdering.Tests.TestData.TestIntegrity.OrderedWithOrderAttribute {
    using NUnit.Framework;

    [TestFixture]
    public class OrderedTest : LoggingTestBase {
        [Test]
        [Order(1)]
        public void One() {
            this.Log();
        }

        [Test]
        [Order(2)]
        public void Two() {
            this.Log();
        }


        [Test]
        [Order(3)]
        public void Three() {
            this.Log();
        }

        [Test]
        [Order(4)]
        public void Four() {
            this.Log();
        }
    }
}
