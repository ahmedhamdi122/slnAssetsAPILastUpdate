using System;
using System.Collections.Generic;

#nullable disable

namespace Asset.API.Models
{
    public partial class CommetieeMember
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string NameAr { get; set; }
        public string Mobile { get; set; }
        public string Website { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string AddressAr { get; set; }
    }
}
