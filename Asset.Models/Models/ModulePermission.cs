using System;
using System.Collections.Generic;

#nullable disable

namespace Asset.API.Models
{
    public partial class ModulePermission
    {
        public int ModuleId { get; set; }
        public int PermissionId { get; set; }

        public virtual Module Module { get; set; }
        public virtual Permission Permission { get; set; }
    }
}
