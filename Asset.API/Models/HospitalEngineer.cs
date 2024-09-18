using System;
using System.Collections.Generic;

#nullable disable

namespace Asset.API.Models
{
    public partial class HospitalEngineer
    {
        public int Id { get; set; }
        public int? EngId { get; set; }
        public int? HospId { get; set; }

        public virtual Hospital Hosp { get; set; }
    }
}
