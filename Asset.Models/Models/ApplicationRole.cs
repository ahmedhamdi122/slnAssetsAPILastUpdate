using Asset.Models;
using Asset.Models.Models;
using Microsoft.AspNetCore.Identity;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Asset.Models
{
    public class ApplicationRole : IdentityRole
    {


        public ApplicationRole() : base()
        {
        }

        public ApplicationRole(string roleName) : base(roleName)
        {
        }

        public  int RoleCategoryId { get; set; }
        public int Counter { get; set; }

        [StringLength(50)]
        public  string DisplayName { get; set; }
        public virtual RoleCategory RoleCategory { get; set; }
        public virtual ICollection<Module> Modules { get; set; }
        public ICollection<RoleModulePermission> RoleModulePermissions { get; set; }
        public ICollection<ApplicationUser> Users { get; set; }
        

    }


}