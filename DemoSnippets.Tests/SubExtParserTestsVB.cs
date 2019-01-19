using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DemoSnippets.Tests
{
    [TestClass]
    public class SubExtParserTestsVB : TestsBase
    {
        [TestMethod]
        public void ExampleFile()
        {
            var actual = ParseExampleFile("./Examples/example.demosnippets.vb");

            var expected = new List<ToolboxEntry>
            {
                new ToolboxEntry{
                    Tab = "VB Demos",
                    Label = "a) imports",
                    Snippet = "Imports System.Runtime.CompilerServices"
                },
                new ToolboxEntry{
                    Tab = "VB Demos",
                    Label = "b) method",
                    Snippet = "        <Extension>"
      + Environment.NewLine + "        Public Sub FireAndForget(task As Task)"
      + Environment.NewLine + "            ' This method allows you to call an async method without awaiting it."
      + Environment.NewLine + "            ' Use it when you don't want or need to wait for the task to complete."
      + Environment.NewLine + "        End Sub"
                },
            };

            Assert.That.ToolboxEntriesAreEqual(expected, actual);
        }
    }
}
