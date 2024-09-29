using Asset.ViewModels.ModuleVM;
using Asset.ViewModels.RoleCategoryVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset.ViewModels.RoleVM
{
    public class IndexRoleVM
    {

        public List<GetData> Results { get; set; }

        public int Count { get; set; }
        public class GetData
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string DisplayName { get; set; }
            public string CategoryName { get; set; }
            public EditRoleCategory Category { get; set; }
            public IEnumerable<ModuleWithPermissionsVM> ModuleWithPermissions { get; set; }
        }
    }
}
