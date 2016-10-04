// ******************************************************************************
//  © 2016 Sebastiaan Dammann - damsteen.nl
// 
//  File:           : NamedPipeStreamExtensions.cs
//  Project         : NUnitTestOrdering.Tests
// ******************************************************************************

namespace NUnitTestOrdering.Tests.Support {
    using System;
    using System.Diagnostics;
    using System.IO.Pipes;
    using System.Threading;
    using System.Threading.Tasks;

    internal static class NamedPipeStreamExtensions {
        public static void WaitForConnection(this NamedPipeServerStream namedPipeServerStream, CancellationToken ct) {
            try {
                namedPipeServerStream.WaitForConnectionAsync(ct).ConfigureAwait(false).GetAwaiter().GetResult();
            }
            catch (Exception ex) {
                Trace.WriteLine("WaitForConnection failure: " + ex);
            }

            ct.ThrowIfCancellationRequested();
        }
    }
}
