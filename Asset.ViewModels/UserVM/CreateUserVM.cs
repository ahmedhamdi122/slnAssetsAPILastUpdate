using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset.ViewModels.UserVM
{
    public class CreateUserVM
    {
    
            public string UserName { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
            public string PhoneNumber { get; set; }
            public int RoleCategoryId { get; set; }
            public int? GovernorateId { get; set; }
            public int? CityId { get; set; }
            public int OrganizationId { get; set; }
            public int SubOrganizationId { get; set; }
            public int? HospitalId { get; set; }
            public List<string> RoleIds { get; set; }=new List<string>();
    }
}
