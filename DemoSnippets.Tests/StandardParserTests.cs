using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DemoSnippets.Tests
{
    [TestClass]
    public class StandardParserTests : TestsBase
    {
        [TestMethod]
        public void BlankLines_ReturnsEmptyList()
        {
            var lines = ""
+ Environment.NewLine + ""
+ Environment.NewLine + ""
+ Environment.NewLine + "";

            var actual = ParseAsLines(lines);

            Assert.AreEqual(0, actual.Count);
        }

        [TestMethod]
        public void JustComments_ReturnsEmptyList()
        {
            var lines = "# comment line 1"
+ Environment.NewLine + "# comment line 2"
+ Environment.NewLine + "";

            var actual = ParseAsLines(lines);

            Assert.AreEqual(0, actual.Count);
        }

        [TestMethod]
        public void SingleItem_AfterComment()
        {
            var lines = "# comment line 1"
+ Environment.NewLine + ""
+ Environment.NewLine + "- Step 1"
+ Environment.NewLine + ""
+ Environment.NewLine + "snippet line 1"
+ Environment.NewLine + "snippet line 2"
+ Environment.NewLine + "";

            var actual = ParseAsLines(lines);

            Assert.AreEqual(1, actual.Count);
            Assert.AreEqual("Step 1", actual[0].Label);
            Assert.AreEqual($"snippet line 1{Environment.NewLine}snippet line 2", actual[0].Snippet);
            Assert.AreEqual(string.Empty, actual[0].Tab);
        }

        [TestMethod]
        public void TwoItems_AfterComment()
        {
            var lines = "# comment line 1"
+ Environment.NewLine + ""
+ Environment.NewLine + "- Step 1"
+ Environment.NewLine + ""
+ Environment.NewLine + "snippet line 1"
+ Environment.NewLine + "snippet line 2"
+ Environment.NewLine + ""
+ Environment.NewLine + "- Step 2"
+ Environment.NewLine + ""
+ Environment.NewLine + "snippet2 line 1"
+ Environment.NewLine + "snippet2 line 2"
+ Environment.NewLine + "";

            var actual = ParseAsLines(lines);

            Assert.AreEqual(2, actual.Count);
            Assert.AreEqual("Step 1", actual[0].Label);
            Assert.AreEqual($"snippet line 1{Environment.NewLine}snippet line 2", actual[0].Snippet);
            Assert.AreEqual(string.Empty, actual[0].Tab);
            Assert.AreEqual("Step 2", actual[1].Label);
            Assert.AreEqual($"snippet2 line 1{Environment.NewLine}snippet2 line 2", actual[1].Snippet);
            Assert.AreEqual(string.Empty, actual[1].Tab);
        }

        [TestMethod]
        public void FirstLineOfSnippet_UsedAsLabelIfNotDefined()
        {
            var lines = "# comment line 1"
+ Environment.NewLine + ""
+ Environment.NewLine + "-"
+ Environment.NewLine + ""
+ Environment.NewLine + "snippet line 1"
+ Environment.NewLine + "snippet line 2"
+ Environment.NewLine + "";

            var actual = ParseAsLines(lines);

            Assert.AreEqual(1, actual.Count);
            Assert.AreEqual("snippet line 1", actual[0].Label);
            Assert.AreEqual($"snippet line 1{Environment.NewLine}snippet line 2", actual[0].Snippet);
            Assert.AreEqual(string.Empty, actual[0].Tab);
        }

        [TestMethod]
        public void CommentsInSnippetsIgnored()
        {
            var lines = "# comment line 1"
+ Environment.NewLine + ""
+ Environment.NewLine + "- Step 1"
+ Environment.NewLine + ""
+ Environment.NewLine + "#Comment"
+ Environment.NewLine + "snippet line 1"
+ Environment.NewLine + "#Comment"
+ Environment.NewLine + "snippet line 2"
+ Environment.NewLine + "#Comment"
+ Environment.NewLine + "";

            var actual = ParseAsLines(lines);

            Assert.AreEqual(1, actual.Count);
            Assert.AreEqual("Step 1", actual[0].Label);
            Assert.AreEqual($"snippet line 1{Environment.NewLine}snippet line 2", actual[0].Snippet);
            Assert.AreEqual(string.Empty, actual[0].Tab);
        }

        [TestMethod]
        public void DuplicateLabels_NotAnIssue()
        {
            var lines = "# comment line 1"
+ Environment.NewLine + ""
+ Environment.NewLine + "- Add this"
+ Environment.NewLine + ""
+ Environment.NewLine + "snippet line 1"
+ Environment.NewLine + "snippet line 2"
+ Environment.NewLine + ""
+ Environment.NewLine + "- Add this "
+ Environment.NewLine + ""
+ Environment.NewLine + "snippet2 line 1"
+ Environment.NewLine + "snippet2 line 2"
+ Environment.NewLine + "";

            var actual = ParseAsLines(lines);

            Assert.AreEqual(2, actual.Count);
            Assert.AreEqual("Add this", actual[0].Label);
            Assert.AreEqual($"snippet line 1{Environment.NewLine}snippet line 2", actual[0].Snippet);
            Assert.AreEqual(string.Empty, actual[0].Tab);
            Assert.AreEqual("Add this", actual[1].Label);
            Assert.AreEqual($"snippet2 line 1{Environment.NewLine}snippet2 line 2", actual[1].Snippet);
            Assert.AreEqual(string.Empty, actual[1].Tab);
        }

        [TestMethod]
        public void SingleLineSnippet()
        {
            var lines = "- Step 1"
+ Environment.NewLine + ""
+ Environment.NewLine + "snippet line 1"
+ Environment.NewLine + "";

            var actual = ParseAsLines(lines);

            Assert.AreEqual(1, actual.Count);
            Assert.AreEqual("Step 1", actual[0].Label);
            Assert.AreEqual($"snippet line 1", actual[0].Snippet);
            Assert.AreEqual(string.Empty, actual[0].Tab);
        }

        [TestMethod]
        public void ItemMustHaveSnippet()
        {
            var lines = "# comment line 1"
+ Environment.NewLine + ""
+ Environment.NewLine + "- Step 0"
+ Environment.NewLine + "- Step 1"
+ Environment.NewLine + ""
+ Environment.NewLine + "snippet line 1"
+ Environment.NewLine + "snippet line 2"
+ Environment.NewLine + ""
+ Environment.NewLine + "- Step 2"
+ Environment.NewLine + ""
+ Environment.NewLine + "";

            var actual = ParseAsLines(lines);

            Assert.AreEqual(1, actual.Count);
            Assert.AreEqual("Step 1", actual[0].Label);
            Assert.AreEqual($"snippet line 1{Environment.NewLine}snippet line 2", actual[0].Snippet);
            Assert.AreEqual(string.Empty, actual[0].Tab);
        }

        [TestMethod]
        public void EmptyItemsWithoutLabelsIgnored()
        {
            var lines = "# comment line 1"
+ Environment.NewLine + ""
+ Environment.NewLine + "- "
+ Environment.NewLine + "- Step 1"
+ Environment.NewLine + ""
+ Environment.NewLine + "snippet line 1"
+ Environment.NewLine + "snippet line 2"
+ Environment.NewLine + ""
+ Environment.NewLine + "- "
+ Environment.NewLine + ""
+ Environment.NewLine + "";

            var actual = ParseAsLines(lines);

            Assert.AreEqual(1, actual.Count);
            Assert.AreEqual("Step 1", actual[0].Label);
            Assert.AreEqual($"snippet line 1{Environment.NewLine}snippet line 2", actual[0].Snippet);
            Assert.AreEqual(string.Empty, actual[0].Tab);
        }

        [TestMethod]
        public void CanSet_Tab()
        {
            var lines = "# comment line 1"
+ Environment.NewLine + ""
+ Environment.NewLine + "Tab: My Demo"
+ Environment.NewLine + "- Step 1"
+ Environment.NewLine + ""
+ Environment.NewLine + "snippet line 1"
+ Environment.NewLine + "snippet line 2"
+ Environment.NewLine + ""
+ Environment.NewLine + "- "
+ Environment.NewLine + ""
+ Environment.NewLine + "";

            var actual = ParseAsLines(lines);

            Assert.AreEqual(1, actual.Count);
            Assert.AreEqual("Step 1", actual[0].Label);
            Assert.AreEqual($"snippet line 1{Environment.NewLine}snippet line 2", actual[0].Snippet);
            Assert.AreEqual("My Demo", actual[0].Tab);
        }

        [TestMethod]
        public void IfMultipleTabsSetInARow_UseLast()
        {
            var lines = "# comment line 1"
+ Environment.NewLine + ""
+ Environment.NewLine + "Tab: My Demo"
+ Environment.NewLine + "Tab: My second Demo"
+ Environment.NewLine + "Tab: My third Demo"
+ Environment.NewLine + "- Step 1"
+ Environment.NewLine + ""
+ Environment.NewLine + "snippet line 1"
+ Environment.NewLine + "snippet line 2"
+ Environment.NewLine + ""
+ Environment.NewLine + "- "
+ Environment.NewLine + ""
+ Environment.NewLine + "";

            var actual = ParseAsLines(lines);

            Assert.AreEqual(1, actual.Count);
            Assert.AreEqual("Step 1", actual[0].Label);
            Assert.AreEqual($"snippet line 1{Environment.NewLine}snippet line 2", actual[0].Snippet);
            Assert.AreEqual("My third Demo", actual[0].Tab);
        }

        [TestMethod]
        public void TabCanApply_ToMultipleSNippets()
        {
            var lines = "# comment line 1"
+ Environment.NewLine + ""
+ Environment.NewLine + "TAB: DemoX"
+ Environment.NewLine + "- Step 1"
+ Environment.NewLine + ""
+ Environment.NewLine + "snippet line 1"
+ Environment.NewLine + "snippet line 2"
+ Environment.NewLine + ""
+ Environment.NewLine + "- Step 2"
+ Environment.NewLine + ""
+ Environment.NewLine + "snippet2 line 1"
+ Environment.NewLine + "snippet2 line 2"
+ Environment.NewLine + "";

            var actual = ParseAsLines(lines);

            Assert.AreEqual(2, actual.Count);
            Assert.AreEqual("Step 1", actual[0].Label);
            Assert.AreEqual($"snippet line 1{Environment.NewLine}snippet line 2", actual[0].Snippet);
            Assert.AreEqual("DemoX", actual[0].Tab);
            Assert.AreEqual("Step 2", actual[1].Label);
            Assert.AreEqual($"snippet2 line 1{Environment.NewLine}snippet2 line 2", actual[1].Snippet);
            Assert.AreEqual("DemoX", actual[1].Tab);
        }

        [TestMethod]
        public void DifferentTabs_InOneFile()
        {
            var lines = "# comment line 1"
+ Environment.NewLine + ""
+ Environment.NewLine + "TAB: Demo 1"
+ Environment.NewLine + "- Step 1"
+ Environment.NewLine + ""
+ Environment.NewLine + "snippet line 1"
+ Environment.NewLine + "snippet line 2"
+ Environment.NewLine + ""
+ Environment.NewLine + "tab: Demo 2"
+ Environment.NewLine + "- Step 2"
+ Environment.NewLine + ""
+ Environment.NewLine + "snippet2 line 1"
+ Environment.NewLine + "snippet2 line 2"
+ Environment.NewLine + "";

            var actual = ParseAsLines(lines);

            Assert.AreEqual(2, actual.Count);
            Assert.AreEqual("Step 1", actual[0].Label);
            Assert.AreEqual($"snippet line 1{Environment.NewLine}snippet line 2", actual[0].Snippet);
            Assert.AreEqual("Demo 1", actual[0].Tab);
            Assert.AreEqual("Step 2", actual[1].Label);
            Assert.AreEqual($"snippet2 line 1{Environment.NewLine}snippet2 line 2", actual[1].Snippet);
            Assert.AreEqual("Demo 2", actual[1].Tab);
        }

        [TestMethod]
        public void DifferentTabs_InOneFile_PlusLabelInference()
        {
            var lines = "# comment line 1"
+ Environment.NewLine + ""
+ Environment.NewLine + "TAB: Demo 1"
+ Environment.NewLine + "-"
+ Environment.NewLine + ""
+ Environment.NewLine + "snippet line 1"
+ Environment.NewLine + "snippet line 2"
+ Environment.NewLine + ""
+ Environment.NewLine + "tab: Demo 2"
+ Environment.NewLine + "-"
+ Environment.NewLine + ""
+ Environment.NewLine + "snippet2 line 1"
+ Environment.NewLine + "snippet2 line 2"
+ Environment.NewLine + "";

            var actual = ParseAsLines(lines);

            Assert.AreEqual(2, actual.Count);
            Assert.AreEqual("snippet line 1", actual[0].Label);
            Assert.AreEqual($"snippet line 1{Environment.NewLine}snippet line 2", actual[0].Snippet);
            Assert.AreEqual("Demo 1", actual[0].Tab);
            Assert.AreEqual("snippet2 line 1", actual[1].Label);
            Assert.AreEqual($"snippet2 line 1{Environment.NewLine}snippet2 line 2", actual[1].Snippet);
            Assert.AreEqual("Demo 2", actual[1].Tab);
        }
    }
}
