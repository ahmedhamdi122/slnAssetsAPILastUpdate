using System;
using System.Collections.Generic;

#nullable disable

namespace Asset.API.Models
{
    public partial class Floor
    {
        public Floor()
        {
            AssetDetails = new HashSet<AssetDetail>();
            AssetMovements = new HashSet<AssetMovement>();
            Rooms = new HashSet<Room>();
        }

        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string NameAr { get; set; }
        public int? BuildingId { get; set; }
        public int? HospitalId { get; set; }

        public virtual Building Building { get; set; }
        public virtual ICollection<AssetDetail> AssetDetails { get; set; }
        public virtual ICollection<AssetMovement> AssetMovements { get; set; }
        public virtual ICollection<Room> Rooms { get; set; }
    }
}
