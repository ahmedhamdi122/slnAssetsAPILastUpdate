using System;
using System.Collections.Generic;

#nullable disable

namespace Asset.API.Models
{
    public partial class VisitType
    {
        public VisitType()
        {
            Visits = new HashSet<Visit>();
        }

        public int Id { get; set; }
        public string TypeDesc { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string NameAr { get; set; }

        public virtual ICollection<Visit> Visits { get; set; }
    }
}
