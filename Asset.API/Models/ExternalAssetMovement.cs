using System;
using System.Collections.Generic;

#nullable disable

namespace Asset.API.Models
{
    public partial class ExternalAssetMovement
    {
        public int Id { get; set; }
        public int? AssetDetailId { get; set; }
        public DateTime? MovementDate { get; set; }
        public string HospitalName { get; set; }
        public string Notes { get; set; }
    }
}
