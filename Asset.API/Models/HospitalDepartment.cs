using System;
using System.Collections.Generic;

#nullable disable

namespace Asset.API.Models
{
    public partial class HospitalDepartment
    {
        public int Id { get; set; }
        public int? HospitalId { get; set; }
        public int? DepartmentId { get; set; }
        public bool? IsActive { get; set; }

        public virtual Department Department { get; set; }
    }
}
