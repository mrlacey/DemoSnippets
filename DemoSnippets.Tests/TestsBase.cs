using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace DemoSnippets.Tests
{
    public class TestsBase
    {
        internal List<ToolboxEntry> ParseAsLines(string lines)
        {
            var sut = new DemoSnippetsParser();
            return sut.GetItemsToAdd(lines.Split(new[] { Environment.NewLine }, StringSplitOptions.None));
        }

        internal List<ToolboxEntry> ParseExampleFile(string fileName)
        {
            return ParseAsLines(File.ReadAllText(fileName));
        }
    }
}
