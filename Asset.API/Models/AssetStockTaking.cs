using System;
using System.Collections.Generic;

#nullable disable

namespace Asset.API.Models
{
    public partial class AssetStockTaking
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int? HospitalId { get; set; }
        public int? AssetDetailId { get; set; }
        public DateTime? CaptureDate { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longtitude { get; set; }
        public int? StschedulesId { get; set; }
    }
}
