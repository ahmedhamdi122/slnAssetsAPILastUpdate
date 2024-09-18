using System;
using System.Collections.Generic;

#nullable disable

namespace Asset.API.Models
{
    public partial class WorkOrderTracking
    {
        public WorkOrderTracking()
        {
            WorkOrderAttachments = new HashSet<WorkOrderAttachment>();
        }

        public int Id { get; set; }
        public DateTime? WorkOrderDate { get; set; }
        public DateTime? CreationDate { get; set; }
        public string Notes { get; set; }
        public string CreatedById { get; set; }
        public string AssignedTo { get; set; }
        public int? WorkOrderStatusId { get; set; }
        public int? WorkOrderId { get; set; }
        public DateTime? ActualStartDate { get; set; }
        public DateTime? ActualEndDate { get; set; }
        public DateTime? PlannedStartDate { get; set; }
        public DateTime? PlannedEndDate { get; set; }
        public int? HospitalId { get; set; }

        public virtual AspNetUser CreatedBy { get; set; }
        public virtual WorkOrder WorkOrder { get; set; }
        public virtual WorkOrderStatus WorkOrderStatus { get; set; }
        public virtual ICollection<WorkOrderAttachment> WorkOrderAttachments { get; set; }
    }
}
