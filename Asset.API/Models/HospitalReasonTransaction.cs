using System;
using System.Collections.Generic;

#nullable disable

namespace Asset.API.Models
{
    public partial class HospitalReasonTransaction
    {
        public HospitalReasonTransaction()
        {
            HospitalApplicationAttachments = new HashSet<HospitalApplicationAttachment>();
        }

        public int Id { get; set; }
        public int? HospitalApplicationId { get; set; }
        public int? ReasonId { get; set; }
        public int? HospitalId { get; set; }

        public virtual HospitalApplication HospitalApplication { get; set; }
        public virtual ICollection<HospitalApplicationAttachment> HospitalApplicationAttachments { get; set; }
    }
}
