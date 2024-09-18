﻿using System;
using System.Collections.Generic;

#nullable disable

namespace Asset.API.Models
{
    public partial class RequestType
    {
        public RequestType()
        {
            Requests = new HashSet<Request>();
        }

        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string NameAr { get; set; }

        public virtual ICollection<Request> Requests { get; set; }
    }
}
