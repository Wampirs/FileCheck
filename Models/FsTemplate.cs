using System.Collections.Generic;
using System.Linq;

namespace FileCheck.Models
{
    public class FsTemplate
    {
        public List<FsItem> Items { get; set; } = new List<FsItem>();

        public int FileCount { get; set; }

        public int DirCount { get; set; }

        public CheckResult Check(FsTemplate template)
        {
            var result = new CheckResult();
            static string Selector(FsItem item) => item.Path;

            result.ExtraElements = Items.ExceptBy(template.Items.Select(Selector), Selector).ToList();

            result.MissingElements = template.Items.ExceptBy(Items.Select(Selector), Selector).ToList();

            if (result.MissingElements.Count == 0 && result.ExtraElements.Count == 0) result.IsValid = true;
            return result;
        }
    }
}
