using System;
using System.Collections.Generic;

#nullable disable

namespace Asset.API.Models
{
    public partial class ContractAttachment
    {
        public int Id { get; set; }
        public int? MasterContractId { get; set; }
        public string DocumentName { get; set; }
        public string FileName { get; set; }
        public int? HospitalId { get; set; }

        public virtual MasterContract MasterContract { get; set; }
    }
}
