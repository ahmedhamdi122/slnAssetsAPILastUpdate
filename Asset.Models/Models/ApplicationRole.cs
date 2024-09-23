using Asset.Models;
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

        [StringLength(50)]
        public  string DisplayName { get; set; }
        public virtual RoleCategory RoleCategory { get; set; }
        public virtual ICollection<RoleModulePermissions> RoleModulePermissions { get; set; }

    }


}