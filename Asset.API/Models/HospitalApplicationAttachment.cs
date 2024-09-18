using System;
using System.Collections.Generic;

#nullable disable

namespace Asset.API.Models
{
    public partial class HospitalApplicationAttachment
    {
        public int Id { get; set; }
        public int? HospitalReasonTransactionId { get; set; }
        public string Title { get; set; }
        public string FileName { get; set; }
        public int? HospitalId { get; set; }

        public virtual HospitalReasonTransaction HospitalReasonTransaction { get; set; }
    }
}
