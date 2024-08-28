﻿
namespace Asset.ViewModels.AssetMovementVM
{
    public class SortAssetMovementVM
    {
        public int Id { get; set; }
        public int HospitalId { get; set; }
        public int AssetId { get; set; }
        public string UserId { get; set; }
        public string RequestCode { get; set; }
        public string BarCode { get; set; }
        public string Serial { get; set; }
        public string AssetName { get; set; }
        public string AssetNameAr { get; set; }
        public string Subject { get; set; }
        public string RequestDate { get; set; }
        public string PeriorityNameAr { get; set; }
        public string PeriorityName { get; set; }
        public string PeriorityColor { get; set; }
        public string PeriorityIcon { get; set; }
        public string StatusName { get; set; }
        public string StatusNameAr { get; set; }
        public string StatusColor { get; set; }
        public string StatusIcon { get; set; }
        public string ModeName { get; set; }
        public string ModeNameAr { get; set; }
        public string ClosedDate { get; set; }
        public string CreatedBy { get; set; }
        public string SortStatus { get; set; }

        public string SortBy { get; set; }
    }
}
