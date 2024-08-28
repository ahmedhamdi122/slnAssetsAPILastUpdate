﻿using System;
using System.Collections.Generic;

namespace Asset.ViewModels.AssetDetailVM
{
    public class ViewAssetDetailVM
    {
        public int Id { get; set; }
        public int MasterAssetId { get; set; }
        public string AssetName { get; set; }
        public string AssetNameAr { get; set; }
        public string Code { get; set; }
        public string MasterCode { get; set; }
        public string PurchaseDate { get; set; }

        public decimal? DepreciationRate { get; set; }

        public string CostCenter { get; set; }


        public string Price { get; set; }

        public string SerialNumber { get; set; }
        public string Serial { get; set; }
        public string Remarks { get; set; }

        public string Barcode { get; set; }

        public string BarCode { get; set; }

        public string InstallationDate { get; set; }

        public string WarrantyExpires { get; set; }
        public string RemainWarrantyExpires { get; set; }

        public string WarrantyExpiresAr { get; set; }
        public string RemainWarrantyExpiresAr { get; set; }



        public int BuildId { get; set; }
        public string BuildName { get; set; }
        public string BuildNameAr { get; set; }

        public int RoomId { get; set; }
        public string RoomName { get; set; }
        public string RoomNameAr { get; set; }
        public int FloorId { get; set; }
        public string FloorName { get; set; }
        public string FloorNameAr { get; set; }

        public string OperationDate { get; set; }
        public string ReceivingDate { get; set; }
        public string PONumber { get; set; }
        public string AssetStatus { get; set; }
        public string AssetStatusAr { get; set; }
        public string DepartmentName { get; set; }
        public string DepartmentNameAr { get; set; }

        public string SupplierName { get; set; }
        public string SupplierNameAr { get; set; }

        public string BrandName { get; set; }
        public string BrandNameAr { get; set; }

        public string PeriorityName { get; set; }
        public string PeriorityNameAr { get; set; }



        public int HospitalId { get; set; }
        public string HospitalName { get; set; }
        public string HospitalNameAr { get; set; }

        public string OriginName { get; set; }
        public string OriginNameAr { get; set; }

        public string CategoryName { get; set; }
        public string CategoryNameAr { get; set; }

        public string SubCategoryName { get; set; }
        public string SubCategoryNameAr { get; set; }

        public string GovernorateName { get; set; }
        public string GovernorateNameAr { get; set; }

        public string CityName { get; set; }
        public string CityNameAr { get; set; }

        public string OrgName { get; set; }
        public string OrgNameAr { get; set; }

        public string SubOrgName { get; set; }
        public string SubOrgNameAr { get; set; }

        public string Length { get; set; }
        public string Height { get; set; }
        public string Width { get; set; }
        public string Weight { get; set; }


        public string ModelNumber { get; set; }
        public string VersionNumber { get; set; }

        public string Description { get; set; }
        public string DescriptionAr { get; set; }
        public int ExpectedLifeTime { get; set; }


        public string WarrantyStart { get; set; }
        public string WarrantyEnd { get; set; }

        public string AssetImg { get; set; }
        public string QrFilePath { get; set; }


        public string MasterAssetName { get; set; }
        public string MasterAssetNameAr { get; set; }

        public string ContractDate { get; set; }
        public string ContractStartDate { get; set; }
        public string ContractEndDate { get; set; }

        public string StrPurchaseDate { get; set; }
        public string StrOperationDate { get; set; }
        public string StrInstallationDate { get; set; }



        public string InContract { get; set; }
        public string InContractAr { get; set; }



        public string ContractFrom { get; set; }
        public string ContractTo { get; set; }

        public List<RequestVM.IndexRequestVM.GetData> ListRequests { get; set; }
        public List<WorkOrderVM.ListWorkOrderVM.GetData> ListWorkOrders { get; set; }
    }


}
