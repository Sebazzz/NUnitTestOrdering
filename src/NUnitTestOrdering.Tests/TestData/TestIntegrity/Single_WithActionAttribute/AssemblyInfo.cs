// ******************************************************************************
//  © 2016 Sebastiaan Dammann - damsteen.nl
// 
//  File:           : AssemblyInfo.cs
//  Project         : NUnitTestOrdering.Tests
// ******************************************************************************

using NUnitTestOrdering.Tests.TestData.TestIntegrity.Single_WithActionAttribute;

[assembly:MyAction]

namespace NUnitTestOrdering.Tests.TestData.TestIntegrity.Single_WithActionAttribute {
    using System;

    using NUnit.Framework;
    using NUnit.Framework.Interfaces;

    using TestData;

    public sealed class MyActionAttribute : Attribute, ITestAction {
        private readonly TestLogger _testLogger;

        public MyActionAttribute() {
            this._testLogger = new TestLogger(this);
        }

        public void BeforeTest(ITest test) {
            this._testLogger.Log(extraData:test.ClassName);
        }

        public void AfterTest(ITest test) {
            this._testLogger.Log(extraData: test.ClassName);
        }

        public ActionTargets Targets => ActionTargets.Test | ActionTargets.Suite;
    }
}