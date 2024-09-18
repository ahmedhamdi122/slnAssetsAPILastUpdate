using System;
using System.Collections.Generic;

#nullable disable

namespace Asset.API.Models
{
    public partial class RequestPeriority
    {
        public RequestPeriority()
        {
            Requests = new HashSet<Request>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string NameAr { get; set; }
        public string Color { get; set; }
        public string Icon { get; set; }

        public virtual ICollection<Request> Requests { get; set; }
    }
}
