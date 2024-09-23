using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset.Models
{
    public class RoleModulePermissions
    {
        public int Id { get; set; }
        public int RoleId { get; set; }
        public ApplicationRole Role { get; set; }
        public int PermissionId { get; set; }
        public Permission Permission { get; set; }
        public int ModuleId { get; set; }
        public Module Module { get; set; }  
    }
}
