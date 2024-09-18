using System;
using System.Collections.Generic;

#nullable disable

namespace Asset.API.Models
{
    public partial class SubOrganization
    {
        public SubOrganization()
        {
            Hospitals = new HashSet<Hospital>();
        }

        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string NameAr { get; set; }
        public string Address { get; set; }
        public string AddressAr { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string DirectorName { get; set; }
        public string DirectorNameAr { get; set; }
        public int OrganizationId { get; set; }

        public virtual ICollection<Hospital> Hospitals { get; set; }
    }
}
