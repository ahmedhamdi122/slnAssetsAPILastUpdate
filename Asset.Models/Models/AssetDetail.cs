using System;
using System.Collections.Generic;

#nullable disable

namespace Asset.API.Models
{
    public partial class AssetDetail
    {
        public AssetDetail()
        {
            Calibrations = new HashSet<Calibration>();
            Scraps = new HashSet<Scrap>();
        }

        public int Id { get; set; }
        public string Code { get; set; }
        public decimal? Price { get; set; }
        public string SerialNumber { get; set; }
        public string Remarks { get; set; }
        public string Barcode { get; set; }
        public DateTime? PurchaseDate { get; set; }
        public decimal? DepreciationRate { get; set; }
        public string CostCenter { get; set; }
        public DateTime? InstallationDate { get; set; }
        public DateTime? OperationDate { get; set; }
        public DateTime? ReceivingDate { get; set; }
        public string Ponumber { get; set; }
        public DateTime? WarrantyStart { get; set; }
        public DateTime? WarrantyEnd { get; set; }
        public string WarrantyExpires { get; set; }
        public int MasterAssetId { get; set; }
        public int? BuildingId { get; set; }
        public int? FloorId { get; set; }
        public int? RoomId { get; set; }
        public int? HospitalId { get; set; }
        public int? DepartmentId { get; set; }
        public int? SupplierId { get; set; }
        public string QrFilePath { get; set; }
        public string CreatedBy { get; set; }
        public decimal? FixCost { get; set; }
        public string QrData { get; set; }
        public int? MasterContractId { get; set; }

        public virtual Building Building { get; set; }
        public virtual Department Department { get; set; }
        public virtual Floor Floor { get; set; }
        public virtual MasterAsset MasterAsset { get; set; }
        public virtual Room Room { get; set; }
        public virtual ICollection<Calibration> Calibrations { get; set; }
        public virtual ICollection<Scrap> Scraps { get; set; }
    }
}
