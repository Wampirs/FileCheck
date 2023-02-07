using FileCheck.ViewModels.Base;
using System;
using System.Collections.Generic;

namespace FileCheck.Models
{
    public class CheckResult : BaseVM
    {
        public bool IsValid { get; set; }

        public List<FsItem> MissingElements { get; set; } = new List<FsItem>();

        public List<FsItem> ExtraElements { get; set; } = new List<FsItem>();

        public Dictionary<string, Tuple<string, string>> HashValues { get; set; } = new Dictionary<string, Tuple<string, string>>();
    }
}
