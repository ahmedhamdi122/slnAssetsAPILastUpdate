using Asset.Models.Models;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Asset.Models
{
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser() : base() { }

        public int? GovernorateId { get; set; }
        public int? CityId { get; set; }
        public int? OrganizationId { get; set; }
        public int? SubOrganizationId { get; set; }
        public int? HospitalId { get; set; }
        //[StringLength(450)]
        //public string RoleId { get; set; }
        public int? RoleCategoryId { get; set; }



        [NotMapped]
        public IEnumerable<string> RoleIds { get; set; }

        [NotMapped]
        public IEnumerable<ApplicationRole> userRoleIds { get; set; }
    }
}
