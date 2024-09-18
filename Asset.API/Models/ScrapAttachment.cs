using System;
using System.Collections.Generic;

#nullable disable

namespace Asset.API.Models
{
    public partial class ScrapAttachment
    {
        public int Id { get; set; }
        public int? ScrapId { get; set; }
        public string FileName { get; set; }
        public string Title { get; set; }

        public virtual Scrap Scrap { get; set; }
    }
}
