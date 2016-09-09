// ******************************************************************************
//  © 2016 Sebastiaan Dammann - damsteen.nl
// 
//  File:           : Test.cs
//  Project         : NUnitTestOrdering.Tests
// ******************************************************************************
namespace NUnitTestOrdering.Tests.TestData.TestIntegrity.Single_WithSetupFixture {
    using NUnit.Framework;

    [TestFixture]
    public class UnorderedTest : LoggingTestBase {
        [OneTimeSetUp]
        public void OneTimeSetUp() {
            this.Log();
        }

        [SetUp]
        public void SetUp() {
            this.Log();
        }

        [Test]
        public void Execute() {
            this.Log();
        }

        [TearDown]
        public void TearDown() {
            this.Log();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown() {
            this.Log();
        }
    }
}
