﻿using System;
using System.Collections.Generic;

#nullable disable

namespace Asset.API.Models
{
    public partial class RequestStatus
    {
        public RequestStatus()
        {
            RequestTrackings = new HashSet<RequestTracking>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string NameAr { get; set; }
        public string Color { get; set; }
        public string Icon { get; set; }

        public virtual ICollection<RequestTracking> RequestTrackings { get; set; }
    }
}
