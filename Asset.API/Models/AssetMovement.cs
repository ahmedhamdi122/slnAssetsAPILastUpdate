using System;
using System.Collections.Generic;

#nullable disable

namespace Asset.API.Models
{
    public partial class AssetMovement
    {
        public int Id { get; set; }
        public int AssetDetailId { get; set; }
        public DateTime? MovementDate { get; set; }
        public int? BuildingId { get; set; }
        public int? FloorId { get; set; }
        public int? RoomId { get; set; }
        public string MoveDesc { get; set; }
        public int? HospitalId { get; set; }

        public virtual Building Building { get; set; }
        public virtual Floor Floor { get; set; }
        public virtual Room Room { get; set; }
    }
}
