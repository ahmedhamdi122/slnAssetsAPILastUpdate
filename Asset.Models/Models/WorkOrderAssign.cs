using System;
using System.Collections.Generic;

#nullable disable

namespace Asset.API.Models
{
    public partial class WorkOrderAssign
    {
        public int Id { get; set; }
        public int? Wotid { get; set; }
        public string UserId { get; set; }
        public int? SupplierId { get; set; }
        public string Notes { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }

        public virtual AspNetUser User { get; set; }
    }
}
