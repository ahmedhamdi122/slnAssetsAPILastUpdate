
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
       
        public int? id { get; set; }
        public string name { get; set; }
        public string nameAr { get; set; }
        public IEnumerable<permissionVM> Permissions { get; set; }
    }
}
