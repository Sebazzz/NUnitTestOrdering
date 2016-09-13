// ******************************************************************************
//  © 2016 Sebastiaan Dammann - damsteen.nl
// 
//  File:           : OrderedTestFixtureAttribute.cs
//  Project         : NUnitTestOrdering
// ******************************************************************************
namespace NUnitTestOrdering.FixtureOrdering {
    using System;

    /// <summary>
    /// In order to order test fixtures, derive a class from <see cref="TestOrderingSpecification"/>
    /// and decorate it with this attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class OrderedTestFixtureAttribute : Attribute {}
}