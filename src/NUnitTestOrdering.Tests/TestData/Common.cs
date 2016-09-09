// ******************************************************************************
//  © 2016 Sebastiaan Dammann - damsteen.nl
// 
//  File:           : Common.cs
//  Project         : NUnitTestOrdering.Tests
// ******************************************************************************
namespace NUnitTestOrdering.Tests.TestData {
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.IO.Pipes;
    using System.Runtime.CompilerServices;
    using System.Text;

    using NUnit.Framework;

    public sealed class Common {
        private static Common LazyInstance;
        public static Common Instance => LazyInstance ?? (LazyInstance = new Common());

        public const string PipeName = "_TEST_PIPENAME";

        private readonly NamedPipeClientStream _namedPipe;
        private readonly StreamWriter _streamWriter;

        public Common() {
            string pipeName = TestContext.Parameters.Get("NAMEDPIPE") ?? Environment.GetEnvironmentVariable(PipeName, EnvironmentVariableTarget.Process);

            Console.WriteLine("Using pipe name: {0}", pipeName);

            if (pipeName == null) throw new InvalidOperationException("Unable to find test pipe name");

            this._namedPipe = new NamedPipeClientStream(".", pipeName, PipeDirection.Out);
            this._namedPipe.Connect();
            
            this._streamWriter = new StreamWriter(this._namedPipe, Encoding.UTF8);

            AppDomain.CurrentDomain.DomainUnload += (o, e) => {
                this._streamWriter.Flush();
                this._namedPipe.WaitForPipeDrain();

                this._streamWriter.Dispose();
                this._namedPipe.Dispose();
            };
        }

        public void Log(string s) {
            this._streamWriter.WriteLine(s);
            this._streamWriter.Flush();
            this._namedPipe.WaitForPipeDrain();
        }
    }

    public sealed class TestLogger {
        private readonly object _testInstance;

        public TestLogger(object testInstance) {
            this._testInstance = testInstance;
        }

        public void Log(string extraData = null, [CallerMemberName] string method = null) {
            string extraDataString = string.Empty;
            if (extraData != null) extraDataString = ' ' + '[' + extraData + ']';

            Common.Instance.Log($"{this._testInstance.GetType().Name}.{method}{extraDataString}");
        }
    }

    public abstract class LoggingTestBase {
        private readonly TestLogger _logger;

        protected LoggingTestBase() {
            this._logger = new TestLogger(this);
        }

        [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
        protected void Log(string extraData = null, [CallerMemberName] string method = null) {
            this._logger.Log(extraData, method);
        }
    }
}
