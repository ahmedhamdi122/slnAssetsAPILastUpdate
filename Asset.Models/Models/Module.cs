using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset.Models
{
    public class Module
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string NameAr { get; set; }
        public virtual ICollection<Permission> Permissions { get; set;}
        public virtual ICollection<RoleModulePermissions> RoleModulePermissions { get; set; }

    }
}
