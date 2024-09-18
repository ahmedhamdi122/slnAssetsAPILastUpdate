using System;
using System.Collections.Generic;

#nullable disable

namespace Asset.API.Models
{
    public partial class Hospital
    {
        public Hospital()
        {
            HospitalEngineers = new HashSet<HospitalEngineer>();
            Visits = new HashSet<Visit>();
        }

        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string NameAr { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string ManagerName { get; set; }
        public string ManagerNameAr { get; set; }
        public double? Latitude { get; set; }
        public double? Longtitude { get; set; }
        public string Address { get; set; }
        public string AddressAr { get; set; }
        public int? GovernorateId { get; set; }
        public int? CityId { get; set; }
        public int? OrganizationId { get; set; }
        public int? SubOrganizationId { get; set; }
        public string ContractName { get; set; }
        public DateTime? ContractStart { get; set; }
        public DateTime? ContractEnd { get; set; }

        public virtual City City { get; set; }
        public virtual Governorate Governorate { get; set; }
        public virtual Organization Organization { get; set; }
        public virtual SubOrganization SubOrganization { get; set; }
        public virtual ICollection<HospitalEngineer> HospitalEngineers { get; set; }
        public virtual ICollection<Visit> Visits { get; set; }
    }
}
