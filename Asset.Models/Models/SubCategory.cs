using System;
using System.Collections.Generic;

#nullable disable

namespace Asset.API.Models
{
    public partial class SubCategory
    {
        public SubCategory()
        {
            MasterAssets = new HashSet<MasterAsset>();
        }

        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string NameAr { get; set; }
        public int? CategoryId { get; set; }

        public virtual ICollection<MasterAsset> MasterAssets { get; set; }
    }
}
