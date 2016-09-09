// ******************************************************************************
//  © 2016 Sebastiaan Dammann - damsteen.nl
// 
//  File:           : GlobalTest.cs
//  Project         : NUnitTestOrdering.Tests
// ******************************************************************************
namespace NUnitTestOrdering.Tests.Support {
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.IO.Pipes;
    using System.Text;
    using System.Threading;

    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Emit;

    using NUnit.Framework;

    using TestData;

    public sealed class TestRunner : IDisposable {
        private static readonly MetadataReference CorlibReference = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
        private static readonly MetadataReference SystemReference = MetadataReference.CreateFromFile(typeof(Uri).Assembly.Location);
        private static readonly MetadataReference SystemCoreReference = MetadataReference.CreateFromFile(typeof(NamedPipeServerStream).Assembly.Location);
        private static readonly MetadataReference NUnitReference = MetadataReference.CreateFromFile(typeof(OneTimeSetUpAttribute).Assembly.Location);

        private readonly string _namedPipeName;
        private readonly string _testAssemblyName;
        private readonly string _testAssemblyPath;
        private readonly string _testAssemblyPdbPath;

        private readonly TestDataDirectory _testDataDirectory;

        private bool _hasCompiled;
        private string _nunitRunnerPath;

        public TestRunner(TestDataDirectory testDataDirectory) {
            this._testDataDirectory = testDataDirectory;
            this._namedPipeName = Guid.NewGuid().ToString("N");

            this._testAssemblyName = this._namedPipeName;
            this._testAssemblyPath =
                Path.Combine(
                    GetCurrentDirectory(),
                    Path.ChangeExtension(this._testAssemblyName, ".dll"));
            this._testAssemblyPdbPath = Path.ChangeExtension(this._testAssemblyPath, ".pdb");

            Environment.SetEnvironmentVariable("_TEST_PIPENAME", this._namedPipeName);
        }

        public void Dispose() {
            if (File.Exists(this._testAssemblyPath)) File.Delete(this._testAssemblyPath);
            if (File.Exists(this._testAssemblyPdbPath)) File.Delete(this._testAssemblyPdbPath);
        }

        [SuppressMessage("ReSharper", "AccessToDisposedClosure", Justification = "Thread ends before disposal")]
        public string Run() {
            this.Compile();

            StringBuilder outputBuilder = new StringBuilder();
            using (NamedPipeServerStream np = new NamedPipeServerStream(this._namedPipeName, PipeDirection.In, 1, PipeTransmissionMode.Byte)) {
                using (StreamReader sr = new StreamReader(np, Encoding.UTF8)) {
                    Process process = new Process {
                        StartInfo = this.GetConsoleArguments()
                    };

                    Trace.WriteLine($"Starting process with arguments: {process.StartInfo.Arguments}");
                    CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
                    ParameterizedThreadStart namedPipePump = o => {
                                                                 CancellationToken ct = (CancellationToken) o;

                                                                 Trace.WriteLine("Waiting for connection...");
                                                                 np.WaitForConnection();
                                                                 Trace.WriteLine("Connected");

                                                                 while (!ct.IsCancellationRequested)
                                                                     if (np.IsConnected) outputBuilder.AppendLine(sr.ReadLine());
                                                                     else Thread.Yield();
                                                             };

                    Thread thread = new Thread(namedPipePump) {
                        Name = "TestPump"
                    };

                    thread.Start(cancellationTokenSource.Token);

                    try {
                        process.Start();

                        process.WaitForExit(1000 * 60 * 10);
                        if (!process.HasExited) process.Kill();
                    }
                    finally {
                        cancellationTokenSource.Cancel();
                        thread.Join(TimeSpan.FromSeconds(5));
                        if (thread.IsAlive) {
                            Trace.WriteLine("Need to abort thread. It didn't want to terminate on its own.");
                            thread.Abort();
                        }
                    }
                }
            }

            // Trim whitespace at end for convenience
            return outputBuilder.ToString().TrimEnd();
        }

        private void Compile() {
            if (this._hasCompiled) return;

            CSharpCompilation compilation =
                CSharpCompilation.Create(this._testAssemblyName)
                    .AddReferences(CorlibReference)
                    .AddReferences(SystemReference)
                    .AddReferences(SystemCoreReference)
                    .AddReferences(NUnitReference)
                    .AddSyntaxTrees(this.GetSyntaxTrees())
                    .WithOptions(
                        new CSharpCompilationOptions(
                            OutputKind.DynamicallyLinkedLibrary,
                            concurrentBuild: true,
                            warningLevel: 0
                        ));

            EmitResult result = compilation.Emit(
                this._testAssemblyPath,
                this._testAssemblyPdbPath);

            Assert.That(result.Diagnostics, Is.Empty, "Error while emitting compilation result");

            this._hasCompiled = true;
        }

        private SyntaxTree[] GetSyntaxTrees() {
            SyntaxTree[] trees = new SyntaxTree[this._testDataDirectory.Files.Length];

            int index = 0;
            foreach (string resourceName in this._testDataDirectory.Files) {
                Stream resourceStream = this.GetType().Assembly.GetManifestResourceStream(resourceName);
                if (resourceStream == null) Assert.Inconclusive("Unable to find embedded resource {0}", resourceName);

                using (StreamReader sr = new StreamReader(resourceStream)) {
                    trees[index++] = CSharpSyntaxTree.ParseText(sr.ReadToEnd());
                }
            }

            return trees;
        }

        private ProcessStartInfo GetConsoleArguments() {
            ProcessStartInfo startInfo = new ProcessStartInfo(this.GetNUnitConsoleRunnerPath()) {
                Arguments = $"\"{this._testAssemblyPath}\" --domain=Single --process=InProcess --verbose --full --workers=1 --params NAMEDPIPE={this._namedPipeName}",
                WorkingDirectory = GetCurrentDirectory(),
                CreateNoWindow = false
            };

            if (Debugger.IsAttached) startInfo.Arguments += " --wait";

            return startInfo;
        }

        private string GetNUnitConsoleRunnerPath() {
            if (this._nunitRunnerPath != null) return this._nunitRunnerPath;

            string rootPath = GetCurrentDirectory();

            DirectoryInfo directoryInfo = new DirectoryInfo(rootPath);
            while (directoryInfo != null) {
                DirectoryInfo[] filter = directoryInfo.GetDirectories("packages");
                if (filter.Length > 0) {
                    directoryInfo = filter[0];
                    break;
                }

                directoryInfo = directoryInfo.Parent;
            }

            if (directoryInfo == null) throw new InvalidOperationException($"Unable to find NUnit path from search path: {rootPath}");

            DirectoryInfo[] consoleRunner = directoryInfo.GetDirectories("NUnit.ConsoleRunner.*");
            if (consoleRunner.Length == 0) throw new InvalidOperationException($"Unable to find console runner in path {directoryInfo.FullName}");

            return this._nunitRunnerPath = Path.Combine(consoleRunner[0].FullName, "tools", "nunit3-console.exe");
        }

        private static string GetCurrentDirectory() {
            return AppDomain.CurrentDomain.BaseDirectory;
        }
    }
}
