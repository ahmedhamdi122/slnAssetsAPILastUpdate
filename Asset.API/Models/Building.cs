using System;
using System.Collections.Generic;

#nullable disable

namespace Asset.API.Models
{
    public partial class Building
    {
        public Building()
        {
            AssetDetails = new HashSet<AssetDetail>();
            AssetMovements = new HashSet<AssetMovement>();
            Floors = new HashSet<Floor>();
        }

        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string NameAr { get; set; }
        public string Brief { get; set; }
        public string BriefAr { get; set; }
        public int? HospitalId { get; set; }

        public virtual ICollection<AssetDetail> AssetDetails { get; set; }
        public virtual ICollection<AssetMovement> AssetMovements { get; set; }
        public virtual ICollection<Floor> Floors { get; set; }
    }
}
