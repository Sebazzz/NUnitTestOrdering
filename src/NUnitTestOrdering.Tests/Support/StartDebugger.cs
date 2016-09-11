// ******************************************************************************
//  © 2016 Sebastiaan Dammann - damsteen.nl
// 
//  File:           : StartDebugger.cs
//  Project         : NUnitTestOrdering.Tests
// ******************************************************************************

namespace NUnitTestOrdering.Tests.Support {
    using NUnit.Framework;
    using NUnit.Framework.Interfaces;
    using NUnit.Framework.Internal;

    public sealed class StartDebuggerAttribute : NUnitAttribute, IApplyToTest {
        public void ApplyToTest(Test test) {
            test.Properties.Set("StartDebugging", true);
        }
    }
}
