// ******************************************************************************
//  © 2016 Sebastiaan Dammann - damsteen.nl
// 
//  File:           : TestOrderingException.cs
//  Project         : NUnitTestOrdering
// ******************************************************************************
namespace NUnitTestOrdering.Common {
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// This exception is thrown when a problem occurs setting up the test hierarchy
    /// </summary>
    [Serializable]
    public class TestOrderingException : Exception {
        /// <inheritdoc />
        public TestOrderingException() {}
        /// <inheritdoc />
        public TestOrderingException(string message) : base(message) {}
        /// <inheritdoc />
        public TestOrderingException(string message, Exception innerException) : base(message, innerException) {}
        /// <inheritdoc />
        protected TestOrderingException(SerializationInfo info, StreamingContext context) : base(info, context) {}
    }
}
