// ******************************************************************************
//  © 2016 Sebastiaan Dammann - damsteen.nl
// 
//  File:           : ResultsDocument.cs
//  Project         : NUnitTestOrdering.Tests
// ******************************************************************************

namespace NUnitTestOrdering.Tests.Support {
    using System;
    using System.Text;
    using System.Xml.Linq;
    using System.Xml.XPath;

    using NUnit.Framework;
    using NUnit.Framework.Interfaces;

    public sealed class ResultsDocument {
        private readonly XElement _rootTestNode;

        public XDocument RawDocument { get; }

        public ResultsDocument(XDocument rawDocument) {
            if (rawDocument == null) throw new ArgumentNullException(nameof(rawDocument));

            this.RawDocument = rawDocument;

            this._rootTestNode = rawDocument.XPathSelectElement("/test-run/test-suite");
            if (this._rootTestNode == null) {
                Assert.Fail("Unable to find test results test-suite node in document: " + rawDocument);
            }
        }

        public TestStatus GetTestResult(string test) {
            XElement testElement = this.GetTestElement(test);

            string resultString = (string) testElement.Attribute("result");
            if (resultString == null) {
                Assert.Fail("Unable to find result attribute for test {1} in node: {0}", testElement, test);
            }

            return (TestStatus) Enum.Parse(typeof(TestStatus), resultString);
        }

        public string GetFailureMessage(string test) {
            XElement testElement = this.GetTestElement(test);

            string resultString = (string)testElement.Element("failure")?.Element("message");

            return resultString;
        }

        public RunState GetRunState(string test) {
            XElement testElement = this.GetTestElement(test);

            string resultString = (string)testElement.Attribute("runstate");
            if (resultString == null) {
                Assert.Fail("Unable to find runstate attribute for test {1} in node: {0}", testElement, test);
            }

            return (RunState)Enum.Parse(typeof(RunState), resultString);
        }

        private XElement GetTestElement(string test) {
            string[] parts = test.Split('.');

            StringBuilder xPathBuilder = new StringBuilder();
            for (int index = 0; index < parts.Length; index++) {
                string part = parts[index];
                bool isLastPart = index + 1 >= parts.Length;

                if (!isLastPart) {
                    xPathBuilder.Append($"test-suite[@name=\"{part}\"]");
                    xPathBuilder.Append("/");
                } else {
                    xPathBuilder.Append($"*[@name=\"{part}\"]");
                }
            }

            XElement element = this._rootTestNode.XPathSelectElement(xPathBuilder.ToString());
            if (element == null) {
                Assert.Fail("Unable to find test '{0}' in the test results file (using XPath {1})", test, xPathBuilder);
            }

            return element;
        }
    }
}
