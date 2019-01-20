using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DemoSnippets.Tests
{
    [TestClass]
    public class SubExtParserTestsHtml : TestsBase
    {
        [TestMethod]
        public void ExampleFile_HTML()
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

        [TestMethod]
        public void NoTab()
        {
            var html = "<!-- DEMOSNIPPETS-LABEL Step 1 - Add Javascript -->"
+ Environment.NewLine + "<script src=\"./scripts/coolstuff.js\"></script>"
+ Environment.NewLine + "<script language=\"javascript\" src=\"./scripts/Logging.js\"></script>";

            var actual = ParseAsLines(html);

            var expected = new List<ToolboxEntry>
            {
                new ToolboxEntry
                {
                    Tab = string.Empty,
                    Label = "Step 1 - Add Javascript",
                    Snippet = @"<script src=""./scripts/coolstuff.js""></script>"
      + Environment.NewLine + @"<script language=""javascript"" src=""./scripts/Logging.js""></script>"
                },
            };

            Assert.That.ToolboxEntriesAreEqual(expected, actual);
        }

        [TestMethod]
        public void Tab_NoLabel()
        {
            var html = "<!-- DEMOSNIPPETS-TAB Head -->"
+ Environment.NewLine + "<script src=\"./scripts/coolstuff.js\"></script>"
+ Environment.NewLine + "<script language=\"javascript\" src=\"./scripts/Logging.js\"></script>";

            var actual = ParseAsLines(html);

            var expected = new List<ToolboxEntry>
            {
                new ToolboxEntry
                {
                    Tab = "Head",
                    Label = "<script src=\"./scripts/coolstuff.js\"></script>",
                    Snippet = @"<script src=""./scripts/coolstuff.js""></script>"
      + Environment.NewLine + @"<script language=""javascript"" src=""./scripts/Logging.js""></script>"
                },
            };

            Assert.That.ToolboxEntriesAreEqual(expected, actual);
        }

        [TestMethod]
        public void WhitespaceAround_TabAndLabel_IsIgnored()
        {
            var html = "<!--  DEMOSNIPPETS-TAB   Head    -->"
+ Environment.NewLine + "<!--  DEMOSNIPPETS-LABEL   Label1   -->"
+ Environment.NewLine + "<script src=\"./scripts/coolstuff.js\"></script>";

            var actual = ParseAsLines(html);

            var expected = new List<ToolboxEntry>
            {
                new ToolboxEntry
                {
                    Tab = "Head",
                    Label = "Label1",
                    Snippet = @"<script src=""./scripts/coolstuff.js""></script>"
                },
            };

            Assert.That.ToolboxEntriesAreEqual(expected, actual);
        }

        [TestMethod]
        public void NoWhitespaceAround_TabAndLabel_IsFine()
        {
            var html = "<!--DEMOSNIPPETS-TABHead-->"
                       + Environment.NewLine + "<!--DEMOSNIPPETS-LABELLabel1-->"
                       + Environment.NewLine + "<script src=\"./scripts/coolstuff.js\"></script>";

            var actual = ParseAsLines(html);

            var expected = new List<ToolboxEntry>
            {
                new ToolboxEntry
                {
                    Tab = "Head",
                    Label = "Label1",
                    Snippet = @"<script src=""./scripts/coolstuff.js""></script>"
                },
            };

            Assert.That.ToolboxEntriesAreEqual(expected, actual);
        }

        [TestMethod]
        public void JustEndSnippet()
        {
            var html = "<!-- DEMOSNIPPETS-ENDSNIPPET -->";

            var actual = ParseAsLines(html);

            var expected = new List<ToolboxEntry>();

            Assert.That.ToolboxEntriesAreEqual(expected, actual);
        }

        [TestMethod]
        public void JustTabAndEndSnippet()
        {
            var html = "<!-- DEMOSNIPPETS-TAB My Snippets -->"
+ Environment.NewLine + "<!-- DEMOSNIPPETS-ENDSNIPPET -->";

            var actual = ParseAsLines(html);

            var expected = new List<ToolboxEntry>();

            Assert.That.ToolboxEntriesAreEqual(expected, actual);
        }

        [TestMethod]
        public void JustTab()
        {
            var html = "<!-- DEMOSNIPPETS-TAB My Snippets -->";

            var actual = ParseAsLines(html);

            var expected = new List<ToolboxEntry>();

            Assert.That.ToolboxEntriesAreEqual(expected, actual);
        }

        [TestMethod]
        public void JustLabel()
        {
            var html = "<!-- DEMOSNIPPETS-LABEL Step 1 -->";

            var actual = ParseAsLines(html);

            var expected = new List<ToolboxEntry>();

            Assert.That.ToolboxEntriesAreEqual(expected, actual);
        }

        [TestMethod]
        public void JustTabAndLabel()
        {
            var html = "<!-- DEMOSNIPPETS-TAB My Snippets -->"
+ Environment.NewLine + "<!-- DEMOSNIPPETS-LABEL Step 1 -->";

            var actual = ParseAsLines(html);

            var expected = new List<ToolboxEntry>();

            Assert.That.ToolboxEntriesAreEqual(expected, actual);
        }

        [TestMethod]
        public void SnippetClosedWithEndSnippet_ButJustBlankLines()
        {
            var html = "<!-- DEMOSNIPPETS-LABEL Step 1 -->"
+ Environment.NewLine + ""
+ Environment.NewLine + ""
+ Environment.NewLine + ""
+ Environment.NewLine + ""
+ Environment.NewLine + ""
+ Environment.NewLine + ""
+ Environment.NewLine + "<!-- DEMOSNIPPETS-ENDSNIPPET -->";

            var actual = ParseAsLines(html);

            var expected = new List<ToolboxEntry>();

            Assert.That.ToolboxEntriesAreEqual(expected, actual);
        }
    }
}
