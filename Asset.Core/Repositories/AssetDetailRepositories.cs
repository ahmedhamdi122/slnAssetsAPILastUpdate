﻿using Asset.Domain.Repositories;
using Asset.Models;
using Asset.ViewModels.AssetDetailAttachmentVM;
using Asset.ViewModels.AssetDetailVM;
using Asset.ViewModels.BrandVM;
using Asset.ViewModels.CityVM;
using Asset.ViewModels.GovernorateVM;
using Asset.ViewModels.HospitalVM;
using Asset.ViewModels.OrganizationVM;
using Asset.ViewModels.PMAssetTaskScheduleVM;
using Asset.ViewModels.PMAssetTaskVM;
using Asset.ViewModels.PmAssetTimeVM;
using Asset.ViewModels.SupplierVM;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using QRCoder;
using System.Drawing;


using System.Threading.Tasks;
using System.IO;
using Asset.ViewModels.RequestVM;
using Asset.ViewModels.WorkOrderVM;
using Microsoft.EntityFrameworkCore;
using Asset.Core.Helpers;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Asset.ViewModels.WorkOrderTrackingVM;

namespace Asset.Core.Repositories
{
    public class AssetDetailRepositories : IAssetDetailRepository
    {
        private ApplicationDbContext _context;
        public AssetDetailRepositories(ApplicationDbContext context)
        {
            _context = context;
        }
        public bool hasAssetWithMasterId(int id)
        {
            return _context.AssetDetails.Any(asset=>asset.MasterAssetId==id);
        }
        public AssetDetail QueryAssetDetailById(int assetId)
        {
            var lstHospitalAssets = _context.AssetDetails.Include(a => a.Supplier)
                                                         .Include(a => a.MasterAsset).Include(a => a.Hospital)
                                                         .Include(a => a.Hospital.Governorate)
                                                         .Include(a => a.Hospital.City)
                                                         .Include(a => a.Hospital.Organization).Include(a => a.Hospital.SubOrganization)
                                                         .Include(a => a.MasterAsset.brand)
                                                         .Include(a => a.MasterAsset.Category)
                                                         .Include(a => a.MasterAsset.SubCategory)
                                                         .Include(a => a.MasterAsset.ECRIS)
                                                         .Include(a => a.MasterAsset.AssetPeriority)
                                                         .Include(a => a.Department)
                                                         .Include(a => a.Building).Include(a => a.Floor).Include(a => a.Room)
                                                         .Include(a => a.MasterAsset.Origin).ToList().Where(a => a.Id == assetId).ToList();
            if (lstHospitalAssets.Count() > 0)
            {
                return lstHospitalAssets[0];
            }

            return null;
        }

        public int Delete(int id)
        {
            var assetDetailObj = _context.AssetDetails.Find(id);
            try
            {
                if (assetDetailObj != null)
                {
                    var lstOwners = _context.AssetOwners.Where(a => a.AssetDetailId == id).ToList();
                    if (lstOwners.Count > 0)
                    {
                        foreach (var ownr in lstOwners)
                        {
                            _context.AssetOwners.Remove(ownr);
                            _context.SaveChanges();
                        }

                    }
                    _context.AssetDetails.Remove(assetDetailObj);
                    return _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                string str = ex.Message;
            }

            return 0;
        }
        public IEnumerable<IndexAssetDetailVM.GetData> GetAll()
        {
            int hospitalTypeNum = 0;
            var lstSettings = _context.Settings.ToList();
            if (lstSettings.Count > 0)
            {
                foreach (var item in lstSettings)
                {
                    if (item.KeyName == "HospitalType")
                        hospitalTypeNum = Convert.ToInt32(item.KeyValue);
                }
            }
            List<IndexAssetDetailVM.GetData> list = new List<IndexAssetDetailVM.GetData>();
            var lstAssetDetails = _context.AssetDetails.Include(a => a.MasterAsset)
                .Include(a => a.MasterAsset.brand)
                .Include(a => a.Department)
                .Include(a => a.Hospital)
                .Include(a => a.Hospital.Governorate)
                .Include(a => a.Hospital.City)
                .ToList();
            if (lstAssetDetails.Count > 0)
            {
                foreach (var asset in lstAssetDetails)
                {
                    IndexAssetDetailVM.GetData item = new IndexAssetDetailVM.GetData();
                    item.Id = asset.Id;
                    item.Code = asset.Code;
                    item.MasterImg = asset.MasterAsset.AssetImg != "" ? asset.MasterAsset.AssetImg : "";
                    item.Model = asset.MasterAsset.ModelNumber;
                    item.Price = asset.Price;
                    item.Serial = asset.SerialNumber;
                    item.BarCode = asset.Barcode;
                    item.SerialNumber = asset.SerialNumber;
                    item.PurchaseDate = asset.PurchaseDate;
                    item.SupplierId = asset.SupplierId;
                    item.DepartmentId = asset.DepartmentId;
                    item.BrandName = asset.MasterAsset.brand != null ? asset.MasterAsset.brand.Name : "";
                    item.BrandNameAr = asset.MasterAsset.brand != null ? asset.MasterAsset.brand.NameAr : "";
                    item.HospitalId = asset.Hospital != null ? asset.Hospital.Id : 0;
                    item.HospitalName = asset.HospitalId > 0 ? asset.Hospital.Name : "";
                    item.HospitalNameAr = asset.HospitalId > 0 ? asset.Hospital.NameAr : "";
                    item.AssetName = asset.MasterAssetId > 0 ? asset.MasterAsset.Name : "";
                    item.AssetNameAr = asset.MasterAssetId > 0 ? asset.MasterAsset.NameAr : "";
                    item.GovernorateName = asset.HospitalId > 0 ? asset.Hospital.Governorate.Name : "";
                    item.GovernorateNameAr = asset.HospitalId > 0 ? asset.Hospital.Governorate.NameAr : "";
                    item.CityName = asset.HospitalId > 0 ? asset.Hospital.City.Name : "";
                    item.CityNameAr = asset.HospitalId > 0 ? asset.Hospital.City.NameAr : "";
                    //item.QrFilePath = asset.QrFilePath;
                    if (hospitalTypeNum == 1)
                    {
                        item.QrFilePath = asset.QrFilePath;
                    }
                    if (hospitalTypeNum == 2)
                    {
                        item.QrFilePath = asset.QrData;
                    }
                    if (hospitalTypeNum == 2)
                    {
                        item.QrFilePath = asset.QrData;
                    }
                    item.QrData = asset.QrData;
                    item.CreatedBy = asset.CreatedBy;
                    item.DepartmentName = asset.Department != null ? asset.Department.Name : "";
                    item.DepartmentNameAr = asset.Department != null ? asset.Department.Name : "";
                    list.Add(item);
                }
            }
            return list;
        }
        public IEnumerable<IndexAssetDetailVM.GetData> GetAssetDetailsByAssetId(int assetId)
        {
            var lstAssetDetails = _context.AssetDetails.ToList().Where(a => a.MasterAssetId == assetId).Select(item => new IndexAssetDetailVM.GetData
            {
                Id = item.Id,
                HospitalId = item.HospitalId,
                Code = item.Code,
                Price = item.Price,
                Serial = item.SerialNumber,
                BarCode = item.Barcode,
                SerialNumber = item.SerialNumber,
                PurchaseDate = item.PurchaseDate,
                CreatedBy = item.CreatedBy,
                HospitalName = _context.Hospitals.Where(a => a.Id == item.HospitalId).ToList().First().Name,
                HospitalNameAr = _context.Hospitals.Where(a => a.Id == item.HospitalId).ToList().First().NameAr,
                AssetName = _context.MasterAssets.Where(a => a.Id == item.MasterAssetId).ToList().First().Name,
                AssetNameAr = _context.MasterAssets.Where(a => a.Id == item.MasterAssetId).ToList().First().NameAr,
                GovernorateName = item.HospitalId > 0 ? item.Hospital.Governorate.Name : "",
                GovernorateNameAr = item.HospitalId > 0 ? item.Hospital.Governorate.NameAr : "",
                CityName = item.HospitalId > 0 ? item.Hospital.City.Name : "",
                CityNameAr = item.HospitalId > 0 ? item.Hospital.City.NameAr : "",
                QrFilePath = item.QrData,
                QrData = item.QrData,
                DepartmentName = item.Department != null ? item.Department.Name : "",
                DepartmentNameAr = item.Department != null ? item.Department.Name : "",

            });
            return lstAssetDetails;
        }


        private static Byte[] BitmapToBytes(Bitmap img, int assetId)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                img.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                img.Save(Directory.GetCurrentDirectory() + "/UploadedAttachments/qrFiles/equipment-" + assetId + ".png", System.Drawing.Imaging.ImageFormat.Png);
                return stream.ToArray();
            }
        }

        public string RemainWarranty(DateTime warrantyEnd)
        {
            DateTime now = DateTime.Today;
            int age = now.Year - warrantyEnd.Year;
            if (now < warrantyEnd.AddYears(age))
                age--;

            return age.ToString();
        }
        public ViewAssetDetailVM ViewAssetDetailByMasterId(int id)
        {
            ViewAssetDetailVM model = new ViewAssetDetailVM();
            var lstHospitalAssets = _context.AssetDetails.Include(a => a.Supplier)
                                                    .Include(a => a.MasterAsset).Include(a => a.Hospital)
                                                    .Include(a => a.Hospital.Governorate)
                                                    .Include(a => a.Hospital.City)
                                                    .Include(a => a.Hospital.Organization).Include(a => a.Hospital.SubOrganization)
                                                    .Include(a => a.MasterAsset.brand)
                                                    .Include(a => a.MasterAsset.Category)
                                                    .Include(a => a.MasterAsset.SubCategory)
                                                    .Include(a => a.MasterAsset.ECRIS)
                                                    .Include(a => a.MasterAsset.AssetPeriority)
                                                    .Include(a => a.Department)
                                                    .Include(a => a.Building).Include(a => a.Floor).Include(a => a.Room)
                                                    .Include(a => a.MasterAsset.Origin).ToList().Where(a => a.Id == id).ToList();
            if (lstHospitalAssets.Count > 0)
            {
                var detailObj = lstHospitalAssets[0];
                model.Id = detailObj.Id;
                model.Code = detailObj.Code;

                model.Price = detailObj.Price.ToString();
                model.SerialNumber = detailObj.SerialNumber;
                model.Remarks = detailObj.Remarks;
                model.Barcode = detailObj.Barcode;

                model.PurchaseDate = detailObj.PurchaseDate != null && detailObj.PurchaseDate.Value.Year != 1900 ? detailObj.PurchaseDate.ToString() : "";
                model.InstallationDate = detailObj.InstallationDate != null && detailObj.InstallationDate.Value.Year != 1900 ? detailObj.InstallationDate.Value.ToShortDateString() : "";
                model.ReceivingDate = detailObj.ReceivingDate != null && detailObj.ReceivingDate.Value.Year != 1900 ? detailObj.ReceivingDate.Value.ToShortDateString() : "";
                model.OperationDate = detailObj.OperationDate != null && detailObj.OperationDate.Value.Year != 1900 ? detailObj.OperationDate.Value.ToShortDateString() : "";
                if (detailObj.WarrantyExpires != null)
                {
                    model.WarrantyExpires = detailObj.WarrantyExpires + " Months";
                    model.WarrantyExpiresAr = detailObj.WarrantyExpires + "  شهر";
                }


                if (detailObj.WarrantyEnd != null)
                {

                    var resultAr = DateTimeExtensions.ToDateStringAr(DateTime.Today.Date, DateTime.Parse(detailObj.WarrantyEnd.Value.Date.ToString()));
                    if (detailObj.WarrantyEnd.Value.Date.Year != 1900)
                    {
                        model.RemainWarrantyExpiresAr = ArabicNumeralHelper.ConvertNumerals(resultAr.ToString());
                    }


                    var result = DateTimeExtensions.ToDateString(DateTime.Parse(detailObj.WarrantyEnd.Value.Date.ToString()), DateTime.Today.Date);
                    if (detailObj.WarrantyEnd.Value.Date.Year != 1900)
                        model.RemainWarrantyExpires = result.ToString();
                }
                model.WarrantyStart = detailObj.WarrantyStart != null ? detailObj.WarrantyStart.Value.ToShortDateString() : "";
                model.WarrantyEnd = detailObj.WarrantyEnd != null ? detailObj.WarrantyEnd.Value.ToShortDateString() : "";





                model.CostCenter = detailObj.CostCenter;
                model.DepreciationRate = detailObj.DepreciationRate;
                model.PONumber = detailObj.PONumber;
                model.QrFilePath = detailObj.QrFilePath;

                model.MasterAssetId = detailObj.MasterAsset.Id;
                model.AssetName = detailObj.MasterAsset.Name;
                model.AssetNameAr = detailObj.MasterAsset.NameAr;
                model.MasterCode = detailObj.MasterAsset.Code;
                model.VersionNumber = detailObj.MasterAsset.VersionNumber;
                model.ModelNumber = detailObj.MasterAsset.ModelNumber;
                model.ExpectedLifeTime = detailObj.MasterAsset.ExpectedLifeTime != null ? (int)detailObj.MasterAsset.ExpectedLifeTime : 0;
                model.Description = detailObj.MasterAsset.Description;
                model.DescriptionAr = detailObj.MasterAsset.DescriptionAr;
                model.Length = detailObj.MasterAsset.Length.ToString();
                model.Width = detailObj.MasterAsset.Width.ToString();
                model.Weight = detailObj.MasterAsset.Weight.ToString();
                model.Height = detailObj.MasterAsset.Height.ToString();
                model.AssetImg = detailObj.MasterAsset.AssetImg;



                List<IndexRequestVM.GetData> lstRequests = new List<IndexRequestVM.GetData>();

                var lstAssetRequests = _context.Request.Where(a => a.AssetDetailId == detailObj.Id).ToList().ToList();
                if (lstAssetRequests.Count > 0)
                {

                    foreach (var item in lstAssetRequests)
                    {
                        IndexRequestVM.GetData reqObj = new IndexRequestVM.GetData();
                        reqObj.RequestCode = item.RequestCode;
                        reqObj.RequestDate = item.RequestDate;
                        reqObj.Subject = item.Subject;
                        var lstWorkOrders = _context.WorkOrders.Where(a => a.RequestId == item.Id).ToList();
                        if (lstWorkOrders.Count > 0)
                        {

                            foreach (var itm in lstWorkOrders)
                            {
                                reqObj.WorkOrderNumber = itm.WorkOrderNumber;
                                reqObj.ActualStartDate = itm.CreationDate;
                                reqObj.WorkOrderSubject = itm.Subject;
                                var lstWOStatus = _context.WorkOrderTrackings.Include(a => a.WorkOrderStatus).Where(a => a.WorkOrder.RequestId == item.Id).ToList().OrderByDescending(a => a.WorkOrderDate.Value.Date).ToList();
                                if (lstWOStatus.Count > 0)
                                {
                                    reqObj.WorkOrderStatusName = lstWOStatus[0].WorkOrderStatus.Name;
                                    reqObj.WorkOrderStatusNameAr = lstWOStatus[0].WorkOrderStatus.NameAr;
                                }
                            }

                        }
                        lstRequests.Add(reqObj);
                    }

                    model.ListRequests = lstRequests;
                }










                var lstAssetStatus = _context.AssetStatusTransactions.Include(a => a.AssetStatus).Where(a => a.AssetDetailId == detailObj.Id).ToList().OrderByDescending(a => a.StatusDate).ToList();
                if (lstAssetStatus.Count > 0)
                {
                    model.AssetStatus = lstAssetStatus[0].AssetStatus.Name;
                    model.AssetStatusAr = lstAssetStatus[0].AssetStatus.NameAr;
                }
                if (detailObj.BuildingId != null)
                {
                    model.BuildId = detailObj.Building.Id;
                    model.BuildName = detailObj.Building.Name;
                    model.BuildNameAr = detailObj.Building.NameAr;
                }
                if (detailObj.Floor != null)
                {
                    model.FloorId = detailObj.Floor.Id;
                    model.FloorName = detailObj.Floor.Name;
                    model.FloorNameAr = detailObj.Floor.NameAr;
                }
                if (detailObj.Room != null)
                {
                    model.RoomId = detailObj.Room.Id;
                    model.RoomName = detailObj.Room.Name;
                    model.RoomNameAr = detailObj.Room.NameAr;
                }
                if (detailObj.Department != null)
                {
                    model.DepartmentName = detailObj.Department.Name;
                    model.DepartmentNameAr = detailObj.Department.NameAr;
                }
                if (detailObj.MasterAsset.Category != null)
                {
                    model.CategoryName = detailObj.MasterAsset.Category.Name;
                    model.CategoryNameAr = detailObj.MasterAsset.Category.NameAr;
                }
                if (detailObj.Hospital != null)
                {
                    model.HospitalId = detailObj.Hospital.Id;
                    model.HospitalName = detailObj.Hospital.Name;
                    model.HospitalNameAr = detailObj.Hospital.NameAr;
                }
                if (detailObj.Hospital.Governorate != null)
                {
                    model.GovernorateName = detailObj.Hospital.Governorate.Name;
                    model.GovernorateNameAr = detailObj.Hospital.Governorate.NameAr;
                }
                if (detailObj.Hospital.City != null)
                {
                    model.CityName = detailObj.Hospital.City.Name;
                    model.CityNameAr = detailObj.Hospital.City.NameAr;
                }
                if (detailObj.Hospital.Organization != null)
                {
                    model.OrgName = detailObj.Hospital.Organization.Name;
                    model.OrgNameAr = detailObj.Hospital.Organization.NameAr;
                }

                if (detailObj.Hospital.SubOrganization != null)
                {
                    model.SubOrgName = detailObj.Hospital.SubOrganization.Name;
                    model.SubOrgNameAr = detailObj.Hospital.SubOrganization.NameAr;
                }
                if (detailObj.Supplier != null)
                {
                    model.SupplierName = detailObj.Supplier.Name;
                    model.SupplierNameAr = detailObj.Supplier.NameAr;
                }
                if (detailObj.MasterAsset.Category != null)
                {
                    model.CategoryName = detailObj.MasterAsset.Category.Name;
                    model.CategoryNameAr = detailObj.MasterAsset.Category.NameAr;
                }
                if (detailObj.MasterAsset.SubCategory != null)
                {
                    model.SubCategoryName = detailObj.MasterAsset.SubCategory.Name;
                    model.SubCategoryNameAr = detailObj.MasterAsset.SubCategory.NameAr;
                }
                if (detailObj.MasterAsset.Origin != null)
                {
                    model.OriginName = detailObj.MasterAsset.Origin.Name;
                    model.OriginNameAr = detailObj.MasterAsset.Origin.NameAr;
                }
                if (detailObj.MasterAsset.brand != null)
                {
                    model.BrandName = detailObj.MasterAsset.brand.Name;
                    model.BrandNameAr = detailObj.MasterAsset.brand.NameAr;
                }


                if (detailObj.MasterAsset.AssetPeriority != null)
                {
                    model.PeriorityName = detailObj.MasterAsset.AssetPeriority.Name;
                    model.PeriorityNameAr = detailObj.MasterAsset.AssetPeriority.NameAr;
                }


                var lstMasterContracts = _context.ContractDetails.Include(a => a.MasterContract).Include(a => a.AssetDetail)
                               .Where(a => a.AssetDetailId == detailObj.Id).ToList();





                if (lstMasterContracts.Count > 0)
                {
                    if (lstMasterContracts[0].MasterContract.ContractDate != null)
                        model.ContractDate = lstMasterContracts[0].MasterContract.ContractDate.Value.ToShortDateString();
                    else
                        model.ContractDate = "";

                    if (lstMasterContracts[0].MasterContract.From != null)
                        model.ContractStartDate = lstMasterContracts[0].MasterContract.From.Value.ToShortDateString();
                    else
                        model.ContractStartDate = "";

                    if (lstMasterContracts[0].MasterContract.To != null)
                        model.ContractEndDate = lstMasterContracts[0].MasterContract.To.Value.ToShortDateString();
                    else
                        model.ContractEndDate = "";
                }
                else
                {
                    model.ContractDate = "";
                    model.ContractStartDate = "";
                    model.ContractEndDate = "";
                }

            }
            return model;
        }




        public IEnumerable<IndexPMAssetTaskScheduleVM.GetData> GetAllPMAssetTaskSchedules(int? hospitalId)
        {
            List<IndexPMAssetTaskScheduleVM.GetData> list = new List<IndexPMAssetTaskScheduleVM.GetData>();


            var lstSchedule = (from detail in _context.AssetDetails
                               join tsktime in _context.PMAssetTimes on detail.Id equals tsktime.AssetDetailId
                               join host in _context.Hospitals on detail.HospitalId equals host.Id
                               join schdl in _context.PMAssetTaskSchedules on tsktime.Id equals schdl.PMAssetTimeId

                               //    _context.PMAssetTimes.Include(a=>a.AssetDetail).Include(a=>a.AssetDetail.Hospital).Include(a=>a.)

                               // where tsktime.PMDate.Value.Month == DateTime.Today.Date.Month
                               where tsktime.PMDate.Value.Year == DateTime.Today.Date.Year
                               select new
                               {
                                   Id = schdl.Id,
                                   TimeId = tsktime.Id,
                                   MasterAssetId = detail.MasterAssetId,
                                   AssetDetailId = detail.Id,
                                   HospitalId = host.Id,
                                   Serial = detail.SerialNumber,
                                   StartDate = tsktime.PMDate,
                                   EndDate = tsktime.PMDate,
                                   start = tsktime.PMDate.Value.ToString(),
                                   end = tsktime.PMDate.Value.ToString(),
                                   allDay = true
                               }).ToList()
                               .GroupBy(a => new
                               {
                                   assetId = a.AssetDetailId,
                                   Day = a.StartDate.Value.Day,
                                   Month = a.StartDate.Value.Month,
                                   Year = a.StartDate.Value.Year
                               });



            if (hospitalId == 0)
            {

                lstSchedule = lstSchedule.ToList();
            }

            else
            {
                lstSchedule = lstSchedule.Where(a => a.FirstOrDefault().HospitalId == hospitalId).ToList();
            }

            foreach (var items in lstSchedule)
            {
                string month = "";
                string day = "";
                string endmonth = "";
                string endday = "";


                if (items.FirstOrDefault().StartDate.Value.Month < 10)
                    month = items.FirstOrDefault().StartDate.Value.Month.ToString().PadLeft(2, '0');
                else
                    month = items.FirstOrDefault().StartDate.Value.Month.ToString();

                if (items.FirstOrDefault().EndDate.Value.Month < 10)
                    endmonth = items.FirstOrDefault().EndDate.Value.Month.ToString().PadLeft(2, '0');
                else
                    endmonth = items.FirstOrDefault().EndDate.Value.Month.ToString();

                if (items.FirstOrDefault().StartDate.Value.Day < 10)
                    day = items.FirstOrDefault().StartDate.Value.Day.ToString().PadLeft(2, '0');
                else
                    day = items.FirstOrDefault().StartDate.Value.Day.ToString();

                if (items.FirstOrDefault().EndDate.Value.Day < 10)
                    endday = (items.FirstOrDefault().EndDate.Value.Day).ToString().PadLeft(2, '0');
                else
                    endday = items.FirstOrDefault().EndDate.Value.Day.ToString();




                IndexPMAssetTaskScheduleVM.GetData getDataObj = new IndexPMAssetTaskScheduleVM.GetData();
                var AssetName = _context.MasterAssets.Where(a => a.Id == items.FirstOrDefault().MasterAssetId).ToList().FirstOrDefault().Name;
                var AssetNameAr = _context.MasterAssets.Where(a => a.Id == items.FirstOrDefault().MasterAssetId).ToList().FirstOrDefault().NameAr;
                var color = _context.MasterAssets.Where(a => a.Id == items.FirstOrDefault().MasterAssetId).ToList().FirstOrDefault().PMBGColor;
                var textColor = _context.MasterAssets.Where(a => a.Id == items.FirstOrDefault().MasterAssetId).ToList().FirstOrDefault().PMColor;
                var Serial = items.FirstOrDefault().Serial;
                getDataObj.start = items.FirstOrDefault().StartDate.Value.Year + "-" + month + "-" + day;
                getDataObj.end = items.FirstOrDefault().EndDate.Value.Year + "-" + endmonth + "-" + endday;
                getDataObj.title = AssetName + " - " + Serial;
                getDataObj.titleAr = AssetNameAr + "  -  " + Serial;




                getDataObj.Id = items.FirstOrDefault().Id;
                getDataObj.color = color;
                getDataObj.textColor = textColor;
                // getDataObj.Serial = Serial;
                getDataObj.start = items.FirstOrDefault().StartDate.Value.Year + "-" + month + "-" + day;
                getDataObj.end = items.FirstOrDefault().EndDate.Value.Year + "-" + endmonth + "-" + endday;
                getDataObj.allDay = true;
                getDataObj.HospitalId = (int)hospitalId;
                getDataObj.ListTasks = _context.PMAssetTasks.Where(a => a.MasterAssetId == items.FirstOrDefault().MasterAssetId).ToList();

                list.Add(getDataObj);

            }
            return list;
        }
        public IEnumerable<AssetDetail> ViewAllAssetDetailByMasterId(int MasterAssetId)
        {
            return _context.AssetDetails.Where(a => a.MasterAssetId == MasterAssetId).ToList();
        }
        public IEnumerable<AssetDetail> GetAllAssetDetailsByHospitalId(int hospitalId)
        {
            var lstAssetDetails = _context.AssetDetails.Where(a => a.HospitalId == hospitalId).ToList();
            return lstAssetDetails;
        }

        public List<CountAssetVM> CountAssetsByHospital()
        {
            List<CountAssetVM> list = new List<CountAssetVM>();
            var lstAssetDetails = _context.AssetDetails.Include(a => a.MasterAsset).Include(a => a.Hospital).Take(5).ToList().GroupBy(a => a.MasterAssetId);
            foreach (var asset in lstAssetDetails)
            {

                CountAssetVM countAssetObj = new CountAssetVM();
                countAssetObj.AssetName = asset.FirstOrDefault().MasterAsset.Name;
                countAssetObj.AssetNameAr = asset.FirstOrDefault().MasterAsset.NameAr;
                countAssetObj.AssetPrice = _context.AssetDetails.Where(a => a.HospitalId == asset.FirstOrDefault().HospitalId).Sum(a => Convert.ToDecimal(asset.FirstOrDefault().Price.ToString()));
                countAssetObj.CountAssetsByHospital = _context.AssetDetails.Where(a => a.HospitalId == asset.FirstOrDefault().HospitalId && a.MasterAssetId == asset.FirstOrDefault().MasterAssetId).Count();
                list.Add(countAssetObj);

            }
            return list;
        }
        public List<PmDateGroupVM> GetAllwithgrouping(int assetId)
        {

            List<PmDateGroupVM> snanns = new List<PmDateGroupVM>();

            var assetObj = _context.AssetDetails.Find(assetId);
            var lstTasks = _context.PMAssetTasks.Where(a => a.MasterAssetId == assetObj.MasterAssetId).ToList();

            var assetTasks = _context.PMAssetTimes.Where(a => a.AssetDetailId == assetId).ToList().GroupBy(
                          e => new
                          {
                              day = e.PMDate.Value.Day,
                              month = e.PMDate.Value.Month,
                              year = e.PMDate.Value.Year
                          }
                      ).ToList();


            if (assetTasks.Count > 0)
            {
                foreach (var item in assetTasks)
                {
                    PmDateGroupVM dates = new PmDateGroupVM();
                    dates.PMDate = item.FirstOrDefault().PMDate;
                    dates.PMAssetTimeId = item.FirstOrDefault().Id;
                    dates.Id = item.FirstOrDefault().Id;
                    dates.AssetDetailId = (int)item.FirstOrDefault().AssetDetailId;

                    List<ListPMAssetTaskScheduleVM.GetData> list = new List<ListPMAssetTaskScheduleVM.GetData>();
                    if (lstTasks.Count > 0)
                    {
                        foreach (var tsk in lstTasks)
                        {
                            ListPMAssetTaskScheduleVM.GetData getDataObj = new ListPMAssetTaskScheduleVM.GetData();
                            getDataObj.MasterAssetId = int.Parse(tsk.MasterAssetId.ToString());
                            getDataObj.TaskNameAr = tsk.NameAr;
                            getDataObj.TaskName = tsk.Name;

                            var lstSchedules = _context.PMAssetTaskSchedules.Where(a => a.PMAssetTaskId == tsk.Id).ToList();
                            foreach (var schedule in lstSchedules)
                            {
                                getDataObj.Checked = schedule.PMAssetTaskId == tsk.Id ? true : false;

                            }
                            list.Add(getDataObj);
                        }
                    }

                    dates.AssetSchduleList = list;
                    snanns.Add(dates);

                    //if (dates.AssetSchduleList.Count > 0)
                    //{
                    //    snanns.Add(dates);
                    //}
                    //else
                    //{
                    //    continue;
                    //}
                }
            }

            return snanns;
        }
        public List<IndexAssetDetailVM.GetData> FilterAsset(filterDto data)
        {
            List<IndexAssetDetailVM.GetData> lstAssetDetails = new List<IndexAssetDetailVM.GetData>();
            var Asset = new List<IndexAssetDetailVM.GetData>();
            lstAssetDetails = _context.AssetDetails.Include(a => a.MasterAsset)
                .Include(a => a.MasterAsset.brand).Include(a => a.MasterAsset.Category).Include(a => a.MasterAsset.AssetPeriority)
                        .Include(a => a.Hospital).ThenInclude(h => h.Organization)
                        .Include(a => a.Hospital).ThenInclude(h => h.Governorate)
                        .Include(a => a.Hospital).ThenInclude(h => h.City)
                        .Include(a => a.Hospital).ThenInclude(h => h.SubOrganization)
                        .OrderBy(a => a.Barcode)
                                             .Select(a => new IndexAssetDetailVM.GetData
                                             {
                                                 Id = a.Id,
                                                 Code = a.Code,
                                                 Price = a.Price,
                                                 CreatedBy = a.CreatedBy,
                                                 Serial = a.SerialNumber,
                                                 SerialNumber = a.SerialNumber,
                                                 MasterAssetId = a.MasterAssetId,
                                                 PurchaseDate = a.PurchaseDate,
                                                 HospitalId = a.HospitalId,
                                                 HospitalName = a.Hospital.Name,
                                                 HospitalNameAr = a.Hospital.NameAr,
                                                 AssetName = a.MasterAsset.Name,
                                                 AssetNameAr = a.MasterAsset.NameAr,
                                                 GovernorateId = a.Hospital.Governorate.Id,
                                                 GovernorateName = a.Hospital.Governorate.Name,
                                                 GovernorateNameAr = a.Hospital.Governorate.NameAr,
                                                 CityId = a.Hospital.City.Id,
                                                 CityName = a.Hospital.City.Name,
                                                 CityNameAr = a.Hospital.City.NameAr,
                                                 OrganizationId = a.Hospital.Organization.Id,
                                                 OrgName = a.Hospital.Organization.Name,
                                                 OrgNameAr = a.Hospital.Organization.NameAr,
                                                 SubOrgName = a.Hospital.SubOrganization.Name,
                                                 SubOrgNameAr = a.Hospital.SubOrganization.NameAr,
                                                 BrandId = a.MasterAsset.brand.Id,
                                                 BrandName = a.MasterAsset.brand.Name,
                                                 BrandNameAr = a.MasterAsset.brand.NameAr,
                                                 SupplierId = a.Supplier.Id,
                                                 SupplierName = a.Supplier.Name,
                                                 SupplierNameAr = a.Supplier.NameAr,


                                                 CategoryName = a.MasterAsset.Category.Name,
                                                 CategoryNameAr = a.MasterAsset.Category.NameAr,


                                                 AssetPeriorityName = a.MasterAsset.AssetPeriority.Name,
                                                 AssetPeriorityNameAr = a.MasterAsset.AssetPeriority.NameAr,


                                             }).ToList();

            if (data.name != null && data.name != "")
            {
                lstAssetDetails = lstAssetDetails.Where(e => e.AssetName == data.name || e.AssetNameAr == data.name).ToList();
            }
            else
            {
                lstAssetDetails = lstAssetDetails.ToList();
            }



            if (data.CategoryName != null && data.CategoryName != "")
            {
                lstAssetDetails = lstAssetDetails.Where(e => e.CategoryName == data.CategoryName || e.CategoryNameAr == data.CategoryName).ToList();
            }
            else
            {
                lstAssetDetails = lstAssetDetails.ToList();
            }



            if (data.AssetPeriorityName != null && data.AssetPeriorityName != "")
            {
                lstAssetDetails = lstAssetDetails.Where(e => e.AssetPeriorityName == data.AssetPeriorityName || e.AssetPeriorityNameAr == data.AssetPeriorityName).ToList();
            }
            else
            {
                lstAssetDetails = lstAssetDetails.ToList();
            }


            if (data.name != null && data.name != "")
            {
                lstAssetDetails = lstAssetDetails.Where(e => e.AssetName == data.name || e.AssetNameAr == data.name).ToList();
            }
            else
            {
                lstAssetDetails = lstAssetDetails.ToList();
            }






            if (data.brandName != null && data.brandName != "")
            {
                lstAssetDetails = lstAssetDetails.Where(e => e.BrandName == data.brandName || e.BrandNameAr == data.brandName).ToList();
            }
            else
            {
                lstAssetDetails = lstAssetDetails.ToList();
            }
            if (data.govName != null && data.govName != "")
            {
                lstAssetDetails = lstAssetDetails.Where(e => e.GovernorateName == data.govName || e.GovernorateNameAr == data.govName).ToList();
            }
            else
            {
                lstAssetDetails = lstAssetDetails.ToList();
            }
            if (data.cityName != null && data.cityName != "")
            {
                lstAssetDetails = lstAssetDetails.Where(e => e.CityName == data.cityName || e.CityNameAr == data.cityName).ToList();
            }
            if (data.hosName != null && data.hosName != "")
            {
                lstAssetDetails = lstAssetDetails.Where(e => e.HospitalName == data.hosName || e.HospitalNameAr == data.hosName).ToList();
            }
            else
            {
                lstAssetDetails = lstAssetDetails.ToList();
            }
            if (data.SupplierName != null && data.SupplierName != "")
            {
                lstAssetDetails = lstAssetDetails.Where(e => e.SupplierName == data.SupplierName || e.SupplierNameAr == data.SupplierName).ToList();
            }
            else
            {
                lstAssetDetails = lstAssetDetails.ToList();
            }
            if (data.purchaseDate != null)
            {
                lstAssetDetails = lstAssetDetails.Where(e => e.PurchaseDate == data.purchaseDate || e.PurchaseDate == data.purchaseDate).ToList();
            }
            else
            {
                lstAssetDetails = lstAssetDetails.ToList();
            }

            if (lstAssetDetails.Count > 0)
            {
                foreach (var item in lstAssetDetails)
                {
                    var As = new IndexAssetDetailVM.GetData
                    {
                        Id = item.Id,
                        Code = item.Code,
                        Price = item.Price,
                        Serial = item.SerialNumber,
                        SerialNumber = item.SerialNumber,
                        MasterAssetId = item.MasterAssetId,
                        PurchaseDate = item.PurchaseDate,
                        HospitalId = item.HospitalId,
                        HospitalName = item.HospitalName,
                        HospitalNameAr = item.HospitalNameAr,
                        AssetName = item.AssetName,
                        AssetNameAr = item.AssetNameAr,
                        GovernorateId = item.GovernorateId,
                        GovernorateName = item.GovernorateName,
                        GovernorateNameAr = item.GovernorateNameAr,
                        CityId = item.CityId,
                        CityName = item.CityName,
                        CityNameAr = item.CityNameAr,
                        OrganizationId = item.OrganizationId,
                        OrgName = item.OrgName,
                        OrgNameAr = item.OrgNameAr,
                        SubOrgName = item.SubOrgName,
                        SubOrgNameAr = item.SubOrgNameAr,
                        BrandId = item.BrandId,
                        BrandName = item.BrandName,
                        BrandNameAr = item.BrandNameAr,
                        SupplierId = item.SupplierId,
                        SupplierName = item.SupplierName,
                        SupplierNameAr = item.SupplierNameAr
                    };
                    Asset.Add(As);
                }
                return Asset;
            }
            return null;
        }
        public List<IndexAssetDetailVM.GetData> FilterDataByDepartmentBrandSupplierId(FilterHospitalAsset data)
        {
            List<IndexAssetDetailVM.GetData> list = new List<IndexAssetDetailVM.GetData>();
            var lstAssetDetails = _context.AssetDetails.Include(a => a.MasterAsset).Include(a => a.MasterAsset.brand)
                         .Include(a => a.MasterAsset.Category)
                         .Include(a => a.MasterAsset.AssetPeriority)
                         .Include(a => a.Supplier).Include(a => a.Department)
                         //.Include(a => a.Hospital)
                         //.Include(h => h.Hospital.Organization)
                         //.Include(h => h.Hospital.Governorate)
                         //.Include(h => h.Hospital.City)
                         //.Include(h => h.Hospital.SubOrganization)
                         .Include(a => a.Hospital).ThenInclude(h => h.Organization)
                         .Include(a => a.Hospital).ThenInclude(h => h.Governorate)
                         .Include(a => a.Hospital).ThenInclude(h => h.City)
                         .Include(a => a.Hospital).ThenInclude(h => h.SubOrganization)
                         .OrderBy(a => a.Barcode).ToList();

            foreach (var item in lstAssetDetails)
            {
                IndexAssetDetailVM.GetData getDataobj = new IndexAssetDetailVM.GetData();
                getDataobj.Id = item.Id;
                getDataobj.CreatedBy = item.CreatedBy;
                getDataobj.Code = item.Code;
                getDataobj.Price = item.Price;
                getDataobj.Barcode = item.Barcode;
                getDataobj.Serial = item.SerialNumber;
                getDataobj.SerialNumber = item.SerialNumber;
                getDataobj.BarCode = item.Barcode;
                getDataobj.Model = item.MasterAsset.ModelNumber;
                getDataobj.MasterAssetId = item.MasterAssetId;
                getDataobj.PurchaseDate = item.PurchaseDate;
                getDataobj.HospitalId = item.HospitalId;
                getDataobj.DepartmentId = item.DepartmentId;
                getDataobj.HospitalName = item.Hospital.Name;
                getDataobj.HospitalNameAr = item.Hospital.NameAr;
                getDataobj.AssetName = item.MasterAsset.Name;
                getDataobj.AssetNameAr = item.MasterAsset.NameAr;
                getDataobj.GovernorateId = item.Hospital.Governorate.Id;
                getDataobj.GovernorateName = item.Hospital.Governorate.Name;
                getDataobj.GovernorateNameAr = item.Hospital.Governorate.NameAr;
                getDataobj.CityId = item.Hospital.City.Id;
                getDataobj.CityName = item.Hospital.City.Name;
                getDataobj.CityNameAr = item.Hospital.City.NameAr;
                getDataobj.OrganizationId = item.Hospital.Organization.Id;
                getDataobj.OrgName = item.Hospital.Organization.Name;
                getDataobj.OrgNameAr = item.Hospital.Organization.NameAr;
                getDataobj.SubOrgName = item.Hospital.SubOrganization.Name;
                getDataobj.SubOrgNameAr = item.Hospital.SubOrganization.NameAr;
                getDataobj.SubOrganizationId = item.Hospital.SubOrganization.Id;
                getDataobj.BrandId = item.MasterAsset.brand != null ? item.MasterAsset.brand.Id : 0;
                getDataobj.BrandName = item.MasterAsset.brand != null ? item.MasterAsset.brand.Name : "";
                getDataobj.BrandNameAr = item.MasterAsset.brand != null ? item.MasterAsset.brand.NameAr : "";
                getDataobj.SupplierId = item.Supplier != null ? item.Supplier.Id : 0;
                getDataobj.SupplierName = item.Supplier != null ? item.Supplier.Name : "";
                getDataobj.SupplierNameAr = item.Supplier != null ? item.Supplier.NameAr : "";
                getDataobj.DepartmentName = item.Department != null ? item.Department.Name : "";
                getDataobj.DepartmentNameAr = item.Department != null ? item.Department.NameAr : "";

                getDataobj.PeriorityId = item.MasterAsset.PeriorityId;
                getDataobj.CategoryId = item.MasterAsset.CategoryId;
                var lstTransactions = _context.AssetStatusTransactions.Where(a => a.AssetDetailId == item.Id).OrderByDescending(a => a.StatusDate.Value.Date).ToList();
                if (lstTransactions.Count > 0)
                {
                    getDataobj.AssetStatusId = lstTransactions[0].AssetStatusId;
                }
                list.Add(getDataobj);
            }


            if (data.CategoryId != 0)
            {
                list = list.Where(e => e.CategoryId == data.CategoryId).ToList();
            }
            else
            {
                list = list.ToList();
            }

            if (data.PeriorityId != 0)
            {
                list = list.Where(e => e.PeriorityId == data.PeriorityId).ToList();
            }
            else
            {
                list = list.ToList();
            }

            if (data.DepartmentId != 0)
            {
                list = list.Where(e => e.DepartmentId == data.DepartmentId).ToList();
            }
            else
            {
                list = list.ToList();
            }
            if (data.BrandId != 0)
            {
                list = list.Where(e => e.BrandId == data.BrandId).ToList();
            }
            else
            {
                list = list.ToList();
            }


            if (data.SupplierId != 0)
            {
                list = list.Where(e => e.SupplierId == data.SupplierId).ToList();
            }
            else
            {
                list = list.ToList();
            }
            if (data.MasterAssetId != 0)
            {
                list = list.Where(e => e.MasterAssetId == data.MasterAssetId).ToList();
            }
            else
            {
                list = list.ToList();
            }


            if (data.StatusId != 0)
            {
                list = list.Where(e => e.AssetStatusId == data.StatusId).ToList();
            }
            else
            {
                list = list.ToList();
            }
            string setstartday, setstartmonth, setendday, setendmonth = "";
            DateTime? startingFrom = new DateTime();
            DateTime? endingTo = new DateTime();
            if (data.Start == "")
            {
                //data.purchaseDateFrom = DateTime.Parse("01/01/1900");
                //startingFrom = DateTime.Parse("01/01/1900");
            }
            else
            {
                data.purchaseDateFrom = DateTime.Parse(data.Start.ToString());
                var startyear = data.purchaseDateFrom.Value.Year;
                var startmonth = data.purchaseDateFrom.Value.Month;
                var startday = data.purchaseDateFrom.Value.Day;
                if (startday < 10)
                    setstartday = data.purchaseDateFrom.Value.Day.ToString().PadLeft(2, '0');
                else
                    setstartday = data.purchaseDateFrom.Value.Day.ToString();

                if (startmonth < 10)
                    setstartmonth = data.purchaseDateFrom.Value.Month.ToString().PadLeft(2, '0');
                else
                    setstartmonth = data.purchaseDateFrom.Value.Month.ToString();

                var sDate = startyear + "-" + setstartmonth + "-" + setstartday;
                startingFrom = DateTime.Parse(sDate);//.AddDays(1);
            }

            if (data.End == "")
            {
                //data.purchaseDateTo = DateTime.Today.Date;
                //endingTo = DateTime.Today.Date;
            }
            else
            {
                data.purchaseDateTo = DateTime.Parse(data.End.ToString());
                var endyear = data.purchaseDateTo.Value.Year;
                var endmonth = data.purchaseDateTo.Value.Month;
                var endday = data.purchaseDateTo.Value.Day;
                if (endday < 10)
                    setendday = data.purchaseDateTo.Value.Day.ToString().PadLeft(2, '0');
                else
                    setendday = data.purchaseDateTo.Value.Day.ToString();
                if (endmonth < 10)
                    setendmonth = data.purchaseDateTo.Value.Month.ToString().PadLeft(2, '0');
                else
                    setendmonth = data.purchaseDateTo.Value.Month.ToString();
                var eDate = endyear + "-" + setendmonth + "-" + setendday;
                endingTo = DateTime.Parse(eDate);
            }



            if ((startingFrom != null && endingTo != null) && (startingFrom != DateTime.Parse("1/1/0001 12:00:00 AM") && endingTo != DateTime.Parse("1/1/0001 12:00:00 AM")))
            {
                list = list.Where(a => a.PurchaseDate != null).ToList();
                list = list.Where(a => a.PurchaseDate.Value.Date >= startingFrom.Value.Date && a.PurchaseDate.Value.Date <= endingTo.Value.Date).ToList();
            }
            else
            {
                list = list.ToList();
            }
            return list;

        }
        public List<DepartmentGroupVM> GetAssetByDepartment(List<IndexAssetDetailVM.GetData> AssetModel)
        {
            List<DepartmentGroupVM> lstAssetDepartments = new List<DepartmentGroupVM>();
            var lsDepartments = (from depart in _context.Departments
                                 select depart).ToList()
                            .GroupBy(a => a.Id).ToList();

            if (lsDepartments.Count > 0)
            {
                foreach (var item in lsDepartments)
                {
                    DepartmentGroupVM departmentGroupObj = new DepartmentGroupVM();
                    departmentGroupObj.Id = item.FirstOrDefault().Id;
                    departmentGroupObj.Name = item.FirstOrDefault().Name;
                    departmentGroupObj.NameAr = item.FirstOrDefault().NameAr;

                    var x = AssetModel.ToList().Where(e => e.DepartmentId == departmentGroupObj.Id);

                    departmentGroupObj.AssetList = AssetModel.ToList().Where(e => e.DepartmentId == departmentGroupObj.Id)
                    .Select(Ass => new IndexAssetDetailVM.GetData
                    {
                        Id = Ass.Id,
                        AssetName = Ass.AssetName,
                        Barcode = Ass.Barcode,
                        SerialNumber = Ass.SerialNumber,
                        Model = Ass.Model,
                        DepartmentName = Ass.DepartmentName,
                        DepartmentNameAr = Ass.DepartmentNameAr,
                        //  CreatedBy = item.CreatedBy,
                        AssetNameAr = Ass.AssetNameAr,
                        BrandName = Ass.BrandName,
                        BrandNameAr = Ass.BrandNameAr,
                        GovernorateName = Ass.GovernorateName,
                        GovernorateNameAr = Ass.GovernorateNameAr,
                        CityName = Ass.CityName,
                        CityNameAr = Ass.CityNameAr,
                        HospitalId = Ass.HospitalId,
                        HospitalName = Ass.HospitalName,
                        HospitalNameAr = Ass.HospitalNameAr,
                        SupplierName = Ass.SupplierName,
                        SupplierNameAr = Ass.SupplierNameAr,
                        OrgName = Ass.OrgName,
                        OrgNameAr = Ass.OrgNameAr,
                        PurchaseDate = Ass.PurchaseDate
                    }).ToList();
                    if (departmentGroupObj.AssetList.Count > 0)
                    {
                        lstAssetDepartments.Add(departmentGroupObj);
                    }
                }
            }
            return lstAssetDepartments;
        }
        public List<BrandGroupVM> GetAssetByBrands(List<IndexAssetDetailVM.GetData> AssetModel)
        {
            List<BrandGroupVM> lstAssetBrand = new List<BrandGroupVM>();
            var lstBrands = (from gov in _context.Brands
                             select gov).ToList()
                            .GroupBy(a => a.Id).ToList();

            if (lstBrands.Count > 0)
            {
                foreach (var item in lstBrands)
                {
                    BrandGroupVM AssetBrandObj = new BrandGroupVM();
                    AssetBrandObj.Id = item.FirstOrDefault().Id;
                    AssetBrandObj.Name = item.FirstOrDefault().Name;
                    AssetBrandObj.NameAr = item.FirstOrDefault().NameAr;

                    AssetBrandObj.AssetList = AssetModel.ToList().Where(e => e.BrandId == AssetBrandObj.Id)
                    .Select(Ass => new IndexAssetDetailVM.GetData
                    {
                        Id = Ass.Id,
                        AssetName = Ass.AssetName,
                        Barcode = Ass.Barcode,
                        SerialNumber = Ass.SerialNumber,
                        Model = Ass.Model,
                        DepartmentName = Ass.DepartmentName,
                        DepartmentNameAr = Ass.DepartmentNameAr,
                        // CreatedBy = item.CreatedBy,
                        AssetNameAr = Ass.AssetNameAr,
                        BrandName = Ass.BrandName,
                        BrandNameAr = Ass.BrandNameAr,
                        GovernorateName = Ass.GovernorateName,
                        GovernorateNameAr = Ass.GovernorateNameAr,
                        CityName = Ass.CityName,
                        CityNameAr = Ass.CityNameAr,
                        HospitalId = Ass.HospitalId,
                        HospitalName = Ass.HospitalName,
                        HospitalNameAr = Ass.HospitalNameAr,
                        SupplierName = Ass.SupplierName,
                        SupplierNameAr = Ass.SupplierNameAr,
                        OrgName = Ass.OrgName,
                        OrgNameAr = Ass.OrgNameAr,
                        PurchaseDate = Ass.PurchaseDate
                    }).ToList();
                    if (AssetBrandObj.AssetList.Count > 0)
                    {
                        lstAssetBrand.Add(AssetBrandObj);
                    }
                }
            }
            return lstAssetBrand;
        }
        public List<GroupHospitalVM> GetAssetByHospital(List<IndexAssetDetailVM.GetData> AssetModel)
        {
            List<GroupHospitalVM> lstAssetHospital = new List<GroupHospitalVM>();
            var lstHosps = (from hos in _context.Hospitals
                            select hos).ToList()
                            .GroupBy(a => a.Id).ToList();

            if (lstHosps.Count > 0)
            {
                foreach (var item in lstHosps)
                {
                    GroupHospitalVM AssetHospitalObj = new GroupHospitalVM();
                    AssetHospitalObj.Id = item.FirstOrDefault().Id;
                    AssetHospitalObj.Name = item.FirstOrDefault().Name;
                    AssetHospitalObj.NameAr = item.FirstOrDefault().NameAr;

                    AssetHospitalObj.AssetList = AssetModel.ToList().Where(e => e.HospitalId == AssetHospitalObj.Id)
                    .Select(Ass => new IndexAssetDetailVM.GetData
                    {
                        Id = Ass.Id,
                        CreatedBy = Ass.CreatedBy,
                        AssetName = Ass.AssetName,
                        AssetNameAr = Ass.AssetNameAr,
                        BrandName = Ass.BrandName,
                        BrandNameAr = Ass.BrandNameAr,
                        GovernorateName = Ass.GovernorateName,
                        GovernorateNameAr = Ass.GovernorateNameAr,
                        CityName = Ass.CityName,
                        CityNameAr = Ass.CityNameAr,
                        HospitalId = Ass.HospitalId,
                        HospitalName = Ass.HospitalName,
                        HospitalNameAr = Ass.HospitalNameAr,
                        SupplierName = Ass.SupplierName,
                        SupplierNameAr = Ass.SupplierNameAr,
                        OrgName = Ass.OrgName,
                        OrgNameAr = Ass.OrgNameAr,
                        PurchaseDate = Ass.PurchaseDate
                    }).ToList();
                    if (AssetHospitalObj.AssetList.Count > 0)
                    {
                        lstAssetHospital.Add(AssetHospitalObj);
                    }
                }
            }
            return lstAssetHospital;
        }
        public List<GroupGovernorateVM> GetAssetByGovernorate(List<IndexAssetDetailVM.GetData> AssetModel)
        {
            List<GroupGovernorateVM> lstAssetGovernorate = new List<GroupGovernorateVM>();
            var lstGovs = (from gov in _context.Governorates
                           select gov).ToList()
                            .GroupBy(a => a.Id).ToList();

            //if (lstGovs.Count > 0)
            // {
            foreach (var item in lstGovs)
            {
                GroupGovernorateVM AssetGovernorateObj = new GroupGovernorateVM();
                AssetGovernorateObj.Id = item.FirstOrDefault().Id;
                AssetGovernorateObj.Name = item.FirstOrDefault().Name;
                AssetGovernorateObj.NameAr = item.FirstOrDefault().NameAr;
                AssetGovernorateObj.CountAssets = AssetModel.Where(a => a.GovernorateId == item.FirstOrDefault().Id).ToList().Count;

                AssetGovernorateObj.AssetList = AssetModel.ToList().Where(e => e.GovernorateId == AssetGovernorateObj.Id)
                .Select(Ass => new IndexAssetDetailVM.GetData
                {
                    Id = Ass.Id,
                    CreatedBy = Ass.CreatedBy,
                    AssetName = Ass.AssetName,
                    AssetNameAr = Ass.AssetNameAr,
                    BrandName = Ass.BrandName,
                    BrandNameAr = Ass.BrandNameAr,
                    GovernorateName = Ass.GovernorateName,
                    GovernorateNameAr = Ass.GovernorateNameAr,
                    CityName = Ass.CityName,
                    CityNameAr = Ass.CityNameAr,
                    HospitalId = Ass.HospitalId,
                    HospitalName = Ass.HospitalName,
                    HospitalNameAr = Ass.HospitalNameAr,
                    SupplierName = Ass.SupplierName,
                    SupplierNameAr = Ass.SupplierNameAr,
                    OrgName = Ass.OrgName,
                    OrgNameAr = Ass.OrgNameAr,
                    PurchaseDate = Ass.PurchaseDate
                }).ToList();
                if (AssetGovernorateObj.AssetList.Count > 0)
                {
                    lstAssetGovernorate.Add(AssetGovernorateObj);
                }
            }
            //   }
            return lstAssetGovernorate;
        }

        public List<GroupCityVM> GetAssetByCity(List<IndexAssetDetailVM.GetData> AssetModel)
        {
            List<GroupCityVM> lstAssetCity = new List<GroupCityVM>();
            var lstCities = (from city in _context.Cities
                             select city).ToList()
                            .GroupBy(a => a.Id).ToList();

            if (lstCities.Count > 0)
            {
                foreach (var item in lstCities)
                {
                    GroupCityVM AssetCityObj = new GroupCityVM();
                    AssetCityObj.Id = item.FirstOrDefault().Id;
                    AssetCityObj.Name = item.FirstOrDefault().Name;
                    AssetCityObj.NameAr = item.FirstOrDefault().NameAr;
                    AssetCityObj.CountAssets = AssetModel.Where(a => a.CityId == item.FirstOrDefault().Id).ToList().Count;
                    AssetCityObj.AssetList = AssetModel.ToList().Where(e => e.CityId == AssetCityObj.Id)
                    .Select(Ass => new IndexAssetDetailVM.GetData
                    {
                        Id = Ass.Id,
                        CreatedBy = Ass.CreatedBy,
                        AssetName = Ass.AssetName,
                        AssetNameAr = Ass.AssetNameAr,
                        BrandName = Ass.BrandName,
                        BrandNameAr = Ass.BrandNameAr,
                        GovernorateName = Ass.GovernorateName,
                        GovernorateNameAr = Ass.GovernorateNameAr,
                        CityName = Ass.CityName,
                        CityNameAr = Ass.CityNameAr,
                        HospitalId = Ass.HospitalId,
                        HospitalName = Ass.HospitalName,
                        HospitalNameAr = Ass.HospitalNameAr,
                        SupplierName = Ass.SupplierName,
                        SupplierNameAr = Ass.SupplierNameAr,
                        OrgName = Ass.OrgName,
                        OrgNameAr = Ass.OrgNameAr,
                        PurchaseDate = Ass.PurchaseDate
                    }).ToList();
                    if (AssetCityObj.AssetList.Count > 0)
                    {
                        lstAssetCity.Add(AssetCityObj);
                    }
                }
            }
            return lstAssetCity;
        }
        public List<GroupSupplierVM> GetAssetBySupplier(List<IndexAssetDetailVM.GetData> AssetModel)
        {
            List<GroupSupplierVM> lstAssetSupplier = new List<GroupSupplierVM>();
            var lstsuppliers = (from sup in _context.Suppliers
                                select sup).ToList()
                            .GroupBy(a => a.Id).ToList();

            if (lstsuppliers.Count > 0)
            {
                foreach (var item in lstsuppliers)
                {
                    GroupSupplierVM AssetSupplierObj = new GroupSupplierVM();
                    AssetSupplierObj.Id = item.FirstOrDefault().Id;
                    AssetSupplierObj.Name = item.FirstOrDefault().Name;
                    AssetSupplierObj.NameAr = item.FirstOrDefault().NameAr;

                    AssetSupplierObj.AssetList = AssetModel.ToList().Where(e => e.SupplierId == AssetSupplierObj.Id)
                    .Select(Ass => new IndexAssetDetailVM.GetData
                    {
                        Id = Ass.Id,
                        CreatedBy = Ass.CreatedBy,
                        Barcode = Ass.Barcode,
                        SerialNumber = Ass.SerialNumber,
                        Model = Ass.Model,
                        AssetName = Ass.AssetName,
                        AssetNameAr = Ass.AssetNameAr,
                        BrandName = Ass.BrandName,
                        BrandNameAr = Ass.BrandNameAr,
                        SupplierName = Ass.SupplierName,
                        SupplierNameAr = Ass.SupplierNameAr,
                        GovernorateName = Ass.GovernorateName,
                        GovernorateNameAr = Ass.GovernorateNameAr,
                        CityName = Ass.CityName,
                        CityNameAr = Ass.CityNameAr,
                        HospitalId = Ass.HospitalId,
                        HospitalName = Ass.HospitalName,
                        HospitalNameAr = Ass.HospitalNameAr,
                        DepartmentName = Ass.DepartmentName,
                        DepartmentNameAr = Ass.DepartmentNameAr,
                        OrgName = Ass.OrgName,
                        OrgNameAr = Ass.OrgNameAr,
                        PurchaseDate = Ass.PurchaseDate
                    }).ToList();
                    if (AssetSupplierObj.AssetList.Count > 0)
                    {
                        lstAssetSupplier.Add(AssetSupplierObj);
                    }
                }
            }
            return lstAssetSupplier;
        }
        public List<GroupOrganizationVM> GetAssetByOrganization(List<IndexAssetDetailVM.GetData> AssetModel)
        {
            List<GroupOrganizationVM> lstAssetOrganization = new List<GroupOrganizationVM>();
            var lstorganizations = (from org in _context.Organizations
                                    select org).ToList()
                            .GroupBy(a => a.Id).ToList();

            if (lstorganizations.Count > 0)
            {
                foreach (var item in lstorganizations)
                {
                    GroupOrganizationVM AssetOrganizationObj = new GroupOrganizationVM();
                    AssetOrganizationObj.Id = item.FirstOrDefault().Id;
                    AssetOrganizationObj.Name = item.FirstOrDefault().Name;
                    AssetOrganizationObj.NameAr = item.FirstOrDefault().NameAr;

                    AssetOrganizationObj.AssetList = AssetModel.ToList().Where(e => e.OrganizationId == AssetOrganizationObj.Id)
                    .Select(Ass => new IndexAssetDetailVM.GetData
                    {
                        Id = Ass.Id,
                        CreatedBy = Ass.CreatedBy,
                        AssetName = Ass.AssetName,
                        AssetNameAr = Ass.AssetNameAr,
                        BrandName = Ass.BrandName,
                        BrandNameAr = Ass.BrandNameAr,
                        GovernorateName = Ass.GovernorateName,
                        GovernorateNameAr = Ass.GovernorateNameAr,
                        CityName = Ass.CityName,
                        CityNameAr = Ass.CityNameAr,
                        HospitalId = Ass.HospitalId,
                        HospitalName = Ass.HospitalName,
                        HospitalNameAr = Ass.HospitalNameAr,
                        SupplierName = Ass.SupplierName,
                        SupplierNameAr = Ass.SupplierNameAr,
                        OrgName = Ass.OrgName,
                        OrgNameAr = Ass.OrgNameAr,
                        PurchaseDate = Ass.PurchaseDate
                    }).ToList();
                    if (AssetOrganizationObj.AssetList.Count > 0)
                    {
                        lstAssetOrganization.Add(AssetOrganizationObj);
                    }
                }
            }
            return lstAssetOrganization;
        }


        public List<HospitalAssetAge> GetAssetsByAgeGroup(int hospitalId)
        {

            List<HospitalAssetAge> lstHospitalAssets = new List<HospitalAssetAge>();
            if (hospitalId != 0)
            {
                lstHospitalAssets = _context.AssetDetails
                            .Where(a => a.HospitalId == hospitalId && a.InstallationDate != null).ToList()
                            .GroupBy(p => (DateTime.Today.Year - p.InstallationDate.Value.Year) / 5)
                           .OrderBy(p => p.Key)
                            .Select(gr => new HospitalAssetAge
                            {
                                AgeGroup = String.Format("{0}-{1}", gr.Key * 5, (gr.Key + 1) * 5),
                                Count = gr.Count()
                            }).ToList();
            }
            else
            {
                lstHospitalAssets = _context.AssetDetails.Where(a => a.InstallationDate != null).ToList()
                    .GroupBy(p => (DateTime.Today.Year - p.InstallationDate.Value.Year) / 5)
                    .OrderBy(p => p.Key)
                            .Select(gr => new HospitalAssetAge
                            {
                                AgeGroup = String.Format("{0}-{1}", gr.Key * 5, (gr.Key + 1) * 5),
                                Count = gr.Count()
                            }).ToList();
            }
            return lstHospitalAssets;
        }

        public List<HospitalAssetAge> GetGeneralAssetsByAgeGroup(FilterHospitalAssetAge searchObj)
        {
            List<HospitalAssetAge> lstHospitalAssets = new List<HospitalAssetAge>();
            var lstHostAssets = _context.AssetDetails.Include(a => a.MasterAsset).Include(a => a.Supplier).Include(a => a.MasterAsset.brand)
                        .Include(a => a.Hospital).Include(a => a.Hospital.Governorate).Include(a => a.Hospital.City)
                        .Include(a => a.Hospital.Organization).Include(a => a.Hospital.SubOrganization).ToList()
                        .Select(item => new IndexAssetDetailVM.GetData
                        {

                            Id = item.Id,
                            Code = item.Code,
                            Model = item.MasterAsset.ModelNumber,
                            BrandName = item.MasterAsset.brand.Name,
                            BrandNameAr = item.MasterAsset.brand.NameAr,
                            SerialNumber = item.SerialNumber,
                            HospitalId = item.HospitalId,
                            SupplierId = item.SupplierId,
                            MasterAssetId = item.MasterAsset.Id,
                            AssetName = item.MasterAsset.Name,
                            BrandId = item.MasterAsset.BrandId,
                            OriginId = item.MasterAsset.OriginId,
                            GovernorateId = item.Hospital.GovernorateId,
                            CityId = item.Hospital.CityId,
                            OrganizationId = item.Hospital.OrganizationId,
                            SubOrganizationId = item.Hospital.SubOrganizationId,
                            InstallationDate = item.InstallationDate
                        }).ToList();


            if (searchObj.GovernorateId != 0)
            {
                lstHostAssets = lstHostAssets.Where(a => a.GovernorateId == searchObj.GovernorateId).ToList();
            }
            else
                lstHostAssets = lstHostAssets.ToList();


            if (searchObj.CityId != 0)
            {
                lstHostAssets = lstHostAssets.Where(a => a.CityId == searchObj.CityId).ToList();
            }
            else
                lstHostAssets = lstHostAssets.ToList();

            if (searchObj.OrganizationId != 0)
            {
                lstHostAssets = lstHostAssets.Where(a => a.OrganizationId == searchObj.OrganizationId).ToList();
            }
            else
                lstHostAssets = lstHostAssets.ToList();

            if (searchObj.SubOrganizationId != 0)
            {
                lstHostAssets = lstHostAssets.Where(a => a.SubOrganizationId == searchObj.SubOrganizationId).ToList();
            }
            else
                lstHostAssets = lstHostAssets.ToList();


            if (searchObj.SupplierId != 0)
            {
                lstHostAssets = lstHostAssets.Where(a => a.SupplierId == searchObj.SupplierId).ToList();
            }
            else
                lstHostAssets = lstHostAssets.ToList();


            if (searchObj.OriginId != 0)
            {
                lstHostAssets = lstHostAssets.Where(a => a.OriginId == searchObj.OriginId).ToList();
            }
            else
                lstHostAssets = lstHostAssets.ToList();



            if (searchObj.BrandId != 0)
            {
                lstHostAssets = lstHostAssets.Where(a => a.BrandId == searchObj.BrandId).ToList();
            }
            else
                lstHostAssets = lstHostAssets.ToList();


            if (searchObj.HospitalId != 0)
            {
                lstHostAssets = lstHostAssets.Where(a => a.HospitalId == searchObj.HospitalId).ToList();
            }
            else
                lstHostAssets = lstHostAssets.ToList();

            if (searchObj.Model != "")
            {
                lstHostAssets = lstHostAssets.Where(b => b.Model == searchObj.Model).ToList();
            }


            lstHospitalAssets = lstHostAssets.Where(a => a.InstallationDate != null).GroupBy(p => (DateTime.Today.Year - p.InstallationDate.Value.Year) / 5)
                                          .OrderBy(p => p.Key).Select(gr => new HospitalAssetAge
                                          {
                                              AgeGroup = String.Format("{0}-{1}", gr.Key * 5, (gr.Key + 1) * 5),
                                              Count = gr.Count()
                                          }).ToList();


            return lstHospitalAssets;
        }

        public IEnumerable<IndexAssetDetailVM.GetData> GetAllAssetsByStatusId(int statusId, string userId)
        {
            ApplicationUser UserObj = new ApplicationUser();
            List<string> userRoleNames = new List<string>();


            var obj = _context.ApplicationUser.Where(a => a.Id == userId).ToList();
            if (obj.Count > 0)
            {
                UserObj = obj[0];
                var roleNames = (from userRole in _context.UserRoles
                                 join role in _context.Roles on userRole.RoleId equals role.Id
                                 where userRole.UserId == userId
                                 select role);
                foreach (var name in roleNames)
                {
                    userRoleNames.Add(name.Name);
                }
            }
            var lstAssets = _context.AssetDetails
                             .Include(t => t.Hospital)
                             .Include(t => t.Hospital.Governorate)
                             .Include(t => t.Hospital.City)
                             .Include(t => t.Hospital.Organization)
                             .Include(t => t.Hospital.SubOrganization)
                             .Include(t => t.Supplier)
                             .Include(t => t.MasterAsset)
                             .Include(t => t.MasterAsset.brand).Include(t => t.Department).OrderBy(a => a.Barcode).ToList();



            List<IndexAssetDetailVM.GetData> list = new List<IndexAssetDetailVM.GetData>();
            if (lstAssets.Count > 0)
            {
                foreach (var asset in lstAssets)
                {
                    IndexAssetDetailVM.GetData detail = new IndexAssetDetailVM.GetData();
                    detail.Id = asset.Id;
                    var lstStatus = _context.AssetStatusTransactions.Where(a => a.AssetDetailId == asset.Id).OrderByDescending(a => a.StatusDate).ToList();
                    if (lstStatus.Count > 0)
                    {
                        detail.AssetStatusId = lstStatus.FirstOrDefault().AssetStatusId;
                    }
                    detail.Id = asset.Id;
                    detail.Code = asset.Code;
                    detail.CreatedBy = asset.CreatedBy;
                    detail.UserId = UserObj.Id;
                    detail.Price = asset.Price;
                    detail.BarCode = asset.Barcode;
                    detail.MasterImg = asset.MasterAsset.AssetImg;
                    detail.Serial = asset.SerialNumber;
                    detail.BrandName = asset.MasterAsset.brand != null ? asset.MasterAsset.brand.Name : "";
                    detail.BrandNameAr = asset.MasterAsset.brand != null ? asset.MasterAsset.brand.NameAr : "";
                    detail.DepartmentName = asset.Department != null ? asset.Department.Name : "";
                    detail.DepartmentNameAr = asset.Department != null ? asset.Department.NameAr : "";
                    detail.Model = asset.MasterAsset.ModelNumber;
                    detail.SerialNumber = asset.SerialNumber;
                    detail.MasterAssetId = asset.MasterAssetId;

                    detail.PurchaseDate = asset.PurchaseDate;
                    detail.InstallationDate = asset.InstallationDate;
                    detail.OperationDate = asset.OperationDate;



                    detail.StrPurchaseDate = (asset.PurchaseDate != null && asset.PurchaseDate.Value != DateTime.Parse("1/1/1900")) ? asset.PurchaseDate.Value.ToShortDateString() : "";
                    detail.StrInstallationDate = (asset.InstallationDate != null && asset.InstallationDate.Value != DateTime.Parse("1/1/1900")) ? asset.InstallationDate.Value.ToShortDateString() : "";
                    detail.StrOperationDate = (asset.OperationDate != null && asset.PurchaseDate != DateTime.Parse("1/1/1900")) ? asset.OperationDate.Value.ToShortDateString() : "";



                    detail.HospitalId = asset.Hospital.Id;
                    detail.HospitalName = asset.Hospital.Name;
                    detail.HospitalNameAr = asset.Hospital.NameAr;
                    detail.AssetName = asset.MasterAsset.Name;
                    detail.AssetNameAr = asset.MasterAsset.NameAr;
                    detail.GovernorateId = asset.Hospital.GovernorateId;
                    detail.GovernorateName = asset.Hospital.Governorate.Name;
                    detail.GovernorateNameAr = asset.Hospital.Governorate.NameAr;
                    detail.CityId = asset.Hospital.CityId;
                    detail.CityName = asset.Hospital.City.Name;
                    detail.CityNameAr = asset.Hospital.City.NameAr;
                    detail.OrganizationId = asset.Hospital.OrganizationId;
                    detail.OrgName = asset.Hospital.Organization.Name;
                    detail.OrgNameAr = asset.Hospital.Organization.NameAr;
                    detail.SubOrganizationId = asset.Hospital.SubOrganizationId;
                    detail.SubOrgName = asset.Hospital.SubOrganization.Name;
                    detail.SubOrgNameAr = asset.Hospital.SubOrganization.NameAr;
                    detail.SupplierName = asset.Supplier != null ? asset.Supplier.Name : "";
                    detail.SupplierNameAr = asset.Supplier != null ? asset.Supplier.NameAr : "";
                    detail.DepartmentName = asset.Department != null ? asset.Department.Name : "";
                    detail.DepartmentNameAr = asset.Department != null ? asset.Department.NameAr : "";
                    detail.QrFilePath = asset.QrFilePath;




                    var IsInContract = _context.ContractDetails.Include(a => a.MasterContract).Include(a => a.AssetDetail)
                        .Where(a => a.AssetDetailId == asset.Id).ToList();
                    if (IsInContract.Count > 0)
                    {

                        var contractObj = IsInContract[0];
                        if (contractObj.MasterContract.To < DateTime.Today.Date)
                        {
                            detail.InContract = "End Contract";
                            detail.InContractAr = "انتهى العقد";
                            detail.ContractFrom = contractObj.MasterContract.From.Value.ToShortDateString();
                            detail.ContractTo = contractObj.MasterContract.To.Value.ToShortDateString();
                        }
                        if (contractObj.MasterContract.To > DateTime.Today.Date)
                        {
                            detail.InContract = "In Contract";
                            detail.InContractAr = "في العقد";
                            detail.ContractFrom = contractObj.MasterContract.From.Value.ToShortDateString();
                            detail.ContractTo = contractObj.MasterContract.To.Value.ToShortDateString();
                        }
                        if (contractObj.MasterContract.To == null || contractObj.MasterContract.To.Value.ToString() == "" || contractObj.MasterContract.To == DateTime.Parse("1/1/1900"))
                        {
                            detail.InContract = "Not In Contract";
                            detail.InContractAr = " ليس في العقد";
                            detail.ContractFrom = "";
                            detail.ContractTo = "";
                        }
                    }
                    list.Add(detail);
                }
            }
            if (UserObj.GovernorateId == 0 && UserObj.CityId == 0 && UserObj.HospitalId == 0 && UserObj.OrganizationId == 0 && UserObj.SubOrganizationId == 0)
            {
                list = list.ToList();
            }
            else if (UserObj.GovernorateId > 0 && UserObj.CityId == 0 && UserObj.HospitalId == 0)
            {
                list = list.Where(t => t.GovernorateId == UserObj.GovernorateId).ToList();
            }
            else if (UserObj.GovernorateId > 0 && UserObj.CityId > 0 && UserObj.HospitalId == 0)
            {
                list = list.Where(t => t.CityId == UserObj.CityId && t.AssetStatusId == statusId).ToList();
            }
            else if (UserObj.OrganizationId > 0 && UserObj.SubOrganizationId == 0 && UserObj.HospitalId == 0)
            {
                list = list.Where(t => t.OrganizationId == UserObj.OrganizationId).ToList();
            }
            else if (UserObj.OrganizationId > 0 && UserObj.SubOrganizationId > 0 && UserObj.HospitalId == 0)
            {
                list = list.Where(t => t.SubOrganizationId == UserObj.SubOrganizationId).ToList();
            }

            else if (UserObj.OrganizationId > 0 && UserObj.SubOrganizationId > 0 && UserObj.HospitalId > 0)
            {
                list = list.Where(t => t.HospitalId == UserObj.HospitalId).ToList();
            }
            else if (UserObj.GovernorateId > 0 && UserObj.CityId > 0 && UserObj.HospitalId > 0)
            {
                list = list.Where(t => t.HospitalId == UserObj.HospitalId).ToList();
            }


            if (statusId != 0)
            {
                list = list.Where(a => a.AssetStatusId == statusId).ToList();
            }
            else
            {
                list = list.ToList();
            }

            return list;
        }

        public IndexAssetDetailVM GetAllAssetsByStatusId(int pageNumber, int pageSize, int statusId, string userId)
        {
            IndexAssetDetailVM mainClass = new IndexAssetDetailVM();
            List<IndexAssetDetailVM.GetData> list = new List<IndexAssetDetailVM.GetData>();

            if (userId != null)
            {
                Employee empObj = new Employee();
                List<string> userRoleNames = new List<string>();
                var obj = _context.ApplicationUser.Where(a => a.Id == userId).ToList();
                var userObj = obj[0];


                var roles = (from userRole in _context.UserRoles
                             join role in _context.ApplicationRole on userRole.RoleId equals role.Id
                             where userRole.UserId == userObj.Id
                             select role);
                foreach (var role in roles)
                {
                    userRoleNames.Add(role.Name);
                }

                var lstEmployees = _context.Employees.Where(a => a.Email == userObj.Email).ToList();
                if (lstEmployees.Count > 0)
                {
                    empObj = lstEmployees[0];
                }

                if ((userRoleNames.Contains("AssetOwner") || userRoleNames.Contains("SRCreator")) && (!userRoleNames.Contains("TLHospitalManager") && !userRoleNames.Contains("EngDepManager") && !userRoleNames.Contains("SRReviewer")))
                {
                    //var lstAllAssets = _context.AssetOwners.Include(a => a.AssetDetail).Include(a => a.Employee).Include(a => a.AssetDetail.MasterAsset).Include(a => a.AssetDetail.Hospital)
                    //                                        .Include(a => a.AssetDetail.Supplier).Include(a => a.AssetDetail.MasterAsset.brand)
                    //                                        .Include(a => a.AssetDetail.Hospital.Governorate).Include(a => a.AssetDetail.Hospital.City).Include(a => a.AssetDetail.Hospital.Organization).Include(a => a.AssetDetail.Hospital.SubOrganization)
                    //                                        .OrderBy(a => a.AssetDetail.Barcode).ToList();

                    var lstAssetOwners = _context.AssetOwners.Where(a => a.EmployeeId == empObj.Id && a.HospitalId == userObj.HospitalId).ToList();
                    if (lstAssetOwners.Count > 0)
                    {
                        foreach (var asset in lstAssetOwners)
                        {
                            IndexAssetDetailVM.GetData detail = new IndexAssetDetailVM.GetData();

                            var lstStatus = _context.AssetStatusTransactions.Where(a => a.AssetDetailId == asset.AssetDetailId).OrderByDescending(a => a.StatusDate).ToList();
                            if (lstStatus.Count > 0)
                            {
                                detail.AssetStatusId = lstStatus.FirstOrDefault().AssetStatusId;
                            }
                            var listAssets = _context.AssetDetails.Include(a => a.Department).Include(a => a.MasterAsset).Include(a => a.Hospital)
                                             .Include(a => a.Supplier).Include(a => a.MasterAsset.brand)
                                             .Include(a => a.Hospital.Governorate).Include(a => a.Hospital.City).Include(a => a.Hospital.Organization)
                                             .Include(a => a.Hospital.SubOrganization)
                                             .OrderBy(a => a.Barcode).Where(a => a.Id == asset.AssetDetailId).ToList();
                            if (listAssets.Count > 0)
                            {
                                var assetObj = listAssets[0];
                                detail.Id = (int)assetObj.Id;
                                detail.Code = assetObj.Code;
                                detail.UserId = userObj.Id;
                                detail.EmployeeId = asset.EmployeeId;
                                detail.Price = assetObj.Price;
                                detail.CreatedBy = assetObj.CreatedBy;
                                detail.BarCode = assetObj.Barcode;
                                detail.MasterImg = assetObj.MasterAsset.AssetImg;
                                detail.Serial = assetObj.SerialNumber;
                                detail.BrandName = assetObj.MasterAsset.brand != null ? assetObj.MasterAsset.brand.Name : "";
                                detail.BrandNameAr = assetObj.MasterAsset.brand != null ? assetObj.MasterAsset.brand.NameAr : "";
                                detail.DepartmentName = assetObj.Department != null ? assetObj.Department.Name : "";
                                detail.DepartmentNameAr = assetObj.Department != null ? assetObj.Department.NameAr : "";
                                detail.Model = assetObj.MasterAsset.ModelNumber;
                                detail.SerialNumber = assetObj.SerialNumber;
                                detail.MasterAssetId = assetObj.MasterAssetId;
                                detail.PurchaseDate = assetObj.PurchaseDate;
                                detail.HospitalId = assetObj.Hospital.Id;
                                detail.HospitalName = assetObj.Hospital.Name;
                                detail.HospitalNameAr = assetObj.Hospital.NameAr;
                                detail.AssetName = assetObj.MasterAsset.Name;
                                detail.AssetNameAr = assetObj.MasterAsset.NameAr;
                                detail.GovernorateId = assetObj.Hospital.GovernorateId;
                                detail.GovernorateName = assetObj.Hospital.Governorate.Name;
                                detail.GovernorateNameAr = assetObj.Hospital.Governorate.NameAr;
                                detail.CityId = assetObj.Hospital.CityId;
                                detail.CityName = assetObj.Hospital.City.Name;
                                detail.CityNameAr = assetObj.Hospital.City.NameAr;
                                detail.OrganizationId = assetObj.Hospital.OrganizationId;
                                detail.OrgName = assetObj.Hospital.Organization.Name;
                                detail.OrgNameAr = assetObj.Hospital.Organization.NameAr;
                                detail.SubOrganizationId = assetObj.Hospital.SubOrganizationId;
                                detail.SubOrgName = assetObj.Hospital.SubOrganization.Name;
                                detail.SubOrgNameAr = assetObj.Hospital.SubOrganization.NameAr;
                                detail.SupplierName = assetObj.Supplier != null ? assetObj.Supplier.Name : "";
                                detail.SupplierNameAr = assetObj.Supplier != null ? assetObj.Supplier.NameAr : "";
                                detail.QrFilePath = assetObj.QrFilePath;
                                detail.DepartmentName = assetObj.Department != null ? assetObj.Department.Name : "";
                                detail.DepartmentNameAr = assetObj.Department != null ? assetObj.Department.NameAr : "";
                            }
                            list.Add(detail);

                        }
                    }
                }
                else
                {


                    var lstAllAssets = _context.AssetDetails.Include(a => a.MasterAsset).Include(a => a.Hospital).Include(a => a.Department)
                                                       .Include(a => a.Supplier).Include(a => a.MasterAsset.brand)
                                                       .Include(a => a.Hospital.Governorate).Include(a => a.Hospital.City).Include(a => a.Hospital.Organization).Include(a => a.Hospital.SubOrganization)
                                                       .OrderBy(a => a.Barcode).ToList();


                    if (lstAllAssets.Count > 0)
                    {
                        foreach (var asset in lstAllAssets)
                        {
                            IndexAssetDetailVM.GetData detail = new IndexAssetDetailVM.GetData();

                            var lstStatus = _context.AssetStatusTransactions.Where(a => a.AssetDetailId == asset.Id).OrderByDescending(a => a.StatusDate).ToList();
                            if (lstStatus.Count > 0)
                            {
                                detail.AssetStatusId = lstStatus.FirstOrDefault().AssetStatusId;
                            }
                            detail.Id = (int)asset.Id;
                            detail.Code = asset.Code;
                            detail.UserId = userObj.Id;
                            var lstOwners = _context.AssetOwners.Where(a => a.AssetDetailId == detail.Id).ToList();
                            if (lstOwners.Count > 0)
                            {
                                var ownerObj = lstOwners[0];
                                detail.EmployeeId = ownerObj.EmployeeId;
                            }
                            detail.Price = asset.Price;
                            detail.BarCode = asset.Barcode;
                            detail.MasterImg = asset.MasterAsset.AssetImg;
                            detail.Serial = asset.SerialNumber;
                            detail.BrandName = asset.MasterAsset.brand != null ? asset.MasterAsset.brand.Name : "";
                            detail.BrandNameAr = asset.MasterAsset.brand != null ? asset.MasterAsset.brand.NameAr : "";
                            detail.Model = asset.MasterAsset.ModelNumber;
                            detail.SerialNumber = asset.SerialNumber;
                            detail.MasterAssetId = asset.MasterAssetId;
                            detail.PurchaseDate = asset.PurchaseDate;
                            detail.HospitalId = asset.Hospital.Id;
                            detail.HospitalName = asset.Hospital.Name;
                            detail.HospitalNameAr = asset.Hospital.NameAr;
                            detail.AssetName = asset.MasterAsset.Name;
                            detail.AssetNameAr = asset.MasterAsset.NameAr;
                            detail.GovernorateId = asset.Hospital.GovernorateId;
                            detail.GovernorateName = asset.Hospital.Governorate.Name;
                            detail.GovernorateNameAr = asset.Hospital.Governorate.NameAr;
                            detail.CityId = asset.Hospital.CityId;
                            detail.CityName = asset.Hospital.City.Name;
                            detail.CityNameAr = asset.Hospital.City.NameAr;
                            detail.OrganizationId = asset.Hospital.OrganizationId;
                            detail.OrgName = asset.Hospital.Organization.Name;
                            detail.OrgNameAr = asset.Hospital.Organization.NameAr;
                            detail.SubOrganizationId = asset.Hospital.SubOrganizationId;
                            detail.SubOrgName = asset.Hospital.SubOrganization.Name;
                            detail.SubOrgNameAr = asset.Hospital.SubOrganization.NameAr;
                            detail.SupplierName = asset.Supplier != null ? asset.Supplier.Name : "";
                            detail.SupplierNameAr = asset.Supplier != null ? asset.Supplier.NameAr : "";
                            detail.QrFilePath = asset.QrFilePath;
                            detail.DepartmentName = asset.Department != null ? asset.Department.Name : "";
                            detail.DepartmentNameAr = asset.Department != null ? asset.Department.NameAr : "";
                            list.Add(detail);
                        }
                    }

                }

                if (userObj.HospitalId > 0)
                {
                    if (userRoleNames.Contains("Admin"))
                    {
                        list = list.ToList();
                    }
                    else if ((userRoleNames.Contains("AssetOwner") || userRoleNames.Contains("SRCreator")) && !userRoleNames.Contains("TLHospitalManager") && !userRoleNames.Contains("EngDepManager") && !userRoleNames.Contains("SRReviewer"))
                    {
                        list = list.Where(a => a.EmployeeId == empObj.Id && a.HospitalId == userObj.HospitalId).ToList();
                    }
                    else if ((userRoleNames.Contains("AssetOwner") && userRoleNames.Contains("SRCreator")) && !userRoleNames.Contains("TLHospitalManager") && !userRoleNames.Contains("EngDepManager") && !userRoleNames.Contains("SRReviewer"))
                    {
                        list = list.Where(a => a.EmployeeId == empObj.Id && a.HospitalId == userObj.HospitalId).ToList();
                    }
                    else if (userRoleNames.Contains("TLHospitalManager") && userRoleNames.Contains("EngDepManager") && userRoleNames.Contains("SRReviewer") && userRoleNames.Contains("SRCreator") && userRoleNames.Contains("AssetOwner"))
                    {
                        list = list.Where(t => t.HospitalId == userObj.HospitalId).ToList();
                    }
                    else if (userRoleNames.Contains("TLHospitalManager") && !userRoleNames.Contains("EngDepManager") && !userRoleNames.Contains("SRReviewer") && !userRoleNames.Contains("SRCreator") && !userRoleNames.Contains("AssetOwner"))
                    {
                        list = list.Where(t => t.HospitalId == userObj.HospitalId).ToList();
                    }
                    else if (userRoleNames.Contains("TLHospitalManager") && userRoleNames.Contains("EngDepManager") && !userRoleNames.Contains("SRReviewer") && !userRoleNames.Contains("SRCreator") && !userRoleNames.Contains("AssetOwner"))
                    {
                        list = list.Where(t => t.HospitalId == userObj.HospitalId).ToList();
                    }
                    else if (userRoleNames.Contains("TLHospitalManager") && userRoleNames.Contains("EngDepManager") && userRoleNames.Contains("SRReviewer") && !userRoleNames.Contains("SRCreator") && !userRoleNames.Contains("AssetOwner"))
                    {
                        list = list.Where(t => t.HospitalId == userObj.HospitalId).ToList();
                    }
                    else if (userRoleNames.Contains("TLHospitalManager") && userRoleNames.Contains("EngDepManager") && userRoleNames.Contains("SRReviewer") && userRoleNames.Contains("SRCreator") && !userRoleNames.Contains("AssetOwner"))
                    {
                        list = list.Where(t => t.HospitalId == userObj.HospitalId).ToList();
                    }
                    else if (userRoleNames.Contains("TLHospitalManager") && userRoleNames.Contains("EngDepManager") && userRoleNames.Contains("SRReviewer") && userRoleNames.Contains("SRCreator") && userRoleNames.Contains("AssetOwner"))
                    {
                        list = list.Where(t => t.HospitalId == userObj.HospitalId).ToList();
                    }
                }





                if (userObj.GovernorateId == 0 && userObj.CityId == 0 && userObj.OrganizationId == 0 && userObj.SubOrganizationId == 0 && userObj.HospitalId == 0)
                {
                    list = list.ToList();
                }
                if (userObj.GovernorateId > 0 && userObj.CityId == 0 && userObj.OrganizationId == 0 && userObj.SubOrganizationId == 0 && userObj.HospitalId == 0)
                {
                    list = list.Where(a => a.GovernorateId == userObj.GovernorateId).ToList();
                }
                if (userObj.GovernorateId > 0 && userObj.CityId > 0 && userObj.OrganizationId == 0 && userObj.SubOrganizationId == 0 && userObj.HospitalId == 0)
                {
                    list = list.Where(a => a.GovernorateId == userObj.GovernorateId && a.CityId == userObj.CityId).ToList();
                }
                if (userObj.GovernorateId == 0 && userObj.CityId == 0 && userObj.OrganizationId > 0 && userObj.SubOrganizationId == 0 && userObj.HospitalId == 0)
                {
                    list = list.Where(a => a.OrganizationId == userObj.OrganizationId).ToList();
                }
                if (userObj.GovernorateId == 0 && userObj.CityId == 0 && userObj.OrganizationId > 0 && userObj.SubOrganizationId > 0 && userObj.HospitalId == 0)
                {
                    list = list.Where(a => a.OrganizationId == userObj.OrganizationId && a.SubOrganizationId == userObj.SubOrganizationId).ToList();
                }



                if (statusId != 0)
                {
                    list = list.Where(a => a.AssetStatusId == statusId).ToList();
                }
                else
                {
                    list = list.ToList();
                }

                var requestsPerPage = list.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                mainClass.Results = requestsPerPage;
                mainClass.Count = list.Count();
                return mainClass;
            }
            return null;

        }

        public IEnumerable<ViewAssetDetailVM> GetListOfAssetDetailsByHospitalNotInContract(int hospitalId)
        {

            List<ViewAssetDetailVM> lstAssetDetails = new List<ViewAssetDetailVM>();
            lstAssetDetails = _context.AssetDetails.Include(a => a.MasterAsset).Include(a => a.Supplier)
                               .Where(a => a.HospitalId == hospitalId)
                               .Select(item => new ViewAssetDetailVM
                               {

                                   Id = item.Id,
                                   AssetName = item.MasterAsset.Name,
                                   AssetNameAr = item.MasterAsset.NameAr,
                                   SerialNumber = item.SerialNumber,
                                   SupplierName = item.Supplier.Name,
                                   SupplierNameAr = item.Supplier.NameAr,
                                   HospitalId = int.Parse(item.HospitalId.ToString()),
                                   HospitalName = item.Hospital.Name,
                                   HospitalNameAr = item.Hospital.NameAr,
                                   Barcode = item.Barcode

                               }).ToList();

            var contractAssetDetailIds = _context.ContractDetails.Include(a => a.AssetDetail).Include(a => a.AssetDetail.Hospital).Where(a => a.AssetDetail.HospitalId == hospitalId).Select(a => a.AssetDetailId).ToList();
            var assetDetailIds = _context.AssetDetails.ToList().Where(a => a.HospitalId == hospitalId).Select(a => a.Id).ToList();
            List<int> lstContractAssetDetailIds = new List<int>();
            if (contractAssetDetailIds.Count > 0)
            {
                foreach (var item in contractAssetDetailIds)
                {
                    lstContractAssetDetailIds.Add(int.Parse(item.ToString()));
                }

                var remainIds = assetDetailIds.Except(lstContractAssetDetailIds);


                lstAssetDetails = lstAssetDetails.Where(a => remainIds.Contains(a.Id)).ToList();
            }
            else
            {
                var remainIds = assetDetailIds;


                lstAssetDetails = lstAssetDetails.Where(a => remainIds.Contains(a.Id)).ToList();
            }



            return lstAssetDetails;
        }

        public IEnumerable<ViewAssetDetailVM> GetNoneExcludedAssetsByHospitalId(int hospitalId)
        {
            List<int> idList = new List<int>();
            List<int> idExcludedList = new List<int>();
            List<ViewAssetDetailVM> viewAssetDetailList = new List<ViewAssetDetailVM>();

            var lstAssetDetailIds = _context.AssetDetails.Include(a => a.MasterAsset).Include(a => a.Supplier)
                             .Where(a => a.HospitalId == hospitalId).ToList();
            foreach (var item in lstAssetDetailIds)
            {
                idList.Add(item.Id);
            }
            var excludedIds = _context.HospitalApplications.Include(a => a.AssetDetail).Where(a => a.AssetDetail.HospitalId == hospitalId).ToList();
            foreach (var itm in excludedIds)
            {
                idExcludedList.Add(int.Parse(itm.AssetId.ToString()));
            }
            var lstRemainIds = idList.Except(idExcludedList);

            foreach (var asset in lstRemainIds)
            {
                var lstAssetTransactions = _context.AssetStatusTransactions.Where(a => a.AssetDetailId == asset).ToList();
                if (lstAssetTransactions.Count > 0)
                {
                    lstAssetTransactions = lstAssetTransactions.OrderByDescending(a => a.StatusDate.Value.ToString()).ToList();

                    var transObj = lstAssetTransactions.FirstOrDefault();
                    if (transObj.AssetStatusId == 3)
                    {
                        viewAssetDetailList.Add(_context.AssetDetails.Include(a => a.MasterAsset).Include(a => a.Supplier)
                                                  .Where(a => a.HospitalId == hospitalId && a.Id == asset)
                                                  .Select(item => new ViewAssetDetailVM
                                                  {
                                                      Id = item.Id,
                                                      AssetName = item.MasterAsset.Name,
                                                      AssetNameAr = item.MasterAsset.NameAr,
                                                      SerialNumber = item.SerialNumber,
                                                      SupplierName = item.Supplier.Name,
                                                      SupplierNameAr = item.Supplier.NameAr,
                                                      HospitalId = item.Hospital.Id,
                                                      HospitalName = item.Hospital.Name,
                                                      HospitalNameAr = item.Hospital.NameAr,
                                                      Barcode = item.Barcode,
                                                  }).FirstOrDefault());
                    }
                }
            }


            return viewAssetDetailList;
        }

        public IEnumerable<ViewAssetDetailVM> GetSupplierNoneExcludedAssetsByHospitalId(int hospitalId)
        {
            List<int> idList = new List<int>();
            List<int> idExcludedList = new List<int>();
            List<ViewAssetDetailVM> viewAssetDetailList = new List<ViewAssetDetailVM>();

            var lstAssetDetailIds = _context.AssetDetails.Include(a => a.MasterAsset).Include(a => a.Supplier)
                             .Where(a => a.HospitalId == hospitalId).ToList();
            foreach (var item in lstAssetDetailIds)
            {
                idList.Add(item.Id);
            }
            var excludedIds = _context.SupplierExecludeAssets.Include(a => a.AssetDetail).Where(a => a.AssetDetail.HospitalId == hospitalId).ToList();
            foreach (var itm in excludedIds)
            {
                idExcludedList.Add(int.Parse(itm.AssetId.ToString()));
            }
            var lstRemainIds = idList.Except(idExcludedList);

            foreach (var asset in lstRemainIds)
            {

                var lstAssetTransactions = _context.AssetStatusTransactions.Where(a => a.AssetDetailId == asset).OrderByDescending(a => a.StatusDate.Value.ToString()).ToList();
                if (lstAssetTransactions.Count > 0)
                {

                    var transObj = lstAssetTransactions.FirstOrDefault();
                    if (transObj.AssetStatusId == 3)
                    {

                        var assetItem = _context.AssetDetails.Include(a => a.MasterAsset).Include(a => a.Supplier).Include(a => a.Hospital)
                                                  .Where(a => a.HospitalId == hospitalId && a.Id == asset).ToList();

                        foreach (var item in assetItem)
                        {
                            ViewAssetDetailVM assetBarCode = new ViewAssetDetailVM();

                            assetBarCode.Id = item.Id;
                            assetBarCode.AssetName = item.MasterAsset.Name;
                            assetBarCode.AssetNameAr = item.MasterAsset.NameAr;
                            assetBarCode.SerialNumber = item.SerialNumber;
                            assetBarCode.SupplierName = item.Supplier != null ? item.Supplier.Name : "";
                            assetBarCode.SupplierNameAr = item.Supplier != null ? item.Supplier.NameAr : "";
                            assetBarCode.HospitalId = item.Hospital.Id;
                            assetBarCode.HospitalName = item.Hospital.Name;
                            assetBarCode.HospitalNameAr = item.Hospital.NameAr;
                            assetBarCode.Barcode = item.Barcode;
                            assetBarCode.BarCode = item.Barcode;
                            assetBarCode.MasterAssetId = item.MasterAsset.Id;
                            assetBarCode.MasterAssetName = item.MasterAsset.Name;
                            assetBarCode.MasterAssetNameAr = item.MasterAsset.NameAr;
                            viewAssetDetailList.Add(assetBarCode);
                        }
                    }
                }
            }
            return viewAssetDetailList;
        }



        public IEnumerable<ViewAssetDetailVM> GetAutoCompleteSupplierNoneExcludedAssetsByHospitalId(string barcode, int hospitalId)
        {

            List<int> idList = new List<int>();
            List<int> idExcludedList = new List<int>();
            List<ViewAssetDetailVM> viewAssetDetailList = new List<ViewAssetDetailVM>();

            var lstAssetDetailIds = _context.AssetDetails.Include(a => a.MasterAsset).Include(a => a.Supplier)
                             .Where(a => a.HospitalId == hospitalId).ToList();
            foreach (var item in lstAssetDetailIds)
            {
                idList.Add(item.Id);
            }
            var excludedIds = _context.SupplierExecludeAssets.Include(a => a.AssetDetail).Where(a => a.AssetDetail.HospitalId == hospitalId).ToList();
            foreach (var itm in excludedIds)
            {
                idExcludedList.Add(int.Parse(itm.AssetId.ToString()));
            }
            var lstRemainIds = idList.Except(idExcludedList);

            foreach (var asset in lstRemainIds)
            {
                var lstAssetTransactions = _context.AssetStatusTransactions.Where(a => a.AssetDetailId == asset).OrderByDescending(a => a.StatusDate.Value.ToString()).ToList();
                if (lstAssetTransactions.Count > 0)
                {
                    var transObj = lstAssetTransactions.FirstOrDefault();
                    if (transObj.AssetStatusId == 3)
                    {

                        var assetItem = _context.AssetDetails.Include(a => a.MasterAsset).Include(a => a.Supplier).Include(a => a.Hospital)
                                                   .Where(a => a.Barcode.Contains(barcode) && a.Id == asset).ToList();

                        foreach (var item in assetItem)
                        {
                            ViewAssetDetailVM assetBarCode = new ViewAssetDetailVM();

                            assetBarCode.Id = item.Id;
                            assetBarCode.AssetName = item.MasterAsset.Name;
                            assetBarCode.AssetNameAr = item.MasterAsset.NameAr;
                            assetBarCode.SerialNumber = item.SerialNumber;
                            assetBarCode.SupplierName = item.Supplier != null ? item.Supplier.Name : "";
                            assetBarCode.SupplierNameAr = item.Supplier != null ? item.Supplier.NameAr : "";
                            assetBarCode.HospitalId = item.Hospital.Id;
                            assetBarCode.HospitalName = item.Hospital.Name;
                            assetBarCode.HospitalNameAr = item.Hospital.NameAr;
                            assetBarCode.Barcode = item.Barcode;
                            assetBarCode.BarCode = item.Barcode;
                            assetBarCode.MasterAssetId = item.MasterAsset.Id;
                            assetBarCode.MasterAssetName = item.MasterAsset.Name;
                            assetBarCode.MasterAssetNameAr = item.MasterAsset.NameAr;
                            viewAssetDetailList.Add(assetBarCode);
                        }
                    }
                }
            }
            return viewAssetDetailList;
        }

        public int CountAssetsByHospitalId(int hospitalId)
        {

            if (hospitalId != 0)
                return _context.AssetDetails.Where(a => a.HospitalId == hospitalId).Count();
            else
                return _context.AssetDetails.Count();
        }

        public List<CountAssetVM> ListTopAssetsByHospitalId(int hospitalId)
        {
            List<CountAssetVM> list = new List<CountAssetVM>();
            if (hospitalId == 0)
            {

                var lstAssetDetails = _context.AssetDetails.Include(a => a.MasterAsset).Include(a => a.Hospital).ToList();
                if (lstAssetDetails.Count > 0)
                {
                    var lstGroupMaster = lstAssetDetails.GroupBy(a => a.MasterAssetId).Select(group => new
                    {
                        masterId = group.Key,
                        CountMasterId = group.Count(),
                        Name = group.FirstOrDefault().MasterAsset.Name,
                        NameAr = group.FirstOrDefault().MasterAsset.NameAr,
                        AssetPrice = group.FirstOrDefault().Price != null ? group.Sum(a => Convert.ToDecimal(group.FirstOrDefault().Price.ToString())) : 0
                    }).OrderByDescending(x => x.CountMasterId).ToList().Take(10).ToList();

                    if (lstGroupMaster.Count > 0)
                    {
                        foreach (var item in lstGroupMaster)
                        {
                            CountAssetVM countAssetObj = new CountAssetVM();
                            countAssetObj.AssetName = item.Name;
                            countAssetObj.AssetNameAr = item.NameAr;
                            countAssetObj.AssetPrice = item.AssetPrice;
                            countAssetObj.CountAssetsByHospital = item.CountMasterId;
                            list.Add(countAssetObj);
                        }
                    }
                }

            }
            else
            {

                var lstAssetDetails = _context.AssetDetails.Include(a => a.MasterAsset).Include(a => a.Hospital).Where(a => a.HospitalId == hospitalId).ToList();
                if (lstAssetDetails.Count > 0)
                {
                    var lstGroupMaster = lstAssetDetails.GroupBy(a => a.MasterAssetId).Select(group => new
                    {
                        masterId = group.Key,
                        CountMasterId = group.Count(),
                        Name = group.FirstOrDefault().MasterAsset.Name,
                        NameAr = group.FirstOrDefault().MasterAsset.NameAr,
                        AssetPrice = group.FirstOrDefault().Price != null ? group.Sum(a => Convert.ToDecimal(group.FirstOrDefault().Price.ToString())) : 0
                    }).OrderByDescending(x => x.CountMasterId).ToList().Take(10).ToList();

                    if (lstGroupMaster.Count > 0)
                    {
                        foreach (var item in lstGroupMaster)
                        {
                            CountAssetVM countAssetObj = new CountAssetVM();
                            countAssetObj.AssetName = item.Name;
                            countAssetObj.AssetNameAr = item.NameAr;
                            countAssetObj.AssetPrice = item.AssetPrice;
                            countAssetObj.CountAssetsByHospital = item.CountMasterId;
                            list.Add(countAssetObj);
                        }
                    }
                }
            }
            return list;
        }

        public List<CountAssetVM> ListAssetsByGovernorateIds()
        {
            List<CountAssetVM> list = new List<CountAssetVM>();
            var lstGovernorates = _context.Governorates.ToList();
            if (lstGovernorates.Count > 0)
            {
                foreach (var gov in lstGovernorates)
                {

                    CountAssetVM countAssetObj = new CountAssetVM();
                    countAssetObj.GovernorateName = gov.Name;
                    countAssetObj.GovernorateNameAr = gov.NameAr;

                    var lstAssetDetails = _context.AssetDetails
                                            .Include(a => a.MasterAsset)
                                            .Include(a => a.Hospital)
                                            .Include(a => a.Hospital.Governorate)
                                            .Include(a => a.Hospital.City)
                                            .Where(a => a.Hospital.GovernorateId == gov.Id).ToList();

                    countAssetObj.CountAssetsByGovernorate = lstAssetDetails.Count();
                    list.Add(countAssetObj);
                }
            }

            return list;
        }

        public List<CountAssetVM> ListAssetsByCityIds()
        {
            List<CountAssetVM> list = new List<CountAssetVM>();

            var lstCities = _context.Cities.ToList();
            if (lstCities.Count > 0)
            {
                foreach (var city in lstCities)
                {

                    CountAssetVM countAssetObj = new CountAssetVM();
                    countAssetObj.CityName = city.Name;
                    countAssetObj.CityNameAr = city.NameAr;

                    var lstAssetDetails = _context.AssetDetails
                                            .Include(a => a.MasterAsset)
                                            .Include(a => a.Hospital)
                                            .Include(a => a.Hospital.Governorate)
                                            .Include(a => a.Hospital.City)
                                            .Where(a => a.Hospital.CityId == city.Id).ToList();

                    countAssetObj.CountAssetsByCity = lstAssetDetails.Count();
                    list.Add(countAssetObj);
                }
            }
            return list;
        }

        public List<CountAssetVM> CountAssetsInHospitalByHospitalId(int hospitalId)
        {
            List<CountAssetVM> list = new List<CountAssetVM>();
            var lstAssetDetails = _context.AssetDetails.Include(a => a.MasterAsset).Include(a => a.Hospital).Where(a => a.HospitalId == hospitalId).ToList();
            if (lstAssetDetails.Count > 0)
            {
                var lstGroupMaster = lstAssetDetails.GroupBy(a => a.MasterAssetId).Select(group => new
                {
                    masterId = group.Key,
                    CountMasterId = group.Count(),
                    Name = group.FirstOrDefault().MasterAsset.Name,
                    NameAr = group.FirstOrDefault().MasterAsset.NameAr,
                    AssetPrice = group.FirstOrDefault().Price != null ? group.Sum(a => Convert.ToDecimal(group.FirstOrDefault().Price.ToString())) : 0
                }).OrderByDescending(x => x.CountMasterId).ToList();

                if (lstGroupMaster.Count > 0)
                {
                    foreach (var item in lstGroupMaster)
                    {
                        CountAssetVM countAssetObj = new CountAssetVM();
                        countAssetObj.AssetName = item.Name;
                        countAssetObj.AssetNameAr = item.NameAr;
                        countAssetObj.AssetPrice = item.AssetPrice;
                        countAssetObj.CountAssetsByHospital = item.CountMasterId;
                        list.Add(countAssetObj);
                    }
                }
            }

            return list;
        }

        public IndexAssetDetailVM SearchHospitalAssetsByDepartmentId(int departmentId, string userId, int pageNumber, int pageSize)
        {
            IndexAssetDetailVM mainClass = new IndexAssetDetailVM();
            List<IndexAssetDetailVM.GetData> list = new List<IndexAssetDetailVM.GetData>();
            if (userId != null)
            {
                Employee empObj = new Employee();
                List<string> userRoleNames = new List<string>();
                var obj = _context.ApplicationUser.Where(a => a.Id == userId).ToList();
                var userObj = obj[0];


                var roles = (from userRole in _context.UserRoles
                             join role in _context.ApplicationRole on userRole.RoleId equals role.Id
                             where userRole.UserId == userObj.Id
                             select role);
                foreach (var role in roles)
                {
                    userRoleNames.Add(role.Name);
                }

                var lstEmployees = _context.Employees.Where(a => a.Email == userObj.Email).ToList();
                if (lstEmployees.Count > 0)
                {
                    empObj = lstEmployees[0];
                }

                //var lstAllAssets = _context.AssetOwners.Include(a => a.AssetDetail).Include(a => a.Employee).Include(a => a.AssetDetail.MasterAsset).Include(a => a.AssetDetail.Hospital).Include(a => a.AssetDetail.Department)
                //                                        .Include(a => a.AssetDetail.Supplier).Include(a => a.AssetDetail.MasterAsset.brand)
                //                                        .Include(a => a.AssetDetail.Hospital.Governorate).Include(a => a.AssetDetail.Hospital.City)
                //                                        .Include(a => a.AssetDetail.Hospital.Organization).Include(a => a.AssetDetail.Hospital.SubOrganization)
                //                                                                                                .OrderBy(a => a.AssetDetail.Barcode).ToList();

                var lstAllAssets = _context.AssetDetails.Include(a => a.MasterAsset).Include(a => a.Hospital).Include(a => a.Department)
                                         .Include(a => a.Supplier).Include(a => a.MasterAsset.brand)
                                         .Include(a => a.Hospital.Governorate).Include(a => a.Hospital.City)
                                         .Include(a => a.Hospital.Organization).Include(a => a.Hospital.SubOrganization)
                                                                                                 .OrderBy(a => a.Barcode).ToList();
                if (lstAllAssets.Count > 0)
                {
                    foreach (var asset in lstAllAssets)
                    {
                        IndexAssetDetailVM.GetData detail = new IndexAssetDetailVM.GetData();

                        var lstStatus = _context.AssetStatusTransactions.Where(a => a.AssetDetailId == asset.Id).OrderByDescending(a => a.StatusDate).ToList();
                        if (lstStatus.Count > 0)
                        {
                            detail.AssetStatusId = lstStatus.FirstOrDefault().AssetStatusId;
                        }
                        /////
                        detail.Id = asset.Id;
                        detail.DepartmentId = asset.DepartmentId != null ? asset.DepartmentId : 0;
                        detail.Code = asset.Code;
                        detail.UserId = userObj.Id;
                        //   detail.EmployeeId = asset.EmployeeId;
                        detail.Price = asset.Price;
                        detail.BarCode = asset.Barcode;
                        detail.MasterImg = asset.MasterAsset.AssetImg;
                        detail.Serial = asset.SerialNumber;
                        detail.BrandName = asset.MasterAsset.brand != null ? asset.MasterAsset.brand.Name : "";
                        detail.BrandNameAr = asset.MasterAsset.brand != null ? asset.MasterAsset.brand.NameAr : "";
                        detail.Model = asset.MasterAsset.ModelNumber;
                        detail.SerialNumber = asset.SerialNumber;
                        detail.MasterAssetId = asset.MasterAssetId;
                        detail.PurchaseDate = asset.PurchaseDate;
                        detail.HospitalId = asset.Hospital.Id;
                        detail.HospitalName = asset.Hospital.Name;
                        detail.HospitalNameAr = asset.Hospital.NameAr;
                        detail.AssetName = asset.MasterAsset.Name;
                        detail.AssetNameAr = asset.MasterAsset.NameAr;
                        detail.DepartmentId = asset.Department != null ? asset.DepartmentId : 0;
                        detail.DepartmentName = asset.Department != null ? asset.Department.Name : "";
                        detail.DepartmentNameAr = asset.Department != null ? asset.Department.NameAr : "";
                        detail.GovernorateId = asset.Hospital.GovernorateId;
                        detail.GovernorateName = asset.Hospital.Governorate.Name;
                        detail.GovernorateNameAr = asset.Hospital.Governorate.NameAr;
                        detail.CityId = asset.Hospital.CityId;
                        detail.CityName = asset.Hospital.City.Name;
                        detail.CityNameAr = asset.Hospital.City.NameAr;
                        detail.OrganizationId = asset.Hospital.OrganizationId;
                        detail.OrgName = asset.Hospital.Organization.Name;
                        detail.OrgNameAr = asset.Hospital.Organization.NameAr;
                        detail.SubOrganizationId = asset.Hospital.SubOrganizationId;
                        detail.SubOrgName = asset.Hospital.SubOrganization.Name;
                        detail.SubOrgNameAr = asset.Hospital.SubOrganization.NameAr;
                        detail.SupplierName = asset.Supplier != null ? asset.Supplier.Name : "";
                        detail.SupplierNameAr = asset.Supplier != null ? asset.Supplier.NameAr : "";
                        detail.QrFilePath = asset.QrFilePath;
                        detail.QrData = asset.QrData;
                        list.Add(detail);
                    }
                }

                if (userObj.HospitalId > 0)
                {
                    list = list.Where(a => a.HospitalId == userObj.HospitalId).ToList();
                }
                if (userObj.GovernorateId == 0 && userObj.CityId == 0 && userObj.OrganizationId == 0 && userObj.SubOrganizationId == 0 && userObj.HospitalId == 0)
                {
                    list = list.ToList();
                }
                if (userObj.GovernorateId > 0 && userObj.CityId == 0 && userObj.OrganizationId == 0 && userObj.SubOrganizationId == 0 && userObj.HospitalId == 0)
                {
                    list = list.Where(a => a.GovernorateId == userObj.GovernorateId).ToList();
                }
                if (userObj.GovernorateId > 0 && userObj.CityId > 0 && userObj.OrganizationId == 0 && userObj.SubOrganizationId == 0 && userObj.HospitalId == 0)
                {
                    list = list.Where(a => a.GovernorateId == userObj.GovernorateId && a.CityId == userObj.CityId).ToList();
                }
                if (userObj.GovernorateId == 0 && userObj.CityId == 0 && userObj.OrganizationId > 0 && userObj.SubOrganizationId == 0 && userObj.HospitalId == 0)
                {
                    list = list.Where(a => a.OrganizationId == userObj.OrganizationId).ToList();
                }
                if (userObj.GovernorateId == 0 && userObj.CityId == 0 && userObj.OrganizationId > 0 && userObj.SubOrganizationId > 0 && userObj.HospitalId == 0)
                {
                    list = list.Where(a => a.OrganizationId == userObj.OrganizationId && a.SubOrganizationId == userObj.SubOrganizationId).ToList();
                }
                if (departmentId != 0)
                {
                    list = list.Where(a => a.DepartmentId == departmentId).ToList();
                }
                else
                {
                    list = list.ToList();
                }


                var requestsPerPage = list.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                mainClass.Results = requestsPerPage;
                mainClass.Count = list.Count();
                return mainClass;
            }
            return null;
        }

        public ViewAssetDetailVM GetAssetHistoryById(int assetId)
        {
            ViewAssetDetailVM model = new ViewAssetDetailVM();
            var lstHospitalAssets = _context.AssetDetails.Include(a => a.Supplier)
                                                    .Include(a => a.MasterAsset).Include(a => a.Hospital)
                                                    .Include(a => a.Hospital.Governorate)
                                                    .Include(a => a.Hospital.City)
                                                    .Include(a => a.Hospital.Organization).Include(a => a.Hospital.SubOrganization)
                                                    .Include(a => a.MasterAsset.brand)
                                                    .Include(a => a.MasterAsset.Category)
                                                    .Include(a => a.MasterAsset.SubCategory)
                                                    .Include(a => a.MasterAsset.ECRIS)
                                                    .Include(a => a.Department)
                                                    .Include(a => a.Building).Include(a => a.Floor).Include(a => a.Room)
                                                    .Include(a => a.MasterAsset.Origin).ToList().Where(a => a.Id == assetId).ToList();
            if (lstHospitalAssets.Count > 0)
            {
                var detailObj = lstHospitalAssets[0];
                model.Id = detailObj.Id;
                model.Code = detailObj.Code;
                model.PurchaseDate = detailObj.PurchaseDate != null ? detailObj.PurchaseDate.ToString() : "";
                model.Price = detailObj.Price.ToString();
                model.SerialNumber = detailObj.SerialNumber;
                model.Serial = detailObj.SerialNumber;
                model.Remarks = detailObj.Remarks;
                model.Barcode = detailObj.Barcode;
                model.InstallationDate = detailObj.InstallationDate != null ? detailObj.InstallationDate.Value.ToShortDateString() : "";
                model.WarrantyExpires = detailObj.WarrantyExpires;
                model.WarrantyStart = detailObj.WarrantyStart != null ? detailObj.WarrantyStart.Value.ToShortDateString() : "";
                model.WarrantyEnd = detailObj.WarrantyEnd != null ? detailObj.WarrantyEnd.Value.ToShortDateString() : "";
                model.ReceivingDate = detailObj.ReceivingDate != null ? detailObj.ReceivingDate.Value.ToShortDateString() : "";
                model.OperationDate = detailObj.OperationDate != null ? detailObj.OperationDate.Value.ToShortDateString() : "";
                model.CostCenter = detailObj.CostCenter;
                model.DepreciationRate = detailObj.DepreciationRate;
                model.PONumber = detailObj.PONumber;
                model.QrFilePath = detailObj.QrFilePath;

                model.MasterAssetId = detailObj.MasterAsset.Id;
                model.AssetName = detailObj.MasterAsset.Name;
                model.AssetNameAr = detailObj.MasterAsset.NameAr;
                model.MasterCode = detailObj.MasterAsset.Code;
                model.VersionNumber = detailObj.MasterAsset.VersionNumber;
                model.ModelNumber = detailObj.MasterAsset.ModelNumber;
                //model.Mo = detailObj.MasterAsset.ModelNumber;
                model.ExpectedLifeTime = detailObj.MasterAsset.ExpectedLifeTime != null ? (int)detailObj.MasterAsset.ExpectedLifeTime : 0;
                model.Description = detailObj.MasterAsset.Description;
                model.DescriptionAr = detailObj.MasterAsset.DescriptionAr;
                model.Length = detailObj.MasterAsset.Length.ToString();
                model.Width = detailObj.MasterAsset.Width.ToString();
                model.Weight = detailObj.MasterAsset.Weight.ToString();
                model.Height = detailObj.MasterAsset.Height.ToString();
                model.AssetImg = detailObj.MasterAsset.AssetImg;


                var lstAssetStatus = _context.AssetStatusTransactions.Include(a => a.AssetStatus).Where(a => a.AssetDetailId == detailObj.Id).ToList().OrderByDescending(a => a.StatusDate).ToList();

                if (lstAssetStatus.Count > 0)
                {
                    model.AssetStatus = lstAssetStatus[0].AssetStatus.Name;
                    model.AssetStatusAr = lstAssetStatus[0].AssetStatus.NameAr;
                }
                if (detailObj.BuildingId != null)
                {
                    model.BuildId = detailObj.Building.Id;
                    model.BuildName = detailObj.Building.Name;
                    model.BuildNameAr = detailObj.Building.NameAr;
                }
                if (detailObj.Floor != null)
                {
                    model.FloorId = detailObj.Floor.Id;
                    model.FloorName = detailObj.Floor.Name;
                    model.FloorNameAr = detailObj.Floor.NameAr;
                }
                if (detailObj.Room != null)
                {
                    model.RoomId = detailObj.Room.Id;
                    model.RoomName = detailObj.Room.Name;
                    model.RoomNameAr = detailObj.Room.NameAr;
                }
                if (detailObj.Department != null)
                {
                    model.DepartmentName = detailObj.Department.Name;
                    model.DepartmentNameAr = detailObj.Department.NameAr;
                }
                if (detailObj.MasterAsset.Category != null)
                {
                    model.CategoryName = detailObj.MasterAsset.Category.Name;
                    model.CategoryNameAr = detailObj.MasterAsset.Category.NameAr;
                }
                if (detailObj.Hospital != null)
                {
                    model.HospitalId = detailObj.Hospital.Id;
                    model.HospitalName = detailObj.Hospital.Name;
                    model.HospitalNameAr = detailObj.Hospital.NameAr;
                }
                if (detailObj.Hospital.Governorate != null)
                {
                    model.GovernorateName = detailObj.Hospital.Governorate.Name;
                    model.GovernorateNameAr = detailObj.Hospital.Governorate.NameAr;
                }
                if (detailObj.Hospital.City != null)
                {
                    model.CityName = detailObj.Hospital.City.Name;
                    model.CityNameAr = detailObj.Hospital.City.NameAr;
                }
                if (detailObj.Hospital.Organization != null)
                {
                    model.OrgName = detailObj.Hospital.Organization.Name;
                    model.OrgNameAr = detailObj.Hospital.Organization.NameAr;
                }

                if (detailObj.Hospital.SubOrganization != null)
                {
                    model.SubOrgName = detailObj.Hospital.SubOrganization.Name;
                    model.SubOrgNameAr = detailObj.Hospital.SubOrganization.NameAr;
                }
                if (detailObj.Supplier != null)
                {
                    model.SupplierName = detailObj.Supplier.Name;
                    model.SupplierNameAr = detailObj.Supplier.NameAr;
                }
                if (detailObj.MasterAsset.Category != null)
                {
                    model.CategoryName = detailObj.MasterAsset.Category.Name;
                    model.CategoryNameAr = detailObj.MasterAsset.Category.NameAr;
                }
                if (detailObj.MasterAsset.SubCategory != null)
                {
                    model.SubCategoryName = detailObj.MasterAsset.SubCategory.Name;
                    model.SubCategoryNameAr = detailObj.MasterAsset.SubCategory.NameAr;
                }
                if (detailObj.MasterAsset.Origin != null)
                {
                    model.OriginName = detailObj.MasterAsset.Origin.Name;
                    model.OriginNameAr = detailObj.MasterAsset.Origin.NameAr;
                }
                if (detailObj.MasterAsset.brand != null)
                {
                    model.BrandName = detailObj.MasterAsset.brand.Name;
                    model.BrandNameAr = detailObj.MasterAsset.brand.NameAr;
                }


                List<IndexRequestVM.GetData> lstAssetRequests = new List<IndexRequestVM.GetData>();
                List<ListWorkOrderVM.GetData> allWorkOrders = new List<ListWorkOrderVM.GetData>();
                var lstRequests = _context.Request.Where(a => a.AssetDetailId == assetId).ToList();
                if (lstRequests.Count > 0)
                {
                    foreach (var req in lstRequests)
                    {

                        IndexRequestVM.GetData requestVMObj = new IndexRequestVM.GetData();
                        requestVMObj.RequestCode = req.RequestCode;
                        requestVMObj.RequestDate = req.RequestDate;
                        requestVMObj.Subject = req.Subject;
                        var lstRequestTracking = _context.RequestTracking.Include(a => a.RequestStatus).Where(a => a.RequestId == req.Id).ToList().OrderByDescending(a => a.DescriptionDate.Value.Date).ToList().GroupBy(a => a.RequestId).ToList();

                        foreach (var reqtrack in lstRequestTracking)
                        {
                            requestVMObj.StatusId = (int)reqtrack.FirstOrDefault().RequestStatusId;
                            requestVMObj.StatusName = reqtrack.FirstOrDefault().RequestStatus.Name;
                            requestVMObj.StatusNameAr = reqtrack.FirstOrDefault().RequestStatus.NameAr;
                        }

                        var lstWorkOrders = _context.WorkOrders.Where(a => a.RequestId == req.Id).ToList();



                        if (lstWorkOrders.Count > 0)
                        {

                            foreach (var wo in lstWorkOrders)
                            {
                                ListWorkOrderVM.GetData workOrderVMObj = new ListWorkOrderVM.GetData();
                                workOrderVMObj.WorkOrderNumber = wo.WorkOrderNumber;
                                workOrderVMObj.ActualStartDate = wo.ActualStartDate;
                                workOrderVMObj.Subject = wo.Subject;
                                var lstWOTracking = _context.WorkOrderTrackings.Include(a => a.WorkOrderStatus).Where(a => a.WorkOrderId == wo.Id).ToList().OrderByDescending(a => a.ActualStartDate.Value.Date).ToList().GroupBy(a => a.WorkOrderId).ToList();

                                foreach (var wotrack in lstWOTracking)
                                {
                                    workOrderVMObj.StatusId = (int)wotrack.FirstOrDefault().WorkOrderStatusId;
                                    workOrderVMObj.StatusName = wotrack.FirstOrDefault().WorkOrderStatus.Name;
                                    workOrderVMObj.StatusNameAr = wotrack.FirstOrDefault().WorkOrderStatus.NameAr;
                                }
                                allWorkOrders.Add(workOrderVMObj);
                            }


                            requestVMObj.ListWorkOrders = allWorkOrders;

                        }

                        lstAssetRequests.Add(requestVMObj);
                    }
                }


                model.ListRequests = lstAssetRequests;

                // model.ListWorkOrders = allWorkOrders;
            }
            return model;
        }

        public IEnumerable<ViewAssetDetailVM> GetListOfAssetDetailsByHospitalNotInContract(string barcode, int hospitalId)
        {
            List<ViewAssetDetailVM> lstAssetDetails = new List<ViewAssetDetailVM>();
            lstAssetDetails = _context.AssetDetails.Include(a => a.MasterAsset).Include(a => a.Supplier)
                               .Where(a => a.Barcode.Contains(barcode) && a.HospitalId == hospitalId)
                               .Select(item => new ViewAssetDetailVM
                               {
                                   Id = item.Id,
                                   AssetName = item.MasterAsset.Name,
                                   AssetNameAr = item.MasterAsset.NameAr,
                                   SerialNumber = item.SerialNumber,
                                   SupplierName = item.Supplier.Name,
                                   SupplierNameAr = item.Supplier.NameAr,
                                   HospitalId = int.Parse(item.HospitalId.ToString()),
                                   HospitalName = item.Hospital.Name,
                                   HospitalNameAr = item.Hospital.NameAr,
                                   Barcode = item.Barcode
                               }).ToList();

            var contractAssetDetailIds = _context.ContractDetails.Include(a => a.AssetDetail).Include(a => a.AssetDetail.Hospital).Where(a => a.AssetDetail.HospitalId == hospitalId).Select(a => a.AssetDetailId).ToList();
            var assetDetailIds = _context.AssetDetails.ToList().Where(a => a.HospitalId == hospitalId).Select(a => a.Id).ToList();
            List<int> lstContractAssetDetailIds = new List<int>();
            if (contractAssetDetailIds.Count > 0)
            {
                foreach (var item in contractAssetDetailIds)
                {
                    lstContractAssetDetailIds.Add(int.Parse(item.ToString()));
                }
                var remainIds = assetDetailIds.Except(lstContractAssetDetailIds);
                lstAssetDetails = lstAssetDetails.Where(a => remainIds.Contains(a.Id)).ToList();
            }
            else
            {
                var remainIds = assetDetailIds;
                lstAssetDetails = lstAssetDetails.Where(a => remainIds.Contains(a.Id)).ToList();
            }
            return lstAssetDetails;
        }

        public IEnumerable<ViewAssetDetailVM> GetListOfAssetDetailsByHospitalNotInContractBySerialNumber(string serialNumber, int hospitalId)
        {
            List<ViewAssetDetailVM> lstAssetDetails = new List<ViewAssetDetailVM>();
            lstAssetDetails = _context.AssetDetails.Include(a => a.MasterAsset).Include(a => a.Supplier)
                               .Where(a => a.SerialNumber.Contains(serialNumber) && a.HospitalId == hospitalId)
                               .Select(item => new ViewAssetDetailVM
                               {
                                   Id = item.Id,
                                   AssetName = item.MasterAsset.Name,
                                   AssetNameAr = item.MasterAsset.NameAr,
                                   SerialNumber = item.SerialNumber,
                                   SupplierName = item.Supplier.Name,
                                   SupplierNameAr = item.Supplier.NameAr,
                                   HospitalId = int.Parse(item.HospitalId.ToString()),
                                   HospitalName = item.Hospital.Name,
                                   HospitalNameAr = item.Hospital.NameAr,
                                   Barcode = item.Barcode
                               }).ToList();

            var contractAssetDetailIds = _context.ContractDetails.Include(a => a.AssetDetail).Include(a => a.AssetDetail.Hospital).Where(a => a.AssetDetail.HospitalId == hospitalId).Select(a => a.AssetDetailId).ToList();
            var assetDetailIds = _context.AssetDetails.ToList().Where(a => a.HospitalId == hospitalId).Select(a => a.Id).ToList();
            List<int> lstContractAssetDetailIds = new List<int>();
            if (contractAssetDetailIds.Count > 0)
            {
                foreach (var item in contractAssetDetailIds)
                {
                    lstContractAssetDetailIds.Add(int.Parse(item.ToString()));
                }
                var remainIds = assetDetailIds.Except(lstContractAssetDetailIds);
                lstAssetDetails = lstAssetDetails.Where(a => remainIds.Contains(a.Id)).ToList();
            }
            else
            {
                var remainIds = assetDetailIds;
                lstAssetDetails = lstAssetDetails.Where(a => remainIds.Contains(a.Id)).ToList();
            }
            return lstAssetDetails;
        }



        public IEnumerable<IndexPMAssetTaskScheduleVM.GetData> GetAllPMAssetTaskScheduleByAssetId(int? assetId)
        {
            List<IndexPMAssetTaskScheduleVM.GetData> list = new List<IndexPMAssetTaskScheduleVM.GetData>();


            var lstSchedule = (from detail in _context.AssetDetails
                               join tsktime in _context.WNPMAssetTimes on detail.Id equals tsktime.AssetDetailId
                               join depart in _context.Departments on detail.DepartmentId equals depart.Id
                               where tsktime.PMDate.Value.Year == DateTime.Today.Date.Year
                                && detail.Id == assetId
                               select new
                               {
                                   Id = tsktime.Id,
                                   TimeId = tsktime.Id,
                                   MasterAssetId = detail.MasterAssetId,
                                   AssetDetailId = detail.Id,
                                   HospitalId = detail.HospitalId,
                                   Barcode = detail.Barcode,
                                   Serial = detail.SerialNumber,
                                   StartDate = tsktime.PMDate,
                                   EndDate = tsktime.PMDate,
                                   start = tsktime.PMDate.Value.ToString(),
                                   end = tsktime.PMDate.Value.ToString(),
                                   DepartmentName = depart.Name,
                                   DepartmentNameAr = depart.NameAr,
                                   IsDone = tsktime.IsDone,
                                   PMDate = tsktime.PMDate,
                                   allDay = true
                               }).ToList()
                               .GroupBy(a => new
                               {
                                   assetId = a.AssetDetailId,
                                   Day = a.StartDate.Value.Day,
                                   Month = a.StartDate.Value.Month,
                                   Year = a.StartDate.Value.Year
                               });


            foreach (var items in lstSchedule)
            {
                string month = "";
                string day = "";
                string endmonth = "";
                string endday = "";


                if (items.FirstOrDefault().StartDate.Value.Month < 10)
                    month = items.FirstOrDefault().StartDate.Value.Month.ToString().PadLeft(2, '0');
                else
                    month = items.FirstOrDefault().StartDate.Value.Month.ToString();

                if (items.FirstOrDefault().EndDate.Value.Month < 10)
                    endmonth = items.FirstOrDefault().EndDate.Value.Month.ToString().PadLeft(2, '0');
                else
                    endmonth = items.FirstOrDefault().EndDate.Value.Month.ToString();

                if (items.FirstOrDefault().StartDate.Value.Day < 10)
                    day = items.FirstOrDefault().StartDate.Value.Day.ToString().PadLeft(2, '0');
                else
                    day = items.FirstOrDefault().StartDate.Value.Day.ToString();

                if (items.FirstOrDefault().EndDate.Value.Day < 10)
                    endday = (items.FirstOrDefault().EndDate.Value.Day).ToString().PadLeft(2, '0');
                else
                    endday = items.FirstOrDefault().EndDate.Value.Day.ToString();




                IndexPMAssetTaskScheduleVM.GetData getDataObj = new IndexPMAssetTaskScheduleVM.GetData();
                var AssetName = _context.MasterAssets.Where(a => a.Id == items.FirstOrDefault().MasterAssetId).ToList().FirstOrDefault().Name;
                var AssetNameAr = _context.MasterAssets.Where(a => a.Id == items.FirstOrDefault().MasterAssetId).ToList().FirstOrDefault().NameAr;
                //  var color = _context.MasterAssets.Where(a => a.Id == items.FirstOrDefault().MasterAssetId).ToList().FirstOrDefault().PMBGColor;
                //var textColor = _context.MasterAssets.Where(a => a.Id == items.FirstOrDefault().MasterAssetId).ToList().FirstOrDefault().PMColor;
                var Serial = items.FirstOrDefault().Serial;
                var DepartmentName = items.FirstOrDefault().DepartmentName;
                var DepartmentNameAr = items.FirstOrDefault().DepartmentNameAr;
                var Barcode = items.FirstOrDefault().Barcode;

                getDataObj.start = items.FirstOrDefault().StartDate.Value.Year + "-" + month + "-" + day;
                getDataObj.end = items.FirstOrDefault().EndDate.Value.Year + "-" + endmonth + "-" + endday;
                getDataObj.title = AssetName + " - " + Serial + " - " + DepartmentName + " - " + Barcode;
                getDataObj.titleAr = AssetNameAr + "  -  " + Serial + " - " + DepartmentNameAr + " - " + Barcode;


                if (items.FirstOrDefault().IsDone == true)
                {
                    getDataObj.color = "#79be47";
                    getDataObj.textColor = "#fff";
                }
                else if (items.FirstOrDefault().PMDate < DateTime.Today.Date && items.FirstOrDefault().IsDone == false)
                {
                    getDataObj.color = "#ff7578";
                    getDataObj.textColor = "#fff";
                }


                getDataObj.Id = items.FirstOrDefault().Id;
                //getDataObj.color = color;
                //getDataObj.textColor = textColor;
                getDataObj.start = items.FirstOrDefault().StartDate.Value.Year + "-" + month + "-" + day;
                getDataObj.end = items.FirstOrDefault().EndDate.Value.Year + "-" + endmonth + "-" + endday;
                getDataObj.allDay = true;
                getDataObj.ListTasks = _context.PMAssetTasks.Where(a => a.MasterAssetId == items.FirstOrDefault().MasterAssetId).ToList();

                list.Add(getDataObj);

            }
            return list;
        }

        public MobileAssetDetailVM GetAssetDetailById(string userId, int assetId)
        {
            ApplicationUser UserObj = new ApplicationUser();
            List<string> userRoleNames = new List<string>();


            var obj = _context.ApplicationUser.Where(a => a.Id == userId).ToList();
            if (obj.Count > 0)
            {
                UserObj = obj[0];
                var roleNames = (from userRole in _context.UserRoles
                                 join role in _context.Roles on userRole.RoleId equals role.Id
                                 where userRole.UserId == userId
                                 select role);
                foreach (var name in roleNames)
                {
                    userRoleNames.Add(name.Name);
                }
            }
            var lstAssetDetails = _context.AssetDetails
                              .Include(a => a.MasterAsset)
                              .Include(a => a.MasterAsset.brand)
                              .Include(a => a.Building)
                              .Include(a => a.Floor).Include(a => a.Room)
                              .Include(a => a.Hospital)
                              .Include(a => a.Hospital.Governorate)
                              .Include(a => a.Hospital.City)
                              .Include(a => a.Hospital.Organization)
                              .Include(a => a.Hospital.SubOrganization)
                            .Include(a => a.Supplier)
                            .Include(a => a.Department)
                         .ToList().Where(a => a.Id == assetId).ToList();
            if (lstAssetDetails.Count > 0)
            {
                var assetDetailObj = lstAssetDetails[0];
                MobileAssetDetailVM item = new MobileAssetDetailVM();
                item.Id = assetDetailObj.Id;
                item.CreatedBy = assetDetailObj.CreatedBy;
                item.MasterAssetId = assetDetailObj.MasterAssetId;
                item.AssetName = assetDetailObj.MasterAsset.Name;
                item.AssetNameAr = assetDetailObj.MasterAsset.NameAr;
                item.ModelNumber = assetDetailObj.MasterAsset.ModelNumber;
                item.Code = assetDetailObj.Code;
                item.PurchaseDate = assetDetailObj.PurchaseDate != null ? assetDetailObj.PurchaseDate.Value.ToShortDateString() : "";
                item.Price = assetDetailObj.Price;
                item.SerialNumber = assetDetailObj.SerialNumber;
                item.Remarks = assetDetailObj.Remarks;
                item.Barcode = assetDetailObj.Barcode;
                item.InstallationDate = assetDetailObj.InstallationDate != null ? assetDetailObj.InstallationDate.Value.ToShortDateString() : "";
                item.OperationDate = assetDetailObj.OperationDate != null ? assetDetailObj.OperationDate.Value.ToShortDateString() : "";
                item.ReceivingDate = assetDetailObj.ReceivingDate != null ? assetDetailObj.ReceivingDate.Value.ToShortDateString() : "";
                item.PONumber = assetDetailObj.PONumber;
                item.WarrantyExpires = assetDetailObj.WarrantyExpires;

                item.AssetImg = assetDetailObj.MasterAsset.AssetImg;

                item.BuildingId = assetDetailObj.BuildingId;
                if (assetDetailObj.BuildingId != null)
                {
                    item.BuildName = assetDetailObj.Building.Name;
                    item.BuildNameAr = assetDetailObj.Building.NameAr;
                }
                item.RoomId = assetDetailObj.RoomId;
                if (assetDetailObj.RoomId != null)
                {
                    item.RoomName = assetDetailObj.Room.Name;
                    item.RoomNameAr = assetDetailObj.Room.NameAr;
                }
                item.FloorId = assetDetailObj.FloorId;
                if (assetDetailObj.FloorId != null)
                {
                    item.FloorName = assetDetailObj.Floor.Name;
                    item.FloorNameAr = assetDetailObj.Floor.NameAr;
                }

                if (assetDetailObj.MasterAsset.brand != null)
                {
                    item.BrandName = assetDetailObj.MasterAsset.brand.Name;
                    item.BrandNameAr = assetDetailObj.MasterAsset.brand.NameAr;
                }

                item.DepartmentId = assetDetailObj.Department != null ? assetDetailObj.DepartmentId : 0;
                item.DepartmentName = assetDetailObj.Department != null ? assetDetailObj.Department.Name : "";
                item.DepartmentNameAr = assetDetailObj.Department != null ? assetDetailObj.Department.NameAr : "";


                var lstAssetTransactions = _context.AssetStatusTransactions.Include(a => a.AssetStatus).Where(a => a.AssetDetailId == assetDetailObj.Id).ToList().OrderByDescending(a => a.StatusDate).ToList();
                if (lstAssetTransactions.Count > 0)
                {
                    item.AssetStatusId = lstAssetTransactions[0].AssetStatus.Id;
                    item.AssetStatus = lstAssetTransactions[0].AssetStatus.Name;
                    item.AssetStatusAr = lstAssetTransactions[0].AssetStatus.NameAr;
                }

                item.DepartmentId = assetDetailObj.DepartmentId;
                item.SupplierId = assetDetailObj.SupplierId;
                item.HospitalId = assetDetailObj.HospitalId;
                if (assetDetailObj.HospitalId != null)
                {
                    item.HospitalName = assetDetailObj.Hospital.Name;
                    item.HospitalNameAr = assetDetailObj.Hospital.NameAr;
                }
                item.MasterAssetId = assetDetailObj.MasterAssetId;
                item.WarrantyStart = assetDetailObj.WarrantyStart != null ? assetDetailObj.WarrantyStart.Value.ToShortDateString() : "";
                item.WarrantyEnd = assetDetailObj.WarrantyEnd != null ? assetDetailObj.WarrantyEnd.Value.ToShortDateString() : "";
                item.CostCenter = assetDetailObj.CostCenter;
                item.DepreciationRate = assetDetailObj.DepreciationRate;
                item.QrFilePath = assetDetailObj.QrFilePath;

                item.GovernorateId = assetDetailObj.Hospital.GovernorateId;
                item.CityId = assetDetailObj.Hospital.CityId;
                item.OrganizationId = assetDetailObj.Hospital.OrganizationId;
                item.SubOrganizationId = assetDetailObj.Hospital.SubOrganizationId;



                item.SupplierName = assetDetailObj.Supplier != null ? assetDetailObj.Supplier.Name : "";
                item.SupplierNameAr = assetDetailObj.Supplier != null ? assetDetailObj.Supplier.NameAr : "";

                item.BrandId = assetDetailObj.MasterAsset.brand != null ? assetDetailObj.MasterAsset.BrandId : 0;
                item.BrandName = assetDetailObj.MasterAsset.brand != null ? assetDetailObj.MasterAsset.brand.Name : "";
                item.BrandNameAr = assetDetailObj.MasterAsset.brand != null ? assetDetailObj.MasterAsset.brand.NameAr : "";
                List<Request> lstRequests = new List<Request>();


                lstRequests = _context.Request
                                          .Include(a => a.Hospital)
                                           .Include(a => a.Hospital.Governorate)
                                            .Include(a => a.Hospital.City)
                                             .Include(a => a.Hospital.Organization)
                                              .Include(a => a.Hospital.SubOrganization)
                                          .Include(a => a.AssetDetail)
                                          .Include(a => a.AssetDetail.MasterAsset)
                                          .Include(a => a.AssetDetail.MasterAsset.brand).Where(a => a.AssetDetailId == assetId).ToList();

                if (userRoleNames.Contains("AssetOwner"))
                {
                    lstRequests = lstRequests.Where(a => a.AssetDetailId == assetId && a.CreatedById == userId).OrderByDescending(a => a.RequestDate).ToList();
                }
                if (userRoleNames.Contains("SRCreator") && !userRoleNames.Contains("SRReviewer"))
                {
                    lstRequests = lstRequests.Where(a => a.AssetDetailId == assetId && a.CreatedById == userId).OrderByDescending(a => a.RequestDate).ToList();
                }
                if (userRoleNames.Contains("SRCreator") && userRoleNames.Contains("SRReviewer"))
                {
                    lstRequests = lstRequests.Where(a => a.AssetDetailId == assetId).OrderByDescending(a => a.RequestDate).ToList();

                }
                if (userRoleNames.Contains("SRReviewer") && !userRoleNames.Contains("SRCreator"))
                {
                    lstRequests = lstRequests.Where(a => a.AssetDetailId == assetId).OrderByDescending(a => a.RequestDate).ToList();
                }
                if (userRoleNames.Contains("EngDepManager"))
                {
                    lstRequests = lstRequests.Where(a => a.AssetDetailId == assetId).OrderByDescending(a => a.RequestDate).ToList();
                }
                if (userRoleNames.Contains("TLHospitalManager"))
                {
                    lstRequests = lstRequests.Where(a => a.AssetDetailId == assetId).OrderByDescending(a => a.RequestDate).ToList();
                }
                if (UserObj.GovernorateId == 0 && UserObj.CityId == 0 && UserObj.OrganizationId == 0 && UserObj.SubOrganizationId == 0 && UserObj.HospitalId == 0)
                {
                    lstRequests = lstRequests.ToList();
                }
                if (UserObj.GovernorateId > 0 && UserObj.CityId == 0 && UserObj.OrganizationId == 0 && UserObj.SubOrganizationId == 0 && UserObj.HospitalId == 0)
                {
                    lstRequests = lstRequests.Where(a => a.AssetDetail.Hospital.GovernorateId == UserObj.GovernorateId).ToList();
                }
                if (UserObj.GovernorateId > 0 && UserObj.CityId > 0 && UserObj.OrganizationId == 0 && UserObj.SubOrganizationId == 0 && UserObj.HospitalId == 0)
                {
                    lstRequests = lstRequests.Where(a => a.AssetDetail.Hospital.GovernorateId == UserObj.GovernorateId && a.AssetDetail.Hospital.CityId == UserObj.CityId).ToList();
                }
                if (UserObj.GovernorateId > 0 && UserObj.CityId > 0 && UserObj.OrganizationId == 0 && UserObj.SubOrganizationId == 0 && UserObj.HospitalId > 0)
                {
                    lstRequests = lstRequests.Where(a => a.AssetDetail.Hospital.GovernorateId == UserObj.GovernorateId && a.AssetDetail.Hospital.CityId == UserObj.CityId && a.HospitalId == UserObj.HospitalId).ToList();
                }
                List<IndexRequestsVM> requests = new List<IndexRequestsVM>();
                foreach (var req in lstRequests)
                {
                    IndexRequestsVM requestObj = new IndexRequestsVM();
                    requestObj.Id = req.Id;
                    requestObj.RequestCode = req.RequestCode;
                    requestObj.RequestDate = req.RequestDate;

                    var lstRequestTracking = _context.RequestTracking.Include(a => a.RequestStatus).Where(a => a.RequestId == req.Id).ToList().OrderByDescending(a => a.DescriptionDate.Value).ToList().GroupBy(a => a.RequestId).ToList();
                    if (lstRequestTracking.Count > 0)
                    {
                        foreach (var reqtrack in lstRequestTracking)
                        {
                            requestObj.StatusId = (int)reqtrack.FirstOrDefault().RequestStatusId;
                            requestObj.StatusName = reqtrack.FirstOrDefault().RequestStatus.Name;
                            requestObj.StatusNameAr = reqtrack.FirstOrDefault().RequestStatus.NameAr;
                        }
                        requests.Add(requestObj);
                    }
                }
                item.ListRequests = requests;

                if (!userRoleNames.Any(s => s == "VisitEngineer" || s == "VisitManagerEngineer"))
                {
                    return item;
                }
            }




            return null;
        }

        public bool GenerateQrCodeForAllAssets(string domainName)
        {
            var lstAssets = _context.AssetDetails.Include(a => a.Department).Include(a => a.MasterAsset).Include(a => a.MasterAsset.brand).ToList();
            foreach (var item in lstAssets)
            {

                string url = domainName + "/#/dash/hospitalassets/detail/" + item.Id;

                //string url = domainName + "/#/dash/hospitalassets/detail/" + item.Id;
                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.L);
                QRCode qrCode = new QRCode(qrCodeData);
                Bitmap qrCodeImage = qrCode.GetGraphic(15);
                var bitmapFiles = BitmapToBytes(qrCodeImage, item.Id);
                var assetObj = _context.AssetDetails.Find(item.Id);
                assetObj.QrFilePath = url;
                _context.Entry(assetObj).State = EntityState.Modified;
                _context.SaveChanges();



                string assetObjData = "AssetName " + item.MasterAsset.Name + ";\nManufacture " + item.MasterAsset.brand.Name + ";\nModel " + item.MasterAsset.ModelNumber + ";\nSerialNumber " + item.SerialNumber + ";\nBarcode " + item.Barcode + ";\nDepartmentName " + item.Department.Name;
                QRCodeGenerator qrGenerator2 = new QRCodeGenerator();
                QRCodeData qrCodeData2 = qrGenerator2.CreateQrCode(assetObjData, QRCodeGenerator.ECCLevel.L, true);
                var asset = _context.AssetDetails.Where(e => e.Id == item.Id).FirstOrDefault();
                asset.QrData = assetObjData;
                _context.Entry(asset).State = EntityState.Modified;
                _context.SaveChanges();


            }
            return true;

        }



        public MobileAssetDetailVM2 GetAssetDetailByIdOnly(string userId, int assetId)
        {
            ApplicationUser UserObj = new ApplicationUser();
            List<string> userRoleNames = new List<string>();


            var obj = _context.ApplicationUser.Where(a => a.Id == userId).ToList();
            if (obj.Count > 0)
            {
                UserObj = obj[0];
                var roleNames = (from userRole in _context.UserRoles
                                 join role in _context.Roles on userRole.RoleId equals role.Id
                                 where userRole.UserId == userId
                                 select role);
                foreach (var name in roleNames)
                {
                    userRoleNames.Add(name.Name);
                }
            }

            var lstAssetDetails = _context.AssetDetails
                               .Include(a => a.MasterAsset)
                               .Include(a => a.MasterAsset.brand)
                               .Include(a => a.Building)
                               .Include(a => a.Floor).Include(a => a.Room)
                               .Include(a => a.Hospital)
                               .Include(a => a.Hospital.Governorate)
                               .Include(a => a.Hospital.City)
                               .Include(a => a.Hospital.Organization)
                               .Include(a => a.Hospital.SubOrganization)
                             .Include(a => a.Supplier)
                             .Include(a => a.Department)
                          .ToList().Where(a => a.Id == assetId).ToList();

            if (lstAssetDetails.Count > 0)
            {
                var assetDetailObj = lstAssetDetails[0];
                MobileAssetDetailVM2 item = new MobileAssetDetailVM2();
                item.Id = assetDetailObj.Id;
                item.CreatedBy = assetDetailObj.CreatedBy;
                item.MasterAssetId = assetDetailObj.MasterAssetId;
                item.AssetName = assetDetailObj.MasterAsset.Name;
                item.AssetNameAr = assetDetailObj.MasterAsset.NameAr;
                item.ModelNumber = assetDetailObj.MasterAsset.ModelNumber;
                item.Code = assetDetailObj.Code;
                item.PurchaseDate = assetDetailObj.PurchaseDate != null ? assetDetailObj.PurchaseDate.Value.ToShortDateString() : "";
                item.Price = assetDetailObj.Price;
                item.SerialNumber = assetDetailObj.SerialNumber;
                item.Remarks = assetDetailObj.Remarks;
                item.Barcode = assetDetailObj.Barcode;
                item.InstallationDate = assetDetailObj.InstallationDate != null ? assetDetailObj.InstallationDate.Value.ToShortDateString() : "";
                item.OperationDate = assetDetailObj.OperationDate != null ? assetDetailObj.OperationDate.Value.ToShortDateString() : "";
                item.ReceivingDate = assetDetailObj.ReceivingDate != null ? assetDetailObj.ReceivingDate.Value.ToShortDateString() : "";
                item.PONumber = assetDetailObj.PONumber;
                item.WarrantyExpires = assetDetailObj.WarrantyExpires;

                item.AssetImg = assetDetailObj.MasterAsset.AssetImg;

                item.BuildingId = assetDetailObj.BuildingId;
                if (assetDetailObj.BuildingId != null)
                {
                    item.BuildName = assetDetailObj.Building.Name;
                    item.BuildNameAr = assetDetailObj.Building.NameAr;
                }
                item.RoomId = assetDetailObj.RoomId;
                if (assetDetailObj.RoomId != null)
                {
                    item.RoomName = assetDetailObj.Room.Name;
                    item.RoomNameAr = assetDetailObj.Room.NameAr;
                }
                item.FloorId = assetDetailObj.FloorId;
                if (assetDetailObj.FloorId != null)
                {
                    item.FloorName = assetDetailObj.Floor.Name;
                    item.FloorNameAr = assetDetailObj.Floor.NameAr;
                }

                if (assetDetailObj.MasterAsset.brand != null)
                {
                    item.BrandName = assetDetailObj.MasterAsset.brand.Name;
                    item.BrandNameAr = assetDetailObj.MasterAsset.brand.NameAr;
                }

                item.DepartmentId = assetDetailObj.Department != null ? assetDetailObj.DepartmentId : 0;
                item.DepartmentName = assetDetailObj.Department != null ? assetDetailObj.Department.Name : "";
                item.DepartmentNameAr = assetDetailObj.Department != null ? assetDetailObj.Department.NameAr : "";


                var lstAssetTransactions = _context.AssetStatusTransactions.Include(a => a.AssetStatus).Where(a => a.AssetDetailId == assetDetailObj.Id).ToList().OrderByDescending(a => a.StatusDate).ToList();
                if (lstAssetTransactions.Count > 0)
                {
                    item.AssetStatusId = lstAssetTransactions[0].AssetStatus.Id;
                    item.AssetStatus = lstAssetTransactions[0].AssetStatus.Name;
                    item.AssetStatusAr = lstAssetTransactions[0].AssetStatus.NameAr;
                }

                item.DepartmentId = assetDetailObj.DepartmentId;
                item.SupplierId = assetDetailObj.SupplierId;
                item.HospitalId = assetDetailObj.HospitalId;
                if (assetDetailObj.HospitalId != null)
                {
                    item.HospitalName = assetDetailObj.Hospital.Name;
                    item.HospitalNameAr = assetDetailObj.Hospital.NameAr;
                }
                item.MasterAssetId = assetDetailObj.MasterAssetId;
                item.WarrantyStart = assetDetailObj.WarrantyStart != null ? assetDetailObj.WarrantyStart.Value.ToShortDateString() : "";
                item.WarrantyEnd = assetDetailObj.WarrantyEnd != null ? assetDetailObj.WarrantyEnd.Value.ToShortDateString() : "";
                item.CostCenter = assetDetailObj.CostCenter;
                item.DepreciationRate = assetDetailObj.DepreciationRate;
                item.QrFilePath = assetDetailObj.QrFilePath;

                item.GovernorateId = assetDetailObj.Hospital.GovernorateId;
                item.CityId = assetDetailObj.Hospital.CityId;
                item.OrganizationId = assetDetailObj.Hospital.OrganizationId;
                item.SubOrganizationId = assetDetailObj.Hospital.SubOrganizationId;



                item.SupplierName = assetDetailObj.Supplier != null ? assetDetailObj.Supplier.Name : "";
                item.SupplierNameAr = assetDetailObj.Supplier != null ? assetDetailObj.Supplier.NameAr : "";

                item.BrandId = assetDetailObj.MasterAsset.brand != null ? assetDetailObj.MasterAsset.BrandId : 0;
                item.BrandName = assetDetailObj.MasterAsset.brand != null ? assetDetailObj.MasterAsset.brand.Name : "";
                item.BrandNameAr = assetDetailObj.MasterAsset.brand != null ? assetDetailObj.MasterAsset.brand.NameAr : "";


                return item;

            }
            return null;
        }

        public IEnumerable<ViewAssetDetailVM> GetAutoCompleteSupplierExcludedAssetsByHospitalId(string barcode, int hospitalId)
        {
            List<ViewAssetDetailVM> viewAssetDetailList = new List<ViewAssetDetailVM>();
            var lstExcludes = _context.SupplierExecludeAssets.Include(a => a.AssetDetail).Where(a => a.AssetDetail.HospitalId == hospitalId).ToList();
            foreach (var asset in lstExcludes)
            {
                var lstAssetTransactions = _context.AssetStatusTransactions.Where(a => a.AssetDetailId == asset.AssetId).OrderByDescending(a => a.StatusDate.Value.ToString()).ToList();
                if (lstAssetTransactions.Count > 0)
                {
                    var transObj = lstAssetTransactions.FirstOrDefault();
                    //if (transObj.AssetStatusId == 3)
                    //{
                    var assetItem = _context.AssetDetails.Include(a => a.MasterAsset).Include(a => a.Supplier).Include(a => a.Hospital)
                                               .Where(a => a.Barcode.Contains(barcode) && a.Id == asset.AssetId).ToList();
                    foreach (var item in assetItem)
                    {
                        ViewAssetDetailVM assetBarCode = new ViewAssetDetailVM();
                        assetBarCode.Id = item.Id;
                        assetBarCode.AssetName = item.MasterAsset.Name;
                        assetBarCode.AssetNameAr = item.MasterAsset.NameAr;
                        assetBarCode.SerialNumber = item.SerialNumber;
                        assetBarCode.SupplierName = item.Supplier != null ? item.Supplier.Name : "";
                        assetBarCode.SupplierNameAr = item.Supplier != null ? item.Supplier.NameAr : "";
                        assetBarCode.HospitalId = item.Hospital.Id;
                        assetBarCode.HospitalName = item.Hospital.Name;
                        assetBarCode.HospitalNameAr = item.Hospital.NameAr;
                        assetBarCode.Barcode = item.Barcode;
                        assetBarCode.BarCode = item.Barcode;
                        assetBarCode.MasterAssetId = item.MasterAsset.Id;
                        assetBarCode.MasterAssetName = item.MasterAsset.Name;
                        assetBarCode.MasterAssetNameAr = item.MasterAsset.NameAr;
                        viewAssetDetailList.Add(assetBarCode);
                    }
                    // }
                }
            }
            return viewAssetDetailList;
        }


        public IndexAssetDetailVM GeoSortAssetsWithoutSearch(Sort sortObj, int pageNumber, int pageSize)
        {




            IndexAssetDetailVM mainClass = new IndexAssetDetailVM();
            List<IndexAssetDetailVM.GetData> list = new List<IndexAssetDetailVM.GetData>();
            IQueryable<AssetDetail> lstAllAssets = _context.AssetDetails.Include(a => a.MasterAsset).Include(a => a.Hospital).Include(a => a.Department)
                                   .Include(a => a.Supplier).Include(a => a.MasterAsset.brand)
                                   .Include(a => a.Hospital.Governorate).Include(a => a.Hospital.City);




            if (lstAllAssets.Count() > 0)
            {
                if (sortObj.GovernorateId != 0)
                {

                    lstAllAssets = lstAllAssets.Where(h => h.Hospital.GovernorateId == sortObj.GovernorateId);
                    if (sortObj.HospitalName != "")
                    {
                        if (sortObj.SortStatus == "descending")
                        {
                            lstAllAssets = lstAllAssets.OrderByDescending(d => d.Hospital.Name);
                        }
                        else
                        {
                            lstAllAssets = lstAllAssets.OrderBy(d => d.Hospital.Name);
                        }
                    }
                    if (sortObj.HospitalNameAr != "")
                    {
                        if (sortObj.SortStatus == "descending")
                        {
                            lstAllAssets = lstAllAssets.OrderByDescending(d => d.Hospital.NameAr);
                        }
                        else
                        {
                            lstAllAssets = lstAllAssets.OrderBy(d => d.Hospital.NameAr);
                        }
                    }
                    if (sortObj.BarCode != "")
                    {
                        if (sortObj.SortStatus == "descending")
                        {
                            lstAllAssets = lstAllAssets.OrderByDescending(d => d.Barcode);
                        }
                        else
                        {
                            lstAllAssets = lstAllAssets.OrderBy(d => d.Barcode);
                        }
                    }

                    if (sortObj.AssetName != "")
                    {
                        if (sortObj.SortStatus == "descending")
                        {
                            lstAllAssets = lstAllAssets.OrderByDescending(d => d.MasterAsset.Name);
                        }
                        else
                        {
                            lstAllAssets = lstAllAssets.OrderBy(d => d.MasterAsset.Name);
                        }
                    }
                    if (sortObj.AssetNameAr != "")
                    {
                        if (sortObj.SortStatus == "descending")
                        {
                            lstAllAssets = lstAllAssets.OrderByDescending(d => d.MasterAsset.NameAr);
                        }
                        else
                        {
                            lstAllAssets = lstAllAssets.OrderBy(d => d.MasterAsset.NameAr);
                        }
                    }
                    if (sortObj.SerialValue != "")
                    {
                        if (sortObj.SortStatus == "descending")
                        {
                            lstAllAssets = lstAllAssets.OrderByDescending(d => d.SerialNumber);
                        }
                        else
                        {
                            lstAllAssets = lstAllAssets.OrderBy(d => d.SerialNumber);
                        }
                    }


                    if (sortObj.Model != "")
                    {
                        if (sortObj.SortStatus == "descending")
                        {
                            lstAllAssets = lstAllAssets.OrderByDescending(d => d.MasterAsset.ModelNumber);
                        }
                        else
                        {
                            lstAllAssets = lstAllAssets.OrderBy(d => d.MasterAsset.ModelNumber);
                        }
                    }
                    if (sortObj.BrandName != "")
                    {
                        if (sortObj.SortStatus == "descending")
                        {
                            lstAllAssets = lstAllAssets.OrderByDescending(d => d.MasterAsset.brand.Name);
                        }
                        else
                        {
                            lstAllAssets = lstAllAssets.OrderBy(d => d.MasterAsset.brand.Name);
                        }
                    }
                    if (sortObj.BrandNameAr != "")
                    {
                        if (sortObj.SortStatus == "descending")
                        {
                            lstAllAssets = lstAllAssets.OrderByDescending(d => d.MasterAsset.brand.Name);
                        }
                        else
                        {
                            lstAllAssets = lstAllAssets.OrderBy(d => d.MasterAsset.brand.Name);
                        }
                    }
                    if (sortObj.SupplierName != "")
                    {
                        if (sortObj.SortStatus == "descending")
                        {
                            lstAllAssets = lstAllAssets.OrderByDescending(d => d.Supplier.Name);
                        }
                        else
                        {
                            lstAllAssets = lstAllAssets.OrderBy(d => d.Supplier.Name);
                        }
                    }
                    if (sortObj.SupplierNameAr != "")
                    {
                        if (sortObj.SortStatus == "descending")
                        {
                            lstAllAssets = lstAllAssets.OrderByDescending(d => d.Supplier.Name);
                        }
                        else
                        {
                            lstAllAssets = lstAllAssets.OrderBy(d => d.Supplier.Name);
                        }
                    }


                }


                if (sortObj.HospitalId != 0)
                {

                    lstAllAssets = lstAllAssets.Where(h => h.HospitalId == sortObj.HospitalId);
                    if (sortObj.BarCode != "")
                    {
                        if (sortObj.SortStatus == "descending")
                        {
                            lstAllAssets = lstAllAssets.OrderByDescending(d => d.Barcode);
                        }
                        else
                        {
                            lstAllAssets = lstAllAssets.OrderBy(d => d.Barcode);
                        }
                    }
                    if (sortObj.SerialValue != "")
                    {
                        if (sortObj.SortStatus == "descending")
                        {
                            lstAllAssets = lstAllAssets.OrderByDescending(d => d.SerialNumber);
                        }
                        else
                        {
                            lstAllAssets = lstAllAssets.OrderBy(d => d.SerialNumber);
                        }
                    }
                    if (sortObj.HospitalName != "")
                    {
                        if (sortObj.SortStatus == "descending")
                        {
                            lstAllAssets = lstAllAssets.OrderByDescending(d => d.Hospital.Name);
                        }
                        else
                        {
                            lstAllAssets = lstAllAssets.OrderBy(d => d.Hospital.Name);
                        }
                    }
                    if (sortObj.HospitalNameAr != "")
                    {
                        if (sortObj.SortStatus == "descending")
                        {
                            lstAllAssets = lstAllAssets.OrderByDescending(d => d.Hospital.NameAr);
                        }
                        else
                        {
                            lstAllAssets = lstAllAssets.OrderBy(d => d.Hospital.NameAr);
                        }
                    }
                    if (sortObj.AssetName != "")
                    {
                        if (sortObj.SortStatus == "descending")
                        {
                            lstAllAssets = lstAllAssets.OrderByDescending(d => d.MasterAsset.Name);
                        }
                        else
                        {
                            lstAllAssets = lstAllAssets.OrderBy(d => d.MasterAsset.Name);
                        }
                    }
                    if (sortObj.AssetNameAr != "")
                    {
                        if (sortObj.SortStatus == "descending")
                        {
                            lstAllAssets = lstAllAssets.OrderByDescending(d => d.MasterAsset.NameAr);
                        }
                        else
                        {
                            lstAllAssets = lstAllAssets.OrderBy(d => d.MasterAsset.NameAr);
                        }
                    }
                    if (sortObj.Model != "")
                    {
                        if (sortObj.SortStatus == "descending")
                        {
                            lstAllAssets = lstAllAssets.OrderByDescending(d => d.MasterAsset.ModelNumber);
                        }
                        else
                        {
                            lstAllAssets = lstAllAssets.OrderBy(d => d.MasterAsset.ModelNumber);
                        }
                    }
                    if (sortObj.BrandName != "")
                    {
                        if (sortObj.SortStatus == "descending")
                        {
                            lstAllAssets = lstAllAssets.OrderByDescending(d => d.MasterAsset.brand.Name);
                        }
                        else
                        {
                            lstAllAssets = lstAllAssets.OrderBy(d => d.MasterAsset.brand.Name);
                        }
                    }
                    if (sortObj.BrandNameAr != "")
                    {
                        if (sortObj.SortStatus == "descending")
                        {
                            lstAllAssets = lstAllAssets.OrderByDescending(d => d.MasterAsset.brand.Name);
                        }
                        else
                        {
                            lstAllAssets = lstAllAssets.OrderBy(d => d.MasterAsset.brand.Name);
                        }
                    }
                    if (sortObj.SupplierName != "")
                    {
                        if (sortObj.SortStatus == "descending")
                        {
                            lstAllAssets = lstAllAssets.OrderByDescending(d => d.Supplier.Name);
                        }
                        else
                        {
                            lstAllAssets = lstAllAssets.OrderBy(d => d.Supplier.Name);
                        }
                    }
                    if (sortObj.SupplierNameAr != "")
                    {
                        if (sortObj.SortStatus == "descending")
                        {
                            lstAllAssets = lstAllAssets.OrderByDescending(d => d.Supplier.Name);
                        }
                        else
                        {
                            lstAllAssets = lstAllAssets.OrderBy(d => d.Supplier.Name);
                        }
                    }
                }


                if (sortObj.DepartmentId != 0)
                {

                    lstAllAssets = lstAllAssets.Where(h => h.DepartmentId == sortObj.DepartmentId && h.HospitalId == sortObj.HospitalId);

                    if (sortObj.HospitalName != "")
                    {
                        if (sortObj.SortStatus == "descending")
                        {
                            lstAllAssets = lstAllAssets.OrderByDescending(d => d.Hospital.Name);
                        }
                        else
                        {
                            lstAllAssets = lstAllAssets.OrderBy(d => d.Hospital.Name);
                        }
                    }
                    if (sortObj.HospitalNameAr != "")
                    {
                        if (sortObj.SortStatus == "descending")
                        {
                            lstAllAssets = lstAllAssets.OrderByDescending(d => d.Hospital.NameAr);
                        }
                        else
                        {
                            lstAllAssets = lstAllAssets.OrderBy(d => d.Hospital.NameAr);
                        }
                    }

                    if (sortObj.AssetName != "")
                    {
                        if (sortObj.SortStatus == "descending")
                        {
                            lstAllAssets = lstAllAssets.OrderByDescending(d => d.MasterAsset.Name);
                        }
                        else
                        {
                            lstAllAssets = lstAllAssets.OrderBy(d => d.MasterAsset.Name);
                        }
                    }
                    if (sortObj.AssetNameAr != "")
                    {
                        if (sortObj.SortStatus == "descending")
                        {
                            lstAllAssets = lstAllAssets.OrderByDescending(d => d.MasterAsset.NameAr);
                        }
                        else
                        {
                            lstAllAssets = lstAllAssets.OrderBy(d => d.MasterAsset.NameAr);
                        }
                    }
                    if (sortObj.BarCode != "")
                    {
                        if (sortObj.SortStatus == "descending")
                        {
                            lstAllAssets = lstAllAssets.OrderByDescending(d => d.Barcode);
                        }
                        else
                        {
                            lstAllAssets = lstAllAssets.OrderBy(d => d.Barcode);
                        }
                    }
                    if (sortObj.Serial != "")
                    {
                        if (sortObj.SortStatus == "descending")
                        {
                            lstAllAssets = lstAllAssets.OrderByDescending(d => d.SerialNumber);
                        }
                        else
                        {
                            lstAllAssets = lstAllAssets.OrderBy(d => d.SerialNumber);
                        }
                    }


                    if (sortObj.Model != "")
                    {
                        if (sortObj.SortStatus == "descending")
                        {
                            lstAllAssets = lstAllAssets.OrderByDescending(d => d.MasterAsset.ModelNumber);
                        }
                        else
                        {
                            lstAllAssets = lstAllAssets.OrderBy(d => d.MasterAsset.ModelNumber);
                        }
                    }
                    if (sortObj.BrandName != "")
                    {
                        if (sortObj.SortStatus == "descending")
                        {
                            lstAllAssets = lstAllAssets.OrderByDescending(d => d.MasterAsset.brand.Name);
                        }
                        else
                        {
                            lstAllAssets = lstAllAssets.OrderBy(d => d.MasterAsset.brand.Name);
                        }
                    }
                    if (sortObj.BrandNameAr != "")
                    {
                        if (sortObj.SortStatus == "descending")
                        {
                            lstAllAssets = lstAllAssets.OrderByDescending(d => d.MasterAsset.brand.Name);
                        }
                        else
                        {
                            lstAllAssets = lstAllAssets.OrderBy(d => d.MasterAsset.brand.Name);
                        }
                    }
                    if (sortObj.SupplierName != "")
                    {
                        if (sortObj.SortStatus == "descending")
                        {
                            lstAllAssets = lstAllAssets.OrderByDescending(d => d.Supplier.Name);
                        }
                        else
                        {
                            lstAllAssets = lstAllAssets.OrderBy(d => d.Supplier.Name);
                        }
                    }
                    if (sortObj.SupplierNameAr != "")
                    {
                        if (sortObj.SortStatus == "descending")
                        {
                            lstAllAssets = lstAllAssets.OrderByDescending(d => d.Supplier.Name);
                        }
                        else
                        {
                            lstAllAssets = lstAllAssets.OrderBy(d => d.Supplier.Name);
                        }
                    }

                }


                else
                {

                    if (sortObj.HospitalName != "")
                    {
                        if (sortObj.SortStatus == "descending")
                        {
                            lstAllAssets = lstAllAssets.OrderByDescending(d => d.Hospital.Name);
                        }
                        else
                        {
                            lstAllAssets = lstAllAssets.OrderBy(d => d.Hospital.Name);
                        }
                    }
                    if (sortObj.HospitalNameAr != "")
                    {
                        if (sortObj.SortStatus == "descending")
                        {
                            lstAllAssets = lstAllAssets.OrderByDescending(d => d.Hospital.NameAr);
                        }
                        else
                        {
                            lstAllAssets = lstAllAssets.OrderBy(d => d.Hospital.NameAr);
                        }
                    }
                    if (sortObj.AssetName != "")
                    {
                        if (sortObj.SortStatus == "descending")
                        {
                            lstAllAssets = lstAllAssets.OrderByDescending(d => d.MasterAsset.Name);
                        }
                        else
                        {
                            lstAllAssets = lstAllAssets.OrderBy(d => d.MasterAsset.Name);
                        }
                    }
                    if (sortObj.AssetNameAr != "")
                    {
                        if (sortObj.SortStatus == "descending")
                        {
                            lstAllAssets = lstAllAssets.OrderByDescending(d => d.MasterAsset.NameAr);
                        }
                        else
                        {
                            lstAllAssets = lstAllAssets.OrderBy(d => d.MasterAsset.NameAr);
                        }
                    }
                    if (sortObj.BarCode != "")
                    {
                        if (sortObj.SortStatus == "descending")
                        {
                            lstAllAssets = lstAllAssets.OrderByDescending(d => d.Barcode);
                        }
                        else
                        {
                            lstAllAssets = lstAllAssets.OrderBy(d => d.Barcode);
                        }
                    }
                    if (sortObj.Serial != "")
                    {
                        if (sortObj.SortStatus == "descending")
                        {
                            lstAllAssets = lstAllAssets.OrderByDescending(d => d.SerialNumber);
                        }
                        else
                        {
                            lstAllAssets = lstAllAssets.OrderBy(d => d.SerialNumber);
                        }
                    }


                    if (sortObj.Model != "")
                    {
                        if (sortObj.SortStatus == "descending")
                        {
                            lstAllAssets = lstAllAssets.OrderByDescending(d => d.MasterAsset.ModelNumber);
                        }
                        else
                        {
                            lstAllAssets = lstAllAssets.OrderBy(d => d.MasterAsset.ModelNumber);
                        }
                    }
                    if (sortObj.BrandName != "")
                    {
                        if (sortObj.SortStatus == "descending")
                        {
                            lstAllAssets = lstAllAssets.OrderByDescending(d => d.MasterAsset.brand.Name);
                        }
                        else
                        {
                            lstAllAssets = lstAllAssets.OrderBy(d => d.MasterAsset.brand.Name);
                        }
                    }
                    if (sortObj.BrandNameAr != "")
                    {
                        if (sortObj.SortStatus == "descending")
                        {
                            lstAllAssets = lstAllAssets.OrderByDescending(d => d.MasterAsset.brand.Name);
                        }
                        else
                        {
                            lstAllAssets = lstAllAssets.OrderBy(d => d.MasterAsset.brand.Name);
                        }
                    }
                    if (sortObj.SupplierName != "")
                    {
                        if (sortObj.SortStatus == "descending")
                        {
                            lstAllAssets = lstAllAssets.OrderByDescending(d => d.Supplier.Name);
                        }
                        else
                        {
                            lstAllAssets = lstAllAssets.OrderBy(d => d.Supplier.Name);
                        }
                    }
                    if (sortObj.SupplierNameAr != "")
                    {
                        if (sortObj.SortStatus == "descending")
                        {
                            lstAllAssets = lstAllAssets.OrderByDescending(d => d.Supplier.Name);
                        }
                        else
                        {
                            lstAllAssets = lstAllAssets.OrderBy(d => d.Supplier.Name);
                        }
                    }
                }

                foreach (var asset in lstAllAssets)
                {
                    IndexAssetDetailVM.GetData detail = new IndexAssetDetailVM.GetData();
                    /////
                    detail.Id = asset.Id;
                    detail.DepartmentId = asset.DepartmentId != null ? asset.DepartmentId : 0;
                    detail.Code = asset.Code;

                    //   detail.EmployeeId = asset.EmployeeId;
                    detail.Price = asset.Price;
                    detail.BarCode = asset.Barcode;
                    detail.MasterImg = asset.MasterAsset.AssetImg;
                    detail.Serial = asset.SerialNumber;
                    detail.BrandName = asset.MasterAsset.brand != null ? asset.MasterAsset.brand.Name : "";
                    detail.BrandNameAr = asset.MasterAsset.brand != null ? asset.MasterAsset.brand.NameAr : "";
                    detail.Model = asset.MasterAsset.ModelNumber;
                    detail.SerialNumber = asset.SerialNumber;
                    detail.MasterAssetId = asset.MasterAssetId;
                    detail.PurchaseDate = asset.PurchaseDate;
                    detail.HospitalId = asset.Hospital.Id;
                    detail.HospitalName = asset.Hospital.Name;
                    detail.HospitalNameAr = asset.Hospital.NameAr;
                    detail.AssetName = asset.MasterAsset.Name;
                    detail.AssetNameAr = asset.MasterAsset.NameAr;
                    detail.DepartmentId = asset.Department != null ? asset.DepartmentId : 0;
                    detail.DepartmentName = asset.Department != null ? asset.Department.Name : "";
                    detail.SupplierName = asset.Supplier != null ? asset.Supplier.Name : "";
                    detail.SupplierNameAr = asset.Supplier != null ? asset.Supplier.NameAr : "";
                    detail.DepartmentNameAr = asset.Department != null ? asset.Department.NameAr : "";
                    detail.GovernorateId = asset.Hospital.GovernorateId;
                    detail.GovernorateName = asset.Hospital.Governorate.Name;
                    detail.GovernorateNameAr = asset.Hospital.Governorate.NameAr;
                    detail.CityId = asset.Hospital.CityId;
                    detail.CityName = asset.Hospital.City.Name;
                    detail.CityNameAr = asset.Hospital.City.NameAr;

                    list.Add(detail);
                }
                var requestsPerPage = list.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                mainClass.Results = requestsPerPage;
                mainClass.Count = list.Count();
                return mainClass;
            }
            return mainClass;
        }

        public IndexAssetDetailVM GetHospitalAssetsBySupplierId(int supplierId, int pageNumber, int pageSize)
        {

            IndexAssetDetailVM mainClass = new IndexAssetDetailVM();
            IQueryable<IndexAssetDetailVM.GetData> assetDetails = _context.AssetDetails
                                .Include(a => a.MasterAsset)
                                 .Include(a => a.Department)
                                .Include(a => a.MasterAsset.brand)
                                .Include(a => a.Supplier)
                                .Include(a => a.Hospital)
                                .Include(a => a.Hospital.Governorate)
                                .Include(a => a.Hospital.City)
                                .Include(a => a.Hospital.Organization)
                                .Include(a => a.Hospital.SubOrganization)
                                .Where(a => a.SupplierId == supplierId)
                                .Skip((pageNumber - 1) * pageSize).Take(pageSize)
                                .Select(item => new IndexAssetDetailVM.GetData
                                {
                                    Id = item.Id,
                                    Code = item.Code,
                                    SerialNumber = item.SerialNumber,
                                    CreatedBy = item.CreatedBy,
                                    HospitalId = item.HospitalId,
                                    AssetId = item.Id,
                                    SupplierId = item.SupplierId,
                                    DepartmentId = item.DepartmentId,
                                    Serial = item.SerialNumber,
                                    Price = item.Price,
                                    BarCode = item.Barcode,
                                    PurchaseDate = item.PurchaseDate,
                                    QrFilePath = item.QrFilePath,
                                    MasterAssetId = item.MasterAssetId,
                                    PeriorityId = item.MasterAsset.PeriorityId != null ? item.MasterAsset.PeriorityId : 0,
                                    AssetName = item.MasterAsset != null ? item.MasterAsset.Name : "",
                                    AssetNameAr = item.MasterAsset != null ? item.MasterAsset.NameAr : "",
                                    OriginId = item.MasterAsset != null ? item.MasterAsset.OriginId : 0,
                                    BrandId = item.MasterAsset != null ? item.MasterAsset.BrandId : 0,
                                    MasterImg = item.MasterAsset != null ? item.MasterAsset.AssetImg : "",
                                    BrandName = item.MasterAsset.brand != null ? item.MasterAsset.brand.Name : "",
                                    BrandNameAr = item.MasterAsset.brand != null ? item.MasterAsset.brand.NameAr : "",
                                    Model = item.MasterAsset != null ? item.MasterAsset.ModelNumber : "",
                                    DepartmentName = item.Department != null ? item.Department.Name : "",
                                    DepartmentNameAr = item.Department != null ? item.Department.NameAr : "",
                                    AssetPeriorityName = item.MasterAsset.AssetPeriority != null ? item.MasterAsset.AssetPeriority.Name : "",
                                    AssetPeriorityNameAr = item.MasterAsset.AssetPeriority != null ? item.MasterAsset.AssetPeriority.NameAr : "",
                                    HospitalName = item.Hospital != null ? item.Hospital.Name : "",
                                    HospitalNameAr = item.Hospital != null ? item.Hospital.NameAr : "",
                                    GovernorateId = item.Hospital.Governorate != null ? item.Hospital.GovernorateId : 0,
                                    CityId = item.Hospital != null ? item.Hospital.CityId : 0,
                                    OrganizationId = item.Hospital != null ? item.Hospital.OrganizationId : 0,
                                    SubOrganizationId = item.Hospital != null ? item.Hospital.SubOrganizationId : 0,
                                    GovernorateName = item.Hospital.Governorate.Name,
                                    GovernorateNameAr = item.Hospital.Governorate.NameAr,
                                    CityName = item.Hospital.City != null ? item.Hospital.City.Name : "",
                                    CityNameAr = item.Hospital.City != null ? item.Hospital.City.NameAr : "",
                                    OrgName = item.Hospital.Organization != null ? item.Hospital.Organization.Name : "",
                                    OrgNameAr = item.Hospital.Organization != null ? item.Hospital.Organization.NameAr : "",
                                    SubOrgName = item.Hospital.SubOrganization != null ? item.Hospital.SubOrganization.Name : "",
                                    SubOrgNameAr = item.Hospital.SubOrganization != null ? item.Hospital.SubOrganization.NameAr : "",
                                    SupplierName = item.Supplier != null ? item.Supplier.Name : "",
                                    SupplierNameAr = item.Supplier != null ? item.Supplier.NameAr : ""
                                })
                                .AsQueryable();

            var lstHospitalAssets = assetDetails.ToList();
            mainClass.Results = lstHospitalAssets.ToList();
            mainClass.Count = _context.AssetDetails
                .Include(a => a.MasterAsset).Include(a => a.MasterAsset.brand).Include(a => a.Supplier)
                .Include(a => a.Hospital).Include(a => a.Hospital.Governorate).Include(a => a.Hospital.City).Include(a => a.Hospital.Organization)
                .Include(a => a.Hospital.SubOrganization)
                                     .Where(a => a.SupplierId == supplierId).Count();
            return mainClass;
        }

        //public IndexAssetDetailVM SearchHospitalAssetsBySupplierId(SearchAssetDetailVM searchObj, int pageNumber, int pageSize)
        //{
        //    IndexAssetDetailVM mainClass = new IndexAssetDetailVM();
        //    if (searchObj.UserId != null)
        //    {
        //        var userObj = _context.Users.FindAsync(searchObj.UserId);
        //        ApplicationRole roleObj = new ApplicationRole();

        //        Employee empObj = new Employee();
        //        List<string> userRoleNames = new List<string>();
        //        var userObj1 = userObj.GetAwaiter().GetResult();

        //        var lstRoles = _context.ApplicationRole.Where(a => a.Id == userObj1.RoleId).ToList();
        //        if (lstRoles.Count > 0)
        //        {
        //            roleObj = lstRoles[0];

        //            var roles = (from userRole in _context.UserRoles
        //                         join role in _context.ApplicationRole on userRole.RoleId equals role.Id
        //                         where userRole.UserId == searchObj.UserId
        //                         select role);
        //            foreach (var role in roles)
        //            {
        //                userRoleNames.Add(role.Name);
        //            }
        //        }

        //        IQueryable<IndexAssetDetailVM.GetData> qryListAssets = _context.AssetDetails.Include(a => a.MasterAsset)
        //            .Include(a => a.MasterAsset.brand).Include(a => a.MasterAsset.AssetPeriority)
        //            .Include(a => a.Hospital)
        //            .Include(a => a.Hospital.Governorate)
        //            .Include(a => a.Hospital.City)
        //            .Include(a => a.Hospital.Organization)
        //            .Include(a => a.Hospital.SubOrganization)
        //              .Where(a => a.Hospital.Id == searchObj.HospitalId && a.SupplierId == searchObj.SupplierId)
        //            .Select(item => new IndexAssetDetailVM.GetData
        //            {
        //                Id = item.Id,
        //                Code = item.Code,
        //                SerialNumber = item.SerialNumber,
        //                CreatedBy = item.CreatedBy,
        //                HospitalId = item.HospitalId,
        //                SupplierId = item.SupplierId,
        //                DepartmentId = item.DepartmentId,
        //                Serial = item.SerialNumber,
        //                Price = item.Price,
        //                BarCode = item.Barcode,
        //                QrFilePath = item.QrFilePath,
        //                MasterAssetId = item.MasterAssetId,
        //                PeriorityId = item.MasterAsset.PeriorityId != null ? item.MasterAsset.PeriorityId : 0,
        //                PurchaseDate = item.PurchaseDate,
        //                AssetName = item.MasterAsset != null ? item.MasterAsset.Name : "",
        //                AssetNameAr = item.MasterAsset != null ? item.MasterAsset.NameAr : "",
        //                OriginId = item.MasterAsset != null ? item.MasterAsset.OriginId : 0,
        //                BrandId = item.MasterAsset != null ? item.MasterAsset.BrandId : 0,
        //                MasterImg = item.MasterAsset != null ? item.MasterAsset.AssetImg : "",
        //                BrandName = item.MasterAsset.brand != null ? item.MasterAsset.brand.Name : "",
        //                BrandNameAr = item.MasterAsset.brand != null ? item.MasterAsset.brand.NameAr : "",
        //                Model = item.MasterAsset != null ? item.MasterAsset.ModelNumber : "",
        //                DepartmentName = item.Department != null ? item.Department.Name : "",
        //                DepartmentNameAr = item.Department != null ? item.Department.NameAr : "",
        //                AssetPeriorityName = item.MasterAsset.AssetPeriority != null ? item.MasterAsset.AssetPeriority.Name : "",
        //                AssetPeriorityNameAr = item.MasterAsset.AssetPeriority != null ? item.MasterAsset.AssetPeriority.NameAr : "",
        //                HospitalName = item.Hospital != null ? item.Hospital.Name : "",
        //                HospitalNameAr = item.Hospital != null ? item.Hospital.NameAr : "",
        //                GovernorateId = item.Hospital != null ? item.Hospital.GovernorateId : 0,
        //                CityId = item.Hospital != null ? item.Hospital.CityId : 0,
        //                OrganizationId = item.Hospital != null ? item.Hospital.OrganizationId : 0,
        //                SubOrganizationId = item.Hospital != null ? item.Hospital.SubOrganizationId : 0,
        //                GovernorateName = item.Hospital.Governorate.Name,
        //                GovernorateNameAr = item.Hospital.Governorate.NameAr,
        //                CityName = item.Hospital.City != null ? item.Hospital.City.Name : "",
        //                CityNameAr = item.Hospital.City != null ? item.Hospital.City.NameAr : "",
        //                OrgName = item.Hospital.Organization != null ? item.Hospital.Organization.Name : "",
        //                OrgNameAr = item.Hospital.Organization != null ? item.Hospital.Organization.NameAr : "",
        //                SubOrgName = item.Hospital.SubOrganization != null ? item.Hospital.SubOrganization.Name : "",
        //                SubOrgNameAr = item.Hospital.SubOrganization != null ? item.Hospital.SubOrganization.NameAr : "",
        //                SupplierName = item.Supplier != null ? item.Supplier.Name : "",
        //                SupplierNameAr = item.Supplier != null ? item.Supplier.NameAr : ""
        //            }).AsQueryable();





        //        if (searchObj.HospitalId > 0)
        //        {
        //            qryListAssets = qryListAssets.Where(b => b.HospitalId == searchObj.HospitalId).AsQueryable();
        //        }
        //        else
        //        {
        //            qryListAssets = qryListAssets.AsQueryable();
        //        }


        //        if (searchObj.SupplierId != 0)
        //        {
        //            qryListAssets = qryListAssets.Where(a => a.SupplierId == searchObj.SupplierId).AsQueryable();
        //        }
        //        else
        //            qryListAssets = qryListAssets.AsQueryable();


        //        //if (searchObj.GovernorateId != 0)
        //        //{
        //        //    qryListAssets = qryListAssets.Where(a => a.GovernorateId == searchObj.GovernorateId).AsQueryable();
        //        //}
        //        //else
        //        //    qryListAssets = qryListAssets.AsQueryable();


        //        //if (searchObj.CityId != 0)
        //        //{
        //        //    qryListAssets = qryListAssets.Where(a => a.CityId == searchObj.CityId).AsQueryable();
        //        //}
        //        //else
        //        //    qryListAssets = qryListAssets.AsQueryable();


        //        if (searchObj.DepartmentId != 0)
        //        {
        //            qryListAssets = qryListAssets.Where(a => a.DepartmentId == searchObj.DepartmentId).AsQueryable();
        //        }
        //        else
        //            qryListAssets = qryListAssets.AsQueryable();


        //        if (searchObj.Model != "")
        //        {
        //            qryListAssets = qryListAssets.Where(a => a.Model == searchObj.Model).AsQueryable();
        //        }
        //        else
        //            qryListAssets = qryListAssets.AsQueryable();



        //        if (searchObj.OriginId != 0)
        //        {
        //            qryListAssets = qryListAssets.Where(a => a.OriginId == searchObj.OriginId).AsQueryable();
        //        }
        //        else
        //            qryListAssets = qryListAssets.AsQueryable();



        //        if (searchObj.BrandId != 0)
        //        {
        //            qryListAssets = qryListAssets.Where(a => a.BrandId == searchObj.BrandId).AsQueryable();
        //        }
        //        else
        //            qryListAssets = qryListAssets.AsQueryable();


        //        if (searchObj.PeriorityId != null)
        //        {
        //            qryListAssets = qryListAssets.Where(a => a.PeriorityId == searchObj.PeriorityId).AsQueryable();
        //        }
        //        else
        //            qryListAssets = qryListAssets.AsQueryable();

        //        if (searchObj.AssetName != "")
        //        {
        //            qryListAssets = qryListAssets.Where(b => b.AssetName.Contains(searchObj.AssetName)).AsQueryable();
        //        }
        //        if (searchObj.AssetNameAr != null)
        //        {
        //            qryListAssets = qryListAssets.Where(b => b.AssetNameAr.Contains(searchObj.AssetNameAr)).AsQueryable();
        //        }
        //        else
        //            qryListAssets = qryListAssets.AsQueryable();


        //        if (searchObj.Serial != "")
        //        {
        //            qryListAssets = qryListAssets.Where(b => b.SerialNumber.Contains(searchObj.Serial)).AsQueryable();

        //        }
        //        else
        //            qryListAssets = qryListAssets.AsQueryable();

        //        if (searchObj.Code != "")
        //        {
        //            qryListAssets = qryListAssets.Where(b => b.Code.Contains(searchObj.Code)).AsQueryable();
        //        }
        //        else
        //            qryListAssets = qryListAssets.AsQueryable();

        //        if (searchObj.BarCode != "")
        //        {
        //            qryListAssets = qryListAssets.Where(b => b.BarCode.Contains(searchObj.BarCode)).AsQueryable();
        //        }
        //        else
        //            qryListAssets = qryListAssets.AsQueryable();





        //        mainClass.Results = qryListAssets.ToList().Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
        //        mainClass.Count = qryListAssets.ToList().Count;
        //        return mainClass;

        //    }
        //    return mainClass;
        //}

        public IndexAssetDetailVM SortHospitalAssetsBySupplierId(Sort sortObj, int pageNumber, int pageSize)
        {
            IndexAssetDetailVM mainClass = new IndexAssetDetailVM();
            List<IndexAssetDetailVM.GetData> lstAssetData = new List<IndexAssetDetailVM.GetData>();
            ApplicationRole roleObj = new ApplicationRole();
            Employee empObj = new Employee();
            List<string> userRoleNames = new List<string>();
            ApplicationUser userObj = new ApplicationUser();
            if (sortObj.UserId != null)
            {

                var obj = _context.ApplicationUser.Where(a => a.Id == sortObj.UserId).ToList();
                userObj = obj[0];

                var roles = (from userRole in _context.UserRoles
                             join role in _context.ApplicationRole on userRole.RoleId equals role.Id
                             where userRole.UserId == userObj.Id
                             select role);
                foreach (var role in roles)
                {
                    userRoleNames.Add(role.Name);
                }
                var lstEmployees = _context.Employees.Where(a => a.Email == userObj.Email).ToList();
                if (lstEmployees.Count > 0)
                {
                    empObj = lstEmployees[0];
                }
            }


            IQueryable<AssetDetail> lstQueryAssets = _context.AssetDetails
                           .Include(a => a.MasterAsset)
                             .Include(a => a.Hospital)
                             .Include(a => a.Supplier).Include(a => a.MasterAsset.brand)
                             .Include(a => a.Hospital.Governorate).Include(a => a.Hospital.City)
                             .Include(a => a.Hospital.Organization).Include(a => a.Hospital.SubOrganization)
                             .Where(a => a.SupplierId == sortObj.SupplierId)
                             .AsQueryable();


            if (sortObj.HospitalId != 0)
            {
                lstQueryAssets = lstQueryAssets.Where(a => a.HospitalId == sortObj.HospitalId).AsQueryable();
            }
            else
            {
                lstQueryAssets = lstQueryAssets.AsQueryable();
            }
            if (sortObj.MasterAssetId != 0)
            {
                lstQueryAssets = lstQueryAssets.Where(a => a.MasterAssetId == sortObj.MasterAssetId).AsQueryable();
            }
            else
            {
                lstQueryAssets = lstQueryAssets.AsQueryable();
            }
            if (sortObj.BarCodeValue != "")
            {
                lstQueryAssets = lstQueryAssets.Where(a => a.Barcode == sortObj.BarCodeValue).AsQueryable();
            }
            else
            {
                lstQueryAssets = lstQueryAssets.AsQueryable();
            }
            if (sortObj.SerialValue != "")
            {
                lstQueryAssets = lstQueryAssets.Where(a => a.SerialNumber == sortObj.SerialValue).AsQueryable();
            }
            else
            {
                lstQueryAssets = lstQueryAssets.AsQueryable();
            }
            var lstAssets = lstQueryAssets.ToList();
            foreach (var item in lstAssets)
            {
                IndexAssetDetailVM.GetData Assetobj = new IndexAssetDetailVM.GetData();
                Assetobj.Id = item.Id;
                Assetobj.Code = item.Code;
                Assetobj.BarCode = item.Barcode;
                Assetobj.Model = item.MasterAsset.ModelNumber;
                Assetobj.Serial = item.SerialNumber;
                Assetobj.CreatedBy = item.CreatedBy;
                Assetobj.BrandName = item.MasterAsset.BrandId > 0 ? item.MasterAsset.brand.Name : "";
                Assetobj.BrandNameAr = item.MasterAsset.BrandId > 0 ? item.MasterAsset.brand.NameAr : "";
                Assetobj.SupplierName = item.SupplierId > 0 ? item.Supplier.Name : "";
                Assetobj.SupplierNameAr = item.SupplierId > 0 ? item.Supplier.NameAr : "";
                Assetobj.HospitalId = item.HospitalId;
                Assetobj.HospitalName = item.HospitalId > 0 ? item.Hospital.Name : "";
                Assetobj.HospitalNameAr = item.HospitalId > 0 ? item.Hospital.NameAr : "";
                Assetobj.AssetName = item.MasterAssetId > 0 ? item.MasterAsset.Name : "";
                Assetobj.AssetNameAr = item.MasterAssetId > 0 ? item.MasterAsset.NameAr : "";
                Assetobj.GovernorateName = item.HospitalId > 0 ? item.Hospital.Governorate.Name : "";
                Assetobj.GovernorateNameAr = item.HospitalId > 0 ? item.Hospital.Governorate.NameAr : "";

                Assetobj.GovernorateId = item.Hospital.GovernorateId;
                Assetobj.CityId = item.Hospital.CityId;
                Assetobj.OrganizationId = item.Hospital.OrganizationId;
                Assetobj.SubOrganizationId = item.Hospital.SubOrganizationId;

                Assetobj.MasterAssetId = item.MasterAssetId;
                Assetobj.GovernorateName = item.Hospital.Governorate.Name;
                Assetobj.GovernorateNameAr = item.Hospital.Governorate.NameAr;
                Assetobj.CityName = item.Hospital.City.Name;
                Assetobj.CityNameAr = item.Hospital.City.NameAr;
                Assetobj.OrgName = item.Hospital.Organization.Name;
                Assetobj.OrgNameAr = item.Hospital.Organization.NameAr;
                Assetobj.SubOrgName = item.Hospital.SubOrganization.Name;
                Assetobj.SubOrgNameAr = item.Hospital.SubOrganization.NameAr;


                lstAssetData.Add(Assetobj);
            }


            if (sortObj.AssetName != "")
            {
                if (sortObj.BarCodeValue != "")
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.BarCode.Contains(sortObj.BarCodeValue)).OrderByDescending(d => d.BarCode).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.BarCode.Contains(sortObj.BarCodeValue)).OrderBy(d => d.BarCode).ToList();
                    }
                }
                if (sortObj.SerialValue != "")
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.SerialNumber.Contains(sortObj.SerialValue)).OrderByDescending(d => d.SerialNumber).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.SerialNumber.Contains(sortObj.SerialValue)).OrderBy(d => d.SerialNumber).ToList();
                    }
                }
                if (sortObj.MasterAssetId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.MasterAssetId == sortObj.MasterAssetId).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.MasterAssetId == sortObj.MasterAssetId).OrderBy(d => d.Serial).ToList();
                    }
                }
                if (sortObj.GovernorateId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.GovernorateId == sortObj.GovernorateId).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.GovernorateId == sortObj.GovernorateId).OrderBy(d => d.Serial).ToList();
                    }
                }
                if (sortObj.CityId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.CityId == sortObj.CityId).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.CityId == sortObj.CityId).OrderBy(d => d.Serial).ToList();
                    }
                }
                if (sortObj.OrganizationId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.OrganizationId == sortObj.OrganizationId).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.OrganizationId == sortObj.OrganizationId).OrderBy(d => d.Serial).ToList();
                    }
                }
                if (sortObj.OriginId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.OriginId == sortObj.OriginId).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.OriginId == sortObj.OriginId).OrderBy(d => d.Serial).ToList();
                    }
                }
                if (sortObj.BrandId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.BrandId == sortObj.BrandId).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.BrandId == sortObj.BrandId).OrderBy(d => d.Serial).ToList();
                    }
                }

                if (sortObj.SubOrganizationId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.SubOrganizationId == sortObj.SubOrganizationId).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.SubOrganizationId == sortObj.SubOrganizationId).OrderBy(d => d.Serial).ToList();
                    }
                }
                if (sortObj.Model != "")
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.Model.Contains(sortObj.Model)).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.Model.Contains(sortObj.Model)).OrderBy(d => d.Serial).ToList();
                    }
                }
                else
                {
                    if (sortObj.SortStatus == "descending")
                        lstAssetData = lstAssetData.OrderByDescending(d => d.AssetName).ToList();
                    else
                        lstAssetData = lstAssetData.OrderBy(d => d.AssetName).ToList();
                }
            }
            else if (sortObj.AssetNameAr != "")
            {
                if (sortObj.BarCodeValue != "")
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.BarCode.Contains(sortObj.BarCodeValue)).OrderByDescending(d => d.BarCode).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.BarCode.Contains(sortObj.BarCodeValue)).OrderBy(d => d.BarCode).ToList();
                    }
                }
                if (sortObj.SerialValue != "")
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.SerialNumber.Contains(sortObj.SerialValue)).OrderByDescending(d => d.SerialNumber).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.SerialNumber.Contains(sortObj.SerialValue)).OrderBy(d => d.SerialNumber).ToList();
                    }
                }
                if (sortObj.MasterAssetId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.MasterAssetId == sortObj.MasterAssetId).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.MasterAssetId == sortObj.MasterAssetId).OrderBy(d => d.Serial).ToList();
                    }
                }
                if (sortObj.GovernorateId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.GovernorateId == sortObj.GovernorateId).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.GovernorateId == sortObj.GovernorateId).OrderBy(d => d.Serial).ToList();
                    }
                }
                if (sortObj.CityId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.CityId == sortObj.CityId).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.CityId == sortObj.CityId).OrderBy(d => d.Serial).ToList();
                    }
                }
                if (sortObj.OrganizationId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.OrganizationId == sortObj.OrganizationId).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.OrganizationId == sortObj.OrganizationId).OrderBy(d => d.Serial).ToList();
                    }
                }
                if (sortObj.OriginId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.OriginId == sortObj.OriginId).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.OriginId == sortObj.OriginId).OrderBy(d => d.Serial).ToList();
                    }
                }
                if (sortObj.BrandId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.BrandId == sortObj.BrandId).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.BrandId == sortObj.BrandId).OrderBy(d => d.Serial).ToList();
                    }
                }

                if (sortObj.SubOrganizationId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.SubOrganizationId == sortObj.SubOrganizationId).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.SubOrganizationId == sortObj.SubOrganizationId).OrderBy(d => d.Serial).ToList();
                    }
                }
                if (sortObj.Model != "")
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.Model.Contains(sortObj.Model)).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.Model.Contains(sortObj.Model)).OrderBy(d => d.Serial).ToList();
                    }
                }
                else
                {
                    if (sortObj.SortStatus == "descending")
                        lstAssetData = lstAssetData.OrderByDescending(d => d.AssetNameAr).ToList();
                    else
                        lstAssetData = lstAssetData.OrderBy(d => d.AssetNameAr).ToList();
                }
            }
            else if (sortObj.GovernorateName != "")
            {
                if (sortObj.BarCodeValue != "")
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.BarCode.Contains(sortObj.BarCodeValue)).OrderByDescending(d => d.BarCode).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.BarCode.Contains(sortObj.BarCodeValue)).OrderBy(d => d.BarCode).ToList();
                    }
                }
                if (sortObj.SerialValue != "")
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.SerialNumber.Contains(sortObj.SerialValue)).OrderByDescending(d => d.SerialNumber).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.SerialNumber.Contains(sortObj.SerialValue)).OrderBy(d => d.SerialNumber).ToList();
                    }
                }
                if (sortObj.MasterAssetId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.MasterAssetId == sortObj.MasterAssetId).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.MasterAssetId == sortObj.MasterAssetId).OrderBy(d => d.Serial).ToList();
                    }
                }
                if (sortObj.GovernorateId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.GovernorateId == sortObj.GovernorateId).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.GovernorateId == sortObj.GovernorateId).OrderBy(d => d.Serial).ToList();
                    }
                }
                if (sortObj.CityId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.CityId == sortObj.CityId).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.CityId == sortObj.CityId).OrderBy(d => d.Serial).ToList();
                    }
                }
                if (sortObj.OrganizationId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.OrganizationId == sortObj.OrganizationId).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.OrganizationId == sortObj.OrganizationId).OrderBy(d => d.Serial).ToList();
                    }
                }
                if (sortObj.OriginId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.OriginId == sortObj.OriginId).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.OriginId == sortObj.OriginId).OrderBy(d => d.Serial).ToList();
                    }
                }
                if (sortObj.BrandId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.BrandId == sortObj.BrandId).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.BrandId == sortObj.BrandId).OrderBy(d => d.Serial).ToList();
                    }
                }
                if (sortObj.SubOrganizationId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.SubOrganizationId == sortObj.SubOrganizationId).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.SubOrganizationId == sortObj.SubOrganizationId).OrderBy(d => d.Serial).ToList();
                    }
                }
                if (sortObj.Model != "")
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.Model.Contains(sortObj.Model)).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.Model.Contains(sortObj.Model)).OrderBy(d => d.Serial).ToList();
                    }
                }
                else
                {
                    if (sortObj.SortStatus == "descending")
                        lstAssetData = lstAssetData.OrderByDescending(d => d.GovernorateName).ToList();
                    else
                        lstAssetData = lstAssetData.OrderBy(d => d.GovernorateName).ToList();
                }
            }
            else if (sortObj.GovernorateNameAr != "")
            {
                if (sortObj.BarCodeValue != "")
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.BarCode.Contains(sortObj.BarCodeValue)).OrderByDescending(d => d.BarCode).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.BarCode.Contains(sortObj.BarCodeValue)).OrderBy(d => d.BarCode).ToList();
                    }
                }
                if (sortObj.SerialValue != "")
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.SerialNumber.Contains(sortObj.SerialValue)).OrderByDescending(d => d.SerialNumber).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.SerialNumber.Contains(sortObj.SerialValue)).OrderBy(d => d.SerialNumber).ToList();
                    }
                }
                if (sortObj.MasterAssetId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.MasterAssetId == sortObj.MasterAssetId).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.MasterAssetId == sortObj.MasterAssetId).OrderBy(d => d.Serial).ToList();
                    }
                }
                if (sortObj.GovernorateId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.GovernorateId == sortObj.GovernorateId).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.GovernorateId == sortObj.GovernorateId).OrderBy(d => d.Serial).ToList();
                    }
                }
                if (sortObj.CityId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.CityId == sortObj.CityId).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.CityId == sortObj.CityId).OrderBy(d => d.Serial).ToList();
                    }
                }
                if (sortObj.OrganizationId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.OrganizationId == sortObj.OrganizationId).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.OrganizationId == sortObj.OrganizationId).OrderBy(d => d.Serial).ToList();
                    }
                }
                if (sortObj.OriginId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.OriginId == sortObj.OriginId).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.OriginId == sortObj.OriginId).OrderBy(d => d.Serial).ToList();
                    }
                }
                if (sortObj.BrandId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.BrandId == sortObj.BrandId).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.BrandId == sortObj.BrandId).OrderBy(d => d.Serial).ToList();
                    }
                }

                if (sortObj.SubOrganizationId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.SubOrganizationId == sortObj.SubOrganizationId).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.SubOrganizationId == sortObj.SubOrganizationId).OrderBy(d => d.Serial).ToList();
                    }
                }
                if (sortObj.Model != "")
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.Model.Contains(sortObj.Model)).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.Model.Contains(sortObj.Model)).OrderBy(d => d.Serial).ToList();
                    }
                }
                else
                {
                    if (sortObj.SortStatus == "descending")
                        lstAssetData = lstAssetData.OrderByDescending(d => d.GovernorateNameAr).ToList();
                    else
                        lstAssetData = lstAssetData.OrderBy(d => d.GovernorateNameAr).ToList();
                }
            }
            else if (sortObj.HospitalName != "")
            {
                if (sortObj.BarCodeValue != "")
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.BarCode.Contains(sortObj.BarCodeValue)).OrderByDescending(d => d.BarCode).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.BarCode.Contains(sortObj.BarCodeValue)).OrderBy(d => d.BarCode).ToList();
                    }
                }
                if (sortObj.SerialValue != "")
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.SerialNumber.Contains(sortObj.SerialValue)).OrderByDescending(d => d.SerialNumber).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.SerialNumber.Contains(sortObj.SerialValue)).OrderBy(d => d.SerialNumber).ToList();
                    }
                }
                if (sortObj.MasterAssetId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.MasterAssetId == sortObj.MasterAssetId).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.MasterAssetId == sortObj.MasterAssetId).OrderBy(d => d.Serial).ToList();
                    }
                }
                if (sortObj.GovernorateId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.GovernorateId == sortObj.GovernorateId).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.GovernorateId == sortObj.GovernorateId).OrderBy(d => d.Serial).ToList();
                    }
                }
                if (sortObj.CityId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.CityId == sortObj.CityId).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.CityId == sortObj.CityId).OrderBy(d => d.Serial).ToList();
                    }
                }
                if (sortObj.OrganizationId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.OrganizationId == sortObj.OrganizationId).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.OrganizationId == sortObj.OrganizationId).OrderBy(d => d.Serial).ToList();
                    }
                }
                if (sortObj.OriginId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.OriginId == sortObj.OriginId).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.OriginId == sortObj.OriginId).OrderBy(d => d.Serial).ToList();
                    }
                }
                if (sortObj.BrandId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.BrandId == sortObj.BrandId).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.BrandId == sortObj.BrandId).OrderBy(d => d.Serial).ToList();
                    }
                }
                if (sortObj.SubOrganizationId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.SubOrganizationId == sortObj.SubOrganizationId).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.SubOrganizationId == sortObj.SubOrganizationId).OrderBy(d => d.Serial).ToList();
                    }
                }
                if (sortObj.Model != "")
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.Model.Contains(sortObj.Model)).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.Model.Contains(sortObj.Model)).OrderBy(d => d.Serial).ToList();
                    }
                }
                else
                {
                    if (sortObj.SortStatus == "descending")
                        lstAssetData = lstAssetData.OrderByDescending(d => d.HospitalName).ToList();
                    else
                        lstAssetData = lstAssetData.OrderBy(d => d.HospitalName).ToList();
                }
            }
            else if (sortObj.HospitalNameAr != "")
            {
                if (sortObj.BarCodeValue != "")
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.BarCode.Contains(sortObj.BarCodeValue)).OrderByDescending(d => d.BarCode).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.BarCode.Contains(sortObj.BarCodeValue)).OrderBy(d => d.BarCode).ToList();
                    }
                }
                if (sortObj.SerialValue != "")
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.SerialNumber.Contains(sortObj.SerialValue)).OrderByDescending(d => d.SerialNumber).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.SerialNumber.Contains(sortObj.SerialValue)).OrderBy(d => d.SerialNumber).ToList();
                    }
                }
                if (sortObj.MasterAssetId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.MasterAssetId == sortObj.MasterAssetId).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.MasterAssetId == sortObj.MasterAssetId).OrderBy(d => d.Serial).ToList();
                    }
                }
                if (sortObj.GovernorateId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.GovernorateId == sortObj.GovernorateId).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.GovernorateId == sortObj.GovernorateId).OrderBy(d => d.Serial).ToList();
                    }
                }
                if (sortObj.CityId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.CityId == sortObj.CityId).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.CityId == sortObj.CityId).OrderBy(d => d.Serial).ToList();
                    }
                }
                if (sortObj.OrganizationId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.OrganizationId == sortObj.OrganizationId).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.OrganizationId == sortObj.OrganizationId).OrderBy(d => d.Serial).ToList();
                    }
                }
                if (sortObj.OriginId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.OriginId == sortObj.OriginId).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.OriginId == sortObj.OriginId).OrderBy(d => d.Serial).ToList();
                    }
                }
                if (sortObj.BrandId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.BrandId == sortObj.BrandId).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.BrandId == sortObj.BrandId).OrderBy(d => d.Serial).ToList();
                    }
                }
                if (sortObj.SupplierId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.SupplierId == sortObj.SupplierId).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.SupplierId == sortObj.SupplierId).OrderBy(d => d.Serial).ToList();
                    }
                }
                if (sortObj.SubOrganizationId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.SubOrganizationId == sortObj.SubOrganizationId).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.SubOrganizationId == sortObj.SubOrganizationId).OrderBy(d => d.Serial).ToList();
                    }
                }
                if (sortObj.Model != "")
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.Model.Contains(sortObj.Model)).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.Model.Contains(sortObj.Model)).OrderBy(d => d.Serial).ToList();
                    }
                }
                else
                {
                    if (sortObj.SortStatus == "descending")
                        lstAssetData = lstAssetData.OrderByDescending(d => d.HospitalNameAr).ToList();
                    else
                        lstAssetData = lstAssetData.OrderBy(d => d.HospitalNameAr).ToList();
                }
            }
            else if (sortObj.GovernorateName != "")
            {
                if (sortObj.BarCodeValue != "")
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.BarCode.Contains(sortObj.BarCodeValue)).OrderByDescending(d => d.BarCode).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.BarCode.Contains(sortObj.BarCodeValue)).OrderBy(d => d.BarCode).ToList();
                    }
                }
                if (sortObj.SerialValue != "")
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.SerialNumber.Contains(sortObj.SerialValue)).OrderByDescending(d => d.SerialNumber).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.SerialNumber.Contains(sortObj.SerialValue)).OrderBy(d => d.SerialNumber).ToList();
                    }
                }
                if (sortObj.MasterAssetId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.MasterAssetId == sortObj.MasterAssetId).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.MasterAssetId == sortObj.MasterAssetId).OrderBy(d => d.Serial).ToList();
                    }
                }
                if (sortObj.GovernorateId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.GovernorateId == sortObj.GovernorateId).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.GovernorateId == sortObj.GovernorateId).OrderBy(d => d.Serial).ToList();
                    }
                }
                if (sortObj.CityId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.CityId == sortObj.CityId).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.CityId == sortObj.CityId).OrderBy(d => d.Serial).ToList();
                    }
                }
                if (sortObj.OrganizationId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.OrganizationId == sortObj.OrganizationId).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.OrganizationId == sortObj.OrganizationId).OrderBy(d => d.Serial).ToList();
                    }
                }
                if (sortObj.OriginId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.OriginId == sortObj.OriginId).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.OriginId == sortObj.OriginId).OrderBy(d => d.Serial).ToList();
                    }
                }
                if (sortObj.BrandId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.BrandId == sortObj.BrandId).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.BrandId == sortObj.BrandId).OrderBy(d => d.Serial).ToList();
                    }
                }
                if (sortObj.SupplierId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.SupplierId == sortObj.SupplierId).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.SupplierId == sortObj.SupplierId).OrderBy(d => d.Serial).ToList();
                    }
                }
                if (sortObj.SubOrganizationId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.SubOrganizationId == sortObj.SubOrganizationId).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.SubOrganizationId == sortObj.SubOrganizationId).OrderBy(d => d.Serial).ToList();
                    }
                }
                if (sortObj.Model != "")
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.Model.Contains(sortObj.Model)).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.Model.Contains(sortObj.Model)).OrderBy(d => d.Serial).ToList();
                    }
                }
                else
                {
                    if (sortObj.SortStatus == "descending")
                        lstAssetData = lstAssetData.OrderByDescending(d => d.GovernorateName).ToList();
                    else
                        lstAssetData = lstAssetData.OrderBy(d => d.GovernorateName).ToList();
                }
            }
            else if (sortObj.GovernorateNameAr != "")
            {
                if (sortObj.BarCodeValue != "")
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.BarCode.Contains(sortObj.BarCodeValue)).OrderByDescending(d => d.BarCode).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.BarCode.Contains(sortObj.BarCodeValue)).OrderBy(d => d.BarCode).ToList();
                    }
                }
                if (sortObj.SerialValue != "")
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.SerialNumber.Contains(sortObj.SerialValue)).OrderByDescending(d => d.SerialNumber).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.SerialNumber.Contains(sortObj.SerialValue)).OrderBy(d => d.SerialNumber).ToList();
                    }
                }
                if (sortObj.MasterAssetId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.MasterAssetId == sortObj.MasterAssetId).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.MasterAssetId == sortObj.MasterAssetId).OrderBy(d => d.Serial).ToList();
                    }
                }
                if (sortObj.GovernorateId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.GovernorateId == sortObj.GovernorateId).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.GovernorateId == sortObj.GovernorateId).OrderBy(d => d.Serial).ToList();
                    }
                }
                if (sortObj.CityId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.CityId == sortObj.CityId).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.CityId == sortObj.CityId).OrderBy(d => d.Serial).ToList();
                    }
                }
                if (sortObj.OrganizationId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.OrganizationId == sortObj.OrganizationId).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.OrganizationId == sortObj.OrganizationId).OrderBy(d => d.Serial).ToList();
                    }
                }
                if (sortObj.OriginId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.OriginId == sortObj.OriginId).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.OriginId == sortObj.OriginId).OrderBy(d => d.Serial).ToList();
                    }
                }
                if (sortObj.BrandId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.BrandId == sortObj.BrandId).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.BrandId == sortObj.BrandId).OrderBy(d => d.Serial).ToList();
                    }
                }
                if (sortObj.SupplierId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.SupplierId == sortObj.SupplierId).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.SupplierId == sortObj.SupplierId).OrderBy(d => d.Serial).ToList();
                    }
                }
                if (sortObj.SubOrganizationId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.SubOrganizationId == sortObj.SubOrganizationId).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.SubOrganizationId == sortObj.SubOrganizationId).OrderBy(d => d.Serial).ToList();
                    }
                }
                if (sortObj.Model != "")
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.Model.Contains(sortObj.Model)).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.Model.Contains(sortObj.Model)).OrderBy(d => d.Serial).ToList();
                    }
                }
                else
                {
                    if (sortObj.SortStatus == "descending")
                        lstAssetData = lstAssetData.OrderByDescending(d => d.GovernorateNameAr).ToList();
                    else
                        lstAssetData = lstAssetData.OrderBy(d => d.GovernorateNameAr).ToList();
                }
            }
            else if (sortObj.OrgName != "")
            {
                if (sortObj.BarCodeValue != "")
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.BarCode.Contains(sortObj.BarCodeValue)).OrderByDescending(d => d.BarCode).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.BarCode.Contains(sortObj.BarCodeValue)).OrderBy(d => d.BarCode).ToList();
                    }
                }
                if (sortObj.SerialValue != "")
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.SerialNumber.Contains(sortObj.SerialValue)).OrderByDescending(d => d.SerialNumber).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.SerialNumber.Contains(sortObj.SerialValue)).OrderBy(d => d.SerialNumber).ToList();
                    }
                }
                if (sortObj.MasterAssetId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.MasterAssetId == sortObj.MasterAssetId).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.MasterAssetId == sortObj.MasterAssetId).OrderBy(d => d.Serial).ToList();
                    }
                }
                if (sortObj.GovernorateId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.GovernorateId == sortObj.GovernorateId).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.GovernorateId == sortObj.GovernorateId).OrderBy(d => d.Serial).ToList();
                    }
                }
                if (sortObj.CityId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.CityId == sortObj.CityId).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.CityId == sortObj.CityId).OrderBy(d => d.Serial).ToList();
                    }
                }
                if (sortObj.OrganizationId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.OrganizationId == sortObj.OrganizationId).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.OrganizationId == sortObj.OrganizationId).OrderBy(d => d.Serial).ToList();
                    }
                }
                if (sortObj.OriginId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.OriginId == sortObj.OriginId).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.OriginId == sortObj.OriginId).OrderBy(d => d.Serial).ToList();
                    }
                }
                if (sortObj.BrandId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.BrandId == sortObj.BrandId).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.BrandId == sortObj.BrandId).OrderBy(d => d.Serial).ToList();
                    }
                }
                if (sortObj.SupplierId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.SupplierId == sortObj.SupplierId).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.SupplierId == sortObj.SupplierId).OrderBy(d => d.Serial).ToList();
                    }
                }
                if (sortObj.SubOrganizationId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.SubOrganizationId == sortObj.SubOrganizationId).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.SubOrganizationId == sortObj.SubOrganizationId).OrderBy(d => d.Serial).ToList();
                    }
                }
                if (sortObj.Model != "")
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.Model.Contains(sortObj.Model)).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.Model.Contains(sortObj.Model)).OrderBy(d => d.Serial).ToList();
                    }
                }
                else
                {
                    if (sortObj.SortStatus == "descending")
                        lstAssetData = lstAssetData.OrderByDescending(d => d.OrgName).ToList();
                    else
                        lstAssetData = lstAssetData.OrderBy(d => d.OrgName).ToList();
                }
            }
            else if (sortObj.OrgNameAr != "")
            {
                if (sortObj.BarCodeValue != "")
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.BarCode.Contains(sortObj.BarCodeValue)).OrderByDescending(d => d.BarCode).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.BarCode.Contains(sortObj.BarCodeValue)).OrderBy(d => d.BarCode).ToList();
                    }
                }
                if (sortObj.SerialValue != "")
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.SerialNumber.Contains(sortObj.SerialValue)).OrderByDescending(d => d.SerialNumber).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.SerialNumber.Contains(sortObj.SerialValue)).OrderBy(d => d.SerialNumber).ToList();
                    }
                }
                if (sortObj.MasterAssetId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.MasterAssetId == sortObj.MasterAssetId).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.MasterAssetId == sortObj.MasterAssetId).OrderBy(d => d.Serial).ToList();
                    }
                }
                if (sortObj.GovernorateId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.GovernorateId == sortObj.GovernorateId).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.GovernorateId == sortObj.GovernorateId).OrderBy(d => d.Serial).ToList();
                    }
                }
                if (sortObj.CityId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.CityId == sortObj.CityId).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.CityId == sortObj.CityId).OrderBy(d => d.Serial).ToList();
                    }
                }
                if (sortObj.OrganizationId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.OrganizationId == sortObj.OrganizationId).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.OrganizationId == sortObj.OrganizationId).OrderBy(d => d.Serial).ToList();
                    }
                }
                if (sortObj.OriginId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.OriginId == sortObj.OriginId).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.OriginId == sortObj.OriginId).OrderBy(d => d.Serial).ToList();
                    }
                }
                if (sortObj.BrandId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.BrandId == sortObj.BrandId).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.BrandId == sortObj.BrandId).OrderBy(d => d.Serial).ToList();
                    }
                }
                if (sortObj.SupplierId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.SupplierId == sortObj.SupplierId).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.SupplierId == sortObj.SupplierId).OrderBy(d => d.Serial).ToList();
                    }
                }
                if (sortObj.SubOrganizationId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.SubOrganizationId == sortObj.SubOrganizationId).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.SubOrganizationId == sortObj.SubOrganizationId).OrderBy(d => d.Serial).ToList();
                    }
                }
                if (sortObj.Model != "")
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.Model.Contains(sortObj.Model)).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.Model.Contains(sortObj.Model)).OrderBy(d => d.Serial).ToList();
                    }
                }
                else
                {
                    if (sortObj.SortStatus == "descending")
                        lstAssetData = lstAssetData.OrderByDescending(d => d.OrgNameAr).ToList();
                    else
                        lstAssetData = lstAssetData.OrderBy(d => d.OrgNameAr).ToList();
                }
            }
            else if (sortObj.SubOrgName != "")
            {
                if (sortObj.BarCodeValue != "")
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.BarCode.Contains(sortObj.BarCodeValue)).OrderByDescending(d => d.BarCode).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.BarCode.Contains(sortObj.BarCodeValue)).OrderBy(d => d.BarCode).ToList();
                    }
                }
                if (sortObj.SerialValue != "")
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.SerialNumber.Contains(sortObj.SerialValue)).OrderByDescending(d => d.SerialNumber).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.SerialNumber.Contains(sortObj.SerialValue)).OrderBy(d => d.SerialNumber).ToList();
                    }
                }
                if (sortObj.MasterAssetId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.MasterAssetId == sortObj.MasterAssetId).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.MasterAssetId == sortObj.MasterAssetId).OrderBy(d => d.Serial).ToList();
                    }
                }
                if (sortObj.GovernorateId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.GovernorateId == sortObj.GovernorateId).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.GovernorateId == sortObj.GovernorateId).OrderBy(d => d.Serial).ToList();
                    }
                }
                if (sortObj.CityId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.CityId == sortObj.CityId).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.CityId == sortObj.CityId).OrderBy(d => d.Serial).ToList();
                    }
                }
                if (sortObj.OrganizationId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.OrganizationId == sortObj.OrganizationId).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.OrganizationId == sortObj.OrganizationId).OrderBy(d => d.Serial).ToList();
                    }
                }
                if (sortObj.OriginId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.OriginId == sortObj.OriginId).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.OriginId == sortObj.OriginId).OrderBy(d => d.Serial).ToList();
                    }
                }
                if (sortObj.BrandId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.BrandId == sortObj.BrandId).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.BrandId == sortObj.BrandId).OrderBy(d => d.Serial).ToList();
                    }
                }
                if (sortObj.SupplierId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.SupplierId == sortObj.SupplierId).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.SupplierId == sortObj.SupplierId).OrderBy(d => d.Serial).ToList();
                    }
                }
                if (sortObj.SubOrganizationId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.SubOrganizationId == sortObj.SubOrganizationId).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.SubOrganizationId == sortObj.SubOrganizationId).OrderBy(d => d.Serial).ToList();
                    }
                }
                if (sortObj.Model != "")
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.Model.Contains(sortObj.Model)).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.Model.Contains(sortObj.Model)).OrderBy(d => d.Serial).ToList();
                    }
                }
                else
                {
                    if (sortObj.SortStatus == "descending")
                        lstAssetData = lstAssetData.OrderByDescending(d => d.SubOrgName).ToList();
                    else
                        lstAssetData = lstAssetData.OrderBy(d => d.SubOrgName).ToList();
                }
            }
            else if (sortObj.SubOrgNameAr != "")
            {

                if (sortObj.BarCodeValue != "")
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.BarCode.Contains(sortObj.BarCodeValue)).OrderByDescending(d => d.BarCode).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.BarCode.Contains(sortObj.BarCodeValue)).OrderBy(d => d.BarCode).ToList();
                    }
                }
                if (sortObj.SerialValue != "")
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.SerialNumber.Contains(sortObj.SerialValue)).OrderByDescending(d => d.SerialNumber).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.SerialNumber.Contains(sortObj.SerialValue)).OrderBy(d => d.SerialNumber).ToList();
                    }
                }
                if (sortObj.MasterAssetId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.MasterAssetId == sortObj.MasterAssetId).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.MasterAssetId == sortObj.MasterAssetId).OrderBy(d => d.Serial).ToList();
                    }
                }
                if (sortObj.GovernorateId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.GovernorateId == sortObj.GovernorateId).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.GovernorateId == sortObj.GovernorateId).OrderBy(d => d.Serial).ToList();
                    }
                }
                if (sortObj.CityId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.CityId == sortObj.CityId).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.CityId == sortObj.CityId).OrderBy(d => d.Serial).ToList();
                    }
                }
                if (sortObj.OrganizationId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.OrganizationId == sortObj.OrganizationId).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.OrganizationId == sortObj.OrganizationId).OrderBy(d => d.Serial).ToList();
                    }
                }
                if (sortObj.OriginId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.OriginId == sortObj.OriginId).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.OriginId == sortObj.OriginId).OrderBy(d => d.Serial).ToList();
                    }
                }
                if (sortObj.BrandId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.BrandId == sortObj.BrandId).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.BrandId == sortObj.BrandId).OrderBy(d => d.Serial).ToList();
                    }
                }
                if (sortObj.SupplierId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.SupplierId == sortObj.SupplierId).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.SupplierId == sortObj.SupplierId).OrderBy(d => d.Serial).ToList();
                    }
                }
                if (sortObj.SubOrganizationId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.SubOrganizationId == sortObj.SubOrganizationId).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.SubOrganizationId == sortObj.SubOrganizationId).OrderBy(d => d.Serial).ToList();
                    }
                }
                if (sortObj.Model != "")
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.Model.Contains(sortObj.Model)).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.Model.Contains(sortObj.Model)).OrderBy(d => d.Serial).ToList();
                    }
                }
                else
                {
                    if (sortObj.SortStatus == "descending")
                        lstAssetData = lstAssetData.OrderByDescending(d => d.SubOrgNameAr).ToList();
                    else
                        lstAssetData = lstAssetData.OrderBy(d => d.SubOrgNameAr).ToList();
                }
            }
            else if (sortObj.BarCode != "")
            {
                if (sortObj.BarCodeValue != "")
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.BarCode.Contains(sortObj.BarCodeValue)).OrderByDescending(d => d.BarCode).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.BarCode.Contains(sortObj.BarCodeValue)).OrderBy(d => d.BarCode).ToList();
                    }
                }
                if (sortObj.SerialValue != "")
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.SerialNumber.Contains(sortObj.SerialValue)).OrderByDescending(d => d.BarCode).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.SerialNumber.Contains(sortObj.SerialValue)).OrderBy(d => d.BarCode).ToList();
                    }
                }
                if (sortObj.MasterAssetId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.MasterAssetId == sortObj.MasterAssetId).OrderByDescending(d => d.BarCode).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.MasterAssetId == sortObj.MasterAssetId).OrderBy(d => d.BarCode).ToList();
                    }
                }
                if (sortObj.GovernorateId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.GovernorateId == sortObj.GovernorateId).OrderByDescending(d => d.BarCode).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.GovernorateId == sortObj.GovernorateId).OrderBy(d => d.BarCode).ToList();
                    }
                }
                if (sortObj.CityId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.CityId == sortObj.CityId).OrderByDescending(d => d.BarCode).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.CityId == sortObj.CityId).OrderBy(d => d.BarCode).ToList();
                    }
                }
                if (sortObj.OrganizationId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.OrganizationId == sortObj.OrganizationId).OrderByDescending(d => d.BarCode).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.OrganizationId == sortObj.OrganizationId).OrderBy(d => d.BarCode).ToList();
                    }
                }
                if (sortObj.OriginId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.OriginId == sortObj.OriginId).OrderByDescending(d => d.BarCode).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.OriginId == sortObj.OriginId).OrderBy(d => d.BarCode).ToList();
                    }
                }
                if (sortObj.BrandId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.BrandId == sortObj.BrandId).OrderByDescending(d => d.BarCode).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.BrandId == sortObj.BrandId).OrderBy(d => d.BarCode).ToList();
                    }
                }
                if (sortObj.SubOrganizationId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.SubOrganizationId == sortObj.SubOrganizationId).OrderByDescending(d => d.BarCode).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.SubOrganizationId == sortObj.SubOrganizationId).OrderBy(d => d.BarCode).ToList();
                    }
                }
                if (sortObj.Model != "")
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.Model.Contains(sortObj.Model)).OrderByDescending(d => d.BarCode).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.Model.Contains(sortObj.Model)).OrderBy(d => d.BarCode).ToList();
                    }
                }
                else
                {
                    if (sortObj.SortStatus == "descending")
                        lstAssetData = lstAssetData.OrderByDescending(d => d.BarCode).ToList();
                    else
                        lstAssetData = lstAssetData.OrderBy(d => d.BarCode).ToList();
                }
            }
            else if (sortObj.Serial != "")
            {
                if (sortObj.BarCodeValue != "")
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.BarCode.Contains(sortObj.BarCodeValue)).OrderByDescending(d => d.BarCode).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.BarCode.Contains(sortObj.BarCodeValue)).OrderBy(d => d.BarCode).ToList();
                    }
                }
                if (sortObj.SerialValue != "")
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.SerialNumber.Contains(sortObj.SerialValue)).OrderByDescending(d => d.SerialNumber).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.SerialNumber.Contains(sortObj.SerialValue)).OrderBy(d => d.SerialNumber).ToList();
                    }
                }
                if (sortObj.MasterAssetId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.MasterAssetId == sortObj.MasterAssetId).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.MasterAssetId == sortObj.MasterAssetId).OrderBy(d => d.Serial).ToList();
                    }
                }
                if (sortObj.GovernorateId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.GovernorateId == sortObj.GovernorateId).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.GovernorateId == sortObj.GovernorateId).OrderBy(d => d.Serial).ToList();
                    }
                }
                if (sortObj.CityId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.CityId == sortObj.CityId).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.CityId == sortObj.CityId).OrderBy(d => d.Serial).ToList();
                    }
                }
                if (sortObj.OrganizationId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.OrganizationId == sortObj.OrganizationId).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.OrganizationId == sortObj.OrganizationId).OrderBy(d => d.Serial).ToList();
                    }
                }
                if (sortObj.OriginId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.OriginId == sortObj.OriginId).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.OriginId == sortObj.OriginId).OrderBy(d => d.Serial).ToList();
                    }
                }
                if (sortObj.BrandId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.BrandId == sortObj.BrandId).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.BrandId == sortObj.BrandId).OrderBy(d => d.Serial).ToList();
                    }
                }
                if (sortObj.SubOrganizationId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.SubOrganizationId == sortObj.SubOrganizationId).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.SubOrganizationId == sortObj.SubOrganizationId).OrderBy(d => d.Serial).ToList();
                    }
                }
                if (sortObj.Model != "")
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.Model.Contains(sortObj.Model)).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.Model.Contains(sortObj.Model)).OrderBy(d => d.Serial).ToList();
                    }
                }

                else
                {
                    if (sortObj.SortStatus == "descending")
                        lstAssetData = lstAssetData.OrderByDescending(d => d.Serial).ToList();
                    else
                        lstAssetData = lstAssetData.OrderBy(d => d.Serial).ToList();
                }
            }
            else if (sortObj.Model != "" && sortObj.Model != null)
            {
                if (sortObj.BarCodeValue != "")
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.BarCode.Contains(sortObj.BarCodeValue)).OrderByDescending(d => d.BarCode).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.BarCode.Contains(sortObj.BarCodeValue)).OrderBy(d => d.BarCode).ToList();
                    }
                }
                if (sortObj.SerialValue != "")
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.SerialNumber.Contains(sortObj.SerialValue)).OrderByDescending(d => d.SerialNumber).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.SerialNumber.Contains(sortObj.SerialValue)).OrderBy(d => d.SerialNumber).ToList();
                    }
                }
                if (sortObj.MasterAssetId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.MasterAssetId == sortObj.MasterAssetId).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.MasterAssetId == sortObj.MasterAssetId).OrderBy(d => d.Serial).ToList();
                    }
                }
                if (sortObj.GovernorateId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.GovernorateId == sortObj.GovernorateId).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.GovernorateId == sortObj.GovernorateId).OrderBy(d => d.Serial).ToList();
                    }
                }
                if (sortObj.CityId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.CityId == sortObj.CityId).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.CityId == sortObj.CityId).OrderBy(d => d.Serial).ToList();
                    }
                }
                if (sortObj.OrganizationId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.OrganizationId == sortObj.OrganizationId).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.OrganizationId == sortObj.OrganizationId).OrderBy(d => d.Serial).ToList();
                    }
                }
                if (sortObj.OriginId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.OriginId == sortObj.OriginId).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.OriginId == sortObj.OriginId).OrderBy(d => d.Serial).ToList();
                    }
                }
                if (sortObj.BrandId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.BrandId == sortObj.BrandId).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.BrandId == sortObj.BrandId).OrderBy(d => d.Serial).ToList();
                    }
                }
                if (sortObj.SubOrganizationId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.SubOrganizationId == sortObj.SubOrganizationId).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.SubOrganizationId == sortObj.SubOrganizationId).OrderBy(d => d.Serial).ToList();
                    }
                }
                if (sortObj.Model != "")
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.Model.Contains(sortObj.Model)).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.Model.Contains(sortObj.Model)).OrderBy(d => d.Serial).ToList();
                    }
                }
                else
                {
                    if (sortObj.SortStatus == "descending")
                        lstAssetData = lstAssetData.OrderByDescending(d => d.Model).ToList();
                    else
                        lstAssetData = lstAssetData.OrderBy(d => d.Model).ToList();
                }
            }
            else if (sortObj.BrandName != "" && sortObj.BrandName != null)
            {
                if (sortObj.BarCodeValue != "")
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.BarCode.Contains(sortObj.BarCodeValue)).OrderByDescending(d => d.BarCode).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.BarCode.Contains(sortObj.BarCodeValue)).OrderBy(d => d.BarCode).ToList();
                    }
                }
                if (sortObj.SerialValue != "")
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.SerialNumber.Contains(sortObj.SerialValue)).OrderByDescending(d => d.SerialNumber).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.SerialNumber.Contains(sortObj.SerialValue)).OrderBy(d => d.SerialNumber).ToList();
                    }
                }
                if (sortObj.MasterAssetId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.MasterAssetId == sortObj.MasterAssetId).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.MasterAssetId == sortObj.MasterAssetId).OrderBy(d => d.Serial).ToList();
                    }
                }
                if (sortObj.GovernorateId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.GovernorateId == sortObj.GovernorateId).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.GovernorateId == sortObj.GovernorateId).OrderBy(d => d.Serial).ToList();
                    }
                }
                if (sortObj.CityId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.CityId == sortObj.CityId).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.CityId == sortObj.CityId).OrderBy(d => d.Serial).ToList();
                    }
                }
                if (sortObj.OrganizationId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.OrganizationId == sortObj.OrganizationId).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.OrganizationId == sortObj.OrganizationId).OrderBy(d => d.Serial).ToList();
                    }
                }
                if (sortObj.OriginId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.OriginId == sortObj.OriginId).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.OriginId == sortObj.OriginId).OrderBy(d => d.Serial).ToList();
                    }
                }
                if (sortObj.BrandId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.BrandId == sortObj.BrandId).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.BrandId == sortObj.BrandId).OrderBy(d => d.Serial).ToList();
                    }
                }

                if (sortObj.SubOrganizationId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.SubOrganizationId == sortObj.SubOrganizationId).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.SubOrganizationId == sortObj.SubOrganizationId).OrderBy(d => d.Serial).ToList();
                    }
                }
                if (sortObj.Model != "")
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.Model.Contains(sortObj.Model)).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.Model.Contains(sortObj.Model)).OrderBy(d => d.Serial).ToList();
                    }
                }
                else
                {
                    if (sortObj.SortStatus == "descending")
                        lstAssetData = lstAssetData.OrderByDescending(d => d.BrandName).ToList();
                    else
                        lstAssetData = lstAssetData.OrderBy(d => d.BrandName).ToList();
                }
            }
            else if (sortObj.BrandNameAr != "" && sortObj.BrandNameAr != null)
            {
                if (sortObj.BarCodeValue != "")
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.BarCode.Contains(sortObj.BarCodeValue)).OrderByDescending(d => d.BarCode).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.BarCode.Contains(sortObj.BarCodeValue)).OrderBy(d => d.BarCode).ToList();
                    }
                }
                if (sortObj.SerialValue != "")
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.SerialNumber.Contains(sortObj.SerialValue)).OrderByDescending(d => d.SerialNumber).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.SerialNumber.Contains(sortObj.SerialValue)).OrderBy(d => d.SerialNumber).ToList();
                    }
                }
                if (sortObj.MasterAssetId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.MasterAssetId == sortObj.MasterAssetId).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.MasterAssetId == sortObj.MasterAssetId).OrderBy(d => d.Serial).ToList();
                    }
                }
                if (sortObj.GovernorateId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.GovernorateId == sortObj.GovernorateId).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.GovernorateId == sortObj.GovernorateId).OrderBy(d => d.Serial).ToList();
                    }
                }
                if (sortObj.CityId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.CityId == sortObj.CityId).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.CityId == sortObj.CityId).OrderBy(d => d.Serial).ToList();
                    }
                }
                if (sortObj.OrganizationId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.OrganizationId == sortObj.OrganizationId).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.OrganizationId == sortObj.OrganizationId).OrderBy(d => d.Serial).ToList();
                    }
                }
                if (sortObj.OriginId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.OriginId == sortObj.OriginId).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.OriginId == sortObj.OriginId).OrderBy(d => d.Serial).ToList();
                    }
                }
                if (sortObj.BrandId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.BrandId == sortObj.BrandId).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.BrandId == sortObj.BrandId).OrderBy(d => d.Serial).ToList();
                    }
                }

                if (sortObj.SubOrganizationId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.SubOrganizationId == sortObj.SubOrganizationId).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.SubOrganizationId == sortObj.SubOrganizationId).OrderBy(d => d.Serial).ToList();
                    }
                }
                if (sortObj.Model != "")
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.Model.Contains(sortObj.Model)).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.Model.Contains(sortObj.Model)).OrderBy(d => d.Serial).ToList();
                    }
                }
                else
                {
                    if (sortObj.SortStatus == "descending")
                        lstAssetData = lstAssetData.OrderByDescending(d => d.BrandNameAr).ToList();
                    else
                        lstAssetData = lstAssetData.OrderBy(d => d.BrandNameAr).ToList();
                }
            }
            else if (sortObj.SupplierName != "" && sortObj.SupplierName != null)
            {
                if (sortObj.BarCodeValue != "")
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.BarCode.Contains(sortObj.BarCodeValue)).OrderByDescending(d => d.BarCode).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.BarCode.Contains(sortObj.BarCodeValue)).OrderBy(d => d.BarCode).ToList();
                    }
                }
                if (sortObj.SerialValue != "")
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.SerialNumber.Contains(sortObj.SerialValue)).OrderByDescending(d => d.SerialNumber).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.SerialNumber.Contains(sortObj.SerialValue)).OrderBy(d => d.SerialNumber).ToList();
                    }
                }
                if (sortObj.MasterAssetId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.MasterAssetId == sortObj.MasterAssetId).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.MasterAssetId == sortObj.MasterAssetId).OrderBy(d => d.Serial).ToList();
                    }
                }
                if (sortObj.GovernorateId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.GovernorateId == sortObj.GovernorateId).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.GovernorateId == sortObj.GovernorateId).OrderBy(d => d.Serial).ToList();
                    }
                }
                if (sortObj.CityId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.CityId == sortObj.CityId).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.CityId == sortObj.CityId).OrderBy(d => d.Serial).ToList();
                    }
                }
                if (sortObj.OrganizationId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.OrganizationId == sortObj.OrganizationId).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.OrganizationId == sortObj.OrganizationId).OrderBy(d => d.Serial).ToList();
                    }
                }
                if (sortObj.OriginId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.OriginId == sortObj.OriginId).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.OriginId == sortObj.OriginId).OrderBy(d => d.Serial).ToList();
                    }
                }
                if (sortObj.BrandId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.BrandId == sortObj.BrandId).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.BrandId == sortObj.BrandId).OrderBy(d => d.Serial).ToList();
                    }
                }
                if (sortObj.SupplierId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.SupplierId == sortObj.SupplierId).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.SupplierId == sortObj.SupplierId).OrderBy(d => d.Serial).ToList();
                    }
                }
                if (sortObj.SubOrganizationId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.SubOrganizationId == sortObj.SubOrganizationId).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.SubOrganizationId == sortObj.SubOrganizationId).OrderBy(d => d.Serial).ToList();
                    }
                }
                if (sortObj.Model != "")
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.Model.Contains(sortObj.Model)).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.Model.Contains(sortObj.Model)).OrderBy(d => d.Serial).ToList();
                    }
                }
                else
                {
                    if (sortObj.SortStatus == "descending")
                        lstAssetData = lstAssetData.OrderByDescending(d => d.SupplierName).ToList();
                    else
                        lstAssetData = lstAssetData.OrderBy(d => d.SupplierName).ToList();
                }
            }
            else if (sortObj.SupplierNameAr != "" && sortObj.SupplierNameAr != null)
            {
                if (sortObj.BarCodeValue != "")
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.BarCode.Contains(sortObj.BarCodeValue)).OrderByDescending(d => d.BarCode).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.BarCode.Contains(sortObj.BarCodeValue)).OrderBy(d => d.BarCode).ToList();
                    }
                }
                if (sortObj.SerialValue != "")
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.SerialNumber.Contains(sortObj.SerialValue)).OrderByDescending(d => d.SerialNumber).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.SerialNumber.Contains(sortObj.SerialValue)).OrderBy(d => d.SerialNumber).ToList();
                    }
                }
                if (sortObj.MasterAssetId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.MasterAssetId == sortObj.MasterAssetId).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.MasterAssetId == sortObj.MasterAssetId).OrderBy(d => d.Serial).ToList();
                    }
                }
                if (sortObj.GovernorateId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.GovernorateId == sortObj.GovernorateId).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.GovernorateId == sortObj.GovernorateId).OrderBy(d => d.Serial).ToList();
                    }
                }
                if (sortObj.CityId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.CityId == sortObj.CityId).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.CityId == sortObj.CityId).OrderBy(d => d.Serial).ToList();
                    }
                }
                if (sortObj.OrganizationId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.OrganizationId == sortObj.OrganizationId).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.OrganizationId == sortObj.OrganizationId).OrderBy(d => d.Serial).ToList();
                    }
                }
                if (sortObj.OriginId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.OriginId == sortObj.OriginId).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.OriginId == sortObj.OriginId).OrderBy(d => d.Serial).ToList();
                    }
                }
                if (sortObj.BrandId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.BrandId == sortObj.BrandId).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.BrandId == sortObj.BrandId).OrderBy(d => d.Serial).ToList();
                    }
                }
                if (sortObj.SupplierId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.SupplierId == sortObj.SupplierId).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.SupplierId == sortObj.SupplierId).OrderBy(d => d.Serial).ToList();
                    }
                }
                if (sortObj.SubOrganizationId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.SubOrganizationId == sortObj.SubOrganizationId).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.SubOrganizationId == sortObj.SubOrganizationId).OrderBy(d => d.Serial).ToList();
                    }
                }
                if (sortObj.Model != "")
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        lstAssetData = lstAssetData.Where(b => b.Model.Contains(sortObj.Model)).OrderByDescending(d => d.Serial).ToList();
                    }
                    else
                    {
                        lstAssetData = lstAssetData.Where(b => b.Model.Contains(sortObj.Model)).OrderBy(d => d.Serial).ToList();
                    }
                }
                else
                {
                    if (sortObj.SortStatus == "descending")
                        lstAssetData = lstAssetData.OrderByDescending(d => d.SupplierNameAr).ToList();
                    else
                        lstAssetData = lstAssetData.OrderBy(d => d.SupplierNameAr).ToList();
                }
            }

            var assetsPerPage = lstAssetData.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            mainClass.Results = assetsPerPage;
            mainClass.Count = lstAssetData.Count;

            return mainClass;


        }


        public IndexAssetDetailVM GetAssetsByBrandId(int brandId)
        {


            #region old code

            //var assets = _context.AssetDetails.Include(a => a.MasterAsset).Include(a => a.MasterAsset.brand).Include(a => a.Hospital)
            //    .Include(a => a.Department).Include(a => a.Supplier).Include(a => a.Hospital.Governorate).Include(a => a.Hospital.City)
            //    .Include(a => a.Hospital.Organization).Include(a => a.Hospital.SubOrganization).Where(a => a.MasterAsset.brand.Id == brandId).ToList();
            //List<IndexAssetDetailVM.GetData> result = new List<IndexAssetDetailVM.GetData>();
            //foreach (var item in assets)
            //{
            //    IndexAssetDetailVM.GetData assetObject = new IndexAssetDetailVM.GetData();
            //    assetObject.Id = item.Id;
            //    assetObject.Code = item.Code;
            //    assetObject.Price = item.Price;
            //    assetObject.Barcode = item.Barcode;
            //    assetObject.Serial = item.SerialNumber;
            //    assetObject.SerialNumber = item.SerialNumber;
            //    assetObject.BarCode = item.Barcode;
            //    assetObject.Model = item.MasterAsset.ModelNumber;
            //    assetObject.MasterAssetId = item.MasterAssetId;
            //    assetObject.PurchaseDate = item.PurchaseDate;
            //    assetObject.HospitalId = item.HospitalId;
            //    assetObject.DepartmentId = item.DepartmentId;
            //    assetObject.HospitalName = item.Hospital.Name;
            //    assetObject.HospitalNameAr = item.Hospital.NameAr;
            //    assetObject.AssetName = item.MasterAsset.Name;
            //    assetObject.AssetNameAr = item.MasterAsset.NameAr;
            //    assetObject.GovernorateId = item.Hospital.Governorate.Id;
            //    assetObject.GovernorateName = item.Hospital.Governorate.Name;
            //    assetObject.GovernorateNameAr = item.Hospital.Governorate.NameAr;
            //    assetObject.CityId = item.Hospital.City.Id;
            //    assetObject.CityName = item.Hospital.City.Name;
            //    assetObject.CityNameAr = item.Hospital.City.NameAr;
            //    assetObject.OrganizationId = item.Hospital.Organization.Id;
            //    assetObject.OrgName = item.Hospital.Organization.Name;
            //    assetObject.OrgNameAr = item.Hospital.Organization.NameAr;
            //    assetObject.SubOrgName = item.Hospital.SubOrganization.Name;
            //    assetObject.SubOrgNameAr = item.Hospital.SubOrganization.NameAr;
            //    assetObject.SubOrganizationId = item.Hospital.SubOrganization.Id;
            //    assetObject.BrandId = item.MasterAsset.brand != null ? item.MasterAsset.brand.Id : 0;
            //    assetObject.BrandName = item.MasterAsset.brand != null ? item.MasterAsset.brand.Name : "";
            //    assetObject.BrandNameAr = item.MasterAsset.brand != null ? item.MasterAsset.brand.NameAr : "";
            //    assetObject.SupplierId = item.Supplier != null ? item.Supplier.Id : 0;
            //    assetObject.SupplierName = item.Supplier != null ? item.Supplier.Name : "";
            //    assetObject.SupplierNameAr = item.Supplier != null ? item.Supplier.NameAr : "";
            //    assetObject.DepartmentName = item.Department != null ? item.Department.Name : "";
            //    assetObject.DepartmentNameAr = item.Department != null ? item.Department.NameAr : "";

            //    var lstTransactions = _context.AssetStatusTransactions.Where(a => a.AssetDetailId == item.Id).OrderByDescending(a => a.StatusDate.Value.Date).ToList();
            //    if (lstTransactions.Count > 0)
            //    {
            //        assetObject.AssetStatusId = lstTransactions[0].AssetStatusId;
            //    }

            //    result.Add(assetObject);

            //}
            //return result; 
            #endregion

            IndexAssetDetailVM mainClass = new IndexAssetDetailVM();

            #region New Code 
            var AssetDetails = _context.AssetDetails.Include(a => a.MasterAsset).Include(a => a.MasterAsset.brand).Include(a => a.Hospital)
               .Include(a => a.Department).Include(a => a.Supplier).Include(a => a.Hospital.Governorate).Include(a => a.Hospital.City)
               .Include(a => a.Hospital.Organization).Include(a => a.Hospital.SubOrganization).Where(a => a.MasterAsset.BrandId == brandId)
               .Select(Ass => new IndexAssetDetailVM.GetData
               {
                   Id = Ass.Id,
                   AssetName = Ass.MasterAsset.Name,
                   Barcode = Ass.Barcode,
                   SerialNumber = Ass.SerialNumber,
                   Model = Ass.MasterAsset.ModelNumber,
                   DepartmentName = Ass.Department.Name,
                   DepartmentNameAr = Ass.Department.NameAr,
                   AssetNameAr = Ass.MasterAsset.NameAr,
                   BrandName = Ass.MasterAsset.brand.Name,
                   BrandNameAr = Ass.MasterAsset.brand.NameAr,
                   GovernorateName = Ass.Hospital.Governorate.Name,
                   GovernorateNameAr = Ass.Hospital.Governorate.NameAr,
                   CityName = Ass.Hospital.City.Name,
                   CityNameAr = Ass.Hospital.City.NameAr,
                   HospitalId = Ass.HospitalId,
                   HospitalName = Ass.Hospital.Name,
                   HospitalNameAr = Ass.Hospital.NameAr,
                   SupplierName = Ass.Supplier.Name,
                   SupplierNameAr = Ass.Supplier.NameAr,
                   OrgName = Ass.Hospital.Organization.Name,
                   OrgNameAr = Ass.Hospital.Organization.NameAr,
                   PurchaseDate = Ass.PurchaseDate,
                   BrandId = Ass.MasterAsset.BrandId

               }).ToList();

            mainClass.Results = AssetDetails;
            mainClass.Count = AssetDetails.Count;
            return mainClass;
            #endregion



        }

        public IndexAssetDetailVM GetAssetsByDepartmentId(int departmentId)
        {

            IndexAssetDetailVM mainClass = new IndexAssetDetailVM();

            #region New Code 
            var AssetDetails = _context.AssetDetails.Include(a => a.MasterAsset).Include(a => a.MasterAsset.brand).Include(a => a.Hospital)
               .Include(a => a.Department).Include(a => a.Supplier).Include(a => a.Hospital.Governorate).Include(a => a.Hospital.City)
               .Include(a => a.Hospital.Organization).Include(a => a.Hospital.SubOrganization).Where(a => a.Department.Id == departmentId)
               .Select(Ass => new IndexAssetDetailVM.GetData
               {
                   Id = Ass.Id,
                   AssetName = Ass.MasterAsset.Name,
                   Barcode = Ass.Barcode,
                   SerialNumber = Ass.SerialNumber,
                   Model = Ass.MasterAsset.ModelNumber,
                   DepartmentName = Ass.Department.Name,
                   DepartmentNameAr = Ass.Department.NameAr,
                   AssetNameAr = Ass.MasterAsset.NameAr,
                   BrandName = Ass.MasterAsset.brand.Name,
                   BrandNameAr = Ass.MasterAsset.brand.NameAr,
                   GovernorateName = Ass.Hospital.Governorate.Name,
                   GovernorateNameAr = Ass.Hospital.Governorate.NameAr,
                   CityName = Ass.Hospital.City.Name,
                   CityNameAr = Ass.Hospital.City.NameAr,
                   HospitalId = Ass.HospitalId,
                   HospitalName = Ass.Hospital.Name,
                   HospitalNameAr = Ass.Hospital.NameAr,
                   SupplierName = Ass.Supplier.Name,
                   SupplierNameAr = Ass.Supplier.NameAr,
                   OrgName = Ass.Hospital.Organization.Name,
                   OrgNameAr = Ass.Hospital.Organization.NameAr,
                   PurchaseDate = Ass.PurchaseDate,
                   BrandId = Ass.MasterAsset.BrandId

               }).ToList();

            mainClass.Results = AssetDetails;
            mainClass.Count = AssetDetails.Count;
            return mainClass;
            #endregion
        }

        public List<IndexAssetDetailVM.GetData> GetAssetsBySupplierId(int supplierId)
        {

            var assets = _context.AssetDetails.Include(a => a.MasterAsset).Include(a => a.MasterAsset.brand).Include(a => a.Hospital)
              .Include(a => a.Department).Include(a => a.Supplier).Include(a => a.Hospital.Governorate).Include(a => a.Hospital.City)
              .Include(a => a.Hospital.Organization).Include(a => a.Hospital.SubOrganization).Where(a => a.Supplier.Id == supplierId).ToList();
            List<IndexAssetDetailVM.GetData> result = new List<IndexAssetDetailVM.GetData>();
            foreach (var item in assets)
            {
                IndexAssetDetailVM.GetData assetObject = new IndexAssetDetailVM.GetData();
                assetObject.Id = item.Id;
                assetObject.Code = item.Code;
                assetObject.Price = item.Price;
                assetObject.Barcode = item.Barcode;
                assetObject.Serial = item.SerialNumber;
                assetObject.SerialNumber = item.SerialNumber;
                assetObject.BarCode = item.Barcode;
                assetObject.Model = item.MasterAsset.ModelNumber;
                assetObject.MasterAssetId = item.MasterAssetId;
                assetObject.PurchaseDate = item.PurchaseDate;
                assetObject.HospitalId = item.HospitalId;
                assetObject.DepartmentId = item.DepartmentId;
                assetObject.HospitalName = item.Hospital.Name;
                assetObject.HospitalNameAr = item.Hospital.NameAr;
                assetObject.AssetName = item.MasterAsset.Name;
                assetObject.AssetNameAr = item.MasterAsset.NameAr;
                assetObject.GovernorateId = item.Hospital.Governorate.Id;
                assetObject.GovernorateName = item.Hospital.Governorate.Name;
                assetObject.GovernorateNameAr = item.Hospital.Governorate.NameAr;
                assetObject.CityId = item.Hospital.City.Id;
                assetObject.CityName = item.Hospital.City.Name;
                assetObject.CityNameAr = item.Hospital.City.NameAr;
                assetObject.OrganizationId = item.Hospital.Organization.Id;
                assetObject.OrgName = item.Hospital.Organization.Name;
                assetObject.OrgNameAr = item.Hospital.Organization.NameAr;
                assetObject.SubOrgName = item.Hospital.SubOrganization.Name;
                assetObject.SubOrgNameAr = item.Hospital.SubOrganization.NameAr;
                assetObject.SubOrganizationId = item.Hospital.SubOrganization.Id;
                assetObject.BrandId = item.MasterAsset.brand != null ? item.MasterAsset.brand.Id : 0;
                assetObject.BrandName = item.MasterAsset.brand != null ? item.MasterAsset.brand.Name : "";
                assetObject.BrandNameAr = item.MasterAsset.brand != null ? item.MasterAsset.brand.NameAr : "";
                assetObject.SupplierId = item.Supplier != null ? item.Supplier.Id : 0;
                assetObject.SupplierName = item.Supplier != null ? item.Supplier.Name : "";
                assetObject.SupplierNameAr = item.Supplier != null ? item.Supplier.NameAr : "";
                assetObject.DepartmentName = item.Department != null ? item.Department.Name : "";
                assetObject.DepartmentNameAr = item.Department != null ? item.Department.NameAr : "";

                var lstTransactions = _context.AssetStatusTransactions.Where(a => a.AssetDetailId == item.Id).OrderByDescending(a => a.StatusDate.Value.Date).ToList();
                if (lstTransactions.Count > 0)
                {
                    assetObject.AssetStatusId = lstTransactions[0].AssetStatusId;
                }

                result.Add(assetObject);

            }
            return result;
        }

        public IndexAssetDetailVM GetAssetsBySupplierIdWithPaging(int supplierId, int pageNumber, int pageSize)
        {
            IndexAssetDetailVM mainClass = new IndexAssetDetailVM();
            var result = new List<IndexAssetDetailVM.GetData>();
            var totalSuppliers = _context.AssetDetails.Include(a => a.MasterAsset).Include(a => a.MasterAsset.brand)
                    .Include(a => a.Hospital)
                    .Include(a => a.Supplier).ToList();

            if (supplierId > 0)
            {
                totalSuppliers = totalSuppliers.Where(a => a.Supplier.Id == supplierId).ToList();
                mainClass.Count = totalSuppliers.Count;
            }
            else
            {
                mainClass.Count = totalSuppliers.Count;
            }

            var assets = _context.AssetDetails.Include(a => a.MasterAsset).Include(a => a.MasterAsset.brand)
                .Include(a => a.Hospital)
                .Include(a => a.Department)
                .Include(a => a.Supplier)
                .Include(a => a.Hospital.Governorate)
                .Include(a => a.Hospital.City)
                .Include(a => a.Hospital.Organization)
                .Include(a => a.Hospital.SubOrganization)
                .Where(a => a.Supplier.Id == supplierId)?
                .Skip((pageNumber - 1) * pageSize).Take(pageSize)
                .ToList();

            foreach (var item in assets)
            {
                IndexAssetDetailVM.GetData assetObject = new IndexAssetDetailVM.GetData();
                assetObject.Id = item.Id;
                assetObject.Code = item.Code;
                assetObject.Price = item.Price;
                assetObject.Barcode = item.Barcode;
                assetObject.Serial = item.SerialNumber;
                assetObject.SerialNumber = item.SerialNumber;
                assetObject.BarCode = item.Barcode;
                assetObject.Model = item.MasterAsset.ModelNumber;
                assetObject.MasterAssetId = item.MasterAssetId;
                assetObject.PurchaseDate = item.PurchaseDate;
                assetObject.HospitalId = item.HospitalId;
                assetObject.DepartmentId = item.DepartmentId;
                assetObject.HospitalName = item.Hospital.Name;
                assetObject.HospitalNameAr = item.Hospital.NameAr;
                assetObject.AssetName = item.MasterAsset.Name;
                assetObject.AssetNameAr = item.MasterAsset.NameAr;
                assetObject.GovernorateId = item.Hospital.Governorate.Id;
                assetObject.GovernorateName = item.Hospital.Governorate.Name;
                assetObject.GovernorateNameAr = item.Hospital.Governorate.NameAr;
                assetObject.CityId = item.Hospital.City.Id;
                assetObject.CityName = item.Hospital.City.Name;
                assetObject.CityNameAr = item.Hospital.City.NameAr;
                assetObject.OrganizationId = item.Hospital.Organization.Id;
                assetObject.OrgName = item.Hospital.Organization.Name;
                assetObject.OrgNameAr = item.Hospital.Organization.NameAr;
                assetObject.SubOrgName = item.Hospital.SubOrganization.Name;
                assetObject.SubOrgNameAr = item.Hospital.SubOrganization.NameAr;
                assetObject.SubOrganizationId = item.Hospital.SubOrganization.Id;
                assetObject.BrandId = item.MasterAsset.brand != null ? item.MasterAsset.brand.Id : 0;
                assetObject.BrandName = item.MasterAsset.brand != null ? item.MasterAsset.brand.Name : "";
                assetObject.BrandNameAr = item.MasterAsset.brand != null ? item.MasterAsset.brand.NameAr : "";
                assetObject.SupplierId = item.Supplier != null ? item.Supplier.Id : 0;
                assetObject.SupplierName = item.Supplier != null ? item.Supplier.Name : "";
                assetObject.SupplierNameAr = item.Supplier != null ? item.Supplier.NameAr : "";
                assetObject.DepartmentName = item.Department != null ? item.Department.Name : "";
                assetObject.DepartmentNameAr = item.Department != null ? item.Department.NameAr : "";

                var lstTransactions = _context.AssetStatusTransactions.Where(a => a.AssetDetailId == item.Id).OrderByDescending(a => a.StatusDate.Value.Date).ToList();
                if (lstTransactions.Count > 0)
                {
                    assetObject.AssetStatusId = lstTransactions[0].AssetStatusId;
                }

                result.Add(assetObject);
            }

            mainClass.Results = result;
            return mainClass;
        }



        private IQueryable<AssetDetail> GetLstAssetDetails2()
        {
            return _context.AssetDetails.Include(a => a.MasterAsset).Include(a => a.MasterAsset.brand)
                         .Include(a => a.Supplier).Include(a => a.Department)
                         .Include(a => a.Hospital).Include(h => h.Hospital.Organization).Include(h => h.Hospital.Governorate)
                         .Include(h => h.Hospital.City).Include(h => h.Hospital.SubOrganization);
        }

        public IndexAssetDetailVM GetAssetsByGovernorateIdWithPaging(int governorateId, int pageNumber, int pageSize)
        {
            IndexAssetDetailVM mainClass = new IndexAssetDetailVM();
            var result = new List<IndexAssetDetailVM.GetData>();

            var list = GetLstAssetDetails2();

            if (governorateId > 0)
            {
                list = list.Where(a => a.Hospital.GovernorateId == governorateId);
                mainClass.Count = list.Count();
            }
            else
            {
                mainClass.Count = list.Count();
            }


            if (pageNumber == 0 && pageSize == 0)
                list = list;
            else
                list = list.Skip((pageNumber - 1) * pageSize).Take(pageSize);



            foreach (var item in list)
            {
                IndexAssetDetailVM.GetData assetObject = new IndexAssetDetailVM.GetData();
                assetObject.Id = item.Id;
                assetObject.Code = item.Code;
                assetObject.Price = item.Price;
                assetObject.Barcode = item.Barcode;
                assetObject.Serial = item.SerialNumber;
                assetObject.SerialNumber = item.SerialNumber;
                assetObject.BarCode = item.Barcode;
                assetObject.Model = item.MasterAsset.ModelNumber;
                assetObject.MasterAssetId = item.MasterAssetId;
                assetObject.PurchaseDate = item.PurchaseDate;
                assetObject.HospitalId = item.HospitalId;
                assetObject.DepartmentId = item.DepartmentId;
                assetObject.HospitalName = item.Hospital.Name;
                assetObject.HospitalNameAr = item.Hospital.NameAr;
                assetObject.AssetName = item.MasterAsset.Name;
                assetObject.AssetNameAr = item.MasterAsset.NameAr;
                assetObject.GovernorateId = item.Hospital.Governorate.Id;
                assetObject.GovernorateName = item.Hospital.Governorate.Name;
                assetObject.GovernorateNameAr = item.Hospital.Governorate.NameAr;
                assetObject.CityId = item.Hospital.City.Id;
                assetObject.CityName = item.Hospital.City.Name;
                assetObject.CityNameAr = item.Hospital.City.NameAr;
                assetObject.OrganizationId = item.Hospital.Organization.Id;
                assetObject.OrgName = item.Hospital.Organization.Name;
                assetObject.OrgNameAr = item.Hospital.Organization.NameAr;
                assetObject.SubOrgName = item.Hospital.SubOrganization.Name;
                assetObject.SubOrgNameAr = item.Hospital.SubOrganization.NameAr;
                assetObject.SubOrganizationId = item.Hospital.SubOrganization.Id;
                assetObject.BrandId = item.MasterAsset.brand != null ? item.MasterAsset.brand.Id : 0;
                assetObject.BrandName = item.MasterAsset.brand != null ? item.MasterAsset.brand.Name : "";
                assetObject.BrandNameAr = item.MasterAsset.brand != null ? item.MasterAsset.brand.NameAr : "";
                assetObject.SupplierId = item.Supplier != null ? item.Supplier.Id : 0;
                assetObject.SupplierName = item.Supplier != null ? item.Supplier.Name : "";
                assetObject.SupplierNameAr = item.Supplier != null ? item.Supplier.NameAr : "";
                assetObject.DepartmentName = item.Department != null ? item.Department.Name : "";
                assetObject.DepartmentNameAr = item.Department != null ? item.Department.NameAr : "";
                assetObject.Count = list.Count();

                var lstTransactions = _context.AssetStatusTransactions.Where(a => a.AssetDetailId == item.Id).OrderByDescending(a => a.StatusDate.Value.Date).ToList();
                if (lstTransactions.Count > 0)
                {
                    assetObject.AssetStatusId = lstTransactions[0].AssetStatusId;
                }

                result.Add(assetObject);
            }

            mainClass.Results = result;
            return mainClass;
        }


        public IndexAssetDetailVM GetAssetsByGovernorateIdWithPaging(int governorateId)
        {
            IndexAssetDetailVM mainClass = new IndexAssetDetailVM();
            var result = new List<IndexAssetDetailVM.GetData>();
            var list = GetLstAssetDetails2();
            if (governorateId > 0)
            {
                list = list.Where(a => a.Hospital.GovernorateId == governorateId);
                mainClass.Count = list.Count();

            }
            else
            {
                list = list;
                mainClass.Count = list.Count();

            }

            foreach (var item in list)
            {
                IndexAssetDetailVM.GetData assetObject = new IndexAssetDetailVM.GetData();
                assetObject.Id = item.Id;
                assetObject.Code = item.Code;
                assetObject.Price = item.Price;
                assetObject.Barcode = item.Barcode;
                assetObject.Serial = item.SerialNumber;
                assetObject.SerialNumber = item.SerialNumber;
                assetObject.BarCode = item.Barcode;
                assetObject.Model = item.MasterAsset.ModelNumber;
                assetObject.HospitalName = item.Hospital.Name;
                assetObject.HospitalNameAr = item.Hospital.NameAr;
                assetObject.AssetName = item.MasterAsset.Name;
                assetObject.AssetNameAr = item.MasterAsset.NameAr;
                assetObject.CityId = item.Hospital.City.Id;
                assetObject.CityName = item.Hospital.City.Name;
                assetObject.CityNameAr = item.Hospital.City.NameAr;
                assetObject.OrgName = item.Hospital.Organization.Name;
                assetObject.OrgNameAr = item.Hospital.Organization.NameAr;
                assetObject.SubOrgName = item.Hospital.SubOrganization.Name;
                assetObject.SubOrgNameAr = item.Hospital.SubOrganization.NameAr;
                assetObject.BrandName = item.MasterAsset.brand != null ? item.MasterAsset.brand.Name : "";
                assetObject.BrandNameAr = item.MasterAsset.brand != null ? item.MasterAsset.brand.NameAr : "";
                assetObject.SupplierId = item.Supplier != null ? item.Supplier.Id : 0;
                assetObject.SupplierName = item.Supplier != null ? item.Supplier.Name : "";
                assetObject.SupplierNameAr = item.Supplier != null ? item.Supplier.NameAr : "";
                assetObject.DepartmentName = item.Department != null ? item.Department.Name : "";
                assetObject.DepartmentNameAr = item.Department != null ? item.Department.NameAr : "";

                var lstTransactions = _context.AssetStatusTransactions.Where(a => a.AssetDetailId == item.Id).OrderByDescending(a => a.StatusDate.Value.Date).ToList();
                if (lstTransactions.Count > 0)
                {
                    assetObject.AssetStatusId = lstTransactions[0].AssetStatusId;
                }

                result.Add(assetObject);
            }

            mainClass.Results = result;
            return mainClass;
        }








        public IndexAssetDetailVM GetAssetsByCityIdWithPaging(int cityId, int pageNumber, int pageSize)
        {
            IndexAssetDetailVM mainClass = new IndexAssetDetailVM();
            var result = new List<IndexAssetDetailVM.GetData>();

            var list = GetLstAssetDetails2();

            if (cityId > 0)
            {
                list = list.Where(a => a.Hospital.CityId == cityId);
                mainClass.Count = list.Count();
            }
            else
            {
                mainClass.Count = list.Count();
            }


            if (pageNumber == 0 && pageSize == 0)
                list = list;
            else
                list = list.Skip((pageNumber - 1) * pageSize).Take(pageSize);



            foreach (var item in list)
            {
                IndexAssetDetailVM.GetData assetObject = new IndexAssetDetailVM.GetData();
                assetObject.Id = item.Id;
                assetObject.Code = item.Code;
                assetObject.Price = item.Price;
                assetObject.Barcode = item.Barcode;
                assetObject.Serial = item.SerialNumber;
                assetObject.SerialNumber = item.SerialNumber;
                assetObject.BarCode = item.Barcode;
                assetObject.Model = item.MasterAsset.ModelNumber;
                assetObject.MasterAssetId = item.MasterAssetId;
                assetObject.PurchaseDate = item.PurchaseDate;
                assetObject.HospitalId = item.HospitalId;
                assetObject.DepartmentId = item.DepartmentId;
                assetObject.HospitalName = item.Hospital.Name;
                assetObject.HospitalNameAr = item.Hospital.NameAr;
                assetObject.AssetName = item.MasterAsset.Name;
                assetObject.AssetNameAr = item.MasterAsset.NameAr;
                assetObject.GovernorateId = item.Hospital.Governorate.Id;
                assetObject.GovernorateName = item.Hospital.Governorate.Name;
                assetObject.GovernorateNameAr = item.Hospital.Governorate.NameAr;
                assetObject.CityId = item.Hospital.City.Id;
                assetObject.CityName = item.Hospital.City.Name;
                assetObject.CityNameAr = item.Hospital.City.NameAr;
                assetObject.OrganizationId = item.Hospital.Organization.Id;
                assetObject.OrgName = item.Hospital.Organization.Name;
                assetObject.OrgNameAr = item.Hospital.Organization.NameAr;
                assetObject.SubOrgName = item.Hospital.SubOrganization.Name;
                assetObject.SubOrgNameAr = item.Hospital.SubOrganization.NameAr;
                assetObject.SubOrganizationId = item.Hospital.SubOrganization.Id;
                assetObject.BrandId = item.MasterAsset.brand != null ? item.MasterAsset.brand.Id : 0;
                assetObject.BrandName = item.MasterAsset.brand != null ? item.MasterAsset.brand.Name : "";
                assetObject.BrandNameAr = item.MasterAsset.brand != null ? item.MasterAsset.brand.NameAr : "";
                assetObject.SupplierId = item.Supplier != null ? item.Supplier.Id : 0;
                assetObject.SupplierName = item.Supplier != null ? item.Supplier.Name : "";
                assetObject.SupplierNameAr = item.Supplier != null ? item.Supplier.NameAr : "";
                assetObject.DepartmentName = item.Department != null ? item.Department.Name : "";
                assetObject.DepartmentNameAr = item.Department != null ? item.Department.NameAr : "";
                assetObject.Count = list.Count();

                var lstTransactions = _context.AssetStatusTransactions.Where(a => a.AssetDetailId == item.Id).OrderByDescending(a => a.StatusDate.Value.Date).ToList();
                if (lstTransactions.Count > 0)
                {
                    assetObject.AssetStatusId = lstTransactions[0].AssetStatusId;
                }

                result.Add(assetObject);
            }

            mainClass.Results = result;
            return mainClass;
        }


        public IndexAssetDetailVM GetAssetsByCityIdWithPaging(int cityId)
        {
            IndexAssetDetailVM mainClass = new IndexAssetDetailVM();
            var result = new List<IndexAssetDetailVM.GetData>();
            var list = GetLstAssetDetails2();
            if (cityId > 0)
            {
                list = list.Where(a => a.Hospital.CityId == cityId);
                mainClass.Count = list.Count();

            }
            else
            {
                list = list;
                mainClass.Count = list.Count();

            }

            foreach (var item in list)
            {
                IndexAssetDetailVM.GetData assetObject = new IndexAssetDetailVM.GetData();
                assetObject.Id = item.Id;
                assetObject.Code = item.Code;
                assetObject.Price = item.Price;
                assetObject.Barcode = item.Barcode;
                assetObject.Serial = item.SerialNumber;
                assetObject.SerialNumber = item.SerialNumber;
                assetObject.BarCode = item.Barcode;
                assetObject.Model = item.MasterAsset.ModelNumber;
                assetObject.HospitalName = item.Hospital.Name;
                assetObject.HospitalNameAr = item.Hospital.NameAr;
                assetObject.AssetName = item.MasterAsset.Name;
                assetObject.AssetNameAr = item.MasterAsset.NameAr;
                assetObject.CityId = item.Hospital.City.Id;
                assetObject.CityName = item.Hospital.City.Name;
                assetObject.CityNameAr = item.Hospital.City.NameAr;
                assetObject.OrgName = item.Hospital.Organization.Name;
                assetObject.OrgNameAr = item.Hospital.Organization.NameAr;
                assetObject.SubOrgName = item.Hospital.SubOrganization.Name;
                assetObject.SubOrgNameAr = item.Hospital.SubOrganization.NameAr;
                assetObject.BrandName = item.MasterAsset.brand != null ? item.MasterAsset.brand.Name : "";
                assetObject.BrandNameAr = item.MasterAsset.brand != null ? item.MasterAsset.brand.NameAr : "";
                assetObject.SupplierId = item.Supplier != null ? item.Supplier.Id : 0;
                assetObject.SupplierName = item.Supplier != null ? item.Supplier.Name : "";
                assetObject.SupplierNameAr = item.Supplier != null ? item.Supplier.NameAr : "";
                assetObject.DepartmentName = item.Department != null ? item.Department.Name : "";
                assetObject.DepartmentNameAr = item.Department != null ? item.Department.NameAr : "";

                var lstTransactions = _context.AssetStatusTransactions.Where(a => a.AssetDetailId == item.Id).OrderByDescending(a => a.StatusDate.Value.Date).ToList();
                if (lstTransactions.Count > 0)
                {
                    assetObject.AssetStatusId = lstTransactions[0].AssetStatusId;
                }

                result.Add(assetObject);
            }

            mainClass.Results = result;
            return mainClass;
        }













        public IndexAssetDetailVM SortAssetDetailAfterSearch(SortAndFilterVM data, int pageNumber, int pageSize)
        {


            IQueryable<AssetDetail> query = null;
            IndexAssetDetailVM mainClass = new IndexAssetDetailVM();
            List<IndexAssetDetailVM.GetData> list = new List<IndexAssetDetailVM.GetData>();
            List<AssetDetail> searchResult = new List<AssetDetail>();


            if (data.SearchObj.AssetNameAr != "")
            {
                if (query is not null)
                {


                    if (data.SortObj.SortBy != "")
                    {
                        switch (data.SortObj.SortBy)
                        {
                            case "الاسم":
                                if (data.SortObj.SortStatus == "ascending")
                                {
                                    // filteration and sort then pagination 
                                    // Where() , OrderBy() then Skip() Take()
                                    query = query.Where(e => e.MasterAsset.NameAr == data.SearchObj.AssetNameAr).
                                      OrderBy(ww => ww.MasterAsset.NameAr);
                                }
                                else
                                {
                                    query = query.Where(e => e.MasterAsset.NameAr == data.SearchObj.AssetNameAr).
                                  OrderByDescending(ww => ww.MasterAsset.NameAr);
                                }
                                break;

                            case "Name":
                                if (data.SortObj.SortStatus == "ascending")
                                {
                                    query = query.Where(e => e.MasterAsset.NameAr == data.SearchObj.AssetNameAr).
                                    OrderBy(ww => ww.MasterAsset.Name);

                                }
                                else
                                {

                                    query = query.Where(e => e.MasterAsset.NameAr == data.SearchObj.AssetNameAr).
                                        OrderByDescending(ww => ww.MasterAsset.Name);

                                }

                                break;

                            case "الباركود":
                            case "Barcode":

                                if (data.SortObj.SortStatus == "ascending")
                                {
                                    query = query.Where(e => e.MasterAsset.NameAr == data.SearchObj.AssetNameAr).
                                    OrderBy(ww => ww.Barcode);


                                }
                                else
                                {
                                    query = query.Where(e => e.MasterAsset.NameAr == data.SearchObj.AssetNameAr).
                                      OrderByDescending(ww => ww.Barcode);

                                }

                                break;

                            case "السيريال":
                            case "Serial":
                                if (data.SortObj.SortStatus == "ascending")
                                {
                                    query = query.Where(e => e.MasterAsset.NameAr == data.SearchObj.AssetNameAr).
                                    OrderBy(ww => ww.SerialNumber);

                                }
                                else
                                {
                                    query = query.Where(e => e.MasterAsset.NameAr == data.SearchObj.AssetNameAr).
                                   OrderByDescending(ww => ww.SerialNumber);
                                }

                                break;

                            case "رقم الموديل":
                            case "Model Number":
                                if (data.SortObj.SortStatus == "ascending")
                                {
                                    query = query.Where(e => e.MasterAsset.NameAr == data.SearchObj.AssetNameAr).
                                         OrderBy(ww => ww.MasterAsset.ModelNumber);
                                }
                                else
                                {
                                    query = query.Where(e => e.MasterAsset.NameAr == data.SearchObj.AssetNameAr).
                                         OrderByDescending(ww => ww.MasterAsset.ModelNumber);
                                }
                                break;
                            case "القسم":
                                if (data.SortObj.SortStatus == "ascending")
                                {
                                    query = query.Where(e => e.MasterAsset.NameAr == data.SearchObj.AssetNameAr).
                                      OrderBy(ww => ww.DepartmentId);
                                }
                                else
                                {
                                    query = query.Where(e => e.MasterAsset.NameAr == data.SearchObj.AssetNameAr).
                                                    OrderByDescending(ww => ww.DepartmentId);

                                }
                                break;


                            case "Department":
                                if (data.SortObj.SortStatus == "ascending")
                                {
                                    query = query.Where(e => e.MasterAsset.NameAr == data.SearchObj.AssetNameAr).
                                         OrderBy(ww => ww.DepartmentId);

                                }
                                else
                                {
                                    query = query.Where(e => e.MasterAsset.NameAr == data.SearchObj.AssetNameAr).
                                                   OrderByDescending(ww => ww.DepartmentId);
                                }
                                break;

                            case "الماركة":
                                if (data.SortObj.SortStatus == "ascending")
                                {
                                    query = query.Where(e => e.MasterAsset.NameAr == data.SearchObj.AssetNameAr).
                                        OrderBy(ww => ww.MasterAsset.brand.NameAr);


                                }
                                else
                                {
                                    query = query.Where(e => e.MasterAsset.NameAr == data.SearchObj.AssetNameAr).
                                       OrderByDescending(ww => ww.MasterAsset.brand.NameAr);
                                }
                                break;
                            case "Manufacture":
                                if (data.SortObj.SortStatus == "ascending")

                                {


                                    query = query.Where(e => e.MasterAsset.NameAr == data.SearchObj.AssetNameAr).
                                      OrderBy(ww => ww.MasterAsset.brand.Name);


                                }
                                else
                                {
                                    query = query.Where(e => e.MasterAsset.NameAr == data.SearchObj.AssetNameAr).
                                OrderByDescending(ww => ww.MasterAsset.brand.Name);
                                }
                                break;
                            case "المورد":
                                if (data.SortObj.SortStatus == "ascending")
                                {

                                    query = query.Where(e => e.MasterAsset.NameAr == data.SearchObj.AssetNameAr).
                                      OrderBy(ww => ww.SupplierId);
                                }
                                else
                                {
                                    query = query.Where(e => e.MasterAsset.NameAr == data.SearchObj.AssetNameAr).
                                    OrderByDescending(ww => ww.SupplierId);
                                }
                                break;
                            case "Supplier":
                                if (data.SortObj.SortStatus == "ascending")
                                {

                                    query = query.Where(e => e.MasterAsset.NameAr == data.SearchObj.AssetNameAr).
                                      OrderBy(ww => ww.SupplierId);
                                }
                                else
                                {
                                    query = query.Where(e => e.MasterAsset.NameAr == data.SearchObj.AssetNameAr).
                              OrderByDescending(ww => ww.SupplierId);

                                }
                                break;
                            case "تاريخ الشراء":
                            case "Purchased Date":
                                if (data.SortObj.SortStatus == "ascending")
                                {
                                    query = query.Where(e => e.MasterAsset.NameAr == data.SearchObj.AssetNameAr).
                                   OrderBy(ww => ww.PurchaseDate);
                                }
                                else
                                {
                                    query = query.Where(e => e.MasterAsset.NameAr == data.SearchObj.AssetNameAr).
                                   OrderByDescending(ww => ww.PurchaseDate);
                                }
                                break;
                        }
                    }
                }

                else
                {

                    if (data.SortObj.SortBy != "")
                    {
                        switch (data.SortObj.SortBy)
                        {
                            case "الاسم":
                                if (data.SortObj.SortStatus == "ascending")
                                {
                                    // filteration and sort then pagination 
                                    // Where() , OrderBy() then Skip() Take()
                                    query = GetLstAssetDetails().Where(e => e.MasterAsset.NameAr == data.SearchObj.AssetNameAr).
                                      OrderBy(ww => ww.MasterAsset.NameAr);
                                }
                                else
                                {
                                    query = GetLstAssetDetails().Where(e => e.MasterAsset.NameAr == data.SearchObj.AssetNameAr).
                                  OrderByDescending(ww => ww.MasterAsset.NameAr);
                                }
                                break;
                            case "Name":
                                if (data.SortObj.SortStatus == "ascending")
                                {
                                    query = GetLstAssetDetails().Where(e => e.MasterAsset.NameAr == data.SearchObj.AssetNameAr).
                                    OrderBy(ww => ww.MasterAsset.Name);
                                }
                                else
                                {
                                    query = GetLstAssetDetails().Where(e => e.MasterAsset.NameAr == data.SearchObj.AssetNameAr).
                                        OrderByDescending(ww => ww.MasterAsset.Name);
                                }
                                break;
                            case "الباركود":
                            case "Barcode":

                                if (data.SortObj.SortStatus == "ascending")
                                {
                                    query = GetLstAssetDetails().Where(e => e.MasterAsset.NameAr == data.SearchObj.AssetNameAr).
                                    OrderBy(ww => ww.Barcode);
                                }
                                else
                                {
                                    query = GetLstAssetDetails().Where(e => e.MasterAsset.NameAr == data.SearchObj.AssetNameAr).
                                      OrderByDescending(ww => ww.Barcode);
                                }
                                break;
                            case "السيريال":
                            case "Serial":
                                if (data.SortObj.SortStatus == "ascending")
                                {
                                    query = GetLstAssetDetails().Where(e => e.MasterAsset.NameAr == data.SearchObj.AssetNameAr).
                                    OrderBy(ww => ww.SerialNumber);
                                }
                                else
                                {
                                    query = GetLstAssetDetails().Where(e => e.MasterAsset.NameAr == data.SearchObj.AssetNameAr).
                                   OrderByDescending(ww => ww.SerialNumber);
                                }
                                break;
                            case "رقم الموديل":
                            case "Model Number":
                                if (data.SortObj.SortStatus == "ascending")
                                {
                                    query = GetLstAssetDetails().Where(e => e.MasterAsset.NameAr == data.SearchObj.AssetNameAr).
                                         OrderBy(ww => ww.MasterAsset.ModelNumber);
                                }
                                else
                                {
                                    query = GetLstAssetDetails().Where(e => e.MasterAsset.NameAr == data.SearchObj.AssetNameAr).
                                         OrderByDescending(ww => ww.MasterAsset.ModelNumber);
                                }
                                break;
                            case "القسم":
                                if (data.SortObj.SortStatus == "ascending")
                                {
                                    query = GetLstAssetDetails().Where(e => e.MasterAsset.NameAr == data.SearchObj.AssetNameAr).
                                      OrderBy(ww => ww.DepartmentId);
                                }
                                else
                                {
                                    query = GetLstAssetDetails().Where(e => e.MasterAsset.NameAr == data.SearchObj.AssetNameAr).
                                                    OrderByDescending(ww => ww.DepartmentId);

                                }
                                break;


                            case "Department":
                                if (data.SortObj.SortStatus == "ascending")
                                {
                                    query = GetLstAssetDetails().Where(e => e.MasterAsset.NameAr == data.SearchObj.AssetNameAr).
                                         OrderBy(ww => ww.DepartmentId);

                                }
                                else
                                {
                                    query = GetLstAssetDetails().Where(e => e.MasterAsset.NameAr == data.SearchObj.AssetNameAr).
                                                   OrderByDescending(ww => ww.DepartmentId);
                                }
                                break;

                            case "الماركة":
                                if (data.SortObj.SortStatus == "ascending")
                                {
                                    query = GetLstAssetDetails().Where(e => e.MasterAsset.NameAr == data.SearchObj.AssetNameAr).
                                        OrderBy(ww => ww.MasterAsset.brand.NameAr);


                                }
                                else
                                {
                                    query = GetLstAssetDetails().Where(e => e.MasterAsset.NameAr == data.SearchObj.AssetNameAr).
                                       OrderByDescending(ww => ww.MasterAsset.brand.NameAr);
                                }
                                break;
                            case "Manufacture":
                                if (data.SortObj.SortStatus == "ascending")

                                {


                                    query = GetLstAssetDetails().Where(e => e.MasterAsset.NameAr == data.SearchObj.AssetNameAr).
                                      OrderBy(ww => ww.MasterAsset.brand.Name);


                                }
                                else
                                {
                                    query = GetLstAssetDetails().Where(e => e.MasterAsset.NameAr == data.SearchObj.AssetNameAr).
                                OrderByDescending(ww => ww.MasterAsset.brand.Name);
                                }
                                break;
                            case "المورد":
                                if (data.SortObj.SortStatus == "ascending")
                                {

                                    query = GetLstAssetDetails().Where(e => e.MasterAsset.NameAr == data.SearchObj.AssetNameAr).
                                      OrderBy(ww => ww.SupplierId);
                                }
                                else
                                {
                                    query = GetLstAssetDetails().Where(e => e.MasterAsset.NameAr == data.SearchObj.AssetNameAr).
                                    OrderByDescending(ww => ww.SupplierId);
                                }
                                break;
                            case "Supplier":
                                if (data.SortObj.SortStatus == "ascending")
                                {

                                    query = GetLstAssetDetails().Where(e => e.MasterAsset.NameAr == data.SearchObj.AssetNameAr).
                                      OrderBy(ww => ww.SupplierId);
                                }
                                else
                                {
                                    query = GetLstAssetDetails().Where(e => e.MasterAsset.NameAr == data.SearchObj.AssetNameAr).
                              OrderByDescending(ww => ww.SupplierId);

                                }
                                break;
                            case "تاريخ الشراء":
                            case "Purchased Date":
                                if (data.SortObj.SortStatus == "ascending")
                                {
                                    query = GetLstAssetDetails().Where(e => e.MasterAsset.NameAr == data.SearchObj.AssetNameAr).
                                   OrderBy(ww => ww.PurchaseDate);
                                }
                                else
                                {
                                    query = GetLstAssetDetails().Where(e => e.MasterAsset.NameAr == data.SearchObj.AssetNameAr).
                                   OrderByDescending(ww => ww.PurchaseDate);
                                }
                                break;
                        }
                    }
                }

                mainClass.Count = query.Count();
                searchResult = query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            }

            if (data.SearchObj.AssetName != "")
            {
                if (query is not null)
                {
                    if (data.SortObj.SortBy != "")
                    {
                        switch (data.SortObj.SortBy)
                        {
                            case "الاسم":
                                if (data.SortObj.SortStatus == "ascending")
                                {
                                    // filteration and sort then pagination 
                                    // Where() , OrderBy() then Skip() Take()
                                    query = query.Where(e => e.MasterAsset.Name == data.SearchObj.AssetName).
                                      OrderBy(ww => ww.MasterAsset.NameAr);
                                }
                                else
                                {
                                    query = query.Where(e => e.MasterAsset.Name == data.SearchObj.AssetName).
                                  OrderByDescending(ww => ww.MasterAsset.NameAr);
                                }
                                break;

                            case "Name":
                                if (data.SortObj.SortStatus == "ascending")
                                {
                                    query = query.Where(e => e.MasterAsset.Name == data.SearchObj.AssetName).
                                    OrderBy(ww => ww.MasterAsset.Name);

                                }
                                else
                                {

                                    query = query.Where(e => e.MasterAsset.Name == data.SearchObj.AssetName).
                                        OrderByDescending(ww => ww.MasterAsset.Name);

                                }

                                break;

                            case "الباركود":
                            case "Barcode":

                                if (data.SortObj.SortStatus == "ascending")
                                {
                                    query = query.Where(e => e.MasterAsset.Name == data.SearchObj.AssetName).
                                    OrderBy(ww => ww.Barcode);


                                }
                                else
                                {
                                    query = query.Where(e => e.MasterAsset.Name == data.SearchObj.AssetName).
                                      OrderByDescending(ww => ww.Barcode);

                                }

                                break;

                            case "السيريال":
                            case "Serial":
                                if (data.SortObj.SortStatus == "ascending")
                                {
                                    query = query.Where(e => e.MasterAsset.Name == data.SearchObj.AssetName).
                                    OrderBy(ww => ww.SerialNumber);

                                }
                                else
                                {
                                    query = query.Where(e => e.MasterAsset.Name == data.SearchObj.AssetName).
                                   OrderByDescending(ww => ww.SerialNumber);
                                }

                                break;

                            case "رقم الموديل":
                            case "Model Number":
                                if (data.SortObj.SortStatus == "ascending")
                                {
                                    query = query.Where(e => e.MasterAsset.Name == data.SearchObj.AssetName).
                                         OrderBy(ww => ww.MasterAsset.ModelNumber);
                                }
                                else
                                {
                                    query = query.Where(e => e.MasterAsset.Name == data.SearchObj.AssetName).
                                         OrderByDescending(ww => ww.MasterAsset.ModelNumber);
                                }
                                break;
                            case "القسم":
                                if (data.SortObj.SortStatus == "ascending")
                                {
                                    query = query.Where(e => e.MasterAsset.Name == data.SearchObj.AssetName).
                                      OrderBy(ww => ww.DepartmentId);
                                }
                                else
                                {
                                    query = query.Where(e => e.MasterAsset.Name == data.SearchObj.AssetName).
                                                    OrderByDescending(ww => ww.DepartmentId);

                                }
                                break;


                            case "Department":
                                if (data.SortObj.SortStatus == "ascending")
                                {
                                    query = query.Where(e => e.MasterAsset.Name == data.SearchObj.AssetName).
                                         OrderBy(ww => ww.DepartmentId);

                                }
                                else
                                {
                                    query = query.Where(e => e.MasterAsset.Name == data.SearchObj.AssetName).
                                                   OrderByDescending(ww => ww.DepartmentId);
                                }
                                break;

                            case "الماركة":
                                if (data.SortObj.SortStatus == "ascending")
                                {
                                    query = query.Where(e => e.MasterAsset.Name == data.SearchObj.AssetName).
                                        OrderBy(ww => ww.MasterAsset.brand.NameAr);


                                }
                                else
                                {
                                    query = query.Where(e => e.MasterAsset.Name == data.SearchObj.AssetName).
                                       OrderByDescending(ww => ww.MasterAsset.brand.NameAr);
                                }
                                break;
                            case "Manufacture":
                                if (data.SortObj.SortStatus == "ascending")

                                {


                                    query = query.Where(e => e.MasterAsset.Name == data.SearchObj.AssetName).
                                      OrderBy(ww => ww.MasterAsset.brand.Name);


                                }
                                else
                                {
                                    query = query.Where(e => e.MasterAsset.Name == data.SearchObj.AssetName).
                                OrderByDescending(ww => ww.MasterAsset.brand.Name);
                                }
                                break;
                            case "المورد":
                                if (data.SortObj.SortStatus == "ascending")
                                {

                                    query = query.Where(e => e.MasterAsset.Name == data.SearchObj.AssetName).
                                      OrderBy(ww => ww.SupplierId);
                                }
                                else
                                {
                                    query = query.Where(e => e.MasterAsset.Name == data.SearchObj.AssetName).
                                    OrderByDescending(ww => ww.SupplierId);
                                }
                                break;
                            case "Supplier":
                                if (data.SortObj.SortStatus == "ascending")
                                {

                                    query = query.Where(e => e.MasterAsset.Name == data.SearchObj.AssetName).
                                      OrderBy(ww => ww.SupplierId);
                                }
                                else
                                {
                                    query = query.Where(e => e.MasterAsset.Name == data.SearchObj.AssetName).
                              OrderByDescending(ww => ww.SupplierId);

                                }
                                break;
                            case "تاريخ الشراء":
                            case "Purchased Date":
                                if (data.SortObj.SortStatus == "ascending")
                                {
                                    query = query.Where(e => e.MasterAsset.Name == data.SearchObj.AssetName).
                                   OrderBy(ww => ww.PurchaseDate);
                                }
                                else
                                {
                                    query = query.Where(e => e.MasterAsset.Name == data.SearchObj.AssetName).
                                   OrderByDescending(ww => ww.PurchaseDate);
                                }
                                break;



                        }
                    }

                }
                else
                {

                    if (data.SortObj.SortBy != "")
                    {
                        switch (data.SortObj.SortBy)
                        {
                            case "الاسم":
                                if (data.SortObj.SortStatus == "ascending")
                                {
                                    // filteration and sort then pagination 
                                    // Where() , OrderBy() then Skip() Take()
                                    query = GetLstAssetDetails().Where(e => e.MasterAsset.Name == data.SearchObj.AssetName).
                                      OrderBy(ww => ww.MasterAsset.NameAr);
                                }
                                else
                                {
                                    query = GetLstAssetDetails().Where(e => e.MasterAsset.Name == data.SearchObj.AssetName).
                                  OrderByDescending(ww => ww.MasterAsset.NameAr);
                                }
                                break;

                            case "Name":
                                if (data.SortObj.SortStatus == "ascending")
                                {
                                    query = GetLstAssetDetails().Where(e => e.MasterAsset.Name == data.SearchObj.AssetName).
                                    OrderBy(ww => ww.MasterAsset.Name);

                                }
                                else
                                {

                                    query = GetLstAssetDetails().Where(e => e.MasterAsset.Name == data.SearchObj.AssetName).
                                        OrderByDescending(ww => ww.MasterAsset.Name);

                                }

                                break;

                            case "الباركود":
                            case "Barcode":

                                if (data.SortObj.SortStatus == "ascending")
                                {
                                    query = GetLstAssetDetails().Where(e => e.MasterAsset.Name == data.SearchObj.AssetName).
                                    OrderBy(ww => ww.Barcode);


                                }
                                else
                                {
                                    query = GetLstAssetDetails().Where(e => e.MasterAsset.Name == data.SearchObj.AssetName).
                                      OrderByDescending(ww => ww.Barcode);

                                }

                                break;

                            case "السيريال":
                            case "Serial":
                                if (data.SortObj.SortStatus == "ascending")
                                {
                                    query = GetLstAssetDetails().Where(e => e.MasterAsset.Name == data.SearchObj.AssetName).
                                    OrderBy(ww => ww.SerialNumber);

                                }
                                else
                                {
                                    query = GetLstAssetDetails().Where(e => e.MasterAsset.Name == data.SearchObj.AssetName).
                                   OrderByDescending(ww => ww.SerialNumber);
                                }

                                break;

                            case "رقم الموديل":
                            case "Model Number":
                                if (data.SortObj.SortStatus == "ascending")
                                {
                                    query = GetLstAssetDetails().Where(e => e.MasterAsset.Name == data.SearchObj.AssetName).
                                         OrderBy(ww => ww.MasterAsset.ModelNumber);
                                }
                                else
                                {
                                    query = GetLstAssetDetails().Where(e => e.MasterAsset.Name == data.SearchObj.AssetName).
                                         OrderByDescending(ww => ww.MasterAsset.ModelNumber);
                                }
                                break;
                            case "القسم":
                                if (data.SortObj.SortStatus == "ascending")
                                {
                                    query = GetLstAssetDetails().Where(e => e.MasterAsset.Name == data.SearchObj.AssetName).
                                      OrderBy(ww => ww.DepartmentId);
                                }
                                else
                                {
                                    query = GetLstAssetDetails().Where(e => e.MasterAsset.Name == data.SearchObj.AssetName).
                                                    OrderByDescending(ww => ww.DepartmentId);

                                }
                                break;


                            case "Department":
                                if (data.SortObj.SortStatus == "ascending")
                                {
                                    query = GetLstAssetDetails().Where(e => e.MasterAsset.Name == data.SearchObj.AssetName).
                                         OrderBy(ww => ww.DepartmentId);

                                }
                                else
                                {
                                    query = GetLstAssetDetails().Where(e => e.MasterAsset.Name == data.SearchObj.AssetName).
                                                   OrderByDescending(ww => ww.DepartmentId);
                                }
                                break;

                            case "الماركة":
                                if (data.SortObj.SortStatus == "ascending")
                                {
                                    query = GetLstAssetDetails().Where(e => e.MasterAsset.Name == data.SearchObj.AssetName).
                                        OrderBy(ww => ww.MasterAsset.brand.NameAr);


                                }
                                else
                                {
                                    query = GetLstAssetDetails().Where(e => e.MasterAsset.Name == data.SearchObj.AssetName).
                                       OrderByDescending(ww => ww.MasterAsset.brand.NameAr);
                                }
                                break;
                            case "Manufacture":
                                if (data.SortObj.SortStatus == "ascending")

                                {


                                    query = GetLstAssetDetails().Where(e => e.MasterAsset.Name == data.SearchObj.AssetName).
                                      OrderBy(ww => ww.MasterAsset.brand.Name);


                                }
                                else
                                {
                                    query = GetLstAssetDetails().Where(e => e.MasterAsset.Name == data.SearchObj.AssetName).
                                OrderByDescending(ww => ww.MasterAsset.brand.Name);
                                }
                                break;
                            case "المورد":
                                if (data.SortObj.SortStatus == "ascending")
                                {

                                    query = GetLstAssetDetails().Where(e => e.MasterAsset.Name == data.SearchObj.AssetName).
                                      OrderBy(ww => ww.SupplierId);
                                }
                                else
                                {
                                    query = GetLstAssetDetails().Where(e => e.MasterAsset.Name == data.SearchObj.AssetName).
                                    OrderByDescending(ww => ww.SupplierId);
                                }
                                break;
                            case "Supplier":
                                if (data.SortObj.SortStatus == "ascending")
                                {

                                    query = GetLstAssetDetails().Where(e => e.MasterAsset.Name == data.SearchObj.AssetName).
                                      OrderBy(ww => ww.SupplierId);
                                }
                                else
                                {
                                    query = GetLstAssetDetails().Where(e => e.MasterAsset.Name == data.SearchObj.AssetName).
                              OrderByDescending(ww => ww.SupplierId);

                                }
                                break;
                            case "تاريخ الشراء":
                            case "Purchased Date":
                                if (data.SortObj.SortStatus == "ascending")
                                {
                                    query = GetLstAssetDetails().Where(e => e.MasterAsset.Name == data.SearchObj.AssetName).
                                   OrderBy(ww => ww.PurchaseDate);
                                }
                                else
                                {
                                    query = GetLstAssetDetails().Where(e => e.MasterAsset.Name == data.SearchObj.AssetName).
                                   OrderByDescending(ww => ww.PurchaseDate);
                                }
                                break;



                        }
                    }



                }

                mainClass.Count = query.Count();
                searchResult = query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            }

            if (data.SearchObj.DepartmentId != 0)
            {
                if (query is not null)
                {
                    if (data.SortObj.SortBy != "")
                    {
                        switch (data.SortObj.SortBy)
                        {
                            case "الاسم":
                                if (data.SortObj.SortStatus == "ascending")
                                {
                                    // filteration and sort then pagination 
                                    // Where() , OrderBy() then Skip() Take()
                                    query = query.Where(e => e.DepartmentId == data.SearchObj.DepartmentId).
                                      OrderBy(ww => ww.MasterAsset.NameAr);
                                }
                                else
                                {
                                    query = query.Where(e => e.DepartmentId == data.SearchObj.DepartmentId).
                                  OrderByDescending(ww => ww.MasterAsset.NameAr);
                                }
                                break;

                            case "Name":
                                if (data.SortObj.SortStatus == "ascending")
                                {
                                    query = query.Where(e => e.DepartmentId == data.SearchObj.DepartmentId).
                                    OrderBy(ww => ww.MasterAsset.Name);

                                }
                                else
                                {

                                    query = query.Where(e => e.DepartmentId == data.SearchObj.DepartmentId).
                                        OrderByDescending(ww => ww.MasterAsset.Name);

                                }

                                break;

                            case "الباركود":
                            case "Barcode":

                                if (data.SortObj.SortStatus == "ascending")
                                {
                                    query = query.Where(e => e.DepartmentId == data.SearchObj.DepartmentId).
                                    OrderBy(ww => ww.Barcode);


                                }
                                else
                                {
                                    query = query.Where(e => e.DepartmentId == data.SearchObj.DepartmentId).
                                      OrderByDescending(ww => ww.Barcode);

                                }

                                break;

                            case "السيريال":
                            case "Serial":
                                if (data.SortObj.SortStatus == "ascending")
                                {
                                    query = query.Where(e => e.DepartmentId == data.SearchObj.DepartmentId).
                                    OrderBy(ww => ww.SerialNumber);

                                }
                                else
                                {
                                    query = query.Where(e => e.DepartmentId == data.SearchObj.DepartmentId).
                                   OrderByDescending(ww => ww.SerialNumber);
                                }

                                break;

                            case "رقم الموديل":
                            case "Model Number":
                                if (data.SortObj.SortStatus == "ascending")
                                {
                                    query = query.Where(e => e.DepartmentId == data.SearchObj.DepartmentId).
                                         OrderBy(ww => ww.MasterAsset.ModelNumber);
                                }
                                else
                                {
                                    query = query.Where(e => e.DepartmentId == data.SearchObj.DepartmentId).
                                         OrderByDescending(ww => ww.MasterAsset.ModelNumber);
                                }
                                break;
                            case "القسم":
                                if (data.SortObj.SortStatus == "ascending")
                                {
                                    query = query.Where(e => e.DepartmentId == data.SearchObj.DepartmentId).
                                      OrderBy(ww => ww.DepartmentId);
                                }
                                else
                                {
                                    query = query.Where(e => e.DepartmentId == data.SearchObj.DepartmentId).
                                                    OrderByDescending(ww => ww.DepartmentId);

                                }
                                break;


                            case "Department":
                                if (data.SortObj.SortStatus == "ascending")
                                {
                                    query = query.Where(e => e.DepartmentId == data.SearchObj.DepartmentId).
                                         OrderBy(ww => ww.DepartmentId);

                                }
                                else
                                {
                                    query = query.Where(e => e.DepartmentId == data.SearchObj.DepartmentId).
                                                   OrderByDescending(ww => ww.DepartmentId);
                                }
                                break;

                            case "الماركة":
                                if (data.SortObj.SortStatus == "ascending")
                                {
                                    query = query.Where(e => e.DepartmentId == data.SearchObj.DepartmentId).
                                        OrderBy(ww => ww.MasterAsset.brand.NameAr);


                                }
                                else
                                {
                                    query = query.Where(e => e.DepartmentId == data.SearchObj.DepartmentId).
                                       OrderByDescending(ww => ww.MasterAsset.brand.NameAr);
                                }
                                break;
                            case "Manufacture":
                                if (data.SortObj.SortStatus == "ascending")

                                {


                                    query = query.Where(e => e.DepartmentId == data.SearchObj.DepartmentId).
                                      OrderBy(ww => ww.MasterAsset.brand.Name);


                                }
                                else
                                {
                                    query = query.Where(e => e.DepartmentId == data.SearchObj.DepartmentId).
                                OrderByDescending(ww => ww.MasterAsset.brand.Name);
                                }
                                break;
                            case "المورد":
                                if (data.SortObj.SortStatus == "ascending")
                                {

                                    query = query.Where(e => e.DepartmentId == data.SearchObj.DepartmentId).
                                      OrderBy(ww => ww.SupplierId);
                                }
                                else
                                {
                                    query = query.Where(e => e.DepartmentId == data.SearchObj.DepartmentId).
                                    OrderByDescending(ww => ww.SupplierId);
                                }
                                break;
                            case "Supplier":
                                if (data.SortObj.SortStatus == "ascending")
                                {

                                    query = query.Where(e => e.DepartmentId == data.SearchObj.DepartmentId).
                                      OrderBy(ww => ww.SupplierId);
                                }
                                else
                                {
                                    query = query.Where(e => e.DepartmentId == data.SearchObj.DepartmentId).
                              OrderByDescending(ww => ww.SupplierId);

                                }
                                break;
                            case "تاريخ الشراء":
                            case "Purchased Date":
                                if (data.SortObj.SortStatus == "ascending")
                                {
                                    query = query.Where(e => e.DepartmentId == data.SearchObj.DepartmentId).
                                   OrderBy(ww => ww.PurchaseDate);
                                }
                                else
                                {
                                    query = query.Where(e => e.MasterAsset.NameAr == data.SearchObj.AssetNameAr).
                                   OrderByDescending(ww => ww.PurchaseDate);
                                }
                                break;



                        }
                    }

                }
                else

                {
                    switch (data.SortObj.SortBy)
                    {
                        case "الاسم":
                            if (data.SortObj.SortStatus == "ascending")
                            {
                                // filteration and sort then pagination 
                                // Where() , OrderBy() then Skip() Take()
                                query = GetLstAssetDetails().Where(e => e.DepartmentId == data.SearchObj.DepartmentId).
                                  OrderBy(ww => ww.MasterAsset.NameAr);
                            }
                            else
                            {
                                query = GetLstAssetDetails().Where(e => e.DepartmentId == data.SearchObj.DepartmentId).
                              OrderByDescending(ww => ww.MasterAsset.NameAr);
                            }
                            break;

                        case "Name":
                            if (data.SortObj.SortStatus == "ascending")
                            {
                                query = GetLstAssetDetails().Where(e => e.DepartmentId == data.SearchObj.DepartmentId).
                                OrderBy(ww => ww.MasterAsset.Name);

                            }
                            else
                            {

                                query = GetLstAssetDetails().Where(e => e.DepartmentId == data.SearchObj.DepartmentId).
                                    OrderByDescending(ww => ww.MasterAsset.Name);

                            }

                            break;

                        case "الباركود":
                        case "Barcode":

                            if (data.SortObj.SortStatus == "ascending")
                            {
                                query = GetLstAssetDetails().Where(e => e.DepartmentId == data.SearchObj.DepartmentId).
                                OrderBy(ww => ww.Barcode);


                            }
                            else
                            {
                                query = GetLstAssetDetails().Where(e => e.DepartmentId == data.SearchObj.DepartmentId).
                                  OrderByDescending(ww => ww.Barcode);

                            }

                            break;

                        case "السيريال":
                        case "Serial":
                            if (data.SortObj.SortStatus == "ascending")
                            {
                                query = GetLstAssetDetails().Where(e => e.DepartmentId == data.SearchObj.DepartmentId).
                                OrderBy(ww => ww.SerialNumber);

                            }
                            else
                            {
                                query = GetLstAssetDetails().Where(e => e.DepartmentId == data.SearchObj.DepartmentId).
                               OrderByDescending(ww => ww.SerialNumber);
                            }

                            break;

                        case "رقم الموديل":
                        case "Model Number":
                            if (data.SortObj.SortStatus == "ascending")
                            {
                                query = GetLstAssetDetails().Where(e => e.DepartmentId == data.SearchObj.DepartmentId).
                                     OrderBy(ww => ww.MasterAsset.ModelNumber);
                            }
                            else
                            {
                                query = GetLstAssetDetails().Where(e => e.DepartmentId == data.SearchObj.DepartmentId).
                                     OrderByDescending(ww => ww.MasterAsset.ModelNumber);
                            }
                            break;
                        case "القسم":
                            if (data.SortObj.SortStatus == "ascending")
                            {
                                query = GetLstAssetDetails().Where(e => e.DepartmentId == data.SearchObj.DepartmentId).
                                  OrderBy(ww => ww.DepartmentId);
                            }
                            else
                            {
                                query = GetLstAssetDetails().Where(e => e.DepartmentId == data.SearchObj.DepartmentId).
                                                OrderByDescending(ww => ww.DepartmentId);

                            }
                            break;


                        case "Department":
                            if (data.SortObj.SortStatus == "ascending")
                            {
                                query = GetLstAssetDetails().Where(e => e.DepartmentId == data.SearchObj.DepartmentId).
                                     OrderBy(ww => ww.DepartmentId);

                            }
                            else
                            {
                                query = GetLstAssetDetails().Where(e => e.DepartmentId == data.SearchObj.DepartmentId).
                                               OrderByDescending(ww => ww.DepartmentId);
                            }
                            break;

                        case "الماركة":
                            if (data.SortObj.SortStatus == "ascending")
                            {
                                query = GetLstAssetDetails().Where(e => e.DepartmentId == data.SearchObj.DepartmentId).
                                    OrderBy(ww => ww.MasterAsset.brand.NameAr);


                            }
                            else
                            {
                                query = GetLstAssetDetails().Where(e => e.DepartmentId == data.SearchObj.DepartmentId).
                                   OrderByDescending(ww => ww.MasterAsset.brand.NameAr);
                            }
                            break;
                        case "Manufacture":
                            if (data.SortObj.SortStatus == "ascending")

                            {


                                query = GetLstAssetDetails().Where(e => e.DepartmentId == data.SearchObj.DepartmentId).
                                  OrderBy(ww => ww.MasterAsset.brand.Name);


                            }
                            else
                            {
                                query = GetLstAssetDetails().Where(e => e.DepartmentId == data.SearchObj.DepartmentId).
                            OrderByDescending(ww => ww.MasterAsset.brand.Name);
                            }
                            break;
                        case "المورد":
                            if (data.SortObj.SortStatus == "ascending")
                            {

                                query = GetLstAssetDetails().Where(e => e.DepartmentId == data.SearchObj.DepartmentId).
                                  OrderBy(ww => ww.SupplierId);
                            }
                            else
                            {
                                query = GetLstAssetDetails().Where(e => e.DepartmentId == data.SearchObj.DepartmentId).
                                OrderByDescending(ww => ww.SupplierId);
                            }
                            break;
                        case "Supplier":
                            if (data.SortObj.SortStatus == "ascending")
                            {

                                query = GetLstAssetDetails().Where(e => e.DepartmentId == data.SearchObj.DepartmentId).
                                  OrderBy(ww => ww.SupplierId);
                            }
                            else
                            {
                                query = GetLstAssetDetails().Where(e => e.DepartmentId == data.SearchObj.DepartmentId).
                          OrderByDescending(ww => ww.SupplierId);

                            }
                            break;
                        case "تاريخ الشراء":
                        case "Purchased Date":
                            if (data.SortObj.SortStatus == "ascending")
                            {
                                query = GetLstAssetDetails().Where(e => e.DepartmentId == data.SearchObj.DepartmentId).
                               OrderBy(ww => ww.PurchaseDate);
                            }
                            else
                            {
                                query = GetLstAssetDetails().Where(e => e.DepartmentId == data.SearchObj.DepartmentId).
                               OrderByDescending(ww => ww.PurchaseDate);
                            }
                            break;



                    }

                }

                mainClass.Count = query.Count();
                searchResult = query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();


            }

            if (data.SearchObj.BrandId != 0)
            {
                if (query is not null)
                {
                    switch (data.SortObj.SortBy)
                    {
                        case "الاسم":
                            if (data.SortObj.SortStatus == "ascending")
                            {
                                // filteration and sort then pagination 
                                // Where() , OrderBy() then Skip() Take()
                                query = query.Where(e => e.MasterAsset.BrandId == data.SearchObj.BrandId).
                                  OrderBy(ww => ww.MasterAsset.NameAr);
                            }
                            else
                            {
                                query = query.Where(e => e.MasterAsset.BrandId == data.SearchObj.BrandId).
                              OrderByDescending(ww => ww.MasterAsset.NameAr);
                            }
                            break;

                        case "Name":
                            if (data.SortObj.SortStatus == "ascending")
                            {
                                query = query.Where(e => e.MasterAsset.BrandId == data.SearchObj.BrandId).
                                OrderBy(ww => ww.MasterAsset.Name);

                            }
                            else
                            {

                                query = query.Where(e => e.MasterAsset.BrandId == data.SearchObj.BrandId).
                                    OrderByDescending(ww => ww.MasterAsset.Name);

                            }

                            break;

                        case "الباركود":
                        case "Barcode":

                            if (data.SortObj.SortStatus == "ascending")
                            {
                                query = query.Where(e => e.MasterAsset.BrandId == data.SearchObj.BrandId).
                                OrderBy(ww => ww.Barcode);


                            }
                            else
                            {
                                query = query.Where(e => e.MasterAsset.BrandId == data.SearchObj.BrandId).
                                  OrderByDescending(ww => ww.Barcode);

                            }

                            break;

                        case "السيريال":
                        case "Serial":
                            if (data.SortObj.SortStatus == "ascending")
                            {
                                query = query.Where(e => e.MasterAsset.BrandId == data.SearchObj.BrandId).
                                OrderBy(ww => ww.SerialNumber);

                            }
                            else
                            {
                                query = query.Where(e => e.MasterAsset.BrandId == data.SearchObj.BrandId).
                               OrderByDescending(ww => ww.SerialNumber);
                            }

                            break;

                        case "رقم الموديل":
                        case "Model Number":
                            if (data.SortObj.SortStatus == "ascending")
                            {
                                query = query.Where(e => e.MasterAsset.BrandId == data.SearchObj.BrandId).
                                     OrderBy(ww => ww.MasterAsset.ModelNumber);
                            }
                            else
                            {
                                query = query.Where(e => e.MasterAsset.BrandId == data.SearchObj.BrandId).
                                     OrderByDescending(ww => ww.MasterAsset.ModelNumber);
                            }
                            break;
                        case "القسم":
                            if (data.SortObj.SortStatus == "ascending")
                            {
                                query = query.Where(e => e.MasterAsset.BrandId == data.SearchObj.BrandId).
                                  OrderBy(ww => ww.DepartmentId);
                            }
                            else
                            {
                                query = query.Where(e => e.MasterAsset.BrandId == data.SearchObj.BrandId).
                                                OrderByDescending(ww => ww.DepartmentId);

                            }
                            break;


                        case "Department":
                            if (data.SortObj.SortStatus == "ascending")
                            {
                                query = query.Where(e => e.MasterAsset.BrandId == data.SearchObj.BrandId).
                                     OrderBy(ww => ww.DepartmentId);

                            }
                            else
                            {
                                query = query.Where(e => e.MasterAsset.BrandId == data.SearchObj.BrandId).
                                               OrderByDescending(ww => ww.DepartmentId);
                            }
                            break;

                        case "الماركة":
                            if (data.SortObj.SortStatus == "ascending")
                            {
                                query = query.Where(e => e.MasterAsset.BrandId == data.SearchObj.BrandId).
                                    OrderBy(ww => ww.MasterAsset.brand.NameAr);


                            }
                            else
                            {
                                query = query.Where(e => e.MasterAsset.BrandId == data.SearchObj.BrandId).
                                   OrderByDescending(ww => ww.MasterAsset.brand.NameAr);
                            }
                            break;
                        case "Manufacture":
                            if (data.SortObj.SortStatus == "ascending")

                            {


                                query = query.Where(e => e.MasterAsset.BrandId == data.SearchObj.BrandId).
                                  OrderBy(ww => ww.MasterAsset.brand.Name);


                            }
                            else
                            {
                                query = query.Where(e => e.MasterAsset.BrandId == data.SearchObj.BrandId).
                            OrderByDescending(ww => ww.MasterAsset.brand.Name);
                            }
                            break;
                        case "المورد":
                            if (data.SortObj.SortStatus == "ascending")
                            {

                                query = query.Where(e => e.MasterAsset.BrandId == data.SearchObj.BrandId).
                                  OrderBy(ww => ww.SupplierId);
                            }
                            else
                            {
                                query = query.Where(e => e.MasterAsset.BrandId == data.SearchObj.BrandId).
                                OrderByDescending(ww => ww.SupplierId);
                            }
                            break;
                        case "Supplier":
                            if (data.SortObj.SortStatus == "ascending")
                            {

                                query = query.Where(e => e.MasterAsset.BrandId == data.SearchObj.BrandId).
                                  OrderBy(ww => ww.SupplierId);
                            }
                            else
                            {
                                query = query.Where(e => e.MasterAsset.BrandId == data.SearchObj.BrandId).
                          OrderByDescending(ww => ww.SupplierId);

                            }
                            break;
                        case "تاريخ الشراء":
                        case "Purchased Date":
                            if (data.SortObj.SortStatus == "ascending")
                            {
                                query = query.Where(e => e.MasterAsset.BrandId == data.SearchObj.BrandId).
                               OrderBy(ww => ww.PurchaseDate);
                            }
                            else
                            {
                                query = query.Where(e => e.MasterAsset.NameAr == data.SearchObj.AssetNameAr).
                               OrderByDescending(ww => ww.PurchaseDate);
                            }
                            break;



                    }

                }

                else
                {
                    switch (data.SortObj.SortBy)
                    {
                        case "الاسم":
                            if (data.SortObj.SortStatus == "ascending")
                            {
                                // filteration and sort then pagination 
                                // Where() , OrderBy() then Skip() Take()
                                query = GetLstAssetDetails().Where(e => e.MasterAsset.BrandId == data.SearchObj.BrandId).
                                  OrderBy(ww => ww.MasterAsset.NameAr);
                            }
                            else
                            {
                                query = GetLstAssetDetails().Where(e => e.MasterAsset.BrandId == data.SearchObj.BrandId).
                              OrderByDescending(ww => ww.MasterAsset.NameAr);
                            }
                            break;

                        case "Name":
                            if (data.SortObj.SortStatus == "ascending")
                            {
                                query = GetLstAssetDetails().Where(e => e.MasterAsset.BrandId == data.SearchObj.BrandId).
                                OrderBy(ww => ww.MasterAsset.Name);

                            }
                            else
                            {

                                query = GetLstAssetDetails().Where(e => e.MasterAsset.BrandId == data.SearchObj.BrandId).
                                    OrderByDescending(ww => ww.MasterAsset.Name);

                            }

                            break;

                        case "الباركود":
                        case "Barcode":

                            if (data.SortObj.SortStatus == "ascending")
                            {
                                query = GetLstAssetDetails().Where(e => e.MasterAsset.BrandId == data.SearchObj.BrandId).
                                OrderBy(ww => ww.Barcode);


                            }
                            else
                            {
                                query = GetLstAssetDetails().Where(e => e.MasterAsset.BrandId == data.SearchObj.BrandId).
                                  OrderByDescending(ww => ww.Barcode);

                            }

                            break;

                        case "السيريال":
                        case "Serial":
                            if (data.SortObj.SortStatus == "ascending")
                            {
                                query = GetLstAssetDetails().Where(e => e.MasterAsset.BrandId == data.SearchObj.BrandId).
                                OrderBy(ww => ww.SerialNumber);

                            }
                            else
                            {
                                query = GetLstAssetDetails().Where(e => e.MasterAsset.BrandId == data.SearchObj.BrandId).
                               OrderByDescending(ww => ww.SerialNumber);
                            }

                            break;

                        case "رقم الموديل":
                        case "Model Number":
                            if (data.SortObj.SortStatus == "ascending")
                            {
                                query = GetLstAssetDetails().Where(e => e.MasterAsset.BrandId == data.SearchObj.BrandId).
                                     OrderBy(ww => ww.MasterAsset.ModelNumber);
                            }
                            else
                            {
                                query = GetLstAssetDetails().Where(e => e.MasterAsset.BrandId == data.SearchObj.BrandId).
                                     OrderByDescending(ww => ww.MasterAsset.ModelNumber);
                            }
                            break;
                        case "القسم":
                            if (data.SortObj.SortStatus == "ascending")
                            {
                                query = GetLstAssetDetails().Where(e => e.MasterAsset.BrandId == data.SearchObj.BrandId).
                                  OrderBy(ww => ww.DepartmentId);
                            }
                            else
                            {
                                query = GetLstAssetDetails().Where(e => e.MasterAsset.BrandId == data.SearchObj.BrandId).
                                                OrderByDescending(ww => ww.DepartmentId);

                            }
                            break;


                        case "Department":
                            if (data.SortObj.SortStatus == "ascending")
                            {
                                query = GetLstAssetDetails().Where(e => e.MasterAsset.BrandId == data.SearchObj.BrandId).
                                     OrderBy(ww => ww.DepartmentId);

                            }
                            else
                            {
                                query = GetLstAssetDetails().Where(e => e.MasterAsset.BrandId == data.SearchObj.BrandId).
                                               OrderByDescending(ww => ww.DepartmentId);
                            }
                            break;

                        case "الماركة":
                            if (data.SortObj.SortStatus == "ascending")
                            {
                                query = GetLstAssetDetails().Where(e => e.MasterAsset.BrandId == data.SearchObj.BrandId).
                                    OrderBy(ww => ww.MasterAsset.brand.NameAr);


                            }
                            else
                            {
                                query = GetLstAssetDetails().Where(e => e.MasterAsset.BrandId == data.SearchObj.BrandId).
                                   OrderByDescending(ww => ww.MasterAsset.brand.NameAr);
                            }
                            break;
                        case "Manufacture":
                            if (data.SortObj.SortStatus == "ascending")

                            {


                                query = GetLstAssetDetails().Where(e => e.MasterAsset.BrandId == data.SearchObj.BrandId).
                                  OrderBy(ww => ww.MasterAsset.brand.Name);


                            }
                            else
                            {
                                query = GetLstAssetDetails().Where(e => e.MasterAsset.BrandId == data.SearchObj.BrandId).
                            OrderByDescending(ww => ww.MasterAsset.brand.Name);
                            }
                            break;
                        case "المورد":
                            if (data.SortObj.SortStatus == "ascending")
                            {

                                query = GetLstAssetDetails().Where(e => e.MasterAsset.BrandId == data.SearchObj.BrandId).
                                  OrderBy(ww => ww.SupplierId);
                            }
                            else
                            {
                                query = GetLstAssetDetails().Where(e => e.MasterAsset.BrandId == data.SearchObj.BrandId).
                                OrderByDescending(ww => ww.SupplierId);
                            }
                            break;
                        case "Supplier":
                            if (data.SortObj.SortStatus == "ascending")
                            {

                                query = GetLstAssetDetails().Where(e => e.MasterAsset.BrandId == data.SearchObj.BrandId).
                                  OrderBy(ww => ww.SupplierId);
                            }
                            else
                            {
                                query = GetLstAssetDetails().Where(e => e.MasterAsset.BrandId == data.SearchObj.BrandId).
                          OrderByDescending(ww => ww.SupplierId);

                            }
                            break;
                        case "تاريخ الشراء":
                        case "Purchased Date":
                            if (data.SortObj.SortStatus == "ascending")
                            {
                                query = GetLstAssetDetails().Where(e => e.MasterAsset.BrandId == data.SearchObj.BrandId).
                               OrderBy(ww => ww.PurchaseDate);
                            }
                            else
                            {
                                query = GetLstAssetDetails().Where(e => e.MasterAsset.BrandId == data.SearchObj.BrandId).
                               OrderByDescending(ww => ww.PurchaseDate);
                            }
                            break;



                    }
                }

                mainClass.Count = query.Count();
                searchResult = query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();


            }

            if (data.SearchObj.SupplierId != 0)
            {
                if (query is not null)
                {
                    switch (data.SortObj.SortBy)
                    {
                        case "الاسم":
                            if (data.SortObj.SortStatus == "ascending")
                            {
                                // filteration and sort then pagination 
                                // Where() , OrderBy() then Skip() Take()
                                query = query.Where(e => e.SupplierId == data.SearchObj.SupplierId).
                                  OrderBy(ww => ww.MasterAsset.NameAr);
                            }
                            else
                            {
                                query = query.Where(e => e.SupplierId == data.SearchObj.SupplierId).
                              OrderByDescending(ww => ww.MasterAsset.NameAr);
                            }
                            break;

                        case "Name":
                            if (data.SortObj.SortStatus == "ascending")
                            {
                                query = query.Where(e => e.SupplierId == data.SearchObj.SupplierId).
                                OrderBy(ww => ww.MasterAsset.Name);

                            }
                            else
                            {

                                query = query.Where(e => e.SupplierId == data.SearchObj.SupplierId).
                                    OrderByDescending(ww => ww.MasterAsset.Name);

                            }

                            break;

                        case "الباركود":
                        case "Barcode":

                            if (data.SortObj.SortStatus == "ascending")
                            {
                                query = query.Where(e => e.SupplierId == data.SearchObj.SupplierId).
                                OrderBy(ww => ww.Barcode);


                            }
                            else
                            {
                                query = query.Where(e => e.SupplierId == data.SearchObj.SupplierId).
                                  OrderByDescending(ww => ww.Barcode);

                            }

                            break;

                        case "السيريال":
                        case "Serial":
                            if (data.SortObj.SortStatus == "ascending")
                            {
                                query = query.Where(e => e.SupplierId == data.SearchObj.SupplierId).
                                OrderBy(ww => ww.SerialNumber);

                            }
                            else
                            {
                                query = query.Where(e => e.SupplierId == data.SearchObj.SupplierId).
                               OrderByDescending(ww => ww.SerialNumber);
                            }

                            break;

                        case "رقم الموديل":
                        case "Model Number":
                            if (data.SortObj.SortStatus == "ascending")
                            {
                                query = query.Where(e => e.SupplierId == data.SearchObj.SupplierId).
                                     OrderBy(ww => ww.MasterAsset.ModelNumber);
                            }
                            else
                            {
                                query = query.Where(e => e.SupplierId == data.SearchObj.SupplierId).
                                     OrderByDescending(ww => ww.MasterAsset.ModelNumber);
                            }
                            break;
                        case "القسم":
                            if (data.SortObj.SortStatus == "ascending")
                            {
                                query = query.Where(e => e.SupplierId == data.SearchObj.SupplierId).
                                  OrderBy(ww => ww.DepartmentId);
                            }
                            else
                            {
                                query = query.Where(e => e.SupplierId == data.SearchObj.SupplierId).
                                                OrderByDescending(ww => ww.DepartmentId);

                            }
                            break;


                        case "Department":
                            if (data.SortObj.SortStatus == "ascending")
                            {
                                query = query.Where(e => e.SupplierId == data.SearchObj.SupplierId).
                                     OrderBy(ww => ww.DepartmentId);

                            }
                            else
                            {
                                query = query.Where(e => e.SupplierId == data.SearchObj.SupplierId).
                                               OrderByDescending(ww => ww.DepartmentId);
                            }
                            break;

                        case "الماركة":
                            if (data.SortObj.SortStatus == "ascending")
                            {
                                query = query.Where(e => e.SupplierId == data.SearchObj.SupplierId).
                                    OrderBy(ww => ww.MasterAsset.brand.NameAr);


                            }
                            else
                            {
                                query = query.Where(e => e.SupplierId == data.SearchObj.SupplierId).
                                   OrderByDescending(ww => ww.MasterAsset.brand.NameAr);
                            }
                            break;
                        case "Manufacture":
                            if (data.SortObj.SortStatus == "ascending")

                            {


                                query = query.Where(e => e.SupplierId == data.SearchObj.SupplierId).
                                  OrderBy(ww => ww.MasterAsset.brand.Name);


                            }
                            else
                            {
                                query = query.Where(e => e.SupplierId == data.SearchObj.SupplierId).
                            OrderByDescending(ww => ww.MasterAsset.brand.Name);
                            }
                            break;
                        case "المورد":
                            if (data.SortObj.SortStatus == "ascending")
                            {

                                query = query.Where(e => e.SupplierId == data.SearchObj.SupplierId).
                                  OrderBy(ww => ww.SupplierId);
                            }
                            else
                            {
                                query = query.Where(e => e.SupplierId == data.SearchObj.SupplierId).
                                OrderByDescending(ww => ww.SupplierId);
                            }
                            break;
                        case "Supplier":
                            if (data.SortObj.SortStatus == "ascending")
                            {

                                query = query.Where(e => e.SupplierId == data.SearchObj.SupplierId).
                                  OrderBy(ww => ww.SupplierId);
                            }
                            else
                            {
                                query = query.Where(e => e.SupplierId == data.SearchObj.SupplierId).
                          OrderByDescending(ww => ww.SupplierId);

                            }
                            break;
                        case "تاريخ الشراء":
                        case "Purchased Date":
                            if (data.SortObj.SortStatus == "ascending")
                            {
                                query = query.Where(e => e.SupplierId == data.SearchObj.SupplierId).
                               OrderBy(ww => ww.PurchaseDate);
                            }
                            else
                            {
                                query = query.Where(e => e.SupplierId == data.SearchObj.SupplierId).
                               OrderByDescending(ww => ww.PurchaseDate);
                            }
                            break;



                    }

                }
                else
                {
                    switch (data.SortObj.SortBy)
                    {
                        case "الاسم":
                            if (data.SortObj.SortStatus == "ascending")
                            {
                                // filteration and sort then pagination 
                                // Where() , OrderBy() then Skip() Take()
                                query = GetLstAssetDetails().Where(e => e.SupplierId == data.SearchObj.SupplierId).
                                  OrderBy(ww => ww.MasterAsset.NameAr);
                            }
                            else
                            {
                                query = GetLstAssetDetails().Where(e => e.SupplierId == data.SearchObj.SupplierId).
                              OrderByDescending(ww => ww.MasterAsset.NameAr);
                            }
                            break;

                        case "Name":
                            if (data.SortObj.SortStatus == "ascending")
                            {
                                query = GetLstAssetDetails().Where(e => e.SupplierId == data.SearchObj.SupplierId).
                                OrderBy(ww => ww.MasterAsset.Name);

                            }
                            else
                            {

                                query = GetLstAssetDetails().Where(e => e.SupplierId == data.SearchObj.SupplierId).
                                    OrderByDescending(ww => ww.MasterAsset.Name);

                            }

                            break;

                        case "الباركود":
                        case "Barcode":

                            if (data.SortObj.SortStatus == "ascending")
                            {
                                query = GetLstAssetDetails().Where(e => e.SupplierId == data.SearchObj.SupplierId).
                                OrderBy(ww => ww.Barcode);


                            }
                            else
                            {
                                query = GetLstAssetDetails().Where(e => e.SupplierId == data.SearchObj.SupplierId).
                                  OrderByDescending(ww => ww.Barcode);

                            }

                            break;

                        case "السيريال":
                        case "Serial":
                            if (data.SortObj.SortStatus == "ascending")
                            {
                                query = GetLstAssetDetails().Where(e => e.SupplierId == data.SearchObj.SupplierId).
                                OrderBy(ww => ww.SerialNumber);

                            }
                            else
                            {
                                query = GetLstAssetDetails().Where(e => e.SupplierId == data.SearchObj.SupplierId).
                               OrderByDescending(ww => ww.SerialNumber);
                            }

                            break;

                        case "رقم الموديل":
                        case "Model Number":
                            if (data.SortObj.SortStatus == "ascending")
                            {
                                query = GetLstAssetDetails().Where(e => e.SupplierId == data.SearchObj.SupplierId).
                                     OrderBy(ww => ww.MasterAsset.ModelNumber);
                            }
                            else
                            {
                                query = GetLstAssetDetails().Where(e => e.SupplierId == data.SearchObj.SupplierId).
                                     OrderByDescending(ww => ww.MasterAsset.ModelNumber);
                            }
                            break;
                        case "القسم":
                            if (data.SortObj.SortStatus == "ascending")
                            {
                                query = GetLstAssetDetails().Where(e => e.SupplierId == data.SearchObj.SupplierId).
                                  OrderBy(ww => ww.DepartmentId);
                            }
                            else
                            {
                                query = GetLstAssetDetails().Where(e => e.SupplierId == data.SearchObj.SupplierId).
                                                OrderByDescending(ww => ww.DepartmentId);

                            }
                            break;


                        case "Department":
                            if (data.SortObj.SortStatus == "ascending")
                            {
                                query = GetLstAssetDetails().Where(e => e.SupplierId == data.SearchObj.SupplierId).
                                     OrderBy(ww => ww.DepartmentId);

                            }
                            else
                            {
                                query = GetLstAssetDetails().Where(e => e.SupplierId == data.SearchObj.SupplierId).
                                               OrderByDescending(ww => ww.DepartmentId);
                            }
                            break;

                        case "الماركة":
                            if (data.SortObj.SortStatus == "ascending")
                            {
                                query = GetLstAssetDetails().Where(e => e.SupplierId == data.SearchObj.SupplierId).
                                    OrderBy(ww => ww.MasterAsset.brand.NameAr);


                            }
                            else
                            {
                                query = GetLstAssetDetails().Where(e => e.SupplierId == data.SearchObj.SupplierId).
                                   OrderByDescending(ww => ww.MasterAsset.brand.NameAr);
                            }
                            break;
                        case "Manufacture":
                            if (data.SortObj.SortStatus == "ascending")

                            {


                                query = GetLstAssetDetails().Where(e => e.SupplierId == data.SearchObj.SupplierId).
                                  OrderBy(ww => ww.MasterAsset.brand.Name);


                            }
                            else
                            {
                                query = GetLstAssetDetails().Where(e => e.SupplierId == data.SearchObj.SupplierId).
                            OrderByDescending(ww => ww.MasterAsset.brand.Name);
                            }
                            break;
                        case "المورد":
                            if (data.SortObj.SortStatus == "ascending")
                            {

                                query = GetLstAssetDetails().Where(e => e.SupplierId == data.SearchObj.SupplierId).
                                  OrderBy(ww => ww.SupplierId);
                            }
                            else
                            {
                                query = GetLstAssetDetails().Where(e => e.SupplierId == data.SearchObj.SupplierId).
                                OrderByDescending(ww => ww.SupplierId);
                            }
                            break;
                        case "Supplier":
                            if (data.SortObj.SortStatus == "ascending")
                            {

                                query = GetLstAssetDetails().Where(e => e.SupplierId == data.SearchObj.SupplierId).
                                  OrderBy(ww => ww.SupplierId);
                            }
                            else
                            {
                                query = GetLstAssetDetails().Where(e => e.SupplierId == data.SearchObj.SupplierId).
                          OrderByDescending(ww => ww.SupplierId);

                            }
                            break;
                        case "تاريخ الشراء":
                        case "Purchased Date":
                            if (data.SortObj.SortStatus == "ascending")
                            {
                                query = GetLstAssetDetails().Where(e => e.SupplierId == data.SearchObj.SupplierId).
                               OrderBy(ww => ww.PurchaseDate);
                            }
                            else
                            {
                                query = GetLstAssetDetails().Where(e => e.SupplierId == data.SearchObj.SupplierId).
                               OrderByDescending(ww => ww.PurchaseDate);
                            }
                            break;



                    }

                }
                mainClass.Count = query.Count();
                searchResult = query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            }

            if (data.SearchObj.MasterAssetId != 0)
            {
                if (query is not null)
                {
                    switch (data.SortObj.SortBy)
                    {
                        case "الاسم":
                            if (data.SortObj.SortStatus == "ascending")
                            {
                                // filteration and sort then pagination 
                                // Where() , OrderBy() then Skip() Take()
                                query = query.Where(e => e.MasterAssetId == data.SearchObj.MasterAssetId).
                                  OrderBy(ww => ww.MasterAsset.NameAr);
                            }
                            else
                            {
                                query = query.Where(e => e.MasterAssetId == data.SearchObj.MasterAssetId).
                              OrderByDescending(ww => ww.MasterAsset.NameAr);
                            }
                            break;

                        case "Name":
                            if (data.SortObj.SortStatus == "ascending")
                            {
                                query = query.Where(e => e.MasterAssetId == data.SearchObj.MasterAssetId).
                                OrderBy(ww => ww.MasterAsset.Name);

                            }
                            else
                            {

                                query = query.Where(e => e.MasterAssetId == data.SearchObj.MasterAssetId).
                                    OrderByDescending(ww => ww.MasterAsset.Name);

                            }

                            break;

                        case "الباركود":
                        case "Barcode":

                            if (data.SortObj.SortStatus == "ascending")
                            {
                                query = query.Where(e => e.MasterAssetId == data.SearchObj.MasterAssetId).
                                OrderBy(ww => ww.Barcode);


                            }
                            else
                            {
                                query = query.Where(e => e.MasterAssetId == data.SearchObj.MasterAssetId).
                                  OrderByDescending(ww => ww.Barcode);

                            }

                            break;

                        case "السيريال":
                        case "Serial":
                            if (data.SortObj.SortStatus == "ascending")
                            {
                                query = query.Where(e => e.MasterAssetId == data.SearchObj.MasterAssetId).
                                OrderBy(ww => ww.SerialNumber);

                            }
                            else
                            {
                                query = query.Where(e => e.MasterAssetId == data.SearchObj.MasterAssetId).
                               OrderByDescending(ww => ww.SerialNumber);
                            }

                            break;

                        case "رقم الموديل":
                        case "Model Number":
                            if (data.SortObj.SortStatus == "ascending")
                            {
                                query = query.Where(e => e.MasterAssetId == data.SearchObj.MasterAssetId).
                                     OrderBy(ww => ww.MasterAsset.ModelNumber);
                            }
                            else
                            {
                                query = query.Where(e => e.MasterAssetId == data.SearchObj.MasterAssetId).
                                     OrderByDescending(ww => ww.MasterAsset.ModelNumber);
                            }
                            break;
                        case "القسم":
                            if (data.SortObj.SortStatus == "ascending")
                            {
                                query = query.Where(e => e.MasterAssetId == data.SearchObj.MasterAssetId).
                                  OrderBy(ww => ww.DepartmentId);
                            }
                            else
                            {
                                query = query.Where(e => e.MasterAssetId == data.SearchObj.MasterAssetId).
                                                OrderByDescending(ww => ww.DepartmentId);

                            }
                            break;


                        case "Department":
                            if (data.SortObj.SortStatus == "ascending")
                            {
                                query = query.Where(e => e.MasterAssetId == data.SearchObj.MasterAssetId).
                                     OrderBy(ww => ww.DepartmentId);

                            }
                            else
                            {
                                query = query.Where(e => e.MasterAssetId == data.SearchObj.MasterAssetId).
                                               OrderByDescending(ww => ww.DepartmentId);
                            }
                            break;

                        case "الماركة":
                            if (data.SortObj.SortStatus == "ascending")
                            {
                                query = query.Where(e => e.MasterAssetId == data.SearchObj.MasterAssetId).
                                    OrderBy(ww => ww.MasterAsset.brand.NameAr);


                            }
                            else
                            {
                                query = query.Where(e => e.MasterAssetId == data.SearchObj.MasterAssetId).
                                   OrderByDescending(ww => ww.MasterAsset.brand.NameAr);
                            }
                            break;
                        case "Manufacture":
                            if (data.SortObj.SortStatus == "ascending")

                            {


                                query = query.Where(e => e.MasterAssetId == data.SearchObj.MasterAssetId).
                                  OrderBy(ww => ww.MasterAsset.brand.Name);


                            }
                            else
                            {
                                query = query.Where(e => e.MasterAssetId == data.SearchObj.MasterAssetId).
                            OrderByDescending(ww => ww.MasterAsset.brand.Name);
                            }
                            break;
                        case "المورد":
                            if (data.SortObj.SortStatus == "ascending")
                            {

                                query = query.Where(e => e.MasterAssetId == data.SearchObj.MasterAssetId).
                                  OrderBy(ww => ww.SupplierId);
                            }
                            else
                            {
                                query = query.Where(e => e.MasterAssetId == data.SearchObj.MasterAssetId).
                                OrderByDescending(ww => ww.SupplierId);
                            }
                            break;
                        case "Supplier":
                            if (data.SortObj.SortStatus == "ascending")
                            {

                                query = query.Where(e => e.MasterAssetId == data.SearchObj.MasterAssetId).
                                  OrderBy(ww => ww.SupplierId);
                            }
                            else
                            {
                                query = query.Where(e => e.MasterAssetId == data.SearchObj.MasterAssetId).
                          OrderByDescending(ww => ww.SupplierId);

                            }
                            break;
                        case "تاريخ الشراء":
                        case "Purchased Date":
                            if (data.SortObj.SortStatus == "ascending")
                            {
                                query = query.Where(e => e.MasterAssetId == data.SearchObj.MasterAssetId).
                               OrderBy(ww => ww.PurchaseDate);
                            }
                            else
                            {
                                query = query.Where(e => e.MasterAssetId == data.SearchObj.MasterAssetId).
                               OrderByDescending(ww => ww.PurchaseDate);
                            }
                            break;



                    }
                }
                else
                {
                    switch (data.SortObj.SortBy)
                    {
                        case "الاسم":
                            if (data.SortObj.SortStatus == "ascending")
                            {
                                // filteration and sort then pagination 
                                // Where() , OrderBy() then Skip() Take()
                                query = GetLstAssetDetails().Where(e => e.MasterAssetId == data.SearchObj.MasterAssetId).
                                  OrderBy(ww => ww.MasterAsset.NameAr);
                            }
                            else
                            {
                                query = GetLstAssetDetails().Where(e => e.MasterAssetId == data.SearchObj.MasterAssetId).
                              OrderByDescending(ww => ww.MasterAsset.NameAr);
                            }
                            break;

                        case "Name":
                            if (data.SortObj.SortStatus == "ascending")
                            {
                                query = GetLstAssetDetails().Where(e => e.MasterAssetId == data.SearchObj.MasterAssetId).
                                OrderBy(ww => ww.MasterAsset.Name);

                            }
                            else
                            {

                                query = GetLstAssetDetails().Where(e => e.MasterAssetId == data.SearchObj.MasterAssetId).
                                    OrderByDescending(ww => ww.MasterAsset.Name);

                            }

                            break;

                        case "الباركود":
                        case "Barcode":

                            if (data.SortObj.SortStatus == "ascending")
                            {
                                query = GetLstAssetDetails().Where(e => e.MasterAssetId == data.SearchObj.MasterAssetId).
                                OrderBy(ww => ww.Barcode);


                            }
                            else
                            {
                                query = GetLstAssetDetails().Where(e => e.MasterAssetId == data.SearchObj.MasterAssetId).
                                  OrderByDescending(ww => ww.Barcode);

                            }

                            break;

                        case "السيريال":
                        case "Serial":
                            if (data.SortObj.SortStatus == "ascending")
                            {
                                query = GetLstAssetDetails().Where(e => e.MasterAssetId == data.SearchObj.MasterAssetId).
                                OrderBy(ww => ww.SerialNumber);

                            }
                            else
                            {
                                query = GetLstAssetDetails().Where(e => e.MasterAssetId == data.SearchObj.MasterAssetId).
                               OrderByDescending(ww => ww.SerialNumber);
                            }

                            break;

                        case "رقم الموديل":
                        case "Model Number":
                            if (data.SortObj.SortStatus == "ascending")
                            {
                                query = GetLstAssetDetails().Where(e => e.MasterAssetId == data.SearchObj.MasterAssetId).
                                     OrderBy(ww => ww.MasterAsset.ModelNumber);
                            }
                            else
                            {
                                query = GetLstAssetDetails().Where(e => e.MasterAssetId == data.SearchObj.MasterAssetId).
                                     OrderByDescending(ww => ww.MasterAsset.ModelNumber);
                            }
                            break;
                        case "القسم":
                            if (data.SortObj.SortStatus == "ascending")
                            {
                                query = GetLstAssetDetails().Where(e => e.MasterAssetId == data.SearchObj.MasterAssetId).
                                  OrderBy(ww => ww.DepartmentId);
                            }
                            else
                            {
                                query = GetLstAssetDetails().Where(e => e.MasterAssetId == data.SearchObj.MasterAssetId).
                                                OrderByDescending(ww => ww.DepartmentId);

                            }
                            break;


                        case "Department":
                            if (data.SortObj.SortStatus == "ascending")
                            {
                                query = GetLstAssetDetails().Where(e => e.MasterAssetId == data.SearchObj.MasterAssetId).
                                     OrderBy(ww => ww.DepartmentId);

                            }
                            else
                            {
                                query = GetLstAssetDetails().Where(e => e.MasterAssetId == data.SearchObj.MasterAssetId).
                                               OrderByDescending(ww => ww.DepartmentId);
                            }
                            break;

                        case "الماركة":
                            if (data.SortObj.SortStatus == "ascending")
                            {
                                query = GetLstAssetDetails().Where(e => e.MasterAssetId == data.SearchObj.MasterAssetId).
                                    OrderBy(ww => ww.MasterAsset.brand.NameAr);


                            }
                            else
                            {
                                query = GetLstAssetDetails().Where(e => e.MasterAssetId == data.SearchObj.MasterAssetId).
                                   OrderByDescending(ww => ww.MasterAsset.brand.NameAr);
                            }
                            break;
                        case "Manufacture":
                            if (data.SortObj.SortStatus == "ascending")

                            {


                                query = GetLstAssetDetails().Where(e => e.MasterAssetId == data.SearchObj.MasterAssetId).
                                  OrderBy(ww => ww.MasterAsset.brand.Name);


                            }
                            else
                            {
                                query = GetLstAssetDetails().Where(e => e.MasterAssetId == data.SearchObj.MasterAssetId).
                            OrderByDescending(ww => ww.MasterAsset.brand.Name);
                            }
                            break;
                        case "المورد":
                            if (data.SortObj.SortStatus == "ascending")
                            {

                                query = GetLstAssetDetails().Where(e => e.MasterAssetId == data.SearchObj.MasterAssetId).
                                  OrderBy(ww => ww.SupplierId);
                            }
                            else
                            {
                                query = GetLstAssetDetails().Where(e => e.MasterAssetId == data.SearchObj.MasterAssetId).
                                OrderByDescending(ww => ww.SupplierId);
                            }
                            break;
                        case "Supplier":
                            if (data.SortObj.SortStatus == "ascending")
                            {

                                query = GetLstAssetDetails().Where(e => e.MasterAssetId == data.SearchObj.MasterAssetId).
                                  OrderBy(ww => ww.SupplierId);
                            }
                            else
                            {
                                query = GetLstAssetDetails().Where(e => e.MasterAssetId == data.SearchObj.MasterAssetId).
                          OrderByDescending(ww => ww.SupplierId);

                            }
                            break;
                        case "تاريخ الشراء":
                        case "Purchased Date":
                            if (data.SortObj.SortStatus == "ascending")
                            {
                                query = GetLstAssetDetails().Where(e => e.MasterAssetId == data.SearchObj.MasterAssetId).
                               OrderBy(ww => ww.PurchaseDate);
                            }
                            else
                            {
                                query = GetLstAssetDetails().Where(e => e.MasterAssetId == data.SearchObj.MasterAssetId).
                               OrderByDescending(ww => ww.PurchaseDate);
                            }
                            break;



                    }
                }
                mainClass.Count = query.Count();

                searchResult = query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            }

            if (data.SearchObj.StatusId != 0)
            {
                if (query is not null)
                {
                    var q2 = query.ToList();
                    var lstOfIdsByStatus = _context.Set<AssetStatusTransaction>()
                        .FromSqlRaw("EXEC GetAssetStatusTransationsWithLastDate @AssetStatusId"
                        , new SqlParameter("@AssetStatusId", data.SearchObj.StatusId)).ToList();
                    // make join between Lst Of Assets After Search and Lst Of AssetSByIts Status
                    // For Example List Of Assets After Search And  List Of Working Assets
                    var res = from q in q2
                              join lst in lstOfIdsByStatus on q.Id equals lst.AssetDetailId
                              select q;
                    mainClass.Count = res.Count();


                    switch (data.SortObj.SortBy)
                    {
                        case "الاسم":
                            if (data.SortObj.SortStatus == "ascending")
                            {
                                // filteration and sort then pagination 
                                // Where() , OrderBy() then Skip() Take()
                                res = res.OrderBy(ww => ww.MasterAsset.NameAr);
                            }
                            else
                            {
                                res = res.OrderByDescending(ww => ww.MasterAsset.NameAr);
                            }
                            break;

                        case "Name":
                            if (data.SortObj.SortStatus == "ascending")
                            {
                                res = res.OrderBy(ww => ww.MasterAsset.Name);

                            }
                            else
                            {

                                res = res.OrderByDescending(ww => ww.MasterAsset.Name);

                            }

                            break;

                        case "الباركود":
                        case "Barcode":

                            if (data.SortObj.SortStatus == "ascending")
                            {
                                res = res.OrderBy(ww => ww.Barcode);


                            }
                            else
                            {
                                res = res.OrderByDescending(ww => ww.Barcode);

                            }

                            break;

                        case "السيريال":
                        case "Serial":
                            if (data.SortObj.SortStatus == "ascending")
                            {
                                res = res.OrderBy(ww => ww.SerialNumber);

                            }
                            else
                            {
                                res = res.OrderByDescending(ww => ww.SerialNumber);
                            }

                            break;

                        case "رقم الموديل":
                        case "Model Number":
                            if (data.SortObj.SortStatus == "ascending")
                            {
                                res = res.OrderBy(ww => ww.MasterAsset.ModelNumber);
                            }
                            else
                            {
                                res = res.OrderByDescending(ww => ww.MasterAsset.ModelNumber);
                            }
                            break;
                        case "القسم":
                            if (data.SortObj.SortStatus == "ascending")
                            {
                                res = res.OrderBy(ww => ww.DepartmentId);
                            }
                            else
                            {
                                res = res.OrderByDescending(ww => ww.DepartmentId);

                            }
                            break;


                        case "Department":
                            if (data.SortObj.SortStatus == "ascending")
                            {
                                res = res.OrderBy(ww => ww.DepartmentId);

                            }
                            else
                            {
                                res = res.OrderByDescending(ww => ww.DepartmentId);
                            }
                            break;

                        case "الماركة":
                            if (data.SortObj.SortStatus == "ascending")
                            {
                                res = res.OrderBy(ww => ww.MasterAsset.brand.NameAr);


                            }
                            else
                            {
                                res = res.OrderByDescending(ww => ww.MasterAsset.brand.NameAr);
                            }
                            break;
                        case "Manufacture":
                            if (data.SortObj.SortStatus == "ascending")

                            {


                                res = res.OrderBy(ww => ww.MasterAsset.brand.Name);


                            }
                            else
                            {
                                res = res.OrderByDescending(ww => ww.MasterAsset.brand.Name);
                            }
                            break;
                        case "المورد":
                            if (data.SortObj.SortStatus == "ascending")
                            {

                                res = res.OrderBy(ww => ww.SupplierId);
                            }
                            else
                            {
                                res = res.OrderByDescending(ww => ww.SupplierId);
                            }
                            break;
                        case "Supplier":
                            if (data.SortObj.SortStatus == "ascending")
                            {
                                res = res.OrderBy(ww => ww.SupplierId);
                            }
                            else
                            {
                                res = res.OrderByDescending(ww => ww.SupplierId);

                            }
                            break;
                        case "تاريخ الشراء":
                        case "Purchased Date":
                            if (data.SortObj.SortStatus == "ascending")
                            {
                                res = res.OrderBy(ww => ww.PurchaseDate);
                            }
                            else
                            {
                                res = res.OrderByDescending(ww => ww.PurchaseDate);
                            }
                            break;



                    }



                    searchResult = res.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();



                }
                else
                {


                    var lstAssetsByStatus = _context.Set<AssetDetail>()
                    .FromSqlRaw("EXEC SP_GetAssetsByStatus @AssetStatusId"
                    , new SqlParameter("@AssetStatusId", data.SearchObj.StatusId)).ToList();
                    mainClass.Count = lstAssetsByStatus.Count;

                    switch (data.SortObj.SortBy)
                    {
                        case "الاسم":
                            if (data.SortObj.SortStatus == "ascending")
                            {
                                // filteration and sort then pagination 
                                // Where() , OrderBy() then Skip() Take()
                                lstAssetsByStatus = lstAssetsByStatus.OrderBy(ww => ww.MasterAsset.NameAr).ToList();
                            }
                            else
                            {
                                lstAssetsByStatus = lstAssetsByStatus.OrderByDescending(ww => ww.MasterAsset.NameAr).ToList();
                            }
                            break;

                        case "Name":
                            if (data.SortObj.SortStatus == "ascending")
                            {
                                lstAssetsByStatus = lstAssetsByStatus.OrderBy(ww => ww.MasterAsset.Name).ToList();

                            }
                            else
                            {

                                lstAssetsByStatus = lstAssetsByStatus.OrderByDescending(ww => ww.MasterAsset.Name).ToList();

                            }

                            break;

                        case "الباركود":
                        case "Barcode":

                            if (data.SortObj.SortStatus == "ascending")
                            {
                                lstAssetsByStatus = lstAssetsByStatus.OrderBy(ww => ww.Barcode).ToList();


                            }
                            else
                            {
                                lstAssetsByStatus = lstAssetsByStatus.OrderByDescending(ww => ww.Barcode).ToList();

                            }

                            break;

                        case "السيريال":
                        case "Serial":
                            if (data.SortObj.SortStatus == "ascending")
                            {
                                lstAssetsByStatus = lstAssetsByStatus.OrderBy(ww => ww.SerialNumber).ToList();

                            }
                            else
                            {
                                lstAssetsByStatus = lstAssetsByStatus.OrderByDescending(ww => ww.SerialNumber).ToList();
                            }

                            break;

                        case "رقم الموديل":
                        case "Model Number":
                            if (data.SortObj.SortStatus == "ascending")
                            {
                                lstAssetsByStatus = lstAssetsByStatus.OrderBy(ww => ww.MasterAsset.ModelNumber).ToList();
                            }
                            else
                            {

                                lstAssetsByStatus = lstAssetsByStatus.OrderByDescending(ww => ww.MasterAsset.ModelNumber).ToList();

                            }
                            break;
                        case "القسم":
                            if (data.SortObj.SortStatus == "ascending")
                            {

                                lstAssetsByStatus = lstAssetsByStatus.OrderBy(ww => ww.DepartmentId).ToList();

                            }
                            else
                            {
                                lstAssetsByStatus = lstAssetsByStatus.OrderByDescending(ww => ww.DepartmentId).ToList();

                            }
                            break;


                        case "Department":
                            if (data.SortObj.SortStatus == "ascending")
                            {
                                lstAssetsByStatus = lstAssetsByStatus.OrderBy(ww => ww.DepartmentId).ToList();

                            }
                            else
                            {
                                lstAssetsByStatus = lstAssetsByStatus.OrderByDescending(ww => ww.DepartmentId).ToList();
                            }
                            break;

                        case "الماركة":
                            if (data.SortObj.SortStatus == "ascending")
                            {
                                lstAssetsByStatus = lstAssetsByStatus.OrderBy(ww => ww.MasterAsset.brand.NameAr).ToList();


                            }
                            else
                            {
                                lstAssetsByStatus = lstAssetsByStatus.OrderByDescending(ww => ww.MasterAsset.brand.NameAr).ToList();
                            }
                            break;
                        case "Manufacture":
                            if (data.SortObj.SortStatus == "ascending")

                            {


                                lstAssetsByStatus = lstAssetsByStatus.OrderBy(ww => ww.MasterAsset.brand.Name).ToList();

                            }
                            else
                            {
                                lstAssetsByStatus = lstAssetsByStatus.OrderByDescending(ww => ww.MasterAsset.brand.Name).ToList();
                            }
                            break;
                        case "المورد":
                            if (data.SortObj.SortStatus == "ascending")
                            {
                                lstAssetsByStatus = lstAssetsByStatus.OrderBy(ww => ww.SupplierId).ToList();


                            }
                            else
                            {
                                lstAssetsByStatus = lstAssetsByStatus.OrderByDescending(ww => ww.SupplierId).ToList();

                            }
                            break;
                        case "Supplier":
                            if (data.SortObj.SortStatus == "ascending")
                            {
                                lstAssetsByStatus = lstAssetsByStatus.OrderBy(ww => ww.SupplierId).ToList();

                            }
                            else
                            {

                                lstAssetsByStatus = lstAssetsByStatus.OrderByDescending(ww => ww.SupplierId).ToList();


                            }
                            break;
                        case "تاريخ الشراء":
                        case "Purchased Date":
                            if (data.SortObj.SortStatus == "ascending")
                            {
                                lstAssetsByStatus = lstAssetsByStatus.OrderBy(ww => ww.PurchaseDate).ToList();

                            }
                            else
                            {
                                lstAssetsByStatus = lstAssetsByStatus.OrderByDescending(ww => ww.PurchaseDate).ToList();

                            }
                            break;



                    }


                    lstAssetsByStatus = lstAssetsByStatus.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

                    foreach (var item in lstAssetsByStatus)
                    {

                        IndexAssetDetailVM.GetData getDataobj = new IndexAssetDetailVM.GetData();
                        getDataobj.Id = item.Id;
                        getDataobj.Code = item.Code;
                        getDataobj.Price = item.Price;
                        getDataobj.Barcode = item.Barcode;
                        getDataobj.Serial = item.SerialNumber;
                        getDataobj.SerialNumber = item.SerialNumber;
                        getDataobj.Model = _context.MasterAssets.Where(x => x.Id == item.MasterAssetId).FirstOrDefault().ModelNumber;
                        getDataobj.MasterAssetId = item.MasterAssetId;
                        getDataobj.PurchaseDate = item.PurchaseDate;
                        getDataobj.HospitalId = item.HospitalId;
                        getDataobj.DepartmentId = item.DepartmentId;
                        getDataobj.HospitalName = _context.Hospitals.Where(x => x.Id == item.HospitalId).FirstOrDefault().Name;
                        getDataobj.HospitalNameAr = item.Hospital.NameAr;
                        getDataobj.AssetName = item.MasterAsset.Name;
                        getDataobj.AssetNameAr = item.MasterAsset.NameAr;
                        if (item.HospitalId != 0)
                        {

                            getDataobj.GovernorateId = _context.Governorates.Where(x => x.Id == item.Hospital.GovernorateId).FirstOrDefault().Id;
                            getDataobj.GovernorateName = item.Hospital.Governorate.Name;
                            getDataobj.GovernorateName = item.Hospital.Governorate.NameAr;
                            getDataobj.CityId = _context.Cities.Where(x => x.Id == item.Hospital.CityId).FirstOrDefault().Id;
                            getDataobj.CityName = item.Hospital.City.Name;
                            getDataobj.CityNameAr = item.Hospital.City.NameAr;
                            getDataobj.OrganizationId = _context.Organizations.Where(x => x.Id == item.Hospital.OrganizationId).FirstOrDefault().Id;
                            getDataobj.OrgName = item.Hospital.Organization.Name;
                            getDataobj.OrgNameAr = item.Hospital.Organization.NameAr;
                            getDataobj.SubOrgName = _context.SubOrganizations.Where(x => x.Id == item.Hospital.SubOrganizationId).FirstOrDefault().Name;
                            getDataobj.SubOrgNameAr = item.Hospital.SubOrganization.NameAr;
                            getDataobj.SubOrganizationId = item.Hospital.SubOrganization.Id;
                        }
                        if (item.MasterAssetId != 0)
                        {
                            var masterObj = _context.MasterAssets.Where(x => x.Id == item.MasterAssetId).AsNoTracking().FirstOrDefault();
                            if (masterObj.BrandId != null || masterObj.BrandId != 0)
                            {
                                getDataobj.BrandId = _context.Brands.Where(x => x.Id == masterObj.BrandId).FirstOrDefault().Id;
                                getDataobj.BrandName = item.MasterAsset.brand.Name;
                                getDataobj.BrandNameAr = item.MasterAsset.brand.NameAr;
                            }
                        }
                        if (item.SupplierId != 0)
                        {
                            var supplierObj = _context.Suppliers.Where(x => x.Id == item.SupplierId).FirstOrDefault() ?? null;
                            if (supplierObj is not null)
                            {
                                getDataobj.SupplierId = supplierObj.Id;
                                getDataobj.SupplierName = supplierObj.Name;
                                getDataobj.SupplierNameAr = supplierObj.NameAr;

                            }
                            else
                            {
                                getDataobj.SupplierId = 0;
                                getDataobj.SupplierName = "";
                                getDataobj.SupplierNameAr = "";
                            }


                        }
                        if (item.DepartmentId != 0)
                        {
                            var deptObj = _context.Departments.Where(x => x.Id == item.DepartmentId).FirstOrDefault() ?? null;
                            if (deptObj is not null)
                            {
                                getDataobj.DepartmentName = deptObj.Name;
                                getDataobj.DepartmentNameAr = deptObj.NameAr;
                            }
                            else
                            {
                                getDataobj.DepartmentId = 0;
                                getDataobj.DepartmentName = "";
                            }

                        }


                        var lstTransactions = _context.AssetStatusTransactions.Where(a => a.AssetDetailId == item.Id).OrderByDescending(a => a.StatusDate.Value.Date).ToList();
                        if (lstTransactions.Count > 0)
                        {
                            getDataobj.AssetStatusId = lstTransactions[0].AssetStatusId;
                        }

                        list.Add(getDataobj);
                    }




                }



            }

            else
            {
                searchResult = searchResult.ToList();
            }
            string setstartday, setstartmonth, setendday, setendmonth = "";
            DateTime? startingFrom = new DateTime();
            DateTime? endingTo = new DateTime();
            if (data.SearchObj.Start == "")
            {
                data.SearchObj.PurchaseDateFrom = DateTime.Parse("01/01/1900");
                startingFrom = DateTime.Parse("01/01/1900");
            }
            else
            {
                data.SearchObj.PurchaseDateFrom = DateTime.Parse(data.SearchObj.Start.ToString());

                var startyear = data.SearchObj.PurchaseDateFrom.Value.Year;
                var startmonth = data.SearchObj.PurchaseDateFrom.Value.Month;
                var startday = data.SearchObj.PurchaseDateFrom.Value.Day;
                if (startday < 10)
                    setstartday = data.SearchObj.PurchaseDateFrom.Value.Day.ToString().PadLeft(2, '0');
                else
                    setstartday = data.SearchObj.PurchaseDateFrom.Value.Day.ToString();

                if (startmonth < 10)
                    setstartmonth = data.SearchObj.PurchaseDateFrom.Value.Month.ToString().PadLeft(2, '0');
                else
                    setstartmonth = data.SearchObj.PurchaseDateFrom.Value.Month.ToString();

                var sDate = startyear + "-" + setstartmonth + "-" + setstartday;
                startingFrom = DateTime.Parse(sDate);//.AddDays(1);
            }

            if (data.SearchObj.End == "")
            {
                data.SearchObj.PurchaseDateTo = DateTime.Today.Date;
                endingTo = DateTime.Today.Date;
            }
            else
            {
                data.SearchObj.PurchaseDateTo = DateTime.Parse(data.SearchObj.End.ToString());
                var endyear = data.SearchObj.PurchaseDateTo.Value.Year;
                var endmonth = data.SearchObj.PurchaseDateTo.Value.Month;
                var endday = data.SearchObj.PurchaseDateTo.Value.Day;
                if (endday < 10)
                    setendday = data.SearchObj.PurchaseDateTo.Value.Day.ToString().PadLeft(2, '0');
                else
                    setendday = data.SearchObj.PurchaseDateTo.Value.Day.ToString();
                if (endmonth < 10)
                    setendmonth = data.SearchObj.PurchaseDateTo.Value.Month.ToString().PadLeft(2, '0');
                else
                    setendmonth = data.SearchObj.PurchaseDateTo.Value.Month.ToString();
                var eDate = endyear + "-" + setendmonth + "-" + setendday;
                endingTo = DateTime.Parse(eDate);
            }
            if (startingFrom != null || endingTo != null)
            {
                foreach (var item in list)
                {
                    if (item.PurchaseDate != null)
                    {
                        list = list.Where(a => a.PurchaseDate.Value.Date >= startingFrom.Value.Date && a.PurchaseDate.Value.Date <= endingTo.Value.Date).ToList();
                    }
                }
            }
            else
            {
                list = list.ToList();
            }

            if (list.Count == 0 || list is null)
            {
                if (searchResult.Count > 0)
                {
                    foreach (var item in searchResult)
                    {
                        IndexAssetDetailVM.GetData getDataobj = new IndexAssetDetailVM.GetData();
                        getDataobj.Id = item.Id;
                        getDataobj.Code = item.Code;
                        getDataobj.Price = item.Price;
                        getDataobj.Barcode = item.Barcode;
                        getDataobj.Serial = item.SerialNumber;
                        getDataobj.SerialNumber = item.SerialNumber;
                        getDataobj.BarCode = item.Barcode;
                        getDataobj.Model = item.MasterAsset.ModelNumber;
                        getDataobj.MasterAssetId = item.MasterAssetId;
                        getDataobj.PurchaseDate = item.PurchaseDate;
                        getDataobj.HospitalId = item.HospitalId;
                        getDataobj.DepartmentId = item.DepartmentId;
                        getDataobj.HospitalName = item.Hospital.Name;
                        getDataobj.HospitalNameAr = item.Hospital.NameAr;
                        getDataobj.AssetName = item.MasterAsset.Name;
                        getDataobj.AssetNameAr = item.MasterAsset.NameAr;
                        getDataobj.GovernorateId = item.Hospital.Governorate.Id;
                        getDataobj.GovernorateName = item.Hospital.Governorate.Name;
                        getDataobj.GovernorateNameAr = item.Hospital.Governorate.NameAr;
                        getDataobj.CityId = item.Hospital.City.Id;
                        getDataobj.CityName = item.Hospital.City.Name;
                        getDataobj.CityNameAr = item.Hospital.City.NameAr;
                        getDataobj.OrganizationId = item.Hospital.Organization.Id;
                        getDataobj.OrgName = item.Hospital.Organization.Name;
                        getDataobj.OrgNameAr = item.Hospital.Organization.NameAr;
                        getDataobj.SubOrgName = item.Hospital.SubOrganization.Name;
                        getDataobj.SubOrgNameAr = item.Hospital.SubOrganization.NameAr;
                        getDataobj.SubOrganizationId = item.Hospital.SubOrganization.Id;
                        getDataobj.BrandId = item.MasterAsset.brand != null ? item.MasterAsset.brand.Id : 0;
                        getDataobj.BrandName = item.MasterAsset.brand != null ? item.MasterAsset.brand.Name : "";
                        getDataobj.BrandNameAr = item.MasterAsset.brand != null ? item.MasterAsset.brand.NameAr : "";
                        getDataobj.SupplierId = item.Supplier != null ? item.Supplier.Id : 0;
                        getDataobj.SupplierName = item.Supplier != null ? item.Supplier.Name : "";
                        getDataobj.SupplierNameAr = item.Supplier != null ? item.Supplier.NameAr : "";
                        getDataobj.DepartmentName = item.Department != null ? item.Department.Name : "";
                        getDataobj.DepartmentNameAr = item.Department != null ? item.Department.NameAr : "";

                        var lstTransactions = _context.AssetStatusTransactions.Where(a => a.AssetDetailId == item.Id).OrderByDescending(a => a.StatusDate.Value.Date).ToList();
                        if (lstTransactions.Count > 0)
                        {
                            getDataobj.AssetStatusId = lstTransactions[0].AssetStatusId;
                        }

                        list.Add(getDataobj);
                    }

                }
            }


            mainClass.Results = list;
            return mainClass;

        }

        public List<IndexAssetDetailVM.GetData> FindAllFilteredAssetsForGrouping(FilterHospitalAsset data)
        {
            List<IndexAssetDetailVM.GetData> result = new List<IndexAssetDetailVM.GetData>();
            List<IndexAssetDetailVM.GetData> query = null;

            if (data.AssetName != "")
            {
                query = GetLstAssetDetails().Where(ww => ww.MasterAsset.Name == data.AssetName)
                    .Select(Ass => new IndexAssetDetailVM.GetData
                    {
                        #region MyRegion
                        Id = Ass.Id,
                        AssetName = Ass.MasterAsset.Name,
                        Barcode = Ass.Barcode,
                        SerialNumber = Ass.SerialNumber,
                        Model = Ass.MasterAsset.ModelNumber,
                        DepartmentName = Ass.Department.Name,
                        DepartmentNameAr = Ass.Department.NameAr,
                        AssetNameAr = Ass.MasterAsset.NameAr,
                        BrandName = Ass.MasterAsset.brand.Name,
                        BrandNameAr = Ass.MasterAsset.brand.NameAr,
                        GovernorateName = Ass.Hospital.Governorate.Name,
                        GovernorateNameAr = Ass.Hospital.Governorate.NameAr,
                        CityName = Ass.Hospital.City.Name,
                        CityNameAr = Ass.Hospital.City.NameAr,
                        HospitalId = Ass.HospitalId,
                        HospitalName = Ass.Hospital.Name,
                        HospitalNameAr = Ass.Hospital.NameAr,
                        SupplierName = Ass.Supplier.Name,
                        SupplierNameAr = Ass.Supplier.NameAr,
                        OrgName = Ass.Hospital.Organization.Name,
                        OrgNameAr = Ass.Hospital.Organization.NameAr,
                        PurchaseDate = Ass.PurchaseDate,
                        BrandId = Ass.MasterAsset.BrandId

                        #endregion

                    }).ToList();

            }
            if (data.AssetNameAr != "")
            {


                query = GetLstAssetDetails().Where(ww => ww.MasterAsset.NameAr == data.AssetNameAr)
                        .Select(Ass => new IndexAssetDetailVM.GetData
                        {
                            #region MyRegion
                            Id = Ass.Id,
                            AssetName = Ass.MasterAsset.Name,
                            Barcode = Ass.Barcode,
                            SerialNumber = Ass.SerialNumber,
                            Model = Ass.MasterAsset.ModelNumber,
                            DepartmentName = Ass.Department.Name,
                            DepartmentNameAr = Ass.Department.NameAr,
                            AssetNameAr = Ass.MasterAsset.NameAr,
                            BrandName = Ass.MasterAsset.brand.Name,
                            BrandNameAr = Ass.MasterAsset.brand.NameAr,
                            GovernorateName = Ass.Hospital.Governorate.Name,
                            GovernorateNameAr = Ass.Hospital.Governorate.NameAr,
                            CityName = Ass.Hospital.City.Name,
                            CityNameAr = Ass.Hospital.City.NameAr,
                            HospitalId = Ass.HospitalId,
                            HospitalName = Ass.Hospital.Name,
                            HospitalNameAr = Ass.Hospital.NameAr,
                            SupplierName = Ass.Supplier.Name,
                            SupplierNameAr = Ass.Supplier.NameAr,
                            OrgName = Ass.Hospital.Organization.Name,
                            OrgNameAr = Ass.Hospital.Organization.NameAr,
                            PurchaseDate = Ass.PurchaseDate,
                            BrandId = Ass.MasterAsset.BrandId

                            #endregion

                        }).ToList();




            }

            if (data.DepartmentId != 0)
            {
                if (query is not null)
                {
                    query = query.Where(a => a.DepartmentId == data.DepartmentId).ToList();

                }
                else
                {
                    query = GetLstAssetDetails().Where(ww => ww.DepartmentId == data.DepartmentId)
                     .Select(Ass => new IndexAssetDetailVM.GetData
                     {
                         #region MyRegion
                         Id = Ass.Id,
                         AssetName = Ass.MasterAsset.Name,
                         Barcode = Ass.Barcode,
                         SerialNumber = Ass.SerialNumber,
                         Model = Ass.MasterAsset.ModelNumber,
                         DepartmentName = Ass.Department.Name,
                         DepartmentNameAr = Ass.Department.NameAr,
                         AssetNameAr = Ass.MasterAsset.NameAr,
                         BrandName = Ass.MasterAsset.brand.Name,
                         BrandNameAr = Ass.MasterAsset.brand.NameAr,
                         GovernorateName = Ass.Hospital.Governorate.Name,
                         GovernorateNameAr = Ass.Hospital.Governorate.NameAr,
                         CityName = Ass.Hospital.City.Name,
                         CityNameAr = Ass.Hospital.City.NameAr,
                         HospitalId = Ass.HospitalId,
                         HospitalName = Ass.Hospital.Name,
                         HospitalNameAr = Ass.Hospital.NameAr,
                         SupplierName = Ass.Supplier.Name,
                         SupplierNameAr = Ass.Supplier.NameAr,
                         OrgName = Ass.Hospital.Organization.Name,
                         OrgNameAr = Ass.Hospital.Organization.NameAr,
                         PurchaseDate = Ass.PurchaseDate,
                         BrandId = Ass.MasterAsset.BrandId

                         #endregion

                     }).ToList();


                }

            }
            if (data.BrandId != 0)

            {
                if (query is not null)
                {
                    query = query.Where(a => a.BrandId == data.BrandId).ToList();
                }
                else
                {
                    query = GetLstAssetDetails().Where(ww => ww.MasterAsset.BrandId == data.BrandId)
                                 .Select(Ass => new IndexAssetDetailVM.GetData
                                 {
                                     #region MyRegion
                                     Id = Ass.Id,
                                     AssetName = Ass.MasterAsset.Name,
                                     Barcode = Ass.Barcode,
                                     SerialNumber = Ass.SerialNumber,
                                     Model = Ass.MasterAsset.ModelNumber,
                                     DepartmentName = Ass.Department.Name,
                                     DepartmentNameAr = Ass.Department.NameAr,
                                     AssetNameAr = Ass.MasterAsset.NameAr,
                                     BrandName = Ass.MasterAsset.brand.Name,
                                     BrandNameAr = Ass.MasterAsset.brand.NameAr,
                                     GovernorateName = Ass.Hospital.Governorate.Name,
                                     GovernorateNameAr = Ass.Hospital.Governorate.NameAr,
                                     CityName = Ass.Hospital.City.Name,
                                     CityNameAr = Ass.Hospital.City.NameAr,
                                     HospitalId = Ass.HospitalId,
                                     HospitalName = Ass.Hospital.Name,
                                     HospitalNameAr = Ass.Hospital.NameAr,
                                     SupplierName = Ass.Supplier.Name,
                                     SupplierNameAr = Ass.Supplier.NameAr,
                                     OrgName = Ass.Hospital.Organization.Name,
                                     OrgNameAr = Ass.Hospital.Organization.NameAr,
                                     PurchaseDate = Ass.PurchaseDate,
                                     BrandId = Ass.MasterAsset.BrandId

                                     #endregion

                                 }).ToList();

                }

            }


            if (data.MasterAssetId != 0)
            {

                if (query is not null)
                {
                    query = query.Where(a => a.MasterAssetId == data.MasterAssetId).ToList();
                }
                else
                {
                    query = GetLstAssetDetails().Where(ww => ww.MasterAsset.BrandId == data.BrandId)
                                   .Select(Ass => new IndexAssetDetailVM.GetData
                                   {
                                       #region MyRegion
                                       Id = Ass.Id,
                                       AssetName = Ass.MasterAsset.Name,
                                       Barcode = Ass.Barcode,
                                       SerialNumber = Ass.SerialNumber,
                                       Model = Ass.MasterAsset.ModelNumber,
                                       DepartmentName = Ass.Department.Name,
                                       DepartmentNameAr = Ass.Department.NameAr,
                                       AssetNameAr = Ass.MasterAsset.NameAr,
                                       BrandName = Ass.MasterAsset.brand.Name,
                                       BrandNameAr = Ass.MasterAsset.brand.NameAr,
                                       GovernorateName = Ass.Hospital.Governorate.Name,
                                       GovernorateNameAr = Ass.Hospital.Governorate.NameAr,
                                       CityName = Ass.Hospital.City.Name,
                                       CityNameAr = Ass.Hospital.City.NameAr,
                                       HospitalId = Ass.HospitalId,
                                       HospitalName = Ass.Hospital.Name,
                                       HospitalNameAr = Ass.Hospital.NameAr,
                                       SupplierName = Ass.Supplier.Name,
                                       SupplierNameAr = Ass.Supplier.NameAr,
                                       OrgName = Ass.Hospital.Organization.Name,
                                       OrgNameAr = Ass.Hospital.Organization.NameAr,
                                       PurchaseDate = Ass.PurchaseDate,
                                       BrandId = Ass.MasterAsset.BrandId

                                       #endregion

                                   }).ToList();
                }
            }

            if (data.SupplierId != 0)
            {
                if (query is not null)
                {
                    query = query.Where(a => a.SupplierId == data.SupplierId).ToList();
                }
                else
                {
                    query = GetLstAssetDetails().Where(ww => ww.SupplierId == data.SupplierId)
                                .Select(Ass => new IndexAssetDetailVM.GetData
                                {
                                    #region MyRegion
                                    Id = Ass.Id,
                                    AssetName = Ass.MasterAsset.Name,
                                    Barcode = Ass.Barcode,
                                    SerialNumber = Ass.SerialNumber,
                                    Model = Ass.MasterAsset.ModelNumber,
                                    DepartmentName = Ass.Department.Name,
                                    DepartmentNameAr = Ass.Department.NameAr,
                                    AssetNameAr = Ass.MasterAsset.NameAr,
                                    BrandName = Ass.MasterAsset.brand.Name,
                                    BrandNameAr = Ass.MasterAsset.brand.NameAr,
                                    GovernorateName = Ass.Hospital.Governorate.Name,
                                    GovernorateNameAr = Ass.Hospital.Governorate.NameAr,
                                    CityName = Ass.Hospital.City.Name,
                                    CityNameAr = Ass.Hospital.City.NameAr,
                                    HospitalId = Ass.HospitalId,
                                    HospitalName = Ass.Hospital.Name,
                                    HospitalNameAr = Ass.Hospital.NameAr,
                                    SupplierName = Ass.Supplier.Name,
                                    SupplierNameAr = Ass.Supplier.NameAr,
                                    OrgName = Ass.Hospital.Organization.Name,
                                    OrgNameAr = Ass.Hospital.Organization.NameAr,
                                    PurchaseDate = Ass.PurchaseDate,
                                    BrandId = Ass.MasterAsset.BrandId

                                    #endregion

                                }).ToList();
                }
            }
            if (data.StatusId != 0)
            {
                if (query is not null)
                {

                    var lastTransationForAssets = _context.Set<AssetStatusTransaction>()
                      .FromSqlRaw("EXEC GetAssetStatusTransationsWithLastDate @AssetStatusId"
                      , new SqlParameter("@AssetStatusId", data.StatusId)).ToList();
                    // make join between Lst Of Assets After Search and Lst Of AssetSByIts Status
                    // For Example List Of Assets After Search And  List Of Working Assets
                    //
                    query = (from q in query
                             join lst in lastTransationForAssets on q.Id equals lst.AssetDetailId
                             select q).ToList(); ;


                }
                else
                {
                    var lstAssetsByStatus = _context.Set<AssetDetail>()
                  .FromSqlRaw("EXEC SP_GetAssetsByStatus @AssetStatusId"
                  , new SqlParameter("@AssetStatusId", data.StatusId)).ToList();

                    query = (from listAssetDetailsByStatus in lstAssetsByStatus
                             join masterAsset in _context.MasterAssets
                             on listAssetDetailsByStatus.MasterAssetId equals masterAsset.Id
                             join brand in _context.Brands
                             on masterAsset.BrandId equals brand.Id
                             join supplier in _context.Suppliers
                             on listAssetDetailsByStatus.SupplierId equals supplier.Id
                             join department in _context.Departments
                             on listAssetDetailsByStatus.DepartmentId equals department.Id
                             join hospital in _context.Hospitals
                             on listAssetDetailsByStatus.HospitalId equals hospital.Id
                             join hospitalOrganisation in _context.Organizations
                             on hospital.OrganizationId equals hospitalOrganisation.Id
                             join hospitalSubOrgainzation in _context.SubOrganizations
                             on hospital.SubOrganizationId equals hospitalSubOrgainzation.Id
                             join hospitalGovernerate in _context.Governorates
                             on hospital.GovernorateId equals hospitalGovernerate.Id
                             join hospitalCity in _context.Cities
                             on hospital.CityId equals hospitalCity.Id

                             select new IndexAssetDetailVM.GetData
                             {
                                 Id = listAssetDetailsByStatus.Id,
                                 AssetName = masterAsset.Name,
                                 Barcode = listAssetDetailsByStatus.Barcode,
                                 SerialNumber = listAssetDetailsByStatus.SerialNumber,
                                 Model = masterAsset.ModelNumber,
                                 DepartmentName = department.Name,
                                 DepartmentNameAr = department.NameAr,
                                 AssetNameAr = masterAsset.NameAr,
                                 BrandName = brand.Name,
                                 BrandNameAr = brand.NameAr,
                                 GovernorateName = hospitalGovernerate.Name,
                                 GovernorateNameAr = hospitalGovernerate.NameAr,
                                 CityName = hospitalCity.Name,
                                 CityNameAr = hospitalCity.NameAr,
                                 HospitalId = listAssetDetailsByStatus.HospitalId,
                                 HospitalName = hospital.Name,
                                 HospitalNameAr = hospital.NameAr,
                                 SupplierName = supplier.Name,
                                 SupplierNameAr = supplier.NameAr,
                                 OrgName = hospitalOrganisation.Name,
                                 OrgNameAr = hospitalSubOrgainzation.NameAr,
                                 PurchaseDate = listAssetDetailsByStatus.PurchaseDate,
                                 BrandId = masterAsset.BrandId

                             }).ToList();

                    if (query is not null)
                    {
                        query = query.ToList();
                    }


                    //query= from AssetByStatus in lstAssetsByStatus
                    //       join 


                }

            }

            return query.ToList();

        }

        public List<BrandGroupVM> GroupAssetDetailsByBrand(FilterHospitalAsset data)
        {
            var AssetModel = FindAllFilteredAssetsForGrouping(data);
            List<BrandGroupVM> lstAssetBrand = new List<BrandGroupVM>();

            var LstBrandsByAssets = AssetModel.GroupBy(a => a.BrandId).ToList();

            if (LstBrandsByAssets is not null)
            {
                if (LstBrandsByAssets.Count > 0)
                {
                    foreach (var item in LstBrandsByAssets)
                    {
                        BrandGroupVM AssetBrandObj = new BrandGroupVM();
                        AssetBrandObj.Id = int.Parse(item.Key.ToString());
                        AssetBrandObj.Name = item.First().BrandName;
                        AssetBrandObj.NameAr = item.First().BrandNameAr;
                        AssetBrandObj.AssetList = AssetModel.ToList().Where(w => w.BrandId == AssetBrandObj.Id).ToList();
                        if (AssetBrandObj.AssetList.Count > 0)
                        {
                            lstAssetBrand.Add(AssetBrandObj);
                        }
                    }

                }
            }


            #region MyRegion
            //            Id = Ass.Id,
            //            AssetName = Ass.AssetName,
            //            Barcode = Ass.Barcode,
            //            SerialNumber = Ass.SerialNumber,
            //            Model = Ass.Model,
            //            DepartmentName = Ass.DepartmentName,
            //            DepartmentNameAr = Ass.DepartmentNameAr,

            //            AssetNameAr = Ass.AssetNameAr,
            //            BrandName = Ass.BrandName,
            //            BrandNameAr = Ass.BrandNameAr,
            //            GovernorateName = Ass.GovernorateName,
            //            GovernorateNameAr = Ass.GovernorateNameAr,
            //            CityName = Ass.CityName,
            //            CityNameAr = Ass.CityNameAr,
            //            HospitalId = Ass.HospitalId,
            //            HospitalName = Ass.HospitalName,
            //            HospitalNameAr = Ass.HospitalNameAr,
            //            SupplierName = Ass.SupplierName,
            //            SupplierNameAr = Ass.SupplierNameAr,
            //            OrgName = Ass.OrgName,
            //            OrgNameAr = Ass.OrgNameAr,
            //            PurchaseDate = Ass.PurchaseDate 
            #endregion



            return lstAssetBrand;
        }

        public List<SupplierGroupVM> GroupAssetDetailsBySupplier(FilterHospitalAsset data)
        {
            var AssetModel = FindAllFilteredAssetsForGrouping(data);
            List<SupplierGroupVM> lstAssetBySupplier = new List<SupplierGroupVM>();

            var LstSupplierByAssets = AssetModel.GroupBy(a => a.SupplierName).ToList();

            if (LstSupplierByAssets is not null)
            {
                if (LstSupplierByAssets.Count > 0)
                {
                    foreach (var item in LstSupplierByAssets)
                    {
                        SupplierGroupVM AssetSupplierObj = new SupplierGroupVM();
                        AssetSupplierObj.Id = item.First().Id;
                        AssetSupplierObj.Name = item.First().SupplierName;
                        AssetSupplierObj.NameAr = item.First().SupplierNameAr;
                        AssetSupplierObj.AssetList = AssetModel.ToList().Where(w => w.SupplierName == AssetSupplierObj.Name).ToList();
                        if (AssetSupplierObj.AssetList.Count > 0)
                        {
                            lstAssetBySupplier.Add(AssetSupplierObj);
                        }
                    }

                }
            }

            return lstAssetBySupplier;
        }

        public List<DepartmentGroupVM> GroupAssetDetailsByDepartment(FilterHospitalAsset data)
        {
            var AssetModel = FindAllFilteredAssetsForGrouping(data);
            List<DepartmentGroupVM> lstAssetsDepartment = new List<DepartmentGroupVM>();

            var LstDepartmentByAssets = AssetModel.GroupBy(a => a.DepartmentName).ToList();

            if (LstDepartmentByAssets is not null)
            {
                if (LstDepartmentByAssets.Count > 0)
                {
                    foreach (var item in LstDepartmentByAssets)
                    {
                        DepartmentGroupVM AssetDepartmentObj = new DepartmentGroupVM();
                        AssetDepartmentObj.Id = item.First().Id;
                        AssetDepartmentObj.Name = item.First().DepartmentName;
                        AssetDepartmentObj.NameAr = item.First().DepartmentNameAr;
                        AssetDepartmentObj.AssetList = AssetModel.ToList().Where(w => w.DepartmentName == AssetDepartmentObj.Name).ToList();
                        if (AssetDepartmentObj.AssetList.Count > 0)
                        {
                            lstAssetsDepartment.Add(AssetDepartmentObj);
                        }
                    }

                }
            }
            return lstAssetsDepartment;

        }





        public List<GovernorateGroupVM> GroupAssetDetailsByGovernorate(FilterHospitalAsset data)
        {
            var AssetModel = FindAllFilteredAssetsForGrouping(data);
            List<GovernorateGroupVM> lstAssetsGovernorate = new List<GovernorateGroupVM>();

            var lstGovernoratesByAssets = AssetModel.GroupBy(a => a.GovernorateName).ToList();

            if (lstGovernoratesByAssets is not null)
            {
                if (lstGovernoratesByAssets.Count > 0)
                {
                    foreach (var item in lstGovernoratesByAssets)
                    {
                        GovernorateGroupVM assetDepartmentGovernorateObj = new GovernorateGroupVM();
                        assetDepartmentGovernorateObj.Id = (int)item.First().GovernorateId;
                        assetDepartmentGovernorateObj.Name = item.First().GovernorateName;
                        assetDepartmentGovernorateObj.NameAr = item.First().GovernorateNameAr;
                        assetDepartmentGovernorateObj.AssetList = AssetModel.ToList().Where(w => w.GovernorateName == assetDepartmentGovernorateObj.Name).ToList();
                        if (assetDepartmentGovernorateObj.AssetList.Count > 0)
                        {
                            lstAssetsGovernorate.Add(assetDepartmentGovernorateObj);
                        }
                    }
                }
            }
            return lstAssetsGovernorate;

        }

        public List<GroupCityVM> GroupAssetDetailsByCity(FilterHospitalAsset data)
        {
            var AssetModel = FindAllFilteredAssetsForGrouping(data);
            List<GroupCityVM> lstAssetsGovernorate = new List<GroupCityVM>();

            var lstGovernoratesByAssets = AssetModel.GroupBy(a => a.CityName).ToList();

            if (lstGovernoratesByAssets is not null)
            {
                if (lstGovernoratesByAssets.Count > 0)
                {
                    foreach (var item in lstGovernoratesByAssets)
                    {
                        GroupCityVM assetDepartmentGovernorateObj = new GroupCityVM();
                        assetDepartmentGovernorateObj.Id = (int)item.First().GovernorateId;
                        assetDepartmentGovernorateObj.Name = item.First().GovernorateName;
                        assetDepartmentGovernorateObj.NameAr = item.First().GovernorateNameAr;
                        assetDepartmentGovernorateObj.AssetList = AssetModel.ToList().Where(w => w.GovernorateName == assetDepartmentGovernorateObj.Name).ToList();
                        if (assetDepartmentGovernorateObj.AssetList.Count > 0)
                        {
                            lstAssetsGovernorate.Add(assetDepartmentGovernorateObj);
                        }
                    }
                }
            }
            return lstAssetsGovernorate;

        }



        public List<GroupHospitalVM> GroupAssetDetailsByHospital(FilterHospitalAsset data)
        {
            var AssetModel = FindAllFilteredAssetsForGrouping(data);
            List<GroupHospitalVM> lstAssetsGovernorate = new List<GroupHospitalVM>();

            var lstGovernoratesByAssets = AssetModel.GroupBy(a => a.HospitalName).ToList();

            if (lstGovernoratesByAssets is not null)
            {
                if (lstGovernoratesByAssets.Count > 0)
                {
                    foreach (var item in lstGovernoratesByAssets)
                    {
                        GroupHospitalVM assetDepartmentGovernorateObj = new GroupHospitalVM();
                        assetDepartmentGovernorateObj.Id = (int)item.First().HospitalId;
                        assetDepartmentGovernorateObj.Name = item.First().HospitalName;
                        assetDepartmentGovernorateObj.NameAr = item.First().HospitalNameAr;
                        assetDepartmentGovernorateObj.AssetList = AssetModel.ToList().Where(w => w.HospitalName == assetDepartmentGovernorateObj.Name).ToList();
                        if (assetDepartmentGovernorateObj.AssetList.Count > 0)
                        {
                            lstAssetsGovernorate.Add(assetDepartmentGovernorateObj);
                        }
                    }
                }
            }
            return lstAssetsGovernorate;

        }

        public async Task<IEnumerable<IndexAssetDetailVM.GetData>> GetAssetsByUserId(string userId)
        {
            List<IndexAssetDetailVM.GetData> list = new List<IndexAssetDetailVM.GetData>();
            if (userId != null)
            {
                var userObj = await _context.Users.FindAsync(userId);
                var lstAssetDetails = await _context.AssetDetails.Include(a => a.MasterAsset)
                            .Include(a => a.MasterAsset.brand)
                               .Include(a => a.Supplier)
                                 .Include(a => a.Department)
                           .Include(a => a.Hospital).ThenInclude(h => h.Organization)
                           .Include(a => a.Hospital).ThenInclude(h => h.Governorate)
                           .Include(a => a.Hospital).ThenInclude(h => h.City)
                           .Include(a => a.Hospital).ThenInclude(h => h.SubOrganization)
                           .OrderBy(a => a.Barcode).ToListAsync();

                foreach (var item in lstAssetDetails)
                {
                    IndexAssetDetailVM.GetData getDataobj = new IndexAssetDetailVM.GetData();

                    getDataobj.Id = item.Id;
                    getDataobj.Code = item.Code;
                    getDataobj.Price = item.Price;
                    getDataobj.CreatedBy = item.CreatedBy;
                    getDataobj.Barcode = item.Barcode;
                    getDataobj.Serial = item.SerialNumber;
                    getDataobj.SerialNumber = item.SerialNumber;
                    getDataobj.BarCode = item.Barcode;
                    getDataobj.Model = item.MasterAsset.ModelNumber;
                    getDataobj.MasterAssetId = item.MasterAssetId;
                    getDataobj.PurchaseDate = item.PurchaseDate;
                    getDataobj.HospitalId = item.HospitalId;
                    getDataobj.DepartmentId = item.DepartmentId;
                    getDataobj.HospitalName = item.Hospital.Name;
                    getDataobj.HospitalNameAr = item.Hospital.NameAr;
                    getDataobj.AssetName = item.MasterAsset.Name;
                    getDataobj.AssetNameAr = item.MasterAsset.NameAr;
                    getDataobj.GovernorateId = item.Hospital.Governorate.Id;
                    getDataobj.GovernorateName = item.Hospital.Governorate.Name;
                    getDataobj.GovernorateNameAr = item.Hospital.Governorate.NameAr;
                    getDataobj.CityId = item.Hospital.City.Id;
                    getDataobj.CityName = item.Hospital.City.Name;
                    getDataobj.CityNameAr = item.Hospital.City.NameAr;
                    getDataobj.OrganizationId = item.Hospital.Organization.Id;
                    getDataobj.OrgName = item.Hospital.Organization.Name;
                    getDataobj.OrgNameAr = item.Hospital.Organization.NameAr;
                    getDataobj.SubOrgName = item.Hospital.SubOrganization.Name;
                    getDataobj.SubOrgNameAr = item.Hospital.SubOrganization.NameAr;
                    getDataobj.SubOrganizationId = item.Hospital.SubOrganization.Id;
                    getDataobj.BrandId = item.MasterAsset.brand != null ? item.MasterAsset.brand.Id : 0;
                    getDataobj.BrandName = item.MasterAsset.brand != null ? item.MasterAsset.brand.Name : "";
                    getDataobj.BrandNameAr = item.MasterAsset.brand != null ? item.MasterAsset.brand.NameAr : "";
                    getDataobj.SupplierId = item.Supplier != null ? item.Supplier.Id : 0;
                    getDataobj.SupplierName = item.Supplier != null ? item.Supplier.Name : "";
                    getDataobj.SupplierNameAr = item.Supplier != null ? item.Supplier.NameAr : "";
                    getDataobj.DepartmentName = item.Department != null ? item.Department.Name : "";
                    getDataobj.DepartmentNameAr = item.Department != null ? item.Department.NameAr : "";

                    var lstTransactions = _context.AssetStatusTransactions.Where(a => a.AssetDetailId == item.Id).OrderByDescending(a => a.StatusDate.Value.Date).ToList();
                    if (lstTransactions.Count > 0)
                    {
                        getDataobj.AssetStatusId = lstTransactions[0].AssetStatusId;
                    }

                    list.Add(getDataobj);
                }


                if (userObj.GovernorateId == 0 && userObj.CityId == 0 && userObj.OrganizationId == 0 && userObj.SubOrganizationId == 0 && userObj.HospitalId == 0)
                {
                    list = list.ToList();
                }

                if (userObj.GovernorateId > 0 && userObj.CityId == 0 && userObj.OrganizationId == 0 && userObj.SubOrganizationId == 0 && userObj.HospitalId == 0)
                {
                    list = list.Where(a => a.GovernorateId == userObj.GovernorateId).ToList();
                }

                if (userObj.GovernorateId > 0 && userObj.CityId > 0 && userObj.OrganizationId == 0 && userObj.SubOrganizationId == 0 && userObj.HospitalId == 0)
                {
                    list = list.Where(a => a.GovernorateId == userObj.GovernorateId && a.CityId == userObj.CityId).ToList();
                }

                if (userObj.GovernorateId > 0 && userObj.CityId > 0 && userObj.OrganizationId == 0 && userObj.SubOrganizationId == 0 && userObj.HospitalId > 0)
                {
                    list = list.Where(a => a.GovernorateId == userObj.GovernorateId && a.CityId == userObj.CityId && a.HospitalId == userObj.HospitalId).ToList();
                }
                if (userObj.GovernorateId == 0 && userObj.CityId == 0 && userObj.OrganizationId > 0 && userObj.SubOrganizationId == 0 && userObj.HospitalId == 0)
                {
                    list = list.Where(a => a.OrganizationId == userObj.OrganizationId).ToList();
                }
                if (userObj.GovernorateId == 0 && userObj.CityId == 0 && userObj.OrganizationId > 0 && userObj.SubOrganizationId > 0 && userObj.HospitalId == 0)
                {
                    list = list.Where(a => a.OrganizationId == userObj.OrganizationId && a.SubOrganizationId == userObj.SubOrganizationId).ToList();
                }

                if (userObj.GovernorateId == 0 && userObj.CityId == 0 && userObj.OrganizationId > 0 && userObj.SubOrganizationId > 0 && userObj.HospitalId > 0)
                {
                    list = list.Where(a => a.OrganizationId == userObj.OrganizationId && a.SubOrganizationId == userObj.SubOrganizationId && a.HospitalId == userObj.HospitalId).ToList();
                }

            }
            return list;

        }












        public List<IndexAssetDetailVM.GetData> PrintListOfPMWorkOrders(List<IndexAssetDetailVM.GetData> assets)
        {
            List<AssetDetail> hospitalAssets = new List<AssetDetail>();
            List<IndexAssetDetailVM.GetData> lstPrintWorkOrders = new List<IndexAssetDetailVM.GetData>();

            if (assets.Count == 0)
            {
                hospitalAssets = _context.AssetDetails.Include(w => w.User).Include(a => a.Department).Include(a => a.MasterAsset)
                .Include(a => a.MasterAsset.brand).Include(a => a.Hospital).Include(a => a.Hospital.Governorate).Include(a => a.Hospital.City)
                .Include(a => a.Hospital.Organization).Include(a => a.Hospital.SubOrganization)
                .ToList();

                foreach (var item in hospitalAssets)
                {
                    IndexAssetDetailVM.GetData printWorkObj = new IndexAssetDetailVM.GetData();
                    printWorkObj.Id = item.Id;
                    printWorkObj.Barcode = item.Barcode;
                    printWorkObj.BrandName = item.MasterAsset.brand != null ? item.MasterAsset.brand.Name : "";
                    printWorkObj.BrandNameAr = item.MasterAsset.brand != null ? item.MasterAsset.brand.Name : "";
                    printWorkObj.Model = item.MasterAsset != null ? item.MasterAsset.ModelNumber : "";
                    printWorkObj.CreatedBy = item.User != null ? item.User.UserName : "";
                    printWorkObj.HospitalId = item.HospitalId;
                    printWorkObj.HospitalName = item.Hospital != null ? item.Hospital.Name : "";
                    printWorkObj.HospitalNameAr = item.Hospital != null ? item.Hospital.NameAr : "";
                    printWorkObj.AssetName = item.MasterAsset != null ? item.MasterAsset.Name : "";
                    printWorkObj.AssetNameAr = item.MasterAsset != null ? item.MasterAsset.NameAr : "";
                    printWorkObj.SerialNumber = item.SerialNumber;
                    printWorkObj.DepartmentName = item.Department != null ? item.Department.Name : "";
                    printWorkObj.DepartmentNameAr = item.Department != null ? item.Department.NameAr : "";

                    var lstPMAssetTasks = _context.PMAssetTasks
                                         .Include(A => A.MasterAsset)
                                        .Where(t => t.MasterAsset.Id == item.MasterAssetId).ToList();
                    List<IndexPMAssetTaskVM.GetData> listPM = new List<IndexPMAssetTaskVM.GetData>();
                    if (lstPMAssetTasks.Count > 0)
                    {
                        foreach (var pm in lstPMAssetTasks)
                        {
                            IndexPMAssetTaskVM.GetData pmObj = new IndexPMAssetTaskVM.GetData();
                            pmObj.TaskName = pm.Name;
                            pmObj.TaskNameAr = pm.NameAr;
                            listPM.Add(pmObj);
                        }
                        printWorkObj.ListPMTasks = listPM;
                    }
                    lstPrintWorkOrders.Add(printWorkObj);
                }
            }

            if (assets.Count > 0)
            {
                foreach (var item in assets)
                {
                    IndexAssetDetailVM.GetData printWorkObj = new IndexAssetDetailVM.GetData();
                    printWorkObj.Id = item.Id;
                    printWorkObj.Barcode = item.BarCode;
                    printWorkObj.BrandName = item.BrandName;
                    printWorkObj.BrandNameAr = item.BrandNameAr;
                    printWorkObj.Model = item.Model;
                    printWorkObj.CreatedBy = item.CreatedBy;
                    printWorkObj.UserName = item.UserName;
                    printWorkObj.HospitalId = item.HospitalId;
                    printWorkObj.HospitalName = item.HospitalName;
                    printWorkObj.HospitalNameAr = item.HospitalNameAr;
                    printWorkObj.AssetName = item.AssetName;
                    printWorkObj.AssetNameAr = item.AssetNameAr;
                    printWorkObj.SerialNumber = item.SerialNumber;
                    printWorkObj.DepartmentName = item.DepartmentName;
                    printWorkObj.DepartmentNameAr = item.DepartmentNameAr;

                    var lstPMAssetTasks = _context.PMAssetTasks
                                         .Include(A => A.MasterAsset)
                                        .Where(t => t.MasterAsset.Id == item.MasterAssetId).ToList();
                    List<IndexPMAssetTaskVM.GetData> listPM = new List<IndexPMAssetTaskVM.GetData>();
                    if (lstPMAssetTasks.Count > 0)
                    {
                        foreach (var pm in lstPMAssetTasks)
                        {
                            IndexPMAssetTaskVM.GetData pmObj = new IndexPMAssetTaskVM.GetData();
                            pmObj.TaskName = pm.Name;
                            pmObj.TaskNameAr = pm.NameAr;
                            listPM.Add(pmObj);
                        }
                        printWorkObj.ListPMTasks = listPM;
                    }
                    lstPrintWorkOrders.Add(printWorkObj);
                }
            }




            return lstPrintWorkOrders;
        }




        public List<IndexAssetDetailVM.GetData> PrintListOfPMWorkOrders(SortAndFilterVM data)
        {
    
            List<IndexAssetDetailVM.GetData> lstPrintWorkOrders = new List<IndexAssetDetailVM.GetData>();

            var assets = _context.AssetDetails.Include(w => w.User).Include(a => a.Department).Include(a => a.MasterAsset)
                .Include(a => a.MasterAsset.brand).Include(a => a.Hospital).Include(a => a.Hospital.Governorate).Include(a => a.Hospital.City)
                .Include(a => a.Hospital.Organization).Include(a => a.Hospital.SubOrganization)
                    .ToList();


            if (data.SearchObj.GovernorateId != 0)
                assets = assets.Where(a => a.Hospital.GovernorateId == data.SearchObj.GovernorateId).ToList();
            if (data.SearchObj.CityId != 0)
                assets = assets.Where(a => a.Hospital.CityId == data.SearchObj.CityId).ToList();
            if (data.SearchObj.OrganizationId != 0)
                assets = assets.Where(a => a.Hospital.OrganizationId == data.SearchObj.OrganizationId).ToList();
            if (data.SearchObj.SubOrganizationId != 0)
                assets = assets.Where(a => a.Hospital.SubOrganizationId == data.SearchObj.SubOrganizationId).ToList();
            if (data.SearchObj.HospitalId != 0)
                assets = assets.Where(a => a.HospitalId == data.SearchObj.HospitalId).ToList();
            if (data.SearchObj.DepartmentId != 0)
                assets = assets.Where(a => a.DepartmentId == data.SearchObj.DepartmentId).ToList();
            if (data.SearchObj.BrandId != 0)
                assets = assets.Where(a => a.MasterAsset.BrandId == data.SearchObj.BrandId).ToList();
            if (data.SearchObj.Serial != "")
                assets = assets.Where(a => a.SerialNumber == data.SearchObj.Serial).ToList();
            if (data.SearchObj.BarCode != "")
                assets = assets.Where(a => a.Barcode == data.SearchObj.BarCode).ToList();
            if (data.SearchObj.Model != "")
                assets = assets.Where(a => a.MasterAsset.ModelNumber == data.SearchObj.Model).ToList();

            if (data.SearchObj.AssetName != "")
                assets = assets.Where(x => x.MasterAsset.Name.Contains(data.SearchObj.AssetName)).ToList();


            if (data.SearchObj.AssetNameAr != null)
                assets = assets.Where(x => x.MasterAsset.NameAr.Contains(data.SearchObj.AssetNameAr)).ToList();


            if (data.SearchObj.OriginId != 0)
                assets = assets.Where(a => a.MasterAsset.OriginId == data.SearchObj.OriginId).ToList();


            if (data.SearchObj.SupplierId != 0)
                assets = assets.Where(a => a.SupplierId == data.SearchObj.SupplierId).ToList();

            if (data.SearchObj.MasterAssetId != 0)
                assets = assets.Where(x => x.MasterAssetId == data.SearchObj.MasterAssetId).ToList();



            if (data.SearchObj.StatusId != 0)
            {
                List<int?> ids = new List<int?>();
                if (data.SearchObj.HospitalId == 0)
                {
                    var allIds = _context.AssetStatusTransactions
                       .OrderByDescending(a => a.StatusDate).ToList().GroupBy(a => a.AssetDetailId)
                       .ToList();

                    foreach (var itm in allIds)
                    {
                        if (itm.FirstOrDefault().AssetStatusId == data.SearchObj.StatusId)
                        {
                            ids.Add(itm.FirstOrDefault().AssetDetailId);
                        }
                    }
                }
                else
                {
                    var allIds = _context.AssetStatusTransactions
                                        .Where(a => a.HospitalId == data.SearchObj.HospitalId)
                                        .OrderByDescending(a => a.StatusDate).ToList().GroupBy(a => a.AssetDetailId)
                                        .ToList();

                    foreach (var itm in allIds)
                    {
                        if (itm.FirstOrDefault().AssetStatusId == data.SearchObj.StatusId)
                        {
                            ids.Add(itm.FirstOrDefault().AssetDetailId);
                        }
                    }
                }
                assets = assets.Where(list => ids.Contains(list.Id)).ToList();
            }




            if (assets.Count > 0)
            {
                foreach (var item in assets)
                {
                    IndexAssetDetailVM.GetData printWorkObj = new IndexAssetDetailVM.GetData();
                    printWorkObj.Id = item.Id;
                    printWorkObj.Id = item.Id;
                    printWorkObj.BarCode = item.Barcode;
                    printWorkObj.Model = item.MasterAsset.ModelNumber;
                    printWorkObj.AssetName = item.MasterAsset.Name;
                    printWorkObj.AssetNameAr = item.MasterAsset.NameAr;
                    printWorkObj.SerialNumber = item.SerialNumber;
                    printWorkObj.MasterAssetId = item.MasterAssetId;
                    printWorkObj.CreatedBy = item.User != null ? item.User.UserName : "";
                    printWorkObj.UserName = item.User != null ? item.User.UserName : "";
                    printWorkObj.HospitalId = item.HospitalId;
                    printWorkObj.GovernorateId = item.Hospital.GovernorateId;
                    printWorkObj.CityId = item.Hospital.CityId;
                    printWorkObj.OrganizationId = item.Hospital.OrganizationId;
                    printWorkObj.SubOrganizationId = item.Hospital.SubOrganizationId;
                    //printWorkObj.CreatedById = item.CreatedBy;
                    printWorkObj.MasterAssetId = item.MasterAssetId;
                    printWorkObj.HospitalId = item.HospitalId;
                    printWorkObj.HospitalName = item.Hospital.Name;
                    printWorkObj.HospitalNameAr = item.Hospital.NameAr;
                    printWorkObj.AssetName = item.MasterAsset.Name;
                    printWorkObj.AssetNameAr = item.MasterAsset.NameAr;
                    printWorkObj.DepartmentName = item.Department != null ? item.Department.Name : "";
                    printWorkObj.DepartmentNameAr = item.Department != null ? item.Department.NameAr : "";
                    printWorkObj.BrandName = item.MasterAsset.brand != null ? item.MasterAsset.brand.Name : "";
                    printWorkObj.BrandNameAr = item.MasterAsset.brand != null ? item.MasterAsset.brand.NameAr : "";

                    var lstPMAssetTasks = _context.PMAssetTasks
                                                           .Include(A => A.MasterAsset)
                                                          .Where(t => t.MasterAsset.Id == item.MasterAssetId).ToList();
                    List<IndexPMAssetTaskVM.GetData> listPM = new List<IndexPMAssetTaskVM.GetData>();
                    if (lstPMAssetTasks.Count > 0)
                    {
                        foreach (var pm in lstPMAssetTasks)
                        {
                            IndexPMAssetTaskVM.GetData pmObj = new IndexPMAssetTaskVM.GetData();
                            pmObj.TaskName = pm.Name;
                            pmObj.TaskNameAr = pm.NameAr;
                            listPM.Add(pmObj);
                        }
                        printWorkObj.ListPMTasks = listPM;
                    }
                    lstPrintWorkOrders.Add(printWorkObj);
                }
            }
            return lstPrintWorkOrders;
        }













        #region Refactor Functions

        /// <summary>
        /// List AssetDetails (hospital assets) as Querable
        /// </summary>
        /// <returns></returns>
        private IQueryable<AssetDetail> ListAssetDetails()
        {
            return _context.AssetDetails.Include(a => a.MasterAsset).ThenInclude(a => a.brand).Include(a => a.Supplier).Include(a => a.Department)
                    .Include(a => a.MasterAsset.Origin).Include(a => a.Building).Include(a => a.Floor).Include(a => a.Room).Include(a => a.Hospital)
                    .ThenInclude(h => h.Organization).Include(a => a.Hospital).ThenInclude(h => h.Governorate).Include(a => a.Hospital)
                    .ThenInclude(h => h.City).Include(a => a.Hospital).ThenInclude(h => h.SubOrganization)
                    .OrderBy(q => q.Barcode);

        }


        /// <summary>
        /// Generic AutoComplete Asset BarCode depend on hospitalId
        /// </summary>
        /// <param name="barcode"></param>
        /// <param name="hospitalId"></param>
        /// <returns>List Of IndexAssetDetailVM.GetData</returns>
        public IEnumerable<IndexAssetDetailVM.GetData> AutoCompleteAssetBarCode(string barcode, int hospitalId)
        {
            List<IndexAssetDetailVM.GetData> list = new List<IndexAssetDetailVM.GetData>();
            var lstAssetByBarcode = _context.AssetDetails.Include(a => a.MasterAsset).Include(a => a.Department).Include(a => a.MasterAsset.brand)
               .Include(a => a.Hospital).Where(a => a.Barcode.Contains(barcode)).OrderBy(a => a.Barcode);

            var lst = lstAssetByBarcode.ToList();
            if (hospitalId == 0)
            {
                lst = lst.ToList();
            }
            else
            {
                lst = lst.Where(a => a.HospitalId == hospitalId).ToList();
            }
            if (lst.Count > 0)
            {
                foreach (var item in lst)
                {
                    IndexAssetDetailVM.GetData getDataObj = new IndexAssetDetailVM.GetData();
                    getDataObj.Id = item.Id;
                    getDataObj.Code = item.Code;
                    getDataObj.BarCode = item.Barcode;
                    getDataObj.Price = item.Price;
                    getDataObj.MasterAssetName = item.MasterAsset != null ? item.MasterAsset.Name : "";
                    getDataObj.MasterAssetNameAr = item.MasterAsset != null ? item.MasterAsset.NameAr : "";
                    getDataObj.BrandName = item.MasterAsset.brand != null ? item.MasterAsset.brand.Name : "";
                    getDataObj.BrandNameAr = item.MasterAsset.brand != null ? item.MasterAsset.brand.NameAr : "";
                    getDataObj.DepartmentName = item.Department != null ? item.Department.Name : "";
                    getDataObj.DepartmentNameAr = item.Department != null ? item.Department.NameAr : "";
                    getDataObj.Model = item.MasterAsset != null ? item.MasterAsset.ModelNumber : "";
                    getDataObj.AssetBarCode = item.Barcode;
                    getDataObj.BarCode = item.Barcode;
                    getDataObj.Serial = item.SerialNumber;
                    getDataObj.SerialNumber = item.SerialNumber;
                    getDataObj.MasterAssetId = item.MasterAssetId;
                    getDataObj.PurchaseDate = item.PurchaseDate;
                    getDataObj.HospitalId = item.HospitalId;
                    getDataObj.HospitalName = item.Hospital.Name;
                    getDataObj.HospitalNameAr = item.Hospital.NameAr;
                    getDataObj.AssetName = item.MasterAsset.Name;
                    getDataObj.AssetNameAr = item.MasterAsset.NameAr;
                    var lstAssetTransactions = _context.AssetStatusTransactions.Include(a => a.AssetStatus).Where(a => a.AssetDetailId == item.Id).ToList().OrderByDescending(a => a.StatusDate).ToList();
                    if (lstAssetTransactions.Count > 0)
                    {
                        getDataObj.AssetStatusId = lstAssetTransactions[0].AssetStatus.Id;
                        getDataObj.AssetStatus = lstAssetTransactions[0].AssetStatus.Name;
                        getDataObj.AssetStatusAr = lstAssetTransactions[0].AssetStatus.NameAr;
                    }
                    list.Add(getDataObj);
                }
            }
            return list;
        }

        /// <summary>
        /// Generic AutoComplete Asset Serial depend on hospitalId
        /// </summary>
        /// <param name="serial"></param>
        /// <param name="hospitalId"></param>
        /// <returns></returns>
        public IEnumerable<IndexAssetDetailVM.GetData> AutoCompleteAssetSerial(string serial, int hospitalId)
        {
            List<IndexAssetDetailVM.GetData> list = new List<IndexAssetDetailVM.GetData>();
            var lst = _context.AssetDetails.Include(a => a.MasterAsset).Include(a => a.MasterAsset.brand)
                .Include(a => a.Hospital).Where(a => a.SerialNumber.Contains(serial)).OrderBy(a => a.SerialNumber).ToList();
            if (hospitalId == 0)
            {
                lst = lst.ToList();
            }
            else
            {
                lst = lst.Where(a => a.HospitalId == hospitalId).ToList();
            }
            if (lst.Count > 0)
            {
                foreach (var item in lst)
                {
                    IndexAssetDetailVM.GetData getDataObj = new IndexAssetDetailVM.GetData();
                    getDataObj.Id = item.Id;
                    getDataObj.Code = item.Code;
                    getDataObj.BarCode = item.Barcode;
                    getDataObj.Price = item.Price;
                    getDataObj.MasterAssetName = item.MasterAsset != null ? item.MasterAsset.Name : "";
                    getDataObj.MasterAssetNameAr = item.MasterAsset != null ? item.MasterAsset.NameAr : "";
                    getDataObj.BrandName = item.MasterAsset.brand != null ? item.MasterAsset.brand.Name : "";
                    getDataObj.BrandNameAr = item.MasterAsset.brand != null ? item.MasterAsset.brand.NameAr : "";
                    getDataObj.Model = item.MasterAsset != null ? item.MasterAsset.ModelNumber : "";
                    getDataObj.AssetBarCode = item.Barcode;
                    getDataObj.BarCode = item.Barcode;
                    getDataObj.Serial = item.SerialNumber;
                    getDataObj.SerialNumber = item.SerialNumber;
                    getDataObj.MasterAssetId = item.MasterAssetId;
                    getDataObj.PurchaseDate = item.PurchaseDate;
                    getDataObj.HospitalId = item.HospitalId;
                    getDataObj.HospitalName = item.Hospital.Name;
                    getDataObj.HospitalNameAr = item.Hospital.NameAr;
                    getDataObj.AssetName = item.MasterAsset.Name;
                    getDataObj.AssetNameAr = item.MasterAsset.NameAr;
                    var lstAssetTransactions = _context.AssetStatusTransactions.Include(a => a.AssetStatus).Where(a => a.AssetDetailId == item.Id).ToList().OrderByDescending(a => a.StatusDate).ToList();
                    if (lstAssetTransactions.Count > 0)
                    {
                        getDataObj.AssetStatusId = lstAssetTransactions[0].AssetStatus.Id;
                        getDataObj.AssetStatus = lstAssetTransactions[0].AssetStatus.Name;
                        getDataObj.AssetStatusAr = lstAssetTransactions[0].AssetStatus.NameAr;
                    }
                    list.Add(getDataObj);
                }
            }
            return list;
        }



        /// <summary>
        ///  AutoComplete Asset BarCode depend on hospitalId and department
        /// </summary>
        /// <param name="barcode"></param>
        /// <param name="hospitalId"></param>
        /// <param name="departmentId"></param>
        /// <returns></returns>
        public IEnumerable<IndexAssetDetailVM.GetData> AutoCompleteAssetBarCodeByDepartmentId(string barcode, int hospitalId, int departmentId)
        {
            List<IndexAssetDetailVM.GetData> list = new List<IndexAssetDetailVM.GetData>();
            var lstAssetByBarcode = _context.AssetDetails.Include(a => a.MasterAsset).Include(a => a.Department).Include(a => a.MasterAsset.brand)
               .Include(a => a.Hospital).Where(a => a.Barcode.Contains(barcode)).OrderBy(a => a.Barcode);

            var lst = lstAssetByBarcode.ToList();
            if (hospitalId == 0)
            {
                lst = lst.ToList();
            }
            else
            {
                lst = lst.Where(a => a.HospitalId == hospitalId).ToList();
            }

            if (departmentId == 0)
            {
                lst = lst.ToList();
            }
            else
            {
                lst = lst.Where(a => a.DepartmentId == departmentId).ToList();
            }



            if (lst.Count > 0)
            {
                foreach (var item in lst)
                {
                    IndexAssetDetailVM.GetData getDataObj = new IndexAssetDetailVM.GetData();
                    getDataObj.Id = item.Id;
                    getDataObj.Code = item.Code;
                    getDataObj.BarCode = item.Barcode;
                    getDataObj.Price = item.Price;
                    getDataObj.MasterAssetName = item.MasterAsset != null ? item.MasterAsset.Name : "";
                    getDataObj.MasterAssetNameAr = item.MasterAsset != null ? item.MasterAsset.NameAr : "";
                    getDataObj.BrandName = item.MasterAsset.brand != null ? item.MasterAsset.brand.Name : "";
                    getDataObj.BrandNameAr = item.MasterAsset.brand != null ? item.MasterAsset.brand.NameAr : "";
                    getDataObj.DepartmentName = item.Department != null ? item.Department.Name : "";
                    getDataObj.DepartmentNameAr = item.Department != null ? item.Department.NameAr : "";
                    getDataObj.Model = item.MasterAsset != null ? item.MasterAsset.ModelNumber : "";
                    getDataObj.AssetBarCode = item.Barcode;
                    getDataObj.BarCode = item.Barcode;
                    getDataObj.Serial = item.SerialNumber;
                    getDataObj.SerialNumber = item.SerialNumber;
                    getDataObj.MasterAssetId = item.MasterAssetId;
                    getDataObj.PurchaseDate = item.PurchaseDate;
                    getDataObj.HospitalId = item.HospitalId;
                    getDataObj.HospitalName = item.Hospital.Name;
                    getDataObj.HospitalNameAr = item.Hospital.NameAr;
                    getDataObj.AssetName = item.MasterAsset.Name;
                    getDataObj.AssetNameAr = item.MasterAsset.NameAr;
                    var lstAssetTransactions = _context.AssetStatusTransactions.Include(a => a.AssetStatus).Where(a => a.AssetDetailId == item.Id).ToList().OrderByDescending(a => a.StatusDate).ToList();
                    if (lstAssetTransactions.Count > 0)
                    {
                        getDataObj.AssetStatusId = lstAssetTransactions[0].AssetStatus.Id;
                        getDataObj.AssetStatus = lstAssetTransactions[0].AssetStatus.Name;
                        getDataObj.AssetStatusAr = lstAssetTransactions[0].AssetStatus.NameAr;
                    }
                    list.Add(getDataObj);
                }
            }
            return list;
        }




        /// <summary>
        /// List all AssetDetails (hospital assets) depend on serach and sort variables and paging data after getting results
        /// </summary>
        /// <param name="data"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns>IndexAssetDetailVM</returns>
        public IndexAssetDetailVM ListHospitalAssets(SortAndFilterVM data, int pageNumber, int pageSize)
        {

            #region Initial Variables
            IQueryable<AssetDetail> query = ListAssetDetails();
            IndexAssetDetailVM mainClass = new IndexAssetDetailVM();
            List<IndexAssetDetailVM.GetData> list = new List<IndexAssetDetailVM.GetData>();
            ApplicationUser userObj = new ApplicationUser();
            List<string> userRoleNames = new List<string>();
            Employee empObj = new Employee();
            #endregion

            #region User Role
            if (data.SearchObj!=null && data.SearchObj.UserId != null)
            {
                var getUserById = _context.ApplicationUser.Where(a => a.Id == data.SearchObj.UserId).ToList();
                userObj = getUserById[0];
                var roles = (from userRole in _context.UserRoles
                             join role in _context.ApplicationRole on userRole.RoleId equals role.Id
                             where userRole.UserId == userObj.Id
                             select role);
                foreach (var role in roles)
                {
                    userRoleNames.Add(role.Name);
                }
                var lstEmployees = _context.Employees.Where(a => a.Email == userObj.Email).ToList();
                if (lstEmployees.Count > 0)
                {
                    empObj = lstEmployees[0];
                }
            }
            #endregion

            #region Load Data Depend on User
            if (userObj.HospitalId > 0)
            {
                if ((userRoleNames.Contains("AssetOwner") || userRoleNames.Contains("SRCreator")) &&
                    !userRoleNames.Contains("TLHospitalManager") && !userRoleNames.Contains("EngDepManager"))
                {
                    query = (from detail in query
                             join owner in _context.AssetOwners on detail.Id equals owner.AssetDetailId
                             where owner.HospitalId == data.SearchObj.HospitalId
                             orderby detail.Barcode
                             select detail);
                }
                else
                    query = query;
            }
            else
            {
                if (userObj.GovernorateId > 0 && userObj.CityId == 0 && userObj.OrganizationId == 0 && userObj.SubOrganizationId == 0 && userObj.HospitalId == 0)
                {
                    query = query.Where(a => a.Hospital.GovernorateId == userObj.GovernorateId);
                }
                if (userObj.GovernorateId > 0 && userObj.CityId > 0 && userObj.OrganizationId == 0 && userObj.SubOrganizationId == 0 && userObj.HospitalId == 0)
                {
                    query = query.Where(a => a.Hospital.GovernorateId == userObj.GovernorateId && a.Hospital.CityId == userObj.CityId);
                }
                if (userObj.GovernorateId == 0 && userObj.CityId == 0 && userObj.OrganizationId > 0 && userObj.SubOrganizationId == 0 && userObj.HospitalId == 0)
                {
                    query = query.Where(a => a.Hospital.OrganizationId == userObj.OrganizationId);
                }
                if (userObj.GovernorateId == 0 && userObj.CityId == 0 && userObj.OrganizationId > 0 && userObj.SubOrganizationId > 0 && userObj.HospitalId == 0)
                {
                    query = query.Where(a => a.Hospital.OrganizationId == userObj.OrganizationId && a.Hospital.SubOrganizationId == userObj.SubOrganizationId);
                }
            }
            #endregion

            #region Search Criteria
            if (data.SearchObj != null)
            {
                if (data.SearchObj.HospitalId != 0)
                {
                    query = query.Where(x => x.HospitalId == data.SearchObj.HospitalId);
                }
                if (!string.IsNullOrEmpty(data.SearchObj.AssetName))
                {
                    query = query.Where(x => x.MasterAsset.Name.Contains(data.SearchObj.AssetName));
                }
                if (!string.IsNullOrEmpty(data.SearchObj.AssetNameAr))
                {
                    query = query.Where(x => x.MasterAsset.NameAr.Contains(data.SearchObj.AssetNameAr));
                }
                if (!string.IsNullOrEmpty(data.SearchObj.BarCode))
                {
                    query = query.Where(x => x.Barcode.Contains(data.SearchObj.BarCode));
                }
                if (!string.IsNullOrEmpty(data.SearchObj.Serial))
                {
                    query = query.Where(x => x.SerialNumber.Contains(data.SearchObj.Serial));
                }
                if (data.SearchObj.BrandId != 0)
                {
                    query = query.Where(x => x.MasterAsset.BrandId == data.SearchObj.BrandId);
                }
                if (data.SearchObj.OriginId != 0)
                {
                    query = query.Where(x => x.MasterAsset.OriginId == data.SearchObj.OriginId);
                }
                if (data.SearchObj.SupplierId != 0)
                {
                    query = query.Where(x => x.SupplierId == data.SearchObj.SupplierId);
                }
                if (data.SearchObj.MasterAssetId != 0)
                {
                    query = query.Where(x => x.MasterAssetId == data.SearchObj.MasterAssetId);
                }
                if (data.SearchObj.DepartmentId != 0)
                {
                    query = query.Where(x => x.DepartmentId == data.SearchObj.DepartmentId);
                }
                if (data.SearchObj.Model != "")
                {
                    query = query.Where(x => x.MasterAsset.ModelNumber == data.SearchObj.Model);
                }
                if (data.SearchObj.StatusId != 0)
                {
                    List<int?> ids = new List<int?>();
                    if (data.SearchObj.HospitalId == 0)
                    {
                        var allIds = _context.AssetStatusTransactions
                           //.Where(a => a.AssetStatusId == data.SearchObj.StatusId)
                           .OrderByDescending(a => a.StatusDate).ToList().GroupBy(a => a.AssetDetailId)
                           //.Select(x => x.FirstOrDefault().AssetDetailId)
                           .ToList();

                        foreach (var itm in allIds)
                        {
                            if (itm.FirstOrDefault().AssetStatusId == data.SearchObj.StatusId)
                            {
                                ids.Add(itm.FirstOrDefault().AssetDetailId);
                            }
                        }
                    }
                    else
                    {
                        var allIds = _context.AssetStatusTransactions
                                            .Where(a => a.HospitalId == data.SearchObj.HospitalId)
                                            .OrderByDescending(a => a.StatusDate).ToList().GroupBy(a => a.AssetDetailId)
                                            .ToList();

                        foreach (var itm in allIds)
                        {
                            if (itm.FirstOrDefault().AssetStatusId == data.SearchObj.StatusId)
                            {
                                ids.Add(itm.FirstOrDefault().AssetDetailId);
                            }
                        }
                    }
                    query = query.Where(list => ids.Contains(list.Id));
                }
                if (data.SearchObj.WarrantyTypeId == 1)
                {
                    query = query.Where(b => b.WarrantyEnd != null);
                    query = query.Where(b => b.WarrantyEnd.Value.Date >= DateTime.Today.Date);
                }
                else if (data.SearchObj.WarrantyTypeId == 2)
                {
                    query = query.Where(b => b.WarrantyEnd != null);
                    query = query.Where(b => b.WarrantyEnd.Value.Date <= DateTime.Today.Date);
                }
                string setstartday, setstartmonth, setendday, setendmonth = "";
                DateTime startingFrom = new DateTime();
                DateTime endingTo = new DateTime();
                if (data.SearchObj.Start == "")
                {
                    startingFrom = DateTime.Parse("1900-01-01").Date;
                }
                else
                {
                    data.SearchObj.StartDate = DateTime.Parse(data.SearchObj.Start.ToString());
                    var startyear = data.SearchObj.StartDate.Value.Year;
                    var startmonth = data.SearchObj.StartDate.Value.Month;
                    var startday = data.SearchObj.StartDate.Value.Day;
                    if (startday < 10)
                        setstartday = data.SearchObj.StartDate.Value.Day.ToString().PadLeft(2, '0');
                    else
                        setstartday = data.SearchObj.StartDate.Value.Day.ToString();

                    if (startmonth < 10)
                        setstartmonth = data.SearchObj.StartDate.Value.Month.ToString().PadLeft(2, '0');
                    else
                        setstartmonth = data.SearchObj.StartDate.Value.Month.ToString();

                    var sDate = startyear + "/" + setstartmonth + "/" + setstartday;
                    startingFrom = DateTime.Parse(sDate);
                }

                if (data.SearchObj.End == "")
                {
                    endingTo = DateTime.Today.Date;
                }
                else
                {
                    data.SearchObj.EndDate = DateTime.Parse(data.SearchObj.End.ToString());
                    var endyear = data.SearchObj.EndDate.Value.Year;
                    var endmonth = data.SearchObj.EndDate.Value.Month;
                    var endday = data.SearchObj.EndDate.Value.Day;
                    if (endday < 10)
                        setendday = data.SearchObj.EndDate.Value.Day.ToString().PadLeft(2, '0');
                    else
                        setendday = data.SearchObj.EndDate.Value.Day.ToString();
                    if (endmonth < 10)
                        setendmonth = data.SearchObj.EndDate.Value.Month.ToString().PadLeft(2, '0');
                    else
                        setendmonth = data.SearchObj.EndDate.Value.Month.ToString();
                    var eDate = endyear + "/" + setendmonth + "/" + setendday;
                    endingTo = DateTime.Parse(eDate);
                }
                if (data.SearchObj.Start != "" && data.SearchObj.End != "")
                {
                    query = query.Where(a => a.WarrantyEnd.Value.Date >= startingFrom.Date && a.WarrantyEnd.Value.Date <= endingTo.Date);
                }

                string setcontractstartday, setcontractstartmonth, setcontractendday, setcontractendmonth = "";
                DateTime? startingContractFrom = new DateTime();
                DateTime? endingContractTo = new DateTime();
                if (data.SearchObj.ContractStart == "")
                {
                    startingContractFrom = DateTime.Parse("1900-01-01").Date;
                }
                else
                {
                    data.SearchObj.ContractStartDate = DateTime.Parse(data.SearchObj.ContractStart.ToString());
                    var startcontractyear = data.SearchObj.ContractStartDate.Value.Year;
                    var startcontractmonth = data.SearchObj.ContractStartDate.Value.Month;
                    var startcontractday = data.SearchObj.ContractStartDate.Value.Day;
                    if (startcontractday < 10)
                        setcontractstartday = data.SearchObj.ContractStartDate.Value.Day.ToString().PadLeft(2, '0');
                    else
                        setcontractstartday = data.SearchObj.ContractStartDate.Value.Day.ToString();

                    if (startcontractmonth < 10)
                        setcontractstartmonth = data.SearchObj.ContractStartDate.Value.Month.ToString().PadLeft(2, '0');
                    else
                        setcontractstartmonth = data.SearchObj.ContractStartDate.Value.Month.ToString();

                    var sDate = startcontractyear + "/" + setcontractstartmonth + "/" + setcontractstartday;
                    startingContractFrom = DateTime.Parse(sDate);
                }
                if (data.SearchObj.ContractEnd == "")
                {
                    endingContractTo = DateTime.Today.Date;
                }
                else
                {
                    data.SearchObj.ContractEndDate = DateTime.Parse(data.SearchObj.ContractEnd.ToString());
                    var endcontractyear = data.SearchObj.ContractEndDate.Value.Year;
                    var endcontractmonth = data.SearchObj.ContractEndDate.Value.Month;
                    var endcontractday = data.SearchObj.ContractEndDate.Value.Day;
                    if (endcontractday < 10)
                        setcontractendday = data.SearchObj.ContractEndDate.Value.Day.ToString().PadLeft(2, '0');
                    else
                        setcontractendday = data.SearchObj.ContractEndDate.Value.Day.ToString();
                    if (endcontractmonth < 10)
                        setcontractendmonth = data.SearchObj.ContractEndDate.Value.Month.ToString().PadLeft(2, '0');
                    else
                        setcontractendmonth = data.SearchObj.ContractEndDate.Value.Month.ToString();
                    var eDate = endcontractyear + "/" + setcontractendmonth + "/" + setcontractendday;
                    endingContractTo = DateTime.Parse(eDate);
                }
                if (data.SearchObj.ContractStart != "" && data.SearchObj.ContractEnd != "")
                {
                    List<AssetDetail> assetsInContract = new List<AssetDetail>();
                    if (data.SearchObj.HospitalId != 0)
                    {
                        assetsInContract = _context.ContractDetails.Include(a => a.MasterContract).Include(a => a.AssetDetail)
                            .Where(a => a.HospitalId == data.SearchObj.HospitalId &&
                             (a.MasterContract.From.Value.Date >= startingContractFrom.Value.Date && a.MasterContract.From.Value.Date <= endingContractTo.Value.Date) ||
                             (a.MasterContract.To.Value.Date >= startingContractFrom.Value.Date && a.MasterContract.To.Value.Date <= endingContractTo.Value.Date))
                            .ToList().GroupBy(a => a.AssetDetailId).Select(a => a.FirstOrDefault().AssetDetail).ToList();
                    }
                    else
                    {
                        assetsInContract = _context.ContractDetails.Include(a => a.MasterContract).Include(a => a.AssetDetail)
                                            .Where(a =>
                                             (a.MasterContract.From.Value.Date >= startingContractFrom.Value.Date && a.MasterContract.From.Value.Date <= endingContractTo.Value.Date) ||
                                             (a.MasterContract.To.Value.Date >= startingContractFrom.Value.Date && a.MasterContract.To.Value.Date <= endingContractTo.Value.Date))
                                            .ToList().GroupBy(a => a.AssetDetailId).Select(a => a.FirstOrDefault().AssetDetail).ToList();
                    }
                    query = query.Where(Ids => assetsInContract.Contains(Ids));
                }


                if (data.SearchObj.ContractTypeId == 1)
                {
                    List<AssetDetail> assetsOutContract = new List<AssetDetail>();
                    if (data.SearchObj.HospitalId != 0)
                    {
                        assetsOutContract = _context.ContractDetails.Include(a => a.MasterContract).Include(a => a.AssetDetail)
                          .Where(a => a.HospitalId == data.SearchObj.HospitalId && a.MasterContract.ContractDate >= DateTime.Today.Date).ToList()
                          .GroupBy(a => a.AssetDetailId).Select(a => a.FirstOrDefault().AssetDetail).ToList();
                    }
                    else
                    {
                        assetsOutContract = _context.ContractDetails.Include(a => a.MasterContract).Include(a => a.AssetDetail)
                         .Where(a => a.MasterContract.ContractDate >= DateTime.Today.Date).ToList()
                         .GroupBy(a => a.AssetDetailId).Select(a => a.FirstOrDefault().AssetDetail).ToList();
                    }
                    query = query.Where(Ids => assetsOutContract.Contains(Ids));
                }
                else if (data.SearchObj.ContractTypeId == 2)
                {
                    List<AssetDetail> notInContract = new List<AssetDetail>();
                    if (data.SearchObj.HospitalId != 0)
                    {
                        notInContract = _context.ContractDetails.Include(a => a.MasterContract).Include(a => a.AssetDetail)
                         .Where(a => a.HospitalId == data.SearchObj.HospitalId && a.MasterContract.ContractDate != null && a.MasterContract.ContractDate <= DateTime.Today.Date).ToList()
                         .GroupBy(a => a.AssetDetailId).Select(a => a.FirstOrDefault().AssetDetail).ToList();
                    }
                    else
                    {
                        notInContract = _context.ContractDetails.Include(a => a.MasterContract).Include(a => a.AssetDetail)
                     .Where(a => a.MasterContract.ContractDate != null && a.MasterContract.ContractDate <= DateTime.Today.Date).ToList()
                     .GroupBy(a => a.AssetDetailId).Select(a => a.FirstOrDefault().AssetDetail).ToList();
                    }
                    query = query.Where(Ids => notInContract.Contains(Ids));

                }
            }

            #endregion

            #region Sort Criteria
            if (data.SortObj != null)
            {


                switch (data.SortObj.SortBy)
                {
                    case "Barcode":
                    case "الباركود":
                        if (data.SortObj.SortStatus == "ascending")
                        {
                            query = query.OrderBy(x => x.Barcode);
                        }
                        else
                        {
                            query = query.OrderByDescending(x => x.Barcode);
                        }
                        break;
                    case "Serial":
                    case "السيريال":
                        if (data.SortObj.SortStatus == "ascending")
                        {
                            query = query.OrderBy(x => x.SerialNumber);
                        }
                        else
                        {
                            query = query.OrderByDescending(x => x.SerialNumber);
                        }
                        break;
                    case "Model":
                    case "الموديل":
                        if (data.SortObj.SortStatus == "ascending")
                        {
                            query = query.OrderBy(x => x.MasterAsset.ModelNumber);
                        }
                        else
                        {
                            query = query.OrderByDescending(x => x.MasterAsset.ModelNumber);
                        }
                        break;
                    case "Name":
                        if (data.SortObj.SortStatus == "ascending")
                        {
                            query.OrderBy(x => x.MasterAsset.Name);
                        }
                        else
                        {
                            query.OrderByDescending(x => x.MasterAsset.Name);
                        }
                        break;

                    case "الاسم":
                        if (data.SortObj.SortStatus == "ascending")
                        {
                            query = query.OrderBy(x => x.MasterAsset.NameAr);
                        }
                        else
                        {
                            query = query.OrderByDescending(x => x.MasterAsset.NameAr);
                        }
                        break;
                    case "Hospital":
                        if (data.SortObj.SortStatus == "ascending")
                        {
                            query = query.OrderBy(x => x.Hospital.Name);
                        }
                        else
                        {
                            query = query.OrderByDescending(x => x.Hospital.Name);
                        }
                        break;

                    case "المستشفى":
                        if (data.SortObj.SortStatus == "ascending")
                        {
                            query = query.OrderBy(x => x.Hospital.NameAr);
                        }
                        else
                        {
                            query = query.OrderByDescending(x => x.Hospital.NameAr);
                        }
                        break;
                    case "Brands":
                        if (data.SortObj.SortStatus == "ascending")
                        {
                            query = query.OrderBy(x => x.MasterAsset.brand.Name);
                        }
                        else
                        {
                            query = query.OrderByDescending(x => x.MasterAsset.brand.Name);
                        }
                        break;
                    case "الماركات":
                        if (data.SortObj.SortStatus == "ascending")
                        {
                            query = query.OrderBy(x => x.MasterAsset.brand.NameAr);
                        }
                        else
                        {
                            query = query.OrderByDescending(x => x.MasterAsset.brand.NameAr);
                        }
                        break;
                    case "Department":
                        if (data.SortObj.SortStatus == "ascending")
                        {
                            query = query.OrderBy(x => x.Department.Name);
                        }
                        else
                        {
                            query = query.OrderByDescending(x => x.Department.Name);
                        }
                        break;
                    case "القسم":
                        if (data.SortObj.SortStatus == "ascending")
                        {
                            query = query.OrderBy(x => x.Department.NameAr);
                        }
                        else
                        {
                            query = query.OrderByDescending(x => x.Department.NameAr);
                        }
                        break;
                    case "Governorate":
                        if (data.SortObj.SortStatus == "ascending")
                        {
                            query = query.OrderBy(x => x.Hospital.Governorate.Name);
                        }
                        else
                        {
                            query = query.OrderByDescending(x => x.Hospital.Governorate.Name);
                        }
                        break;

                    case "المحافظة":
                        if (data.SortObj.SortStatus == "ascending")
                        {
                            query = query.OrderBy(x => x.Hospital.Governorate.NameAr);
                        }
                        else
                        {
                            query = query.OrderByDescending(x => x.Hospital.Governorate.NameAr);
                        }
                        break;

                    case "City":
                        if (data.SortObj.SortStatus == "ascending")
                        {
                            query = query.OrderBy(x => x.Hospital.City.Name);
                        }
                        else
                        {
                            query = query.OrderByDescending(x => x.Hospital.City.Name);
                        }
                        break;

                    case "المدينة":
                        if (data.SortObj.SortStatus == "ascending")
                        {
                            query = query.OrderBy(x => x.Hospital.City.NameAr);
                        }
                        else
                        {
                            query = query.OrderByDescending(x => x.Hospital.City.NameAr);
                        }
                        break;

                    case "Organization":
                        if (data.SortObj.SortStatus == "ascending")
                        {
                            query = query.OrderBy(x => x.Hospital.Organization.Name);
                        }
                        else
                        {
                            query = query.OrderByDescending(x => x.Hospital.Organization.Name);
                        }
                        break;

                    case "الهيئة":
                        if (data.SortObj.SortStatus == "ascending")
                        {
                            query = query.OrderBy(x => x.Hospital.Organization.NameAr);
                        }
                        else
                        {
                            query = query.OrderByDescending(x => x.Hospital.Organization.NameAr);
                        }
                        break;

                    case "SubOrganization":
                        if (data.SortObj.SortStatus == "ascending")
                        {
                            query = query.OrderBy(x => x.Hospital.SubOrganization.Name);
                        }
                        else
                        {
                            query = query.OrderByDescending(x => x.Hospital.SubOrganization.Name);
                        }
                        break;

                    case "هيئة فرعية":
                        if (data.SortObj.SortStatus == "ascending")
                        {
                            query = query.OrderBy(x => x.Hospital.SubOrganization.NameAr);
                        }
                        else
                        {
                            query = query.OrderByDescending(x => x.Hospital.SubOrganization.NameAr);
                        }
                        break;
                    default:
                        query = query.OrderBy(x => x.Barcode);
                        break;
                }
            }

            #endregion

            #region Count data and fiter By Paging
            IQueryable<AssetDetail> lstResults = null;
            mainClass.Count = query.Count();
            if (pageNumber == 0 && pageSize == 0)
                lstResults = query;
            else
                lstResults = query.Skip((pageNumber - 1) * pageSize).Take(pageSize);
            #endregion

            #region Loop to get Items after serach and sort
            foreach (var item in lstResults)
            {

                IndexAssetDetailVM.GetData getDataobj = new IndexAssetDetailVM.GetData();
                getDataobj.Id = item.Id;
                getDataobj.Code = item.Code;
                getDataobj.Price = item.Price;
                getDataobj.Barcode = item.Barcode;
                getDataobj.Serial = item.SerialNumber;
                getDataobj.SerialNumber = item.SerialNumber;
                getDataobj.BarCode = item.Barcode;
                getDataobj.Model = item.MasterAsset.ModelNumber;
                getDataobj.MasterAssetId = item.MasterAssetId;
                getDataobj.PurchaseDate = item.PurchaseDate;
                getDataobj.HospitalId = item.HospitalId;
                getDataobj.DepartmentId = item.DepartmentId;
                getDataobj.HospitalName = item.Hospital.Name;
                getDataobj.HospitalNameAr = item.Hospital.NameAr;
                getDataobj.AssetName = item.MasterAsset.Name;
                getDataobj.AssetNameAr = item.MasterAsset.NameAr;
                getDataobj.GovernorateId = item.Hospital.Governorate.Id;
                getDataobj.GovernorateName = item.Hospital.Governorate.Name;
                getDataobj.GovernorateNameAr = item.Hospital.Governorate.NameAr;
                getDataobj.CityId = item.Hospital.City.Id;
                getDataobj.CityName = item.Hospital.City.Name;
                getDataobj.CityNameAr = item.Hospital.City.NameAr;
                getDataobj.OrganizationId = item.Hospital.Organization.Id;
                getDataobj.OrgName = item.Hospital.Organization.Name;
                getDataobj.OrgNameAr = item.Hospital.Organization.NameAr;
                getDataobj.SubOrgName = item.Hospital.SubOrganization.Name;
                getDataobj.SubOrgNameAr = item.Hospital.SubOrganization.NameAr;
                getDataobj.SubOrganizationId = item.Hospital.SubOrganization.Id;
                getDataobj.BrandId = item.MasterAsset.brand != null ? item.MasterAsset.brand.Id : 0;
                getDataobj.BrandName = item.MasterAsset.brand != null ? item.MasterAsset.brand.Name : "";
                getDataobj.BrandNameAr = item.MasterAsset.brand != null ? item.MasterAsset.brand.NameAr : "";
                getDataobj.SupplierId = item.Supplier != null ? item.Supplier.Id : 0;
                getDataobj.SupplierName = item.Supplier != null ? item.Supplier.Name : "";
                getDataobj.SupplierNameAr = item.Supplier != null ? item.Supplier.NameAr : "";
                getDataobj.DepartmentName = item.Department != null ? item.Department.Name : "";
                getDataobj.DepartmentNameAr = item.Department != null ? item.Department.NameAr : "";

                getDataobj.PurchaseDate = item.PurchaseDate != null ? item.PurchaseDate : null;
                getDataobj.InstallationDate = item.InstallationDate != null ? item.InstallationDate : null;
                getDataobj.OperationDate = item.OperationDate != null ? item.OperationDate : null;
                getDataobj.ReceivingDate = item.ReceivingDate != null ? item.ReceivingDate : null;

                getDataobj.StrPurchaseDate = item.PurchaseDate != null ? item.PurchaseDate.Value.ToShortDateString() : "";
                getDataobj.StrInstallationDate = item.InstallationDate != null ? item.InstallationDate.Value.ToShortDateString() : "";
                getDataobj.StrOperationDate = item.OperationDate != null ? item.OperationDate.Value.ToShortDateString() : "";
                getDataobj.StrReceivingDate = item.ReceivingDate != null ? item.ReceivingDate.Value.ToShortDateString() : "";
                getDataobj.WarrantyStart = item.WarrantyStart != null ? item.WarrantyStart : null;
                getDataobj.WarrantyEnd = item.WarrantyEnd != null ? item.WarrantyEnd : null;
                getDataobj.WarrantyExpires = item.WarrantyExpires;
                getDataobj.PONumber = item.PONumber;
                getDataobj.QrFilePath = item.QrFilePath;
                getDataobj.QrData = item.QrData;
                getDataobj.CostCenter = item.CostCenter;

                getDataobj.BuildingId = item.BuildingId;
                if (item.BuildingId != null)
                {
                    getDataobj.BuildName = item.Building.Name;
                    getDataobj.BuildNameAr = item.Building.NameAr;
                }
                getDataobj.RoomId = item.RoomId;
                if (item.RoomId != null)
                {
                    getDataobj.RoomName = item.Room.Name;
                    getDataobj.RoomNameAr = item.Room.NameAr;
                }
                getDataobj.FloorId = item.FloorId;
                if (item.FloorId != null)
                {
                    getDataobj.FloorName = item.Floor.Name;
                    getDataobj.FloorNameAr = item.Floor.NameAr;
                }
                if (item.MasterAsset.brand != null)
                {
                    getDataobj.BrandName = item.MasterAsset.brand.Name;
                    getDataobj.BrandNameAr = item.MasterAsset.brand.NameAr;
                }


                getDataobj.DepartmentId = item.DepartmentId;
                getDataobj.SupplierId = item.SupplierId;
                getDataobj.HospitalId = item.HospitalId;
                if (item.HospitalId != null)
                {
                    getDataobj.HospitalName = item.Hospital.Name;
                    getDataobj.HospitalNameAr = item.Hospital.NameAr;
                    getDataobj.GovernorateId = item.Hospital.GovernorateId;
                    getDataobj.CityId = item.Hospital.CityId;
                    getDataobj.OrganizationId = item.Hospital.OrganizationId;
                    getDataobj.SubOrganizationId = item.Hospital.SubOrganizationId;
                }


                getDataobj.SupplierName = item.Supplier != null ? item.Supplier.Name : "";
                getDataobj.SupplierNameAr = item.Supplier != null ? item.Supplier.NameAr : "";
                getDataobj.BrandId = item.MasterAsset.brand != null ? item.MasterAsset.BrandId : 0;
                getDataobj.BrandName = item.MasterAsset.brand != null ? item.MasterAsset.brand.Name : "";
                getDataobj.BrandNameAr = item.MasterAsset.brand != null ? item.MasterAsset.brand.NameAr : "";
                getDataobj.DepartmentId = item.Department != null ? item.DepartmentId : 0;
                getDataobj.DepartmentName = item.Department != null ? item.Department.Name : "";
                getDataobj.DepartmentNameAr = item.Department != null ? item.Department.NameAr : "";

                var IsInContract = _context.ContractDetails.Include(a => a.MasterContract).Include(a => a.AssetDetail).Where(a => a.AssetDetailId == item.Id).ToList();
                if (IsInContract.Count > 0)
                {
                    var contractObj = IsInContract[0];
                    if (contractObj.MasterContract.To < DateTime.Today.Date)
                    {
                        getDataobj.InContract = "End Contract";
                        getDataobj.InContractAr = "انتهى العقد";
                        getDataobj.ContractFrom = contractObj.MasterContract.From.Value.ToShortDateString();
                        getDataobj.ContractTo = contractObj.MasterContract.To.Value.ToShortDateString();
                    }
                    if (contractObj.MasterContract.To > DateTime.Today.Date)
                    {
                        getDataobj.InContract = "In Contract";
                        getDataobj.InContractAr = "في العقد";
                        getDataobj.ContractFrom = contractObj.MasterContract.From.Value.ToShortDateString();
                        getDataobj.ContractTo = contractObj.MasterContract.To.Value.ToShortDateString();
                    }
                    if (contractObj.MasterContract.To == null || contractObj.MasterContract.To.Value.ToString() == "" || contractObj.MasterContract.To == DateTime.Parse("1/1/1900"))
                    {
                        getDataobj.InContract = "Not In Contract";
                        getDataobj.InContractAr = " ليس في العقد";
                        getDataobj.ContractFrom = "";
                        getDataobj.ContractTo = "";
                    }
                }
                list.Add(getDataobj);
            }
            #endregion

            #region Represent data after Paging and count
            mainClass.Results = list;
            #endregion

            return mainClass;
        }


        /// <summary>
        /// Get Asset Detail By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public EditAssetDetailVM GetById(int id)
        {
            var lstAssetDetails = _context.AssetDetails.Include(a => a.MasterAsset).Include(a => a.User)
                .Include(a => a.MasterAsset.brand).Include(a => a.Building).Include(a => a.Floor).Include(a => a.Room)
                                .Include(a => a.Hospital).Include(a => a.Hospital.Governorate)
                                .Include(a => a.Hospital.City).Include(a => a.Hospital.Organization).Include(a => a.Hospital.SubOrganization)
                                  .Include(a => a.Supplier).Include(a => a.Department).Where(a => a.Id == id).ToList();

            if (lstAssetDetails.Count > 0)
            {
                var assetDetailObj = lstAssetDetails[0];
                EditAssetDetailVM item = new EditAssetDetailVM();
                item.Id = assetDetailObj.Id;
                item.UserName = assetDetailObj.CreatedBy != null ? _context.ApplicationUser.Where(a => a.Id == assetDetailObj.CreatedBy).FirstOrDefault().UserName : "";
                item.CreatedBy = assetDetailObj.CreatedBy;
                item.MasterAssetId = assetDetailObj.MasterAssetId;
                item.AssetName = assetDetailObj.MasterAsset.Name;
                item.AssetNameAr = assetDetailObj.MasterAsset.NameAr;
                item.Model = assetDetailObj.MasterAsset.ModelNumber;
                item.Code = assetDetailObj.Code;
                item.PurchaseDate = assetDetailObj.PurchaseDate != null ? assetDetailObj.PurchaseDate.Value.ToShortDateString() : "";
                item.Price = assetDetailObj.Price;
                item.Serial = assetDetailObj.SerialNumber;
                item.SerialNumber = assetDetailObj.SerialNumber;
                item.Remarks = assetDetailObj.Remarks;
                item.Barcode = assetDetailObj.Barcode;
                item.BarCode = assetDetailObj.Barcode;
                item.InstallationDate = assetDetailObj.InstallationDate != null ? assetDetailObj.InstallationDate.Value.ToShortDateString() : "";
                item.OperationDate = assetDetailObj.OperationDate != null ? assetDetailObj.OperationDate.Value.ToShortDateString() : "";
                item.ReceivingDate = assetDetailObj.ReceivingDate != null ? assetDetailObj.ReceivingDate.Value.ToShortDateString() : "";
                item.PONumber = assetDetailObj.PONumber;
                item.WarrantyExpires = assetDetailObj.WarrantyExpires;
                item.QrFilePath = assetDetailObj.QrFilePath;
                item.QrData = assetDetailObj.QrData;
                item.AssetImg = assetDetailObj.MasterAsset.AssetImg;
                item.MasterAssetId = assetDetailObj.MasterAssetId;
                item.WarrantyStart = assetDetailObj.WarrantyStart != null ? assetDetailObj.WarrantyStart.Value.ToShortDateString() : "";
                item.WarrantyEnd = assetDetailObj.WarrantyEnd != null ? assetDetailObj.WarrantyEnd.Value.ToShortDateString() : "";
                item.CostCenter = assetDetailObj.CostCenter;
                item.DepreciationRate = assetDetailObj.DepreciationRate;
                item.StrPurchaseDate = (assetDetailObj.PurchaseDate != null && assetDetailObj.PurchaseDate.Value.ToShortDateString() != "1/1/1900") ? assetDetailObj.PurchaseDate.Value.ToShortDateString() : "";
                item.StrInstallationDate = (assetDetailObj.InstallationDate != null && assetDetailObj.InstallationDate.Value.ToShortDateString() != "1/1/1900") ? assetDetailObj.InstallationDate.Value.ToShortDateString() : "";
                item.StrOperationDate = (assetDetailObj.OperationDate != null && assetDetailObj.OperationDate.Value.ToShortDateString() != "1/1/1900") ? assetDetailObj.OperationDate.Value.ToShortDateString() : "";
                item.StrReceivingDate = (assetDetailObj.ReceivingDate != null && assetDetailObj.ReceivingDate.Value.ToShortDateString() != "1/1/1900") ? assetDetailObj.ReceivingDate.Value.ToShortDateString() : "";

                item.BuildingId = assetDetailObj.BuildingId;
                if (assetDetailObj.BuildingId != null)
                {
                    item.BuildName = assetDetailObj.Building.Name;
                    item.BuildNameAr = assetDetailObj.Building.NameAr;
                }
                item.RoomId = assetDetailObj.RoomId;
                if (assetDetailObj.RoomId != null)
                {
                    item.RoomName = assetDetailObj.Room.Name;
                    item.RoomNameAr = assetDetailObj.Room.NameAr;
                }
                item.FloorId = assetDetailObj.FloorId;
                if (assetDetailObj.FloorId != null)
                {
                    item.FloorName = assetDetailObj.Floor.Name;
                    item.FloorNameAr = assetDetailObj.Floor.NameAr;
                }
                if (assetDetailObj.MasterAsset.brand != null)
                {
                    item.BrandId = (int)assetDetailObj.MasterAsset.BrandId;
                    item.BrandName = assetDetailObj.MasterAsset.brand.Name;
                    item.BrandNameAr = assetDetailObj.MasterAsset.brand.NameAr;
                }


                item.DepartmentId = assetDetailObj.DepartmentId;
                item.SupplierId = assetDetailObj.SupplierId;
                item.HospitalId = assetDetailObj.HospitalId;
                if (assetDetailObj.HospitalId != null)
                {
                    item.HospitalName = assetDetailObj.Hospital.Name;
                    item.HospitalNameAr = assetDetailObj.Hospital.NameAr;
                    item.GovernorateId = assetDetailObj.Hospital.GovernorateId;
                    item.CityId = assetDetailObj.Hospital.CityId;
                    item.OrganizationId = assetDetailObj.Hospital.OrganizationId;
                    item.SubOrganizationId = assetDetailObj.Hospital.SubOrganizationId;
                }


                item.SupplierName = assetDetailObj.Supplier != null ? assetDetailObj.Supplier.Name : "";
                item.SupplierNameAr = assetDetailObj.Supplier != null ? assetDetailObj.Supplier.NameAr : "";
                item.BrandId = assetDetailObj.MasterAsset.brand != null ? assetDetailObj.MasterAsset.BrandId : 0;
                item.BrandName = assetDetailObj.MasterAsset.brand != null ? assetDetailObj.MasterAsset.brand.Name : "";
                item.BrandNameAr = assetDetailObj.MasterAsset.brand != null ? assetDetailObj.MasterAsset.brand.NameAr : "";
                item.DepartmentId = assetDetailObj.Department != null ? assetDetailObj.DepartmentId : 0;
                item.DepartmentName = assetDetailObj.Department != null ? assetDetailObj.Department.Name : "";
                item.DepartmentNameAr = assetDetailObj.Department != null ? assetDetailObj.Department.NameAr : "";

                var lstAssetTransactions = _context.AssetStatusTransactions.Include(a => a.AssetStatus).Where(a => a.AssetDetailId == assetDetailObj.Id).OrderByDescending(a => a.StatusDate).ToList();
                if (lstAssetTransactions.Count > 0)
                {
                    item.AssetStatusId = lstAssetTransactions[0].AssetStatus.Id;
                    item.AssetStatus = lstAssetTransactions[0].AssetStatus.Name;
                    item.AssetStatusAr = lstAssetTransactions[0].AssetStatus.NameAr;
                }

                var IsInContract = _context.ContractDetails.Include(a => a.MasterContract).Include(a => a.AssetDetail).Where(a => a.AssetDetailId == id).ToList();
                if (IsInContract.Count > 0)
                {

                    var contractObj = IsInContract[0];
                    if (contractObj.MasterContract.To < DateTime.Today.Date)
                    {
                        item.InContract = "End Contract";
                        item.InContractAr = "انتهى العقد";
                        item.ContractFrom = contractObj.MasterContract.From.Value.ToShortDateString();
                        item.ContractTo = contractObj.MasterContract.To.Value.ToShortDateString();
                    }
                    if (contractObj.MasterContract.To > DateTime.Today.Date)
                    {
                        item.InContract = "In Contract";
                        item.InContractAr = "في العقد";
                        item.ContractFrom = contractObj.MasterContract.From.Value.ToShortDateString();
                        item.ContractTo = contractObj.MasterContract.To.Value.ToShortDateString();
                    }
                    if (contractObj.MasterContract.To == null || contractObj.MasterContract.To.Value.ToString() == "" || contractObj.MasterContract.To == DateTime.Parse("1/1/1900"))
                    {
                        item.InContract = "Not In Contract";
                        item.InContractAr = " ليس في العقد";

                        item.ContractFrom = "";
                        item.ContractTo = "";
                    }
                }

                return item;

            }
            return null;
        }

        /// <summary>
        /// Create Barcode for each new assetDetail depend on last Barcode exist
        /// </summary>
        /// <returns></returns>
        public GeneratedAssetDetailBCVM GenerateAssetDetailBarcode()
        {
            GeneratedAssetDetailBCVM numberObj = new GeneratedAssetDetailBCVM();
            int barCode = 0;

            var lastId = _context.AssetDetails.ToList();
            if (lastId.Count > 0)
            {
                var code = lastId.Max(a => a.Barcode);
                if (code.Contains('-'))
                {
                    string[] barcodenumber = code.Split("-");
                    var barcode = (int.Parse(barcodenumber[0]) + 1).ToString();
                    var lastcode = barcode.ToString().PadLeft(9, '0');
                    numberObj.BarCode = lastcode;
                }
                else
                {
                    var barcode = (int.Parse(code) + 1).ToString();
                    var lastcode = barcode.ToString().PadLeft(9, '0');
                    numberObj.BarCode = lastcode;
                }
            }
            else
            {
                numberObj.BarCode = (barCode + 1).ToString();
            }

            return numberObj;
        }

        /// <summary>
        /// Create New Asset Detail
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Add(CreateAssetDetailVM model)
        {
            AssetDetail assetDetailObj = new AssetDetail();
            try
            {
                if (model != null)
                {
                    assetDetailObj.Code = model.Code;
                    if (model.PurchaseDate != "")
                        assetDetailObj.PurchaseDate = DateTime.Parse(model.PurchaseDate);
                    assetDetailObj.Price = model.Price;
                    assetDetailObj.SerialNumber = model.SerialNumber;
                    assetDetailObj.Remarks = model.Remarks;
                    assetDetailObj.Barcode = model.Barcode;
                    if (model.InstallationDate != "")
                        assetDetailObj.InstallationDate = DateTime.Parse(model.InstallationDate);
                    assetDetailObj.RoomId = model.RoomId;
                    assetDetailObj.FloorId = model.FloorId;
                    assetDetailObj.BuildingId = model.BuildingId;
                    if (model.ReceivingDate != "")
                        assetDetailObj.ReceivingDate = DateTime.Parse(model.ReceivingDate);
                    if (model.OperationDate != "")
                        assetDetailObj.OperationDate = DateTime.Parse(model.OperationDate);
                    assetDetailObj.PONumber = model.PONumber;
                    assetDetailObj.DepartmentId = model.DepartmentId;
                    if (model.SupplierId > 0)
                        assetDetailObj.SupplierId = model.SupplierId;
                    assetDetailObj.HospitalId = model.HospitalId;
                    assetDetailObj.MasterAssetId = model.MasterAssetId;
                    if (model.WarrantyStart != "")
                        assetDetailObj.WarrantyStart = DateTime.Parse(model.WarrantyStart);
                    if (model.WarrantyEnd != "")
                        assetDetailObj.WarrantyEnd = DateTime.Parse(model.WarrantyEnd);

                    assetDetailObj.CreatedBy = model.CreatedBy;
                    assetDetailObj.DepreciationRate = model.DepreciationRate;
                    assetDetailObj.CostCenter = model.CostCenter;
                    assetDetailObj.WarrantyExpires = model.WarrantyExpires;
                    assetDetailObj.FixCost = model.FixCost;

                    _context.AssetDetails.Add(assetDetailObj);
                    _context.SaveChanges();

                    model.Id = assetDetailObj.Id;
                    int assetDetailId = model.Id;

                    if (model.ListOwners.Count > 0)
                    {
                        foreach (var item in model.ListOwners)
                        {
                            AssetOwner ownerObj = new AssetOwner();
                            ownerObj.AssetDetailId = assetDetailId;
                            ownerObj.EmployeeId = int.Parse(item.ToString());
                            ownerObj.HospitalId = int.Parse(model.HospitalId.ToString());
                            _context.AssetOwners.Add(ownerObj);
                            _context.SaveChanges();
                        }
                    }
                    if (model.MasterAssetId > 0 && model.InstallationDate != null)
                    {
                        var dates = new List<DateTime>();
                        var masterObj = _context.MasterAssets.Find(model.MasterAssetId);
                        var pmtimeId = masterObj.PMTimeId;
                        if (pmtimeId == 1)
                        {
                            var pmdate = DateTime.Parse(model.InstallationDate).AddYears(1);
                            if (pmdate.DayOfWeek == DayOfWeek.Friday)
                            {
                                pmdate = pmdate.AddDays(-1);
                            }
                            if (pmdate.DayOfWeek == DayOfWeek.Saturday)
                            {
                                pmdate = pmdate.AddDays(1);
                            }

                        }
                        if (pmtimeId == 2)
                        {

                            for (var dt = DateTime.Parse(model.InstallationDate).AddDays(1); dt <= DateTime.Parse(model.InstallationDate).AddYears(1); dt = dt.AddMonths(6))
                            {
                                if (dt.DayOfWeek == DayOfWeek.Friday)
                                {
                                    dt = dt.AddDays(-1);
                                }
                                if (dt.DayOfWeek == DayOfWeek.Saturday)
                                {
                                    dt = dt.AddDays(1);
                                }

                                dates.Add(dt);
                            }

                        }
                        if (pmtimeId == 3)
                        {
                            for (var dt = DateTime.Parse(model.InstallationDate).AddDays(1); dt <= DateTime.Parse(model.InstallationDate).AddYears(1); dt = dt.AddMonths(3))
                            {
                                if (dt.DayOfWeek == DayOfWeek.Friday)
                                {
                                    dt = dt.AddDays(-1);
                                }
                                if (dt.DayOfWeek == DayOfWeek.Saturday)
                                {
                                    dt = dt.AddDays(1);
                                }

                                dates.Add(dt);
                            }
                        }

                        if (dates.Count > 0)
                        {
                            foreach (var item in dates)
                            {
                                PMAssetTime assetTimeObj = new PMAssetTime();
                                assetTimeObj.PMDate = item;
                                assetTimeObj.AssetDetailId = assetDetailId;
                                assetTimeObj.HospitalId = model.HospitalId;
                                _context.PMAssetTimes.Add(assetTimeObj);
                                _context.SaveChanges();
                            }
                        }
                    }

                    if (model.BuildingId != 0 && model.FloorId != 0 && model.RoomId != 0)
                    {
                        AssetMovement movementObj = new AssetMovement();
                        movementObj.RoomId = model.RoomId;
                        movementObj.FloorId = model.FloorId;
                        movementObj.BuildingId = model.BuildingId;
                        movementObj.HospitalId = model.HospitalId;
                        _context.AssetMovements.Add(movementObj);
                        _context.SaveChanges();
                    }

                    var date = DateTime.UtcNow;
                    DateTime date2 = date.ToLocalTime();
                    var lstTransactions = _context.AssetStatusTransactions.Where(a => a.AssetDetailId == assetDetailId && a.HospitalId == model.HospitalId).ToList();
                    if (lstTransactions.Count == 0)
                    {
                        AssetStatusTransaction transObj = new AssetStatusTransaction();
                        transObj.AssetDetailId = assetDetailId;
                        transObj.AssetStatusId = int.Parse(model.AssetStatusId.ToString());
                        transObj.StatusDate = date2;
                        transObj.HospitalId = model.HospitalId;
                        _context.AssetStatusTransactions.Add(transObj);
                        _context.SaveChanges();

                        if (int.Parse(model.AssetStatusId.ToString()) == 4)
                        {
                            GeneratedRequestNumberVM numberObj = new GeneratedRequestNumberVM();
                            string reqCode = "Req";

                            var lstIds = _context.Request.ToList();
                            if (lstIds.Count > 0)
                            {
                                var code = lstIds.Max(a => a.Id);
                                numberObj.RequestCode = reqCode + (code + 1);
                            }
                            else
                            {
                                numberObj.RequestCode = reqCode + 1;
                            }

                            Request request = new Request();
                            request.Subject = "إضافة جهاز تحت الصيانة";
                            request.RequestCode = numberObj.RequestCode;
                            request.Description = "إضافة جهاز تحت الصيانة";

                            request.RequestDate = date2;
                            request.RequestModeId = 5;
                            request.RequestPeriorityId = 4;
                            request.AssetDetailId = assetDetailId;

                            var lstUsers = (from userRole in _context.UserRoles
                                            join usr in _context.Users on userRole.UserId equals usr.Id
                                            join role in _context.Roles on userRole.RoleId equals role.Id
                                            where role.Name == "AssetOwner"
                                            && usr.HospitalId == model.HospitalId
                                            && usr.UserName != "hospitalao"
                                            && (usr.Email != "hospitalao@gmail.com" || usr.Email != "hospitalao@assets.com")
                                            select usr).ToList();
                            if (lstUsers.Count > 0)
                            {
                                request.CreatedById = lstUsers[0].Id;
                            }
                            else
                            {
                                var lstUsers2 = (from userRole in _context.UserRoles
                                                 join usr in _context.Users on userRole.UserId equals usr.Id
                                                 join role in _context.Roles on userRole.RoleId equals role.Id
                                                 where role.Name == "AssetOwner"
                                                 && usr.HospitalId == model.HospitalId
                                                 select usr).ToList();
                                if (lstUsers2.Count > 0)
                                {
                                    request.CreatedById = lstUsers2[0].Id;
                                }
                            }
                            request.HospitalId = model.HospitalId;
                            request.IsOpened = false;
                            request.RequestTypeId = 2;
                            _context.Request.Add(request);
                            _context.SaveChanges();
                            var RequestId = request.Id;

                            RequestTracking requestTracking = new RequestTracking();
                            requestTracking.Description = " إضافة جهاز تحت الصيانة عند إضافة أصل جديد";
                            requestTracking.DescriptionDate = date2;
                            requestTracking.RequestId = RequestId;
                            requestTracking.RequestStatusId = 1;
                            var lstReqTrackingUsers = (from userRole in _context.UserRoles
                                                       join usr in _context.Users on userRole.UserId equals usr.Id
                                                       join role in _context.Roles on userRole.RoleId equals role.Id
                                                       where role.Name == "AssetOwner"
                                                       && usr.HospitalId == model.HospitalId
                                                       && usr.UserName != "hospitalao"
                                                       && (usr.Email != "hospitalao@gmail.com" || usr.Email != "hospitalao@assets.com")
                                                       select usr).ToList();
                            if (lstReqTrackingUsers.Count > 0)
                            {
                                requestTracking.CreatedById = lstReqTrackingUsers[0].Id;
                            }
                            else
                            {
                                var lstReqTrackingUsers2 = (from userRole in _context.UserRoles
                                                            join usr in _context.Users on userRole.UserId equals usr.Id
                                                            join role in _context.Roles on userRole.RoleId equals role.Id
                                                            where role.Name == "AssetOwner"
                                                            && usr.HospitalId == model.HospitalId
                                                            select usr).ToList();
                                if (lstReqTrackingUsers2.Count > 0)
                                {
                                    requestTracking.CreatedById = lstReqTrackingUsers2[0].Id;
                                }
                            }
                            requestTracking.HospitalId = model.HospitalId;
                            requestTracking.IsOpened = false;
                            _context.RequestTracking.Add(requestTracking);
                            _context.SaveChanges();

                        }
                    }
                    return assetDetailId;
                }
            }
            catch (SqlException ex)
            {
                string str = ex.Message;
            }
            catch (Exception ex)
            {
                string str = ex.Message;
            }
            return assetDetailObj.Id;
        }

        /// <summary>
        /// Update Asset Detail
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Update(EditAssetDetailVM model)
        {
            try
            {

                var assetDetailObj = _context.AssetDetails.Find(model.Id);
                assetDetailObj.Id = model.Id;
                assetDetailObj.Code = model.Code;
                assetDetailObj.PurchaseDate = model.PurchaseDate != null ? DateTime.Parse(model.PurchaseDate) : null;
                assetDetailObj.Price = model.Price;
                assetDetailObj.SerialNumber = model.SerialNumber;
                assetDetailObj.Remarks = model.Remarks;
                assetDetailObj.Barcode = model.Barcode;
                assetDetailObj.CreatedBy = model.CreatedBy;
                assetDetailObj.FixCost = model.FixCost;
                assetDetailObj.InstallationDate = model.InstallationDate != null ? DateTime.Parse(model.InstallationDate) : null;

                var lstAssetMovements = _context.AssetMovements.Where(a => a.AssetDetailId == model.Id).ToList();
                if (lstAssetMovements.Count == 0)
                {
                    if (model.BuildingId != 0 && model.FloorId != 0 && model.RoomId != 0)
                    {
                        AssetMovement movementObj = new AssetMovement();
                        movementObj.RoomId = model.RoomId;
                        movementObj.FloorId = model.FloorId;
                        movementObj.BuildingId = model.BuildingId;
                        movementObj.AssetDetailId = model.Id;
                        movementObj.MovementDate = DateTime.Now;
                        _context.AssetMovements.Add(movementObj);
                        _context.SaveChanges();
                    }
                }

                assetDetailObj.RoomId = model.RoomId;
                assetDetailObj.FloorId = model.FloorId;
                assetDetailObj.BuildingId = model.BuildingId;
                assetDetailObj.ReceivingDate = model.ReceivingDate != null ? DateTime.Parse(model.ReceivingDate) : null;
                assetDetailObj.OperationDate = model.OperationDate != null ? DateTime.Parse(model.OperationDate) : null;
                assetDetailObj.PONumber = model.PONumber;
                assetDetailObj.DepartmentId = model.DepartmentId;
                assetDetailObj.SupplierId = model.SupplierId;
                assetDetailObj.HospitalId = model.HospitalId;
                assetDetailObj.MasterAssetId = model.MasterAssetId;
                assetDetailObj.WarrantyStart = model.WarrantyStart != null ? DateTime.Parse(model.WarrantyStart) : null;
                assetDetailObj.WarrantyEnd = model.WarrantyEnd != null ? DateTime.Parse(model.WarrantyEnd) : null;
                assetDetailObj.WarrantyExpires = model.WarrantyExpires;
                assetDetailObj.DepreciationRate = model.DepreciationRate;
                assetDetailObj.CostCenter = model.CostCenter;

                if (assetDetailObj.QrFilePath == null)
                {
                    string url = model.DomainName + "#/dash/hospitalassets/detail/" + model.Id;
                    QRCodeGenerator qrGenerator = new QRCodeGenerator();
                    QRCodeData qrCodeData = qrGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.L);
                    assetDetailObj.QrFilePath = url;
                }
                else
                {
                    assetDetailObj.QrFilePath = model.QrFilePath;

                }


                if (assetDetailObj.QrData == null)
                {
                    var lstAssets = _context.AssetDetails.Include(a => a.Department).Include(a => a.MasterAsset).Include(a => a.MasterAsset.brand).Where(a => a.Id == model.Id).ToList();
                    if (lstAssets.Count > 0)
                    {
                        var assetObj = lstAssets[0];
                        string assetObjData = "AssetName " + assetObj.MasterAsset.Name + ";\nManufacture " + assetObj.MasterAsset.brand.Name + ";\nModel " + assetObj.MasterAsset.ModelNumber + ";\nSerialNumber " + assetObj.SerialNumber + ";\nBarcode " + assetObj.Barcode + ";\nDepartment " + assetObj.Department.Name;
                        QRCodeGenerator qrGenerator1 = new QRCodeGenerator();
                        QRCodeData qrCodeData1 = qrGenerator1.CreateQrCode(assetObjData, QRCodeGenerator.ECCLevel.L, true);
                        assetDetailObj.QrData = assetObjData;
                    }
                    else
                    {
                        assetDetailObj.QrData = model.QrData;
                    }
                }
                _context.Entry(assetDetailObj).State = EntityState.Modified;
                _context.SaveChanges();


                List<int> oldIds = new List<int>();
                List<int> newIds = new List<int>();

                var lstSavedIds = _context.AssetOwners.Where(a => a.AssetDetailId == assetDetailObj.Id).Select(q => q.EmployeeId).ToList();
                foreach (var saved in lstSavedIds)
                {
                    oldIds.Add(int.Parse(saved.ToString()));
                }


                var lstNewIds = model.ListOwners;
                foreach (var news in lstNewIds)
                {
                    newIds.Add(int.Parse(news.ToString()));
                }


                var savedIds = oldIds.Except(newIds);
                foreach (var item in savedIds)
                {
                    var row = _context.AssetOwners.Where(a => a.AssetDetailId == assetDetailObj.Id && a.EmployeeId == item).ToList();
                    if (row.Count > 0)
                    {
                        var ownerObj = row[0];
                        _context.AssetOwners.Remove(ownerObj);
                        _context.SaveChanges();
                    }
                }


                var neewIds = newIds.Except(oldIds);
                foreach (var itm in neewIds)
                {
                    AssetOwner ownerObj = new AssetOwner();
                    ownerObj.AssetDetailId = assetDetailObj.Id;
                    ownerObj.EmployeeId = int.Parse(itm.ToString());
                    ownerObj.HospitalId = int.Parse(model.HospitalId.ToString());
                    _context.AssetOwners.Add(ownerObj);
                    _context.SaveChanges();
                }

                if (model.MasterAssetId > 0 && model.InstallationDate != null)
                {
                    var dates = new List<DateTime>();
                    var masterObj = _context.MasterAssets.Find(model.MasterAssetId);


                    var pmtimeId = masterObj.PMTimeId;
                    if (pmtimeId == 1)
                    {
                        var pmdate = DateTime.Parse(model.InstallationDate).AddYears(1);
                        if (pmdate.DayOfWeek == DayOfWeek.Friday)
                        {
                            pmdate = pmdate.AddDays(-1);
                        }
                        if (pmdate.DayOfWeek == DayOfWeek.Saturday)
                        {
                            pmdate = pmdate.AddDays(1);
                        }

                    }
                    if (pmtimeId == 2)
                    {

                        for (var dt = DateTime.Parse(model.InstallationDate).AddDays(1); dt <= DateTime.Parse(model.InstallationDate).AddYears(1); dt = dt.AddMonths(6))
                        {
                            if (dt.DayOfWeek == DayOfWeek.Friday)
                            {
                                dt = dt.AddDays(-1);
                            }
                            if (dt.DayOfWeek == DayOfWeek.Saturday)
                            {
                                dt = dt.AddDays(1);
                            }

                            dates.Add(dt);
                        }

                    }
                    if (pmtimeId == 3)
                    {
                        for (var dt = DateTime.Parse(model.InstallationDate).AddDays(1); dt <= DateTime.Parse(model.InstallationDate).AddYears(1); dt = dt.AddMonths(3))
                        {
                            if (dt.DayOfWeek == DayOfWeek.Friday)
                            {
                                dt = dt.AddDays(-1);
                            }
                            if (dt.DayOfWeek == DayOfWeek.Saturday)
                            {
                                dt = dt.AddDays(1);
                            }

                            dates.Add(dt);
                        }
                    }

                    if (dates.Count > 0)
                    {
                        //   var lstSavedDates = _context.PMAssetTimes.ToList().Where(a=>a.Id == model.Id)

                        foreach (var item in dates)
                        {
                            PMAssetTime assetTimeObj = new PMAssetTime();
                            assetTimeObj.PMDate = item;
                            assetTimeObj.AssetDetailId = model.Id;
                            assetTimeObj.HospitalId = model.HospitalId;
                            _context.PMAssetTimes.Add(assetTimeObj);
                            _context.SaveChanges();
                        }
                    }
                }



                return assetDetailObj.Id;
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }

            return 0;
        }

        /// <summary>
        /// Create Documents and Files to each Asset Detail
        /// </summary>
        /// <param name="attachObj"></param>
        /// <returns></returns>
        public int CreateAssetDetailDocuments(CreateAssetDetailAttachmentVM attachObj)
        {
            AssetDetailAttachment assetAttachmentObj = new AssetDetailAttachment();
            assetAttachmentObj.AssetDetailId = attachObj.AssetDetailId;
            assetAttachmentObj.Title = attachObj.Title;
            assetAttachmentObj.FileName = attachObj.FileName;
            assetAttachmentObj.HospitalId = attachObj.HospitalId;
            _context.AssetDetailAttachments.Add(assetAttachmentObj);
            _context.SaveChanges();
            int Id = assetAttachmentObj.Id;
            return Id;
        }

        /// <summary>
        /// Delete File for Asset Detail
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int DeleteAssetDetailAttachment(int id)
        {
            try
            {
                var attachObj = _context.AssetDetailAttachments.Find(id);
                _context.AssetDetailAttachments.Remove(attachObj);
                return _context.SaveChanges();
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }

            return 0;
        }

        /// <summary>
        /// In Edit Asset If I need to add new Document and there's already list of files before
        /// </summary>
        /// <param name="assetDetailId"></param>
        /// <returns></returns>
        public AssetDetailAttachment GetLastDocumentForAssetDetailId(int assetDetailId)
        {
            AssetDetailAttachment documentObj = new AssetDetailAttachment();
            var lstDocuments = _context.AssetDetailAttachments.Where(a => a.AssetDetailId == assetDetailId).OrderBy(a => a.FileName).ToList();
            if (lstDocuments.Count > 0)
            {
                documentObj = lstDocuments.Last();
            }
            return documentObj;
        }


        /// <summary>
        /// Get List of Attachments By AssetId
        /// </summary>
        /// <param name="assetId"></param>
        /// <returns></returns>
        public IEnumerable<AssetDetailAttachment> GetAttachmentByAssetDetailId(int assetId)
        {
            return _context.AssetDetailAttachments.Where(a => a.AssetDetailId == assetId).OrderBy(a => a.FileName).ToList();
        }

        /// <summary>
        /// Alert Assets WarrantyEnd Before 3 Monthes of Expire In DashBoard Page
        /// </summary>
        /// <param name="duration"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IndexAssetDetailVM AlertAssetsWarrantyEndBefore3Monthes(int hospitalId, int duration, int pageNumber, int pageSize)
        {
            int durationDays = 0;
            if (duration == 1)
                durationDays = 30;
            if (duration == 2)
                durationDays = 60;
            if (duration == 3)
                durationDays = 90;

            IndexAssetDetailVM mainClass = new IndexAssetDetailVM();
            List<IndexAssetDetailVM.GetData> lstAssetDetails = new List<IndexAssetDetailVM.GetData>();
            List<AssetDetail> allAssetDetails = _context.AssetDetails.Include(a => a.MasterAsset).Include(a => a.MasterAsset.brand)
                                            .Include(a => a.Hospital).Include(a => a.Department).Include(a => a.Hospital.Governorate)
                                            .Include(a => a.Hospital.City)
                                            .Where(a => a.WarrantyEnd.HasValue && a.WarrantyEnd != null).OrderBy(a => a.Id).ToList();

            if (hospitalId != 0)
                allAssetDetails = allAssetDetails.Where(a => a.HospitalId == hospitalId).ToList();
            else
                allAssetDetails = allAssetDetails.ToList();


            if (allAssetDetails.Count > 0)
            {
                foreach (var itm in allAssetDetails)
                {
                    IndexAssetDetailVM.GetData item = new IndexAssetDetailVM.GetData();
                    item.Id = itm.Id;
                    item.Code = itm.Code;
                    item.Model = itm.MasterAsset.ModelNumber;
                    item.Price = itm.Price;
                    item.Serial = itm.SerialNumber;
                    item.BarCode = itm.Barcode;
                    item.SerialNumber = itm.SerialNumber;
                    item.PurchaseDate = itm.PurchaseDate;
                    item.SupplierId = itm.SupplierId;
                    item.DepartmentId = itm.DepartmentId;
                    item.AssetName = itm.MasterAssetId > 0 ? itm.MasterAsset.Name : "";
                    item.AssetNameAr = itm.MasterAssetId > 0 ? itm.MasterAsset.NameAr : "";
                    item.DepartmentName = itm.DepartmentId > 0 ? itm.Department.Name : "";
                    item.DepartmentNameAr = itm.DepartmentId > 0 ? itm.Department.NameAr : "";

                    item.BrandName = itm.MasterAsset.brand != null ? itm.MasterAsset.brand.Name : "";
                    item.BrandNameAr = itm.MasterAsset.brand != null ? itm.MasterAsset.brand.NameAr : "";
                    item.WarrantyEnd = itm.WarrantyEnd;

                    if (duration != 0)
                    {
                        var days = NumberOfDateDaysPassed(itm.WarrantyEnd.Value, durationDays);
                        if (days > 0)
                        {
                            item.EndWarrantyDate = days.ToString();
                        }
                        var isPassed = CheckIfDateHasPasseddurationDays(itm.WarrantyEnd.Value, durationDays);
                        if (isPassed)
                        {
                            lstAssetDetails.Add(item);
                        }
                    }
                }
            }
            if (lstAssetDetails.Count > 0)
            {
                lstAssetDetails.RemoveAll(s => s.WarrantyEnd == null);
                var itemsPerPage = lstAssetDetails.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                mainClass.Results = itemsPerPage;
                mainClass.Count = lstAssetDetails.Count();
            }
            else
            {
                mainClass.Results = new List<IndexAssetDetailVM.GetData>();
                mainClass.Count = lstAssetDetails.Count();
            }
            return mainClass;

        }



        private bool CheckIfDateHasPasseddurationDays(DateTime targetDate, int durationDays)
        {
            DateTime currentDate = DateTime.Now;
            DateTime daysAgo = currentDate.AddDays(-durationDays);
            return targetDate <= daysAgo;
        }



        private double NumberOfDateDaysPassed(DateTime targetDate, int durationDays)
        {
            DateTime currentDate = DateTime.Now;
            DateTime daysAgo = currentDate.AddDays(-durationDays);
            return (currentDate - daysAgo).TotalDays;
        }


        /// <summary>
        /// Geo Search Page Use GovernorateId,DepartmentId and HospitalId with Paging
        /// </summary>
        /// <param name="departmentId"></param>
        /// <param name="govId"></param>
        /// <param name="hospitalId"></param>
        /// <param name="userId"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IndexAssetDetailVM GetHospitalAssetsByGovIdAndDeptIdAndHospitalId(int departmentId, int govId, int hospitalId, string userId, int pageNumber, int pageSize)
        {

            IndexAssetDetailVM mainClass = new IndexAssetDetailVM();
            List<IndexAssetDetailVM.GetData> list = new List<IndexAssetDetailVM.GetData>();



            ApplicationUser UserObj = new ApplicationUser();
            List<string> userRoleNames = new List<string>();


            var obj = _context.ApplicationUser.Where(a => a.Id == userId).ToList();
            if (obj.Count > 0)
            {
                UserObj = obj[0];
                var roleNames = (from userRole in _context.UserRoles
                                 join role in _context.Roles on userRole.RoleId equals role.Id
                                 where userRole.UserId == userId
                                 select role);
                foreach (var name in roleNames)
                {
                    userRoleNames.Add(name.Name);
                }
            }



            var lstAllAssets = _context.AssetDetails.Include(a => a.MasterAsset).Include(a => a.Hospital).Include(a => a.Department)
                                     .Include(a => a.Supplier).Include(a => a.MasterAsset.brand)
                                     .Include(a => a.Hospital.Governorate).Include(a => a.Hospital.City)
                                     .Include(a => a.Hospital.Organization).Include(a => a.Hospital.SubOrganization)
                                     .OrderBy(a => a.Barcode).ToList();

            if (lstAllAssets.Count > 0)
            {
                foreach (var asset in lstAllAssets)
                {


                    IndexAssetDetailVM.GetData detail = new IndexAssetDetailVM.GetData();
                    detail.Id = asset.Id;
                    detail.DepartmentId = asset.DepartmentId != null ? asset.DepartmentId : 0;
                    detail.Code = asset.Code;
                    detail.Price = asset.Price;
                    detail.BarCode = asset.Barcode;
                    detail.MasterImg = asset.MasterAsset.AssetImg;
                    detail.Serial = asset.SerialNumber;
                    detail.BrandName = asset.MasterAsset.brand != null ? asset.MasterAsset.brand.Name : "";
                    detail.BrandNameAr = asset.MasterAsset.brand != null ? asset.MasterAsset.brand.NameAr : "";
                    detail.Model = asset.MasterAsset.ModelNumber;
                    detail.SerialNumber = asset.SerialNumber;
                    detail.MasterAssetId = asset.MasterAssetId;
                    detail.PurchaseDate = asset.PurchaseDate;
                    detail.HospitalId = asset.Hospital.Id;
                    detail.HospitalName = asset.Hospital.Name;
                    detail.HospitalNameAr = asset.Hospital.NameAr;
                    detail.AssetName = asset.MasterAsset.Name;
                    detail.AssetNameAr = asset.MasterAsset.NameAr;
                    detail.DepartmentId = asset.Department != null ? asset.DepartmentId : 0;
                    detail.DepartmentName = asset.Department != null ? asset.Department.Name : "";
                    detail.DepartmentNameAr = asset.Department != null ? asset.Department.NameAr : "";
                    detail.GovernorateId = asset.Hospital.GovernorateId;
                    detail.GovernorateName = asset.Hospital.Governorate.Name;
                    detail.GovernorateNameAr = asset.Hospital.Governorate.NameAr;
                    detail.CityId = asset.Hospital.CityId;
                    detail.CityName = asset.Hospital.City.Name;
                    detail.CityNameAr = asset.Hospital.City.NameAr;
                    detail.OrganizationId = asset.Hospital.OrganizationId;
                    detail.OrgName = asset.Hospital.Organization.Name;
                    detail.OrgNameAr = asset.Hospital.Organization.NameAr;
                    detail.SubOrganizationId = asset.Hospital.SubOrganizationId;
                    detail.SubOrgName = asset.Hospital.SubOrganization.Name;
                    detail.SubOrgNameAr = asset.Hospital.SubOrganization.NameAr;
                    detail.SupplierName = asset.Supplier != null ? asset.Supplier.Name : "";
                    detail.SupplierNameAr = asset.Supplier != null ? asset.Supplier.NameAr : "";
                    detail.QrFilePath = asset.QrFilePath;
                    detail.QrData = asset.QrData;



                    list.Add(detail);
                }





                if (govId != 0)
                {

                    list = list.Where(h => h.GovernorateId == govId).ToList();
                }
                if (hospitalId != 0)
                {
                    list = list.Where(h => h.HospitalId == hospitalId).ToList();
                }
                if (departmentId != 0)
                {


                    list = list.Where(h => h.DepartmentId == departmentId && h.HospitalId == hospitalId).ToList();
                }
                else
                {
                    list = list.ToList();
                }
                var requestsPerPage = list.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                mainClass.Results = requestsPerPage;
                mainClass.Count = list.Count();
                return mainClass;
            }
            return null;


        }






        #region Generic Report

        private IQueryable<AssetDetail> GetLstAssetDetails()
        {
            return _context.AssetDetails.Include(a => a.MasterAsset).Include(a => a.MasterAsset.brand)
                         .Include(a => a.Supplier).Include(a => a.Department)
                         .Include(a => a.Hospital).ThenInclude(h => h.Organization)
                         .Include(a => a.Hospital).ThenInclude(h => h.Governorate)
                         .Include(a => a.Hospital).ThenInclude(h => h.City)
                         .Include(a => a.Hospital).ThenInclude(h => h.SubOrganization).OrderBy(q => q.Barcode);
        }
        public IndexAssetDetailVM GenericReportGetAssetsByUserIdAndPaging(string userId, int pageNumber, int pageSize)
        {
            IndexAssetDetailVM mainClass = new IndexAssetDetailVM();
            List<IndexAssetDetailVM.GetData> list = new List<IndexAssetDetailVM.GetData>();
            if (userId != null)
            {
                Employee empObj = new Employee();
                List<string> userRoleNames = new List<string>();
                var obj = _context.ApplicationUser.Where(a => a.Id == userId).ToList();
                var userObj = obj[0];


                var roles = (from userRole in _context.UserRoles
                             join role in _context.ApplicationRole on userRole.RoleId equals role.Id
                             where userRole.UserId == userObj.Id
                             select role);
                foreach (var role in roles)
                {
                    userRoleNames.Add(role.Name);
                }

                var lstEmployees = _context.Employees.Where(a => a.Email == userObj.Email).ToList();
                if (lstEmployees.Count > 0)
                {
                    empObj = lstEmployees[0];
                }

                mainClass.Count = _context.AssetDetails.Count();

                var lstAllAssets = _context.AssetDetails.Include(a => a.MasterAsset).Include(a => a.Hospital)
                                    .Include(a => a.Department).Include(a => a.Supplier).Include(a => a.MasterAsset.brand)
                                    .Include(a => a.Hospital.Governorate).Include(a => a.Hospital.City)
                                    .Include(a => a.Hospital.Organization).Include(a => a.Hospital.SubOrganization)
                                    .OrderBy(a => a.Barcode).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

                if (lstAllAssets.Count > 0)
                {
                    foreach (var asset in lstAllAssets)
                    {
                        IndexAssetDetailVM.GetData detail = new IndexAssetDetailVM.GetData();

                        var lstStatus = _context.AssetStatusTransactions.Where(a => a.AssetDetailId == asset.Id).OrderByDescending(a => a.StatusDate).ToList();
                        if (lstStatus.Count > 0)
                        {
                            detail.AssetStatusId = lstStatus.FirstOrDefault().AssetStatusId;
                        }
                        detail.Id = asset.Id;
                        detail.DepartmentId = asset.DepartmentId != null ? asset.DepartmentId : 0;
                        detail.Code = asset.Code;
                        detail.UserId = userObj.Id;
                        detail.Price = asset.Price;
                        detail.BarCode = asset.Barcode;
                        detail.MasterImg = asset.MasterAsset.AssetImg;
                        detail.Serial = asset.SerialNumber;
                        detail.BrandName = asset.MasterAsset.brand != null ? asset.MasterAsset.brand.Name : "";
                        detail.BrandNameAr = asset.MasterAsset.brand != null ? asset.MasterAsset.brand.NameAr : "";
                        detail.Model = asset.MasterAsset.ModelNumber;
                        detail.SerialNumber = asset.SerialNumber;
                        detail.MasterAssetId = asset.MasterAssetId;
                        detail.PurchaseDate = asset.PurchaseDate;
                        detail.HospitalId = asset.Hospital.Id;
                        detail.HospitalName = asset.Hospital.Name;
                        detail.HospitalNameAr = asset.Hospital.NameAr;
                        detail.AssetName = asset.MasterAsset.Name;
                        detail.AssetNameAr = asset.MasterAsset.NameAr;
                        detail.DepartmentId = asset.Department != null ? asset.DepartmentId : 0;
                        detail.DepartmentName = asset.Department != null ? asset.Department.Name : "";
                        detail.DepartmentNameAr = asset.Department != null ? asset.Department.NameAr : "";
                        detail.GovernorateId = asset.Hospital.GovernorateId;
                        detail.GovernorateName = asset.Hospital.Governorate.Name;
                        detail.GovernorateNameAr = asset.Hospital.Governorate.NameAr;
                        detail.CityId = asset.Hospital.CityId;
                        detail.CityName = asset.Hospital.City.Name;
                        detail.CityNameAr = asset.Hospital.City.NameAr;
                        detail.OrganizationId = asset.Hospital.OrganizationId;
                        detail.OrgName = asset.Hospital.Organization.Name;
                        detail.OrgNameAr = asset.Hospital.Organization.NameAr;
                        detail.SubOrganizationId = asset.Hospital.SubOrganizationId;
                        detail.SubOrgName = asset.Hospital.SubOrganization.Name;
                        detail.SubOrgNameAr = asset.Hospital.SubOrganization.NameAr;
                        detail.SupplierName = asset.Supplier != null ? asset.Supplier.Name : "";
                        detail.SupplierNameAr = asset.Supplier != null ? asset.Supplier.NameAr : "";
                        detail.QrFilePath = asset.QrFilePath;
                        list.Add(detail);
                    }
                }

                mainClass.Results = list;

                return mainClass;
            }
            return null;

        }


        public List<DrawChart> DrawingChart()
        {
            List<DrawChart> list = new List<DrawChart>();
            var lstOrgs = _context.Organizations.ToList();
            var lstHospital = _context.Hospitals.Include(a => a.Governorate).Include(a => a.Organization).ToList().GroupBy(c => c.OrganizationId);

            if (lstHospital.Count() > 0)
            {
                foreach (var host in lstHospital)
                {
                    DrawChart chartObj = new DrawChart();
                    //Key
                    chartObj.OrganizationId = host.FirstOrDefault().Organization.Id;
                    chartObj.OrganizationName = host.FirstOrDefault().Organization.Name;
                    chartObj.OrganizationNameAr = host.FirstOrDefault().Organization.NameAr;

                    List<DrawBarChart> listBars = new List<DrawBarChart>();

                    foreach (var gov in host.Where(c => c.GovernorateId == host.FirstOrDefault().GovernorateId).ToList())
                    {
                        DrawBarChart barObj = new DrawBarChart();
                        var CountOfAsset = _context.AssetDetails.Where(a => a.Hospital.GovernorateId == gov.Id).Count();
                        barObj.AssetCount = CountOfAsset;
                        barObj.GovernorateId = gov.Id;
                        barObj.GovernorateName = gov.Name;
                        barObj.GovernorateNameAr = gov.NameAr;
                        listBars.Add(barObj);
                    }
                    chartObj.ListBars = listBars;
                    list.Add(chartObj);

                }
            }
            return list;
        }

        public IndexAssetDetailVM FilterDataByDepartmentBrandSupplierIdAndPaging(FilterHospitalAsset data, string userId, int pageNumber, int pageSize)
        {


            IQueryable<AssetDetail> query = null;
            IndexAssetDetailVM mainClass = new IndexAssetDetailVM();
            List<IndexAssetDetailVM.GetData> list = new List<IndexAssetDetailVM.GetData>();
            List<AssetDetail> searchResult = new List<AssetDetail>();


            if (data.AssetNameAr != "")
            {


                query = GetLstAssetDetails().Where(e => e.MasterAsset.NameAr == data.AssetNameAr);
                mainClass.Count = query.Count();
                if (query != null)
                    searchResult = GetLstAssetDetails().Where(e => e.MasterAsset.NameAr == data.AssetNameAr).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();


            }

            if (data.AssetName != "")
            {
                if (query is not null)
                {
                    query = query.Where(e => e.MasterAsset.Name == data.AssetName);
                }
                else if (query is null)
                {
                    query = GetLstAssetDetails().Where(e => e.MasterAsset.Name == data.AssetName);
                }

                mainClass.Count = query.Count();
                searchResult = query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            }



            if (data.DepartmentId != 0)
            {
                if (query is not null)
                {
                    query = query.Where(e => e.DepartmentId == data.DepartmentId);

                }
                else if (query is null)

                {
                    query = GetLstAssetDetails().Where(e => e.DepartmentId == data.DepartmentId);
                }

                mainClass.Count = query.Count();
                searchResult = query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();


            }


            if (data.BrandId != 0)
            {
                if (query is not null)
                {
                    query = query.Where(e => e.MasterAsset.BrandId == data.BrandId);

                }
                else if (query is null)
                {
                    query = GetLstAssetDetails().Where(e => e.MasterAsset.BrandId == data.BrandId);
                }

                mainClass.Count = query.Count();
                searchResult = query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();


            }


            if (data.SupplierId != 0)
            {
                if (query is not null)
                {
                    query = query.Where(ww => ww.SupplierId == data.SupplierId);
                }
                else if (query is null)
                {
                    query = GetLstAssetDetails().Where(ww => ww.SupplierId == data.SupplierId);
                }
                mainClass.Count = query.Count();
                searchResult = query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            }

            if (data.MasterAssetId != 0)
            {
                if (query is not null)
                {
                    query = query.Where(ww => ww.MasterAssetId == data.MasterAssetId);
                }
                else if (query is null)
                {
                    query = GetLstAssetDetails().Where(ww => ww.MasterAssetId == data.MasterAssetId);
                }
                mainClass.Count = query.Count();
                searchResult = query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            }



            if (data.GovernorateId != 0)
            {
                if (query is not null)
                {
                    query = query.Where(e => e.Hospital.GovernorateId == data.GovernorateId);

                }
                else if (query is null)
                {
                    query = GetLstAssetDetails().Where(e => e.Hospital.GovernorateId == data.GovernorateId);
                }

                mainClass.Count = query.Count();
                searchResult = query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            }




            if (data.CityId != 0)
            {
                if (query is not null)
                {
                    query = query.Where(e => e.Hospital.CityId == data.CityId);

                }
                else if (query is null)

                {
                    query = GetLstAssetDetails().Where(e => e.Hospital.CityId == data.CityId);
                }

                mainClass.Count = query.Count();
                searchResult = query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            }





            if (data.HospitalId != 0)
            {
                if (query is not null)
                {
                    query = query.Where(e => e.Hospital.Id == data.HospitalId);

                }
                else if (query is null)

                {
                    query = GetLstAssetDetails().Where(e => e.Hospital.Id == data.HospitalId);
                }

                mainClass.Count = query.Count();
                searchResult = query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            }



            if (data.HospitalIds.Count > 0)
            {
                if (query is not null)
                {

                    query = query.Where(e => data.HospitalIds.Contains((int)e.HospitalId));

                }
                else if (query is null)

                {
                    query = GetLstAssetDetails().Where(e => data.HospitalIds.Contains((int)e.HospitalId));
                }

                mainClass.Count = query.Count();
                searchResult = query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            }


            if (data.StatusId != 0)
            {
                if (query is not null)
                {
                    var listQuery = query.ToList();
                    var lastTransationForAssets = _context.Set<AssetStatusTransaction>()
                        .FromSqlRaw("EXEC GetAssetStatusTransationsWithLastDate @AssetStatusId"
                        , new SqlParameter("@AssetStatusId", data.StatusId)).ToList();
                    // make join between Lst Of Assets After Search and Lst Of AssetSByIts Status
                    // For Example List Of Assets After Search And  List Of Working Assets
                    var res = from q in listQuery
                              join lst in lastTransationForAssets on q.Id equals lst.AssetDetailId
                              select q;
                    mainClass.Count = res.Count();
                    searchResult = res.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();



                }
                else if (query is null)
                {

                    //"EXEC SP_GetAssetsByStatus @AssetStatusId 
                    // procedure that select AssetDetails Which is Last Status of them is (AssetStatusId)
                    // select Asset Detail information that last transation is assetStatusId









                    var lstAssetsByStatus = _context.Set<AssetDetail>()
                    .FromSqlRaw("EXEC SP_GetAssetsByStatus @AssetStatusId"
                    , new SqlParameter("@AssetStatusId", data.StatusId)).ToList();

                    mainClass.Count = lstAssetsByStatus.Count;
                    lstAssetsByStatus = lstAssetsByStatus.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

                    foreach (var item in lstAssetsByStatus)
                    {

                        IndexAssetDetailVM.GetData getDataobj = new IndexAssetDetailVM.GetData();
                        getDataobj.Id = item.Id;
                        getDataobj.Code = item.Code;
                        getDataobj.Price = item.Price;
                        getDataobj.Barcode = item.Barcode;
                        getDataobj.Serial = item.SerialNumber;
                        getDataobj.SerialNumber = item.SerialNumber;
                        getDataobj.Model = _context.MasterAssets.Where(x => x.Id == item.MasterAssetId).FirstOrDefault().ModelNumber;
                        getDataobj.MasterAssetId = item.MasterAssetId;
                        getDataobj.PurchaseDate = item.PurchaseDate;
                        getDataobj.HospitalId = item.HospitalId;
                        getDataobj.DepartmentId = item.DepartmentId;
                        getDataobj.HospitalName = _context.Hospitals.Where(x => x.Id == item.HospitalId).FirstOrDefault().Name;
                        getDataobj.HospitalNameAr = item.Hospital.NameAr;
                        getDataobj.AssetName = item.MasterAsset.Name;
                        getDataobj.AssetNameAr = item.MasterAsset.NameAr;
                        if (item.HospitalId != 0)
                        {
                            //var hospitablObj = _context.Hospitals.Where(x => x.Id == item.HospitalId).AsNoTracking().FirstOrDefault();
                            //Console.WriteLine(hospitablObj.NameAr); For Test
                            getDataobj.GovernorateId = _context.Governorates.Where(x => x.Id == item.Hospital.GovernorateId).FirstOrDefault().Id;
                            getDataobj.GovernorateName = item.Hospital.Governorate.Name;
                            getDataobj.GovernorateName = item.Hospital.Governorate.NameAr;
                            //getDataobj.GovernorateNameAr = _context.Governorates.Where(x => x.Id == item.Hospital.GovernorateId).AsNoTracking().FirstOrDefault().NameAr;
                            getDataobj.CityId = _context.Cities.Where(x => x.Id == item.Hospital.CityId).FirstOrDefault().Id;
                            getDataobj.CityName = item.Hospital.City.Name;
                            getDataobj.CityNameAr = item.Hospital.City.NameAr;
                            getDataobj.OrganizationId = _context.Organizations.Where(x => x.Id == item.Hospital.OrganizationId).FirstOrDefault().Id;
                            getDataobj.OrgName = item.Hospital.Organization.Name;
                            getDataobj.OrgNameAr = item.Hospital.Organization.NameAr;
                            getDataobj.SubOrgName = _context.SubOrganizations.Where(x => x.Id == item.Hospital.SubOrganizationId).FirstOrDefault().Name;
                            getDataobj.SubOrgNameAr = item.Hospital.SubOrganization.NameAr;
                            getDataobj.SubOrganizationId = item.Hospital.SubOrganization.Id;
                        }
                        if (item.MasterAssetId != 0)
                        {
                            var masterObj = _context.MasterAssets.Where(x => x.Id == item.MasterAssetId).AsNoTracking().FirstOrDefault();
                            if (masterObj.BrandId != null || masterObj.BrandId != 0)
                            {
                                getDataobj.BrandId = _context.Brands.Where(x => x.Id == masterObj.BrandId).FirstOrDefault().Id;
                                getDataobj.BrandName = item.MasterAsset.brand.Name;
                                getDataobj.BrandNameAr = item.MasterAsset.brand.NameAr;
                            }
                        }
                        if (item.SupplierId != 0)
                        {
                            var supplierObj = _context.Suppliers.Where(x => x.Id == item.SupplierId).FirstOrDefault() ?? null;
                            if (supplierObj is not null)
                            {
                                getDataobj.SupplierId = supplierObj.Id;
                                getDataobj.SupplierName = supplierObj.Name;
                                getDataobj.SupplierNameAr = supplierObj.NameAr;

                            }
                            else
                            {
                                getDataobj.SupplierId = 0;
                                getDataobj.SupplierName = "";
                                getDataobj.SupplierNameAr = "";
                            }


                        }
                        if (item.DepartmentId != 0)
                        {
                            var deptObj = _context.Departments.Where(x => x.Id == item.DepartmentId).FirstOrDefault() ?? null;
                            if (deptObj is not null)
                            {
                                getDataobj.DepartmentName = deptObj.Name;
                                getDataobj.DepartmentNameAr = deptObj.NameAr;
                            }
                            else
                            {
                                getDataobj.DepartmentId = 0;
                                getDataobj.DepartmentName = "";
                            }

                        }


                        var lstTransactions = _context.AssetStatusTransactions.Where(a => a.AssetDetailId == item.Id).OrderByDescending(a => a.StatusDate.Value.Date).ToList();
                        if (lstTransactions.Count > 0)
                        {
                            getDataobj.AssetStatusId = lstTransactions[0].AssetStatusId;
                        }

                        list.Add(getDataobj);
                    }




                }



            }
            else
            {
                searchResult = searchResult.ToList();
            }






            string setstartday, setstartmonth, setendday, setendmonth = "";
            DateTime? startingFrom = new DateTime();
            DateTime? endingTo = new DateTime();
            if (data.Start == "")
            {
                data.purchaseDateFrom = DateTime.Parse("01/01/1900");
                startingFrom = DateTime.Parse("01/01/1900");
            }
            else
            {
                data.purchaseDateFrom = DateTime.Parse(data.Start.ToString());
                var startyear = data.purchaseDateFrom.Value.Year;
                var startmonth = data.purchaseDateFrom.Value.Month;
                var startday = data.purchaseDateFrom.Value.Day;
                if (startday < 10)
                    setstartday = data.purchaseDateFrom.Value.Day.ToString().PadLeft(2, '0');
                else
                    setstartday = data.purchaseDateFrom.Value.Day.ToString();

                if (startmonth < 10)
                    setstartmonth = data.purchaseDateFrom.Value.Month.ToString().PadLeft(2, '0');
                else
                    setstartmonth = data.purchaseDateFrom.Value.Month.ToString();

                var sDate = startyear + "-" + setstartmonth + "-" + setstartday;
                startingFrom = DateTime.Parse(sDate);//.AddDays(1);
            }

            if (data.End == "")
            {
                data.purchaseDateTo = DateTime.Today.Date;
                endingTo = DateTime.Today.Date;
            }
            else
            {
                data.purchaseDateTo = DateTime.Parse(data.End.ToString());
                var endyear = data.purchaseDateTo.Value.Year;
                var endmonth = data.purchaseDateTo.Value.Month;
                var endday = data.purchaseDateTo.Value.Day;
                if (endday < 10)
                    setendday = data.purchaseDateTo.Value.Day.ToString().PadLeft(2, '0');
                else
                    setendday = data.purchaseDateTo.Value.Day.ToString();
                if (endmonth < 10)
                    setendmonth = data.purchaseDateTo.Value.Month.ToString().PadLeft(2, '0');
                else
                    setendmonth = data.purchaseDateTo.Value.Month.ToString();
                var eDate = endyear + "-" + setendmonth + "-" + setendday;
                endingTo = DateTime.Parse(eDate);
            }
            if (startingFrom != null || endingTo != null)
            {
                if (query is not null)
                {
                    query = query.Where(a => a.PurchaseDate.Value.Date >= startingFrom.Value.Date && a.PurchaseDate.Value.Date <= endingTo.Value.Date);
                }
                else if (query is null)
                {
                    query = GetLstAssetDetails().Where(a => a.PurchaseDate.Value.Date >= startingFrom.Value.Date && a.PurchaseDate.Value.Date <= endingTo.Value.Date);
                }
                mainClass.Count = query.Count();
                searchResult = query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            }




            if (searchResult.Count > 0)
            {
                foreach (var item in searchResult)
                {
                    IndexAssetDetailVM.GetData getDataobj = new IndexAssetDetailVM.GetData();
                    getDataobj.Id = item.Id;
                    getDataobj.Code = item.Code;
                    getDataobj.Price = item.Price;
                    getDataobj.Barcode = item.Barcode;
                    getDataobj.Serial = item.SerialNumber;
                    getDataobj.SerialNumber = item.SerialNumber;
                    getDataobj.BarCode = item.Barcode;
                    getDataobj.Model = item.MasterAsset.ModelNumber;
                    getDataobj.MasterAssetId = item.MasterAssetId;
                    getDataobj.PurchaseDate = item.PurchaseDate;
                    getDataobj.HospitalId = item.HospitalId;
                    getDataobj.DepartmentId = item.DepartmentId;
                    getDataobj.HospitalName = item.Hospital.Name;
                    getDataobj.HospitalNameAr = item.Hospital.NameAr;
                    getDataobj.AssetName = item.MasterAsset.Name;
                    getDataobj.AssetNameAr = item.MasterAsset.NameAr;
                    getDataobj.GovernorateId = item.Hospital.Governorate.Id;
                    getDataobj.GovernorateName = item.Hospital.Governorate.Name;
                    getDataobj.GovernorateNameAr = item.Hospital.Governorate.NameAr;
                    getDataobj.CityId = item.Hospital.City.Id;
                    getDataobj.CityName = item.Hospital.City.Name;
                    getDataobj.CityNameAr = item.Hospital.City.NameAr;
                    getDataobj.OrganizationId = item.Hospital.Organization.Id;
                    getDataobj.OrgName = item.Hospital.Organization.Name;
                    getDataobj.OrgNameAr = item.Hospital.Organization.NameAr;
                    getDataobj.SubOrgName = item.Hospital.SubOrganization.Name;
                    getDataobj.SubOrgNameAr = item.Hospital.SubOrganization.NameAr;
                    getDataobj.SubOrganizationId = item.Hospital.SubOrganization.Id;
                    getDataobj.BrandId = item.MasterAsset.brand != null ? item.MasterAsset.brand.Id : 0;
                    getDataobj.BrandName = item.MasterAsset.brand != null ? item.MasterAsset.brand.Name : "";
                    getDataobj.BrandNameAr = item.MasterAsset.brand != null ? item.MasterAsset.brand.NameAr : "";
                    getDataobj.SupplierId = item.Supplier != null ? item.Supplier.Id : 0;
                    getDataobj.SupplierName = item.Supplier != null ? item.Supplier.Name : "";
                    getDataobj.SupplierNameAr = item.Supplier != null ? item.Supplier.NameAr : "";
                    getDataobj.DepartmentName = item.Department != null ? item.Department.Name : "";
                    getDataobj.DepartmentNameAr = item.Department != null ? item.Department.NameAr : "";

                    var lstTransactions = _context.AssetStatusTransactions.Where(a => a.AssetDetailId == item.Id).OrderByDescending(a => a.StatusDate.Value.Date).ToList();
                    if (lstTransactions.Count > 0)
                    {
                        getDataobj.AssetStatusId = lstTransactions[0].AssetStatusId;
                    }

                    list.Add(getDataobj);
                }

            }



            mainClass.Results = list;

            return mainClass;


        }

        #endregion


        /// <summary>
        /// Get Asset Name By MasterAssetId And HospitalId
        /// </summary>
        /// <param name="masterAssetId"></param>
        /// <param name="hospitalId"></param>
        /// <returns></returns>
        public IEnumerable<AssetDetail> GetAssetNameByMasterAssetIdAndHospitalId(int masterAssetId, int hospitalId)
        {
            return _context.AssetDetails.Include(a => a.MasterAsset).Where(a => a.HospitalId == hospitalId && a.MasterAssetId == masterAssetId).ToList();
        }



        /// <summary>
        /// Search Assets In Mobile Application
        /// </summary>
        /// <param name="searchObj"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IndexAssetDetailVM MobSearchAssetInHospital(SearchMasterAssetVM searchObj, int pageNumber, int pageSize)
        {
            Employee empObj = new Employee();
            List<string> userRoleNames = new List<string>();
            ApplicationUser userObj = new ApplicationUser();
            IndexAssetDetailVM mainClass = new IndexAssetDetailVM();
            List<IndexAssetDetailVM.GetData> list = new List<IndexAssetDetailVM.GetData>();
            if (searchObj.UserId != null)
            {
                var lstUsers = _context.ApplicationUser.Where(a => a.Id == searchObj.UserId).ToList();
                if (lstUsers.Count > 0)
                {
                    userObj = lstUsers[0];
                    var roles = (from userRole in _context.UserRoles
                                 join role in _context.ApplicationRole on userRole.RoleId equals role.Id
                                 where userRole.UserId == searchObj.UserId
                                 select role);
                    foreach (var role in roles)
                    {
                        userRoleNames.Add(role.Name);
                    }


                    var lstEmployees = _context.Employees.Where(a => a.Email == userObj.Email).ToList();
                    if (lstEmployees.Count > 0)
                    {
                        empObj = lstEmployees[0];
                    }
                }
                if (userRoleNames.Contains("AssetOwner"))
                {
                    list = _context.AssetOwners.Include(a => a.AssetDetail)
                                         .Include(a => a.AssetDetail.MasterAsset).Include(a => a.AssetDetail.Hospital).Include(a => a.AssetDetail.Supplier)
                                         .Include(a => a.AssetDetail.MasterAsset.brand).Include(a => a.AssetDetail.Hospital.Governorate).Include(a => a.AssetDetail.Hospital.City)
                                         .Include(a => a.AssetDetail.Hospital.Organization).Include(a => a.AssetDetail.Hospital.SubOrganization).Include(a => a.AssetDetail.Department)
                                         .Where(a => a.HospitalId == userObj.HospitalId && a.EmployeeId == empObj.Id)
                                         .OrderBy(a => a.AssetDetail.Barcode)
                                           .Select(item => new IndexAssetDetailVM.GetData
                                           {
                                               Id = item.Id,
                                               Code = item.AssetDetail.Code,
                                               SerialNumber = item.AssetDetail.SerialNumber,
                                               CreatedBy = item.AssetDetail.CreatedBy,
                                               HospitalId = item.AssetDetail.HospitalId,
                                               AssetId = item.AssetDetail.Id,
                                               SupplierId = item.AssetDetail.SupplierId,
                                               DepartmentId = item.AssetDetail.DepartmentId,
                                               Serial = item.AssetDetail.SerialNumber,
                                               Price = item.AssetDetail.Price,
                                               BarCode = item.AssetDetail.Barcode,

                                               PurchaseDate = item.AssetDetail.PurchaseDate,
                                               QrFilePath = item.AssetDetail.QrFilePath,
                                               MasterAssetId = item.AssetDetail.MasterAssetId,
                                               PeriorityId = item.AssetDetail.MasterAsset.PeriorityId != null ? item.AssetDetail.MasterAsset.PeriorityId : 0,
                                               AssetName = item.AssetDetail.MasterAsset != null ? item.AssetDetail.MasterAsset.Name : "",
                                               AssetNameAr = item.AssetDetail.MasterAsset != null ? item.AssetDetail.MasterAsset.NameAr : "",
                                               OriginId = item.AssetDetail.MasterAsset != null ? item.AssetDetail.MasterAsset.OriginId : 0,
                                               BrandId = item.AssetDetail.MasterAsset != null ? item.AssetDetail.MasterAsset.BrandId : 0,
                                               MasterImg = item.AssetDetail.MasterAsset != null ? item.AssetDetail.MasterAsset.AssetImg : "",
                                               BrandName = item.AssetDetail.MasterAsset.brand != null ? item.AssetDetail.MasterAsset.brand.Name : "",
                                               BrandNameAr = item.AssetDetail.MasterAsset.brand != null ? item.AssetDetail.MasterAsset.brand.NameAr : "",
                                               Model = item.AssetDetail.MasterAsset != null ? item.AssetDetail.MasterAsset.ModelNumber : "",
                                               DepartmentName = item.AssetDetail.Department != null ? item.AssetDetail.Department.Name : "",
                                               DepartmentNameAr = item.AssetDetail.Department != null ? item.AssetDetail.Department.NameAr : "",
                                               AssetPeriorityName = item.AssetDetail.MasterAsset.AssetPeriority != null ? item.AssetDetail.MasterAsset.AssetPeriority.Name : "",
                                               AssetPeriorityNameAr = item.AssetDetail.MasterAsset.AssetPeriority != null ? item.AssetDetail.MasterAsset.AssetPeriority.NameAr : "",
                                               HospitalName = item.AssetDetail.Hospital != null ? item.AssetDetail.Hospital.Name : "",
                                               HospitalNameAr = item.AssetDetail.Hospital != null ? item.AssetDetail.Hospital.NameAr : "",


                                               GovernorateId = item.AssetDetail.Hospital.Governorate != null ? item.AssetDetail.Hospital.GovernorateId : 0,
                                               CityId = item.AssetDetail.Hospital != null ? item.AssetDetail.Hospital.CityId : 0,
                                               OrganizationId = item.AssetDetail.Hospital != null ? item.AssetDetail.Hospital.OrganizationId : 0,
                                               SubOrganizationId = item.AssetDetail.Hospital != null ? item.AssetDetail.Hospital.SubOrganizationId : 0,
                                               GovernorateName = item.AssetDetail.Hospital.Governorate.Name,
                                               GovernorateNameAr = item.AssetDetail.Hospital.Governorate.NameAr,
                                               CityName = item.AssetDetail.Hospital.City != null ? item.AssetDetail.Hospital.City.Name : "",
                                               CityNameAr = item.AssetDetail.Hospital.City != null ? item.AssetDetail.Hospital.City.NameAr : "",
                                               OrgName = item.AssetDetail.Hospital.Organization != null ? item.AssetDetail.Hospital.Organization.Name : "",
                                               OrgNameAr = item.AssetDetail.Hospital.Organization != null ? item.AssetDetail.Hospital.Organization.NameAr : "",
                                               SubOrgName = item.AssetDetail.Hospital.SubOrganization != null ? item.AssetDetail.Hospital.SubOrganization.Name : "",
                                               SubOrgNameAr = item.AssetDetail.Hospital.SubOrganization != null ? item.AssetDetail.Hospital.SubOrganization.NameAr : "",





                                               SupplierName = item.AssetDetail.Supplier != null ? item.AssetDetail.Supplier.Name : "",
                                               SupplierNameAr = item.AssetDetail.Supplier != null ? item.AssetDetail.Supplier.NameAr : ""
                                           }).ToList();
                }
                if (userRoleNames.Contains("SRCreator"))
                {
                    list = _context.AssetOwners.Include(a => a.AssetDetail)
                                         .Include(a => a.AssetDetail.MasterAsset).Include(a => a.AssetDetail.Hospital).Include(a => a.AssetDetail.Supplier)
                                         .Include(a => a.AssetDetail.MasterAsset.brand).Include(a => a.AssetDetail.Hospital.Governorate).Include(a => a.AssetDetail.Hospital.City)
                                         .Include(a => a.AssetDetail.Hospital.Organization).Include(a => a.AssetDetail.Hospital.SubOrganization).Include(a => a.AssetDetail.Department)
                                         .Where(a => a.HospitalId == userObj.HospitalId && a.EmployeeId == empObj.Id)
                                         .OrderBy(a => a.AssetDetail.Barcode)
                                           .Select(item => new IndexAssetDetailVM.GetData
                                           {
                                               Id = item.Id,
                                               Code = item.AssetDetail.Code,
                                               SerialNumber = item.AssetDetail.SerialNumber,
                                               CreatedBy = item.AssetDetail.CreatedBy,
                                               HospitalId = item.AssetDetail.HospitalId,
                                               SupplierId = item.AssetDetail.SupplierId,
                                               DepartmentId = item.AssetDetail.DepartmentId,
                                               Serial = item.AssetDetail.SerialNumber,
                                               Price = item.AssetDetail.Price,
                                               AssetId = item.AssetDetail.Id,
                                               BarCode = item.AssetDetail.Barcode,
                                               PurchaseDate = item.AssetDetail.PurchaseDate,
                                               QrFilePath = item.AssetDetail.QrFilePath,
                                               MasterAssetId = item.AssetDetail.MasterAssetId,
                                               PeriorityId = item.AssetDetail.MasterAsset.PeriorityId != null ? item.AssetDetail.MasterAsset.PeriorityId : 0,
                                               AssetName = item.AssetDetail.MasterAsset != null ? item.AssetDetail.MasterAsset.Name : "",
                                               AssetNameAr = item.AssetDetail.MasterAsset != null ? item.AssetDetail.MasterAsset.NameAr : "",
                                               OriginId = item.AssetDetail.MasterAsset != null ? item.AssetDetail.MasterAsset.OriginId : 0,
                                               BrandId = item.AssetDetail.MasterAsset != null ? item.AssetDetail.MasterAsset.BrandId : 0,
                                               MasterImg = item.AssetDetail.MasterAsset != null ? item.AssetDetail.MasterAsset.AssetImg : "",
                                               BrandName = item.AssetDetail.MasterAsset.brand != null ? item.AssetDetail.MasterAsset.brand.Name : "",
                                               BrandNameAr = item.AssetDetail.MasterAsset.brand != null ? item.AssetDetail.MasterAsset.brand.NameAr : "",
                                               Model = item.AssetDetail.MasterAsset != null ? item.AssetDetail.MasterAsset.ModelNumber : "",
                                               DepartmentName = item.AssetDetail.Department != null ? item.AssetDetail.Department.Name : "",
                                               DepartmentNameAr = item.AssetDetail.Department != null ? item.AssetDetail.Department.NameAr : "",
                                               AssetPeriorityName = item.AssetDetail.MasterAsset.AssetPeriority != null ? item.AssetDetail.MasterAsset.AssetPeriority.Name : "",
                                               AssetPeriorityNameAr = item.AssetDetail.MasterAsset.AssetPeriority != null ? item.AssetDetail.MasterAsset.AssetPeriority.NameAr : "",
                                               HospitalName = item.AssetDetail.Hospital != null ? item.AssetDetail.Hospital.Name : "",
                                               HospitalNameAr = item.AssetDetail.Hospital != null ? item.AssetDetail.Hospital.NameAr : "",

                                               GovernorateId = item.AssetDetail.Hospital.Governorate != null ? item.AssetDetail.Hospital.GovernorateId : 0,
                                               CityId = item.AssetDetail.Hospital != null ? item.AssetDetail.Hospital.CityId : 0,
                                               OrganizationId = item.AssetDetail.Hospital != null ? item.AssetDetail.Hospital.OrganizationId : 0,
                                               SubOrganizationId = item.AssetDetail.Hospital != null ? item.AssetDetail.Hospital.SubOrganizationId : 0,
                                               GovernorateName = item.AssetDetail.Hospital.Governorate.Name,
                                               GovernorateNameAr = item.AssetDetail.Hospital.Governorate.NameAr,
                                               CityName = item.AssetDetail.Hospital.City != null ? item.AssetDetail.Hospital.City.Name : "",
                                               CityNameAr = item.AssetDetail.Hospital.City != null ? item.AssetDetail.Hospital.City.NameAr : "",
                                               OrgName = item.AssetDetail.Hospital.Organization != null ? item.AssetDetail.Hospital.Organization.Name : "",
                                               OrgNameAr = item.AssetDetail.Hospital.Organization != null ? item.AssetDetail.Hospital.Organization.NameAr : "",
                                               SubOrgName = item.AssetDetail.Hospital.SubOrganization != null ? item.AssetDetail.Hospital.SubOrganization.Name : "",
                                               SubOrgNameAr = item.AssetDetail.Hospital.SubOrganization != null ? item.AssetDetail.Hospital.SubOrganization.NameAr : "",



                                               SupplierName = item.AssetDetail.Supplier != null ? item.AssetDetail.Supplier.Name : "",
                                               SupplierNameAr = item.AssetDetail.Supplier != null ? item.AssetDetail.Supplier.NameAr : ""
                                           }).ToList();
                }
                else if (!userRoleNames.Contains("SRCreator") && !userRoleNames.Contains("AssetOwner"))
                {

                    list = _context.AssetDetails.Include(a => a.MasterAsset)
                     .Include(a => a.MasterAsset.brand).Include(a => a.MasterAsset.AssetPeriority)
                     .Include(a => a.Hospital)
                     .Include(a => a.Hospital.Governorate)
                     .Include(a => a.Hospital.City)
                     .Include(a => a.Hospital.Organization)
                     .Include(a => a.Hospital.SubOrganization)
                     .OrderBy(a => a.Barcode)
                     .Select(item => new IndexAssetDetailVM.GetData
                     {
                         Id = item.Id,
                         Code = item.Code,
                         SerialNumber = item.SerialNumber,
                         CreatedBy = item.CreatedBy,
                         HospitalId = item.HospitalId,
                         SupplierId = item.SupplierId,
                         DepartmentId = item.DepartmentId,
                         Serial = item.SerialNumber,
                         AssetId = item.Id,
                         Price = item.Price,
                         BarCode = item.Barcode,
                         QrFilePath = item.QrFilePath,
                         AssetImg = item.MasterAsset.AssetImg,
                         MasterAssetId = item.MasterAssetId,
                         PeriorityId = item.MasterAsset.PeriorityId != null ? item.MasterAsset.PeriorityId : 0,
                         PurchaseDate = item.PurchaseDate,

                         AssetName = item.MasterAsset != null ? item.MasterAsset.Name : "",
                         AssetNameAr = item.MasterAsset != null ? item.MasterAsset.NameAr : "",
                         OriginId = item.MasterAsset != null ? item.MasterAsset.OriginId : 0,
                         BrandId = item.MasterAsset != null ? item.MasterAsset.BrandId : 0,
                         MasterImg = item.MasterAsset != null ? item.MasterAsset.AssetImg : "",
                         BrandName = item.MasterAsset.brand != null ? item.MasterAsset.brand.Name : "",
                         BrandNameAr = item.MasterAsset.brand != null ? item.MasterAsset.brand.NameAr : "",
                         Model = item.MasterAsset != null ? item.MasterAsset.ModelNumber : "",
                         DepartmentName = item.Department != null ? item.Department.Name : "",
                         DepartmentNameAr = item.Department != null ? item.Department.NameAr : "",
                         AssetPeriorityName = item.MasterAsset.AssetPeriority != null ? item.MasterAsset.AssetPeriority.Name : "",
                         AssetPeriorityNameAr = item.MasterAsset.AssetPeriority != null ? item.MasterAsset.AssetPeriority.NameAr : "",
                         HospitalName = item.Hospital != null ? item.Hospital.Name : "",
                         HospitalNameAr = item.Hospital != null ? item.Hospital.NameAr : "",
                         GovernorateId = item.Hospital.Governorate != null ? item.Hospital.GovernorateId : 0,
                         CityId = item.Hospital.City != null ? item.Hospital.CityId : 0,
                         OrganizationId = item.Hospital != null ? item.Hospital.OrganizationId : 0,
                         SubOrganizationId = item.Hospital != null ? item.Hospital.SubOrganizationId : 0,
                         GovernorateName = item.Hospital.Governorate.Name,
                         GovernorateNameAr = item.Hospital.Governorate.NameAr,
                         CityName = item.Hospital.City != null ? item.Hospital.City.Name : "",
                         CityNameAr = item.Hospital.City != null ? item.Hospital.City.NameAr : "",
                         OrgName = item.Hospital.Organization != null ? item.Hospital.Organization.Name : "",
                         OrgNameAr = item.Hospital.Organization != null ? item.Hospital.Organization.NameAr : "",
                         SubOrgName = item.Hospital.SubOrganization != null ? item.Hospital.SubOrganization.Name : "",
                         SubOrgNameAr = item.Hospital.SubOrganization != null ? item.Hospital.SubOrganization.NameAr : "",
                         SupplierName = item.Supplier != null ? item.Supplier.Name : "",
                         SupplierNameAr = item.Supplier != null ? item.Supplier.NameAr : ""
                     }).ToList();
                }


                if (searchObj.GovernorateId != 0)
                {
                    list = list.Where(a => a.GovernorateId == searchObj.GovernorateId).ToList();
                }
                else
                    list = list.ToList();
                if (searchObj.CityId != 0)
                {
                    list = list.Where(a => a.CityId == searchObj.CityId).ToList();
                }
                else
                    list = list.ToList();



                if (searchObj.HospitalId > 0)
                {
                    list = list.Where(b => b.HospitalId == searchObj.HospitalId).ToList();
                }
                else
                {
                    list = list.ToList();
                }


                if (searchObj.SupplierId != 0)
                {
                    list = list.Where(a => a.SupplierId == searchObj.SupplierId).ToList();
                }
                else
                    list = list.ToList();

                if (searchObj.DepartmentId != 0)
                {
                    list = list.Where(a => a.DepartmentId == searchObj.DepartmentId).ToList();
                }
                else
                    list = list.ToList();


                if (searchObj.OriginId != 0)
                {
                    list = list.Where(a => a.OriginId == searchObj.OriginId).ToList();
                }
                else
                    list = list.ToList();



                if (searchObj.BrandId != 0)
                {
                    list = list.Where(a => a.BrandId == searchObj.BrandId).ToList();
                }
                else
                    list = list.ToList();

                if (searchObj.DepartmentId != 0)
                {
                    list = list.Where(a => a.DepartmentId == searchObj.DepartmentId).ToList();
                }
                else
                    list = list.ToList();

                if (searchObj.PeriorityId != 0)
                {
                    list = list.Where(a => a.PeriorityId == searchObj.PeriorityId).ToList();
                }
                else
                    list = list.ToList();



                if (searchObj.StrSearch != "")
                {
                    list = list.Where(b => b.AssetNameAr != null).ToList();
                    list = list.Where(b => b.AssetName != null).ToList();
                    list = list.Where(b =>
                     b.AssetName.ToLower().Contains(searchObj.StrSearch.ToLower())
                    || b.AssetNameAr.ToLower().Contains(searchObj.StrSearch.ToLower())
                    || b.SerialNumber.ToLower().Contains(searchObj.StrSearch.ToLower())
                    || b.Serial.ToLower().Contains(searchObj.StrSearch.ToLower())
                    || b.Code.ToLower().Contains(searchObj.StrSearch.ToLower())
                    || b.BarCode.ToLower().Contains(searchObj.StrSearch.ToLower())
                    || b.Model.ToLower().Contains(searchObj.StrSearch.ToLower())
                    ).ToList();
                }

                if (searchObj.BarCode != "")
                {
                    list = list.Where(b => b.BarCode.ToLower().Contains(searchObj.BarCode.ToLower())).ToList();
                }
                if (searchObj.Serial != "")
                {
                    list = list.Where(b => b.Serial.ToLower().Contains(searchObj.Serial.ToLower())).ToList();
                }
                if (searchObj.Model != "")
                {
                    list = list.Where(b => b.Model.ToLower().Contains(searchObj.Model.ToLower())).ToList();
                }

                if (searchObj.AssetName != "")
                {
                    list = list.Where(b => b.AssetName.ToLower().Contains(searchObj.AssetName.ToLower())).ToList();
                }
                if (searchObj.AssetNameAr != "")
                {
                    list = list.Where(b => b.AssetNameAr.ToLower().Contains(searchObj.AssetNameAr.ToLower())).ToList();
                }






                if (searchObj.FromPrice != 0 && searchObj.ToPrice != 0)
                {
                    list = list.Where(b => b.Price >= searchObj.FromPrice && b.Price <= searchObj.ToPrice).ToList();
                }
                else if (searchObj.FromPrice != 0 && searchObj.ToPrice == 0)
                {
                    var maxPrice = Convert.ToDecimal(list.Max(a => a.Price).Value);
                    list = list.Where(b => b.Price >= searchObj.FromPrice && b.Price <= maxPrice).ToList();
                }
                else if (searchObj.FromPrice == 0 && searchObj.ToPrice != 0)
                {
                    list = list.Where(a => a.Price != null).ToList();
                    if (list.Count > 0)
                    {
                        decimal minPrice = Convert.ToDecimal(list.Min(a => a.Price).Value);
                        list = list.Where(b => b.Price >= minPrice && b.Price <= searchObj.ToPrice).ToList();
                    }
                }


                string setstartday, setstartmonth, setendday, setendmonth = "";
                DateTime? startingFrom = new DateTime();
                DateTime? endingTo = new DateTime();
                if (searchObj.Start == "")
                {
                }
                else
                {
                    searchObj.FromPurchaseDate = DateTime.Parse(searchObj.Start.ToString());
                    var startyear = searchObj.FromPurchaseDate.Value.Year;
                    var startmonth = searchObj.FromPurchaseDate.Value.Month;
                    var startday = searchObj.FromPurchaseDate.Value.Day;
                    if (startday < 10)
                        setstartday = searchObj.FromPurchaseDate.Value.Day.ToString().PadLeft(2, '0');
                    else
                        setstartday = searchObj.FromPurchaseDate.Value.Day.ToString();

                    if (startmonth < 10)
                        setstartmonth = searchObj.FromPurchaseDate.Value.Month.ToString().PadLeft(2, '0');
                    else
                        setstartmonth = searchObj.FromPurchaseDate.Value.Month.ToString();

                    var sDate = startyear + "-" + setstartmonth + "-" + setstartday;
                    startingFrom = DateTime.Parse(sDate);
                }

                if (searchObj.End == "")
                {
                    endingTo = DateTime.Today.Date;
                }
                else
                {
                    searchObj.ToPurchaseDate = DateTime.Parse(searchObj.End.ToString());
                    var endyear = searchObj.ToPurchaseDate.Value.Year;
                    var endmonth = searchObj.ToPurchaseDate.Value.Month;
                    var endday = searchObj.ToPurchaseDate.Value.Day;
                    if (endday < 10)
                        setendday = searchObj.ToPurchaseDate.Value.Day.ToString().PadLeft(2, '0');
                    else
                        setendday = searchObj.ToPurchaseDate.Value.Day.ToString();
                    if (endmonth < 10)
                        setendmonth = searchObj.ToPurchaseDate.Value.Month.ToString().PadLeft(2, '0');
                    else
                        setendmonth = searchObj.ToPurchaseDate.Value.Month.ToString();
                    var eDate = endyear + "-" + setendmonth + "-" + setendday;
                    endingTo = DateTime.Parse(eDate);
                }



                if (searchObj.Start != "" && searchObj.End != "")
                {
                    list = list.Where(a => a.PurchaseDate != null).ToList();
                    list = list.Where(a => a.PurchaseDate.Value.Date >= startingFrom.Value.Date && a.PurchaseDate.Value.Date <= endingTo.Value.Date).ToList();
                }


                if (searchObj.SortField == "BarCode")
                {
                    if (searchObj.StrSearch != "")
                    {
                        if (searchObj.SortStatus == "descending")
                        {
                            list = list.Where(b => b.AssetNameAr != null).ToList();
                            list = list.Where(b => b.AssetName != null).ToList();
                            list = list.Where(b =>
                             b.AssetName.ToLower().Contains(searchObj.StrSearch.ToLower())
                            || b.AssetNameAr.ToLower().Contains(searchObj.StrSearch.ToLower())
                            || b.SerialNumber.ToLower().Contains(searchObj.StrSearch.ToLower())
                            || b.Serial.ToLower().Contains(searchObj.StrSearch.ToLower())
                            || b.Code.ToLower().Contains(searchObj.StrSearch.ToLower())
                            || b.BarCode.ToLower().Contains(searchObj.StrSearch.ToLower())
                            || b.Model.ToLower().Contains(searchObj.StrSearch.ToLower())
                            ).OrderByDescending(d => d.BarCode).ToList();
                            //list = list.Where(b => b.BarCode.Contains(searchObj.StrSearch)).OrderByDescending(d => d.BarCode).ToList();
                        }
                        else
                        {

                            list = list.Where(b => b.AssetNameAr != null).ToList();
                            list = list.Where(b => b.AssetName != null).ToList();
                            list = list.Where(b =>
                       b.AssetName.ToLower().Contains(searchObj.StrSearch.ToLower())
                            || b.AssetNameAr.ToLower().Contains(searchObj.StrSearch.ToLower())
                            || b.SerialNumber.ToLower().Contains(searchObj.StrSearch.ToLower())
                            || b.Serial.ToLower().Contains(searchObj.StrSearch.ToLower())
                            || b.Code.ToLower().Contains(searchObj.StrSearch.ToLower())
                            || b.BarCode.ToLower().Contains(searchObj.StrSearch.ToLower())
                            || b.Model.ToLower().Contains(searchObj.StrSearch.ToLower())
                            ).OrderBy(d => d.BarCode).ToList();
                            // list = list.Where(b => b.BarCode.Contains(searchObj.StrSearch)).OrderBy(d => d.BarCode).ToList();
                        }
                    }
                    else
                    {
                        if (searchObj.SortStatus == "descending")
                        {
                            list = list.OrderByDescending(d => d.BarCode).ToList();
                        }
                        else
                        {
                            list = list.OrderBy(d => d.BarCode).ToList();
                        }
                    }
                }
                if (searchObj.SortField == "AssetName")
                {
                    if (searchObj.StrSearch != "")
                    {
                        if (searchObj.SortStatus == "descending")
                        {
                            list = list.Where(b => b.AssetNameAr != null).ToList();
                            list = list.Where(b => b.AssetName != null).ToList();
                            list = list.Where(b =>
                          b.AssetName.ToLower().Contains(searchObj.StrSearch.ToLower())
                            || b.AssetNameAr.ToLower().Contains(searchObj.StrSearch.ToLower())
                            || b.SerialNumber.ToLower().Contains(searchObj.StrSearch.ToLower())
                            || b.Serial.ToLower().Contains(searchObj.StrSearch.ToLower())
                            || b.Code.ToLower().Contains(searchObj.StrSearch.ToLower())
                            || b.BarCode.ToLower().Contains(searchObj.StrSearch.ToLower())
                            || b.Model.ToLower().Contains(searchObj.StrSearch.ToLower())
                            ).OrderByDescending(d => d.AssetName).ToList();
                            //   list = list.Where(b => b.AssetName.Contains(searchObj.AssetName)).OrderByDescending(d => d.AssetName).ToList();
                        }
                        else
                        {
                            list = list.Where(b => b.AssetNameAr != null).ToList();
                            list = list.Where(b => b.AssetName != null).ToList();
                            list = list.Where(b =>
                           b.AssetName.ToLower().Contains(searchObj.StrSearch.ToLower())
                            || b.AssetNameAr.ToLower().Contains(searchObj.StrSearch.ToLower())
                            || b.SerialNumber.ToLower().Contains(searchObj.StrSearch.ToLower())
                            || b.Serial.ToLower().Contains(searchObj.StrSearch.ToLower())
                            || b.Code.ToLower().Contains(searchObj.StrSearch.ToLower())
                            || b.BarCode.ToLower().Contains(searchObj.StrSearch.ToLower())
                            || b.Model.ToLower().Contains(searchObj.StrSearch.ToLower())
                            ).OrderBy(d => d.AssetName).ToList();

                            //    list = list.Where(b => b.AssetName.Contains(searchObj.AssetName)).OrderBy(d => d.AssetName).ToList();
                        }
                    }
                    else
                    {
                        if (searchObj.SortStatus == "descending")
                        {
                            list = list.OrderByDescending(d => d.AssetName).ToList();
                        }
                        else
                        {
                            list = list.OrderBy(d => d.AssetName).ToList();
                        }
                    }
                }
                if (searchObj.SortField == "AssetNameAr")
                {
                    if (searchObj.StrSearch != "")
                    {
                        if (searchObj.SortStatus == "descending")
                        {

                            list = list.Where(b => b.AssetNameAr != null).ToList();
                            list = list.Where(b => b.AssetName != null).ToList();
                            list = list.Where(b =>
                             b.AssetName.ToLower().Contains(searchObj.StrSearch.ToLower())
                            || b.AssetNameAr.ToLower().Contains(searchObj.StrSearch.ToLower())
                            || b.SerialNumber.ToLower().Contains(searchObj.StrSearch.ToLower())
                            || b.Serial.ToLower().Contains(searchObj.StrSearch.ToLower())
                            || b.Code.ToLower().Contains(searchObj.StrSearch.ToLower())
                            || b.BarCode.ToLower().Contains(searchObj.StrSearch.ToLower())
                            || b.Model.ToLower().Contains(searchObj.StrSearch.ToLower())
                            ).OrderByDescending(d => d.AssetNameAr).ToList();
                        }
                        else
                        {
                            list = list.Where(b => b.AssetNameAr != null).ToList();
                            list = list.Where(b => b.AssetName != null).ToList();
                            list = list.Where(b =>
                              b.AssetName.ToLower().Contains(searchObj.StrSearch.ToLower())
                            || b.AssetNameAr.ToLower().Contains(searchObj.StrSearch.ToLower())
                            || b.SerialNumber.ToLower().Contains(searchObj.StrSearch.ToLower())
                            || b.Serial.ToLower().Contains(searchObj.StrSearch.ToLower())
                            || b.Code.ToLower().Contains(searchObj.StrSearch.ToLower())
                            || b.BarCode.ToLower().Contains(searchObj.StrSearch.ToLower())
                            || b.Model.ToLower().Contains(searchObj.StrSearch.ToLower())
                            ).OrderBy(d => d.AssetNameAr).ToList();
                        }
                    }
                    else
                    {
                        if (searchObj.SortStatus == "descending")
                        {
                            list = list.OrderByDescending(d => d.AssetNameAr).ToList();
                        }
                        else
                        {
                            list = list.OrderBy(d => d.AssetNameAr).ToList();
                        }
                    }
                }


                var itemsPerPage = list.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                mainClass.Results = itemsPerPage;
                mainClass.Count = list.Count;
                return mainClass;

            }
            return mainClass;
        }


        /// <summary>
        /// Get All Requests by Asset Detail Id
        /// </summary>
        /// <param name="assetId"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IndexRequestVM GetRequestsForAssetId(int assetId, int pageNumber, int pageSize)
        {

            IndexRequestVM mainClass = new IndexRequestVM();
            List<IndexRequestVM.GetData> list = new List<IndexRequestVM.GetData>();
            var lstRquests = _context.Request.Where(a => a.AssetDetailId == assetId).AsQueryable();
            mainClass.Count = lstRquests.Count();

            if (lstRquests.Count() > 0)
            {
                var lstRquestsForAssetObj = lstRquests.ToList();
                foreach (var item in lstRquestsForAssetObj)
                {

                    IndexRequestVM.GetData getDataObj = new IndexRequestVM.GetData();
                    getDataObj.Id = item.Id;
                    getDataObj.HospitalId = int.Parse(item.HospitalId.ToString());
                    getDataObj.Subject = item.Subject;
                    getDataObj.RequestCode = item.RequestCode;
                    getDataObj.RequestDate = item.RequestDate;
                    getDataObj.PeriorityId = item.RequestPeriorityId != null ? (int)item.RequestPeriorityId : 0;
                    getDataObj.PeriorityName = item.RequestPeriority.Name;
                    getDataObj.PeriorityNameAr = item.RequestPeriority.NameAr;
                    getDataObj.ModeId = item.RequestModeId != null ? (int)item.RequestModeId : 0;
                    getDataObj.ModeName = item.RequestMode.Name;
                    getDataObj.ModeNameAr = item.RequestMode.NameAr;
                    getDataObj.CreatedById = item.CreatedById;
                    getDataObj.CreatedBy = item.User.UserName;
                    var lstStatus = _context.RequestTracking.Include(t => t.Request).Include(t => t.RequestStatus)
                                   .Where(a => a.RequestId == item.Id).ToList().OrderByDescending(a => DateTime.Parse(a.DescriptionDate.ToString())).ToList();
                    if (lstStatus.Count > 0)
                    {

                        getDataObj.StatusId = lstStatus[0].RequestStatus.Id;
                        getDataObj.StatusName = lstStatus[0].RequestStatus.Name;
                        getDataObj.StatusNameAr = lstStatus[0].RequestStatus.NameAr;
                        getDataObj.StatusColor = lstStatus[0].RequestStatus.Color;
                        getDataObj.StatusIcon = lstStatus[0].RequestStatus.Icon;
                        if (getDataObj.StatusId == 2)
                        {
                            getDataObj.ClosedDate = lstStatus[0].DescriptionDate.ToString();
                        }
                        else
                        {
                            getDataObj.ClosedDate = "";
                        }
                        list.Add(getDataObj);
                    }
                }
            }
            mainClass.Results = list.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            return mainClass;
        }
        #endregion




        public IndexAssetDetailVM SortAssetDetailAfterSearch(SortAndFilterDataModel data, int pageNumber, int pageSize)
        {


            IQueryable<AssetDetail> query = null;
            IndexAssetDetailVM mainClass = new IndexAssetDetailVM();
            List<IndexAssetDetailVM.GetData> list = new List<IndexAssetDetailVM.GetData>();
            List<AssetDetail> searchResult = new List<AssetDetail>();


            if (data.FilteredObj.AssetNameAr != "")
            {
                if (query is not null)
                {


                    if (data.SortObject.SortBy != "")
                    {
                        switch (data.SortObject.SortBy)
                        {
                            case "الاسم":
                                if (data.SortObject.SortStatus == "ascending")
                                {
                                    // filteration and sort then pagination 
                                    // Where() , OrderBy() then Skip() Take()
                                    query = query.Where(e => e.MasterAsset.NameAr == data.FilteredObj.AssetNameAr).
                                      OrderBy(ww => ww.MasterAsset.NameAr);
                                }
                                else
                                {
                                    query = query.Where(e => e.MasterAsset.NameAr == data.FilteredObj.AssetNameAr).
                                  OrderByDescending(ww => ww.MasterAsset.NameAr);
                                }
                                break;

                            case "Name":
                                if (data.SortObject.SortStatus == "ascending")
                                {
                                    query = query.Where(e => e.MasterAsset.NameAr == data.FilteredObj.AssetNameAr).
                                    OrderBy(ww => ww.MasterAsset.Name);

                                }
                                else
                                {

                                    query = query.Where(e => e.MasterAsset.NameAr == data.FilteredObj.AssetNameAr).
                                        OrderByDescending(ww => ww.MasterAsset.Name);

                                }

                                break;

                            case "الباركود":
                            case "Barcode":

                                if (data.SortObject.SortStatus == "ascending")
                                {
                                    query = query.Where(e => e.MasterAsset.NameAr == data.FilteredObj.AssetNameAr).
                                    OrderBy(ww => ww.Barcode);


                                }
                                else
                                {
                                    query = query.Where(e => e.MasterAsset.NameAr == data.FilteredObj.AssetNameAr).
                                      OrderByDescending(ww => ww.Barcode);

                                }

                                break;

                            case "السيريال":
                            case "Serial":
                                if (data.SortObject.SortStatus == "ascending")
                                {
                                    query = query.Where(e => e.MasterAsset.NameAr == data.FilteredObj.AssetNameAr).
                                    OrderBy(ww => ww.SerialNumber);

                                }
                                else
                                {
                                    query = query.Where(e => e.MasterAsset.NameAr == data.FilteredObj.AssetNameAr).
                                   OrderByDescending(ww => ww.SerialNumber);
                                }

                                break;

                            case "رقم الموديل":
                            case "Model Number":
                                if (data.SortObject.SortStatus == "ascending")
                                {
                                    query = query.Where(e => e.MasterAsset.NameAr == data.FilteredObj.AssetNameAr).
                                         OrderBy(ww => ww.MasterAsset.ModelNumber);
                                }
                                else
                                {
                                    query = query.Where(e => e.MasterAsset.NameAr == data.FilteredObj.AssetNameAr).
                                         OrderByDescending(ww => ww.MasterAsset.ModelNumber);
                                }
                                break;
                            case "القسم":
                                if (data.SortObject.SortStatus == "ascending")
                                {
                                    query = query.Where(e => e.MasterAsset.NameAr == data.FilteredObj.AssetNameAr).
                                      OrderBy(ww => ww.DepartmentId);
                                }
                                else
                                {
                                    query = query.Where(e => e.MasterAsset.NameAr == data.FilteredObj.AssetNameAr).
                                                    OrderByDescending(ww => ww.DepartmentId);

                                }
                                break;


                            case "Department":
                                if (data.SortObject.SortStatus == "ascending")
                                {
                                    query = query.Where(e => e.MasterAsset.NameAr == data.FilteredObj.AssetNameAr).
                                         OrderBy(ww => ww.DepartmentId);

                                }
                                else
                                {
                                    query = query.Where(e => e.MasterAsset.NameAr == data.FilteredObj.AssetNameAr).
                                                   OrderByDescending(ww => ww.DepartmentId);
                                }
                                break;

                            case "الماركة":
                                if (data.SortObject.SortStatus == "ascending")
                                {
                                    query = query.Where(e => e.MasterAsset.NameAr == data.FilteredObj.AssetNameAr).
                                        OrderBy(ww => ww.MasterAsset.brand.NameAr);


                                }
                                else
                                {
                                    query = query.Where(e => e.MasterAsset.NameAr == data.FilteredObj.AssetNameAr).
                                       OrderByDescending(ww => ww.MasterAsset.brand.NameAr);
                                }
                                break;
                            case "Manufacture":
                                if (data.SortObject.SortStatus == "ascending")

                                {


                                    query = query.Where(e => e.MasterAsset.NameAr == data.FilteredObj.AssetNameAr).
                                      OrderBy(ww => ww.MasterAsset.brand.Name);


                                }
                                else
                                {
                                    query = query.Where(e => e.MasterAsset.NameAr == data.FilteredObj.AssetNameAr).
                                OrderByDescending(ww => ww.MasterAsset.brand.Name);
                                }
                                break;
                            case "المورد":
                                if (data.SortObject.SortStatus == "ascending")
                                {

                                    query = query.Where(e => e.MasterAsset.NameAr == data.FilteredObj.AssetNameAr).
                                      OrderBy(ww => ww.SupplierId);
                                }
                                else
                                {
                                    query = query.Where(e => e.MasterAsset.NameAr == data.FilteredObj.AssetNameAr).
                                    OrderByDescending(ww => ww.SupplierId);
                                }
                                break;
                            case "Supplier":
                                if (data.SortObject.SortStatus == "ascending")
                                {

                                    query = query.Where(e => e.MasterAsset.NameAr == data.FilteredObj.AssetNameAr).
                                      OrderBy(ww => ww.SupplierId);
                                }
                                else
                                {
                                    query = query.Where(e => e.MasterAsset.NameAr == data.FilteredObj.AssetNameAr).
                              OrderByDescending(ww => ww.SupplierId);

                                }
                                break;
                            case "تاريخ الشراء":
                            case "Purchased Date":
                                if (data.SortObject.SortStatus == "ascending")
                                {
                                    query = query.Where(e => e.MasterAsset.NameAr == data.FilteredObj.AssetNameAr).
                                   OrderBy(ww => ww.PurchaseDate);
                                }
                                else
                                {
                                    query = query.Where(e => e.MasterAsset.NameAr == data.FilteredObj.AssetNameAr).
                                   OrderByDescending(ww => ww.PurchaseDate);
                                }
                                break;



                        }
                    }


                }

                else
                {

                    if (data.SortObject.SortBy != "")
                    {
                        switch (data.SortObject.SortBy)
                        {
                            case "الاسم":
                                if (data.SortObject.SortStatus == "ascending")
                                {
                                    // filteration and sort then pagination 
                                    // Where() , OrderBy() then Skip() Take()
                                    query = GetLstAssetDetails().Where(e => e.MasterAsset.NameAr == data.FilteredObj.AssetNameAr).
                                      OrderBy(ww => ww.MasterAsset.NameAr);
                                }
                                else
                                {
                                    query = GetLstAssetDetails().Where(e => e.MasterAsset.NameAr == data.FilteredObj.AssetNameAr).
                                  OrderByDescending(ww => ww.MasterAsset.NameAr);
                                }
                                break;

                            case "Name":
                                if (data.SortObject.SortStatus == "ascending")
                                {
                                    query = GetLstAssetDetails().Where(e => e.MasterAsset.NameAr == data.FilteredObj.AssetNameAr).
                                    OrderBy(ww => ww.MasterAsset.Name);

                                }
                                else
                                {

                                    query = GetLstAssetDetails().Where(e => e.MasterAsset.NameAr == data.FilteredObj.AssetNameAr).
                                        OrderByDescending(ww => ww.MasterAsset.Name);

                                }

                                break;

                            case "الباركود":
                            case "Barcode":

                                if (data.SortObject.SortStatus == "ascending")
                                {
                                    query = GetLstAssetDetails().Where(e => e.MasterAsset.NameAr == data.FilteredObj.AssetNameAr).
                                    OrderBy(ww => ww.Barcode);


                                }
                                else
                                {
                                    query = GetLstAssetDetails().Where(e => e.MasterAsset.NameAr == data.FilteredObj.AssetNameAr).
                                      OrderByDescending(ww => ww.Barcode);

                                }

                                break;

                            case "السيريال":
                            case "Serial":
                                if (data.SortObject.SortStatus == "ascending")
                                {
                                    query = GetLstAssetDetails().Where(e => e.MasterAsset.NameAr == data.FilteredObj.AssetNameAr).
                                    OrderBy(ww => ww.SerialNumber);

                                }
                                else
                                {
                                    query = GetLstAssetDetails().Where(e => e.MasterAsset.NameAr == data.FilteredObj.AssetNameAr).
                                   OrderByDescending(ww => ww.SerialNumber);
                                }

                                break;

                            case "رقم الموديل":
                            case "Model Number":
                                if (data.SortObject.SortStatus == "ascending")
                                {
                                    query = GetLstAssetDetails().Where(e => e.MasterAsset.NameAr == data.FilteredObj.AssetNameAr).
                                         OrderBy(ww => ww.MasterAsset.ModelNumber);
                                }
                                else
                                {
                                    query = GetLstAssetDetails().Where(e => e.MasterAsset.NameAr == data.FilteredObj.AssetNameAr).
                                         OrderByDescending(ww => ww.MasterAsset.ModelNumber);
                                }
                                break;
                            case "القسم":
                                if (data.SortObject.SortStatus == "ascending")
                                {
                                    query = GetLstAssetDetails().Where(e => e.MasterAsset.NameAr == data.FilteredObj.AssetNameAr).
                                      OrderBy(ww => ww.DepartmentId);
                                }
                                else
                                {
                                    query = GetLstAssetDetails().Where(e => e.MasterAsset.NameAr == data.FilteredObj.AssetNameAr).
                                                    OrderByDescending(ww => ww.DepartmentId);

                                }
                                break;


                            case "Department":
                                if (data.SortObject.SortStatus == "ascending")
                                {
                                    query = GetLstAssetDetails().Where(e => e.MasterAsset.NameAr == data.FilteredObj.AssetNameAr).
                                         OrderBy(ww => ww.DepartmentId);

                                }
                                else
                                {
                                    query = GetLstAssetDetails().Where(e => e.MasterAsset.NameAr == data.FilteredObj.AssetNameAr).
                                                   OrderByDescending(ww => ww.DepartmentId);
                                }
                                break;

                            case "الماركة":
                                if (data.SortObject.SortStatus == "ascending")
                                {
                                    query = GetLstAssetDetails().Where(e => e.MasterAsset.NameAr == data.FilteredObj.AssetNameAr).
                                        OrderBy(ww => ww.MasterAsset.brand.NameAr);


                                }
                                else
                                {
                                    query = GetLstAssetDetails().Where(e => e.MasterAsset.NameAr == data.FilteredObj.AssetNameAr).
                                       OrderByDescending(ww => ww.MasterAsset.brand.NameAr);
                                }
                                break;
                            case "Manufacture":
                                if (data.SortObject.SortStatus == "ascending")

                                {


                                    query = GetLstAssetDetails().Where(e => e.MasterAsset.NameAr == data.FilteredObj.AssetNameAr).
                                      OrderBy(ww => ww.MasterAsset.brand.Name);


                                }
                                else
                                {
                                    query = GetLstAssetDetails().Where(e => e.MasterAsset.NameAr == data.FilteredObj.AssetNameAr).
                                OrderByDescending(ww => ww.MasterAsset.brand.Name);
                                }
                                break;
                            case "المورد":
                                if (data.SortObject.SortStatus == "ascending")
                                {

                                    query = GetLstAssetDetails().Where(e => e.MasterAsset.NameAr == data.FilteredObj.AssetNameAr).
                                      OrderBy(ww => ww.SupplierId);
                                }
                                else
                                {
                                    query = GetLstAssetDetails().Where(e => e.MasterAsset.NameAr == data.FilteredObj.AssetNameAr).
                                    OrderByDescending(ww => ww.SupplierId);
                                }
                                break;
                            case "Supplier":
                                if (data.SortObject.SortStatus == "ascending")
                                {

                                    query = GetLstAssetDetails().Where(e => e.MasterAsset.NameAr == data.FilteredObj.AssetNameAr).
                                      OrderBy(ww => ww.SupplierId);
                                }
                                else
                                {
                                    query = GetLstAssetDetails().Where(e => e.MasterAsset.NameAr == data.FilteredObj.AssetNameAr).
                              OrderByDescending(ww => ww.SupplierId);

                                }
                                break;
                            case "تاريخ الشراء":
                            case "Purchased Date":
                                if (data.SortObject.SortStatus == "ascending")
                                {
                                    query = GetLstAssetDetails().Where(e => e.MasterAsset.NameAr == data.FilteredObj.AssetNameAr).
                                   OrderBy(ww => ww.PurchaseDate);
                                }
                                else
                                {
                                    query = GetLstAssetDetails().Where(e => e.MasterAsset.NameAr == data.FilteredObj.AssetNameAr).
                                   OrderByDescending(ww => ww.PurchaseDate);
                                }
                                break;



                        }
                    }






                }

                mainClass.Count = query.Count();
                searchResult = query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            }

            if (data.FilteredObj.AssetName != "")
            {
                if (query is not null)
                {
                    if (data.SortObject.SortBy != "")
                    {
                        switch (data.SortObject.SortBy)
                        {
                            case "الاسم":
                                if (data.SortObject.SortStatus == "ascending")
                                {
                                    // filteration and sort then pagination 
                                    // Where() , OrderBy() then Skip() Take()
                                    query = query.Where(e => e.MasterAsset.Name == data.FilteredObj.AssetName).
                                      OrderBy(ww => ww.MasterAsset.NameAr);
                                }
                                else
                                {
                                    query = query.Where(e => e.MasterAsset.Name == data.FilteredObj.AssetName).
                                  OrderByDescending(ww => ww.MasterAsset.NameAr);
                                }
                                break;

                            case "Name":
                                if (data.SortObject.SortStatus == "ascending")
                                {
                                    query = query.Where(e => e.MasterAsset.Name == data.FilteredObj.AssetName).
                                    OrderBy(ww => ww.MasterAsset.Name);

                                }
                                else
                                {

                                    query = query.Where(e => e.MasterAsset.Name == data.FilteredObj.AssetName).
                                        OrderByDescending(ww => ww.MasterAsset.Name);

                                }

                                break;

                            case "الباركود":
                            case "Barcode":

                                if (data.SortObject.SortStatus == "ascending")
                                {
                                    query = query.Where(e => e.MasterAsset.Name == data.FilteredObj.AssetName).
                                    OrderBy(ww => ww.Barcode);


                                }
                                else
                                {
                                    query = query.Where(e => e.MasterAsset.Name == data.FilteredObj.AssetName).
                                      OrderByDescending(ww => ww.Barcode);

                                }

                                break;

                            case "السيريال":
                            case "Serial":
                                if (data.SortObject.SortStatus == "ascending")
                                {
                                    query = query.Where(e => e.MasterAsset.Name == data.FilteredObj.AssetName).
                                    OrderBy(ww => ww.SerialNumber);

                                }
                                else
                                {
                                    query = query.Where(e => e.MasterAsset.Name == data.FilteredObj.AssetName).
                                   OrderByDescending(ww => ww.SerialNumber);
                                }

                                break;

                            case "رقم الموديل":
                            case "Model Number":
                                if (data.SortObject.SortStatus == "ascending")
                                {
                                    query = query.Where(e => e.MasterAsset.Name == data.FilteredObj.AssetName).
                                         OrderBy(ww => ww.MasterAsset.ModelNumber);
                                }
                                else
                                {
                                    query = query.Where(e => e.MasterAsset.Name == data.FilteredObj.AssetName).
                                         OrderByDescending(ww => ww.MasterAsset.ModelNumber);
                                }
                                break;
                            case "القسم":
                                if (data.SortObject.SortStatus == "ascending")
                                {
                                    query = query.Where(e => e.MasterAsset.Name == data.FilteredObj.AssetName).
                                      OrderBy(ww => ww.DepartmentId);
                                }
                                else
                                {
                                    query = query.Where(e => e.MasterAsset.Name == data.FilteredObj.AssetName).
                                                    OrderByDescending(ww => ww.DepartmentId);

                                }
                                break;


                            case "Department":
                                if (data.SortObject.SortStatus == "ascending")
                                {
                                    query = query.Where(e => e.MasterAsset.Name == data.FilteredObj.AssetName).
                                         OrderBy(ww => ww.DepartmentId);

                                }
                                else
                                {
                                    query = query.Where(e => e.MasterAsset.Name == data.FilteredObj.AssetName).
                                                   OrderByDescending(ww => ww.DepartmentId);
                                }
                                break;

                            case "الماركة":
                                if (data.SortObject.SortStatus == "ascending")
                                {
                                    query = query.Where(e => e.MasterAsset.Name == data.FilteredObj.AssetName).
                                        OrderBy(ww => ww.MasterAsset.brand.NameAr);


                                }
                                else
                                {
                                    query = query.Where(e => e.MasterAsset.Name == data.FilteredObj.AssetName).
                                       OrderByDescending(ww => ww.MasterAsset.brand.NameAr);
                                }
                                break;
                            case "Manufacture":
                                if (data.SortObject.SortStatus == "ascending")

                                {


                                    query = query.Where(e => e.MasterAsset.Name == data.FilteredObj.AssetName).
                                      OrderBy(ww => ww.MasterAsset.brand.Name);


                                }
                                else
                                {
                                    query = query.Where(e => e.MasterAsset.Name == data.FilteredObj.AssetName).
                                OrderByDescending(ww => ww.MasterAsset.brand.Name);
                                }
                                break;
                            case "المورد":
                                if (data.SortObject.SortStatus == "ascending")
                                {

                                    query = query.Where(e => e.MasterAsset.Name == data.FilteredObj.AssetName).
                                      OrderBy(ww => ww.SupplierId);
                                }
                                else
                                {
                                    query = query.Where(e => e.MasterAsset.Name == data.FilteredObj.AssetName).
                                    OrderByDescending(ww => ww.SupplierId);
                                }
                                break;
                            case "Supplier":
                                if (data.SortObject.SortStatus == "ascending")
                                {

                                    query = query.Where(e => e.MasterAsset.Name == data.FilteredObj.AssetName).
                                      OrderBy(ww => ww.SupplierId);
                                }
                                else
                                {
                                    query = query.Where(e => e.MasterAsset.Name == data.FilteredObj.AssetName).
                              OrderByDescending(ww => ww.SupplierId);

                                }
                                break;
                            case "تاريخ الشراء":
                            case "Purchased Date":
                                if (data.SortObject.SortStatus == "ascending")
                                {
                                    query = query.Where(e => e.MasterAsset.Name == data.FilteredObj.AssetName).
                                   OrderBy(ww => ww.PurchaseDate);
                                }
                                else
                                {
                                    query = query.Where(e => e.MasterAsset.Name == data.FilteredObj.AssetName).
                                   OrderByDescending(ww => ww.PurchaseDate);
                                }
                                break;



                        }
                    }

                }
                else
                {

                    if (data.SortObject.SortBy != "")
                    {
                        switch (data.SortObject.SortBy)
                        {
                            case "الاسم":
                                if (data.SortObject.SortStatus == "ascending")
                                {
                                    // filteration and sort then pagination 
                                    // Where() , OrderBy() then Skip() Take()
                                    query = GetLstAssetDetails().Where(e => e.MasterAsset.Name == data.FilteredObj.AssetName).
                                      OrderBy(ww => ww.MasterAsset.NameAr);
                                }
                                else
                                {
                                    query = GetLstAssetDetails().Where(e => e.MasterAsset.Name == data.FilteredObj.AssetName).
                                  OrderByDescending(ww => ww.MasterAsset.NameAr);
                                }
                                break;

                            case "Name":
                                if (data.SortObject.SortStatus == "ascending")
                                {
                                    query = GetLstAssetDetails().Where(e => e.MasterAsset.Name == data.FilteredObj.AssetName).
                                    OrderBy(ww => ww.MasterAsset.Name);

                                }
                                else
                                {

                                    query = GetLstAssetDetails().Where(e => e.MasterAsset.Name == data.FilteredObj.AssetName).
                                        OrderByDescending(ww => ww.MasterAsset.Name);

                                }

                                break;

                            case "الباركود":
                            case "Barcode":

                                if (data.SortObject.SortStatus == "ascending")
                                {
                                    query = GetLstAssetDetails().Where(e => e.MasterAsset.Name == data.FilteredObj.AssetName).
                                    OrderBy(ww => ww.Barcode);


                                }
                                else
                                {
                                    query = GetLstAssetDetails().Where(e => e.MasterAsset.Name == data.FilteredObj.AssetName).
                                      OrderByDescending(ww => ww.Barcode);

                                }

                                break;

                            case "السيريال":
                            case "Serial":
                                if (data.SortObject.SortStatus == "ascending")
                                {
                                    query = GetLstAssetDetails().Where(e => e.MasterAsset.Name == data.FilteredObj.AssetName).
                                    OrderBy(ww => ww.SerialNumber);

                                }
                                else
                                {
                                    query = GetLstAssetDetails().Where(e => e.MasterAsset.Name == data.FilteredObj.AssetName).
                                   OrderByDescending(ww => ww.SerialNumber);
                                }

                                break;

                            case "رقم الموديل":
                            case "Model Number":
                                if (data.SortObject.SortStatus == "ascending")
                                {
                                    query = GetLstAssetDetails().Where(e => e.MasterAsset.Name == data.FilteredObj.AssetName).
                                         OrderBy(ww => ww.MasterAsset.ModelNumber);
                                }
                                else
                                {
                                    query = GetLstAssetDetails().Where(e => e.MasterAsset.Name == data.FilteredObj.AssetName).
                                         OrderByDescending(ww => ww.MasterAsset.ModelNumber);
                                }
                                break;
                            case "القسم":
                                if (data.SortObject.SortStatus == "ascending")
                                {
                                    query = GetLstAssetDetails().Where(e => e.MasterAsset.Name == data.FilteredObj.AssetName).
                                      OrderBy(ww => ww.DepartmentId);
                                }
                                else
                                {
                                    query = GetLstAssetDetails().Where(e => e.MasterAsset.Name == data.FilteredObj.AssetName).
                                                    OrderByDescending(ww => ww.DepartmentId);

                                }
                                break;


                            case "Department":
                                if (data.SortObject.SortStatus == "ascending")
                                {
                                    query = GetLstAssetDetails().Where(e => e.MasterAsset.Name == data.FilteredObj.AssetName).
                                         OrderBy(ww => ww.DepartmentId);

                                }
                                else
                                {
                                    query = GetLstAssetDetails().Where(e => e.MasterAsset.Name == data.FilteredObj.AssetName).
                                                   OrderByDescending(ww => ww.DepartmentId);
                                }
                                break;

                            case "الماركة":
                                if (data.SortObject.SortStatus == "ascending")
                                {
                                    query = GetLstAssetDetails().Where(e => e.MasterAsset.Name == data.FilteredObj.AssetName).
                                        OrderBy(ww => ww.MasterAsset.brand.NameAr);


                                }
                                else
                                {
                                    query = GetLstAssetDetails().Where(e => e.MasterAsset.Name == data.FilteredObj.AssetName).
                                       OrderByDescending(ww => ww.MasterAsset.brand.NameAr);
                                }
                                break;
                            case "Manufacture":
                                if (data.SortObject.SortStatus == "ascending")

                                {


                                    query = GetLstAssetDetails().Where(e => e.MasterAsset.Name == data.FilteredObj.AssetName).
                                      OrderBy(ww => ww.MasterAsset.brand.Name);


                                }
                                else
                                {
                                    query = GetLstAssetDetails().Where(e => e.MasterAsset.Name == data.FilteredObj.AssetName).
                                OrderByDescending(ww => ww.MasterAsset.brand.Name);
                                }
                                break;
                            case "المورد":
                                if (data.SortObject.SortStatus == "ascending")
                                {

                                    query = GetLstAssetDetails().Where(e => e.MasterAsset.Name == data.FilteredObj.AssetName).
                                      OrderBy(ww => ww.SupplierId);
                                }
                                else
                                {
                                    query = GetLstAssetDetails().Where(e => e.MasterAsset.Name == data.FilteredObj.AssetName).
                                    OrderByDescending(ww => ww.SupplierId);
                                }
                                break;
                            case "Supplier":
                                if (data.SortObject.SortStatus == "ascending")
                                {

                                    query = GetLstAssetDetails().Where(e => e.MasterAsset.Name == data.FilteredObj.AssetName).
                                      OrderBy(ww => ww.SupplierId);
                                }
                                else
                                {
                                    query = GetLstAssetDetails().Where(e => e.MasterAsset.Name == data.FilteredObj.AssetName).
                              OrderByDescending(ww => ww.SupplierId);

                                }
                                break;
                            case "تاريخ الشراء":
                            case "Purchased Date":
                                if (data.SortObject.SortStatus == "ascending")
                                {
                                    query = GetLstAssetDetails().Where(e => e.MasterAsset.Name == data.FilteredObj.AssetName).
                                   OrderBy(ww => ww.PurchaseDate);
                                }
                                else
                                {
                                    query = GetLstAssetDetails().Where(e => e.MasterAsset.Name == data.FilteredObj.AssetName).
                                   OrderByDescending(ww => ww.PurchaseDate);
                                }
                                break;



                        }
                    }



                }

                mainClass.Count = query.Count();
                searchResult = query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            }



            if (data.FilteredObj.DepartmentId != 0)
            {
                if (query is not null)
                {
                    if (data.SortObject.SortBy != "")
                    {
                        switch (data.SortObject.SortBy)
                        {
                            case "الاسم":
                                if (data.SortObject.SortStatus == "ascending")
                                {
                                    // filteration and sort then pagination 
                                    // Where() , OrderBy() then Skip() Take()
                                    query = query.Where(e => e.DepartmentId == data.FilteredObj.DepartmentId).
                                      OrderBy(ww => ww.MasterAsset.NameAr);
                                }
                                else
                                {
                                    query = query.Where(e => e.DepartmentId == data.FilteredObj.DepartmentId).
                                  OrderByDescending(ww => ww.MasterAsset.NameAr);
                                }
                                break;

                            case "Name":
                                if (data.SortObject.SortStatus == "ascending")
                                {
                                    query = query.Where(e => e.DepartmentId == data.FilteredObj.DepartmentId).
                                    OrderBy(ww => ww.MasterAsset.Name);

                                }
                                else
                                {

                                    query = query.Where(e => e.DepartmentId == data.FilteredObj.DepartmentId).
                                        OrderByDescending(ww => ww.MasterAsset.Name);

                                }

                                break;

                            case "الباركود":
                            case "Barcode":

                                if (data.SortObject.SortStatus == "ascending")
                                {
                                    query = query.Where(e => e.DepartmentId == data.FilteredObj.DepartmentId).
                                    OrderBy(ww => ww.Barcode);


                                }
                                else
                                {
                                    query = query.Where(e => e.DepartmentId == data.FilteredObj.DepartmentId).
                                      OrderByDescending(ww => ww.Barcode);

                                }

                                break;

                            case "السيريال":
                            case "Serial":
                                if (data.SortObject.SortStatus == "ascending")
                                {
                                    query = query.Where(e => e.DepartmentId == data.FilteredObj.DepartmentId).
                                    OrderBy(ww => ww.SerialNumber);

                                }
                                else
                                {
                                    query = query.Where(e => e.DepartmentId == data.FilteredObj.DepartmentId).
                                   OrderByDescending(ww => ww.SerialNumber);
                                }

                                break;

                            case "رقم الموديل":
                            case "Model Number":
                                if (data.SortObject.SortStatus == "ascending")
                                {
                                    query = query.Where(e => e.DepartmentId == data.FilteredObj.DepartmentId).
                                         OrderBy(ww => ww.MasterAsset.ModelNumber);
                                }
                                else
                                {
                                    query = query.Where(e => e.DepartmentId == data.FilteredObj.DepartmentId).
                                         OrderByDescending(ww => ww.MasterAsset.ModelNumber);
                                }
                                break;
                            case "القسم":
                                if (data.SortObject.SortStatus == "ascending")
                                {
                                    query = query.Where(e => e.DepartmentId == data.FilteredObj.DepartmentId).
                                      OrderBy(ww => ww.DepartmentId);
                                }
                                else
                                {
                                    query = query.Where(e => e.DepartmentId == data.FilteredObj.DepartmentId).
                                                    OrderByDescending(ww => ww.DepartmentId);

                                }
                                break;


                            case "Department":
                                if (data.SortObject.SortStatus == "ascending")
                                {
                                    query = query.Where(e => e.DepartmentId == data.FilteredObj.DepartmentId).
                                         OrderBy(ww => ww.DepartmentId);

                                }
                                else
                                {
                                    query = query.Where(e => e.DepartmentId == data.FilteredObj.DepartmentId).
                                                   OrderByDescending(ww => ww.DepartmentId);
                                }
                                break;

                            case "الماركة":
                                if (data.SortObject.SortStatus == "ascending")
                                {
                                    query = query.Where(e => e.DepartmentId == data.FilteredObj.DepartmentId).
                                        OrderBy(ww => ww.MasterAsset.brand.NameAr);


                                }
                                else
                                {
                                    query = query.Where(e => e.DepartmentId == data.FilteredObj.DepartmentId).
                                       OrderByDescending(ww => ww.MasterAsset.brand.NameAr);
                                }
                                break;
                            case "Manufacture":
                                if (data.SortObject.SortStatus == "ascending")

                                {


                                    query = query.Where(e => e.DepartmentId == data.FilteredObj.DepartmentId).
                                      OrderBy(ww => ww.MasterAsset.brand.Name);


                                }
                                else
                                {
                                    query = query.Where(e => e.DepartmentId == data.FilteredObj.DepartmentId).
                                OrderByDescending(ww => ww.MasterAsset.brand.Name);
                                }
                                break;
                            case "المورد":
                                if (data.SortObject.SortStatus == "ascending")
                                {

                                    query = query.Where(e => e.DepartmentId == data.FilteredObj.DepartmentId).
                                      OrderBy(ww => ww.SupplierId);
                                }
                                else
                                {
                                    query = query.Where(e => e.DepartmentId == data.FilteredObj.DepartmentId).
                                    OrderByDescending(ww => ww.SupplierId);
                                }
                                break;
                            case "Supplier":
                                if (data.SortObject.SortStatus == "ascending")
                                {

                                    query = query.Where(e => e.DepartmentId == data.FilteredObj.DepartmentId).
                                      OrderBy(ww => ww.SupplierId);
                                }
                                else
                                {
                                    query = query.Where(e => e.DepartmentId == data.FilteredObj.DepartmentId).
                              OrderByDescending(ww => ww.SupplierId);

                                }
                                break;
                            case "تاريخ الشراء":
                            case "Purchased Date":
                                if (data.SortObject.SortStatus == "ascending")
                                {
                                    query = query.Where(e => e.DepartmentId == data.FilteredObj.DepartmentId).
                                   OrderBy(ww => ww.PurchaseDate);
                                }
                                else
                                {
                                    query = query.Where(e => e.MasterAsset.NameAr == data.FilteredObj.AssetNameAr).
                                   OrderByDescending(ww => ww.PurchaseDate);
                                }
                                break;



                        }
                    }

                }
                else

                {
                    switch (data.SortObject.SortBy)
                    {
                        case "الاسم":
                            if (data.SortObject.SortStatus == "ascending")
                            {
                                // filteration and sort then pagination 
                                // Where() , OrderBy() then Skip() Take()
                                query = GetLstAssetDetails().Where(e => e.DepartmentId == data.FilteredObj.DepartmentId).
                                  OrderBy(ww => ww.MasterAsset.NameAr);
                            }
                            else
                            {
                                query = GetLstAssetDetails().Where(e => e.DepartmentId == data.FilteredObj.DepartmentId).
                              OrderByDescending(ww => ww.MasterAsset.NameAr);
                            }
                            break;

                        case "Name":
                            if (data.SortObject.SortStatus == "ascending")
                            {
                                query = GetLstAssetDetails().Where(e => e.DepartmentId == data.FilteredObj.DepartmentId).
                                OrderBy(ww => ww.MasterAsset.Name);

                            }
                            else
                            {

                                query = GetLstAssetDetails().Where(e => e.DepartmentId == data.FilteredObj.DepartmentId).
                                    OrderByDescending(ww => ww.MasterAsset.Name);

                            }

                            break;

                        case "الباركود":
                        case "Barcode":

                            if (data.SortObject.SortStatus == "ascending")
                            {
                                query = GetLstAssetDetails().Where(e => e.DepartmentId == data.FilteredObj.DepartmentId).
                                OrderBy(ww => ww.Barcode);


                            }
                            else
                            {
                                query = GetLstAssetDetails().Where(e => e.DepartmentId == data.FilteredObj.DepartmentId).
                                  OrderByDescending(ww => ww.Barcode);

                            }

                            break;

                        case "السيريال":
                        case "Serial":
                            if (data.SortObject.SortStatus == "ascending")
                            {
                                query = GetLstAssetDetails().Where(e => e.DepartmentId == data.FilteredObj.DepartmentId).
                                OrderBy(ww => ww.SerialNumber);

                            }
                            else
                            {
                                query = GetLstAssetDetails().Where(e => e.DepartmentId == data.FilteredObj.DepartmentId).
                               OrderByDescending(ww => ww.SerialNumber);
                            }

                            break;

                        case "رقم الموديل":
                        case "Model Number":
                            if (data.SortObject.SortStatus == "ascending")
                            {
                                query = GetLstAssetDetails().Where(e => e.DepartmentId == data.FilteredObj.DepartmentId).
                                     OrderBy(ww => ww.MasterAsset.ModelNumber);
                            }
                            else
                            {
                                query = GetLstAssetDetails().Where(e => e.DepartmentId == data.FilteredObj.DepartmentId).
                                     OrderByDescending(ww => ww.MasterAsset.ModelNumber);
                            }
                            break;
                        case "القسم":
                            if (data.SortObject.SortStatus == "ascending")
                            {
                                query = GetLstAssetDetails().Where(e => e.DepartmentId == data.FilteredObj.DepartmentId).
                                  OrderBy(ww => ww.DepartmentId);
                            }
                            else
                            {
                                query = GetLstAssetDetails().Where(e => e.DepartmentId == data.FilteredObj.DepartmentId).
                                                OrderByDescending(ww => ww.DepartmentId);

                            }
                            break;


                        case "Department":
                            if (data.SortObject.SortStatus == "ascending")
                            {
                                query = GetLstAssetDetails().Where(e => e.DepartmentId == data.FilteredObj.DepartmentId).
                                     OrderBy(ww => ww.DepartmentId);

                            }
                            else
                            {
                                query = GetLstAssetDetails().Where(e => e.DepartmentId == data.FilteredObj.DepartmentId).
                                               OrderByDescending(ww => ww.DepartmentId);
                            }
                            break;

                        case "الماركة":
                            if (data.SortObject.SortStatus == "ascending")
                            {
                                query = GetLstAssetDetails().Where(e => e.DepartmentId == data.FilteredObj.DepartmentId).
                                    OrderBy(ww => ww.MasterAsset.brand.NameAr);


                            }
                            else
                            {
                                query = GetLstAssetDetails().Where(e => e.DepartmentId == data.FilteredObj.DepartmentId).
                                   OrderByDescending(ww => ww.MasterAsset.brand.NameAr);
                            }
                            break;
                        case "Manufacture":
                            if (data.SortObject.SortStatus == "ascending")

                            {


                                query = GetLstAssetDetails().Where(e => e.DepartmentId == data.FilteredObj.DepartmentId).
                                  OrderBy(ww => ww.MasterAsset.brand.Name);


                            }
                            else
                            {
                                query = GetLstAssetDetails().Where(e => e.DepartmentId == data.FilteredObj.DepartmentId).
                            OrderByDescending(ww => ww.MasterAsset.brand.Name);
                            }
                            break;
                        case "المورد":
                            if (data.SortObject.SortStatus == "ascending")
                            {

                                query = GetLstAssetDetails().Where(e => e.DepartmentId == data.FilteredObj.DepartmentId).
                                  OrderBy(ww => ww.SupplierId);
                            }
                            else
                            {
                                query = GetLstAssetDetails().Where(e => e.DepartmentId == data.FilteredObj.DepartmentId).
                                OrderByDescending(ww => ww.SupplierId);
                            }
                            break;
                        case "Supplier":
                            if (data.SortObject.SortStatus == "ascending")
                            {

                                query = GetLstAssetDetails().Where(e => e.DepartmentId == data.FilteredObj.DepartmentId).
                                  OrderBy(ww => ww.SupplierId);
                            }
                            else
                            {
                                query = GetLstAssetDetails().Where(e => e.DepartmentId == data.FilteredObj.DepartmentId).
                          OrderByDescending(ww => ww.SupplierId);

                            }
                            break;
                        case "تاريخ الشراء":
                        case "Purchased Date":
                            if (data.SortObject.SortStatus == "ascending")
                            {
                                query = GetLstAssetDetails().Where(e => e.DepartmentId == data.FilteredObj.DepartmentId).
                               OrderBy(ww => ww.PurchaseDate);
                            }
                            else
                            {
                                query = GetLstAssetDetails().Where(e => e.DepartmentId == data.FilteredObj.DepartmentId).
                               OrderByDescending(ww => ww.PurchaseDate);
                            }
                            break;



                    }

                }

                mainClass.Count = query.Count();
                searchResult = query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();


            }


            if (data.FilteredObj.BrandId != 0)
            {
                if (query is not null)
                {
                    switch (data.SortObject.SortBy)
                    {
                        case "الاسم":
                            if (data.SortObject.SortStatus == "ascending")
                            {
                                // filteration and sort then pagination 
                                // Where() , OrderBy() then Skip() Take()
                                query = query.Where(e => e.MasterAsset.BrandId == data.FilteredObj.BrandId).
                                  OrderBy(ww => ww.MasterAsset.NameAr);
                            }
                            else
                            {
                                query = query.Where(e => e.MasterAsset.BrandId == data.FilteredObj.BrandId).
                              OrderByDescending(ww => ww.MasterAsset.NameAr);
                            }
                            break;

                        case "Name":
                            if (data.SortObject.SortStatus == "ascending")
                            {
                                query = query.Where(e => e.MasterAsset.BrandId == data.FilteredObj.BrandId).
                                OrderBy(ww => ww.MasterAsset.Name);

                            }
                            else
                            {

                                query = query.Where(e => e.MasterAsset.BrandId == data.FilteredObj.BrandId).
                                    OrderByDescending(ww => ww.MasterAsset.Name);

                            }

                            break;

                        case "الباركود":
                        case "Barcode":

                            if (data.SortObject.SortStatus == "ascending")
                            {
                                query = query.Where(e => e.MasterAsset.BrandId == data.FilteredObj.BrandId).
                                OrderBy(ww => ww.Barcode);


                            }
                            else
                            {
                                query = query.Where(e => e.MasterAsset.BrandId == data.FilteredObj.BrandId).
                                  OrderByDescending(ww => ww.Barcode);

                            }

                            break;

                        case "السيريال":
                        case "Serial":
                            if (data.SortObject.SortStatus == "ascending")
                            {
                                query = query.Where(e => e.MasterAsset.BrandId == data.FilteredObj.BrandId).
                                OrderBy(ww => ww.SerialNumber);

                            }
                            else
                            {
                                query = query.Where(e => e.MasterAsset.BrandId == data.FilteredObj.BrandId).
                               OrderByDescending(ww => ww.SerialNumber);
                            }

                            break;

                        case "رقم الموديل":
                        case "Model Number":
                            if (data.SortObject.SortStatus == "ascending")
                            {
                                query = query.Where(e => e.MasterAsset.BrandId == data.FilteredObj.BrandId).
                                     OrderBy(ww => ww.MasterAsset.ModelNumber);
                            }
                            else
                            {
                                query = query.Where(e => e.MasterAsset.BrandId == data.FilteredObj.BrandId).
                                     OrderByDescending(ww => ww.MasterAsset.ModelNumber);
                            }
                            break;
                        case "القسم":
                            if (data.SortObject.SortStatus == "ascending")
                            {
                                query = query.Where(e => e.MasterAsset.BrandId == data.FilteredObj.BrandId).
                                  OrderBy(ww => ww.DepartmentId);
                            }
                            else
                            {
                                query = query.Where(e => e.MasterAsset.BrandId == data.FilteredObj.BrandId).
                                                OrderByDescending(ww => ww.DepartmentId);

                            }
                            break;


                        case "Department":
                            if (data.SortObject.SortStatus == "ascending")
                            {
                                query = query.Where(e => e.MasterAsset.BrandId == data.FilteredObj.BrandId).
                                     OrderBy(ww => ww.DepartmentId);

                            }
                            else
                            {
                                query = query.Where(e => e.MasterAsset.BrandId == data.FilteredObj.BrandId).
                                               OrderByDescending(ww => ww.DepartmentId);
                            }
                            break;

                        case "الماركة":
                            if (data.SortObject.SortStatus == "ascending")
                            {
                                query = query.Where(e => e.MasterAsset.BrandId == data.FilteredObj.BrandId).
                                    OrderBy(ww => ww.MasterAsset.brand.NameAr);


                            }
                            else
                            {
                                query = query.Where(e => e.MasterAsset.BrandId == data.FilteredObj.BrandId).
                                   OrderByDescending(ww => ww.MasterAsset.brand.NameAr);
                            }
                            break;
                        case "Manufacture":
                            if (data.SortObject.SortStatus == "ascending")

                            {


                                query = query.Where(e => e.MasterAsset.BrandId == data.FilteredObj.BrandId).
                                  OrderBy(ww => ww.MasterAsset.brand.Name);


                            }
                            else
                            {
                                query = query.Where(e => e.MasterAsset.BrandId == data.FilteredObj.BrandId).
                            OrderByDescending(ww => ww.MasterAsset.brand.Name);
                            }
                            break;
                        case "المورد":
                            if (data.SortObject.SortStatus == "ascending")
                            {

                                query = query.Where(e => e.MasterAsset.BrandId == data.FilteredObj.BrandId).
                                  OrderBy(ww => ww.SupplierId);
                            }
                            else
                            {
                                query = query.Where(e => e.MasterAsset.BrandId == data.FilteredObj.BrandId).
                                OrderByDescending(ww => ww.SupplierId);
                            }
                            break;
                        case "Supplier":
                            if (data.SortObject.SortStatus == "ascending")
                            {

                                query = query.Where(e => e.MasterAsset.BrandId == data.FilteredObj.BrandId).
                                  OrderBy(ww => ww.SupplierId);
                            }
                            else
                            {
                                query = query.Where(e => e.MasterAsset.BrandId == data.FilteredObj.BrandId).
                          OrderByDescending(ww => ww.SupplierId);

                            }
                            break;
                        case "تاريخ الشراء":
                        case "Purchased Date":
                            if (data.SortObject.SortStatus == "ascending")
                            {
                                query = query.Where(e => e.MasterAsset.BrandId == data.FilteredObj.BrandId).
                               OrderBy(ww => ww.PurchaseDate);
                            }
                            else
                            {
                                query = query.Where(e => e.MasterAsset.NameAr == data.FilteredObj.AssetNameAr).
                               OrderByDescending(ww => ww.PurchaseDate);
                            }
                            break;



                    }

                }

                else
                {
                    switch (data.SortObject.SortBy)
                    {
                        case "الاسم":
                            if (data.SortObject.SortStatus == "ascending")
                            {
                                // filteration and sort then pagination 
                                // Where() , OrderBy() then Skip() Take()
                                query = GetLstAssetDetails().Where(e => e.MasterAsset.BrandId == data.FilteredObj.BrandId).
                                  OrderBy(ww => ww.MasterAsset.NameAr);
                            }
                            else
                            {
                                query = GetLstAssetDetails().Where(e => e.MasterAsset.BrandId == data.FilteredObj.BrandId).
                              OrderByDescending(ww => ww.MasterAsset.NameAr);
                            }
                            break;

                        case "Name":
                            if (data.SortObject.SortStatus == "ascending")
                            {
                                query = GetLstAssetDetails().Where(e => e.MasterAsset.BrandId == data.FilteredObj.BrandId).
                                OrderBy(ww => ww.MasterAsset.Name);

                            }
                            else
                            {

                                query = GetLstAssetDetails().Where(e => e.MasterAsset.BrandId == data.FilteredObj.BrandId).
                                    OrderByDescending(ww => ww.MasterAsset.Name);

                            }

                            break;

                        case "الباركود":
                        case "Barcode":

                            if (data.SortObject.SortStatus == "ascending")
                            {
                                query = GetLstAssetDetails().Where(e => e.MasterAsset.BrandId == data.FilteredObj.BrandId).
                                OrderBy(ww => ww.Barcode);


                            }
                            else
                            {
                                query = GetLstAssetDetails().Where(e => e.MasterAsset.BrandId == data.FilteredObj.BrandId).
                                  OrderByDescending(ww => ww.Barcode);

                            }

                            break;

                        case "السيريال":
                        case "Serial":
                            if (data.SortObject.SortStatus == "ascending")
                            {
                                query = GetLstAssetDetails().Where(e => e.MasterAsset.BrandId == data.FilteredObj.BrandId).
                                OrderBy(ww => ww.SerialNumber);

                            }
                            else
                            {
                                query = GetLstAssetDetails().Where(e => e.MasterAsset.BrandId == data.FilteredObj.BrandId).
                               OrderByDescending(ww => ww.SerialNumber);
                            }

                            break;

                        case "رقم الموديل":
                        case "Model Number":
                            if (data.SortObject.SortStatus == "ascending")
                            {
                                query = GetLstAssetDetails().Where(e => e.MasterAsset.BrandId == data.FilteredObj.BrandId).
                                     OrderBy(ww => ww.MasterAsset.ModelNumber);
                            }
                            else
                            {
                                query = GetLstAssetDetails().Where(e => e.MasterAsset.BrandId == data.FilteredObj.BrandId).
                                     OrderByDescending(ww => ww.MasterAsset.ModelNumber);
                            }
                            break;
                        case "القسم":
                            if (data.SortObject.SortStatus == "ascending")
                            {
                                query = GetLstAssetDetails().Where(e => e.MasterAsset.BrandId == data.FilteredObj.BrandId).
                                  OrderBy(ww => ww.DepartmentId);
                            }
                            else
                            {
                                query = GetLstAssetDetails().Where(e => e.MasterAsset.BrandId == data.FilteredObj.BrandId).
                                                OrderByDescending(ww => ww.DepartmentId);

                            }
                            break;


                        case "Department":
                            if (data.SortObject.SortStatus == "ascending")
                            {
                                query = GetLstAssetDetails().Where(e => e.MasterAsset.BrandId == data.FilteredObj.BrandId).
                                     OrderBy(ww => ww.DepartmentId);

                            }
                            else
                            {
                                query = GetLstAssetDetails().Where(e => e.MasterAsset.BrandId == data.FilteredObj.BrandId).
                                               OrderByDescending(ww => ww.DepartmentId);
                            }
                            break;

                        case "الماركة":
                            if (data.SortObject.SortStatus == "ascending")
                            {
                                query = GetLstAssetDetails().Where(e => e.MasterAsset.BrandId == data.FilteredObj.BrandId).
                                    OrderBy(ww => ww.MasterAsset.brand.NameAr);


                            }
                            else
                            {
                                query = GetLstAssetDetails().Where(e => e.MasterAsset.BrandId == data.FilteredObj.BrandId).
                                   OrderByDescending(ww => ww.MasterAsset.brand.NameAr);
                            }
                            break;
                        case "Manufacture":
                            if (data.SortObject.SortStatus == "ascending")

                            {


                                query = GetLstAssetDetails().Where(e => e.MasterAsset.BrandId == data.FilteredObj.BrandId).
                                  OrderBy(ww => ww.MasterAsset.brand.Name);


                            }
                            else
                            {
                                query = GetLstAssetDetails().Where(e => e.MasterAsset.BrandId == data.FilteredObj.BrandId).
                            OrderByDescending(ww => ww.MasterAsset.brand.Name);
                            }
                            break;
                        case "المورد":
                            if (data.SortObject.SortStatus == "ascending")
                            {

                                query = GetLstAssetDetails().Where(e => e.MasterAsset.BrandId == data.FilteredObj.BrandId).
                                  OrderBy(ww => ww.SupplierId);
                            }
                            else
                            {
                                query = GetLstAssetDetails().Where(e => e.MasterAsset.BrandId == data.FilteredObj.BrandId).
                                OrderByDescending(ww => ww.SupplierId);
                            }
                            break;
                        case "Supplier":
                            if (data.SortObject.SortStatus == "ascending")
                            {

                                query = GetLstAssetDetails().Where(e => e.MasterAsset.BrandId == data.FilteredObj.BrandId).
                                  OrderBy(ww => ww.SupplierId);
                            }
                            else
                            {
                                query = GetLstAssetDetails().Where(e => e.MasterAsset.BrandId == data.FilteredObj.BrandId).
                          OrderByDescending(ww => ww.SupplierId);

                            }
                            break;
                        case "تاريخ الشراء":
                        case "Purchased Date":
                            if (data.SortObject.SortStatus == "ascending")
                            {
                                query = GetLstAssetDetails().Where(e => e.MasterAsset.BrandId == data.FilteredObj.BrandId).
                               OrderBy(ww => ww.PurchaseDate);
                            }
                            else
                            {
                                query = GetLstAssetDetails().Where(e => e.MasterAsset.BrandId == data.FilteredObj.BrandId).
                               OrderByDescending(ww => ww.PurchaseDate);
                            }
                            break;



                    }
                }

                mainClass.Count = query.Count();
                searchResult = query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();


            }


            if (data.FilteredObj.SupplierId != 0)
            {
                if (query is not null)
                {
                    switch (data.SortObject.SortBy)
                    {
                        case "الاسم":
                            if (data.SortObject.SortStatus == "ascending")
                            {
                                // filteration and sort then pagination 
                                // Where() , OrderBy() then Skip() Take()
                                query = query.Where(e => e.SupplierId == data.FilteredObj.SupplierId).
                                  OrderBy(ww => ww.MasterAsset.NameAr);
                            }
                            else
                            {
                                query = query.Where(e => e.SupplierId == data.FilteredObj.SupplierId).
                              OrderByDescending(ww => ww.MasterAsset.NameAr);
                            }
                            break;

                        case "Name":
                            if (data.SortObject.SortStatus == "ascending")
                            {
                                query = query.Where(e => e.SupplierId == data.FilteredObj.SupplierId).
                                OrderBy(ww => ww.MasterAsset.Name);

                            }
                            else
                            {

                                query = query.Where(e => e.SupplierId == data.FilteredObj.SupplierId).
                                    OrderByDescending(ww => ww.MasterAsset.Name);

                            }

                            break;

                        case "الباركود":
                        case "Barcode":

                            if (data.SortObject.SortStatus == "ascending")
                            {
                                query = query.Where(e => e.SupplierId == data.FilteredObj.SupplierId).
                                OrderBy(ww => ww.Barcode);


                            }
                            else
                            {
                                query = query.Where(e => e.SupplierId == data.FilteredObj.SupplierId).
                                  OrderByDescending(ww => ww.Barcode);

                            }

                            break;

                        case "السيريال":
                        case "Serial":
                            if (data.SortObject.SortStatus == "ascending")
                            {
                                query = query.Where(e => e.SupplierId == data.FilteredObj.SupplierId).
                                OrderBy(ww => ww.SerialNumber);

                            }
                            else
                            {
                                query = query.Where(e => e.SupplierId == data.FilteredObj.SupplierId).
                               OrderByDescending(ww => ww.SerialNumber);
                            }

                            break;

                        case "رقم الموديل":
                        case "Model Number":
                            if (data.SortObject.SortStatus == "ascending")
                            {
                                query = query.Where(e => e.SupplierId == data.FilteredObj.SupplierId).
                                     OrderBy(ww => ww.MasterAsset.ModelNumber);
                            }
                            else
                            {
                                query = query.Where(e => e.SupplierId == data.FilteredObj.SupplierId).
                                     OrderByDescending(ww => ww.MasterAsset.ModelNumber);
                            }
                            break;
                        case "القسم":
                            if (data.SortObject.SortStatus == "ascending")
                            {
                                query = query.Where(e => e.SupplierId == data.FilteredObj.SupplierId).
                                  OrderBy(ww => ww.DepartmentId);
                            }
                            else
                            {
                                query = query.Where(e => e.SupplierId == data.FilteredObj.SupplierId).
                                                OrderByDescending(ww => ww.DepartmentId);

                            }
                            break;


                        case "Department":
                            if (data.SortObject.SortStatus == "ascending")
                            {
                                query = query.Where(e => e.SupplierId == data.FilteredObj.SupplierId).
                                     OrderBy(ww => ww.DepartmentId);

                            }
                            else
                            {
                                query = query.Where(e => e.SupplierId == data.FilteredObj.SupplierId).
                                               OrderByDescending(ww => ww.DepartmentId);
                            }
                            break;

                        case "الماركة":
                            if (data.SortObject.SortStatus == "ascending")
                            {
                                query = query.Where(e => e.SupplierId == data.FilteredObj.SupplierId).
                                    OrderBy(ww => ww.MasterAsset.brand.NameAr);


                            }
                            else
                            {
                                query = query.Where(e => e.SupplierId == data.FilteredObj.SupplierId).
                                   OrderByDescending(ww => ww.MasterAsset.brand.NameAr);
                            }
                            break;
                        case "Manufacture":
                            if (data.SortObject.SortStatus == "ascending")

                            {


                                query = query.Where(e => e.SupplierId == data.FilteredObj.SupplierId).
                                  OrderBy(ww => ww.MasterAsset.brand.Name);


                            }
                            else
                            {
                                query = query.Where(e => e.SupplierId == data.FilteredObj.SupplierId).
                            OrderByDescending(ww => ww.MasterAsset.brand.Name);
                            }
                            break;
                        case "المورد":
                            if (data.SortObject.SortStatus == "ascending")
                            {

                                query = query.Where(e => e.SupplierId == data.FilteredObj.SupplierId).
                                  OrderBy(ww => ww.SupplierId);
                            }
                            else
                            {
                                query = query.Where(e => e.SupplierId == data.FilteredObj.SupplierId).
                                OrderByDescending(ww => ww.SupplierId);
                            }
                            break;
                        case "Supplier":
                            if (data.SortObject.SortStatus == "ascending")
                            {

                                query = query.Where(e => e.SupplierId == data.FilteredObj.SupplierId).
                                  OrderBy(ww => ww.SupplierId);
                            }
                            else
                            {
                                query = query.Where(e => e.SupplierId == data.FilteredObj.SupplierId).
                          OrderByDescending(ww => ww.SupplierId);

                            }
                            break;
                        case "تاريخ الشراء":
                        case "Purchased Date":
                            if (data.SortObject.SortStatus == "ascending")
                            {
                                query = query.Where(e => e.SupplierId == data.FilteredObj.SupplierId).
                               OrderBy(ww => ww.PurchaseDate);
                            }
                            else
                            {
                                query = query.Where(e => e.SupplierId == data.FilteredObj.SupplierId).
                               OrderByDescending(ww => ww.PurchaseDate);
                            }
                            break;



                    }

                }
                else
                {
                    switch (data.SortObject.SortBy)
                    {
                        case "الاسم":
                            if (data.SortObject.SortStatus == "ascending")
                            {
                                // filteration and sort then pagination 
                                // Where() , OrderBy() then Skip() Take()
                                query = GetLstAssetDetails().Where(e => e.SupplierId == data.FilteredObj.SupplierId).
                                  OrderBy(ww => ww.MasterAsset.NameAr);
                            }
                            else
                            {
                                query = GetLstAssetDetails().Where(e => e.SupplierId == data.FilteredObj.SupplierId).
                              OrderByDescending(ww => ww.MasterAsset.NameAr);
                            }
                            break;

                        case "Name":
                            if (data.SortObject.SortStatus == "ascending")
                            {
                                query = GetLstAssetDetails().Where(e => e.SupplierId == data.FilteredObj.SupplierId).
                                OrderBy(ww => ww.MasterAsset.Name);

                            }
                            else
                            {

                                query = GetLstAssetDetails().Where(e => e.SupplierId == data.FilteredObj.SupplierId).
                                    OrderByDescending(ww => ww.MasterAsset.Name);

                            }

                            break;

                        case "الباركود":
                        case "Barcode":

                            if (data.SortObject.SortStatus == "ascending")
                            {
                                query = GetLstAssetDetails().Where(e => e.SupplierId == data.FilteredObj.SupplierId).
                                OrderBy(ww => ww.Barcode);


                            }
                            else
                            {
                                query = GetLstAssetDetails().Where(e => e.SupplierId == data.FilteredObj.SupplierId).
                                  OrderByDescending(ww => ww.Barcode);

                            }

                            break;

                        case "السيريال":
                        case "Serial":
                            if (data.SortObject.SortStatus == "ascending")
                            {
                                query = GetLstAssetDetails().Where(e => e.SupplierId == data.FilteredObj.SupplierId).
                                OrderBy(ww => ww.SerialNumber);

                            }
                            else
                            {
                                query = GetLstAssetDetails().Where(e => e.SupplierId == data.FilteredObj.SupplierId).
                               OrderByDescending(ww => ww.SerialNumber);
                            }

                            break;

                        case "رقم الموديل":
                        case "Model Number":
                            if (data.SortObject.SortStatus == "ascending")
                            {
                                query = GetLstAssetDetails().Where(e => e.SupplierId == data.FilteredObj.SupplierId).
                                     OrderBy(ww => ww.MasterAsset.ModelNumber);
                            }
                            else
                            {
                                query = GetLstAssetDetails().Where(e => e.SupplierId == data.FilteredObj.SupplierId).
                                     OrderByDescending(ww => ww.MasterAsset.ModelNumber);
                            }
                            break;
                        case "القسم":
                            if (data.SortObject.SortStatus == "ascending")
                            {
                                query = GetLstAssetDetails().Where(e => e.SupplierId == data.FilteredObj.SupplierId).
                                  OrderBy(ww => ww.DepartmentId);
                            }
                            else
                            {
                                query = GetLstAssetDetails().Where(e => e.SupplierId == data.FilteredObj.SupplierId).
                                                OrderByDescending(ww => ww.DepartmentId);

                            }
                            break;


                        case "Department":
                            if (data.SortObject.SortStatus == "ascending")
                            {
                                query = GetLstAssetDetails().Where(e => e.SupplierId == data.FilteredObj.SupplierId).
                                     OrderBy(ww => ww.DepartmentId);

                            }
                            else
                            {
                                query = GetLstAssetDetails().Where(e => e.SupplierId == data.FilteredObj.SupplierId).
                                               OrderByDescending(ww => ww.DepartmentId);
                            }
                            break;

                        case "الماركة":
                            if (data.SortObject.SortStatus == "ascending")
                            {
                                query = GetLstAssetDetails().Where(e => e.SupplierId == data.FilteredObj.SupplierId).
                                    OrderBy(ww => ww.MasterAsset.brand.NameAr);


                            }
                            else
                            {
                                query = GetLstAssetDetails().Where(e => e.SupplierId == data.FilteredObj.SupplierId).
                                   OrderByDescending(ww => ww.MasterAsset.brand.NameAr);
                            }
                            break;
                        case "Manufacture":
                            if (data.SortObject.SortStatus == "ascending")

                            {


                                query = GetLstAssetDetails().Where(e => e.SupplierId == data.FilteredObj.SupplierId).
                                  OrderBy(ww => ww.MasterAsset.brand.Name);


                            }
                            else
                            {
                                query = GetLstAssetDetails().Where(e => e.SupplierId == data.FilteredObj.SupplierId).
                            OrderByDescending(ww => ww.MasterAsset.brand.Name);
                            }
                            break;
                        case "المورد":
                            if (data.SortObject.SortStatus == "ascending")
                            {

                                query = GetLstAssetDetails().Where(e => e.SupplierId == data.FilteredObj.SupplierId).
                                  OrderBy(ww => ww.SupplierId);
                            }
                            else
                            {
                                query = GetLstAssetDetails().Where(e => e.SupplierId == data.FilteredObj.SupplierId).
                                OrderByDescending(ww => ww.SupplierId);
                            }
                            break;
                        case "Supplier":
                            if (data.SortObject.SortStatus == "ascending")
                            {

                                query = GetLstAssetDetails().Where(e => e.SupplierId == data.FilteredObj.SupplierId).
                                  OrderBy(ww => ww.SupplierId);
                            }
                            else
                            {
                                query = GetLstAssetDetails().Where(e => e.SupplierId == data.FilteredObj.SupplierId).
                          OrderByDescending(ww => ww.SupplierId);

                            }
                            break;
                        case "تاريخ الشراء":
                        case "Purchased Date":
                            if (data.SortObject.SortStatus == "ascending")
                            {
                                query = GetLstAssetDetails().Where(e => e.SupplierId == data.FilteredObj.SupplierId).
                               OrderBy(ww => ww.PurchaseDate);
                            }
                            else
                            {
                                query = GetLstAssetDetails().Where(e => e.SupplierId == data.FilteredObj.SupplierId).
                               OrderByDescending(ww => ww.PurchaseDate);
                            }
                            break;



                    }

                }
                mainClass.Count = query.Count();
                searchResult = query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            }

            if (data.FilteredObj.MasterAssetId != 0)
            {
                if (query is not null)
                {
                    switch (data.SortObject.SortBy)
                    {
                        case "الاسم":
                            if (data.SortObject.SortStatus == "ascending")
                            {
                                // filteration and sort then pagination 
                                // Where() , OrderBy() then Skip() Take()
                                query = query.Where(e => e.MasterAssetId == data.FilteredObj.MasterAssetId).
                                  OrderBy(ww => ww.MasterAsset.NameAr);
                            }
                            else
                            {
                                query = query.Where(e => e.MasterAssetId == data.FilteredObj.MasterAssetId).
                              OrderByDescending(ww => ww.MasterAsset.NameAr);
                            }
                            break;

                        case "Name":
                            if (data.SortObject.SortStatus == "ascending")
                            {
                                query = query.Where(e => e.MasterAssetId == data.FilteredObj.MasterAssetId).
                                OrderBy(ww => ww.MasterAsset.Name);

                            }
                            else
                            {

                                query = query.Where(e => e.MasterAssetId == data.FilteredObj.MasterAssetId).
                                    OrderByDescending(ww => ww.MasterAsset.Name);

                            }

                            break;

                        case "الباركود":
                        case "Barcode":

                            if (data.SortObject.SortStatus == "ascending")
                            {
                                query = query.Where(e => e.MasterAssetId == data.FilteredObj.MasterAssetId).
                                OrderBy(ww => ww.Barcode);


                            }
                            else
                            {
                                query = query.Where(e => e.MasterAssetId == data.FilteredObj.MasterAssetId).
                                  OrderByDescending(ww => ww.Barcode);

                            }

                            break;

                        case "السيريال":
                        case "Serial":
                            if (data.SortObject.SortStatus == "ascending")
                            {
                                query = query.Where(e => e.MasterAssetId == data.FilteredObj.MasterAssetId).
                                OrderBy(ww => ww.SerialNumber);

                            }
                            else
                            {
                                query = query.Where(e => e.MasterAssetId == data.FilteredObj.MasterAssetId).
                               OrderByDescending(ww => ww.SerialNumber);
                            }

                            break;

                        case "رقم الموديل":
                        case "Model Number":
                            if (data.SortObject.SortStatus == "ascending")
                            {
                                query = query.Where(e => e.MasterAssetId == data.FilteredObj.MasterAssetId).
                                     OrderBy(ww => ww.MasterAsset.ModelNumber);
                            }
                            else
                            {
                                query = query.Where(e => e.MasterAssetId == data.FilteredObj.MasterAssetId).
                                     OrderByDescending(ww => ww.MasterAsset.ModelNumber);
                            }
                            break;
                        case "القسم":
                            if (data.SortObject.SortStatus == "ascending")
                            {
                                query = query.Where(e => e.MasterAssetId == data.FilteredObj.MasterAssetId).
                                  OrderBy(ww => ww.DepartmentId);
                            }
                            else
                            {
                                query = query.Where(e => e.MasterAssetId == data.FilteredObj.MasterAssetId).
                                                OrderByDescending(ww => ww.DepartmentId);

                            }
                            break;


                        case "Department":
                            if (data.SortObject.SortStatus == "ascending")
                            {
                                query = query.Where(e => e.MasterAssetId == data.FilteredObj.MasterAssetId).
                                     OrderBy(ww => ww.DepartmentId);

                            }
                            else
                            {
                                query = query.Where(e => e.MasterAssetId == data.FilteredObj.MasterAssetId).
                                               OrderByDescending(ww => ww.DepartmentId);
                            }
                            break;

                        case "الماركة":
                            if (data.SortObject.SortStatus == "ascending")
                            {
                                query = query.Where(e => e.MasterAssetId == data.FilteredObj.MasterAssetId).
                                    OrderBy(ww => ww.MasterAsset.brand.NameAr);


                            }
                            else
                            {
                                query = query.Where(e => e.MasterAssetId == data.FilteredObj.MasterAssetId).
                                   OrderByDescending(ww => ww.MasterAsset.brand.NameAr);
                            }
                            break;
                        case "Manufacture":
                            if (data.SortObject.SortStatus == "ascending")

                            {


                                query = query.Where(e => e.MasterAssetId == data.FilteredObj.MasterAssetId).
                                  OrderBy(ww => ww.MasterAsset.brand.Name);


                            }
                            else
                            {
                                query = query.Where(e => e.MasterAssetId == data.FilteredObj.MasterAssetId).
                            OrderByDescending(ww => ww.MasterAsset.brand.Name);
                            }
                            break;
                        case "المورد":
                            if (data.SortObject.SortStatus == "ascending")
                            {

                                query = query.Where(e => e.MasterAssetId == data.FilteredObj.MasterAssetId).
                                  OrderBy(ww => ww.SupplierId);
                            }
                            else
                            {
                                query = query.Where(e => e.MasterAssetId == data.FilteredObj.MasterAssetId).
                                OrderByDescending(ww => ww.SupplierId);
                            }
                            break;
                        case "Supplier":
                            if (data.SortObject.SortStatus == "ascending")
                            {

                                query = query.Where(e => e.MasterAssetId == data.FilteredObj.MasterAssetId).
                                  OrderBy(ww => ww.SupplierId);
                            }
                            else
                            {
                                query = query.Where(e => e.MasterAssetId == data.FilteredObj.MasterAssetId).
                          OrderByDescending(ww => ww.SupplierId);

                            }
                            break;
                        case "تاريخ الشراء":
                        case "Purchased Date":
                            if (data.SortObject.SortStatus == "ascending")
                            {
                                query = query.Where(e => e.MasterAssetId == data.FilteredObj.MasterAssetId).
                               OrderBy(ww => ww.PurchaseDate);
                            }
                            else
                            {
                                query = query.Where(e => e.MasterAssetId == data.FilteredObj.MasterAssetId).
                               OrderByDescending(ww => ww.PurchaseDate);
                            }
                            break;



                    }
                }
                else
                {
                    switch (data.SortObject.SortBy)
                    {
                        case "الاسم":
                            if (data.SortObject.SortStatus == "ascending")
                            {
                                // filteration and sort then pagination 
                                // Where() , OrderBy() then Skip() Take()
                                query = GetLstAssetDetails().Where(e => e.MasterAssetId == data.FilteredObj.MasterAssetId).
                                  OrderBy(ww => ww.MasterAsset.NameAr);
                            }
                            else
                            {
                                query = GetLstAssetDetails().Where(e => e.MasterAssetId == data.FilteredObj.MasterAssetId).
                              OrderByDescending(ww => ww.MasterAsset.NameAr);
                            }
                            break;

                        case "Name":
                            if (data.SortObject.SortStatus == "ascending")
                            {
                                query = GetLstAssetDetails().Where(e => e.MasterAssetId == data.FilteredObj.MasterAssetId).
                                OrderBy(ww => ww.MasterAsset.Name);

                            }
                            else
                            {

                                query = GetLstAssetDetails().Where(e => e.MasterAssetId == data.FilteredObj.MasterAssetId).
                                    OrderByDescending(ww => ww.MasterAsset.Name);

                            }

                            break;

                        case "الباركود":
                        case "Barcode":

                            if (data.SortObject.SortStatus == "ascending")
                            {
                                query = GetLstAssetDetails().Where(e => e.MasterAssetId == data.FilteredObj.MasterAssetId).
                                OrderBy(ww => ww.Barcode);


                            }
                            else
                            {
                                query = GetLstAssetDetails().Where(e => e.MasterAssetId == data.FilteredObj.MasterAssetId).
                                  OrderByDescending(ww => ww.Barcode);

                            }

                            break;

                        case "السيريال":
                        case "Serial":
                            if (data.SortObject.SortStatus == "ascending")
                            {
                                query = GetLstAssetDetails().Where(e => e.MasterAssetId == data.FilteredObj.MasterAssetId).
                                OrderBy(ww => ww.SerialNumber);

                            }
                            else
                            {
                                query = GetLstAssetDetails().Where(e => e.MasterAssetId == data.FilteredObj.MasterAssetId).
                               OrderByDescending(ww => ww.SerialNumber);
                            }

                            break;

                        case "رقم الموديل":
                        case "Model Number":
                            if (data.SortObject.SortStatus == "ascending")
                            {
                                query = GetLstAssetDetails().Where(e => e.MasterAssetId == data.FilteredObj.MasterAssetId).
                                     OrderBy(ww => ww.MasterAsset.ModelNumber);
                            }
                            else
                            {
                                query = GetLstAssetDetails().Where(e => e.MasterAssetId == data.FilteredObj.MasterAssetId).
                                     OrderByDescending(ww => ww.MasterAsset.ModelNumber);
                            }
                            break;
                        case "القسم":
                            if (data.SortObject.SortStatus == "ascending")
                            {
                                query = GetLstAssetDetails().Where(e => e.MasterAssetId == data.FilteredObj.MasterAssetId).
                                  OrderBy(ww => ww.DepartmentId);
                            }
                            else
                            {
                                query = GetLstAssetDetails().Where(e => e.MasterAssetId == data.FilteredObj.MasterAssetId).
                                                OrderByDescending(ww => ww.DepartmentId);

                            }
                            break;


                        case "Department":
                            if (data.SortObject.SortStatus == "ascending")
                            {
                                query = GetLstAssetDetails().Where(e => e.MasterAssetId == data.FilteredObj.MasterAssetId).
                                     OrderBy(ww => ww.DepartmentId);

                            }
                            else
                            {
                                query = GetLstAssetDetails().Where(e => e.MasterAssetId == data.FilteredObj.MasterAssetId).
                                               OrderByDescending(ww => ww.DepartmentId);
                            }
                            break;

                        case "الماركة":
                            if (data.SortObject.SortStatus == "ascending")
                            {
                                query = GetLstAssetDetails().Where(e => e.MasterAssetId == data.FilteredObj.MasterAssetId).
                                    OrderBy(ww => ww.MasterAsset.brand.NameAr);


                            }
                            else
                            {
                                query = GetLstAssetDetails().Where(e => e.MasterAssetId == data.FilteredObj.MasterAssetId).
                                   OrderByDescending(ww => ww.MasterAsset.brand.NameAr);
                            }
                            break;
                        case "Manufacture":
                            if (data.SortObject.SortStatus == "ascending")

                            {


                                query = GetLstAssetDetails().Where(e => e.MasterAssetId == data.FilteredObj.MasterAssetId).
                                  OrderBy(ww => ww.MasterAsset.brand.Name);


                            }
                            else
                            {
                                query = GetLstAssetDetails().Where(e => e.MasterAssetId == data.FilteredObj.MasterAssetId).
                            OrderByDescending(ww => ww.MasterAsset.brand.Name);
                            }
                            break;
                        case "المورد":
                            if (data.SortObject.SortStatus == "ascending")
                            {

                                query = GetLstAssetDetails().Where(e => e.MasterAssetId == data.FilteredObj.MasterAssetId).
                                  OrderBy(ww => ww.SupplierId);
                            }
                            else
                            {
                                query = GetLstAssetDetails().Where(e => e.MasterAssetId == data.FilteredObj.MasterAssetId).
                                OrderByDescending(ww => ww.SupplierId);
                            }
                            break;
                        case "Supplier":
                            if (data.SortObject.SortStatus == "ascending")
                            {

                                query = GetLstAssetDetails().Where(e => e.MasterAssetId == data.FilteredObj.MasterAssetId).
                                  OrderBy(ww => ww.SupplierId);
                            }
                            else
                            {
                                query = GetLstAssetDetails().Where(e => e.MasterAssetId == data.FilteredObj.MasterAssetId).
                          OrderByDescending(ww => ww.SupplierId);

                            }
                            break;
                        case "تاريخ الشراء":
                        case "Purchased Date":
                            if (data.SortObject.SortStatus == "ascending")
                            {
                                query = GetLstAssetDetails().Where(e => e.MasterAssetId == data.FilteredObj.MasterAssetId).
                               OrderBy(ww => ww.PurchaseDate);
                            }
                            else
                            {
                                query = GetLstAssetDetails().Where(e => e.MasterAssetId == data.FilteredObj.MasterAssetId).
                               OrderByDescending(ww => ww.PurchaseDate);
                            }
                            break;



                    }
                }
                mainClass.Count = query.Count();

                searchResult = query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            }


            if (data.FilteredObj.StatusId != 0)
            {
                if (query is not null)
                {
                    var q2 = query.ToList();
                    var lstOfIdsByStatus = _context.Set<AssetStatusTransaction>()
                        .FromSqlRaw("EXEC GetAssetStatusTransationsWithLastDate @AssetStatusId"
                        , new SqlParameter("@AssetStatusId", data.FilteredObj.StatusId)).ToList();
                    // make join between Lst Of Assets After Search and Lst Of AssetSByIts Status
                    // For Example List Of Assets After Search And  List Of Working Assets
                    var res = from q in q2
                              join lst in lstOfIdsByStatus on q.Id equals lst.AssetDetailId
                              select q;
                    mainClass.Count = res.Count();


                    switch (data.SortObject.SortBy)
                    {
                        case "الاسم":
                            if (data.SortObject.SortStatus == "ascending")
                            {
                                // filteration and sort then pagination 
                                // Where() , OrderBy() then Skip() Take()
                                res = res.OrderBy(ww => ww.MasterAsset.NameAr);
                            }
                            else
                            {
                                res = res.OrderByDescending(ww => ww.MasterAsset.NameAr);
                            }
                            break;

                        case "Name":
                            if (data.SortObject.SortStatus == "ascending")
                            {
                                res = res.OrderBy(ww => ww.MasterAsset.Name);

                            }
                            else
                            {

                                res = res.OrderByDescending(ww => ww.MasterAsset.Name);

                            }

                            break;

                        case "الباركود":
                        case "Barcode":

                            if (data.SortObject.SortStatus == "ascending")
                            {
                                res = res.OrderBy(ww => ww.Barcode);


                            }
                            else
                            {
                                res = res.OrderByDescending(ww => ww.Barcode);

                            }

                            break;

                        case "السيريال":
                        case "Serial":
                            if (data.SortObject.SortStatus == "ascending")
                            {
                                res = res.OrderBy(ww => ww.SerialNumber);

                            }
                            else
                            {
                                res = res.OrderByDescending(ww => ww.SerialNumber);
                            }

                            break;

                        case "رقم الموديل":
                        case "Model Number":
                            if (data.SortObject.SortStatus == "ascending")
                            {
                                res = res.OrderBy(ww => ww.MasterAsset.ModelNumber);
                            }
                            else
                            {
                                res = res.OrderByDescending(ww => ww.MasterAsset.ModelNumber);
                            }
                            break;
                        case "القسم":
                            if (data.SortObject.SortStatus == "ascending")
                            {
                                res = res.OrderBy(ww => ww.DepartmentId);
                            }
                            else
                            {
                                res = res.OrderByDescending(ww => ww.DepartmentId);

                            }
                            break;


                        case "Department":
                            if (data.SortObject.SortStatus == "ascending")
                            {
                                res = res.OrderBy(ww => ww.DepartmentId);

                            }
                            else
                            {
                                res = res.OrderByDescending(ww => ww.DepartmentId);
                            }
                            break;

                        case "الماركة":
                            if (data.SortObject.SortStatus == "ascending")
                            {
                                res = res.OrderBy(ww => ww.MasterAsset.brand.NameAr);


                            }
                            else
                            {
                                res = res.OrderByDescending(ww => ww.MasterAsset.brand.NameAr);
                            }
                            break;
                        case "Manufacture":
                            if (data.SortObject.SortStatus == "ascending")

                            {


                                res = res.OrderBy(ww => ww.MasterAsset.brand.Name);


                            }
                            else
                            {
                                res = res.OrderByDescending(ww => ww.MasterAsset.brand.Name);
                            }
                            break;
                        case "المورد":
                            if (data.SortObject.SortStatus == "ascending")
                            {

                                res = res.OrderBy(ww => ww.SupplierId);
                            }
                            else
                            {
                                res = res.OrderByDescending(ww => ww.SupplierId);
                            }
                            break;
                        case "Supplier":
                            if (data.SortObject.SortStatus == "ascending")
                            {
                                res = res.OrderBy(ww => ww.SupplierId);
                            }
                            else
                            {
                                res = res.OrderByDescending(ww => ww.SupplierId);

                            }
                            break;
                        case "تاريخ الشراء":
                        case "Purchased Date":
                            if (data.SortObject.SortStatus == "ascending")
                            {
                                res = res.OrderBy(ww => ww.PurchaseDate);
                            }
                            else
                            {
                                res = res.OrderByDescending(ww => ww.PurchaseDate);
                            }
                            break;



                    }



                    searchResult = res.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();



                }
                else
                {


                    var lstAssetsByStatus = _context.Set<AssetDetail>()
                    .FromSqlRaw("EXEC SP_GetAssetsByStatus @AssetStatusId"
                    , new SqlParameter("@AssetStatusId", data.FilteredObj.StatusId)).ToList();
                    mainClass.Count = lstAssetsByStatus.Count;

                    switch (data.SortObject.SortBy)
                    {
                        case "الاسم":
                            if (data.SortObject.SortStatus == "ascending")
                            {
                                // filteration and sort then pagination 
                                // Where() , OrderBy() then Skip() Take()
                                lstAssetsByStatus = lstAssetsByStatus.OrderBy(ww => ww.MasterAsset.NameAr).ToList();
                            }
                            else
                            {
                                lstAssetsByStatus = lstAssetsByStatus.OrderByDescending(ww => ww.MasterAsset.NameAr).ToList();
                            }
                            break;

                        case "Name":
                            if (data.SortObject.SortStatus == "ascending")
                            {
                                lstAssetsByStatus = lstAssetsByStatus.OrderBy(ww => ww.MasterAsset.Name).ToList();

                            }
                            else
                            {

                                lstAssetsByStatus = lstAssetsByStatus.OrderByDescending(ww => ww.MasterAsset.Name).ToList();

                            }

                            break;

                        case "الباركود":
                        case "Barcode":

                            if (data.SortObject.SortStatus == "ascending")
                            {
                                lstAssetsByStatus = lstAssetsByStatus.OrderBy(ww => ww.Barcode).ToList();


                            }
                            else
                            {
                                lstAssetsByStatus = lstAssetsByStatus.OrderByDescending(ww => ww.Barcode).ToList();

                            }

                            break;

                        case "السيريال":
                        case "Serial":
                            if (data.SortObject.SortStatus == "ascending")
                            {
                                lstAssetsByStatus = lstAssetsByStatus.OrderBy(ww => ww.SerialNumber).ToList();

                            }
                            else
                            {
                                lstAssetsByStatus = lstAssetsByStatus.OrderByDescending(ww => ww.SerialNumber).ToList();
                            }

                            break;

                        case "رقم الموديل":
                        case "Model Number":
                            if (data.SortObject.SortStatus == "ascending")
                            {
                                lstAssetsByStatus = lstAssetsByStatus.OrderBy(ww => ww.MasterAsset.ModelNumber).ToList();
                            }
                            else
                            {

                                lstAssetsByStatus = lstAssetsByStatus.OrderByDescending(ww => ww.MasterAsset.ModelNumber).ToList();

                            }
                            break;
                        case "القسم":
                            if (data.SortObject.SortStatus == "ascending")
                            {

                                lstAssetsByStatus = lstAssetsByStatus.OrderBy(ww => ww.DepartmentId).ToList();

                            }
                            else
                            {
                                lstAssetsByStatus = lstAssetsByStatus.OrderByDescending(ww => ww.DepartmentId).ToList();

                            }
                            break;


                        case "Department":
                            if (data.SortObject.SortStatus == "ascending")
                            {
                                lstAssetsByStatus = lstAssetsByStatus.OrderBy(ww => ww.DepartmentId).ToList();

                            }
                            else
                            {
                                lstAssetsByStatus = lstAssetsByStatus.OrderByDescending(ww => ww.DepartmentId).ToList();
                            }
                            break;

                        case "الماركة":
                            if (data.SortObject.SortStatus == "ascending")
                            {
                                lstAssetsByStatus = lstAssetsByStatus.OrderBy(ww => ww.MasterAsset.brand.NameAr).ToList();


                            }
                            else
                            {
                                lstAssetsByStatus = lstAssetsByStatus.OrderByDescending(ww => ww.MasterAsset.brand.NameAr).ToList();
                            }
                            break;
                        case "Manufacture":
                            if (data.SortObject.SortStatus == "ascending")

                            {


                                lstAssetsByStatus = lstAssetsByStatus.OrderBy(ww => ww.MasterAsset.brand.Name).ToList();

                            }
                            else
                            {
                                lstAssetsByStatus = lstAssetsByStatus.OrderByDescending(ww => ww.MasterAsset.brand.Name).ToList();
                            }
                            break;
                        case "المورد":
                            if (data.SortObject.SortStatus == "ascending")
                            {
                                lstAssetsByStatus = lstAssetsByStatus.OrderBy(ww => ww.SupplierId).ToList();


                            }
                            else
                            {
                                lstAssetsByStatus = lstAssetsByStatus.OrderByDescending(ww => ww.SupplierId).ToList();

                            }
                            break;
                        case "Supplier":
                            if (data.SortObject.SortStatus == "ascending")
                            {
                                lstAssetsByStatus = lstAssetsByStatus.OrderBy(ww => ww.SupplierId).ToList();

                            }
                            else
                            {

                                lstAssetsByStatus = lstAssetsByStatus.OrderByDescending(ww => ww.SupplierId).ToList();


                            }
                            break;
                        case "تاريخ الشراء":
                        case "Purchased Date":
                            if (data.SortObject.SortStatus == "ascending")
                            {
                                lstAssetsByStatus = lstAssetsByStatus.OrderBy(ww => ww.PurchaseDate).ToList();

                            }
                            else
                            {
                                lstAssetsByStatus = lstAssetsByStatus.OrderByDescending(ww => ww.PurchaseDate).ToList();

                            }
                            break;



                    }


                    lstAssetsByStatus = lstAssetsByStatus.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

                    foreach (var item in lstAssetsByStatus)
                    {

                        IndexAssetDetailVM.GetData getDataobj = new IndexAssetDetailVM.GetData();
                        getDataobj.Id = item.Id;
                        getDataobj.Code = item.Code;
                        getDataobj.Price = item.Price;
                        getDataobj.Barcode = item.Barcode;
                        getDataobj.Serial = item.SerialNumber;
                        getDataobj.SerialNumber = item.SerialNumber;
                        getDataobj.Model = _context.MasterAssets.Where(x => x.Id == item.MasterAssetId).FirstOrDefault().ModelNumber;
                        getDataobj.MasterAssetId = item.MasterAssetId;
                        getDataobj.PurchaseDate = item.PurchaseDate;
                        getDataobj.HospitalId = item.HospitalId;
                        getDataobj.DepartmentId = item.DepartmentId;
                        getDataobj.HospitalName = _context.Hospitals.Where(x => x.Id == item.HospitalId).FirstOrDefault().Name;
                        getDataobj.HospitalNameAr = item.Hospital.NameAr;
                        getDataobj.AssetName = item.MasterAsset.Name;
                        getDataobj.AssetNameAr = item.MasterAsset.NameAr;
                        if (item.HospitalId != 0)
                        {

                            getDataobj.GovernorateId = _context.Governorates.Where(x => x.Id == item.Hospital.GovernorateId).FirstOrDefault().Id;
                            getDataobj.GovernorateName = item.Hospital.Governorate.Name;
                            getDataobj.GovernorateName = item.Hospital.Governorate.NameAr;
                            getDataobj.CityId = _context.Cities.Where(x => x.Id == item.Hospital.CityId).FirstOrDefault().Id;
                            getDataobj.CityName = item.Hospital.City.Name;
                            getDataobj.CityNameAr = item.Hospital.City.NameAr;
                            getDataobj.OrganizationId = _context.Organizations.Where(x => x.Id == item.Hospital.OrganizationId).FirstOrDefault().Id;
                            getDataobj.OrgName = item.Hospital.Organization.Name;
                            getDataobj.OrgNameAr = item.Hospital.Organization.NameAr;
                            getDataobj.SubOrgName = _context.SubOrganizations.Where(x => x.Id == item.Hospital.SubOrganizationId).FirstOrDefault().Name;
                            getDataobj.SubOrgNameAr = item.Hospital.SubOrganization.NameAr;
                            getDataobj.SubOrganizationId = item.Hospital.SubOrganization.Id;
                        }
                        if (item.MasterAssetId != 0)
                        {
                            var masterObj = _context.MasterAssets.Where(x => x.Id == item.MasterAssetId).AsNoTracking().FirstOrDefault();
                            if (masterObj.BrandId != null || masterObj.BrandId != 0)
                            {
                                getDataobj.BrandId = _context.Brands.Where(x => x.Id == masterObj.BrandId).FirstOrDefault().Id;
                                getDataobj.BrandName = item.MasterAsset.brand.Name;
                                getDataobj.BrandNameAr = item.MasterAsset.brand.NameAr;
                            }
                        }
                        if (item.SupplierId != 0)
                        {
                            var supplierObj = _context.Suppliers.Where(x => x.Id == item.SupplierId).FirstOrDefault() ?? null;
                            if (supplierObj is not null)
                            {
                                getDataobj.SupplierId = supplierObj.Id;
                                getDataobj.SupplierName = supplierObj.Name;
                                getDataobj.SupplierNameAr = supplierObj.NameAr;

                            }
                            else
                            {
                                getDataobj.SupplierId = 0;
                                getDataobj.SupplierName = "";
                                getDataobj.SupplierNameAr = "";
                            }


                        }
                        if (item.DepartmentId != 0)
                        {
                            var deptObj = _context.Departments.Where(x => x.Id == item.DepartmentId).FirstOrDefault() ?? null;
                            if (deptObj is not null)
                            {
                                getDataobj.DepartmentName = deptObj.Name;
                                getDataobj.DepartmentNameAr = deptObj.NameAr;
                            }
                            else
                            {
                                getDataobj.DepartmentId = 0;
                                getDataobj.DepartmentName = "";
                            }

                        }


                        var lstTransactions = _context.AssetStatusTransactions.Where(a => a.AssetDetailId == item.Id).OrderByDescending(a => a.StatusDate.Value.Date).ToList();
                        if (lstTransactions.Count > 0)
                        {
                            getDataobj.AssetStatusId = lstTransactions[0].AssetStatusId;
                        }

                        list.Add(getDataobj);
                    }
                }
            }

            else
            {
                searchResult = searchResult.ToList();
            }
            string setstartday, setstartmonth, setendday, setendmonth = "";
            DateTime? startingFrom = new DateTime();
            DateTime? endingTo = new DateTime();
            if (data.FilteredObj.Start == "")
            {
                data.FilteredObj.purchaseDateFrom = DateTime.Parse("01/01/1900");
                startingFrom = DateTime.Parse("01/01/1900");
            }
            else
            {
                data.FilteredObj.purchaseDateFrom = DateTime.Parse(data.FilteredObj.Start.ToString());
                var startyear = data.FilteredObj.purchaseDateFrom.Value.Year;
                var startmonth = data.FilteredObj.purchaseDateFrom.Value.Month;
                var startday = data.FilteredObj.purchaseDateFrom.Value.Day;
                if (startday < 10)
                    setstartday = data.FilteredObj.purchaseDateFrom.Value.Day.ToString().PadLeft(2, '0');
                else
                    setstartday = data.FilteredObj.purchaseDateFrom.Value.Day.ToString();

                if (startmonth < 10)
                    setstartmonth = data.FilteredObj.purchaseDateFrom.Value.Month.ToString().PadLeft(2, '0');
                else
                    setstartmonth = data.FilteredObj.purchaseDateFrom.Value.Month.ToString();

                var sDate = startyear + "-" + setstartmonth + "-" + setstartday;
                startingFrom = DateTime.Parse(sDate);//.AddDays(1);
            }

            if (data.FilteredObj.End == "")
            {
                data.FilteredObj.purchaseDateTo = DateTime.Today.Date;
                endingTo = DateTime.Today.Date;
            }
            else
            {
                data.FilteredObj.purchaseDateTo = DateTime.Parse(data.FilteredObj.End.ToString());
                var endyear = data.FilteredObj.purchaseDateTo.Value.Year;
                var endmonth = data.FilteredObj.purchaseDateTo.Value.Month;
                var endday = data.FilteredObj.purchaseDateTo.Value.Day;
                if (endday < 10)
                    setendday = data.FilteredObj.purchaseDateTo.Value.Day.ToString().PadLeft(2, '0');
                else
                    setendday = data.FilteredObj.purchaseDateTo.Value.Day.ToString();
                if (endmonth < 10)
                    setendmonth = data.FilteredObj.purchaseDateTo.Value.Month.ToString().PadLeft(2, '0');
                else
                    setendmonth = data.FilteredObj.purchaseDateTo.Value.Month.ToString();
                var eDate = endyear + "-" + setendmonth + "-" + setendday;
                endingTo = DateTime.Parse(eDate);
            }
            if (startingFrom != null || endingTo != null)
            {
                foreach (var item in list)
                {
                    if (item.PurchaseDate != null)
                    {
                        list = list.Where(a => a.PurchaseDate.Value.Date >= startingFrom.Value.Date && a.PurchaseDate.Value.Date <= endingTo.Value.Date).ToList();

                    }

                }

            }
            else
            {
                list = list.ToList();
            }




            if (list.Count == 0 || list is null)
            {
                if (searchResult.Count > 0)
                {
                    foreach (var item in searchResult)
                    {
                        IndexAssetDetailVM.GetData getDataobj = new IndexAssetDetailVM.GetData();
                        getDataobj.Id = item.Id;
                        getDataobj.Code = item.Code;
                        getDataobj.Price = item.Price;
                        getDataobj.Barcode = item.Barcode;
                        getDataobj.Serial = item.SerialNumber;
                        getDataobj.SerialNumber = item.SerialNumber;
                        getDataobj.BarCode = item.Barcode;
                        getDataobj.Model = item.MasterAsset.ModelNumber;
                        getDataobj.MasterAssetId = item.MasterAssetId;
                        getDataobj.PurchaseDate = item.PurchaseDate;
                        getDataobj.HospitalId = item.HospitalId;
                        getDataobj.DepartmentId = item.DepartmentId;
                        getDataobj.HospitalName = item.Hospital.Name;
                        getDataobj.HospitalNameAr = item.Hospital.NameAr;
                        getDataobj.AssetName = item.MasterAsset.Name;
                        getDataobj.AssetNameAr = item.MasterAsset.NameAr;
                        getDataobj.GovernorateId = item.Hospital.Governorate.Id;
                        getDataobj.GovernorateName = item.Hospital.Governorate.Name;
                        getDataobj.GovernorateNameAr = item.Hospital.Governorate.NameAr;
                        getDataobj.CityId = item.Hospital.City.Id;
                        getDataobj.CityName = item.Hospital.City.Name;
                        getDataobj.CityNameAr = item.Hospital.City.NameAr;
                        getDataobj.OrganizationId = item.Hospital.Organization.Id;
                        getDataobj.OrgName = item.Hospital.Organization.Name;
                        getDataobj.OrgNameAr = item.Hospital.Organization.NameAr;
                        getDataobj.SubOrgName = item.Hospital.SubOrganization.Name;
                        getDataobj.SubOrgNameAr = item.Hospital.SubOrganization.NameAr;
                        getDataobj.SubOrganizationId = item.Hospital.SubOrganization.Id;
                        getDataobj.BrandId = item.MasterAsset.brand != null ? item.MasterAsset.brand.Id : 0;
                        getDataobj.BrandName = item.MasterAsset.brand != null ? item.MasterAsset.brand.Name : "";
                        getDataobj.BrandNameAr = item.MasterAsset.brand != null ? item.MasterAsset.brand.NameAr : "";
                        getDataobj.SupplierId = item.Supplier != null ? item.Supplier.Id : 0;
                        getDataobj.SupplierName = item.Supplier != null ? item.Supplier.Name : "";
                        getDataobj.SupplierNameAr = item.Supplier != null ? item.Supplier.NameAr : "";
                        getDataobj.DepartmentName = item.Department != null ? item.Department.Name : "";
                        getDataobj.DepartmentNameAr = item.Department != null ? item.Department.NameAr : "";

                        var lstTransactions = _context.AssetStatusTransactions.Where(a => a.AssetDetailId == item.Id).OrderByDescending(a => a.StatusDate.Value.Date).ToList();
                        if (lstTransactions.Count > 0)
                        {
                            getDataobj.AssetStatusId = lstTransactions[0].AssetStatusId;
                        }

                        list.Add(getDataobj);
                    }

                }
            }


            mainClass.Results = list;

            return mainClass;



        }

        public IndexAssetDetailVM GetAssetsByHospitalIdWithPaging(int hospitalId, int pageNumber, int pageSize)
        {
            IndexAssetDetailVM mainClass = new IndexAssetDetailVM();
            var result = new List<IndexAssetDetailVM.GetData>();

            var list = GetLstAssetDetails2();

            if (hospitalId > 0)
            {
                list = list.Where(a => a.HospitalId == hospitalId);
                mainClass.Count = list.Count();
            }
            else
            {
                mainClass.Count = list.Count();
            }


            if (pageNumber == 0 && pageSize == 0)
                list = list;
            else
                list = list.Skip((pageNumber - 1) * pageSize).Take(pageSize);



            foreach (var item in list)
            {
                IndexAssetDetailVM.GetData assetObject = new IndexAssetDetailVM.GetData();
                assetObject.Id = item.Id;
                assetObject.Code = item.Code;
                assetObject.Price = item.Price;
                assetObject.Barcode = item.Barcode;
                assetObject.Serial = item.SerialNumber;
                assetObject.SerialNumber = item.SerialNumber;
                assetObject.BarCode = item.Barcode;
                assetObject.Model = item.MasterAsset.ModelNumber;
                assetObject.MasterAssetId = item.MasterAssetId;
                assetObject.PurchaseDate = item.PurchaseDate;
                assetObject.HospitalId = item.HospitalId;
                assetObject.DepartmentId = item.DepartmentId;
                assetObject.HospitalName = item.Hospital.Name;
                assetObject.HospitalNameAr = item.Hospital.NameAr;
                assetObject.AssetName = item.MasterAsset.Name;
                assetObject.AssetNameAr = item.MasterAsset.NameAr;
                assetObject.GovernorateId = item.Hospital.Governorate.Id;
                assetObject.GovernorateName = item.Hospital.Governorate.Name;
                assetObject.GovernorateNameAr = item.Hospital.Governorate.NameAr;
                assetObject.CityId = item.Hospital.City.Id;
                assetObject.CityName = item.Hospital.City.Name;
                assetObject.CityNameAr = item.Hospital.City.NameAr;
                assetObject.OrganizationId = item.Hospital.Organization.Id;
                assetObject.OrgName = item.Hospital.Organization.Name;
                assetObject.OrgNameAr = item.Hospital.Organization.NameAr;
                assetObject.SubOrgName = item.Hospital.SubOrganization.Name;
                assetObject.SubOrgNameAr = item.Hospital.SubOrganization.NameAr;
                assetObject.SubOrganizationId = item.Hospital.SubOrganization.Id;
                assetObject.BrandId = item.MasterAsset.brand != null ? item.MasterAsset.brand.Id : 0;
                assetObject.BrandName = item.MasterAsset.brand != null ? item.MasterAsset.brand.Name : "";
                assetObject.BrandNameAr = item.MasterAsset.brand != null ? item.MasterAsset.brand.NameAr : "";
                assetObject.SupplierId = item.Supplier != null ? item.Supplier.Id : 0;
                assetObject.SupplierName = item.Supplier != null ? item.Supplier.Name : "";
                assetObject.SupplierNameAr = item.Supplier != null ? item.Supplier.NameAr : "";
                assetObject.DepartmentName = item.Department != null ? item.Department.Name : "";
                assetObject.DepartmentNameAr = item.Department != null ? item.Department.NameAr : "";
                assetObject.Count = list.Count();

                var lstTransactions = _context.AssetStatusTransactions.Where(a => a.AssetDetailId == item.Id).OrderByDescending(a => a.StatusDate.Value.Date).ToList();
                if (lstTransactions.Count > 0)
                {
                    assetObject.AssetStatusId = lstTransactions[0].AssetStatusId;
                }

                result.Add(assetObject);
            }

            mainClass.Results = result;
            return mainClass;
        }

        public IndexAssetDetailVM GetAssetsByHospitalIdWithPaging(int hospitalId)
        {
            IndexAssetDetailVM mainClass = new IndexAssetDetailVM();
            var result = new List<IndexAssetDetailVM.GetData>();
            var list = GetLstAssetDetails2();
            if (hospitalId > 0)
            {
                list = list.Where(a => a.HospitalId == hospitalId);
                mainClass.Count = list.Count();

            }
            else
            {
                list = list;
                mainClass.Count = list.Count();

            }

            foreach (var item in list)
            {
                IndexAssetDetailVM.GetData assetObject = new IndexAssetDetailVM.GetData();
                assetObject.Id = item.Id;
                assetObject.Code = item.Code;
                assetObject.Price = item.Price;
                assetObject.Barcode = item.Barcode;
                assetObject.Serial = item.SerialNumber;
                assetObject.SerialNumber = item.SerialNumber;
                assetObject.BarCode = item.Barcode;
                assetObject.Model = item.MasterAsset.ModelNumber;
                assetObject.HospitalName = item.Hospital.Name;
                assetObject.HospitalNameAr = item.Hospital.NameAr;
                assetObject.AssetName = item.MasterAsset.Name;
                assetObject.AssetNameAr = item.MasterAsset.NameAr;
                assetObject.CityId = item.Hospital.City.Id;
                assetObject.CityName = item.Hospital.City.Name;
                assetObject.CityNameAr = item.Hospital.City.NameAr;
                assetObject.OrgName = item.Hospital.Organization.Name;
                assetObject.OrgNameAr = item.Hospital.Organization.NameAr;
                assetObject.SubOrgName = item.Hospital.SubOrganization.Name;
                assetObject.SubOrgNameAr = item.Hospital.SubOrganization.NameAr;
                assetObject.BrandName = item.MasterAsset.brand != null ? item.MasterAsset.brand.Name : "";
                assetObject.BrandNameAr = item.MasterAsset.brand != null ? item.MasterAsset.brand.NameAr : "";
                assetObject.SupplierId = item.Supplier != null ? item.Supplier.Id : 0;
                assetObject.SupplierName = item.Supplier != null ? item.Supplier.Name : "";
                assetObject.SupplierNameAr = item.Supplier != null ? item.Supplier.NameAr : "";
                assetObject.DepartmentName = item.Department != null ? item.Department.Name : "";
                assetObject.DepartmentNameAr = item.Department != null ? item.Department.NameAr : "";

                var lstTransactions = _context.AssetStatusTransactions.Where(a => a.AssetDetailId == item.Id).OrderByDescending(a => a.StatusDate.Value.Date).ToList();
                if (lstTransactions.Count > 0)
                {
                    assetObject.AssetStatusId = lstTransactions[0].AssetStatusId;
                }

                result.Add(assetObject);
            }

            mainClass.Results = result;
            return mainClass;
        }
    }
}

