// ******************************************************************************
//  © 2016 Sebastiaan Dammann - damsteen.nl
// 
//  File:           : Test.cs
//  Project         : NUnitTestOrdering.Tests
// ******************************************************************************
namespace NUnitTestOrdering.Tests.TestData.TestIntegrity.Single_WithActionAttribute {
    using NUnit.Framework;

    [TestFixture]
    public class UnorderedTest : LoggingTestBase {
        [Test]
        public void Execute() {
            this.Log();
        }
    }
}
