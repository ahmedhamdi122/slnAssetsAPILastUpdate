using System;
using System.Collections.Generic;

#nullable disable

namespace Asset.API.Models
{
    public partial class WorkOrderStatus
    {
        public WorkOrderStatus()
        {
            WorkOrderTrackings = new HashSet<WorkOrderTracking>();
        }

        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string NameAr { get; set; }
        public string Color { get; set; }
        public string Icon { get; set; }

        public virtual ICollection<WorkOrderTracking> WorkOrderTrackings { get; set; }
    }
}
