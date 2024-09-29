using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset.ViewModels.ModuleVM
{
    public class ModuleIdWithPermissionsVM
    {
        public int moduleId { get; set; } 
        public IEnumerable<int> permissionIDs { get; set; }
    }
}
