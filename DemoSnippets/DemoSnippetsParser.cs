using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoSnippets
{
    public class DemoSnippetsParser
    {
        public List<ItemToAdd> GetItemsToAdd(string[] lines)
        {
            var result = new List<ItemToAdd>();

            ItemToAdd toAdd = null;

            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i];

                if (line.StartsWith("#"))
                {
                    // Ignore comments
                    continue;
                }

                if (line.StartsWith("-"))
                {
                    if (toAdd == null)
                    {
                        toAdd = new ItemToAdd { Label = line.Substring(1).Trim() };
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

                            toAdd = new ItemToAdd { Label = line.Substring(1).Trim() };
                        }
                    }
                }
                else
                {
                    if (toAdd != null)
                    {
                        if (string.IsNullOrWhiteSpace(toAdd.Label))
                        {
                            toAdd.Label = line;
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
