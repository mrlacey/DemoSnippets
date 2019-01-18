using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DemoSnippets.Tests
{
    public static class AssertExtensions
    {
        public static void ToolboxEntriesAreEqual(this Assert assert, List<ToolboxEntry> expected, List<ToolboxEntry> actual)
        {
            Assert.AreEqual(expected.Count, actual.Count, $"List sizes differ.");

            for (var index = 0; index < expected.Count; index++)
            {
                var exp = expected[index];
                var act = actual[index];
                Assert.AreEqual(exp.Tab, act.Tab, $"Item {index + 1} has differing 'Tab' value.");
                Assert.AreEqual(exp.Label, act.Label, $"Item {index + 1} has differing 'Label' value.");
                Assert.AreEqual(exp.Snippet, act.Snippet, $"Item {index + 1} has differing 'Snippet' value.");
            }
        }
    }
}
