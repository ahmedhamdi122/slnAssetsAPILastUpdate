using System;
using System.Collections.Generic;

#nullable disable

namespace Asset.API.Models
{
    public partial class VisitAttachment
    {
        public int Id { get; set; }
        public int? VisitId { get; set; }
        public string FileName { get; set; }
        public string Title { get; set; }

        public virtual Visit Visit { get; set; }
    }
}
