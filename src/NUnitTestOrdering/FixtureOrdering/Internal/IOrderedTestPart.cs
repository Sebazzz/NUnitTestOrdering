// ******************************************************************************
//  © 2016 Sebastiaan Dammann - damsteen.nl
// 
//  File:           : IOrderedTestPart.cs
//  Project         : NUnitTestOrdering
// ******************************************************************************
namespace NUnitTestOrdering.FixtureOrdering.Internal {
    using NUnit.Framework.Internal;

    internal interface IOrderedTestPart {
        Test GetTest(TestHierarchyContext context);
    }
}