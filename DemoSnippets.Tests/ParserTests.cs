using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DemoSnippets.Tests
{
    [TestClass]
    public class ParserTests
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
            Assert.AreEqual("Step 2", actual[1].Label);
            Assert.AreEqual($"snippet2 line 1{Environment.NewLine}snippet2 line 2", actual[1].Snippet);
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
            Assert.AreEqual("Add this", actual[1].Label);
            Assert.AreEqual($"snippet2 line 1{Environment.NewLine}snippet2 line 2", actual[1].Snippet);
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
        }

        private List<ItemToAdd> ParseAsLines(string lines)
        {
            var sut = new DemoSnippetsParser();
            return sut.GetItemsToAdd(lines.Split(Environment.NewLine));
        }
    }
}
