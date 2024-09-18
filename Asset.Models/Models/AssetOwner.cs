using System;
using System.Collections.Generic;

#nullable disable

namespace Asset.API.Models
{
    public partial class AssetOwner
    {
        public int Id { get; set; }
        public int? EmployeeId { get; set; }
        public int? AssetDetailId { get; set; }
        public int? HospitalId { get; set; }
    }
}
