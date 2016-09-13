// ******************************************************************************
//  © 2016 Sebastiaan Dammann - damsteen.nl
// 
//  File:           : IDependencyIndicator.cs
//  Project         : NUnitTestOrdering
// ******************************************************************************
namespace NUnitTestOrdering.Common {
    using System;

    internal interface IDependencyIndicator<T> : IEquatable<T> {
        bool HasDependencies { get; }

        /// <summary>
        ///     Returns <c>true</c> if the current instance is a dependency of the other instance
        /// </summary>
        bool IsDependencyOf(T other);
    }
}