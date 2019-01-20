using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DemoSnippets.Tests
{
    [TestClass]
    public class SubExtParserTestsJS : TestsBase
    {
        [TestMethod]
        public void ExampleFile_JS()
        {
            var actual = ParseExampleFile("./Examples/example.demosnippets.js");

            var expected = new List<ToolboxEntry>
            {
                new ToolboxEntry{
                    Tab = "DEMO-DOM",
                    Label = "Step 1",
                    Snippet = "document.getElementById(\"demo\").innerHTML = \"Hello World!\";"
                },
                new ToolboxEntry{
                    Tab = "DEMO-Functions",
                    Label = "Step 1 - Empty function",
                    Snippet = "function myFunction(p1, p2) {"
      + Environment.NewLine + "}"
                },
                new ToolboxEntry{
                    Tab = "DEMO-Functions",
                    Label = "Step 2 - Function body",
                    Snippet = "    return p1 * p2;   // The function returns the product of p1 and p2"
                },
            };

            Assert.That.ToolboxEntriesAreEqual(expected, actual);
        }
    }
}
