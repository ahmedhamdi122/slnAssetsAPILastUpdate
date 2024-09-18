using System;
using System.Collections.Generic;

#nullable disable

namespace Asset.API.Models
{
    public partial class SupplierExecludeReason
    {
        public SupplierExecludeReason()
        {
            SupplierExecludes = new HashSet<SupplierExeclude>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string NameAr { get; set; }

        public virtual ICollection<SupplierExeclude> SupplierExecludes { get; set; }
    }
}
