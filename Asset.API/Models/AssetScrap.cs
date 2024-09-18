using System;
using System.Collections.Generic;

#nullable disable

namespace Asset.API.Models
{
    public partial class AssetScrap
    {
        public int Id { get; set; }
        public int? ScrapId { get; set; }
        public int? ScrapReasonId { get; set; }

        public virtual Scrap Scrap { get; set; }
        public virtual ScrapReason ScrapReason { get; set; }
    }
}
