using System;
using System.Collections.Generic;

#nullable disable

namespace Asset.API.Models
{
    public partial class MasterContract
    {
        public MasterContract()
        {
            ContractAttachments = new HashSet<ContractAttachment>();
            ContractDetails = new HashSet<ContractDetail>();
        }

        public int Id { get; set; }
        public string Serial { get; set; }
        public string Subject { get; set; }
        public DateTime? ContractDate { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public decimal? Cost { get; set; }
        public int? SupplierId { get; set; }
        public int? HospitalId { get; set; }
        public string Notes { get; set; }
        public int? TotalVisits { get; set; }

        public virtual ICollection<ContractAttachment> ContractAttachments { get; set; }
        public virtual ICollection<ContractDetail> ContractDetails { get; set; }
    }
}
