using System;
using System.Collections.Generic;

#nullable disable

namespace Asset.API.Models
{
    public partial class Room
    {
        public Room()
        {
            AssetDetails = new HashSet<AssetDetail>();
            AssetMovements = new HashSet<AssetMovement>();
        }

        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string NameAr { get; set; }
        public int? FloorId { get; set; }
        public int? HospitalId { get; set; }

        public virtual Floor Floor { get; set; }
        public virtual ICollection<AssetDetail> AssetDetails { get; set; }
        public virtual ICollection<AssetMovement> AssetMovements { get; set; }
    }
}
