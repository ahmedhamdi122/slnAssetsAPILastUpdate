using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset.Models.Models
{
    public class RoleModulePermission
    {
        public int Id { get; set; } 
        public string RoleId { get; set; }
        public int ModuleId { get; set; }
        public int PermissionId { get; set; }
        public ApplicationRole Role { get; set; }
        public Module Module { get; set; }
        public Permission Permission { get; set; }
    }
}
