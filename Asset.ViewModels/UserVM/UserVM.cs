using Asset.Models;
using Asset.ViewModels.RoleCategoryVM;
using Asset.ViewModels.RoleVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset.ViewModels.UserVM
{
    public class UserVM
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public ReadRoleCategoryVM RoleCategory {get; set;}
        public string DisplayName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
    }
}
