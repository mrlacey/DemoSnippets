using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DemoSnippets.Tests
{
    [TestClass]
    public class SubExtParserTestsCPP : TestsBase
    {
        [TestMethod]
        public void ExampleFile_CPP()
        {
            var actual = ParseExampleFile("./Examples/example.demosnippets.cpp");

            var expected = new List<ToolboxEntry>
            {
                new ToolboxEntry{
                    Tab = "C++",
                    Label = "includes",
                    Snippet = "#include \"pch.h\""
      + Environment.NewLine + "#include \"common.h\""
      + Environment.NewLine + "#include \"ColorChangedEventArgs.h\""
                },
                new ToolboxEntry{
                    Tab = "C++",
                    Label = "OldColor",
                    Snippet = "winrt::Color ColorChangedEventArgs::OldColor()"
      + Environment.NewLine + "{"
      + Environment.NewLine + "	return m_oldColor;"
      + Environment.NewLine + "}"
                },
                new ToolboxEntry{
                    Tab = "C++",
                    Label = "NewColor",
                    Snippet = "void ColorChangedEventArgs::NewColor(winrt::Color const& value)"
      + Environment.NewLine + "{"
      + Environment.NewLine + "	m_newColor = value;"
      + Environment.NewLine + "}"
                },
            };

            Assert.That.ToolboxEntriesAreEqual(expected, actual);
        }
    }
}
