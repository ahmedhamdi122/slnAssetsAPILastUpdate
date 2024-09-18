using System;
using System.Collections.Generic;

#nullable disable

namespace Asset.API.Models
{
    public partial class ManufacturerPmasset
    {
        public int Id { get; set; }
        public int? AssetDetailId { get; set; }
        public DateTime? Pmdate { get; set; }
        public bool? IsDone { get; set; }
        public DateTime? DoneDate { get; set; }
        public DateTime? SysDoneDate { get; set; }
        public string Comment { get; set; }
        public DateTime? DueDate { get; set; }
        public int? HospitalId { get; set; }
        public int? AgencyId { get; set; }
    }
}
