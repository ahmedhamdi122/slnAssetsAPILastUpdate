using Asset.ViewModels.PermissionVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset.ViewModels.ModuleVM
{
    public class ModulesPermissionsResult
    {
        public IEnumerable<ModuleWithPermissionsVM> results { get; set; }
        public int count { get; set; }

    }
    public class ModuleWithPermissionsVM
    {
        public ModuleWithPermissionsVM(int Id, string Name, string NameAr, IEnumerable<permissionVM> permissions)
        {
            id = Id;
            name = Name;
            nameAr = NameAr;
            Permissions = permissions;
        }
        public int id { get; set; }
        public string name { get; set; }
        public string nameAr { get; set; }
        public IEnumerable<permissionVM> Permissions { get; set; }
    }
}

