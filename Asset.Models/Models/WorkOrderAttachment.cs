using System;
using System.Collections.Generic;

#nullable disable

namespace Asset.API.Models
{
    public partial class WorkOrderAttachment
    {
        public int Id { get; set; }
        public string DocumentName { get; set; }
        public string FileName { get; set; }
        public int? WorkOrderTrackingId { get; set; }
        public int? HospitalId { get; set; }

        public virtual WorkOrderTracking WorkOrderTracking { get; set; }
    }
}
