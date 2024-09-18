using System;
using System.Collections.Generic;

#nullable disable

namespace Asset.API.Models
{
    public partial class PmassetTask
    {
        public PmassetTask()
        {
            PmassetTaskSchedules = new HashSet<PmassetTaskSchedule>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string NameAr { get; set; }
        public int? MasterAssetId { get; set; }

        public virtual ICollection<PmassetTaskSchedule> PmassetTaskSchedules { get; set; }
    }
}
