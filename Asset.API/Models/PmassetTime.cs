using System;
using System.Collections.Generic;

#nullable disable

namespace Asset.API.Models
{
    public partial class PmassetTime
    {
        public PmassetTime()
        {
            PmassetTaskSchedules = new HashSet<PmassetTaskSchedule>();
        }

        public int Id { get; set; }
        public int? AssetDetailId { get; set; }
        public DateTime? Pmdate { get; set; }
        public int? HospitalId { get; set; }

        public virtual ICollection<PmassetTaskSchedule> PmassetTaskSchedules { get; set; }
    }
}
