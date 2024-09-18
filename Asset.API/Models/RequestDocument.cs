using System;
using System.Collections.Generic;

#nullable disable

namespace Asset.API.Models
{
    public partial class RequestDocument
    {
        public int Id { get; set; }
        public string DocumentName { get; set; }
        public string FileName { get; set; }
        public int? RequestTrackingId { get; set; }
        public int? HospitalId { get; set; }

        public virtual RequestTracking RequestTracking { get; set; }
    }
}
