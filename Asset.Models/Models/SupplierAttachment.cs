using System;
using System.Collections.Generic;

#nullable disable

namespace Asset.API.Models
{
    public partial class SupplierAttachment
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string FileName { get; set; }
        public int? SupplierId { get; set; }
    }
}
