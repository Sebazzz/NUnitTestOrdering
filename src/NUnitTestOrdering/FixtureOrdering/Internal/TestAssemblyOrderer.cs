// ******************************************************************************
//  © 2016 Sebastiaan Dammann - damsteen.nl
// 
//  File:           : TestAssemblyOrderer.cs
//  Project         : NUnitTestOrdering
// ******************************************************************************

namespace NUnitTestOrdering.FixtureOrdering.Internal {
    using Common;

    using NUnit.Framework.Internal;

    public sealed class TestAssemblyOrderer {
        private readonly TestAssembly _testAssembly;
        private readonly TestAssemblyOrderingContext _context;

        public TestAssemblyOrderer(TestAssembly testAssembly) {
            this._testAssembly = testAssembly;
            this._context = TestAssemblyOrderingContext.Create(testAssembly);
        }

        public void ApplyOrdering() {
            this._context.Initialize();
        }

        
    }


}
