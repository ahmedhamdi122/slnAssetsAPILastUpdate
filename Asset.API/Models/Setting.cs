using System;
using System.Collections.Generic;

#nullable disable

namespace Asset.API.Models
{
    public partial class Setting
    {
        public int Id { get; set; }
        public string KeyName { get; set; }
        public string KeyValue { get; set; }
        public string KeyValueAr { get; set; }
    }
}
