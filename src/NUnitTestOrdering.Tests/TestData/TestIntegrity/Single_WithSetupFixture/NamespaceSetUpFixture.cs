// ******************************************************************************
//  © 2016 Sebastiaan Dammann - damsteen.nl
// 
//  File:           : NamespaceSetUpFixture.cs
//  Project         : NUnitTestOrdering.Tests
// ******************************************************************************
namespace NUnitTestOrdering.Tests.TestData.TestIntegrity.Single_WithSetupFixture {
    using NUnit.Framework;

    [SetUpFixture]
    public class NamespaceSetUpFixture : LoggingTestBase {
        [OneTimeSetUp]
        public void OneTimeSetUp() {
            this.Log();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown() {
            this.Log();
        }
    }
}
