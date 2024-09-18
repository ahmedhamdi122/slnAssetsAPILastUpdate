using System;
using System.Collections.Generic;

#nullable disable

namespace Asset.API.Models
{
    public partial class WorkOrder
    {
        public WorkOrder()
        {
            WorkOrderTasks = new HashSet<WorkOrderTask>();
            WorkOrderTrackings = new HashSet<WorkOrderTracking>();
        }

        public int Id { get; set; }
        public string Subject { get; set; }
        public string WorkOrderNumber { get; set; }
        public DateTime? CreationDate { get; set; }
        public DateTime? PlannedStartDate { get; set; }
        public DateTime? PlannedEndDate { get; set; }
        public DateTime? ActualStartDate { get; set; }
        public DateTime? ActualEndDate { get; set; }
        public string Note { get; set; }
        public string CreatedById { get; set; }
        public int? WorkOrderPeriorityId { get; set; }
        public int? WorkOrderTypeId { get; set; }
        public int? RequestId { get; set; }
        public int? HospitalId { get; set; }

        public virtual AspNetUser CreatedBy { get; set; }
        public virtual Request Request { get; set; }
        public virtual WorkOrderPeriority WorkOrderPeriority { get; set; }
        public virtual WorkOrderType WorkOrderType { get; set; }
        public virtual ICollection<WorkOrderTask> WorkOrderTasks { get; set; }
        public virtual ICollection<WorkOrderTracking> WorkOrderTrackings { get; set; }
    }
}
