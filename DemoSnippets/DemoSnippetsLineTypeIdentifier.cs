// <copyright file="DemoSnippetsLineTypeIdentifier.cs" company="Matt Lacey Ltd.">
// Copyright (c) Matt Lacey Ltd. All rights reserved.
// </copyright>

using System;

namespace DemoSnippets
{
    public static class DemoSnippetsLineTypeIdentifier
    {
        private const string LineStartComment = "#";
        private const string LineStartTab = "tab:";
        private const string LineStartLabel = "-";
        private const string CommentTab = "DEMOSNIPPETS-TAB";
        private const string CommentLabel = "DEMOSNIPPETS-LABEL";
        private const string CommentEndSnippet = "DEMOSNIPPETS-ENDSNIPPET";

        public static DemoSnippetsLineType GetLineType(string line)
        {
            if (line.StartsWith(LineStartComment))
            {
                return DemoSnippetsLineType.Comment;
            }
            else if (line.StartsWith(LineStartLabel) || line.ToUpperInvariant().Contains(CommentLabel))
            {
                return DemoSnippetsLineType.Label;
            }
            else if (line.ToLowerInvariant().StartsWith(LineStartTab) || line.ToUpperInvariant().Contains(CommentTab))
            {
                return DemoSnippetsLineType.Tab;
            }
            else if (line.ToUpperInvariant().Contains(CommentEndSnippet))
            {
                return DemoSnippetsLineType.EndSnippet;
            }
            else
            {
                return DemoSnippetsLineType.Other;
            }
        }

        public static string GetTabName(string line)
        {
            if (line.ToLowerInvariant().StartsWith(LineStartTab))
            {
                return line.Substring(4).Trim();
            }
            else
            {
                var startIndex = line.IndexOf(CommentTab, StringComparison.InvariantCultureIgnoreCase);

                return RemoveClosingCommentTabs(line.Substring(startIndex + CommentTab.Length)).Trim();
            }
        }

        public static string GetLabelName(string line)
        {
            if (line.ToLowerInvariant().StartsWith(LineStartLabel))
            {
                return line.Substring(1).Trim();
            }
            else
            {
                var startIndex = line.IndexOf(CommentLabel, StringComparison.InvariantCultureIgnoreCase);

                return RemoveClosingCommentTabs(line.Substring(startIndex + CommentLabel.Length)).Trim();
            }
        }

        private static string RemoveClosingCommentTabs(string input)
        {
            return input.Replace("*/", string.Empty)
                        .Replace("-->", string.Empty)
                        .TrimEnd();
        }
    }
}
