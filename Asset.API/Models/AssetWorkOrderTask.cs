using System;
using System.Collections.Generic;

#nullable disable

namespace Asset.API.Models
{
    public partial class AssetWorkOrderTask
    {
        public AssetWorkOrderTask()
        {
            WorkOrderTasks = new HashSet<WorkOrderTask>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string NameAr { get; set; }
        public string Code { get; set; }
        public int MasterAssetId { get; set; }

        public virtual ICollection<WorkOrderTask> WorkOrderTasks { get; set; }
    }
}
