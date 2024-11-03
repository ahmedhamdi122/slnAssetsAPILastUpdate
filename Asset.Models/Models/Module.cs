using Asset.API;
using Asset.Models.Models;
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
        public int SectionId { get; set; }
        public virtual Section Section { get; set; }

        public virtual ICollection<Permission> Permissions { get; set;}
        public virtual ICollection<ApplicationRole> Roles { get; set; }
        public ICollection<RoleModulePermission> RoleModulePermissions { get; set; }
        

    }
}
