using System;
using System.Collections.Generic;

#nullable disable

namespace Asset.API.Models
{
    public partial class AssetStatusTransaction
    {
        public int Id { get; set; }
        public int? AssetDetailId { get; set; }
        public int? AssetStatusId { get; set; }
        public DateTime? StatusDate { get; set; }
        public int? HospitalId { get; set; }

        public virtual AssetStatus AssetStatus { get; set; }
    }
}
