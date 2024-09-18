using System;
using System.Collections.Generic;

#nullable disable

namespace Asset.API.Models
{
    public partial class StockTakingSchedule
    {
        public int Id { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? CreationDate { get; set; }
        public string UserId { get; set; }
        public string Stcode { get; set; }
    }
}
