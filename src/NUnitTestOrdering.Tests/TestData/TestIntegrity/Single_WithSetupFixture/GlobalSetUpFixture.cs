// ******************************************************************************
//  © 2016 Sebastiaan Dammann - damsteen.nl
// 
//  File:           : GlobalSetUpFixture.cs
//  Project         : NUnitTestOrdering.Tests
// ******************************************************************************
using System.Diagnostics.CodeAnalysis;

using NUnit.Framework;
using NUnitTestOrdering.Tests.TestData;

[SetUpFixture]
[SuppressMessage("ReSharper", "CheckNamespace")]
public class GlobalSetUpFixture : LoggingTestBase {
    [OneTimeSetUp]
    public void OneTimeSetUp() {
        this.Log();
    }

    [OneTimeTearDown]
    public void OneTimeTearDown() {
        this.Log();
    }
}
