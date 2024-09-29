using Asset.ViewModels.ModuleVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset.ViewModels.RoleVM
{
    public class CreateRoleVM
    {
        public int RolecategoryID { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public IEnumerable<ModuleIdWithPermissionsVM> ModuleIdsWithPermissions { get; set; }
    }
}
