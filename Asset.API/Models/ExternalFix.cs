using System;
using System.Collections.Generic;

#nullable disable

namespace Asset.API.Models
{
    public partial class ExternalFix
    {
        public int Id { get; set; }
        public int? AssetDetailId { get; set; }
        public DateTime? OutDate { get; set; }
        public string ComingNotes { get; set; }
        public int? HospitalId { get; set; }
        public int? SupplierId { get; set; }
        public DateTime? ExpectedDate { get; set; }
        public string Notes { get; set; }
        public DateTime? ComingDate { get; set; }
        public string OutNumber { get; set; }
    }
}
