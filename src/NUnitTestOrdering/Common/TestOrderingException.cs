// ******************************************************************************
//  © 2016 Sebastiaan Dammann - damsteen.nl
// 
//  File:           : TestOrderingException.cs
//  Project         : NUnitTestOrdering
// ******************************************************************************
namespace NUnitTestOrdering.Common {
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public class TestOrderingException : Exception {
        public TestOrderingException() {}
        public TestOrderingException(string message) : base(message) {}
        public TestOrderingException(string message, Exception innerException) : base(message, innerException) {}
        protected TestOrderingException(SerializationInfo info, StreamingContext context) : base(info, context) {}
    }
}
