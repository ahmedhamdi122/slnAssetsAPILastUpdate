using System;
using System.Collections.Generic;

#nullable disable

namespace Asset.API.Models
{
    public partial class SupplierExeclude
    {
        public int Id { get; set; }
        public int? SupplierExecludeAssetId { get; set; }
        public int? ReasonId { get; set; }
        public int? HospitalId { get; set; }

        public virtual SupplierExecludeReason Reason { get; set; }
        public virtual SupplierExecludeAsset SupplierExecludeAsset { get; set; }
    }
}
