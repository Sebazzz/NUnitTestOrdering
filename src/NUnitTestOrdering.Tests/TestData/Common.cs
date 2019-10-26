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

    /// <summary>
    /// Helper class for logging. Note we explicitly _dont_  use one of the initializer attributes here,
    /// as we don't want to influence how NUnit runs the tests
    /// </summary>
    internal sealed class Common {
        private static Common LazyInstance;
        public static Common Instance => LazyInstance ?? (LazyInstance = new Common());

        public const string PipeName = "_TEST_PIPENAME";

        private readonly NamedPipeClientStream _namedPipe;
        private readonly TextWriter _logWriter;

        public Common() {
            string pipeName = TestContext.Parameters.Get("NAMEDPIPE") ?? Environment.GetEnvironmentVariable(PipeName, EnvironmentVariableTarget.Process);

            Console.WriteLine("Using pipe name: {0}", pipeName);

            // Pipename will be null when directly running tests from test assmebly runner
            if (pipeName == null) {
                this._logWriter = new StringWriter();

                TestContext.Out.WriteLine("Pipe name was not provided! Tests may fail!");
            }
            else {
                this._namedPipe = new NamedPipeClientStream(".", pipeName, PipeDirection.Out);
                this._namedPipe.Connect();

                this._logWriter = new StreamWriter(this._namedPipe, Encoding.UTF8);
            }

            AppDomain.CurrentDomain.DomainUnload += (o, e) => {
                this._logWriter.Flush();
                this._namedPipe?.WaitForPipeDrain();

                this._logWriter.Dispose();
                this._namedPipe?.Dispose();
            };
        }

        public void Log(string s) {
            TestContext.Out.WriteLine(s);

            this._logWriter.WriteLine(s);
            this._logWriter.Flush();
            this._namedPipe?.WaitForPipeDrain();
        }
    }

    internal sealed class TestLogger {
        private readonly object _testInstance;

        public TestLogger(object testInstance) {
            this._testInstance = testInstance;
        }

        public void Log(string extraData = null, [CallerMemberName] string method = null) {
            string extraDataString = string.Empty;
            if (!String.IsNullOrEmpty(extraData)) extraDataString = $" [{extraData}]";

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
