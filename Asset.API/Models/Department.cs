using System;
using System.Collections.Generic;

#nullable disable

namespace Asset.API.Models
{
    public partial class Department
    {
        public Department()
        {
            AssetDetails = new HashSet<AssetDetail>();
            HospitalDepartments = new HashSet<HospitalDepartment>();
        }

        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string NameAr { get; set; }

        public virtual ICollection<AssetDetail> AssetDetails { get; set; }
        public virtual ICollection<HospitalDepartment> HospitalDepartments { get; set; }
    }
}
