using System;
using System.Collections.Generic;

#nullable disable

namespace Asset.API.Models
{
    public partial class Category
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string NameAr { get; set; }
        public int? CategoryTypeId { get; set; }
    }
}
