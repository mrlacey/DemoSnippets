// <copyright file="DemoSnippetsParser.cs" company="Matt Lacey Ltd.">
// Copyright (c) Matt Lacey Ltd. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoSnippets
{
    public class DemoSnippetsParser
    {
        public List<ToolboxEntry> GetItemsToAdd(string[] lines)
        {
            var result = new List<ToolboxEntry>();

            ToolboxEntry toAdd = null;

            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i];

                if (line.StartsWith("#"))
                {
                    // Ignore comments
                    continue;
                }

                if (line.ToLowerInvariant().StartsWith("tab:"))
                {
                    if (toAdd == null)
                    {
                        toAdd = new ToolboxEntry();
                    }

                    if (!string.IsNullOrWhiteSpace(toAdd.Snippet))
                    {
                        toAdd.Snippet = toAdd.Snippet.Trim();

                        result.Add(toAdd);

                        toAdd = new ToolboxEntry();
                    }

                    toAdd.Tab = line.Substring(4).Trim();

                    continue;
                }

                if (line.StartsWith("-"))
                {
                    if (toAdd == null)
                    {
                        toAdd = new ToolboxEntry { Label = line.Substring(1).Trim() };
                    }
                    else
                    {
                        if (string.IsNullOrWhiteSpace(toAdd.Snippet))
                        {
                            toAdd.Label = line.Substring(1).Trim();
                        }
                        else
                        {
                            toAdd.Snippet = toAdd.Snippet.Trim();

                            result.Add(toAdd);

                            var currentTab = toAdd.Tab;

                            toAdd = new ToolboxEntry { Label = line.Substring(1).Trim(), Tab = currentTab };
                        }
                    }
                }
                else
                {
                    if (toAdd != null)
                    {
                        if (string.IsNullOrWhiteSpace(toAdd.Label))
                        {
                            toAdd.Label = line.Trim();
                        }

                        if (string.IsNullOrWhiteSpace(toAdd.Snippet))
                        {
                            toAdd.Snippet = line;
                        }
                        else
                        {
                            toAdd.Snippet += $"{Environment.NewLine}{line}";
                        }
                    }
                }
            }

            if (toAdd != null && !string.IsNullOrWhiteSpace(toAdd.Snippet))
            {
                toAdd.Snippet = toAdd.Snippet.Trim();

                result.Add(toAdd);
            }

            return result;
        }
    }
}
