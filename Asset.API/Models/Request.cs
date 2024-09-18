using System;
using System.Collections.Generic;

#nullable disable

namespace Asset.API.Models
{
    public partial class Request
    {
        public Request()
        {
            RequestPhases = new HashSet<RequestPhase>();
            RequestTrackings = new HashSet<RequestTracking>();
            WorkOrders = new HashSet<WorkOrder>();
        }

        public int Id { get; set; }
        public string Subject { get; set; }
        public string RequestCode { get; set; }
        public string Description { get; set; }
        public DateTime? RequestDate { get; set; }
        public int? RequestModeId { get; set; }
        public int? SubProblemId { get; set; }
        public int? AssetDetailId { get; set; }
        public int? RequestPeriorityId { get; set; }
        public int? RequestTypeId { get; set; }
        public string CreatedById { get; set; }
        public bool? IsOpened { get; set; }
        public int? HospitalId { get; set; }

        public virtual AspNetUser CreatedBy { get; set; }
        public virtual RequestMode RequestMode { get; set; }
        public virtual RequestPeriority RequestPeriority { get; set; }
        public virtual RequestType RequestType { get; set; }
        public virtual SubProblem SubProblem { get; set; }
        public virtual ICollection<RequestPhase> RequestPhases { get; set; }
        public virtual ICollection<RequestTracking> RequestTrackings { get; set; }
        public virtual ICollection<WorkOrder> WorkOrders { get; set; }
    }
}
