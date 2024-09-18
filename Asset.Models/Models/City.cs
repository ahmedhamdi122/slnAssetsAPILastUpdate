using System;
using System.Collections.Generic;

#nullable disable

namespace Asset.API.Models
{
    public partial class City
    {
        public City()
        {
            Hospitals = new HashSet<Hospital>();
        }

        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string NameAr { get; set; }
        public int? GovernorateId { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longtitude { get; set; }

        public virtual Governorate Governorate { get; set; }
        public virtual ICollection<Hospital> Hospitals { get; set; }
    }
}
