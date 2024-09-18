using System;
using System.Collections.Generic;

#nullable disable

namespace Asset.API.Models
{
    public partial class ExternalAssetMovementAttachment
    {
        public int Id { get; set; }
        public int? ExternalAssetMovementId { get; set; }
        public string FileName { get; set; }
        public string Title { get; set; }
    }
}
