using System;
using System.Collections.Generic;

#nullable disable

namespace Asset.API.Models
{
    public partial class RolePermission
    {
        public string RoleId { get; set; }
        public int PermissionId { get; set; }

        public virtual Permission Permission { get; set; }
        public virtual AspNetRole Role { get; set; }
    }
}
