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
            const string lf = "\n";

            var envnl = Environment.NewLine;

            var splitString = envnl;

            if (lines.Contains(crlf))
            {
                splitString = crlf;
            }
            else if (lines.Contains(cr))
            {
                splitString = cr;
            }
            else if (lines.Contains(lf))
            {
                splitString = lf;
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
