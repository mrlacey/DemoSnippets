using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DemoSnippets.Tests
{
    [TestClass]
    public class SubExtParserTestsCSharp : TestsBase
    {
        [TestMethod]
        public void ExampleFile_CS()
        {
            var actual = ParseExampleFile("./Examples/example.demosnippets.cs");

            var expected = new List<ToolboxEntry>
            {
                new ToolboxEntry{
                    Tab = "My C# Demo",
                    Label = "Step 1",
                    Snippet = "// Add necessary namespace"
      + Environment.NewLine + "usings MyCoolNamespace;"
                },
                new ToolboxEntry{
                    Tab = "My C# Demo",
                    Label = "Step 2",
                    Snippet = "public struct SuperOptions"
      + Environment.NewLine + "{"
      + Environment.NewLine + "    public int Id { get; set; }"
      + Environment.NewLine + "    public string Label { get; set; }"
      + Environment.NewLine + "    public bool IsEnabled { get; set; }"
      + Environment.NewLine + "}"
                },
                new ToolboxEntry{
                    Tab = "My C# Demo",
                    Label = "Step 3",
                    Snippet = "if (UserOptions.TryGetOptions(out SuperOptions[] options))"
      + Environment.NewLine + "{"
      + Environment.NewLine + "    Debug.WriteLine(options.Length);"
      + Environment.NewLine + "}"
                }

            };

            Assert.That.ToolboxEntriesAreEqual(expected, actual);
        }
    }
}
