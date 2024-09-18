using System;
using System.Collections.Generic;

#nullable disable

namespace Asset.API.Models
{
    public partial class Visit
    {
        public Visit()
        {
            VisitAttachments = new HashSet<VisitAttachment>();
        }

        public int Id { get; set; }
        public int? EngineerId { get; set; }
        public int? HospitalId { get; set; }
        public DateTime? VisitDate { get; set; }
        public int? VisitTypeId { get; set; }
        public string VisitDescr { get; set; }
        public int? StatusId { get; set; }
        public string Code { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longtitude { get; set; }
        public bool? IsMode { get; set; }

        public virtual Hospital Hospital { get; set; }
        public virtual VisitType VisitType { get; set; }
        public virtual ICollection<VisitAttachment> VisitAttachments { get; set; }
    }
}
