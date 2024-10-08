using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset.ViewModels.RoleVM
{
    public class RoleVM
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public RoleCategoryNamesVM CategoryName { get; set; }
    }
    public class RoleCategoryNamesVM
    {
       public string Name { get; set; }
       public string NameAr { get; set; }

    }
}
