using System;
using System.Collections.Generic;

#nullable disable

namespace Asset.API.Models
{
    public partial class MasterAssetAttachment
    {
        public int Id { get; set; }
        public int MasterAssetId { get; set; }
        public string Title { get; set; }
        public string FileName { get; set; }
    }
}
