using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoSnippets
{
    public class DemoSnippetsParser
    {
        public List<ItemsToAdd> GetItemsToAdd(string[] lines)
        {
            var result = new List<ItemsToAdd>();


            result.Add(new ItemsToAdd { Label = "lbl", Snippet = "add this text" });

            return result;
        }
    }
}
