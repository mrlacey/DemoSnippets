// <copyright file="DemoSnippetsLineTypeIdentifier.cs" company="Matt Lacey Ltd.">
// Copyright (c) Matt Lacey Ltd. All rights reserved.
// </copyright>

namespace DemoSnippets
{
    public static class DemoSnippetsLineTypeIdentifier
    {
        public static DemoSnippetsLineType GetLineType(string line)
        {
            if (line.StartsWith("#"))
            {
                return DemoSnippetsLineType.Comment;
            }
            else if (line.StartsWith("-"))
            {
                return DemoSnippetsLineType.Label;
            }
            else if (line.ToLowerInvariant().StartsWith("tab:"))
            {
                return DemoSnippetsLineType.Tab;
            }
            else
            {
                return DemoSnippetsLineType.Other;
            }
        }
    }
}
