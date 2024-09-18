using System;
using System.Collections.Generic;

#nullable disable

namespace Asset.API.Models
{
    public partial class ContractDetail
    {
        public int Id { get; set; }
        public int? MasterContractId { get; set; }
        public int? AssetDetailId { get; set; }
        public bool? HasSpareParts { get; set; }
        public DateTime? ContractDate { get; set; }
        public int? ResponseTime { get; set; }
        public int? HospitalId { get; set; }

        public virtual MasterContract MasterContract { get; set; }
    }
}
