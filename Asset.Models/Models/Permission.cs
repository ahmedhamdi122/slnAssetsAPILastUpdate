using System;
using System.Collections.Generic;

#nullable disable

namespace Asset.API.Models
{
    public partial class Permission
    {
        public Permission()
        {
            ModulePermissions = new HashSet<ModulePermission>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }

        public virtual ICollection<ModulePermission> ModulePermissions { get; set; }
    }
}
