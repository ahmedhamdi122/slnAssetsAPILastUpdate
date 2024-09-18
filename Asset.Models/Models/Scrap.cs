using System;
using System.Collections.Generic;

#nullable disable

namespace Asset.API.Models
{
    public partial class Scrap
    {
        public Scrap()
        {
            AssetScraps = new HashSet<AssetScrap>();
            ScrapAttachments = new HashSet<ScrapAttachment>();
        }

        public int Id { get; set; }
        public string ScrapNo { get; set; }
        public int? AssetDetailId { get; set; }
        public string Comment { get; set; }
        public DateTime? ScrapDate { get; set; }
        public DateTime? SysDate { get; set; }

        public virtual AssetDetail AssetDetail { get; set; }
        public virtual ICollection<AssetScrap> AssetScraps { get; set; }
        public virtual ICollection<ScrapAttachment> ScrapAttachments { get; set; }
    }
}
