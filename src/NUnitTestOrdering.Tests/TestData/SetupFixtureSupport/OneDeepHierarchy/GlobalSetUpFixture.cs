// ******************************************************************************
//  © 2016 Sebastiaan Dammann - damsteen.nl
// 
//  File:           : GlobalSetUpFixture.cs
//  Project         : NUnitTestOrdering.Tests
// ******************************************************************************

using NUnit.Framework;

using NUnitTestOrdering.FixtureOrdering.Internal;
using NUnitTestOrdering.Tests.TestData;

[SetUpFixture]
[OrderedTestGlobalSetUpFixture]
public sealed class GlobalSetUpFixture : LoggingTestBase {
    [OneTimeSetUp]
    public void SetUp() {
        this.Log();
    }

    [OneTimeTearDown]
    public void TearDown() {
        this.Log();
    }
}
