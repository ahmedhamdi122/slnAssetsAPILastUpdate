using System;
using System.Collections.Generic;

#nullable disable

namespace Asset.API.Models
{
    public partial class Governorate
    {
        public Governorate()
        {
            Cities = new HashSet<City>();
            Hospitals = new HashSet<Hospital>();
        }

        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string NameAr { get; set; }
        public decimal? Population { get; set; }
        public decimal? Area { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longtitude { get; set; }
        public string Logo { get; set; }

        public virtual ICollection<City> Cities { get; set; }
        public virtual ICollection<Hospital> Hospitals { get; set; }
    }
}
