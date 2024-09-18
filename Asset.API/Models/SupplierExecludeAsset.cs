using System;
using System.Collections.Generic;

#nullable disable

namespace Asset.API.Models
{
    public partial class SupplierExecludeAsset
    {
        public SupplierExecludeAsset()
        {
            SupplierExecludes = new HashSet<SupplierExeclude>();
        }

        public int Id { get; set; }
        public int? AppTypeId { get; set; }
        public int? AssetId { get; set; }
        public int? StatusId { get; set; }
        public string UserId { get; set; }
        public DateTime? Date { get; set; }
        public DateTime? ExecludeDate { get; set; }
        public string ExNumber { get; set; }
        public DateTime? ActionDate { get; set; }
        public string Comment { get; set; }
        public int? HospitalId { get; set; }

        public virtual ApplicationType AppType { get; set; }
        public virtual HospitalSupplierStatus Status { get; set; }
        public virtual AspNetUser User { get; set; }
        public virtual ICollection<SupplierExeclude> SupplierExecludes { get; set; }
    }
}
