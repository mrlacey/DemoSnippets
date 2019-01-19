// <copyright file="DemoSnippetsLineTypeIdentifier.cs" company="Matt Lacey Ltd.">
// Copyright (c) Matt Lacey Ltd. All rights reserved.
// </copyright>

using System;

namespace DemoSnippets
{
    public class DemoSnippetsLineTypeIdentifier
    {
        private const string LineStartComment = "#";
        private const string LineStartTab = "tab:";
        private const string LineStartLabel = "-";
        private const string CommentTab = "DEMOSNIPPETS-TAB";
        private const string CommentLabel = "DEMOSNIPPETS-LABEL";
        private const string CommentEndSnippet = "DEMOSNIPPETS-ENDSNIPPET";

        // For some languages (e.g C++) '#' has a separate meaning to a DemoSnippets comment indicator
        // Track if in a file containing subExt formatting so know not to look for standard formatting
        private bool subExtFormattingOnly = false;

        public DemoSnippetsLineType GetLineType(string line)
        {
            if (!this.subExtFormattingOnly && line.StartsWith(LineStartComment))
            {
                return DemoSnippetsLineType.Comment;
            }
            else if (!this.subExtFormattingOnly && line.StartsWith(LineStartLabel))
            {
                return DemoSnippetsLineType.Label;
            }
            else if (line.ToUpperInvariant().Contains(CommentLabel))
            {
                this.subExtFormattingOnly = true;
                return DemoSnippetsLineType.Label;
            }
            else if (!this.subExtFormattingOnly && line.ToLowerInvariant().StartsWith(LineStartTab))
            {
                return DemoSnippetsLineType.Tab;
            }
            else if (line.ToUpperInvariant().Contains(CommentTab))
            {
                this.subExtFormattingOnly = true;
                return DemoSnippetsLineType.Tab;
            }
            else if (line.ToUpperInvariant().Contains(CommentEndSnippet))
            {
                this.subExtFormattingOnly = true;
                return DemoSnippetsLineType.EndSnippet;
            }
            else
            {
                return DemoSnippetsLineType.Other;
            }
        }

        public string GetTabName(string line)
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

        public string GetLabelName(string line)
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
