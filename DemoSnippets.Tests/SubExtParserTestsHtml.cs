using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DemoSnippets.Tests
{
    [TestClass]
    public class SubExtParserTestsHtml : TestsBase
    {
        // TODO: add test for - endsnippet with label but no snippet
        // TODO: add test for - no tab
        // TODO: add test for - no label
        // TODO: add test for - endsnippet but snippet is just blank lines
        // TODO: add test for - tab end snippet
        // TODO: add test for - file just endsnippet
        // TODO: add test for - file just tab
        // TODO: add test for - file just label
        // TODO: add test for - file just tab & label
        [TestMethod]
        public void ExampleFile()
        {
            var actual = ParseExampleFile("./Examples/example.demosnippets.html");

            var expected = new List<ToolboxEntry>
            {
                new ToolboxEntry
                {
                    Tab = "Head",
                    Label = "Step 1 - Add Javascript",
                    Snippet = @"    <script src=""./scripts/coolstuff.js""></script>
    <script language=""javascript"" src=""./scripts/Logging.js""></script>"
                },
                new ToolboxEntry
                {
                    Tab = "Head",
                    Label = "Step 2 - Add CSS",
                    Snippet = @"    <link href=""./assets/standard.css"" />"
                },
                new ToolboxEntry
                {
                    Tab = "Body",
                    Label = "Step 3 - Add links",
                    Snippet = @"<ul>
    <li>Home</li>
    <li>Store</li>
    <li>About</li>
    <li>Admin</li>
</ul>"
                },
                new ToolboxEntry
                {
                    Tab = "Body",
                    Label = "Step 4 - Add text",
                    Snippet = @"<p>Some important text to have on the page.</p>"
                },
            };

            Assert.That.ToolboxEntriesAreEqual(expected, actual);
        }
    }
}
