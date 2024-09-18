using System;
using System.Collections.Generic;

#nullable disable

namespace Asset.API.Models
{
    public partial class ApplicationType
    {
        public ApplicationType()
        {
            HospitalApplications = new HashSet<HospitalApplication>();
            SupplierExecludeAssets = new HashSet<SupplierExecludeAsset>();
        }

        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string NameAr { get; set; }

        public virtual ICollection<HospitalApplication> HospitalApplications { get; set; }
        public virtual ICollection<SupplierExecludeAsset> SupplierExecludeAssets { get; set; }
    }
}
