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
            const string crlf = "\r\n";
            const string cr = "\r";

            var splitString = Environment.NewLine;

            if (lines.Contains(crlf))
            {
                if (Environment.NewLine != crlf)
                {
                    splitString = crlf;
                }
            }
            else if (lines.Contains(cr))
            {
                if (Environment.NewLine != cr)
                {
                    splitString = cr;
                }
            }

            var sut = new DemoSnippetsParser();

            return sut.GetItemsToAdd(lines.Split(new[] { splitString }, StringSplitOptions.None));
        }

        internal List<ToolboxEntry> ParseExampleFile(string fileName)
        {
            return ParseAsLines(File.ReadAllText(fileName));
        }
    }
}
