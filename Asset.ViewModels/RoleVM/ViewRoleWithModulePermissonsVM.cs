using Asset.ViewModels.ModuleVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset.ViewModels.RoleVM
{
    public class ViewRoleWithModulePermissonsVM
    {
        public string RolecategoryName { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public IEnumerable<ModuleNameWithPermissionsVM> ModulesWithPermissions { get; set; }
    }
    public class ModuleNameWithPermissionsVM
    {
        public string name { get; set; }
        public string nameAr { get; set; }
        public IEnumerable<string> Permissions { get; set; }
    }
}
