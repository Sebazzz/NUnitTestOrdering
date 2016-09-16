// ******************************************************************************
//  © 2016 Sebastiaan Dammann - damsteen.nl
// 
//  File:           : Test.cs
//  Project         : NUnitTestOrdering.Tests
// ******************************************************************************

using NUnitTestOrdering.Tests.TestData.TestIntegrity.ActionAttributeRunsOnEverything;

[assembly: MyAction]
namespace NUnitTestOrdering.Tests.TestData.TestIntegrity.ActionAttributeRunsOnEverything {
    using System;

    using NUnit.Framework;
    using NUnit.Framework.Interfaces;

    public sealed class MyActionAttribute : Attribute, ITestAction {
        private readonly TestLogger _logger;

        /// <inheritdoc />
        public MyActionAttribute() {
            this._logger = new TestLogger(this);
        }

        /// <inheritdoc />
        public void BeforeTest(ITest test) {
            this._logger.Log(test.MethodName);
        }

        /// <inheritdoc />
        public void AfterTest(ITest test) {
            this._logger.Log(test.MethodName);
        }

        /// <inheritdoc />
        public ActionTargets Targets => ActionTargets.Suite | ActionTargets.Test;
    }

    [TestFixture]
    public class UnorderedTest1 : LoggingTestBase {
        [Test]
        public void Execute() {
            this.Log();
        }
    }

    [TestFixture]
    public class UnorderedTest2 : LoggingTestBase {
        [Test]
        public void Execute() {
            this.Log();
        }
    }
}
