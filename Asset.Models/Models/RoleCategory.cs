using System;
using System.Collections.Generic;

#nullable disable

namespace Asset.API.Models
{
    public partial class RoleCategory
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string NameAr { get; set; }
        public int? OrderId { get; set; }
    }
}
