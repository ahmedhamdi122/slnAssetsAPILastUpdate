using System;
using System.Collections.Generic;

#nullable disable

namespace Asset.API.Models
{
    public partial class WnpmassetTimeAttachment
    {
        public int Id { get; set; }
        public int? WnpmassetTimeId { get; set; }
        public string FileName { get; set; }
        public string Title { get; set; }
        public int? HospitalId { get; set; }
    }
}
