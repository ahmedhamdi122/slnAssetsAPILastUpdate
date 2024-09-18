using System;
using System.Collections.Generic;

#nullable disable

namespace Asset.API.Models
{
    public partial class HospitalSupplierStatus
    {
        public HospitalSupplierStatus()
        {
            HospitalApplications = new HashSet<HospitalApplication>();
            SupplierExecludeAssets = new HashSet<SupplierExecludeAsset>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string NameAr { get; set; }
        public string Color { get; set; }
        public string Icon { get; set; }

        public virtual ICollection<HospitalApplication> HospitalApplications { get; set; }
        public virtual ICollection<SupplierExecludeAsset> SupplierExecludeAssets { get; set; }
    }
}
