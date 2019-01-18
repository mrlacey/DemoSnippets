using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DemoSnippets.Tests
{
    public static class AssertExtensions
    {
        public static void ToolboxEntriesAreEqual(this Assert assert, List<ToolboxEntry> expected, List<ToolboxEntry> actual)
        {
            Assert.AreEqual(expected.Count, actual.Count, $"List sizes differ. Expected {expected.Count} but got {actual.Count}.");

            for (var index = 0; index < expected.Count; index++)
            {
                var exp = expected[index];
                var act = actual[index];
                Assert.AreEqual(exp.Tab, act.Tab, $"Item {index} has differing 'Tab' value. Expected '{exp.Tab}' but got '{act.Tab}'.");
                Assert.AreEqual(exp.Label, act.Label, $"Item {index} has differing 'Label' value. Expected '{exp.Label}' but got '{act.Label}'.");
                Assert.AreEqual(exp.Snippet, act.Snippet, $"Item {index} has differing 'Snippet' value. Expected '{exp.Snippet}' but got '{act.Snippet}'.");
            }
        }
    }
}
