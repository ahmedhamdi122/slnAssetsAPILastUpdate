using Asset.ViewModels.PermissionVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset.ViewModels.ModuleVM
{
    public class ModuleWithPermissionsVM
    {
        public ModuleWithPermissionsVM(int id,string name,string nameAr,IEnumerable<permissionVM>permissions)
        {
            Id = id;
            Name = name;
            NameAr = nameAr;
            Permissions = permissions;
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public string NameAr { get; set; }
        public IEnumerable<permissionVM> Permissions { get; set;}
    }
}
