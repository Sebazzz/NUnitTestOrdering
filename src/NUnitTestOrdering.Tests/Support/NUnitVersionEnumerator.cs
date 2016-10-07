namespace NUnitTestOrdering.Tests.Support {
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Xml.Linq;

    using NUnit.Framework;
    using NUnit.Framework.Interfaces;
    using NUnit.Framework.Internal;

    [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
    [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
    internal sealed class NUnitVersionEnumerator {
        private const string PackagesConfigPath = "NUnitTestOrdering.Tests.Support.NUnitTestVersions.packages.config";

        private static readonly Lazy<IEnumerable<NUnitVersion>> PrivateInstance = new Lazy<IEnumerable<NUnitVersion>>(GetNUnitVersions, LazyThreadSafetyMode.PublicationOnly);

        private static IEnumerable<NUnitVersion> GetNUnitVersions() {
            var nunitVersions = new List<NUnitVersion>();

            // Find packages folder
            int traversalThreshold = 5;
            string packagesFolderFullPath = null;

            Assembly myAssembly = typeof(NUnitVersionEnumerator).Assembly;

            string thisAssemblyLocation = myAssembly.Location;
            DirectoryInfo currentDirectory = new FileInfo(thisAssemblyLocation).Directory;
            while (traversalThreshold > 0 && currentDirectory.Parent != null) {
                currentDirectory = currentDirectory.Parent;

                if (currentDirectory == null) break;

                string possiblePackageFolderPath = Path.Combine(currentDirectory.FullName, "packages");
                if (Directory.Exists(possiblePackageFolderPath)) {
                    packagesFolderFullPath = possiblePackageFolderPath;
                    break;
                }
            }

            if (packagesFolderFullPath == null) {
                throw new InvalidOperationException($"Unable to find packages folder of NuGet, started from directory {Path.GetDirectoryName(thisAssemblyLocation)}");
            }

            // Read package config
            var versionStrings =
                XDocument.Load(myAssembly.GetManifestResourceStream(PackagesConfigPath))
                    .Elements("packages")
                    .Elements("package")
                    .Where(x => (string) x.Attribute("id") == "NUnit")
                    .Select(x => (string) x.Attribute("version"));

            foreach (string versionString in versionStrings.OrderBy(x => x)) {
                string packageName = $"NUnit.{versionString}";
                string fullPackageFolderPath = Path.Combine(packagesFolderFullPath, packageName);

                const string nunitAssemblyRelativePath = @"lib\net45\nunit.framework.dll";

                string nunitAssemblyAbsolutePath = Path.Combine(fullPackageFolderPath, nunitAssemblyRelativePath);

                if (!File.Exists(nunitAssemblyAbsolutePath)) {
                    throw new InvalidOperationException($"Unable to find NUnit version {versionString} at location {nunitAssemblyAbsolutePath}");
                }

                nunitVersions.Add(new NUnitVersion {
                    AssemblyLocation = nunitAssemblyAbsolutePath,
                    Version = versionString
                });
            }

            return nunitVersions;
        }

        public static IEnumerable<ITestCaseData> GetTestCases() {
            int order = 0;

            foreach (NUnitVersion version in PrivateInstance.Value) {
                yield return new TestCaseData(version)
                    .SetName($"NUnit-{version.Version}")
                    .SetCategory(TestCategory.IntegrationTest)
                    .SetProperty(PropertyNames.Order, order)
                    .SetDescription($"Run integration test on NUnit v{version.Version}");
            }
        }
    }
}