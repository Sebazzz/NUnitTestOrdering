

namespace NUnitTestOrdering.Tests.TestData {
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Reflection;
    using System.Text;

    public class TestDataDirectory {
       public string [] Files { get; set; }
       public string ExpectedResultFile { get;set; }

       public string ReadResultsFile() {
           Assembly thisAssembly = this.GetType().Assembly;
           StringBuilder resultsFile = new StringBuilder();

           Stream stream = thisAssembly.GetManifestResourceStream(this.ExpectedResultFile);
           if (stream == null) {
               throw new InvalidOperationException("Unknown manifest resource stream " + this.ExpectedResultFile);
           }

           using (StreamReader sr = new StreamReader(stream)) {
               string line;
               while ((line = sr.ReadLine()) != null) {
                   if (line.Length == 0 || line[0] == '#') {
                       // Skip "comment" line
                       continue;
                   }

                   resultsFile.AppendLine(line);
               }
           }

           resultsFile.Remove(resultsFile.Length - Environment.NewLine.Length, Environment.NewLine.Length);
           return resultsFile.ToString();
       }
    }

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public static class TestDataDirectories {

        
        public static class MethodOrdering {

            
                public static TestDataDirectory Simple() {

                    return new TestDataDirectory {
                        Files = new [] {
                                                            
                                typeof(TestDataDirectory).Assembly.GetName().Name + "." + "TestData.MethodOrdering.Simple.Test.cs",

                            
                            typeof(TestDataDirectory).Assembly.GetName().Name + "." + @"TestData.Common.cs"
                        },

                        
                        ExpectedResultFile = typeof(TestDataDirectory).Assembly.GetName().Name + ".TestData.MethodOrdering.Simple.ExpectedTestResult.txt"

                        
                    };
                }

            
            }
            
        
        public static class TestIntegrity {

            
                public static TestDataDirectory OrderedWithOrderAttribute() {

                    return new TestDataDirectory {
                        Files = new [] {
                                                            
                                typeof(TestDataDirectory).Assembly.GetName().Name + "." + "TestData.TestIntegrity.OrderedWithOrderAttribute.Test.cs",

                            
                            typeof(TestDataDirectory).Assembly.GetName().Name + "." + @"TestData.Common.cs"
                        },

                        
                        ExpectedResultFile = typeof(TestDataDirectory).Assembly.GetName().Name + ".TestData.TestIntegrity.OrderedWithOrderAttribute.ExpectedTestResult.txt"

                        
                    };
                }

            
                public static TestDataDirectory Single() {

                    return new TestDataDirectory {
                        Files = new [] {
                                                            
                                typeof(TestDataDirectory).Assembly.GetName().Name + "." + "TestData.TestIntegrity.Single.Test.cs",

                            
                            typeof(TestDataDirectory).Assembly.GetName().Name + "." + @"TestData.Common.cs"
                        },

                        
                        ExpectedResultFile = typeof(TestDataDirectory).Assembly.GetName().Name + ".TestData.TestIntegrity.Single.ExpectedTestResult.txt"

                        
                    };
                }

            
                public static TestDataDirectory Single_WithSetupFixture() {

                    return new TestDataDirectory {
                        Files = new [] {
                                                            
                                typeof(TestDataDirectory).Assembly.GetName().Name + "." + "TestData.TestIntegrity.Single_WithSetupFixture.GlobalSetUpFixture.cs",

                                                            
                                typeof(TestDataDirectory).Assembly.GetName().Name + "." + "TestData.TestIntegrity.Single_WithSetupFixture.NamespaceSetUpFixture.cs",

                                                            
                                typeof(TestDataDirectory).Assembly.GetName().Name + "." + "TestData.TestIntegrity.Single_WithSetupFixture.Test.cs",

                            
                            typeof(TestDataDirectory).Assembly.GetName().Name + "." + @"TestData.Common.cs"
                        },

                        
                        ExpectedResultFile = typeof(TestDataDirectory).Assembly.GetName().Name + ".TestData.TestIntegrity.Single_WithSetupFixture.ExpectedTestResult.txt"

                        
                    };
                }

            
                public static TestDataDirectory Single_WithSetupTeardown() {

                    return new TestDataDirectory {
                        Files = new [] {
                                                            
                                typeof(TestDataDirectory).Assembly.GetName().Name + "." + "TestData.TestIntegrity.Single_WithSetupTeardown.Test.cs",

                            
                            typeof(TestDataDirectory).Assembly.GetName().Name + "." + @"TestData.Common.cs"
                        },

                        
                        ExpectedResultFile = typeof(TestDataDirectory).Assembly.GetName().Name + ".TestData.TestIntegrity.Single_WithSetupTeardown.ExpectedTestResult.txt"

                        
                    };
                }

            
            }
            
        
    }
}

namespace NUnitTestOrdering.Tests.IntegrationTests {
    using NUnit.Framework;
    using System.Diagnostics.CodeAnalysis;
    using NUnitTestOrdering.Tests.TestData;
    using Support;

    
    [TestFixture]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public sealed class MethodOrdering {
             
            
        [Test]
        public void Simple() {
            // Given
            var input = TestDataDirectories.MethodOrdering.Simple();
            string expectedResult = input.ReadResultsFile();

            // When
            string result;
            using (TestRunner testRunner = new TestRunner(input)) {
                result = testRunner.Run();
            }

            // Then
            Assert.That(result, Is.EqualTo(expectedResult));
        }

            
    }

    
    [TestFixture]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public sealed class TestIntegrity {
             
            
        [Test]
        public void OrderedWithOrderAttribute() {
            // Given
            var input = TestDataDirectories.TestIntegrity.OrderedWithOrderAttribute();
            string expectedResult = input.ReadResultsFile();

            // When
            string result;
            using (TestRunner testRunner = new TestRunner(input)) {
                result = testRunner.Run();
            }

            // Then
            Assert.That(result, Is.EqualTo(expectedResult));
        }

            
        [Test]
        public void Single() {
            // Given
            var input = TestDataDirectories.TestIntegrity.Single();
            string expectedResult = input.ReadResultsFile();

            // When
            string result;
            using (TestRunner testRunner = new TestRunner(input)) {
                result = testRunner.Run();
            }

            // Then
            Assert.That(result, Is.EqualTo(expectedResult));
        }

            
        [Test]
        public void Single_WithSetupFixture() {
            // Given
            var input = TestDataDirectories.TestIntegrity.Single_WithSetupFixture();
            string expectedResult = input.ReadResultsFile();

            // When
            string result;
            using (TestRunner testRunner = new TestRunner(input)) {
                result = testRunner.Run();
            }

            // Then
            Assert.That(result, Is.EqualTo(expectedResult));
        }

            
        [Test]
        public void Single_WithSetupTeardown() {
            // Given
            var input = TestDataDirectories.TestIntegrity.Single_WithSetupTeardown();
            string expectedResult = input.ReadResultsFile();

            // When
            string result;
            using (TestRunner testRunner = new TestRunner(input)) {
                result = testRunner.Run();
            }

            // Then
            Assert.That(result, Is.EqualTo(expectedResult));
        }

            
    }

    }