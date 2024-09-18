using System;
using System.Collections.Generic;

#nullable disable

namespace Asset.API.Models
{
    public partial class PmassetTaskSchedule
    {
        public int Id { get; set; }
        public int? PmassetTimeId { get; set; }
        public int? PmassetTaskId { get; set; }
        public int? HospitalId { get; set; }

        public virtual PmassetTask PmassetTask { get; set; }
        public virtual PmassetTime PmassetTime { get; set; }
    }
}
