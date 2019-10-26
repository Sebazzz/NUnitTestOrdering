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
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading;
    using System.Xml.Linq;

    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Emit;

    using NUnit.Framework;
    using NUnit.Framework.Interfaces;
    using NUnit.Framework.Internal;

    using NUnitTestOrdering.MethodOrdering;

    using TestData;

    internal sealed class TestRunner : IDisposable {
        private static readonly MetadataReference CorlibReference = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
        private static readonly MetadataReference SystemReference = MetadataReference.CreateFromFile(typeof(Uri).Assembly.Location);
        private static readonly MetadataReference SystemCoreReference = MetadataReference.CreateFromFile(typeof(NamedPipeServerStream).Assembly.Location);
        private static readonly MetadataReference NUnitTestOrderingReference = MetadataReference.CreateFromFile(typeof(TestMethodWithoutDependencyAttribute).Assembly.Location);

        private readonly string _namedPipeName;
        private readonly string _testAssemblyName;
        private readonly string _testAssemblyConfigPath;
        private readonly string _testAssemblyPath;
        private readonly string _testAssemblyPdbPath;
        private readonly string _testErrorPath;
        private readonly string _testOutputPath;
        private readonly string _testResultPath;

        private readonly bool _startDebugger;

        private readonly TestDataDirectory _testDataDirectory;
        private readonly NUnitVersion _nUnitVersion;

        private bool _hasCompiled;
        private string _nunitRunnerPath;

        public XDocument TestResultsDocument { get; private set; }

        public TestRunner(TestDataDirectory testDataDirectory, NUnitVersion nUnitVersion) {
            this._testDataDirectory = testDataDirectory;
            this._nUnitVersion = nUnitVersion;
            this._namedPipeName = Guid.NewGuid().ToString("N");

            string testFqName = TestExecutionContext.CurrentContext.CurrentTest.Parent.FullName;

            this._testAssemblyName = testFqName.Replace('.', '_');

            string testDirectory = Path.Combine(GetCurrentDirectory(), this._testAssemblyName);
            string nunitVersionName = "NUnit_v" + this._nUnitVersion.Version.Replace('.', '_');

            testDirectory = Path.Combine(testDirectory, nunitVersionName);

            if (!Directory.Exists(testDirectory)) {
                Directory.CreateDirectory(testDirectory);
            }

            CopyTestDependency(testDirectory, nUnitVersion.AssemblyLocation);
            CopyTestDependency(testDirectory, typeof(TestMethodWithoutDependencyAttribute).Assembly.Location);

            this._testAssemblyPath = Path.Combine(testDirectory, Path.ChangeExtension("Test", ".dll"));
            this._testAssemblyPdbPath = Path.ChangeExtension(this._testAssemblyPath, ".pdb");
            this._testAssemblyConfigPath = Path.ChangeExtension(this._testAssemblyPath, ".dll.config");
            this._testErrorPath = Path.ChangeExtension(this._testAssemblyPath, ".err.log");
            this._testOutputPath = Path.ChangeExtension(this._testAssemblyPath, ".out.log");
            this._testResultPath = Path.ChangeExtension(this._testAssemblyPath, ".xml");

            Environment.SetEnvironmentVariable("_TEST_PIPENAME", this._namedPipeName);

            // Note: using internal type of NUnit here
            var executionContext = TestExecutionContext.CurrentContext;
            Test currentTest = executionContext.CurrentTest;
            ITest parentTest = currentTest.Parent;

            this._startDebugger = currentTest.Properties.ContainsKey("StartDebugging") || parentTest.Properties.ContainsKey("StartDebugging");

            this.CleanUp();
        }

        private static void CopyTestDependency(string testDirectory, string testDependencySourcePath) {
            string testDependencyFileName = Path.GetFileName(testDependencySourcePath);
            Debug.Assert(testDependencyFileName != null);

            string testDependencyTargetAssemblyPath = Path.Combine(testDirectory, testDependencyFileName);

            if (!File.Exists(testDependencyTargetAssemblyPath)) {
                File.Copy(testDependencySourcePath, testDependencyTargetAssemblyPath);
            }

            if (!testDependencySourcePath.EndsWith("pdb")) {
                string pdbFile = testDependencySourcePath.Substring(0, testDependencySourcePath.Length - 3) + "pdb";

                if (File.Exists(pdbFile)) {
                    CopyTestDependency(testDirectory, pdbFile);
                }
            }
        }

        public void Dispose() {
            try {
                WriteArtifacts(this._testErrorPath, TestContext.Error);
                WriteArtifacts(this._testOutputPath, TestContext.Out);
            }
            catch (IOException ex) {
                TestContext.Error.WriteLine($"Unable to write artifacts: {ex}");
            }

            //this.CleanUp();
        }

        private void CleanUp() {
            TryDelete(this._testAssemblyPath);
            TryDelete(this._testAssemblyPdbPath);
            TryDelete(this._testOutputPath);
            TryDelete(this._testErrorPath);
            TryDelete(this._testResultPath);
        }

        private static void WriteArtifacts(string path, TextWriter target) {
            if (File.Exists(path)) {
                target.WriteLine(File.ReadAllText(path));
            }
        }

        private static void TryDelete(string path) {
            if (File.Exists(path)) {
                try {
                    File.Delete(path);
                }
                catch (Exception ex) {
                    TestContext.Out.WriteLine("Failed to delete file {0}: {1}", path, ex);
                }
            }
        }

        [SuppressMessage("ReSharper", "AccessToDisposedClosure", Justification = "Thread ends before disposal")]
        public string Run() {
            this.Compile();

            StringBuilder outputBuilder = new StringBuilder();
            using (NamedPipeServerStream np = new NamedPipeServerStream(this._namedPipeName, PipeDirection.In, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous)) {
                using (StreamReader sr = new StreamReader(np, Encoding.UTF8)) {
                    Process process = new Process {
                        StartInfo = this.GetConsoleArguments()
                    };

                    TestContext.WriteLine($"Starting process with arguments: \"{process.StartInfo.FileName}\" {process.StartInfo.Arguments}");
                    CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

                    ParameterizedThreadStart namedPipePump = o => {
                        CancellationToken ct = (CancellationToken)o;

                        Trace.WriteLine("Waiting for connection...");

                        try {
                            np.WaitForConnection(ct);
                        }
                        catch (OperationCanceledException) {
                            return;
                        }

                        Trace.WriteLine("Connected");

                        while (!ct.IsCancellationRequested)
                            if (np.IsConnected) outputBuilder.AppendLine(sr.ReadLine());
                            else Thread.Yield();
                    };

                    Thread thread = new Thread(namedPipePump) {
                        Name = "TestPump " + TestContext.CurrentContext.Test.FullName,
                        IsBackground = true
                    };

                    thread.Start(cancellationTokenSource.Token);

                    try {
                        process.Start();

                        if (this._startDebugger) {
                            process.WaitForExit();
                        }
                        else {
                            process.WaitForExit(1000 * 10 /* Reasonable time in which a test should complete */);
                        }

                        if (!process.HasExited) process.Kill();
                    }
                    finally {
                        cancellationTokenSource.Cancel();

                        thread.Join(TimeSpan.FromSeconds(5));

                        if (thread.IsAlive) {
                            Trace.WriteLine("Need to abort thread. It didn't want to terminate on its own.");
                            thread.Abort();
                        }

                        GC.KeepAlive(cancellationTokenSource);
                    }
                }
            }

            // Read results file
            this.TryReadResultsFile();

            // Trim whitespace at end for convenience
            return outputBuilder.ToString().TrimEnd();
        }

        private void TryReadResultsFile() {
            if (File.Exists(this._testResultPath)) {
                try {
                    this.TestResultsDocument = XDocument.Load(this._testResultPath);
                }
                catch (Exception ex) {
                    TestContext.Out.WriteLine("Failed to read results file {0}: {1}", this._testResultPath, ex);
                }
            }
        }

        private void Compile() {
            if (this._hasCompiled) return;

            this.CreateAppConfig();

            PortableExecutableReference nunitReference = this.GetNUnitReference();

            using (var appConfig = File.OpenRead(this._testAssemblyConfigPath)) {
                CSharpCompilation compilation =
                    CSharpCompilation.Create("Test")
                        .AddReferences(CorlibReference)
                        .AddReferences(SystemReference)
                        .AddReferences(SystemCoreReference)
                        .AddReferences(nunitReference)
                        .AddReferences(NUnitTestOrderingReference)
                        .AddSyntaxTrees(this.GetSyntaxTrees())
                        .WithOptions(
                            new CSharpCompilationOptions(
                                OutputKind.DynamicallyLinkedLibrary,
                                concurrentBuild: true,
                                warningLevel: 0,
                                assemblyIdentityComparer: DesktopAssemblyIdentityComparer.LoadFromXml(appConfig))
                        )
                ;

                EmitResult result = compilation.Emit(
                    this._testAssemblyPath,
                    this._testAssemblyPdbPath);

                Assert.That(result.Diagnostics, Is.Empty, "Error while emitting compilation result");

                this._hasCompiled = true;
            }
        }

        private PortableExecutableReference GetNUnitReference() {
            Assembly referenceNunitVersion = typeof(TestFixture).Assembly;

            if (new Version(this._nUnitVersion.Version) < referenceNunitVersion.GetName().Version) {
                // Causes CS1705 because the C# compiler refuses to use an older version of NUnit than
                // the NUnitTestOrdering assembly references, so instead we reference the up-to-date
                // version and at runtime load the older version instead
                return MetadataReference.CreateFromFile(referenceNunitVersion.Location);
            }

            return MetadataReference.CreateFromFile(this._nUnitVersion.AssemblyLocation);
        }

        private void CreateAppConfig() {
            AssemblyName nunitFrameworkAssembly = typeof(TestFixture).Assembly.GetName();

            XNamespace ns = XNamespace.Get("urn:schemas-microsoft-com:asm.v1");

            XDocument document = new XDocument(new XDeclaration("1.0", "utf-8", null));
            document.Add(
                new XElement("configuration",
                    new XElement("runtime",
                        new XElement(ns + "assemblyBinding",
                            new XElement(ns + "dependentAssembly",
                                new XElement(ns + "assemblyIdentity",
                                    new XAttribute("name", nunitFrameworkAssembly.Name),
                                    new XAttribute("publicKeyToken", String.Join(String.Empty, nunitFrameworkAssembly.GetPublicKeyToken().Select(x => x.ToString("x2")))),
                                    new XAttribute("culture", "neutral")
                                ),

                                new XElement(ns + "bindingRedirect",
                                    new XAttribute("oldVersion", "0.0.0.0-10.0.0.0"), // Wide version range, don't care
                                    new XAttribute("newVersion", this._nUnitVersion.Version + ".0")
                                )
                            )
                        )
                    )
                )
            );

            document.Save(this._testAssemblyConfigPath);
        }

        private SyntaxTree[] GetSyntaxTrees() {
            SyntaxTree[] trees = new SyntaxTree[this._testDataDirectory.Files.Length];

            int index = 0;
            foreach (string resourceName in this._testDataDirectory.Files) {
                Stream resourceStream = this.GetType().Assembly.GetManifestResourceStream(resourceName);
                if (resourceStream == null) Assert.Inconclusive("Unable to find embedded resource {0}", resourceName);

                string path = resourceName.Replace('.', '/');
                int lastDot = resourceName.LastIndexOf('.');
                path = path.Substring(0, lastDot) + '.' + path.Substring(lastDot + 1);

                using (StreamReader sr = new StreamReader(resourceStream)) {
                    trees[index++] = CSharpSyntaxTree.ParseText(
                        path: path,
                        text: sr.ReadToEnd(),
                        options: new CSharpParseOptions(LanguageVersion.CSharp6, DocumentationMode.None),
                        encoding: Encoding.UTF8);
                }
            }

            return trees;
        }

        private ProcessStartInfo GetConsoleArguments() {
            string nunitPath = this.GetNUnitConsoleRunnerPath();
            string process = this._startDebugger ? "vsjitdebugger.exe" : nunitPath;

            ProcessStartInfo startInfo = new ProcessStartInfo {
                FileName = process,
                Arguments = $"\"{this._testAssemblyPath}\" --domain=Single \"--result={this._testResultPath};format=nunit3\" \"--out={this._testOutputPath}\"  \"--err={this._testErrorPath}\" --process=InProcess --verbose --full --workers=1 --params NAMEDPIPE={this._namedPipeName}",
                WorkingDirectory = GetCurrentDirectory(),
                CreateNoWindow = false,
                WindowStyle = ProcessWindowStyle.Minimized
            };

            if (this._startDebugger) startInfo.Arguments = $"\"{nunitPath}\" " + startInfo.Arguments;
            if (Debugger.IsAttached || this._startDebugger) startInfo.Arguments += " --wait";

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
