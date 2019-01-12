// <copyright file="ToolboxEntry.cs" company="Matt Lacey Ltd.">
// Copyright (c) Matt Lacey Ltd. All rights reserved.
// </copyright>

namespace DemoSnippets
{
    public class ToolboxEntry
    {
        public ToolboxEntry()
        {
            this.Label = string.Empty;
            this.Snippet = string.Empty;
            this.Tab = string.Empty;
        }

        public string Label { get; set; }

        public string Snippet { get; set; }

        public string Tab { get; set; }
    }
}
