using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DemoSnippets.Tests
{
    [TestClass]
    public class SubExtParserTestsCSS : TestsBase
    {
        [TestMethod]
        public void ExampleFile()
        {
            var actual = ParseExampleFile("./Examples/example.demosnippets.css");

            var expected = new List<ToolboxEntry>
            {
                new ToolboxEntry{
                    Tab = string.Empty,
                    Label = "Step 1",
                    Snippet = "body {"
      + Environment.NewLine + "    padding-left: 11em;"
      + Environment.NewLine + "    font-family: Georgia, \"Times New Roman\", Times, serif;"
      + Environment.NewLine + "    color: purple;"
      + Environment.NewLine + "    background-color: #d8da3d"
      + Environment.NewLine + "}"
                },
                new ToolboxEntry{
                    Tab = "CSS Stuff",
                    Label = "Step 2",
                    Snippet = "ul.navbar {"
      + Environment.NewLine + "    list-style-type: none;"
      + Environment.NewLine + "    padding: 0;"
      + Environment.NewLine + "    margin: 0;"
      + Environment.NewLine + "    position: absolute;"
      + Environment.NewLine + "    top: 2em;"
      + Environment.NewLine + "    left: 1em;"
      + Environment.NewLine + "    width: 9em }"
                },
                new ToolboxEntry{
                    Tab = "Demo",
                    Label = "Step 3",
                    Snippet = "h1 {"
     + Environment.NewLine + "    font-family: Helvetica, Geneva, Arial,"
     + Environment.NewLine + "                 SunSans-Regular, sans-serif }"
                },
                new ToolboxEntry{
                    Tab = "CSS Stuff",
                    Label = "Step 4",
                    Snippet = "ul.navbar li {"
      + Environment.NewLine + "    background: white;"
      + Environment.NewLine + "    margin: 0.5em 0;"
      + Environment.NewLine + "    padding: 0.3em;"
      + Environment.NewLine + "    border-right: 1em solid black }"
      + Environment.NewLine + "ul.navbar a {"
      + Environment.NewLine + "    text-decoration: none }"
                },
                new ToolboxEntry{
                    Tab = "CSS Stuff",
                    Label = "Step 5",
                    Snippet = "a:link {"
      + Environment.NewLine + "    color: blue }"
      + Environment.NewLine + "a:visited {"
      + Environment.NewLine + "    color: purple }"
                },
            };

            Assert.That.ToolboxEntriesAreEqual(expected, actual);
        }
    }
}