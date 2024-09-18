using System;
using System.Collections.Generic;

#nullable disable

namespace Asset.API.Models
{
    public partial class Ecri
    {
        public Ecri()
        {
            MasterAssets = new HashSet<MasterAsset>();
        }

        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string NameAr { get; set; }

        public virtual ICollection<MasterAsset> MasterAssets { get; set; }
    }
}
