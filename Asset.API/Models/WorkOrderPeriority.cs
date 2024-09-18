using System;
using System.Collections.Generic;

#nullable disable

namespace Asset.API.Models
{
    public partial class WorkOrderPeriority
    {
        public WorkOrderPeriority()
        {
            WorkOrders = new HashSet<WorkOrder>();
        }

        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string NameAr { get; set; }

        public virtual ICollection<WorkOrder> WorkOrders { get; set; }
    }
}
