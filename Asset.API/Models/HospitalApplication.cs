using System;
using System.Collections.Generic;

#nullable disable

namespace Asset.API.Models
{
    public partial class HospitalApplication
    {
        public HospitalApplication()
        {
            HospitalReasonTransactions = new HashSet<HospitalReasonTransaction>();
        }

        public int Id { get; set; }
        public string AppNumber { get; set; }
        public int? AssetId { get; set; }
        public int? StatusId { get; set; }
        public string UserId { get; set; }
        public int? AppTypeId { get; set; }
        public DateTime? AppDate { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? ActionDate { get; set; }
        public string Comment { get; set; }
        public int? HospitalId { get; set; }

        public virtual ApplicationType AppType { get; set; }
        public virtual HospitalSupplierStatus Status { get; set; }
        public virtual AspNetUser User { get; set; }
        public virtual ICollection<HospitalReasonTransaction> HospitalReasonTransactions { get; set; }
    }
}
