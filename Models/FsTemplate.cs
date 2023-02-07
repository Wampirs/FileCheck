using System;
using System.Collections.Generic;
using System.Linq;

namespace FileCheck.Models
{
    public class FsTemplate
    {
        public List<FsItem> Items { get; set; } = new List<FsItem>();

        public int FileCount { get; set; }

        public int DirCount { get; set; }

        public CheckResult Check(FsTemplate template, bool useHash)
        {
            var result = new CheckResult();
            string Selector(FsItem item) => item.Path;

            result.ExtraElements = Items.ExceptBy(template.Items.Select(Selector), Selector).ToList();

            result.MissingElements = template.Items.ExceptBy(Items.Select(Selector), Selector).ToList();

            if (useHash)
            {
                var EqItems = Items.Except(result.ExtraElements).Except(result.MissingElements).Where(it=>it.IsFolder==false).ToDictionary(it=>it,it=>template.Items.SingleOrDefault(it2=>it.Path==it2.Path));
                foreach (var eqItem in EqItems)
                {
                    if (eqItem.Key.Hash != eqItem.Value.Hash)
                    {
                        result.HashValues.Add(eqItem.Key.Path, Tuple.Create(eqItem.Key.Hash, eqItem.Value.Hash));
                    }
                }
            }

            if (result.MissingElements.Count == 0 && result.ExtraElements.Count == 0&&result.HashValues.Count==0) result.IsValid = true;
            return result;
        }
    }
}
