using System;
using System.Collections.Generic;
using System.Linq;

namespace CACI_Life.Features.Events.Models
{
    public class SponsorPackageModel
    {
        public string PackageName { get; set; } = string.Empty;
        public decimal PackagePrice { get; set; }
        public string PackageDescription { get; set; } = string.Empty;
        public bool IsActive { get; set; }

        public List<string> DescriptionPoints => 
            string.IsNullOrEmpty(PackageDescription) 
                ? new List<string>() 
                : PackageDescription.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();
    }
}
