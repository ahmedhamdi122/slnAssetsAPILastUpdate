using System;
using System.Collections.Generic;

#nullable disable

namespace Asset.API.Models
{
    public partial class AssetStatus
    {
        public AssetStatus()
        {
            AssetStatusTransactions = new HashSet<AssetStatusTransaction>();
        }

        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string NameAr { get; set; }

        public virtual ICollection<AssetStatusTransaction> AssetStatusTransactions { get; set; }
    }
}
