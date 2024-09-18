using System;
using System.Collections.Generic;

#nullable disable

namespace Asset.API.Models
{
    public partial class WorkOrderTask
    {
        public int Id { get; set; }
        public string Comment { get; set; }
        public int AssetWorkOrderTaskId { get; set; }
        public int WorkOrderId { get; set; }

        public virtual AssetWorkOrderTask AssetWorkOrderTask { get; set; }
        public virtual WorkOrder WorkOrder { get; set; }
    }
}
