using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset.ViewModels.RoleVM
{
    public class RoleVM
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public RoleCategoryVM RoleCategory { get; set; }
    }
    public class RoleCategoryVM
    {
        public int Id { get; set; } 
       public string Name { get; set; }
       public string NameAr { get; set; }

    }
}
