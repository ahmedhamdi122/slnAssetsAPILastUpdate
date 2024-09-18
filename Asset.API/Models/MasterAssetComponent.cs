using System;
using System.Collections.Generic;

#nullable disable

namespace Asset.API.Models
{
    public partial class MasterAssetComponent
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string NameAr { get; set; }
        public string PartNo { get; set; }
        public int? MasterAssetId { get; set; }
        public decimal? Price { get; set; }
        public string Description { get; set; }
        public string DescriptionAr { get; set; }
    }
}
