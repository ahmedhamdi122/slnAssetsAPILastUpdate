﻿using Asset.Domain.Repositories;
using Asset.Models;
using Asset.ViewModels.PMAssetTaskVM;
using Asset.ViewModels.WorkOrderTrackingVM;
using Asset.ViewModels.WorkOrderVM;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset.Core.Repositories
{
    public class WorkOrderRepository : IWorkOrderRepository
    {
        private ApplicationDbContext _context;
        string msg;
        public WorkOrderRepository(ApplicationDbContext context)
        {
            _context = context;
        }


        public IEnumerable<IndexWorkOrderVM> ExportWorkOrdersByStatusId(int? hospitalId, string userId, int statusId)
        {
            List<IndexWorkOrderVM> list = new List<IndexWorkOrderVM>();
            List<IndexWorkOrderVM> listEngineer = new List<IndexWorkOrderVM>();
            ApplicationUser UserObj = new ApplicationUser();
            List<string> userRoleNames = new List<string>();

            var obj = _context.ApplicationUser.Where(a => a.Id == userId).ToList();
            if (obj.Count > 0)
            {
                UserObj = obj[0];
                var roles = (from userRole in _context.UserRoles
                             join role in _context.ApplicationRole on userRole.RoleId equals role.Id
                             where userRole.UserId == userId
                             select role);
                foreach (var role in roles)
                {
                    userRoleNames.Add(role.Name);
                }
            }


            var lstWorkOrders = _context.WorkOrders
                      .Include(w => w.WorkOrderType)
                      .Include(w => w.WorkOrderPeriority)
                      .Include(w => w.Request)
                      .Include(w => w.Request.AssetDetail)
                      .Include(w => w.Request.AssetDetail.MasterAsset)
                      .Include(w => w.Request.AssetDetail.MasterAsset.brand)
                      .Include(w => w.Request.AssetDetail.Department)
                      .Include(w => w.User).ToList();
            foreach (var item in lstWorkOrders)
            {
                IndexWorkOrderVM work = new IndexWorkOrderVM();
                work.Id = item.Id;
                work.WorkOrderNumber = item.WorkOrderNumber;
                work.Subject = item.Subject;
                work.ModelNumber = item.Request.AssetDetail.MasterAsset != null ? item.Request.AssetDetail.MasterAsset.ModelNumber : "";
                work.RequestSubject = item.Request.Subject;
                work.CreationDate = item.CreationDate;
                work.Note = item.Note;
                work.BarCode = item.Request.AssetDetail != null ? item.Request.AssetDetail.Barcode : "";
                work.CreatedById = item.CreatedById;
                work.CreatedBy = item.User.UserName;
                work.TypeName = item.WorkOrderType.Name;
                work.TypeNameAr = item.WorkOrderType.NameAr;
                work.AssetName = item.Request.AssetDetail.MasterAsset.Name;
                work.AssetNameAr = item.Request.AssetDetail.MasterAsset.NameAr;
                work.SerialNumber = item.Request.AssetDetail.SerialNumber;
                work.PeriorityName = item.WorkOrderPeriority != null ? item.WorkOrderPeriority.Name : "";
                work.PeriorityNameAr = item.WorkOrderPeriority != null ? item.WorkOrderPeriority.NameAr : "";
                var lstStatus = _context.WorkOrderTrackings
                       .Include(t => t.WorkOrder).Include(t => t.WorkOrderStatus)
                       .Where(a => a.WorkOrderId == item.Id).ToList().OrderByDescending(a => a.WorkOrderDate).ToList();
                if (lstStatus.Count > 0)
                {
                    work.AssignedTo = lstStatus[0].AssignedTo;
                    work.CreatedById = lstStatus[0].CreatedById;
                    work.WorkOrderStatusId = lstStatus[0].WorkOrderStatus.Id;
                    work.StatusName = lstStatus[0].WorkOrderStatus.Name;
                    work.StatusNameAr = lstStatus[0].WorkOrderStatus.NameAr;
                    work.statusColor = lstStatus[0].WorkOrderStatus.Color;
                    work.statusIcon = lstStatus[0].WorkOrderStatus.Icon;
                }

                work.ActualStartDate = item.ActualStartDate;
                work.ActualEndDate = item.ActualEndDate;
                work.RequestId = item.RequestId != null ? (int)item.RequestId : 0;
                work.HospitalId = item.Request.AssetDetail.HospitalId;
                if (item.Request.AssetDetail.MasterAsset.brand != null)
                {
                    work.BrandName = item.Request.AssetDetail.MasterAsset.brand.Name;
                    work.BrandNameAr = item.Request.AssetDetail.MasterAsset.brand.NameAr;
                }

                if (item.Request.AssetDetail.Department != null)
                {
                    work.DepartmentName = item.Request.AssetDetail.Department.Name;
                    work.DepartmentNameAr = item.Request.AssetDetail.Department.NameAr;
                }
                var lstClosedDate = _context.WorkOrderTrackings
                      .Include(t => t.WorkOrder).Include(t => t.WorkOrderStatus)
                      .Where(a => a.WorkOrderId == item.Id && a.WorkOrderStatusId == 12).ToList().OrderByDescending(a => a.WorkOrderDate).ToList().GroupBy(a => item.Id).ToList();
                if (lstClosedDate.Count > 0)
                {
                    work.ClosedDate = lstClosedDate[0].FirstOrDefault().CreationDate;
                }
                work.ListTracks = _context.WorkOrderTrackings.Include(a => a.WorkOrderStatus).Where(a => a.WorkOrderId == work.Id)
                .ToList().Select(item => new LstWorkOrderFromTracking
                {
                    Id = item.Id,
                    StatusName = item.WorkOrderStatus.Name,
                    StatusNameAr = item.WorkOrderStatus.NameAr,
                    CreationDate = DateTime.Parse(item.CreationDate.ToString())
                }).ToList();

                list.Add(work);
            }
            if (userRoleNames.Contains("EngDepManager"))
            {
                list = list.Where(a => a.HospitalId == UserObj.HospitalId).ToList();
            }
            if (statusId > 0)
                list = list.Where(a => a.WorkOrderStatusId == statusId).ToList();
            else
                list = list.ToList();

            return list;
        }

        public IEnumerable<IndexWorkOrderVM> GetAllWorkOrdersByHospitalId(int? hospitalId, string userId, int statusId)
        {
            List<IndexWorkOrderVM> list = new List<IndexWorkOrderVM>();
            List<IndexWorkOrderVM> listEngineer = new List<IndexWorkOrderVM>();
            ApplicationUser UserObj = new ApplicationUser();
            List<string> userRoleNames = new List<string>();

            var obj = _context.ApplicationUser.Where(a => a.Id == userId).ToList();
            if (obj.Count > 0)
            {
                UserObj = obj[0];
                var roles = (from userRole in _context.UserRoles
                             join role in _context.ApplicationRole on userRole.RoleId equals role.Id
                             where userRole.UserId == userId
                             select role);
                foreach (var role in roles)
                {
                    userRoleNames.Add(role.Name);
                }
            }


            var lstWorkOrders = _context.WorkOrders
                      .Include(w => w.WorkOrderType)
                      .Include(w => w.WorkOrderPeriority)
                      .Include(w => w.Request)
                      .Include(w => w.Request.AssetDetail)
                      .Include(w => w.Request.AssetDetail.MasterAsset)
                      .Include(w => w.User).ToList();
            foreach (var item in lstWorkOrders)
            {
                IndexWorkOrderVM work = new IndexWorkOrderVM();
                work.Id = item.Id;
                work.WorkOrderNumber = item.WorkOrderNumber;
                work.Subject = item.Subject;
                work.ModelNumber = item.Request.AssetDetail.MasterAsset != null ? item.Request.AssetDetail.MasterAsset.ModelNumber : "";
                work.RequestSubject = item.Request.Subject;
                work.CreationDate = item.CreationDate;
                work.Note = item.Note;
                work.BarCode = item.Request.AssetDetail != null ? item.Request.AssetDetail.Barcode : "";
                work.CreatedById = item.CreatedById;
                work.CreatedBy = item.User.UserName;
                work.TypeName = item.WorkOrderType.Name;
                work.TypeNameAr = item.WorkOrderType.NameAr;
                work.AssetName = item.Request.AssetDetail.MasterAsset.Name;
                work.AssetNameAr = item.Request.AssetDetail.MasterAsset.NameAr;
                work.SerialNumber = item.Request.AssetDetail.SerialNumber;
                work.PeriorityName = item.WorkOrderPeriority != null ? item.WorkOrderPeriority.Name : "";
                work.PeriorityNameAr = item.WorkOrderPeriority != null ? item.WorkOrderPeriority.NameAr : "";



                var lstStatus = _context.WorkOrderTrackings
                       .Include(t => t.WorkOrder).Include(t => t.WorkOrderStatus)
                       .Where(a => a.WorkOrderId == item.Id).ToList().OrderByDescending(a => a.WorkOrderDate).ToList();
                if (lstStatus.Count > 0)
                {
                    work.AssignedTo = lstStatus[0].AssignedTo;
                    work.CreatedById = lstStatus[0].CreatedById;
                    work.WorkOrderStatusId = lstStatus[0].WorkOrderStatus.Id;
                    //if (work.WorkOrderStatusId == 3 || work.WorkOrderStatusId == 4 || work.WorkOrderStatusId == 5)
                    //{
                    //    var pendingStatus = _context.WorkOrderStatuses.Where(a => a.Id == 6).ToList().FirstOrDefault();
                    //    work.StatusName = lstStatus[0].WorkOrderStatus.Name + " - " + pendingStatus.Name;
                    //    work.StatusNameAr = lstStatus[0].WorkOrderStatus.NameAr + " - " + pendingStatus.NameAr;
                    //    work.statusColor = lstStatus[0].WorkOrderStatus.Color;
                    //    work.statusIcon = lstStatus[0].WorkOrderStatus.Icon;
                    //}

                    //else
                    //{
                    work.StatusName = lstStatus[0].WorkOrderStatus.Name;
                    work.StatusNameAr = lstStatus[0].WorkOrderStatus.NameAr;
                    work.statusColor = lstStatus[0].WorkOrderStatus.Color;
                    work.statusIcon = lstStatus[0].WorkOrderStatus.Icon;
                    // }
                }

                work.ActualStartDate = item.ActualStartDate;
                work.ActualEndDate = item.ActualEndDate;
                work.RequestId = item.RequestId != null ? (int)item.RequestId : 0;
                work.HospitalId = item.Request.AssetDetail.HospitalId;

                var lstClosedDate = _context.WorkOrderTrackings
                      .Include(t => t.WorkOrder).Include(t => t.WorkOrderStatus)
                      .Where(a => a.WorkOrderId == item.Id && a.WorkOrderStatusId == 12).ToList().OrderByDescending(a => a.WorkOrderDate).ToList().GroupBy(a => item.Id).ToList();
                if (lstClosedDate.Count > 0)
                {
                    work.ClosedDate = lstClosedDate[0].FirstOrDefault().CreationDate;
                }
                work.ListTracks = _context.WorkOrderTrackings.Include(a => a.WorkOrderStatus).Where(a => a.WorkOrderId == work.Id)
                .ToList().Select(item => new LstWorkOrderFromTracking
                {
                    Id = item.Id,
                    StatusName = item.WorkOrderStatus.Name,
                    StatusNameAr = item.WorkOrderStatus.NameAr,
                    CreationDate = DateTime.Parse(item.CreationDate.ToString())
                }).ToList();

                list.Add(work);
            }
            //if (userRoleNames.Contains("EngDepManager") && !userRoleNames.Contains("Eng"))
            //{
            //    list = list.Where(t => t.HospitalId == UserObj.HospitalId).ToList();
            //}
            //if (userRoleNames.Contains("Eng") && !userRoleNames.Contains("EngDepManager"))
            //{
            //    List<IndexWorkOrderVM> listAssignedUsers = new List<IndexWorkOrderVM>();
            //    var lstAssigned = (from wo in _context.WorkOrders
            //                       join trk in _context.WorkOrderTrackings on wo.Id equals trk.WorkOrderId
            //                       where trk.AssignedTo == userId
            //                       select wo).ToList().GroupBy(a => a.Id).ToList();

            //    foreach (var item in lstAssigned)
            //    {
            //        IndexWorkOrderVM work = new IndexWorkOrderVM();
            //        work.Id = item.FirstOrDefault().Id;
            //        work.WorkOrderNumber = item.FirstOrDefault().WorkOrderNumber;
            //        work.BarCode = item.FirstOrDefault().Request.AssetDetail.Barcode;
            //        work.Subject = item.FirstOrDefault().Subject;
            //        work.RequestSubject = item.FirstOrDefault().Request.Subject;
            //        work.CreationDate = item.FirstOrDefault().CreationDate;
            //        work.Note = item.FirstOrDefault().Note;
            //        work.CreatedById = item.FirstOrDefault().CreatedById;
            //        work.CreatedBy = _context.WorkOrderTrackings.Where(a => a.WorkOrderId == item.FirstOrDefault().Id).ToList().FirstOrDefault().User.UserName;
            //        work.TypeName = _context.WorkOrderTypes.Where(a => a.Id == item.FirstOrDefault().WorkOrderTypeId).ToList().FirstOrDefault().Name;
            //        work.TypeNameAr = _context.WorkOrderTypes.Where(a => a.Id == item.FirstOrDefault().WorkOrderTypeId).ToList().FirstOrDefault().NameAr;
            //        work.WorkOrderPeriorityId = _context.WorkOrderPeriorities.Where(a => a.Id == item.FirstOrDefault().WorkOrderPeriorityId).ToList().FirstOrDefault().Id;
            //        work.PeriorityName = _context.WorkOrderPeriorities.Where(a => a.Id == item.FirstOrDefault().WorkOrderPeriorityId).ToList().FirstOrDefault().Name;
            //        work.PeriorityNameAr = _context.WorkOrderPeriorities.Where(a => a.Id == item.FirstOrDefault().WorkOrderPeriorityId).ToList().FirstOrDefault().NameAr;
            //        var lstStatus = _context.WorkOrderTrackings
            //               .Include(t => t.WorkOrder).Include(t => t.WorkOrderStatus)
            //               .Where(a => a.WorkOrderId == item.FirstOrDefault().Id).ToList().OrderByDescending(a => a.WorkOrderDate).ToList();
            //        if (lstStatus.Count > 0)
            //        {
            //            work.StatusId = lstStatus[0].WorkOrderStatus.Id;
            //            work.WorkOrderStatusId = lstStatus[0].WorkOrderStatus.Id;
            //            work.StatusName = lstStatus[0].WorkOrderStatus.Name;
            //            work.StatusNameAr = lstStatus[0].WorkOrderStatus.NameAr;
            //            work.statusColor = lstStatus[0].WorkOrderStatus.Color;
            //            work.statusIcon = lstStatus[0].WorkOrderStatus.Icon;

            //        }

            //        var lstStatusIds = _context.WorkOrderTrackings
            //              .Where(a => a.WorkOrderId == item.FirstOrDefault().Id).ToList().Select(a => a.WorkOrderStatusId).ToList();



            //        var exist = lstStatusIds.Contains(9);
            //        work.ExistStatusId = exist;

            //        work.ActualStartDate = item.FirstOrDefault().ActualStartDate;
            //        work.ActualEndDate = item.FirstOrDefault().ActualEndDate;
            //        //work.RequestId = item.FirstOrDefault().RequestId;
            //        work.RequestId = item.FirstOrDefault().RequestId != null ? (int)item.FirstOrDefault().RequestId : 0;
            //        work.HospitalId = item.FirstOrDefault().Request.AssetDetail.HospitalId;
            //        work.AssignedTo = _context.WorkOrderTrackings.Where(a => a.WorkOrderId == item.FirstOrDefault().Id).ToList().FirstOrDefault().AssignedTo;
            //        listAssignedUsers.Add(work);
            //    }



            //    List<IndexWorkOrderVM> listCreatedByUser = new List<IndexWorkOrderVM>();
            //    var lstcreated = (from wo in _context.WorkOrders
            //                      join trk in _context.WorkOrderTrackings on wo.Id equals trk.WorkOrderId
            //                      where trk.CreatedById == userId
            //                      select wo).ToList().GroupBy(a => a.Id).ToList();

            //    foreach (var item in lstcreated)
            //    {
            //        IndexWorkOrderVM work = new IndexWorkOrderVM();
            //        work.Id = item.FirstOrDefault().Id;
            //        work.WorkOrderNumber = item.FirstOrDefault().WorkOrderNumber;
            //        work.Subject = item.FirstOrDefault().Subject;
            //        work.BarCode = item.FirstOrDefault().Request.AssetDetail.Barcode;
            //        work.RequestSubject = _context.Request.Where(a => a.Id == item.FirstOrDefault().RequestId).ToList().FirstOrDefault().Subject;
            //        work.CreationDate = item.FirstOrDefault().CreationDate;
            //        work.Note = item.FirstOrDefault().Note;
            //        work.CreatedById = item.FirstOrDefault().CreatedById;
            //        work.CreatedBy = _context.WorkOrderTrackings.Where(a => a.WorkOrderId == item.FirstOrDefault().Id).ToList().FirstOrDefault().User.UserName;
            //        work.TypeName = _context.WorkOrderTypes.Where(a => a.Id == item.FirstOrDefault().WorkOrderTypeId).ToList().FirstOrDefault().Name;
            //        work.TypeNameAr = _context.WorkOrderTypes.Where(a => a.Id == item.FirstOrDefault().WorkOrderTypeId).ToList().FirstOrDefault().NameAr;
            //        work.PeriorityName = _context.WorkOrderPeriorities.Where(a => a.Id == item.FirstOrDefault().WorkOrderPeriorityId).ToList().FirstOrDefault().Name;
            //        work.PeriorityNameAr = _context.WorkOrderPeriorities.Where(a => a.Id == item.FirstOrDefault().WorkOrderPeriorityId).ToList().FirstOrDefault().NameAr;
            //        var lstStatus = _context.WorkOrderTrackings
            //               .Include(t => t.WorkOrder).Include(t => t.WorkOrderStatus)
            //               .Where(a => a.WorkOrderId == item.FirstOrDefault().Id).ToList().OrderByDescending(a => a.WorkOrderDate).ToList();
            //        if (lstStatus.Count > 0)
            //        {
            //            work.StatusName = lstStatus[0].WorkOrderStatus.Name;
            //            work.StatusNameAr = lstStatus[0].WorkOrderStatus.NameAr;
            //            work.WorkOrderStatusId = lstStatus[0].WorkOrderStatus.Id;
            //            work.statusColor = lstStatus[0].WorkOrderStatus.Color;
            //            work.statusIcon = lstStatus[0].WorkOrderStatus.Icon;

            //        }

            //        work.ActualStartDate = item.FirstOrDefault().ActualStartDate;
            //        work.ActualEndDate = item.FirstOrDefault().ActualEndDate;
            //        //   work.RequestId = item.FirstOrDefault().RequestId;
            //        work.RequestId = item.FirstOrDefault().RequestId != null ? (int)item.FirstOrDefault().RequestId : 0;
            //        work.HospitalId = item.FirstOrDefault().Request.AssetDetail.HospitalId;
            //        work.AssignedTo = _context.WorkOrderTrackings.Where(a => a.WorkOrderId == item.FirstOrDefault().Id).ToList().FirstOrDefault().AssignedTo;
            //        listAssignedUsers.Add(work);
            //    }

            //    list = listAssignedUsers.Where(a => a.HospitalId == UserObj.HospitalId).Concat(listCreatedByUser.Where(a => a.HospitalId == UserObj.HospitalId)).ToList();
            //}

            //if (userRoleNames.Contains("Eng") && userRoleNames.Contains("EngDepManager"))
            //{
            //    list = list.Where(a => a.CreatedById == userId && a.AssignedTo == userId).ToList();
            //}

            if (userRoleNames.Contains("EngDepManager"))
            {
                list = list.Where(a => a.HospitalId == UserObj.HospitalId).ToList();
            }


            if (statusId > 0)
                list = list.Where(a => a.WorkOrderStatusId == statusId).ToList();
            else
                list = list.ToList();

            return list;
        }

        public int GetTotalWorkOrdersForAssetInHospital(int assetDetailId)
        {
            var lstRequestsByAsset = _context.WorkOrders
                                   .Include(a => a.Request)
                                   .Include(t => t.Request.AssetDetail).Where(a => a.Request.AssetDetailId == assetDetailId).ToList();
            if (lstRequestsByAsset.Count > 0)
            {
                return lstRequestsByAsset.Count;
            }

            return 0;
        }

        public IEnumerable<IndexWorkOrderVM> GetworkOrder(string userId)
        {
            List<IndexWorkOrderVM> list = new List<IndexWorkOrderVM>();

            ApplicationUser UserObj = new ApplicationUser();
            ApplicationRole roleObj = new ApplicationRole();
            string userRoleName = "";
            var obj = _context.ApplicationUser.Where(a => a.Id == userId).ToList();
            if (obj.Count > 0)
            {
                UserObj = obj[0];

                var lstRoles = _context.ApplicationRole.Where(a => a.Id == UserObj.RoleId).ToList();
                if (lstRoles.Count > 0)
                {
                    roleObj = lstRoles[0];
                    userRoleName = roleObj.Name;
                }
            }

            var lstWorkOrders = _context.WorkOrderTrackings
                                .Include(w => w.WorkOrder)
                                .Include(w => w.WorkOrder.Request)
                                .Include(w => w.WorkOrder.WorkOrderType)
                                .Include(w => w.WorkOrder.WorkOrderPeriority)
                                .Include(w => w.WorkOrder.User)
                                .Include(w => w.AssignedToUser)

                 .Select(wo => new IndexWorkOrderVM
                 {
                     Id = wo.WorkOrder.Id,
                     Subject = wo.WorkOrder.Subject,

                     AssetName = wo.WorkOrder.Request.AssetDetail.MasterAsset.Name,
                     AssetNameAr = wo.WorkOrder.Request.AssetDetail.MasterAsset.NameAr,

                     BarCode = wo.WorkOrder.Request.AssetDetail.Barcode,
                     SerialNumber = wo.WorkOrder.Request.AssetDetail.SerialNumber,
                     ModelNumber = wo.WorkOrder.Request.AssetDetail.MasterAsset.ModelNumber,
                     UserName = wo.User.UserName,
                     AssignedTo = wo.AssignedToUser.UserName,
                     StatusName = wo.WorkOrderStatus.Name,
                     StatusNameAr = wo.WorkOrderStatus.NameAr,
                     WorkOrderNumber = wo.WorkOrder.WorkOrderNumber,
                     CreationDate = wo.WorkOrder.CreationDate,
                     PlannedStartDate = wo.WorkOrder.PlannedStartDate,
                     PlannedEndDate = wo.WorkOrder.PlannedEndDate,
                     ActualStartDate = wo.WorkOrder.ActualStartDate,
                     ActualEndDate = wo.WorkOrder.ActualEndDate,
                     Note = wo.WorkOrder.Note,
                     WorkOrderTrackingId = wo.Id,
                     MasterAssetId = wo.WorkOrder.Request.AssetDetail.MasterAssetId,
                     CreatedById = wo.User.Id,
                     CreatedBy = wo.User.UserName,
                     //  WorkOrderPeriorityId = wo.WorkOrder.WorkOrderPeriorityId,
                     WorkOrderPeriorityId = wo.WorkOrder.WorkOrderPeriorityId != null ? (int)wo.WorkOrder.WorkOrderPeriorityId : 0,
                     WorkOrderPeriorityName = wo.WorkOrder.WorkOrderPeriority.Name,
                     // WorkOrderTypeId = wo.WorkOrder.WorkOrderTypeId,
                     WorkOrderTypeId = wo.WorkOrder.WorkOrderTypeId != null ? (int)wo.WorkOrder.WorkOrderTypeId : 0,
                     WorkOrderTypeName = wo.WorkOrder.WorkOrderType.Name,
                     //RequestId = wo.WorkOrder.RequestId,
                     RequestId = wo.WorkOrder.RequestId != null ? (int)wo.WorkOrder.RequestId : 0,
                     RequestSubject = wo.WorkOrder.Request.Subject,
                     HospitalId = wo.User.HospitalId,
                     GovernorateId = wo.User.GovernorateId,
                     CityId = wo.User.CityId,
                     OrganizationId = wo.User.OrganizationId,
                     SubOrganizationId = wo.User.SubOrganizationId,
                     RoleId = wo.User.RoleId,

                 }).OrderByDescending(o => o.CreationDate).ToList();






            foreach (var order in lstWorkOrders)
            {
                IndexWorkOrderVM getDataObj = new IndexWorkOrderVM();
                getDataObj.Id = order.Id;
                getDataObj.Subject = order.Subject;
                getDataObj.BarCode = order.BarCode;
                getDataObj.ModelNumber = order.ModelNumber;
                getDataObj.AssetName = order.AssetName;
                getDataObj.AssetNameAr = order.AssetNameAr;
                getDataObj.SerialNumber = order.SerialNumber;
                getDataObj.WorkOrderNumber = order.WorkOrderNumber;
                getDataObj.WorkOrderTypeName = order.WorkOrderTypeName;
                getDataObj.StatusName = order.StatusName;
                getDataObj.UserName = order.UserName;
                getDataObj.AssignedTo = order.AssignedTo;
                getDataObj.CreatedById = order.CreatedById;
                getDataObj.CreatedBy = order.CreatedBy;
                getDataObj.UserName = order.UserName;
                getDataObj.ActualStartDate = order.ActualStartDate;
                getDataObj.ActualEndDate = order.ActualEndDate;
                getDataObj.PlannedStartDate = order.PlannedStartDate;
                getDataObj.PlannedEndDate = order.PlannedEndDate;
                getDataObj.WorkOrderPeriorityId = order.WorkOrderPeriorityId;
                getDataObj.WorkOrderPeriorityName = order.WorkOrderPeriorityName;
                getDataObj.HospitalId = order.HospitalId;
                getDataObj.GovernorateId = order.GovernorateId;
                getDataObj.CityId = order.CityId;
                getDataObj.OrganizationId = order.OrganizationId;
                getDataObj.SubOrganizationId = order.SubOrganizationId;
                getDataObj.CreatedById = order.CreatedById;
                getDataObj.CreationDate = order.CreationDate;
                getDataObj.AssetName = _context.MasterAssets.Where(a => a.Id == order.MasterAssetId).ToList().FirstOrDefault().Name;
                getDataObj.AssetNameAr = _context.MasterAssets.Where(a => a.Id == order.MasterAssetId).ToList().FirstOrDefault().NameAr;
                getDataObj.WorkOrderTrackingId = order.WorkOrderTrackingId;
                //  getDataObj.ListTracks = _context.orderuestTracking.Where(a => a.orderuestId == order.Id)
                //          .ToList().Select(item => new IndexorderuestTrackingVM.GetData
                //          {
                //              Id = item.Id,
                //              StatusName = _context.orderuestStatus.Where(a => a.Id == item.orderuestStatusId).First().StatusName,
                //              Description = item.Description,
                //              Date = item.DescriptionDate,
                //              StatusId = item.orderuestStatusId,
                //              isExpanded = (_context.orderuestDocument.Where(a => a.orderuestTrackingId == item.Id).Count()) > 0 ? true : false,
                //              ListDocuments = _context.orderuestDocument.Where(a => a.orderuestTrackingId == item.Id).ToList(),
                //          }).ToList();
                list.Add(getDataObj);
            }

            if (UserObj.GovernorateId > 0 && UserObj.CityId == 0 && UserObj.HospitalId == 0)
            {
                list = list.Where(t => t.GovernorateId == UserObj.GovernorateId).ToList();
            }
            if (UserObj.GovernorateId > 0 && UserObj.CityId > 0 && UserObj.HospitalId == 0)
            {
                list = list.Where(t => t.CityId == UserObj.CityId).ToList();
            }
            if (UserObj.OrganizationId > 0 && UserObj.SubOrganizationId == 0 && UserObj.HospitalId == 0)
            {
                list = list.Where(t => t.OrganizationId == UserObj.OrganizationId).ToList();
            }
            if (UserObj.OrganizationId > 0 && UserObj.SubOrganizationId > 0 && UserObj.HospitalId == 0)
            {
                list = list.Where(t => t.SubOrganizationId == UserObj.SubOrganizationId).ToList();
            }

            if (UserObj.OrganizationId > 0 && UserObj.SubOrganizationId > 0 && UserObj.HospitalId > 0)
            {
                if (userRoleName == "Admin")
                {
                    list = list.ToList();
                }

                if (userRoleName == "TLHospitalManager")
                {
                    list = list.Where(t => t.HospitalId == UserObj.HospitalId).ToList();
                }

                if (userRoleName == "EngDepManager")
                {
                    list = list.Where(t => t.HospitalId == UserObj.HospitalId).ToList();
                }

                if (userRoleName == "AssetOwner")
                {
                    list = list = new List<IndexWorkOrderVM>();
                }
                if (userRoleName == "Eng")
                {
                    var lstAssigned = list.Where(t => t.HospitalId == UserObj.HospitalId && t.AssignedTo == userId).ToList();
                    var lstCreatedItems = list.Where(t => t.HospitalId == UserObj.HospitalId && t.CreatedById == userId).ToList();
                    list = lstAssigned.Concat(lstCreatedItems).ToList();
                }

                if (userRoleName == "DE")
                {
                    list = new List<IndexWorkOrderVM>();
                }
                if (userRoleName == "HR")
                {
                    list = new List<IndexWorkOrderVM>();
                }
            }
            if (UserObj.GovernorateId > 0 && UserObj.CityId > 0 && UserObj.HospitalId > 0)
            {
                if (userRoleName == "Admin")
                {
                    list = list.Where(t => t.HospitalId == UserObj.HospitalId).ToList();
                }

                if (userRoleName == "Admin")
                {
                    list = list.ToList();
                }
                if (userRoleName == "TLHospitalManager")
                {
                    list = list.Where(t => t.HospitalId == UserObj.HospitalId).ToList();
                }
                if (userRoleName == "EngManager")
                {
                    list = list.Where(t => t.HospitalId == UserObj.HospitalId).ToList();
                }

                if (userRoleName == "EngDepManager")
                {
                    list = list.Where(t => t.HospitalId == UserObj.HospitalId).ToList();
                }

                if (userRoleName == "AssetOwner")
                {
                    list = list = new List<IndexWorkOrderVM>();
                }

                if (userRoleName == "Eng")
                {
                    var lstAssigned = list.Where(t => t.HospitalId == UserObj.HospitalId && t.AssignedTo == userId).ToList();
                    var lstCreatedItems = list.Where(t => t.HospitalId == UserObj.HospitalId && t.CreatedById == userId).ToList();
                    list = lstAssigned.Concat(lstCreatedItems).ToList();
                }

                if (userRoleName == "DE")
                {
                    list = list = new List<IndexWorkOrderVM>();
                }

                if (userRoleName == "HR")
                {
                    list = list = new List<IndexWorkOrderVM>();
                }

            }
            return list;
        }

        public IndexWorkOrderVM GetWorkOrderByRequestId(int requestId)
        {
            return _context.WorkOrders.Where(a => a.RequestId == requestId)
                            .Include(w => w.Request)
                            .Include(w => w.WorkOrderType)

                            .Include(w => w.WorkOrderPeriority)
                            .Include(w => w.User)
                   .Select(wo => new IndexWorkOrderVM
                   {
                       Id = wo.Id,
                       Subject = wo.Subject,
                       BarCode = wo.Request.AssetDetail.Barcode,
                       ModelNumber = wo.Request.AssetDetail.MasterAsset.ModelNumber,
                       AssetName = wo.Request.AssetDetail.MasterAsset.Name,
                       AssetNameAr = wo.Request.AssetDetail.MasterAsset.NameAr,
                       SerialNumber = wo.Request.AssetDetail.SerialNumber,
                       WorkOrderNumber = wo.WorkOrderNumber,
                       CreationDate = wo.CreationDate,
                       PlannedStartDate = wo.PlannedStartDate,
                       PlannedEndDate = wo.PlannedEndDate,
                       ActualStartDate = wo.ActualStartDate,
                       ActualEndDate = wo.ActualEndDate,
                       Note = wo.Note,
                       CreatedById = wo.CreatedById,
                       CreatedBy = wo.User.UserName,
                       // WorkOrderPeriorityId = wo.WorkOrderPeriorityId,
                       WorkOrderPeriorityId = wo.WorkOrderPeriorityId != null ? (int)wo.WorkOrderPeriorityId : 0,
                       WorkOrderPeriorityName = wo.WorkOrderPeriority.Name,
                       PeriorityName = wo.WorkOrderPeriority.Name,
                       PeriorityNameAr = wo.WorkOrderPeriority.NameAr,
                       // WorkOrderTypeId = wo.WorkOrderTypeId,
                       WorkOrderTypeId = wo.WorkOrderTypeId != null ? (int)wo.WorkOrderTypeId : 0,
                       TypeName = wo.WorkOrderType.Name,
                       TypeNameAr = wo.WorkOrderType.NameAr,
                       StatusName = _context.WorkOrderTrackings.Where(t => t.WorkOrderId == wo.Id).FirstOrDefault().WorkOrderStatus.Name,
                       StatusNameAr = _context.WorkOrderTrackings.Where(t => t.WorkOrderId == wo.Id).FirstOrDefault().WorkOrderStatus.NameAr,
                       // RequestId = wo.RequestId,
                       RequestId = wo.RequestId != null ? (int)wo.RequestId : 0,
                       WorkOrderTrackingId = _context.WorkOrderTrackings.Where(t => t.WorkOrderId == wo.Id).FirstOrDefault().Id,
                       WorkOrderStatusId = _context.WorkOrderTrackings.Where(t => t.WorkOrderId == wo.Id).FirstOrDefault().WorkOrderStatusId
                   }).FirstOrDefault();
        }

        public IEnumerable<IndexWorkOrderVM> GetworkOrderByUserId(int requestId, string userId)
        {
            List<IndexWorkOrderVM> list = new List<IndexWorkOrderVM>();

            ApplicationUser UserObj = new ApplicationUser();
            ApplicationRole roleObj = new ApplicationRole();
            string userRoleName = "";
            var obj = _context.ApplicationUser.Where(a => a.Id == userId).ToList();
            if (obj.Count > 0)
            {
                UserObj = obj[0];

                var lstRoles = _context.ApplicationRole.Where(a => a.Id == UserObj.RoleId).ToList();
                if (lstRoles.Count > 0)
                {
                    roleObj = lstRoles[0];
                    userRoleName = roleObj.Name;
                }
            }

            var lstWorkOrders = _context.WorkOrderTrackings
                                .Include(w => w.WorkOrder)
                                .Include(w => w.WorkOrder.Request)
                                .Include(w => w.WorkOrder.WorkOrderType)
                                .Include(w => w.WorkOrder.WorkOrderPeriority)
                                .Include(w => w.WorkOrder.User)
                                .Include(w => w.AssignedToUser)

                 .Select(wo => new IndexWorkOrderVM
                 {
                     Id = wo.WorkOrder.Id,
                     Subject = wo.WorkOrder.Subject,
                     SerialNumber = wo.WorkOrder.Request.AssetDetail.SerialNumber,
                     BarCode = wo.WorkOrder.Request.AssetDetail.Barcode,
                     ModelNumber = wo.WorkOrder.Request.AssetDetail.MasterAsset.ModelNumber,
                     AssetName = wo.WorkOrder.Request.AssetDetail.MasterAsset.Name,
                     AssetNameAr = wo.WorkOrder.Request.AssetDetail.MasterAsset.NameAr,
                     UserName = wo.User.UserName,
                     AssignedTo = wo.AssignedToUser.UserName,
                     StatusName = wo.WorkOrderStatus.Name,
                     StatusNameAr = wo.WorkOrderStatus.NameAr,
                     WorkOrderNumber = wo.WorkOrder.WorkOrderNumber,
                     CreationDate = wo.WorkOrder.CreationDate,
                     PlannedStartDate = wo.WorkOrder.PlannedStartDate,
                     PlannedEndDate = wo.WorkOrder.PlannedEndDate,
                     ActualStartDate = wo.WorkOrder.ActualStartDate,
                     ActualEndDate = wo.WorkOrder.ActualEndDate,
                     Note = wo.WorkOrder.Note,
                     WorkOrderTrackingId = wo.Id,  //_context.WorkOrderTrackings.Where(w=>w.WorkOrderId== wo.WorkOrder.Id).FirstOrDefault().Id,

                     MasterAssetId = wo.WorkOrder.Request.AssetDetail.MasterAssetId,
                     CreatedById = wo.User.Id,
                     CreatedBy = wo.User.UserName,
                     //WorkOrderPeriorityId = wo.WorkOrder.WorkOrderPeriorityId,
                     WorkOrderPeriorityId = wo.WorkOrder.WorkOrderPeriorityId != null ? (int)wo.WorkOrder.WorkOrderPeriorityId : 0,
                     WorkOrderPeriorityName = wo.WorkOrder.WorkOrderPeriority.Name,
                     //    WorkOrderTypeId = wo.WorkOrder.WorkOrderTypeId,
                     WorkOrderTypeId = wo.WorkOrder.WorkOrderTypeId != null ? (int)wo.WorkOrder.WorkOrderTypeId : 0,
                     WorkOrderTypeName = wo.WorkOrder.WorkOrderType.Name,
                     //  RequestId = wo.WorkOrder.RequestId,
                     RequestId = wo.WorkOrder.RequestId != null ? (int)wo.WorkOrder.RequestId : 0,
                     RequestSubject = wo.WorkOrder.Request.Subject,
                     HospitalId = wo.User.HospitalId,
                     GovernorateId = wo.User.GovernorateId,
                     CityId = wo.User.CityId,
                     OrganizationId = wo.User.OrganizationId,
                     SubOrganizationId = wo.User.SubOrganizationId,
                     RoleId = wo.User.RoleId,

                 }).OrderByDescending(o => o.CreationDate).Where(a => a.RequestId == requestId).ToList();






            foreach (var order in lstWorkOrders)
            {
                IndexWorkOrderVM getDataObj = new IndexWorkOrderVM();
                getDataObj.Id = order.Id;
                getDataObj.Subject = order.Subject;
                getDataObj.SerialNumber = order.SerialNumber;
                getDataObj.ModelNumber = order.ModelNumber;
                getDataObj.BarCode = order.BarCode;
                getDataObj.AssetName = order.AssetName;
                getDataObj.AssetNameAr = order.AssetNameAr;
                getDataObj.WorkOrderNumber = order.WorkOrderNumber;
                getDataObj.WorkOrderTypeName = order.WorkOrderTypeName;
                getDataObj.StatusName = order.StatusName;
                getDataObj.UserName = order.UserName;
                getDataObj.AssignedTo = order.AssignedTo;
                getDataObj.CreatedById = order.CreatedById;
                getDataObj.CreatedBy = order.CreatedBy;
                getDataObj.UserName = order.UserName;
                getDataObj.ActualStartDate = order.ActualStartDate;
                getDataObj.ActualEndDate = order.ActualEndDate;
                getDataObj.PlannedStartDate = order.PlannedStartDate;
                getDataObj.PlannedEndDate = order.PlannedEndDate;
                getDataObj.WorkOrderPeriorityId = order.WorkOrderPeriorityId;
                getDataObj.WorkOrderPeriorityName = order.WorkOrderPeriorityName;
                getDataObj.HospitalId = order.HospitalId;
                getDataObj.GovernorateId = order.GovernorateId;
                getDataObj.CityId = order.CityId;
                getDataObj.OrganizationId = order.OrganizationId;
                getDataObj.SubOrganizationId = order.SubOrganizationId;
                getDataObj.CreatedById = order.CreatedById;
                getDataObj.CreationDate = order.CreationDate;
                getDataObj.AssetName = _context.MasterAssets.Where(a => a.Id == order.MasterAssetId).ToList().FirstOrDefault().Name;
                getDataObj.AssetNameAr = _context.MasterAssets.Where(a => a.Id == order.MasterAssetId).ToList().FirstOrDefault().NameAr;
                getDataObj.WorkOrderTrackingId = order.WorkOrderTrackingId;



                //getDataObj.ListTracks = _context.orderuestTracking.Where(a => a.orderuestId == order.Id)
                //        .ToList().Select(item => new IndexorderuestTrackingVM.GetData
                //        {
                //            Id = item.Id,
                //            StatusName = _context.WorkOrderStatuses.Where(a => a.Id == item.orderuestStatusId).First().StatusName,
                //            Description = item.Description,
                //            Date = item.DescriptionDate,
                //            StatusId = item.orderuestStatusId,
                //            ListDocuments = _context.orderuestDocument.Where(a => a.orderuestTrackingId == item.Id).ToList(),
                //        }).ToList();
                list.Add(getDataObj);
            }

            if (UserObj.GovernorateId > 0 && UserObj.CityId == 0 && UserObj.HospitalId == 0)
            {
                list = list.Where(t => t.GovernorateId == UserObj.GovernorateId).ToList();
            }
            if (UserObj.GovernorateId > 0 && UserObj.CityId > 0 && UserObj.HospitalId == 0)
            {
                list = list.Where(t => t.CityId == UserObj.CityId).ToList();
            }
            if (UserObj.OrganizationId > 0 && UserObj.SubOrganizationId == 0 && UserObj.HospitalId == 0)
            {
                list = list.Where(t => t.OrganizationId == UserObj.OrganizationId).ToList();
            }
            if (UserObj.OrganizationId > 0 && UserObj.SubOrganizationId > 0 && UserObj.HospitalId == 0)
            {
                list = list.Where(t => t.SubOrganizationId == UserObj.SubOrganizationId).ToList();
            }

            if (UserObj.OrganizationId > 0 && UserObj.SubOrganizationId > 0 && UserObj.HospitalId > 0)
            {

                if (userRoleName == "TLHospitalManager")
                {
                    list = list.Where(t => t.HospitalId == UserObj.HospitalId).ToList();
                }

                if (userRoleName == "EngDepManager")
                {
                    list = list.Where(t => t.HospitalId == UserObj.HospitalId).ToList();
                }

                if (userRoleName == "Eng")
                {
                    var lstAssigned = list.Where(t => t.HospitalId == UserObj.HospitalId && t.AssignedTo == userId).ToList();
                    var lstCreatedItems = list.Where(t => t.HospitalId == UserObj.HospitalId && t.CreatedById == userId).ToList();
                    list = lstAssigned.Concat(lstCreatedItems).ToList();
                }

                if (userRoleName == "DE")
                {
                    list = new List<IndexWorkOrderVM>();
                }
                if (userRoleName == "HR")
                {
                    list = new List<IndexWorkOrderVM>();
                }
            }
            if (UserObj.GovernorateId > 0 && UserObj.CityId > 0 && UserObj.HospitalId > 0)
            {
                if (userRoleName == "Admin")
                {
                    list = list.ToList();
                }
                if (userRoleName == "TLHospitalManager")
                {
                    list = list.Where(t => t.HospitalId == UserObj.HospitalId).ToList();
                }
                if (userRoleName == "EngManager")
                {
                    list = list.Where(t => t.HospitalId == UserObj.HospitalId).ToList();
                }

                if (userRoleName == "EngDepManager")
                {
                    list = list.Where(t => t.HospitalId == UserObj.HospitalId).ToList();
                }

                if (userRoleName == "Eng")
                {
                    var lstAssigned = list.Where(t => t.HospitalId == UserObj.HospitalId && t.AssignedTo == userId).ToList();
                    var lstCreatedItems = list.Where(t => t.HospitalId == UserObj.HospitalId && t.CreatedById == userId).ToList();
                    list = lstAssigned.Concat(lstCreatedItems).ToList();
                }

                if (userRoleName == "DE")
                {
                    list = list = new List<IndexWorkOrderVM>();
                }

                if (userRoleName == "HR")
                {
                    list = list = new List<IndexWorkOrderVM>();
                }

            }
            return list;
        }

        public IEnumerable<IndexWorkOrderVM> GetworkOrderByUserAssetId(int assetId, string userId)
        {
            List<IndexWorkOrderVM> list = new List<IndexWorkOrderVM>();

            ApplicationUser UserObj = new ApplicationUser();
            ApplicationRole roleObj = new ApplicationRole();
            string userRoleName = "";
            var obj = _context.ApplicationUser.Where(a => a.Id == userId).ToList();
            if (obj.Count > 0)
            {
                UserObj = obj[0];

                var lstRoles = _context.ApplicationRole.Where(a => a.Id == UserObj.RoleId).ToList();
                if (lstRoles.Count > 0)
                {
                    roleObj = lstRoles[0];
                    userRoleName = roleObj.Name;
                }
            }

            //var lstWorkOrders = _context.WorkOrderTrackings
            //                    .Include(w => w.WorkOrder)
            //                    .Include(w => w.WorkOrder.Request)
            //                    .Include(w => w.WorkOrder.WorkOrderType)
            //                    .Include(w => w.WorkOrder.WorkOrderPeriority)
            //                    .Include(w => w.WorkOrder.User)
            //                    .Include(w => w.AssignedToUser)
            //                    .Where(a => a.WorkOrder.Request.AssetDetailId == assetId)
            //     .Select(wo => new IndexWorkOrderVM
            //     {
            //         Id = wo.WorkOrder.Id,
            //         Subject = wo.WorkOrder.Subject,
            //         SerialNumber = wo.WorkOrder.Request.AssetDetail.SerialNumber,
            //         BarCode = wo.WorkOrder.Request.AssetDetail.Barcode,
            //         ModelNumber = wo.WorkOrder.Request.AssetDetail.MasterAsset.ModelNumber,
            //         AssetName = wo.WorkOrder.Request.AssetDetail.MasterAsset.Name,
            //         AssetNameAr = wo.WorkOrder.Request.AssetDetail.MasterAsset.NameAr,
            //         UserName = wo.User.UserName,
            //         AssignedTo = wo.AssignedToUser.UserName,
            //         StatusName = wo.WorkOrderStatus.Name,
            //         StatusNameAr = wo.WorkOrderStatus.NameAr,
            //         WorkOrderNumber = wo.WorkOrder.WorkOrderNumber,
            //         CreationDate = wo.WorkOrder.CreationDate,
            //         PlannedStartDate = wo.WorkOrder.PlannedStartDate,
            //         PlannedEndDate = wo.WorkOrder.PlannedEndDate,
            //         ActualStartDate = wo.WorkOrder.ActualStartDate,
            //         ActualEndDate = wo.WorkOrder.ActualEndDate,
            //         Note = wo.WorkOrder.Note,
            //         WorkOrderTrackingId = wo.Id,  //_context.WorkOrderTrackings.Where(w=>w.WorkOrderId== wo.WorkOrder.Id).FirstOrDefault().Id,

            //         MasterAssetId = wo.WorkOrder.Request.AssetDetail.MasterAssetId,
            //         CreatedById = wo.User.Id,
            //         CreatedBy = wo.User.UserName,
            //         WorkOrderPeriorityId = wo.WorkOrder.WorkOrderPeriorityId != null ? (int)wo.WorkOrder.WorkOrderPeriorityId : 0,
            //         WorkOrderPeriorityName = wo.WorkOrder.WorkOrderPeriority.Name,
            //         WorkOrderTypeId = wo.WorkOrder.WorkOrderTypeId != null ? (int)wo.WorkOrder.WorkOrderTypeId : 0,
            //         WorkOrderTypeName = wo.WorkOrder.WorkOrderType.Name,
            //         RequestId = wo.WorkOrder.RequestId != null ? (int)wo.WorkOrder.RequestId : 0,
            //         RequestSubject = wo.WorkOrder.Request.Subject,
            //         HospitalId = wo.User.HospitalId,
            //         GovernorateId = wo.User.GovernorateId,
            //         CityId = wo.User.CityId,
            //         OrganizationId = wo.User.OrganizationId,
            //         SubOrganizationId = wo.User.SubOrganizationId,
            //         RoleId = wo.User.RoleId,

            //     }).OrderByDescending(o => o.CreationDate).ToList();
            //foreach (var order in lstWorkOrders)
            //{
            //    IndexWorkOrderVM getDataObj = new IndexWorkOrderVM();
            //    getDataObj.Id = order.Id;
            //    getDataObj.Subject = order.Subject;
            //    getDataObj.SerialNumber = order.SerialNumber;
            //    getDataObj.ModelNumber = order.ModelNumber;
            //    getDataObj.BarCode = order.BarCode;
            //    getDataObj.AssetName = order.AssetName;
            //    getDataObj.AssetNameAr = order.AssetNameAr;
            //    getDataObj.WorkOrderNumber = order.WorkOrderNumber;
            //    getDataObj.WorkOrderTypeName = order.WorkOrderTypeName;
            //    getDataObj.StatusName = order.StatusName;
            //    getDataObj.UserName = order.UserName;
            //    getDataObj.AssignedTo = order.AssignedTo;
            //    getDataObj.CreatedById = order.CreatedById;
            //    getDataObj.CreatedBy = order.CreatedBy;
            //    getDataObj.UserName = order.UserName;
            //    getDataObj.ActualStartDate = order.ActualStartDate;
            //    getDataObj.ActualEndDate = order.ActualEndDate;
            //    getDataObj.PlannedStartDate = order.PlannedStartDate;
            //    getDataObj.PlannedEndDate = order.PlannedEndDate;
            //    getDataObj.WorkOrderPeriorityId = order.WorkOrderPeriorityId;
            //    getDataObj.WorkOrderPeriorityName = order.WorkOrderPeriorityName;
            //    getDataObj.HospitalId = order.HospitalId;
            //    getDataObj.GovernorateId = order.GovernorateId;
            //    getDataObj.CityId = order.CityId;
            //    getDataObj.OrganizationId = order.OrganizationId;
            //    getDataObj.SubOrganizationId = order.SubOrganizationId;
            //    getDataObj.CreatedById = order.CreatedById;
            //    getDataObj.CreationDate = order.CreationDate;
            //    getDataObj.AssetName = _context.MasterAssets.Where(a => a.Id == order.MasterAssetId).ToList().FirstOrDefault().Name;
            //    getDataObj.AssetNameAr = _context.MasterAssets.Where(a => a.Id == order.MasterAssetId).ToList().FirstOrDefault().NameAr;
            //    getDataObj.WorkOrderTrackingId = order.WorkOrderTrackingId;
            //    list.Add(getDataObj);
            //}
            //if (UserObj.GovernorateId > 0 && UserObj.CityId == 0 && UserObj.HospitalId == 0)
            //{
            //    list = list.Where(t => t.GovernorateId == UserObj.GovernorateId).ToList();
            //}
            //if (UserObj.GovernorateId > 0 && UserObj.CityId > 0 && UserObj.HospitalId == 0)
            //{
            //    list = list.Where(t => t.CityId == UserObj.CityId).ToList();
            //}
            //if (UserObj.OrganizationId > 0 && UserObj.SubOrganizationId == 0 && UserObj.HospitalId == 0)
            //{
            //    list = list.Where(t => t.OrganizationId == UserObj.OrganizationId).ToList();
            //}
            //if (UserObj.OrganizationId > 0 && UserObj.SubOrganizationId > 0 && UserObj.HospitalId == 0)
            //{
            //    list = list.Where(t => t.SubOrganizationId == UserObj.SubOrganizationId).ToList();
            //}
            //if (UserObj.HospitalId > 0)
            //{
            //    if (userRoleName == "Admin")
            //    {
            //        list = list.ToList();
            //    }
            //    if (userRoleName == "TLHospitalManager")
            //    {
            //        list = list.Where(t => t.HospitalId == UserObj.HospitalId).ToList();
            //    }
            //    if (userRoleName == "EngManager")
            //    {
            //        list = list.Where(t => t.HospitalId == UserObj.HospitalId).ToList();
            //    }

            //    if (userRoleName == "EngDepManager")
            //    {
            //        list = list.Where(t => t.HospitalId == UserObj.HospitalId).ToList();
            //    }

            //    if (userRoleName == "Eng")
            //    {
            //        var lstAssigned = list.Where(t => t.HospitalId == UserObj.HospitalId && t.AssignedTo == userId).ToList();
            //        var lstCreatedItems = list.Where(t => t.HospitalId == UserObj.HospitalId && t.CreatedById == userId).ToList();
            //        list = lstAssigned.Concat(lstCreatedItems).ToList();
            //    }

            //}
            //return list;


            var lstWorkOrders = _context.WorkOrderTrackings
                                    .Include(w => w.WorkOrder)
                                    .Include(w => w.WorkOrder.Request)
                                    .Include(w => w.WorkOrder.Request.AssetDetail)
                                    .Include(w => w.WorkOrder.Request.AssetDetail.MasterAsset)
                                    .Include(w => w.WorkOrder.WorkOrderType)
                                    .Include(w => w.WorkOrder.WorkOrderPeriority)
                                    .Include(w => w.WorkOrder.User)
                                    .Include(w => w.AssignedToUser)
                                    .Where(a => a.WorkOrder.Request.AssetDetailId == assetId)
                                    .OrderByDescending(o => o.CreationDate).ToList().GroupBy(a => a.WorkOrderId).ToList();

            foreach (var order in lstWorkOrders)
            {
                IndexWorkOrderVM getDataObj = new IndexWorkOrderVM();
                getDataObj.Id = order.FirstOrDefault().Id;
                getDataObj.Subject = order.FirstOrDefault().WorkOrder.Subject;
                getDataObj.WorkOrderNumber = order.FirstOrDefault().WorkOrder.WorkOrderNumber;
                // getDataObj.WorkOrderTypeName = order.WorkOrder.WorkOrderType.Name;
                //getDataObj.StatusName = order.StatusName;
                //getDataObj.UserName = order.UserName;
                //  getDataObj.AssignedTo = order.AssignedTo;
                // getDataObj.CreatedById = order.CreatedById;
                //getDataObj.CreatedBy = order.CreatedBy;
                //getDataObj.UserName = order.UserName;
                // getDataObj.ActualStartDate = order.ActualStartDate;
                //  getDataObj.ActualEndDate = order.ActualEndDate;
                // getDataObj.WorkOrderPeriorityName = order.WorkOrderPeriorityName;
                getDataObj.CreationDate = order.FirstOrDefault().CreationDate;
                list.Add(getDataObj);
            }
            if (UserObj.GovernorateId > 0 && UserObj.CityId == 0 && UserObj.HospitalId == 0)
            {
                list = list.Where(t => t.GovernorateId == UserObj.GovernorateId).ToList();
            }
            if (UserObj.GovernorateId > 0 && UserObj.CityId > 0 && UserObj.HospitalId == 0)
            {
                list = list.Where(t => t.CityId == UserObj.CityId).ToList();
            }
            if (UserObj.OrganizationId > 0 && UserObj.SubOrganizationId == 0 && UserObj.HospitalId == 0)
            {
                list = list.Where(t => t.OrganizationId == UserObj.OrganizationId).ToList();
            }
            if (UserObj.OrganizationId > 0 && UserObj.SubOrganizationId > 0 && UserObj.HospitalId == 0)
            {
                list = list.Where(t => t.SubOrganizationId == UserObj.SubOrganizationId).ToList();
            }
            if (UserObj.HospitalId > 0)
            {
                if (userRoleName == "Admin")
                {
                    list = list.ToList();
                }
                if (userRoleName == "TLHospitalManager")
                {
                    list = list.Where(t => t.HospitalId == UserObj.HospitalId).ToList();
                }
                if (userRoleName == "EngManager")
                {
                    list = list.Where(t => t.HospitalId == UserObj.HospitalId).ToList();
                }

                if (userRoleName == "EngDepManager")
                {
                    list = list.Where(t => t.HospitalId == UserObj.HospitalId).ToList();
                }

                if (userRoleName == "Eng")
                {
                    var lstAssigned = list.Where(t => t.HospitalId == UserObj.HospitalId && t.AssignedTo == userId).ToList();
                    var lstCreatedItems = list.Where(t => t.HospitalId == UserObj.HospitalId && t.CreatedById == userId).ToList();
                    list = lstAssigned.Concat(lstCreatedItems).ToList();
                }

            }
            return list;


        }

        public IEnumerable<IndexWorkOrderVM> SearchWorkOrders(SearchWorkOrderVM searchObj)
        {
            List<IndexWorkOrderVM> lstData = new List<IndexWorkOrderVM>();

            ApplicationUser UserObj = new ApplicationUser();
            var obj = _context.ApplicationUser.Where(a => a.Id == searchObj.UserId).ToList();
            if (obj.Count > 0)
            {
                UserObj = obj[0];
            }


            var list = _context.WorkOrders.Include(w => w.Request).Include(a => a.Request.AssetDetail)
               .Include(a => a.Request.RequestType).Include(a => a.Request.SubProblem).Include(a => a.Request.SubProblem.Problem)
               .Include(a => a.Request.RequestMode)
                .Include(a => a.Request.AssetDetail.Department)
                 .Include(a => a.Request.AssetDetail)
               .Include(a => a.Request.AssetDetail.Hospital)
               .Include(a => a.Request.AssetDetail.MasterAsset)
               .Include(w => w.WorkOrderType).Include(w => w.WorkOrderPeriority).Include(w => w.User).ToList();



            if (list.Count > 0)
            {
                if (UserObj.GovernorateId == 0 && UserObj.CityId == 0 && UserObj.HospitalId == 0)
                {
                    list = list.ToList();
                }
                if (UserObj.GovernorateId > 0 && UserObj.CityId == 0 && UserObj.HospitalId == 0)
                {
                    list = list.Where(t => t.Request.AssetDetail.Hospital.GovernorateId == UserObj.GovernorateId).ToList();
                }
                if (UserObj.GovernorateId > 0 && UserObj.CityId > 0 && UserObj.HospitalId == 0)
                {
                    list = list.Where(t => t.Request.AssetDetail.Hospital.CityId == UserObj.CityId).ToList();
                }

                if (UserObj.OrganizationId > 0 && UserObj.SubOrganizationId == 0 && UserObj.HospitalId == 0)
                {
                    list = list.Where(t => t.Request.AssetDetail.Hospital.OrganizationId == UserObj.OrganizationId).ToList();
                }
                if (UserObj.OrganizationId > 0 && UserObj.SubOrganizationId > 0 && UserObj.HospitalId == 0)
                {
                    list = list.Where(t => t.Request.AssetDetail.Hospital.SubOrganizationId == UserObj.SubOrganizationId).ToList();
                }

                if (UserObj.HospitalId > 0 && searchObj.HospitalId == 0)
                {
                    list = list.Where(t => t.Request.AssetDetail.Hospital.Id == UserObj.HospitalId).ToList();
                }
                else if (searchObj.HospitalId != 0 && UserObj.HospitalId == 0)
                {
                    list = list.Where(a => a.Request.AssetDetail.HospitalId == searchObj.HospitalId).ToList();
                }
                else
                    list = list.ToList();



                if (searchObj.DepartmentId != null)
                {
                    list = list.Where(a => a.Request.AssetDetail.DepartmentId == searchObj.DepartmentId).ToList();
                }
                else
                    list = list.ToList();



                if (searchObj.PeriorityId != null)
                {
                    list = list.Where(a => a.WorkOrderPeriorityId == searchObj.PeriorityId).ToList();
                }
                else
                    list = list.ToList();



                if (searchObj.MasterAssetId != 0)
                {
                    list = list.Where(a => a.Request.AssetDetail.MasterAssetId == searchObj.MasterAssetId).ToList();
                }
                else
                    list = list.ToList();


                if (searchObj.Subject != "")
                {
                    list = list.Where(a => a.Subject.Contains(searchObj.Subject)).ToList();
                }
                else
                    list = list.ToList();

                if (searchObj.ModelNumber != "")
                {
                    lstData = lstData.Where(a => a.ModelNumber.Contains(searchObj.ModelNumber)).ToList();
                }
                else
                    list = list.ToList();

                if (searchObj.BarCode != "")
                {
                    list = list.Where(b => b.Request.AssetDetail.Barcode.Contains(searchObj.BarCode)).ToList();
                }
                else
                    list = list.ToList();



                if (searchObj.WONumber != "")
                {
                    list = list.Where(b => b.WorkOrderNumber.Contains(searchObj.WONumber)).ToList();
                }
                else
                    list = list.ToList();

                if (searchObj.SerialNumber != "")
                {
                    list = list.Where(b => b.Request.AssetDetail.SerialNumber.Contains(searchObj.SerialNumber)).ToList();
                }
                else
                    list = list.ToList();



                if (searchObj.RequestSubject != "")
                {
                    list = list.Where(b => b.Subject.Contains(searchObj.RequestSubject)).ToList();
                }
                else
                    list = list.ToList();

            }

            foreach (var item in list)
            {
                IndexWorkOrderVM getDataObj = new IndexWorkOrderVM();
                getDataObj.Id = item.Id;
                getDataObj.Subject = item.Subject;
                getDataObj.BarCode = item.Request.AssetDetail.Barcode;
                getDataObj.ModelNumber = item.Request.AssetDetail.MasterAsset.ModelNumber;
                getDataObj.AssetName = item.Request.AssetDetail.MasterAsset.Name;
                //getDataObj.MasterAssetId = item.Request.AssetDetail.MasterAssetId;
                getDataObj.AssetNameAr = item.Request.AssetDetail.MasterAsset.NameAr;
                getDataObj.SerialNumber = item.Request.AssetDetail.SerialNumber;
                getDataObj.WorkOrderNumber = item.WorkOrderNumber;
                getDataObj.WorkOrderTypeName = item.WorkOrderType.Name;
                getDataObj.RequestSubject = item.Request.Subject;
                getDataObj.AssetId = item.Request.AssetDetailId;
                getDataObj.CreatedById = item.CreatedById;
                getDataObj.CreatedBy = item.User.UserName;
                getDataObj.ActualStartDate = item.ActualStartDate;
                getDataObj.ActualEndDate = item.ActualEndDate;
                getDataObj.PlannedStartDate = item.PlannedStartDate;
                getDataObj.PlannedEndDate = item.PlannedEndDate;
                getDataObj.WorkOrderPeriorityId = item.WorkOrderPeriorityId != null ? (int)item.WorkOrderPeriorityId : 0;
                getDataObj.WorkOrderPeriorityName = item.WorkOrderPeriority != null ? item.WorkOrderPeriority.Name : "";
                getDataObj.HospitalId = item.Request.AssetDetail.HospitalId;
                getDataObj.GovernorateId = item.Request.AssetDetail.Hospital.GovernorateId;
                getDataObj.CityId = item.Request.AssetDetail.Hospital.CityId;
                getDataObj.OrganizationId = item.Request.AssetDetail.Hospital.OrganizationId;
                getDataObj.SubOrganizationId = item.Request.AssetDetail.Hospital.SubOrganizationId;
                getDataObj.CreatedById = item.CreatedById;
                getDataObj.CreationDate = item.CreationDate;
                getDataObj.AssetName = item.Request.AssetDetail.MasterAsset.Name + " - " + item.Request.AssetDetail.SerialNumber;
                getDataObj.AssetNameAr = item.Request.AssetDetail.MasterAsset.NameAr + " - " + item.Request.AssetDetail.SerialNumber;
                getDataObj.MasterAssetId = item.Request.AssetDetail.MasterAssetId;
                getDataObj.Note = item.Note;
                var lstStatus = _context.WorkOrderTrackings
                            .Include(t => t.WorkOrder).Include(t => t.WorkOrderStatus)
                            .Where(a => a.WorkOrderId == item.Id).ToList().OrderByDescending(a => a.Id).ToList();
                if (lstStatus.Count > 0)
                {

                    getDataObj.WorkOrderStatusId = (int)lstStatus[0].WorkOrderStatus.Id;
                    getDataObj.StatusName = lstStatus[0].WorkOrderStatus.Name;
                    getDataObj.StatusNameAr = lstStatus[0].WorkOrderStatus.NameAr;
                    getDataObj.statusColor = lstStatus[0].WorkOrderStatus.Color;
                    getDataObj.statusIcon = lstStatus[0].WorkOrderStatus.Icon;
                }

                if (item.Request.AssetDetailId != null)
                {
                    getDataObj.AssetName = item.Request.AssetDetail.MasterAsset.Name;
                    getDataObj.AssetNameAr = item.Request.AssetDetail.MasterAsset.NameAr;
                }


                if (item.WorkOrderPeriorityId != null)
                {
                    getDataObj.WorkOrderPeriorityId = (int)item.WorkOrderPeriorityId;
                    getDataObj.PeriorityName = item.WorkOrderPeriority.Name;
                    getDataObj.PeriorityNameAr = item.WorkOrderPeriority.NameAr;

                }
                if (item.Request.AssetDetail.DepartmentId != null)
                {
                    getDataObj.DepartmentId = (int)item.Request.AssetDetail.DepartmentId;
                    getDataObj.DepartmentName = item.Request.AssetDetail.Department.Name;
                    getDataObj.DepartmentNameAr = item.Request.AssetDetail.Department.NameAr;

                }
                lstData.Add(getDataObj);
            }



            if (lstData.Count > 0)
            {
                if (searchObj.StatusId != 0)
                {
                    lstData = lstData.Where(a => a.WorkOrderStatusId == searchObj.StatusId).ToList();
                }
                else
                    lstData = lstData.ToList();




                string setstartday, setstartmonth, setendday, setendmonth = "";
                DateTime startingFrom = new DateTime();
                DateTime endingTo = new DateTime();
                if (searchObj.Start == "")
                {
                    //  searchObj.PlannedStartDate = DateTime.Parse("01/01/1900");
                }
                if (searchObj.Start == null)
                {
                    //  searchObj.PlannedStartDate = DateTime.Parse("01/01/1900");
                }
                else if (searchObj.Start != "")
                {
                    searchObj.PlannedStartDate = DateTime.Parse(searchObj.Start.ToString());

                    var startyear = searchObj.PlannedStartDate.Value.Year;
                    var startmonth = searchObj.PlannedStartDate.Value.Month;
                    var startday = searchObj.PlannedStartDate.Value.Day;
                    if (startday < 10)
                        setstartday = searchObj.PlannedStartDate.Value.Day.ToString().PadLeft(2, '0');
                    else
                        setstartday = searchObj.PlannedStartDate.Value.Day.ToString();

                    if (startmonth < 10)
                        setstartmonth = searchObj.PlannedStartDate.Value.Month.ToString().PadLeft(2, '0');
                    else
                        setstartmonth = searchObj.PlannedStartDate.Value.Month.ToString();

                    var sDate = startyear + "/" + setstartmonth + "/" + setstartday;
                    startingFrom = DateTime.Parse(sDate);

                }

                if (searchObj.End == "")
                {
                    // searchObj.PlannedEndDate = DateTime.Today.Date;
                }
                if (searchObj.End == null)
                {
                    // searchObj.PlannedEndDate = DateTime.Today.Date;
                }
                else if (searchObj.End != "")
                {
                    searchObj.PlannedEndDate = DateTime.Parse(searchObj.End.ToString());
                    var endyear = searchObj.PlannedEndDate.Value.Year;
                    var endmonth = searchObj.PlannedEndDate.Value.Month;
                    var endday = searchObj.PlannedEndDate.Value.Day;
                    if (endday < 10)
                        setendday = searchObj.PlannedEndDate.Value.Day.ToString().PadLeft(2, '0');
                    else
                        setendday = searchObj.PlannedEndDate.Value.Day.ToString();

                    if (endmonth < 10)
                        setendmonth = searchObj.PlannedEndDate.Value.Month.ToString().PadLeft(2, '0');
                    else
                        setendmonth = searchObj.PlannedEndDate.Value.Month.ToString();

                    var eDate = endyear + "/" + setendmonth + "/" + setendday;
                    endingTo = DateTime.Parse(eDate);
                }



                if (searchObj.Start != "" && searchObj.End != "")
                {
                    lstData = lstData.Where(a => a.PlannedStartDate.Value.Date >= startingFrom.Date && a.PlannedStartDate.Value.Date <= endingTo.Date).ToList();
                }
            }

            return lstData;
        }

        public IndexWorkOrderVM2 SearchWorkOrders(SearchWorkOrderVM searchObj, int pageNumber, int pageSize)
        {
            IndexWorkOrderVM2 mainClass = new IndexWorkOrderVM2();
            List<IndexWorkOrderVM2.GetData> lstData = new List<IndexWorkOrderVM2.GetData>();

            ApplicationUser UserObj = new ApplicationUser();
            var obj = _context.ApplicationUser.Where(a => a.Id == searchObj.UserId).ToList();
            if (obj.Count > 0)
            {
                UserObj = obj[0];
            }


            var list = _context.WorkOrders.Include(w => w.Request).Include(a => a.Request.AssetDetail)
               .Include(a => a.Request.RequestType).Include(a => a.Request.SubProblem).Include(a => a.Request.SubProblem.Problem)
               .Include(a => a.Request.RequestMode)
                .Include(a => a.Request.AssetDetail.Department)
                 .Include(a => a.Request.AssetDetail)
               .Include(a => a.Request.AssetDetail.Hospital)
               .Include(a => a.Request.AssetDetail.MasterAsset)
               .Include(w => w.WorkOrderType).Include(w => w.WorkOrderPeriority).Include(w => w.User).ToList();



            if (list.Count > 0)
            {
                if (UserObj.GovernorateId == 0 && UserObj.CityId == 0 && UserObj.HospitalId == 0)
                {
                    list = list.ToList();
                }
                if (UserObj.GovernorateId > 0 && UserObj.CityId == 0 && UserObj.HospitalId == 0)
                {
                    list = list.Where(t => t.Request.AssetDetail.Hospital.GovernorateId == UserObj.GovernorateId).ToList();
                }
                if (UserObj.GovernorateId > 0 && UserObj.CityId > 0 && UserObj.HospitalId == 0)
                {
                    list = list.Where(t => t.Request.AssetDetail.Hospital.CityId == UserObj.CityId).ToList();
                }

                if (UserObj.OrganizationId > 0 && UserObj.SubOrganizationId == 0 && UserObj.HospitalId == 0)
                {
                    list = list.Where(t => t.Request.AssetDetail.Hospital.OrganizationId == UserObj.OrganizationId).ToList();
                }
                if (UserObj.OrganizationId > 0 && UserObj.SubOrganizationId > 0 && UserObj.HospitalId == 0)
                {
                    list = list.Where(t => t.Request.AssetDetail.Hospital.SubOrganizationId == UserObj.SubOrganizationId).ToList();
                }

                if (UserObj.HospitalId > 0 && searchObj.HospitalId == 0)
                {
                    list = list.Where(t => t.Request.AssetDetail.Hospital.Id == UserObj.HospitalId).ToList();
                }
                else if (searchObj.HospitalId != 0 && UserObj.HospitalId == 0)
                {
                    list = list.Where(a => a.Request.AssetDetail.HospitalId == searchObj.HospitalId).ToList();
                }
                else
                    list = list.ToList();



                if (searchObj.DepartmentId != null)
                {
                    list = list.Where(a => a.Request.AssetDetail.DepartmentId == searchObj.DepartmentId).ToList();
                }
                else
                    list = list.ToList();



                if (searchObj.PeriorityId != null)
                {
                    list = list.Where(a => a.WorkOrderPeriorityId == searchObj.PeriorityId).ToList();
                }
                else
                    list = list.ToList();



                if (searchObj.MasterAssetId != 0)
                {
                    list = list.Where(a => a.Request.AssetDetail.MasterAssetId == searchObj.MasterAssetId).ToList();
                }
                else
                    list = list.ToList();


                if (searchObj.Subject != "")
                {
                    list = list.Where(a => a.Subject.Contains(searchObj.Subject)).ToList();
                }
                else
                    list = list.ToList();

                if (searchObj.ModelNumber != "")
                {
                    list = list.Where(a => a.Request.AssetDetail.MasterAsset.ModelNumber.Contains(searchObj.ModelNumber)).ToList();
                }
                else
                    list = list.ToList();

                if (searchObj.BarCode != "")
                {
                    list = list.Where(b => b.Request.AssetDetail.Barcode.Contains(searchObj.BarCode)).ToList();
                }
                else
                    list = list.ToList();



                if (searchObj.WONumber != "")
                {
                    list = list.Where(b => b.WorkOrderNumber.Contains(searchObj.WONumber)).ToList();
                }
                else
                    list = list.ToList();

                if (searchObj.SerialNumber != "")
                {
                    list = list.Where(b => b.Request.AssetDetail.SerialNumber.Contains(searchObj.SerialNumber)).ToList();
                }
                else
                    list = list.ToList();



                if (searchObj.RequestSubject != "")
                {
                    list = list.Where(b => b.Subject.Contains(searchObj.RequestSubject)).ToList();
                }
                else
                    list = list.ToList();

            }

            foreach (var item in list)
            {
                IndexWorkOrderVM2.GetData getDataObj = new IndexWorkOrderVM2.GetData();
                getDataObj.Id = item.Id;
                getDataObj.Subject = item.Subject;
                getDataObj.BarCode = item.Request.AssetDetail.Barcode;
                getDataObj.ModelNumber = item.Request.AssetDetail.MasterAsset.ModelNumber;
                getDataObj.AssetName = item.Request.AssetDetail.MasterAsset.Name;
                getDataObj.AssetNameAr = item.Request.AssetDetail.MasterAsset.NameAr;
                getDataObj.SerialNumber = item.Request.AssetDetail.SerialNumber;
                getDataObj.WorkOrderNumber = item.WorkOrderNumber;
                getDataObj.WorkOrderTypeName = item.WorkOrderType.Name;
                getDataObj.RequestSubject = item.Request.Subject;
                getDataObj.AssetId = item.Request.AssetDetailId;
                getDataObj.CreatedById = item.CreatedById;
                getDataObj.CreatedBy = item.User.UserName;
                getDataObj.ActualStartDate = item.ActualStartDate;
                getDataObj.ActualEndDate = item.ActualEndDate;
                getDataObj.PlannedStartDate = item.PlannedStartDate;
                getDataObj.PlannedEndDate = item.PlannedEndDate;
                getDataObj.WorkOrderPeriorityId = item.WorkOrderPeriorityId != null ? (int)item.WorkOrderPeriorityId : 0;
                getDataObj.WorkOrderPeriorityName = item.WorkOrderPeriority != null ? item.WorkOrderPeriority.Name : "";
                getDataObj.HospitalId = item.Request.AssetDetail.HospitalId;
                getDataObj.GovernorateId = item.Request.AssetDetail.Hospital.GovernorateId;
                getDataObj.CityId = item.Request.AssetDetail.Hospital.CityId;
                getDataObj.OrganizationId = item.Request.AssetDetail.Hospital.OrganizationId;
                getDataObj.SubOrganizationId = item.Request.AssetDetail.Hospital.SubOrganizationId;
                getDataObj.CreatedById = item.CreatedById;
                getDataObj.CreationDate = item.CreationDate;
                getDataObj.AssetName = item.Request.AssetDetail.MasterAsset.Name + " - " + item.Request.AssetDetail.SerialNumber;
                getDataObj.AssetNameAr = item.Request.AssetDetail.MasterAsset.NameAr + " - " + item.Request.AssetDetail.SerialNumber;
                getDataObj.MasterAssetId = item.Request.AssetDetail.MasterAssetId;
                getDataObj.Note = item.Note;
                var lstStatus = _context.WorkOrderTrackings
                            .Include(t => t.WorkOrder).Include(t => t.WorkOrderStatus)
                            .Where(a => a.WorkOrderId == item.Id).ToList().OrderByDescending(a => a.Id).ToList();
                if (lstStatus.Count > 0)
                {

                    getDataObj.WorkOrderStatusId = (int)lstStatus[0].WorkOrderStatus.Id;
                    getDataObj.StatusName = lstStatus[0].WorkOrderStatus.Name;
                    getDataObj.StatusNameAr = lstStatus[0].WorkOrderStatus.NameAr;
                    getDataObj.statusColor = lstStatus[0].WorkOrderStatus.Color;
                    getDataObj.statusIcon = lstStatus[0].WorkOrderStatus.Icon;
                }

                if (item.Request.AssetDetailId != null)
                {
                    getDataObj.AssetName = item.Request.AssetDetail.MasterAsset.Name;
                    getDataObj.AssetNameAr = item.Request.AssetDetail.MasterAsset.NameAr;
                }


                if (item.WorkOrderPeriorityId != null)
                {
                    getDataObj.WorkOrderPeriorityId = (int)item.WorkOrderPeriorityId;
                    getDataObj.PeriorityName = item.WorkOrderPeriority.Name;
                    getDataObj.PeriorityNameAr = item.WorkOrderPeriority.NameAr;

                }
                if (item.Request.AssetDetail.DepartmentId != null)
                {
                    getDataObj.DepartmentId = (int)item.Request.AssetDetail.DepartmentId;
                    getDataObj.DepartmentName = item.Request.AssetDetail.Department.Name;
                    getDataObj.DepartmentNameAr = item.Request.AssetDetail.Department.NameAr;

                }
                lstData.Add(getDataObj);
            }



            if (lstData.Count > 0)
            {
                if (searchObj.StatusId != 0)
                {
                    lstData = lstData.Where(a => a.WorkOrderStatusId == searchObj.StatusId).ToList();
                }
                else
                    lstData = lstData.ToList();




                string setstartday, setstartmonth, setendday, setendmonth = "";
                DateTime startingFrom = new DateTime();
                DateTime endingTo = new DateTime();
                if (searchObj.Start == "")
                {
                    //  searchObj.PlannedStartDate = DateTime.Parse("01/01/1900");
                }
                if (searchObj.Start == null)
                {
                    //  searchObj.PlannedStartDate = DateTime.Parse("01/01/1900");
                }
                else if (searchObj.Start != "")
                {
                    searchObj.PlannedStartDate = DateTime.Parse(searchObj.Start.ToString());

                    var startyear = searchObj.PlannedStartDate.Value.Year;
                    var startmonth = searchObj.PlannedStartDate.Value.Month;
                    var startday = searchObj.PlannedStartDate.Value.Day;
                    if (startday < 10)
                        setstartday = searchObj.PlannedStartDate.Value.Day.ToString().PadLeft(2, '0');
                    else
                        setstartday = searchObj.PlannedStartDate.Value.Day.ToString();

                    if (startmonth < 10)
                        setstartmonth = searchObj.PlannedStartDate.Value.Month.ToString().PadLeft(2, '0');
                    else
                        setstartmonth = searchObj.PlannedStartDate.Value.Month.ToString();

                    var sDate = startyear + "/" + setstartmonth + "/" + setstartday;
                    startingFrom = DateTime.Parse(sDate);

                }

                if (searchObj.End == "")
                {
                    // searchObj.PlannedEndDate = DateTime.Today.Date;
                }
                if (searchObj.End == null)
                {
                    // searchObj.PlannedEndDate = DateTime.Today.Date;
                }
                else if (searchObj.End != "")
                {
                    searchObj.PlannedEndDate = DateTime.Parse(searchObj.End.ToString());
                    var endyear = searchObj.PlannedEndDate.Value.Year;
                    var endmonth = searchObj.PlannedEndDate.Value.Month;
                    var endday = searchObj.PlannedEndDate.Value.Day;
                    if (endday < 10)
                        setendday = searchObj.PlannedEndDate.Value.Day.ToString().PadLeft(2, '0');
                    else
                        setendday = searchObj.PlannedEndDate.Value.Day.ToString();

                    if (endmonth < 10)
                        setendmonth = searchObj.PlannedEndDate.Value.Month.ToString().PadLeft(2, '0');
                    else
                        setendmonth = searchObj.PlannedEndDate.Value.Month.ToString();

                    var eDate = endyear + "/" + setendmonth + "/" + setendday;
                    endingTo = DateTime.Parse(eDate);
                }



                if (searchObj.Start != "" && searchObj.End != "")
                {
                    lstData = lstData.Where(a => a.PlannedStartDate.Value.Date >= startingFrom.Date && a.PlannedStartDate.Value.Date <= endingTo.Date).ToList();
                }
            }
            var requestsPerPage = lstData.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            mainClass.Results = requestsPerPage;
            mainClass.Count = lstData.Count();
            return mainClass;
        }

        public IEnumerable<IndexWorkOrderVM> SortWorkOrders(int hosId, string userId, SortWorkOrderVM sortObj, int statusId)
        {
            var list = GetAllWorkOrdersByHospitalId(hosId);



            if (sortObj.AssetName != "")
            {
                if (sortObj.StatusId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        list = list.Where(b => b.StatusId == sortObj.StatusId).OrderByDescending(d => d.StatusName).ToList();
                    }
                    else
                    {
                        list = list.Where(b => b.StatusId == sortObj.StatusId).OrderBy(d => d.StatusName).ToList();
                    }
                }
                if (sortObj.StrRequestSubject != "")
                {
                    if (sortObj.SortStatus == "descending")
                        list = list.Where(b => b.RequestSubject.Contains(sortObj.StrRequestSubject)).OrderByDescending(d => d.RequestSubject).ToList();

                    else
                        list = list.Where(b => b.RequestSubject.Contains(sortObj.StrRequestSubject)).OrderBy(d => d.RequestSubject).ToList();
                }
                if (sortObj.StrBarCode != "")
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        list = list.Where(b => b.BarCode.Contains(sortObj.StrBarCode)).OrderByDescending(d => d.BarCode).ToList();
                    }
                    else
                    {
                        list = list.Where(b => b.BarCode.Contains(sortObj.StrBarCode)).OrderBy(d => d.BarCode).ToList();
                    }
                }
                if (sortObj.StrSerial != "")
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        list = list.Where(b => b.SerialNumber.Contains(sortObj.StrSerial)).OrderByDescending(d => d.SerialNumber).ToList();
                    }
                    else
                    {
                        list = list.Where(b => b.SerialNumber.Contains(sortObj.StrSerial)).OrderBy(d => d.SerialNumber).ToList();
                    }
                }
                if (sortObj.StrModel != "")
                {
                    if (sortObj.SortStatus == "descending")
                        list = list.Where(b => b.ModelNumber.Contains(sortObj.StrModel)).OrderByDescending(d => d.ModelNumber).ToList();
                    else
                        list = list.Where(b => b.ModelNumber.Contains(sortObj.StrModel)).OrderBy(d => d.ModelNumber).ToList();
                }
                if (sortObj.StrWorkOrderNumber != "")
                {
                    if (sortObj.SortStatus == "descending")
                        list = list.Where(b => b.WorkOrderNumber.Contains(sortObj.StrWorkOrderNumber)).OrderByDescending(d => d.WorkOrderNumber).ToList();
                    else
                        list = list.Where(b => b.WorkOrderNumber.Contains(sortObj.StrWorkOrderNumber)).OrderBy(d => d.WorkOrderNumber).ToList();
                }
                if (sortObj.StrSubject != "")
                {
                    if (sortObj.SortStatus == "descending")
                        list = list.Where(b => b.Subject.Contains(sortObj.StrSubject)).OrderByDescending(d => d.Subject).ToList();

                    else
                        list = list.Where(b => b.Subject.Contains(sortObj.StrSubject)).OrderBy(d => d.Subject).ToList();
                }
                else
                {
                    if (sortObj.SortStatus == "descending")
                        list = list.OrderByDescending(d => d.AssetName).ToList();
                    else
                        list = list.OrderBy(d => d.AssetName).ToList();
                }
            }

            if (sortObj.AssetNameAr != "")
            {
                if (sortObj.StatusId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        list = list.Where(b => b.StatusId == sortObj.StatusId).OrderByDescending(d => d.StatusName).ToList();
                    }
                    else
                    {
                        list = list.Where(b => b.StatusId == sortObj.StatusId).OrderBy(d => d.StatusName).ToList();
                    }
                }
                if (sortObj.StrRequestSubject != "")
                {
                    if (sortObj.SortStatus == "descending")
                        list = list.Where(b => b.RequestSubject.Contains(sortObj.StrRequestSubject)).OrderByDescending(d => d.RequestSubject).ToList();

                    else
                        list = list.Where(b => b.RequestSubject.Contains(sortObj.StrRequestSubject)).OrderBy(d => d.RequestSubject).ToList();
                }
                if (sortObj.StrBarCode != "")
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        list = list.Where(b => b.BarCode.Contains(sortObj.StrBarCode)).OrderByDescending(d => d.BarCode).ToList();
                    }
                    else
                    {
                        list = list.Where(b => b.BarCode.Contains(sortObj.StrBarCode)).OrderBy(d => d.BarCode).ToList();
                    }
                }
                if (sortObj.StrSerial != "")
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        list = list.Where(b => b.SerialNumber.Contains(sortObj.StrSerial)).OrderByDescending(d => d.SerialNumber).ToList();
                    }
                    else
                    {
                        list = list.Where(b => b.SerialNumber.Contains(sortObj.StrSerial)).OrderBy(d => d.SerialNumber).ToList();
                    }
                }
                if (sortObj.StrModel != "")
                {
                    if (sortObj.SortStatus == "descending")
                        list = list.Where(b => b.ModelNumber.Contains(sortObj.StrModel)).OrderByDescending(d => d.ModelNumber).ToList();
                    else
                        list = list.Where(b => b.ModelNumber.Contains(sortObj.StrModel)).OrderBy(d => d.ModelNumber).ToList();
                }
                if (sortObj.StrWorkOrderNumber != "")
                {
                    if (sortObj.SortStatus == "descending")
                        list = list.Where(b => b.WorkOrderNumber.Contains(sortObj.StrWorkOrderNumber)).OrderByDescending(d => d.WorkOrderNumber).ToList();
                    else
                        list = list.Where(b => b.WorkOrderNumber.Contains(sortObj.StrWorkOrderNumber)).OrderBy(d => d.WorkOrderNumber).ToList();
                }
                if (sortObj.StrSubject != "")
                {
                    if (sortObj.SortStatus == "descending")
                        list = list.Where(b => b.Subject.Contains(sortObj.StrSubject)).OrderByDescending(d => d.Subject).ToList();

                    else
                        list = list.Where(b => b.Subject.Contains(sortObj.StrSubject)).OrderBy(d => d.Subject).ToList();
                }
                else
                {
                    if (sortObj.SortStatus == "descending")
                        list = list.OrderByDescending(d => d.AssetNameAr).ToList();
                    else
                        list = list.OrderBy(d => d.AssetNameAr).ToList();
                }
            }

            if (sortObj.StatusName != "")
            {
                if (sortObj.StatusId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        list = list.Where(b => b.StatusId == sortObj.StatusId).OrderByDescending(d => d.StatusName).ToList();
                    }
                    else
                    {
                        list = list.Where(b => b.StatusId == sortObj.StatusId).OrderBy(d => d.StatusName).ToList();
                    }
                }
                if (sortObj.StrBarCode != "")
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        list = list.Where(b => b.BarCode.Contains(sortObj.StrBarCode)).OrderByDescending(d => d.BarCode).ToList();
                    }
                    else
                    {
                        list = list.Where(b => b.BarCode.Contains(sortObj.StrBarCode)).OrderBy(d => d.BarCode).ToList();
                    }
                }
                if (sortObj.StrRequestSubject != "")
                {
                    if (sortObj.SortStatus == "descending")
                        list = list.Where(b => b.RequestSubject.Contains(sortObj.StrRequestSubject)).OrderByDescending(d => d.RequestSubject).ToList();

                    else
                        list = list.Where(b => b.RequestSubject.Contains(sortObj.StrRequestSubject)).OrderBy(d => d.RequestSubject).ToList();
                }
                if (sortObj.StrSerial != "")
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        list = list.Where(b => b.SerialNumber.Contains(sortObj.StrSerial)).OrderByDescending(d => d.SerialNumber).ToList();
                    }
                    else
                    {
                        list = list.Where(b => b.SerialNumber.Contains(sortObj.StrSerial)).OrderBy(d => d.SerialNumber).ToList();
                    }
                }
                if (sortObj.StrModel != "")
                {
                    if (sortObj.SortStatus == "descending")
                        list = list.Where(b => b.ModelNumber.Contains(sortObj.StrModel)).OrderByDescending(d => d.ModelNumber).ToList();
                    else
                        list = list.Where(b => b.ModelNumber.Contains(sortObj.StrModel)).OrderBy(d => d.ModelNumber).ToList();
                }
                if (sortObj.StrWorkOrderNumber != "")
                {
                    if (sortObj.SortStatus == "descending")
                        list = list.Where(b => b.WorkOrderNumber.Contains(sortObj.StrWorkOrderNumber)).OrderByDescending(d => d.WorkOrderNumber).ToList();
                    else
                        list = list.Where(b => b.WorkOrderNumber.Contains(sortObj.StrWorkOrderNumber)).OrderBy(d => d.WorkOrderNumber).ToList();
                }
                if (sortObj.StrSubject != "")
                {
                    if (sortObj.SortStatus == "descending")
                        list = list.Where(b => b.Subject.Contains(sortObj.StrSubject)).OrderByDescending(d => d.Subject).ToList();

                    else
                        list = list.Where(b => b.Subject.Contains(sortObj.StrSubject)).OrderBy(d => d.Subject).ToList();
                }

                else
                {
                    if (sortObj.SortStatus == "descending")
                        list = list.OrderByDescending(d => d.StatusName).ToList();
                    else
                        list = list.OrderBy(d => d.StatusName).ToList();
                }
            }
            if (sortObj.StatusNameAr != "")
            {

                if (sortObj.StatusId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        list = list.Where(b => b.StatusId == sortObj.StatusId).OrderByDescending(d => d.StatusId).ToList();
                    }
                    else
                    {
                        list = list.Where(b => b.StatusId == sortObj.StatusId).OrderBy(d => d.StatusId).ToList();
                    }
                }
                if (sortObj.StrBarCode != "")
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        list = list.Where(b => b.BarCode.Contains(sortObj.StrBarCode)).OrderByDescending(d => d.BarCode).ToList();
                    }
                    else
                    {
                        list = list.Where(b => b.BarCode.Contains(sortObj.StrBarCode)).OrderBy(d => d.BarCode).ToList();
                    }
                }
                if (sortObj.StrRequestSubject != "")
                {
                    if (sortObj.SortStatus == "descending")
                        list = list.Where(b => b.RequestSubject.Contains(sortObj.StrRequestSubject)).OrderByDescending(d => d.RequestSubject).ToList();

                    else
                        list = list.Where(b => b.RequestSubject.Contains(sortObj.StrRequestSubject)).OrderBy(d => d.RequestSubject).ToList();
                }
                if (sortObj.StrSerial != "")
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        list = list.Where(b => b.SerialNumber.Contains(sortObj.StrSerial)).OrderByDescending(d => d.SerialNumber).ToList();
                    }
                    else
                    {
                        list = list.Where(b => b.SerialNumber.Contains(sortObj.StrSerial)).OrderBy(d => d.SerialNumber).ToList();
                    }
                }
                if (sortObj.StrModel != "")
                {
                    if (sortObj.SortStatus == "descending")
                        list = list.Where(b => b.ModelNumber.Contains(sortObj.StrModel)).OrderByDescending(d => d.ModelNumber).ToList();
                    else
                        list = list.Where(b => b.ModelNumber.Contains(sortObj.StrModel)).OrderBy(d => d.ModelNumber).ToList();
                }
                if (sortObj.StrWorkOrderNumber != "")
                {
                    if (sortObj.SortStatus == "descending")
                        list = list.Where(b => b.WorkOrderNumber.Contains(sortObj.StrWorkOrderNumber)).OrderByDescending(d => d.WorkOrderNumber).ToList();
                    else
                        list = list.Where(b => b.WorkOrderNumber.Contains(sortObj.StrWorkOrderNumber)).OrderBy(d => d.WorkOrderNumber).ToList();
                }
                if (sortObj.StrSubject != "")
                {
                    if (sortObj.SortStatus == "descending")
                        list = list.Where(b => b.Subject.Contains(sortObj.StrSubject)).OrderByDescending(d => d.Subject).ToList();

                    else
                        list = list.Where(b => b.Subject.Contains(sortObj.StrSubject)).OrderBy(d => d.Subject).ToList();
                }
                else
                {
                    if (sortObj.SortStatus == "descending")
                        list = list.OrderByDescending(d => d.StatusNameAr).ToList();
                    else
                        list = list.OrderBy(d => d.StatusNameAr).ToList();
                }
            }
            if (sortObj.Barcode != "")
            {
                if (sortObj.StatusId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        list = list.Where(b => b.StatusId == sortObj.StatusId).OrderByDescending(d => d.StatusId).ToList();
                    }
                    else
                    {
                        list = list.Where(b => b.StatusId == sortObj.StatusId).OrderBy(d => d.StatusId).ToList();
                    }
                }
                if (sortObj.StrBarCode != "")
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        list = list.Where(b => b.BarCode.Contains(sortObj.StrBarCode)).OrderByDescending(d => d.BarCode).ToList();
                    }
                    else
                    {
                        list = list.Where(b => b.BarCode.Contains(sortObj.StrBarCode)).OrderBy(d => d.BarCode).ToList();
                    }
                }
                if (sortObj.StrRequestSubject != "")
                {
                    if (sortObj.SortStatus == "descending")
                        list = list.Where(b => b.RequestSubject.Contains(sortObj.StrRequestSubject)).OrderByDescending(d => d.RequestSubject).ToList();

                    else
                        list = list.Where(b => b.RequestSubject.Contains(sortObj.StrRequestSubject)).OrderBy(d => d.RequestSubject).ToList();
                }
                if (sortObj.StrSerial != "")
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        list = list.Where(b => b.SerialNumber.Contains(sortObj.StrSerial)).OrderByDescending(d => d.SerialNumber).ToList();
                    }
                    else
                    {
                        list = list.Where(b => b.SerialNumber.Contains(sortObj.StrSerial)).OrderBy(d => d.SerialNumber).ToList();
                    }
                }
                if (sortObj.StrModel != "")
                {
                    if (sortObj.SortStatus == "descending")
                        list = list.Where(b => b.ModelNumber.Contains(sortObj.StrModel)).OrderByDescending(d => d.ModelNumber).ToList();
                    else
                        list = list.Where(b => b.ModelNumber.Contains(sortObj.StrModel)).OrderBy(d => d.ModelNumber).ToList();
                }
                if (sortObj.StrWorkOrderNumber != "")
                {
                    if (sortObj.SortStatus == "descending")
                        list = list.Where(b => b.WorkOrderNumber.Contains(sortObj.StrWorkOrderNumber)).OrderByDescending(d => d.WorkOrderNumber).ToList();
                    else
                        list = list.Where(b => b.WorkOrderNumber.Contains(sortObj.StrWorkOrderNumber)).OrderBy(d => d.WorkOrderNumber).ToList();
                }
                if (sortObj.StrSubject != "")
                {
                    if (sortObj.SortStatus == "descending")
                        list = list.Where(b => b.Subject.Contains(sortObj.StrSubject)).OrderByDescending(d => d.Subject).ToList();

                    else
                        list = list.Where(b => b.Subject.Contains(sortObj.StrSubject)).OrderBy(d => d.Subject).ToList();
                }
                else
                {
                    if (sortObj.SortStatus == "descending")
                        list = list.OrderByDescending(d => d.BarCode).ToList();
                    else
                        list = list.OrderBy(d => d.BarCode).ToList();
                }
            }
            if (sortObj.SerialNumber != "")
            {
                if (sortObj.StatusId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        list = list.Where(b => b.StatusId == sortObj.StatusId).OrderByDescending(d => d.StatusId).ToList();
                    }
                    else
                    {
                        list = list.Where(b => b.StatusId == sortObj.StatusId).OrderBy(d => d.StatusId).ToList();
                    }
                }
                if (sortObj.StrSerial != "")
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        list = list.Where(b => b.SerialNumber.Contains(sortObj.StrSerial)).OrderByDescending(d => d.SerialNumber).ToList();
                    }
                    else
                    {
                        list = list.Where(b => b.SerialNumber.Contains(sortObj.StrSerial)).OrderBy(d => d.SerialNumber).ToList();
                    }
                }
                if (sortObj.StrRequestSubject != "")
                {
                    if (sortObj.SortStatus == "descending")
                        list = list.Where(b => b.RequestSubject.Contains(sortObj.StrRequestSubject)).OrderByDescending(d => d.RequestSubject).ToList();

                    else
                        list = list.Where(b => b.RequestSubject.Contains(sortObj.StrRequestSubject)).OrderBy(d => d.RequestSubject).ToList();
                }
                if (sortObj.StrBarCode != "")
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        list = list.Where(b => b.BarCode.Contains(sortObj.StrBarCode)).OrderByDescending(d => d.BarCode).ToList();
                    }
                    else
                    {
                        list = list.Where(b => b.BarCode.Contains(sortObj.StrBarCode)).OrderBy(d => d.BarCode).ToList();
                    }
                }

                if (sortObj.StrModel != "")
                {
                    if (sortObj.SortStatus == "descending")
                        list = list.Where(b => b.ModelNumber.Contains(sortObj.StrModel)).OrderByDescending(d => d.ModelNumber).ToList();
                    else
                        list = list.Where(b => b.ModelNumber.Contains(sortObj.StrModel)).OrderBy(d => d.ModelNumber).ToList();
                }
                if (sortObj.StrWorkOrderNumber != "")
                {
                    if (sortObj.SortStatus == "descending")
                        list = list.Where(b => b.WorkOrderNumber.Contains(sortObj.StrWorkOrderNumber)).OrderByDescending(d => d.WorkOrderNumber).ToList();
                    else
                        list = list.Where(b => b.WorkOrderNumber.Contains(sortObj.StrWorkOrderNumber)).OrderBy(d => d.WorkOrderNumber).ToList();
                }
                if (sortObj.StrSubject != "")
                {
                    if (sortObj.SortStatus == "descending")
                        list = list.Where(b => b.Subject.Contains(sortObj.StrSubject)).OrderByDescending(d => d.Subject).ToList();

                    else
                        list = list.Where(b => b.Subject.Contains(sortObj.StrSubject)).OrderBy(d => d.Subject).ToList();
                }
                else
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        list = list.OrderByDescending(d => d.SerialNumber).ToList();
                    }
                    else
                        list = list.OrderBy(d => d.SerialNumber).ToList();
                }
            }
            if (sortObj.ModelNumber != "")
            {
                if (sortObj.StatusId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        list = list.Where(b => b.StatusId == sortObj.StatusId).OrderByDescending(d => d.StatusId).ToList();
                    }
                    else
                    {
                        list = list.Where(b => b.StatusId == sortObj.StatusId).OrderBy(d => d.StatusId).ToList();
                    }
                }
                if (sortObj.StrModel != "")
                {
                    if (sortObj.SortStatus == "descending")
                        list = list.Where(b => b.ModelNumber.Contains(sortObj.StrModel)).OrderByDescending(d => d.ModelNumber).ToList();
                    else
                        list = list.Where(b => b.ModelNumber.Contains(sortObj.StrModel)).OrderBy(d => d.ModelNumber).ToList();
                }
                if (sortObj.StrRequestSubject != "")
                {
                    if (sortObj.SortStatus == "descending")
                        list = list.Where(b => b.RequestSubject.Contains(sortObj.StrRequestSubject)).OrderByDescending(d => d.RequestSubject).ToList();

                    else
                        list = list.Where(b => b.RequestSubject.Contains(sortObj.StrRequestSubject)).OrderBy(d => d.RequestSubject).ToList();
                }
                if (sortObj.StrBarCode != "")
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        list = list.Where(b => b.BarCode.Contains(sortObj.StrBarCode)).OrderByDescending(d => d.BarCode).ToList();
                    }
                    else
                    {
                        list = list.Where(b => b.BarCode.Contains(sortObj.StrBarCode)).OrderBy(d => d.BarCode).ToList();
                    }
                }
                if (sortObj.StrSerial != "")
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        list = list.Where(b => b.SerialNumber.Contains(sortObj.StrSerial)).OrderByDescending(d => d.SerialNumber).ToList();
                    }
                    else
                    {
                        list = list.Where(b => b.SerialNumber.Contains(sortObj.StrSerial)).OrderBy(d => d.SerialNumber).ToList();
                    }
                }

                if (sortObj.StrWorkOrderNumber != "")
                {
                    if (sortObj.SortStatus == "descending")
                        list = list.Where(b => b.WorkOrderNumber.Contains(sortObj.StrWorkOrderNumber)).OrderByDescending(d => d.WorkOrderNumber).ToList();
                    else
                        list = list.Where(b => b.WorkOrderNumber.Contains(sortObj.StrWorkOrderNumber)).OrderBy(d => d.WorkOrderNumber).ToList();
                }
                if (sortObj.StrSubject != "")
                {
                    if (sortObj.SortStatus == "descending")
                        list = list.Where(b => b.Subject.Contains(sortObj.StrSubject)).OrderByDescending(d => d.Subject).ToList();

                    else
                        list = list.Where(b => b.Subject.Contains(sortObj.StrSubject)).OrderBy(d => d.Subject).ToList();
                }
                else
                {
                    if (sortObj.SortStatus == "descending")
                        list = list.OrderByDescending(d => d.ModelNumber).ToList();
                    else
                        list = list.OrderBy(d => d.ModelNumber).ToList();
                }
            }
            if (sortObj.WorkOrderNumber != "")
            {
                if (sortObj.StatusId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        list = list.Where(b => b.StatusId == sortObj.StatusId).OrderByDescending(d => d.StatusId).ToList();
                    }
                    else
                    {
                        list = list.Where(b => b.StatusId == sortObj.StatusId).OrderBy(d => d.StatusId).ToList();
                    }
                }
                if (sortObj.StrWorkOrderNumber != "")
                {
                    if (sortObj.SortStatus == "descending")
                        list = list.Where(b => b.WorkOrderNumber.Contains(sortObj.StrWorkOrderNumber)).OrderByDescending(d => d.WorkOrderNumber).ToList();
                    else
                        list = list.Where(b => b.WorkOrderNumber.Contains(sortObj.StrWorkOrderNumber)).OrderBy(d => d.WorkOrderNumber).ToList();
                }
                if (sortObj.StrRequestSubject != "")
                {
                    if (sortObj.SortStatus == "descending")
                        list = list.Where(b => b.RequestSubject.Contains(sortObj.StrRequestSubject)).OrderByDescending(d => d.RequestSubject).ToList();

                    else
                        list = list.Where(b => b.RequestSubject.Contains(sortObj.StrRequestSubject)).OrderBy(d => d.RequestSubject).ToList();
                }
                if (sortObj.StrBarCode != "")
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        list = list.Where(b => b.BarCode.Contains(sortObj.StrBarCode)).OrderByDescending(d => d.BarCode).ToList();
                    }
                    else
                    {
                        list = list.Where(b => b.BarCode.Contains(sortObj.StrBarCode)).OrderBy(d => d.BarCode).ToList();
                    }
                }
                if (sortObj.StrSerial != "")
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        list = list.Where(b => b.SerialNumber.Contains(sortObj.StrSerial)).OrderByDescending(d => d.SerialNumber).ToList();
                    }
                    else
                    {
                        list = list.Where(b => b.SerialNumber.Contains(sortObj.StrSerial)).OrderBy(d => d.SerialNumber).ToList();
                    }
                }
                if (sortObj.StrModel != "")
                {
                    if (sortObj.SortStatus == "descending")
                        list = list.Where(b => b.ModelNumber.Contains(sortObj.StrModel)).OrderByDescending(d => d.ModelNumber).ToList();
                    else
                        list = list.Where(b => b.ModelNumber.Contains(sortObj.StrModel)).OrderBy(d => d.ModelNumber).ToList();
                }

                if (sortObj.StrSubject != "")
                {
                    if (sortObj.SortStatus == "descending")
                        list = list.Where(b => b.Subject.Contains(sortObj.StrSubject)).OrderByDescending(d => d.Subject).ToList();

                    else
                        list = list.Where(b => b.Subject.Contains(sortObj.StrSubject)).OrderBy(d => d.Subject).ToList();
                }
                else
                {
                    if (sortObj.SortStatus == "descending")
                        list = list.OrderByDescending(d => d.WorkOrderNumber).ToList();
                    else
                        list = list.OrderBy(d => d.WorkOrderNumber).ToList();
                }
            }
            if (sortObj.Subject != "")
            {
                if (sortObj.StatusId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        list = list.Where(b => b.StatusId == sortObj.StatusId).OrderByDescending(d => d.StatusId).ToList();
                    }
                    else
                    {
                        list = list.Where(b => b.StatusId == sortObj.StatusId).OrderBy(d => d.StatusId).ToList();
                    }
                }
                if (sortObj.StrSubject != "")
                {
                    if (sortObj.SortStatus == "descending")
                        list = list.Where(b => b.Subject.Contains(sortObj.StrSubject)).OrderByDescending(d => d.Subject).ToList();

                    else
                        list = list.Where(b => b.Subject.Contains(sortObj.StrSubject)).OrderBy(d => d.Subject).ToList();
                }

                if (sortObj.StrRequestSubject != "")
                {
                    if (sortObj.SortStatus == "descending")
                        list = list.Where(b => b.RequestSubject.Contains(sortObj.StrRequestSubject)).OrderByDescending(d => d.RequestSubject).ToList();

                    else
                        list = list.Where(b => b.RequestSubject.Contains(sortObj.StrRequestSubject)).OrderBy(d => d.RequestSubject).ToList();
                }
                if (sortObj.StrBarCode != "")
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        list = list.Where(b => b.BarCode.Contains(sortObj.StrBarCode)).OrderByDescending(d => d.BarCode).ToList();
                    }
                    else
                    {
                        list = list.Where(b => b.BarCode.Contains(sortObj.StrBarCode)).OrderBy(d => d.BarCode).ToList();
                    }
                }
                if (sortObj.StrSerial != "")
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        list = list.Where(b => b.SerialNumber.Contains(sortObj.StrSerial)).OrderByDescending(d => d.SerialNumber).ToList();
                    }
                    else
                    {
                        list = list.Where(b => b.SerialNumber.Contains(sortObj.StrSerial)).OrderBy(d => d.SerialNumber).ToList();
                    }
                }
                if (sortObj.StrModel != "")
                {
                    if (sortObj.SortStatus == "descending")
                        list = list.Where(b => b.ModelNumber.Contains(sortObj.StrModel)).OrderByDescending(d => d.ModelNumber).ToList();
                    else
                        list = list.Where(b => b.ModelNumber.Contains(sortObj.StrModel)).OrderBy(d => d.ModelNumber).ToList();
                }
                if (sortObj.StrWorkOrderNumber != "")
                {
                    if (sortObj.SortStatus == "descending")
                        list = list.Where(b => b.WorkOrderNumber.Contains(sortObj.StrWorkOrderNumber)).OrderByDescending(d => d.WorkOrderNumber).ToList();
                    else
                        list = list.Where(b => b.WorkOrderNumber.Contains(sortObj.StrWorkOrderNumber)).OrderBy(d => d.WorkOrderNumber).ToList();
                }

                else
                {
                    if (sortObj.SortStatus == "descending")
                        list = list.OrderByDescending(d => d.Subject).ToList();
                    else
                        list = list.OrderBy(d => d.Subject).ToList();
                }
            }
            if (sortObj.RequestSubject != "")
            {
                if (sortObj.StatusId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        list = list.Where(b => b.WorkOrderStatusId == sortObj.StatusId).OrderByDescending(d => d.RequestSubject).ToList();
                    }
                    else
                    {
                        list = list.Where(b => b.WorkOrderStatusId == sortObj.StatusId).OrderBy(d => d.RequestSubject).ToList();
                    }
                }
                if (sortObj.StrRequestSubject != "")
                {
                    if (sortObj.SortStatus == "descending")
                        list = list.Where(b => b.RequestSubject.Contains(sortObj.StrRequestSubject)).OrderByDescending(d => d.RequestSubject).ToList();

                    else
                        list = list.Where(b => b.RequestSubject.Contains(sortObj.StrRequestSubject)).OrderBy(d => d.RequestSubject).ToList();
                }
                if (sortObj.StrBarCode != "")
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        list = list.Where(b => b.BarCode.Contains(sortObj.StrBarCode)).OrderByDescending(d => d.BarCode).ToList();
                    }
                    else
                    {
                        list = list.Where(b => b.BarCode.Contains(sortObj.StrBarCode)).OrderBy(d => d.BarCode).ToList();
                    }
                }
                if (sortObj.StrSerial != "")
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        list = list.Where(b => b.SerialNumber.Contains(sortObj.StrSerial)).OrderByDescending(d => d.SerialNumber).ToList();
                    }
                    else
                    {
                        list = list.Where(b => b.SerialNumber.Contains(sortObj.StrSerial)).OrderBy(d => d.SerialNumber).ToList();
                    }
                }
                if (sortObj.StrModel != "")
                {
                    if (sortObj.SortStatus == "descending")
                        list = list.Where(b => b.ModelNumber.Contains(sortObj.StrModel)).OrderByDescending(d => d.ModelNumber).ToList();
                    else
                        list = list.Where(b => b.ModelNumber.Contains(sortObj.StrModel)).OrderBy(d => d.ModelNumber).ToList();
                }
                if (sortObj.StrWorkOrderNumber != "")
                {
                    if (sortObj.SortStatus == "descending")
                        list = list.Where(b => b.WorkOrderNumber.Contains(sortObj.StrWorkOrderNumber)).OrderByDescending(d => d.WorkOrderNumber).ToList();
                    else
                        list = list.Where(b => b.WorkOrderNumber.Contains(sortObj.StrWorkOrderNumber)).OrderBy(d => d.WorkOrderNumber).ToList();
                }
                if (sortObj.StrSubject != "")
                {
                    if (sortObj.SortStatus == "descending")
                        list = list.Where(b => b.Subject.Contains(sortObj.StrSubject)).OrderByDescending(d => d.Subject).ToList();

                    else
                        list = list.Where(b => b.Subject.Contains(sortObj.StrSubject)).OrderBy(d => d.Subject).ToList();
                }
                else
                {
                    if (sortObj.SortStatus == "descending")
                        list = list.OrderByDescending(d => d.RequestSubject).ToList();
                    else
                        list = list.OrderBy(d => d.RequestSubject).ToList();
                }
            }
            if (sortObj.CreatedBy != "")
            {

                if (sortObj.StatusId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        list = list.Where(b => b.StatusId == sortObj.StatusId).OrderByDescending(d => d.StatusId).ToList();
                    }
                    else
                    {
                        list = list.Where(b => b.StatusId == sortObj.StatusId).OrderBy(d => d.StatusId).ToList();
                    }
                }
                if (sortObj.StrRequestSubject != "")
                {
                    if (sortObj.SortStatus == "descending")
                        list = list.Where(b => b.RequestSubject.Contains(sortObj.StrRequestSubject)).OrderByDescending(d => d.RequestSubject).ToList();

                    else
                        list = list.Where(b => b.RequestSubject.Contains(sortObj.StrRequestSubject)).OrderBy(d => d.RequestSubject).ToList();
                }
                if (sortObj.StrBarCode != "")
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        list = list.Where(b => b.BarCode.Contains(sortObj.StrBarCode)).OrderByDescending(d => d.BarCode).ToList();
                    }
                    else
                    {
                        list = list.Where(b => b.BarCode.Contains(sortObj.StrBarCode)).OrderBy(d => d.BarCode).ToList();
                    }
                }
                if (sortObj.StrSerial != "")
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        list = list.Where(b => b.SerialNumber.Contains(sortObj.StrSerial)).OrderByDescending(d => d.SerialNumber).ToList();
                    }
                    else
                    {
                        list = list.Where(b => b.SerialNumber.Contains(sortObj.StrSerial)).OrderBy(d => d.SerialNumber).ToList();
                    }
                }
                if (sortObj.StrModel != "")
                {
                    if (sortObj.SortStatus == "descending")
                        list = list.Where(b => b.ModelNumber.Contains(sortObj.StrModel)).OrderByDescending(d => d.ModelNumber).ToList();
                    else
                        list = list.Where(b => b.ModelNumber.Contains(sortObj.StrModel)).OrderBy(d => d.ModelNumber).ToList();
                }
                if (sortObj.StrWorkOrderNumber != "")
                {
                    if (sortObj.SortStatus == "descending")
                        list = list.Where(b => b.WorkOrderNumber.Contains(sortObj.StrWorkOrderNumber)).OrderByDescending(d => d.WorkOrderNumber).ToList();
                    else
                        list = list.Where(b => b.WorkOrderNumber.Contains(sortObj.StrWorkOrderNumber)).OrderBy(d => d.WorkOrderNumber).ToList();
                }
                if (sortObj.StrSubject != "")
                {
                    if (sortObj.SortStatus == "descending")
                        list = list.Where(b => b.Subject.Contains(sortObj.StrSubject)).OrderByDescending(d => d.Subject).ToList();

                    else
                        list = list.Where(b => b.Subject.Contains(sortObj.StrSubject)).OrderBy(d => d.Subject).ToList();
                }
                else
                {
                    if (sortObj.SortStatus == "descending")
                        list = list.OrderByDescending(d => d.CreatedBy).ToList();
                    else
                        list = list.OrderBy(d => d.CreatedBy).ToList();
                }
            }
            if (sortObj.CreationDate != "")
            {

                if (sortObj.StatusId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        list = list.Where(b => b.StatusId == sortObj.StatusId).OrderByDescending(d => d.StatusId).ToList();
                    }
                    else
                    {
                        list = list.Where(b => b.StatusId == sortObj.StatusId).OrderBy(d => d.StatusId).ToList();
                    }
                }
                if (sortObj.StrRequestSubject != "")
                {
                    if (sortObj.SortStatus == "descending")
                        list = list.Where(b => b.RequestSubject.Contains(sortObj.StrRequestSubject)).OrderByDescending(d => d.RequestSubject).ToList();

                    else
                        list = list.Where(b => b.RequestSubject.Contains(sortObj.StrRequestSubject)).OrderBy(d => d.RequestSubject).ToList();
                }
                if (sortObj.StrBarCode != "")
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        list = list.Where(b => b.BarCode.Contains(sortObj.StrBarCode)).OrderByDescending(d => d.BarCode).ToList();
                    }
                    else
                    {
                        list = list.Where(b => b.BarCode.Contains(sortObj.StrBarCode)).OrderBy(d => d.BarCode).ToList();
                    }
                }
                if (sortObj.StrSerial != "")
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        list = list.Where(b => b.SerialNumber.Contains(sortObj.StrSerial)).OrderByDescending(d => d.SerialNumber).ToList();
                    }
                    else
                    {
                        list = list.Where(b => b.SerialNumber.Contains(sortObj.StrSerial)).OrderBy(d => d.SerialNumber).ToList();
                    }
                }
                if (sortObj.StrModel != "")
                {
                    if (sortObj.SortStatus == "descending")
                        list = list.Where(b => b.ModelNumber.Contains(sortObj.StrModel)).OrderByDescending(d => d.ModelNumber).ToList();
                    else
                        list = list.Where(b => b.ModelNumber.Contains(sortObj.StrModel)).OrderBy(d => d.ModelNumber).ToList();
                }
                if (sortObj.StrWorkOrderNumber != "")
                {
                    if (sortObj.SortStatus == "descending")
                        list = list.Where(b => b.WorkOrderNumber.Contains(sortObj.StrWorkOrderNumber)).OrderByDescending(d => d.WorkOrderNumber).ToList();
                    else
                        list = list.Where(b => b.WorkOrderNumber.Contains(sortObj.StrWorkOrderNumber)).OrderBy(d => d.WorkOrderNumber).ToList();
                }
                if (sortObj.StrSubject != "")
                {
                    if (sortObj.SortStatus == "descending")
                        list = list.Where(b => b.Subject.Contains(sortObj.StrSubject)).OrderByDescending(d => d.Subject).ToList();

                    else
                        list = list.Where(b => b.Subject.Contains(sortObj.StrSubject)).OrderBy(d => d.Subject).ToList();
                }
                else
                {
                    if (sortObj.SortStatus == "descending")
                        list = list.OrderByDescending(d => d.CreationDate).ToList();
                    else
                        list = list.OrderBy(d => d.CreationDate).ToList();
                }
            }
            if (sortObj.ClosedDate != "")
            {
                if (sortObj.StatusId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        list = list.Where(b => b.StatusId == sortObj.StatusId).OrderByDescending(d => d.StatusId).ToList();
                    }
                    else
                    {
                        list = list.Where(b => b.StatusId == sortObj.StatusId).OrderBy(d => d.StatusId).ToList();
                    }
                }
                if (sortObj.StrRequestSubject != "")
                {
                    if (sortObj.SortStatus == "descending")
                        list = list.Where(b => b.RequestSubject.Contains(sortObj.StrRequestSubject)).OrderByDescending(d => d.RequestSubject).ToList();

                    else
                        list = list.Where(b => b.RequestSubject.Contains(sortObj.StrRequestSubject)).OrderBy(d => d.RequestSubject).ToList();
                }
                if (sortObj.StrBarCode != "")
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        list = list.Where(b => b.BarCode.Contains(sortObj.StrBarCode)).OrderByDescending(d => d.BarCode).ToList();
                    }
                    else
                    {
                        list = list.Where(b => b.BarCode.Contains(sortObj.StrBarCode)).OrderBy(d => d.BarCode).ToList();
                    }
                }
                if (sortObj.StrSerial != "")
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        list = list.Where(b => b.SerialNumber.Contains(sortObj.StrSerial)).OrderByDescending(d => d.SerialNumber).ToList();
                    }
                    else
                    {
                        list = list.Where(b => b.SerialNumber.Contains(sortObj.StrSerial)).OrderBy(d => d.SerialNumber).ToList();
                    }
                }
                if (sortObj.StrModel != "")
                {
                    if (sortObj.SortStatus == "descending")
                        list = list.Where(b => b.ModelNumber.Contains(sortObj.StrModel)).OrderByDescending(d => d.ModelNumber).ToList();
                    else
                        list = list.Where(b => b.ModelNumber.Contains(sortObj.StrModel)).OrderBy(d => d.ModelNumber).ToList();
                }
                if (sortObj.StrWorkOrderNumber != "")
                {
                    if (sortObj.SortStatus == "descending")
                        list = list.Where(b => b.WorkOrderNumber.Contains(sortObj.StrWorkOrderNumber)).OrderByDescending(d => d.WorkOrderNumber).ToList();
                    else
                        list = list.Where(b => b.WorkOrderNumber.Contains(sortObj.StrWorkOrderNumber)).OrderBy(d => d.WorkOrderNumber).ToList();
                }
                if (sortObj.StrSubject != "")
                {
                    if (sortObj.SortStatus == "descending")
                        list = list.Where(b => b.Subject.Contains(sortObj.StrSubject)).OrderByDescending(d => d.Subject).ToList();

                    else
                        list = list.Where(b => b.Subject.Contains(sortObj.StrSubject)).OrderBy(d => d.Subject).ToList();
                }
                else
                {
                    if (sortObj.SortStatus == "descending")
                        list = list.OrderByDescending(d => d.ClosedDate).ToList();
                    else
                        list = list.OrderBy(d => d.ClosedDate).ToList();
                }
            }
            if (sortObj.Note != "")
            {

                if (sortObj.StatusId != 0)
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        list = list.Where(b => b.StatusId == sortObj.StatusId).OrderByDescending(d => d.StatusId).ToList();
                    }
                    else
                    {
                        list = list.Where(b => b.StatusId == sortObj.StatusId).OrderBy(d => d.StatusId).ToList();
                    }
                }
                if (sortObj.StrRequestSubject != "")
                {
                    if (sortObj.SortStatus == "descending")
                        list = list.Where(b => b.RequestSubject.Contains(sortObj.StrRequestSubject)).OrderByDescending(d => d.RequestSubject).ToList();

                    else
                        list = list.Where(b => b.RequestSubject.Contains(sortObj.StrRequestSubject)).OrderBy(d => d.RequestSubject).ToList();
                }
                if (sortObj.StrBarCode != "")
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        list = list.Where(b => b.BarCode.Contains(sortObj.StrBarCode)).OrderByDescending(d => d.BarCode).ToList();
                    }
                    else
                    {
                        list = list.Where(b => b.BarCode.Contains(sortObj.StrBarCode)).OrderBy(d => d.BarCode).ToList();
                    }
                }
                if (sortObj.StrSerial != "")
                {
                    if (sortObj.SortStatus == "descending")
                    {
                        list = list.Where(b => b.SerialNumber.Contains(sortObj.StrSerial)).OrderByDescending(d => d.SerialNumber).ToList();
                    }
                    else
                    {
                        list = list.Where(b => b.SerialNumber.Contains(sortObj.StrSerial)).OrderBy(d => d.SerialNumber).ToList();
                    }
                }
                if (sortObj.StrModel != "")
                {
                    if (sortObj.SortStatus == "descending")
                        list = list.Where(b => b.ModelNumber.Contains(sortObj.StrModel)).OrderByDescending(d => d.ModelNumber).ToList();
                    else
                        list = list.Where(b => b.ModelNumber.Contains(sortObj.StrModel)).OrderBy(d => d.ModelNumber).ToList();
                }
                if (sortObj.StrWorkOrderNumber != "")
                {
                    if (sortObj.SortStatus == "descending")
                        list = list.Where(b => b.WorkOrderNumber.Contains(sortObj.StrWorkOrderNumber)).OrderByDescending(d => d.WorkOrderNumber).ToList();
                    else
                        list = list.Where(b => b.WorkOrderNumber.Contains(sortObj.StrWorkOrderNumber)).OrderBy(d => d.WorkOrderNumber).ToList();
                }
                if (sortObj.StrSubject != "")
                {
                    if (sortObj.SortStatus == "descending")
                        list = list.Where(b => b.Subject.Contains(sortObj.StrSubject)).OrderByDescending(d => d.Subject).ToList();

                    else
                        list = list.Where(b => b.Subject.Contains(sortObj.StrSubject)).OrderBy(d => d.Subject).ToList();
                }
                else
                {
                    if (sortObj.SortStatus == "descending")
                        list = list.OrderByDescending(d => d.Note).ToList();
                    else
                        list = list.OrderBy(d => d.Note).ToList();
                }
            }






            return list;
        }

        public List<IndexWorkOrderVM> GetLastRequestAndWorkOrderByAssetId(int assetId)
        {

            return _context.WorkOrders.Include(a => a.Request)
                                    .Include(a => a.Request.AssetDetail)
                                    .Include(a => a.Request.AssetDetail.Hospital)
                                .Where(a => a.Request.AssetDetailId == assetId)
                                .OrderByDescending(a => a.Request.RequestDate)
                                .ToList()
                                .Select(item => new IndexWorkOrderVM
                                {
                                    Id = item.Id,
                                    RequestId = item.Request.Id,
                                    WorkOrderNumber = item.WorkOrderNumber,
                                    Subject = item.Subject,
                                    RequestSubject = item.Request.Subject,
                                    RequestNumber = item.Request.RequestCode,
                                    CreationDate = item.CreationDate,
                                    HospitalId = item.Request.AssetDetail.HospitalId
                                }).ToList();
        }

        public int CountWorkOrdersByHospitalId(int hospitalId, string userId)
        {
            ApplicationUser UserObj = new ApplicationUser();
            ApplicationRole roleObj = new ApplicationRole();

            List<string> userRoleNames = new List<string>();
            var user = _context.ApplicationUser.Where(a => a.Id == userId).ToList();
            if (user.Count > 0)
            {
                UserObj = user[0];
                var roles = (from userRole in _context.UserRoles
                             join role in _context.ApplicationRole on userRole.RoleId equals role.Id
                             where userRole.UserId == userId
                             select role);
                foreach (var role in roles)
                {
                    userRoleNames.Add(role.Name);
                }
            }

            var lstWorkOrders = _context.WorkOrderTrackings
                 .Include(w => w.WorkOrder)
                                  .Include(w => w.WorkOrder.Request)
                                  .Include(w => w.WorkOrder.Request.AssetDetail)
                                  .Include(w => w.WorkOrder.Request.AssetDetail.Hospital)
                                  .Include(w => w.User).ToList().GroupBy(a => a.WorkOrder.RequestId).ToList();



            if (lstWorkOrders.Count > 0)
            {
                if (UserObj.GovernorateId == 0 && UserObj.CityId == 0 && UserObj.HospitalId == 0)
                {
                    lstWorkOrders = lstWorkOrders.ToList();
                }
                if (UserObj.GovernorateId > 0 && UserObj.CityId == 0 && UserObj.HospitalId == 0)
                {
                    lstWorkOrders = lstWorkOrders.Where(t => t.FirstOrDefault().WorkOrder.Request.AssetDetail.Hospital.GovernorateId == UserObj.GovernorateId).ToList();
                }
                if (UserObj.GovernorateId > 0 && UserObj.CityId > 0 && UserObj.HospitalId == 0)
                {
                    lstWorkOrders = lstWorkOrders.Where(t => t.FirstOrDefault().WorkOrder.Request.AssetDetail.Hospital.GovernorateId == UserObj.GovernorateId && t.FirstOrDefault().WorkOrder.Request.AssetDetail.Hospital.CityId == UserObj.CityId).ToList();
                }
                if (UserObj.OrganizationId > 0 && UserObj.SubOrganizationId == 0 && UserObj.HospitalId == 0)
                {
                    lstWorkOrders = lstWorkOrders.Where(t => t.FirstOrDefault().WorkOrder.Request.AssetDetail.Hospital.OrganizationId == UserObj.OrganizationId).ToList();
                }
                if (UserObj.OrganizationId > 0 && UserObj.SubOrganizationId > 0 && UserObj.HospitalId == 0)
                {
                    lstWorkOrders = lstWorkOrders.Where(t => t.FirstOrDefault().WorkOrder.Request.AssetDetail.Hospital.OrganizationId == UserObj.OrganizationId && t.FirstOrDefault().WorkOrder.Request.AssetDetail.Hospital.SubOrganizationId == UserObj.SubOrganizationId).ToList();
                }
                if (UserObj.HospitalId > 0)
                {
                    if (userRoleNames.Contains("Admin"))
                    {
                        lstWorkOrders = lstWorkOrders.Where(t => t.FirstOrDefault().WorkOrder.Request.AssetDetail.Hospital.Id == UserObj.HospitalId).ToList();
                    }
                    else if ((userRoleNames.Contains("AssetOwner") || userRoleNames.Contains("SRCreator")) && !userRoleNames.Contains("TLHospitalManager") && !userRoleNames.Contains("EngDepManager") && !userRoleNames.Contains("SRReviewer"))
                    {
                        lstWorkOrders = lstWorkOrders.Where(t => t.FirstOrDefault().WorkOrder.Request.AssetDetail.HospitalId == UserObj.HospitalId && t.FirstOrDefault().CreatedById == userId).ToList();
                    }
                    else if ((userRoleNames.Contains("AssetOwner") && userRoleNames.Contains("SRCreator")) && !userRoleNames.Contains("TLHospitalManager") && !userRoleNames.Contains("EngDepManager") && !userRoleNames.Contains("SRReviewer"))
                    {
                        lstWorkOrders = lstWorkOrders.Where(t => t.FirstOrDefault().WorkOrder.Request.AssetDetail.HospitalId == UserObj.HospitalId && t.FirstOrDefault().CreatedById == userId).ToList();
                    }
                    else if (userRoleNames.Contains("TLHospitalManager") && userRoleNames.Contains("EngDepManager") && userRoleNames.Contains("SRReviewer") && userRoleNames.Contains("SRCreator") && userRoleNames.Contains("AssetOwner") && userRoleNames.Contains("SRCreator"))
                    {
                        lstWorkOrders = lstWorkOrders.Where(t => t.FirstOrDefault().WorkOrder.Request.AssetDetail.HospitalId == UserObj.HospitalId).ToList();
                    }
                    else if (userRoleNames.Contains("TLHospitalManager") && !userRoleNames.Contains("EngDepManager") && !userRoleNames.Contains("SRReviewer") && !userRoleNames.Contains("SRCreator") && !userRoleNames.Contains("AssetOwner"))
                    {
                        lstWorkOrders = lstWorkOrders.Where(t => t.FirstOrDefault().WorkOrder.Request.AssetDetail.HospitalId == UserObj.HospitalId).ToList();
                    }
                    else if (userRoleNames.Contains("TLHospitalManager") && userRoleNames.Contains("EngDepManager") && !userRoleNames.Contains("SRReviewer") && !userRoleNames.Contains("SRCreator") && !userRoleNames.Contains("AssetOwner"))
                    {
                        lstWorkOrders = lstWorkOrders.Where(t => t.FirstOrDefault().WorkOrder.Request.AssetDetail.HospitalId == UserObj.HospitalId).ToList();
                    }
                    else if (userRoleNames.Contains("TLHospitalManager") && userRoleNames.Contains("EngDepManager") && userRoleNames.Contains("SRReviewer") && !userRoleNames.Contains("SRCreator") && !userRoleNames.Contains("AssetOwner"))
                    {
                        lstWorkOrders = lstWorkOrders.Where(t => t.FirstOrDefault().WorkOrder.Request.AssetDetail.HospitalId == UserObj.HospitalId).ToList();
                    }
                    else if (userRoleNames.Contains("TLHospitalManager") && userRoleNames.Contains("EngDepManager") && userRoleNames.Contains("SRReviewer") && userRoleNames.Contains("SRCreator") && !userRoleNames.Contains("AssetOwner"))
                    {
                        lstWorkOrders = lstWorkOrders.Where(t => t.FirstOrDefault().WorkOrder.Request.AssetDetail.HospitalId == UserObj.HospitalId).ToList(); ;
                    }
                    else if (userRoleNames.Contains("TLHospitalManager") && userRoleNames.Contains("EngDepManager") && userRoleNames.Contains("SRReviewer") && userRoleNames.Contains("SRCreator") && userRoleNames.Contains("AssetOwner"))
                    {
                        lstWorkOrders = lstWorkOrders.Where(t => t.FirstOrDefault().WorkOrder.Request.AssetDetail.HospitalId == UserObj.HospitalId).ToList();
                    }
                    else if (userRoleNames.Contains("EngDepManager"))
                    {
                        lstWorkOrders = lstWorkOrders.Where(t => t.FirstOrDefault().WorkOrder.Request.AssetDetail.HospitalId == UserObj.HospitalId).ToList();
                    }
                }
            }
            return lstWorkOrders.Count();

        }

        public List<IndexWorkOrderVM> GetLastRequestAndWorkOrderByAssetId(int assetId, int requestId)
        {
            return _context.WorkOrders.Include(a => a.Request)
                                     .Include(a => a.Request.AssetDetail)
                                     .Include(a => a.Request.AssetDetail.Hospital)
                                 .Where(a => a.Request.AssetDetailId == assetId && a.Request.Id == requestId)
                                 .OrderByDescending(a => a.Request.RequestDate)
                                 .ToList()
                                 .Select(item => new IndexWorkOrderVM
                                 {
                                     Id = item.Id,
                                     RequestId = item.Request.Id,
                                     WorkOrderNumber = item.WorkOrderNumber,
                                     Subject = item.Subject,
                                     RequestSubject = item.Request.Subject,
                                     RequestNumber = item.Request.RequestCode,
                                     CreationDate = item.CreationDate,
                                     HospitalId = item.Request.AssetDetail.HospitalId
                                 }).ToList();
        }

        public List<IndexWorkOrderVM> GetAllWorkOrdersByHospitalIdAndPaging(int? hospitalId, string userId, int statusId, int pageNumber, int pageSize)
        {

            List<IndexWorkOrderVM> lstModel = new List<IndexWorkOrderVM>();
            ApplicationUser UserObj = new ApplicationUser();
            List<string> lstRoleNames = new List<string>();

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
                    lstRoleNames.Add(name.Name);
                }
            }

            IQueryable<WorkOrder> lstWorkOrders = _context.WorkOrders
                      .Include(w => w.WorkOrderType)
                      .Include(w => w.WorkOrderPeriority)
                      .Include(w => w.Request)
                      .Include(w => w.Request.AssetDetail)
                      .Include(w => w.Request.AssetDetail.MasterAsset)
                      .Include(a => a.Request.AssetDetail.Hospital)
                      .Include(a => a.Request.AssetDetail.Hospital.Governorate)
                      .Include(a => a.Request.AssetDetail.Hospital.City)
                      .Include(a => a.Request.AssetDetail.Hospital.Organization)
                      .Include(a => a.Request.AssetDetail.Hospital.SubOrganization)
                      .Include(w => w.User)

                      .OrderByDescending(a => a.CreationDate)
                      .AsQueryable();

            var allWorkOrders = lstWorkOrders.ToList<WorkOrder>();

            if (hospitalId != 0)
            {
                allWorkOrders = allWorkOrders.Where(a => a.Request.AssetDetail.HospitalId == hospitalId).ToList();
            }
            else
            {
                allWorkOrders = allWorkOrders.ToList();
            }

            if (UserObj.GovernorateId == 0 && UserObj.CityId == 0 && UserObj.HospitalId == 0)
            {
                allWorkOrders = allWorkOrders.ToList();
            }
            if (UserObj.GovernorateId > 0 && UserObj.CityId == 0 && UserObj.HospitalId == 0)
            {
                allWorkOrders = allWorkOrders.Where(t => t.Request.AssetDetail.Hospital.GovernorateId == UserObj.GovernorateId).ToList();
            }
            if (UserObj.GovernorateId > 0 && UserObj.CityId > 0 && UserObj.HospitalId == 0)
            {
                allWorkOrders = allWorkOrders.Where(t => t.Request.AssetDetail.Hospital.CityId == UserObj.CityId).ToList();
            }
            if (UserObj.OrganizationId > 0 && UserObj.SubOrganizationId == 0 && UserObj.HospitalId == 0)
            {
                allWorkOrders = allWorkOrders.Where(t => t.Request.AssetDetail.Hospital.OrganizationId == UserObj.OrganizationId).ToList();
            }
            if (UserObj.OrganizationId > 0 && UserObj.SubOrganizationId > 0 && UserObj.HospitalId == 0)
            {
                allWorkOrders = allWorkOrders.Where(t => t.Request.AssetDetail.Hospital.SubOrganizationId == UserObj.SubOrganizationId).ToList();
            }
            if (UserObj.HospitalId > 0)
            {
                if (lstRoleNames.Contains("Admin"))
                {
                    allWorkOrders = allWorkOrders.ToList();
                }
                else if ((lstRoleNames.Contains("AssetOwner") || lstRoleNames.Contains("SRCreator")) && !lstRoleNames.Contains("TLHospitalManager") && !lstRoleNames.Contains("EngDepManager") && !lstRoleNames.Contains("SRReviewer"))
                {
                    allWorkOrders = allWorkOrders.Where(t => t.Request.AssetDetail.Hospital.Id == UserObj.HospitalId && t.CreatedById == userId).ToList();
                }
                else if ((lstRoleNames.Contains("AssetOwner") && lstRoleNames.Contains("SRCreator")) && !lstRoleNames.Contains("TLHospitalManager") && !lstRoleNames.Contains("EngDepManager") && !lstRoleNames.Contains("SRReviewer"))
                {
                    allWorkOrders = allWorkOrders.Where(t => t.Request.AssetDetail.Hospital.Id == UserObj.HospitalId && t.CreatedById == userId).ToList();
                }
                else if (lstRoleNames.Contains("TLHospitalManager") && lstRoleNames.Contains("EngDepManager") && lstRoleNames.Contains("SRReviewer") && lstRoleNames.Contains("SRCreator") && lstRoleNames.Contains("AssetOwner") && lstRoleNames.Contains("SRCreator"))
                {
                    allWorkOrders = allWorkOrders.Where(t => t.Request.AssetDetail.HospitalId == UserObj.HospitalId).ToList();
                }
                else if (lstRoleNames.Contains("TLHospitalManager") && !lstRoleNames.Contains("EngDepManager") && !lstRoleNames.Contains("SRReviewer") && !lstRoleNames.Contains("SRCreator") && !lstRoleNames.Contains("AssetOwner") && !lstRoleNames.Contains("SRCreator"))
                {
                    allWorkOrders = allWorkOrders.Where(t => t.Request.AssetDetail.HospitalId == UserObj.HospitalId).ToList();
                }
                else if (lstRoleNames.Contains("TLHospitalManager") && lstRoleNames.Contains("EngDepManager") && !lstRoleNames.Contains("SRReviewer") && !lstRoleNames.Contains("SRCreator") && !lstRoleNames.Contains("AssetOwner") && !lstRoleNames.Contains("SRCreator"))
                {
                    allWorkOrders = allWorkOrders.Where(t => t.Request.AssetDetail.HospitalId == UserObj.HospitalId).ToList();
                }
                else if (lstRoleNames.Contains("TLHospitalManager") && lstRoleNames.Contains("EngDepManager") && lstRoleNames.Contains("SRReviewer") && !lstRoleNames.Contains("SRCreator") && !lstRoleNames.Contains("AssetOwner") && !lstRoleNames.Contains("SRCreator"))
                {
                    allWorkOrders = allWorkOrders.Where(t => t.Request.AssetDetail.HospitalId == UserObj.HospitalId).ToList();
                }
                else if (lstRoleNames.Contains("TLHospitalManager") && lstRoleNames.Contains("EngDepManager") && lstRoleNames.Contains("SRReviewer") && lstRoleNames.Contains("SRCreator") && !lstRoleNames.Contains("AssetOwner") && !lstRoleNames.Contains("SRCreator"))
                {
                    allWorkOrders = allWorkOrders.Where(t => t.Request.AssetDetail.HospitalId == UserObj.HospitalId).ToList();
                }
                else if (lstRoleNames.Contains("TLHospitalManager") && lstRoleNames.Contains("EngDepManager") && lstRoleNames.Contains("SRReviewer") && lstRoleNames.Contains("SRCreator") && lstRoleNames.Contains("AssetOwner") && !lstRoleNames.Contains("SRCreator"))
                {
                    allWorkOrders = allWorkOrders.Where(t => t.Request.AssetDetail.HospitalId == UserObj.HospitalId).ToList();
                }
            }




            if (allWorkOrders.Count() > 0)
            {
                foreach (var item in allWorkOrders)
                {

                    if (statusId > 0)
                    {
                        var lstTracks = _context.WorkOrderTrackings.Include(a => a.WorkOrderStatus).Where(a => a.WorkOrderId == item.Id).OrderByDescending(a => a.CreationDate).ToList();
                        if (lstTracks.Count > 0)
                        {
                            var workOrderStatusId = lstTracks[0].WorkOrderStatusId;
                            if (workOrderStatusId == statusId)
                            {
                                IndexWorkOrderVM work = new IndexWorkOrderVM();
                                work.Id = item.Id;
                                work.AssetName = item.Request.AssetDetail.MasterAsset.Name;
                                work.AssetNameAr = item.Request.AssetDetail.MasterAsset.NameAr;
                                work.SerialNumber = item.Request.AssetDetail.SerialNumber;
                                work.BarCode = item.Request.AssetDetail.Barcode;
                                work.ModelNumber = item.Request.AssetDetail.MasterAsset.ModelNumber;
                                work.WorkOrderNumber = item.WorkOrderNumber;
                                work.ModelNumber = item.Request.AssetDetail.MasterAsset.ModelNumber;
                                work.Subject = item.Subject;
                                work.RequestSubject = item.Request.Subject;
                                work.CreationDate = item.CreationDate;
                                // work.Note = item.Note;
                                work.Note = lstTracks[0].Notes;
                                work.ActualStartDate = item.ActualStartDate;
                                work.ActualEndDate = item.ActualEndDate;
                                work.RequestId = item.RequestId != null ? (int)item.RequestId : 0;
                                work.HospitalId = item.Request.AssetDetail.HospitalId;
                                work.CreatedById = item.CreatedById;
                                work.CreatedBy = item.User.UserName;
                                work.BarCode = item.Request.AssetDetail.Barcode;
                                work.TypeName = item.WorkOrderType != null ? item.WorkOrderType.Name : "";
                                work.TypeNameAr = item.WorkOrderType != null ? item.WorkOrderType.NameAr : "";
                                work.PeriorityName = item.WorkOrderPeriority != null ? item.WorkOrderPeriority.Name : "";
                                work.PeriorityNameAr = item.WorkOrderPeriority != null ? item.WorkOrderPeriority.NameAr : "";
                                work.AssignedTo = lstTracks[0].AssignedTo;
                                work.WorkOrderStatusId = lstTracks[0].WorkOrderStatusId;

                                //  work.WorkOrderStatusId = lstTracks[0].WorkOrderStatusId;
                                work.StatusName = lstTracks[0].WorkOrderStatus.Name;
                                work.StatusNameAr = lstTracks[0].WorkOrderStatus.NameAr;
                                work.statusColor = lstTracks[0].WorkOrderStatus.Color;
                                work.statusIcon = lstTracks[0].WorkOrderStatus.Icon;

                                if (work.WorkOrderStatusId == 12)
                                {
                                    work.ClosedDate = lstTracks[0].CreationDate;
                                }


                                if (userId != null)
                                {
                                    var lstAssigned = _context.WorkOrderTrackings.Where(a => a.AssignedTo == userId && a.WorkOrderId == work.Id).ToList();
                                    if (lstAssigned.Count > 0)
                                    {
                                        work.AssignedTo = lstAssigned[0].AssignedTo;
                                    }
                                }
                                work.ListTracks = _context.WorkOrderTrackings.Include(a => a.WorkOrderStatus).Where(a => a.WorkOrderId == work.Id)
                                .ToList().Select(item => new LstWorkOrderFromTracking
                                {
                                    Id = item.Id,
                                    StatusName = item.WorkOrderStatus.Name,
                                    StatusNameAr = item.WorkOrderStatus.NameAr,
                                    CreationDate = DateTime.Parse(item.CreationDate.ToString())
                                }).ToList();

                                lstModel.Add(work);
                            }
                        }
                    }
                    else
                    {
                        var lstTracks = _context.WorkOrderTrackings.Include(a => a.WorkOrderStatus).Where(a => a.WorkOrderId == item.Id).OrderByDescending(a => a.CreationDate).ToList();

                        if (lstTracks.Count > 0)
                        {
                            var workOrderStatusId = lstTracks[0].WorkOrderStatusId;
                            IndexWorkOrderVM work = new IndexWorkOrderVM();
                            work.Id = item.Id;
                            work.AssetName = item.Request.AssetDetail.MasterAsset.Name;
                            work.AssetNameAr = item.Request.AssetDetail.MasterAsset.NameAr;
                            work.SerialNumber = item.Request.AssetDetail.SerialNumber;
                            work.BarCode = item.Request.AssetDetail.Barcode;
                            work.ModelNumber = item.Request.AssetDetail.MasterAsset.ModelNumber;
                            work.WorkOrderNumber = item.WorkOrderNumber;
                            work.ModelNumber = item.Request.AssetDetail.MasterAsset.ModelNumber;
                            work.Subject = item.Subject;
                            work.RequestSubject = item.Request.Subject;
                            work.CreationDate = item.CreationDate;
                            work.Note = lstTracks[0].Notes;
                            work.ActualStartDate = item.ActualStartDate;
                            work.ActualEndDate = item.ActualEndDate;
                            work.RequestId = item.RequestId != null ? (int)item.RequestId : 0;
                            work.HospitalId = item.Request.AssetDetail.HospitalId;
                            work.CreatedById = item.CreatedById;
                            work.CreatedBy = item.User.UserName;
                            work.BarCode = item.Request.AssetDetail.Barcode;
                            work.TypeName = item.WorkOrderType != null ? item.WorkOrderType.Name : "";
                            work.TypeNameAr = item.WorkOrderType != null ? item.WorkOrderType.NameAr : "";
                            work.PeriorityName = item.WorkOrderPeriority != null ? item.WorkOrderPeriority.Name : "";
                            work.PeriorityNameAr = item.WorkOrderPeriority != null ? item.WorkOrderPeriority.NameAr : "";
                            work.AssignedTo = lstTracks[0].AssignedTo;
                            work.WorkOrderStatusId = lstTracks[0].WorkOrderStatusId;
                            // work.WorkOrderStatusId = lstTracks[0].WorkOrderStatusId;
                            work.StatusName = lstTracks[0].WorkOrderStatus.Name;
                            work.StatusNameAr = lstTracks[0].WorkOrderStatus.NameAr;
                            work.statusColor = lstTracks[0].WorkOrderStatus.Color;
                            work.statusIcon = lstTracks[0].WorkOrderStatus.Icon;

                            if (work.WorkOrderStatusId == 12)
                            {
                                work.ClosedDate = lstTracks[0].CreationDate;
                            }

                            if (userId != null)
                            {
                                var lstAssigned = _context.WorkOrderTrackings.Where(a => a.AssignedTo == userId && a.WorkOrderId == work.Id).ToList();
                                if (lstAssigned.Count > 0)
                                {
                                    work.AssignedTo = lstAssigned[0].AssignedTo;
                                }
                            }
                            work.ListTracks = _context.WorkOrderTrackings.Include(a => a.WorkOrderStatus).Where(a => a.WorkOrderId == work.Id)
                            .ToList().Select(item => new LstWorkOrderFromTracking
                            {
                                Id = item.Id,
                                StatusName = item.WorkOrderStatus.Name,
                                StatusNameAr = item.WorkOrderStatus.NameAr,
                                CreationDate = DateTime.Parse(item.CreationDate.ToString())
                            }).ToList();

                            lstModel.Add(work);
                        }
                    }
                }
            }

            var workOrdersPerPage = lstModel.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            return workOrdersPerPage.ToList();


        }

        public IndexWorkOrderVM2 GetAllWorkOrdersByHospitalIdAndPaging2(int? hospitalId, string userId, int statusId, int pageNumber, int pageSize)
        {
            IndexWorkOrderVM2 mainClass = new IndexWorkOrderVM2();
            List<IndexWorkOrderVM2.GetData> lstModel = new List<IndexWorkOrderVM2.GetData>();
            ApplicationUser UserObj = new ApplicationUser();
            List<string> lstRoleNames = new List<string>();

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
                    lstRoleNames.Add(name.Name);
                }
            }

            IQueryable<WorkOrder> lstWorkOrders = _context.WorkOrders
                      .Include(w => w.WorkOrderType)
                      .Include(w => w.WorkOrderPeriority)
                      .Include(w => w.Request)
                      .Include(w => w.Request.AssetDetail)
                      .Include(w => w.Request.AssetDetail.MasterAsset)
                      .Include(a => a.Request.AssetDetail.Hospital)
                      .Include(a => a.Request.AssetDetail.Hospital.Governorate)
                      .Include(a => a.Request.AssetDetail.Hospital.City)
                      .Include(a => a.Request.AssetDetail.Hospital.Organization)
                      .Include(a => a.Request.AssetDetail.Hospital.SubOrganization)
                      .Include(w => w.User)

                      .OrderByDescending(a => a.CreationDate)
                      .AsQueryable();

            var allWorkOrders = lstWorkOrders.ToList<WorkOrder>();

            if (hospitalId != 0)
            {
                allWorkOrders = allWorkOrders.Where(a => a.Request.AssetDetail.HospitalId == hospitalId).ToList();
            }
            else
            {
                allWorkOrders = allWorkOrders.ToList();
            }

            if (UserObj.GovernorateId == 0 && UserObj.CityId == 0 && UserObj.HospitalId == 0)
            {
                allWorkOrders = allWorkOrders.ToList();
            }
            if (UserObj.GovernorateId > 0 && UserObj.CityId == 0 && UserObj.HospitalId == 0)
            {
                allWorkOrders = allWorkOrders.Where(t => t.Request.AssetDetail.Hospital.GovernorateId == UserObj.GovernorateId).ToList();
            }
            if (UserObj.GovernorateId > 0 && UserObj.CityId > 0 && UserObj.HospitalId == 0)
            {
                allWorkOrders = allWorkOrders.Where(t => t.Request.AssetDetail.Hospital.CityId == UserObj.CityId).ToList();
            }
            if (UserObj.OrganizationId > 0 && UserObj.SubOrganizationId == 0 && UserObj.HospitalId == 0)
            {
                allWorkOrders = allWorkOrders.Where(t => t.Request.AssetDetail.Hospital.OrganizationId == UserObj.OrganizationId).ToList();
            }
            if (UserObj.OrganizationId > 0 && UserObj.SubOrganizationId > 0 && UserObj.HospitalId == 0)
            {
                allWorkOrders = allWorkOrders.Where(t => t.Request.AssetDetail.Hospital.SubOrganizationId == UserObj.SubOrganizationId).ToList();
            }
            //if (UserObj.HospitalId > 0)
            //{
            //    if (lstRoleNames.Contains("Admin"))
            //    {
            //        allWorkOrders = allWorkOrders.ToList();
            //    }
            //    if (lstRoleNames.Contains("TLHospitalManager"))
            //    {
            //        allWorkOrders = allWorkOrders.Where(t => t.Request.AssetDetail.Hospital.Id == UserObj.HospitalId).ToList();
            //    }
            //    if (lstRoleNames.Contains("EngDepManager") && lstRoleNames.Contains("Eng"))
            //    {
            //        allWorkOrders = allWorkOrders.Where(t => t.Request.AssetDetail.Hospital.Id == UserObj.HospitalId).ToList();
            //    }
            //    if (!lstRoleNames.Contains("EngDepManager") && lstRoleNames.Contains("Eng"))
            //    {
            //        allWorkOrders = allWorkOrders.Where(t => t.Request.AssetDetail.Hospital.Id == UserObj.HospitalId && t.CreatedById == userId).ToList();
            //    }
            //    if (!lstRoleNames.Contains("EngDepManager") && !lstRoleNames.Contains("Eng"))
            //    {
            //        allWorkOrders = allWorkOrders.Where(t => t.Request.AssetDetail.Hospital.Id == UserObj.HospitalId).ToList();
            //    }
            //    if (lstRoleNames.Contains("AssetOwner"))
            //    {
            //        allWorkOrders = allWorkOrders.Where(t => t.Request.AssetDetail.Hospital.Id == UserObj.HospitalId && t.CreatedById == userId).ToList();
            //    }
            //    if (lstRoleNames.Contains("EngDepManager"))
            //    {
            //        allWorkOrders = allWorkOrders.Where(t => t.Request.AssetDetail.HospitalId == UserObj.HospitalId).ToList();
            //    }
            //}
            if (UserObj.HospitalId > 0)
            {
                if (lstRoleNames.Contains("Admin"))
                {
                    allWorkOrders = allWorkOrders.ToList();
                }
                else if ((lstRoleNames.Contains("AssetOwner") || lstRoleNames.Contains("SRCreator")) && !lstRoleNames.Contains("TLHospitalManager") && !lstRoleNames.Contains("EngDepManager") && !lstRoleNames.Contains("SRReviewer"))
                {
                    allWorkOrders = allWorkOrders.Where(t => t.Request.AssetDetail.Hospital.Id == UserObj.HospitalId && t.CreatedById == userId).ToList();
                }
                else if ((lstRoleNames.Contains("AssetOwner") && lstRoleNames.Contains("SRCreator")) && !lstRoleNames.Contains("TLHospitalManager") && !lstRoleNames.Contains("EngDepManager") && !lstRoleNames.Contains("SRReviewer"))
                {
                    allWorkOrders = allWorkOrders.Where(t => t.Request.AssetDetail.Hospital.Id == UserObj.HospitalId && t.CreatedById == userId).ToList();
                }
                else if (lstRoleNames.Contains("TLHospitalManager") && lstRoleNames.Contains("EngDepManager") && lstRoleNames.Contains("SRReviewer") && lstRoleNames.Contains("SRCreator") && lstRoleNames.Contains("AssetOwner") && lstRoleNames.Contains("SRCreator"))
                {
                    allWorkOrders = allWorkOrders.Where(t => t.Request.AssetDetail.HospitalId == UserObj.HospitalId).ToList();
                }
                else if (lstRoleNames.Contains("TLHospitalManager") && !lstRoleNames.Contains("EngDepManager") && !lstRoleNames.Contains("SRReviewer") && !lstRoleNames.Contains("SRCreator") && !lstRoleNames.Contains("AssetOwner") && !lstRoleNames.Contains("SRCreator"))
                {
                    allWorkOrders = allWorkOrders.Where(t => t.Request.AssetDetail.HospitalId == UserObj.HospitalId).ToList();
                }
                else if (lstRoleNames.Contains("TLHospitalManager") && lstRoleNames.Contains("EngDepManager") && !lstRoleNames.Contains("SRReviewer") && !lstRoleNames.Contains("SRCreator") && !lstRoleNames.Contains("AssetOwner") && !lstRoleNames.Contains("SRCreator"))
                {
                    allWorkOrders = allWorkOrders.Where(t => t.Request.AssetDetail.HospitalId == UserObj.HospitalId).ToList();
                }
                else if (lstRoleNames.Contains("TLHospitalManager") && lstRoleNames.Contains("EngDepManager") && lstRoleNames.Contains("SRReviewer") && !lstRoleNames.Contains("SRCreator") && !lstRoleNames.Contains("AssetOwner") && !lstRoleNames.Contains("SRCreator"))
                {
                    allWorkOrders = allWorkOrders.Where(t => t.Request.AssetDetail.HospitalId == UserObj.HospitalId).ToList();
                }
                else if (lstRoleNames.Contains("TLHospitalManager") && lstRoleNames.Contains("EngDepManager") && lstRoleNames.Contains("SRReviewer") && lstRoleNames.Contains("SRCreator") && !lstRoleNames.Contains("AssetOwner") && !lstRoleNames.Contains("SRCreator"))
                {
                    allWorkOrders = allWorkOrders.Where(t => t.Request.AssetDetail.HospitalId == UserObj.HospitalId).ToList();
                }
                else if (lstRoleNames.Contains("TLHospitalManager") && lstRoleNames.Contains("EngDepManager") && lstRoleNames.Contains("SRReviewer") && lstRoleNames.Contains("SRCreator") && lstRoleNames.Contains("AssetOwner") && !lstRoleNames.Contains("SRCreator"))
                {
                    allWorkOrders = allWorkOrders.Where(t => t.Request.AssetDetail.HospitalId == UserObj.HospitalId).ToList();
                }
            }

            if (allWorkOrders.Count() > 0)
            {
                foreach (var item in allWorkOrders)
                {

                    if (statusId > 0)
                    {
                        var lstTracks = _context.WorkOrderTrackings.Include(a => a.WorkOrderStatus).Where(a => a.WorkOrderId == item.Id).OrderByDescending(a => a.CreationDate).ToList();
                        if (lstTracks.Count > 0)
                        {
                            var workOrderStatusId = lstTracks[0].WorkOrderStatusId;
                            if (workOrderStatusId == statusId)
                            {
                                IndexWorkOrderVM2.GetData work = new IndexWorkOrderVM2.GetData();
                                work.Id = item.Id;
                                work.AssetName = item.Request.AssetDetail.MasterAsset.Name;
                                work.AssetNameAr = item.Request.AssetDetail.MasterAsset.NameAr;
                                work.SerialNumber = item.Request.AssetDetail.SerialNumber;
                                work.BarCode = item.Request.AssetDetail.Barcode;
                                work.ModelNumber = item.Request.AssetDetail.MasterAsset.ModelNumber;
                                work.WorkOrderNumber = item.WorkOrderNumber;
                                work.ModelNumber = item.Request.AssetDetail.MasterAsset.ModelNumber;
                                work.Subject = item.Subject;
                                work.RequestSubject = item.Request.Subject;
                                work.CreationDate = item.CreationDate;
                                // work.Note = item.Note;
                                work.Note = lstTracks[0].Notes;
                                work.ActualStartDate = item.ActualStartDate;
                                work.ActualEndDate = item.ActualEndDate;
                                work.RequestId = item.RequestId != null ? (int)item.RequestId : 0;
                                work.HospitalId = item.Request.AssetDetail.HospitalId;
                                work.CreatedById = item.CreatedById;
                                work.CreatedBy = item.User.UserName;
                                work.BarCode = item.Request.AssetDetail.Barcode;
                                work.TypeName = item.WorkOrderType != null ? item.WorkOrderType.Name : "";
                                work.TypeNameAr = item.WorkOrderType != null ? item.WorkOrderType.NameAr : "";
                                work.PeriorityName = item.WorkOrderPeriority != null ? item.WorkOrderPeriority.Name : "";
                                work.PeriorityNameAr = item.WorkOrderPeriority != null ? item.WorkOrderPeriority.NameAr : "";
                                work.AssignedTo = lstTracks[0].AssignedTo;
                                work.WorkOrderStatusId = lstTracks[0].WorkOrderStatusId;

                                work.WorkOrderStatusId = lstTracks[0].WorkOrderStatusId;
                                work.StatusName = lstTracks[0].WorkOrderStatus.Name;
                                work.StatusNameAr = lstTracks[0].WorkOrderStatus.NameAr;
                                work.statusColor = lstTracks[0].WorkOrderStatus.Color;
                                work.statusIcon = lstTracks[0].WorkOrderStatus.Icon;

                                if (work.WorkOrderStatusId == 12)
                                {
                                    work.ClosedDate = lstTracks[0].CreationDate;
                                }


                                if (userId != null)
                                {
                                    var lstAssigned = _context.WorkOrderTrackings.Where(a => a.AssignedTo == userId && a.WorkOrderId == work.Id).ToList();
                                    if (lstAssigned.Count > 0)
                                    {
                                        work.AssignedTo = lstAssigned[0].AssignedTo;
                                    }
                                }
                                work.ListTracks = _context.WorkOrderTrackings.Include(a => a.WorkOrderStatus).Where(a => a.WorkOrderId == work.Id)
                                .ToList().Select(item => new LstWorkOrderFromTracking
                                {
                                    Id = item.Id,
                                    StatusName = item.WorkOrderStatus.Name,
                                    StatusNameAr = item.WorkOrderStatus.NameAr,
                                    CreationDate = DateTime.Parse(item.CreationDate.ToString())
                                }).ToList();

                                lstModel.Add(work);
                            }
                        }
                    }
                    else
                    {
                        var lstTracks = _context.WorkOrderTrackings.Include(a => a.WorkOrderStatus).Where(a => a.WorkOrderId == item.Id).OrderByDescending(a => a.CreationDate).ToList();

                        if (lstTracks.Count > 0)
                        {
                            var workOrderStatusId = lstTracks[0].WorkOrderStatusId;
                            IndexWorkOrderVM2.GetData work = new IndexWorkOrderVM2.GetData();
                            work.Id = item.Id;
                            work.AssetName = item.Request.AssetDetail.MasterAsset.Name;
                            work.AssetNameAr = item.Request.AssetDetail.MasterAsset.NameAr;
                            work.SerialNumber = item.Request.AssetDetail.SerialNumber;
                            work.BarCode = item.Request.AssetDetail.Barcode;
                            work.ModelNumber = item.Request.AssetDetail.MasterAsset.ModelNumber;
                            work.WorkOrderNumber = item.WorkOrderNumber;
                            work.ModelNumber = item.Request.AssetDetail.MasterAsset.ModelNumber;
                            work.Subject = item.Subject;
                            work.RequestSubject = item.Request.Subject;
                            work.CreationDate = item.CreationDate;
                            work.Note = lstTracks[0].Notes;
                            work.ActualStartDate = item.ActualStartDate;
                            work.ActualEndDate = item.ActualEndDate;
                            work.RequestId = item.RequestId != null ? (int)item.RequestId : 0;
                            work.HospitalId = item.Request.AssetDetail.HospitalId;
                            work.CreatedById = item.CreatedById;
                            work.CreatedBy = item.User.UserName;
                            work.BarCode = item.Request.AssetDetail.Barcode;
                            work.TypeName = item.WorkOrderType != null ? item.WorkOrderType.Name : "";
                            work.TypeNameAr = item.WorkOrderType != null ? item.WorkOrderType.NameAr : "";
                            work.PeriorityName = item.WorkOrderPeriority != null ? item.WorkOrderPeriority.Name : "";
                            work.PeriorityNameAr = item.WorkOrderPeriority != null ? item.WorkOrderPeriority.NameAr : "";
                            work.AssignedTo = lstTracks[0].AssignedTo;
                            work.WorkOrderStatusId = lstTracks[0].WorkOrderStatusId;
                            work.WorkOrderStatusId = lstTracks[0].WorkOrderStatusId;
                            work.StatusName = lstTracks[0].WorkOrderStatus.Name;
                            work.StatusNameAr = lstTracks[0].WorkOrderStatus.NameAr;
                            work.statusColor = lstTracks[0].WorkOrderStatus.Color;
                            work.statusIcon = lstTracks[0].WorkOrderStatus.Icon;

                            if (work.WorkOrderStatusId == 12)
                            {
                                work.ClosedDate = lstTracks[0].CreationDate;
                            }

                            if (userId != null)
                            {
                                var lstAssigned = _context.WorkOrderTrackings.Where(a => a.AssignedTo == userId && a.WorkOrderId == work.Id).ToList();
                                if (lstAssigned.Count > 0)
                                {
                                    work.AssignedTo = lstAssigned[0].AssignedTo;
                                }
                            }
                            work.ListTracks = _context.WorkOrderTrackings.Include(a => a.WorkOrderStatus).Where(a => a.WorkOrderId == work.Id)
                            .ToList().Select(item => new LstWorkOrderFromTracking
                            {
                                Id = item.Id,
                                StatusName = item.WorkOrderStatus.Name,
                                StatusNameAr = item.WorkOrderStatus.NameAr,
                                CreationDate = DateTime.Parse(item.CreationDate.ToString())
                            }).ToList();

                            lstModel.Add(work);
                        }
                    }
                }
            }

            var workOrdersPerPage = lstModel.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            mainClass.Results = workOrdersPerPage.ToList();
            mainClass.Count = lstModel.Count;
            return mainClass;


        }

        public int GetWorkOrdersCountByStatusIdAndPaging(int? hospitalId, string userId, int statusId)
        {
            IQueryable<WorkOrder> lstWorkOrders = _context.WorkOrders
                     .Include(w => w.WorkOrderType)
                     .Include(w => w.WorkOrderPeriority)
                     .Include(w => w.Request)
                     .Include(w => w.Request.AssetDetail)
                     .Include(w => w.Request.AssetDetail.MasterAsset)
                     .Include(a => a.Request.AssetDetail.Hospital)
                     .Include(a => a.Request.AssetDetail.Hospital.Governorate)
                     .Include(a => a.Request.AssetDetail.Hospital.City)
                     .Include(a => a.Request.AssetDetail.Hospital.Organization)
                     .Include(a => a.Request.AssetDetail.Hospital.SubOrganization)
                     .Include(w => w.User)
                     .OrderByDescending(a => a.CreationDate)

                     .AsQueryable();

            if (hospitalId != null)
            {
                lstWorkOrders = lstWorkOrders.Where(a => a.Request.HospitalId == hospitalId).AsQueryable();
            }
            else
            {
                lstWorkOrders = lstWorkOrders.AsQueryable();
            }

            if (statusId == 0)
            {
                List<WorkOrderTracking> listTracks = new List<WorkOrderTracking>();
                var allWorkOrders = lstWorkOrders.ToList<WorkOrder>();
                var workOrdersPerPage = allWorkOrders.ToList<WorkOrder>();
                if (workOrdersPerPage.Count() > 0)
                {
                    foreach (var item in workOrdersPerPage)
                    {
                        var lstTracks = _context.WorkOrderTrackings.Include(a => a.WorkOrder).Include(a => a.WorkOrderStatus).Where(a => a.WorkOrderId == item.Id).OrderByDescending(a => a.CreationDate).ToList();
                        if (lstTracks.Count > 0)
                        {
                            var workOrderStatusId = lstTracks[0].WorkOrderStatusId;
                            listTracks.Add(lstTracks[0]);
                        }
                    }
                    var count = listTracks.Select(a => a.WorkOrder).Count();
                }
            }
            else
            {
                List<WorkOrderTracking> listTracks = new List<WorkOrderTracking>();

                var allWorkOrders = lstWorkOrders.ToList<WorkOrder>();
                var workOrdersPerPage = allWorkOrders.ToList<WorkOrder>();
                if (workOrdersPerPage.Count() > 0)
                {
                    foreach (var item in workOrdersPerPage)
                    {
                        var lstTracks = _context.WorkOrderTrackings.Include(a => a.WorkOrder).Include(a => a.WorkOrderStatus).Where(a => a.WorkOrderId == item.Id).OrderByDescending(a => a.CreationDate).ToList();
                        if (lstTracks.Count > 0)
                        {

                            var workOrderStatusId = lstTracks[0].WorkOrderStatusId;
                            if (workOrderStatusId == statusId)
                                listTracks.Add(lstTracks[0]);
                        }
                    }
                    var count = listTracks.Select(a => a.WorkOrder).Count();

                    return count;
                }
            }

            return lstWorkOrders.Count();
        }

        public IndexWorkOrderVM2 GetWorkOrdersByDateAndStatus(SearchWorkOrderByDateVM woDateObj, int pageNumber, int pageSize)
        {


            DateTime? start = new DateTime();
            DateTime? end = new DateTime();
            IndexWorkOrderVM2 mainClass = new IndexWorkOrderVM2();
            List<IndexWorkOrderVM2.GetData> list = new List<IndexWorkOrderVM2.GetData>();
            ApplicationUser UserObj = new ApplicationUser();
            List<string> userRoleNames = new List<string>();
            var obj = _context.ApplicationUser.Where(a => a.Id == woDateObj.UserId).ToList();
            if (obj.Count > 0)
            {
                UserObj = obj[0];
                var roles = (from userRole in _context.UserRoles
                             join role in _context.ApplicationRole on userRole.RoleId equals role.Id
                             where userRole.UserId == woDateObj.UserId
                             select role);
                foreach (var role in roles)
                {
                    userRoleNames.Add(role.Name);
                }
            }

            IQueryable<WorkOrderTracking> lstWorkOrders = GetAllWorkOrders();

            //_context.WorkOrderTrackings.Include(a => a.WorkOrder).Include(a => a.WorkOrderStatus)
            //     .Include(a => a.WorkOrder.WorkOrderPeriority).Include(a => a.WorkOrder.WorkOrderType).Include(a => a.User)
            //     .Include(a => a.WorkOrder.Request)
            //     .Include(a => a.WorkOrder.Request.AssetDetail)
            //     .Include(a => a.WorkOrder.Request.AssetDetail.MasterAsset)
            //     .Include(a => a.WorkOrder.Request.AssetDetail.MasterAsset.brand)
            //     .Include(a => a.WorkOrder.Request.AssetDetail.Supplier)
            //     .Include(a => a.WorkOrder.Request.AssetDetail.Department)
            //     .Include(a => a.WorkOrder.Request.AssetDetail.MasterAsset.Origin)
            //     .Include(a => a.WorkOrder.Request.AssetDetail.Building)
            //     .Include(a => a.WorkOrder.Request.AssetDetail.Floor)
            //     .Include(a => a.WorkOrder.Request.AssetDetail.Room)
            //     .Include(a => a.WorkOrder.Request.AssetDetail.Hospital).ThenInclude(h => h.Organization)
            //     .Include(a => a.WorkOrder.Request.AssetDetail.Hospital).ThenInclude(h => h.Governorate)
            //     .Include(a => a.WorkOrder.Request.AssetDetail.Hospital).ThenInclude(h => h.City)
            //     .Include(a => a.WorkOrder.Request.AssetDetail.Hospital).ThenInclude(h => h.SubOrganization)
            //     .ToList()
            //     .GroupBy(track => track.WorkOrderId)
            //     .Select(g => g.OrderByDescending(track => track.WorkOrderDate).FirstOrDefault())
            //     .AsQueryable();

            if (UserObj.GovernorateId == 0 && UserObj.CityId == 0 && UserObj.HospitalId == 0)
            {
                lstWorkOrders = lstWorkOrders;
            }
            if (UserObj.GovernorateId > 0 && UserObj.CityId == 0 && UserObj.HospitalId == 0)
            {
                lstWorkOrders = lstWorkOrders.Where(t => t.WorkOrder.Hospital.GovernorateId == UserObj.GovernorateId);
            }
            if (UserObj.GovernorateId > 0 && UserObj.CityId > 0 && UserObj.HospitalId == 0)
            {
                lstWorkOrders = lstWorkOrders.Where(t => t.WorkOrder.Hospital.CityId == UserObj.CityId);
            }
            if (UserObj.OrganizationId > 0 && UserObj.SubOrganizationId == 0 && UserObj.HospitalId == 0)
            {
                lstWorkOrders = lstWorkOrders.Where(t => t.WorkOrder.Hospital.OrganizationId == UserObj.OrganizationId);
            }
            if (UserObj.OrganizationId > 0 && UserObj.SubOrganizationId > 0 && UserObj.HospitalId == 0)
            {
                lstWorkOrders = lstWorkOrders.Where(t => t.WorkOrder.Hospital.SubOrganizationId == UserObj.SubOrganizationId);
            }



            if (woDateObj.StrStartDate != "")
            {
                woDateObj.StartDate = DateTime.Parse(woDateObj.StrStartDate);
                start = DateTime.Parse(woDateObj.StrStartDate);
                woDateObj.StartDate = start;
            }
            else
            {
                woDateObj.StartDate = DateTime.Parse("01/01/1900");
                start = DateTime.Parse(woDateObj.StartDate.ToString());
            }
            if (woDateObj.StrEndDate != "")
            {
                woDateObj.EndDate = DateTime.Parse(woDateObj.StrEndDate);
                end = DateTime.Parse(woDateObj.StrEndDate);
                woDateObj.EndDate = end;
            }
            else
            {
                woDateObj.EndDate = DateTime.Today.Date;
                end = DateTime.Parse(woDateObj.EndDate.ToString());
            }
            if (start != null || end != null)
            {
                lstWorkOrders = lstWorkOrders.Where(a => a.WorkOrder.CreationDate.Value.Date >= start.Value.Date && a.WorkOrder.CreationDate.Value.Date <= end.Value.Date);
            }
            else
            {
                lstWorkOrders = lstWorkOrders;
            }

            if (woDateObj.HospitalId > 0)
            {
                lstWorkOrders = lstWorkOrders.Where(a => a.WorkOrder.HospitalId == woDateObj.HospitalId);
            }
            else
            {
                lstWorkOrders = lstWorkOrders;
            }

            if (woDateObj.StatusId != null)
            {
                lstWorkOrders = lstWorkOrders.Where(a => a.WorkOrderStatusId == woDateObj.StatusId);

            }
            else
            {
                lstWorkOrders = lstWorkOrders;
            }


            if (UserObj.HospitalId > 0)
            {
                lstWorkOrders = lstWorkOrders.Where(t => t.WorkOrder.HospitalId == UserObj.HospitalId);
            }
            else
            {
                lstWorkOrders = lstWorkOrders;
            }

            #region Count data and fiter By Paging
            IQueryable<WorkOrderTracking> lstResults;
            var countItems = lstWorkOrders.ToList();
            mainClass.Count = countItems.Count();
            if (pageNumber == 0 && pageSize == 0)
                lstResults = lstWorkOrders;
            else
                lstResults = lstWorkOrders.Skip((pageNumber - 1) * pageSize).Take(pageSize);
            #endregion


            foreach (var item in lstResults.ToList())
            {
                IndexWorkOrderVM2.GetData work = new IndexWorkOrderVM2.GetData();
                work.Id = item.Id;
                if (item.WorkOrderStatusId != 0)
                {
                    work.WorkOrderStatusId = item.WorkOrderStatus.Id;
                    work.StatusId = item.WorkOrderStatusId;
                    work.StatusName = item.WorkOrderStatus.Name;
                    work.StatusNameAr = item.WorkOrderStatus.NameAr;
                    work.statusColor = item.WorkOrderStatus.Color;
                    work.statusIcon = item.WorkOrderStatus.Icon;
                    work.Note = item.Notes;
                    if (work.WorkOrderStatusId == 12)
                    {
                        work.ClosedDate = item.CreationDate;
                    }
                    else
                    {
                        work.ClosedDate = null;
                    }
                }

                work.WorkOrderNumber = item.WorkOrder.WorkOrderNumber;
                work.BarCode = item.WorkOrder.Request.AssetDetail.Barcode;
                work.ModelNumber = item.WorkOrder.Request.AssetDetail.MasterAsset.ModelNumber;
                work.AssetName = item.WorkOrder.Request.AssetDetail.MasterAsset.Name;
                work.AssetNameAr = item.WorkOrder.Request.AssetDetail.MasterAsset.NameAr;
                work.SerialNumber = item.WorkOrder.Request.AssetDetail.SerialNumber;
                work.Subject = item.WorkOrder.Subject;
                work.RequestSubject = item.WorkOrder.Request.Subject;
                var openWorkOrder = _context.WorkOrderTrackings
                    .Include(t => t.WorkOrder).Where(a => a.WorkOrderId == item.WorkOrderId && a.WorkOrderStatusId == 1).ToList();
                if (openWorkOrder.Count > 0)
                {
                    work.CreationDate = openWorkOrder[0].CreationDate;
                }
                else
                {
                    work.CreationDate = item.CreationDate;
                }

                work.Note = item.Notes;
                work.CreatedById = item.CreatedById;
                work.CreatedBy = item.User.UserName;
                work.TypeName = item.WorkOrder.WorkOrderType.Name;
                work.TypeNameAr = item.WorkOrder.WorkOrderType.NameAr;
                work.PeriorityName = item.WorkOrder.WorkOrderPeriority != null ? item.WorkOrder.WorkOrderPeriority.Name : "";
                work.PeriorityNameAr = item.WorkOrder.WorkOrderPeriority != null ? item.WorkOrder.WorkOrderPeriority.NameAr : "";
                work.AssignedTo = item.AssignedTo;
                work.ActualStartDate = item.ActualStartDate;
                work.ActualEndDate = item.ActualEndDate;
                work.RequestId = item.WorkOrder.RequestId != null ? (int)item.WorkOrder.RequestId : 0;
                work.HospitalId = item.WorkOrder.Request.AssetDetail.HospitalId;
                list.Add(work);
            }
            mainClass.Results = list;
            return mainClass;
        }

        public IndexWorkOrderVM2 GetWorkOrdersByDateAndStatus(SearchWorkOrderByDateVM woDateObj)
        {
            IndexWorkOrderVM2 mainClass = new IndexWorkOrderVM2();
            List<IndexWorkOrderVM2.GetData> list = new List<IndexWorkOrderVM2.GetData>();
            ApplicationUser UserObj = new ApplicationUser();
            List<string> userRoleNames = new List<string>();
            List<string> listNotes = new List<string>();
            var obj = _context.ApplicationUser.Where(a => a.Id == woDateObj.UserId).ToList();
            if (obj.Count > 0)
            {
                UserObj = obj[0];

                var roles = (from userRole in _context.UserRoles
                             join role in _context.ApplicationRole on userRole.RoleId equals role.Id
                             where userRole.UserId == woDateObj.UserId
                             select role);
                foreach (var role in roles)
                {
                    userRoleNames.Add(role.Name);
                }
            }

            DateTime? start = new DateTime();
            DateTime? end = new DateTime();
            if (woDateObj.StrStartDate != "")
            {
                woDateObj.StartDate = DateTime.Parse(woDateObj.StrStartDate);
                start = DateTime.Parse(woDateObj.StrStartDate);
                woDateObj.StartDate = start;
            }
            else
            {
                woDateObj.StartDate = DateTime.Parse("01/01/1900");
                start = DateTime.Parse(woDateObj.StartDate.ToString());
            }


            if (woDateObj.StrEndDate != "")
            {
                woDateObj.EndDate = DateTime.Parse(woDateObj.StrEndDate);
                end = DateTime.Parse(woDateObj.StrEndDate);
                woDateObj.EndDate = end;
            }
            else
            {
                woDateObj.EndDate = DateTime.Today.Date;
                end = DateTime.Parse(woDateObj.EndDate.ToString());
            }

            var lstWorkOrders = _context.WorkOrderTrackings
                 .Include(t => t.WorkOrder)
                 .Include(t => t.WorkOrderStatus)
                  .Include(w => w.WorkOrder.WorkOrderType)
                  .Include(w => w.WorkOrder.WorkOrderPeriority)
                  .Include(w => w.WorkOrder.Request)
                  .Include(w => w.WorkOrder.Request.AssetDetail)
                  .Include(w => w.WorkOrder.Request.AssetDetail.MasterAsset)
                  .Include(w => w.User)
                  .ToList().OrderByDescending(a => a.CreationDate).ToList().GroupBy(a => a.WorkOrderId).ToList();




            if (woDateObj.StatusId > 0)
            {
                lstWorkOrders = lstWorkOrders.Where(a => a.FirstOrDefault().WorkOrderStatusId == woDateObj.StatusId).ToList();
            }
            else
            {
                lstWorkOrders = lstWorkOrders.ToList();
            }


            if (start != null && end != null)
            {
                lstWorkOrders = lstWorkOrders.Where(a => a.FirstOrDefault().WorkOrder.CreationDate.Value.Date >= start.Value.Date && a.FirstOrDefault().WorkOrder.CreationDate.Value.Date <= end.Value.Date).ToList();
            }
            else
            {
                lstWorkOrders = lstWorkOrders.ToList();
            }


            if (UserObj.GovernorateId == 0 && UserObj.CityId == 0 && UserObj.HospitalId == 0)
            {
                lstWorkOrders = lstWorkOrders.ToList();
            }
            if (UserObj.GovernorateId == 0 && UserObj.CityId == 0 && UserObj.OrganizationId == 0 && UserObj.SubOrganizationId == 0 && UserObj.HospitalId == 0)
            {
                lstWorkOrders = lstWorkOrders.ToList();
            }
            if (UserObj.GovernorateId > 0 && UserObj.CityId == 0 && UserObj.HospitalId == 0)
            {
                lstWorkOrders = lstWorkOrders.Where(t => t.FirstOrDefault().WorkOrder.Request.AssetDetail.Hospital.GovernorateId == UserObj.GovernorateId).ToList();
            }
            if (UserObj.GovernorateId > 0 && UserObj.CityId > 0 && UserObj.HospitalId == 0)
            {
                lstWorkOrders = lstWorkOrders.Where(t => t.FirstOrDefault().WorkOrder.Request.AssetDetail.Hospital.CityId == UserObj.CityId).ToList();
            }
            if (UserObj.OrganizationId > 0 && UserObj.SubOrganizationId == 0 && UserObj.HospitalId == 0)
            {
                lstWorkOrders = lstWorkOrders.Where(t => t.FirstOrDefault().WorkOrder.Request.AssetDetail.Hospital.OrganizationId == UserObj.OrganizationId).ToList();
            }
            if (UserObj.OrganizationId > 0 && UserObj.SubOrganizationId > 0 && UserObj.HospitalId == 0)
            {
                lstWorkOrders = lstWorkOrders.Where(t => t.FirstOrDefault().WorkOrder.Request.AssetDetail.Hospital.SubOrganizationId == UserObj.SubOrganizationId).ToList();
            }
            if (UserObj.HospitalId > 0)
            {
                lstWorkOrders = lstWorkOrders.Where(t => t.FirstOrDefault().WorkOrder.Request.HospitalId == UserObj.HospitalId).ToList();
            }
            foreach (var item in lstWorkOrders)
            {
                IndexWorkOrderVM2.GetData work = new IndexWorkOrderVM2.GetData();
                work.Id = item.FirstOrDefault().Id;
                if (item.FirstOrDefault().WorkOrderStatusId != 0)
                {
                    work.WorkOrderStatusId = item.FirstOrDefault().WorkOrderStatus.Id;
                    work.StatusId = item.FirstOrDefault().WorkOrderStatusId;
                    work.StatusName = item.FirstOrDefault().WorkOrderStatus.Name;
                    work.StatusNameAr = item.FirstOrDefault().WorkOrderStatus.NameAr;
                    work.statusColor = item.FirstOrDefault().WorkOrderStatus.Color;
                    work.statusIcon = item.FirstOrDefault().WorkOrderStatus.Icon;


                    work.Note = item.FirstOrDefault().Notes;
                    if (work.WorkOrderStatusId == 12)
                    {
                        work.ClosedDate = item.FirstOrDefault().CreationDate;
                    }
                    else
                    {
                        work.ClosedDate = null;
                    }
                }

                work.WorkOrderNumber = item.FirstOrDefault().WorkOrder.WorkOrderNumber;
                work.BarCode = item.FirstOrDefault().WorkOrder.Request.AssetDetail.Barcode;
                work.ModelNumber = item.FirstOrDefault().WorkOrder.Request.AssetDetail.MasterAsset.ModelNumber;
                work.AssetName = item.FirstOrDefault().WorkOrder.Request.AssetDetail.MasterAsset.Name;
                work.AssetNameAr = item.FirstOrDefault().WorkOrder.Request.AssetDetail.MasterAsset.NameAr;
                work.SerialNumber = item.FirstOrDefault().WorkOrder.Request.AssetDetail.SerialNumber;
                work.Subject = item.FirstOrDefault().WorkOrder.Subject;
                work.RequestSubject = item.FirstOrDefault().WorkOrder.Request.Subject;

                //var openWorkOrder = _context.WorkOrderTrackings
                //    .Include(t => t.WorkOrder).Where(a => a.WorkOrderId == item.FirstOrDefault().WorkOrderId && a.WorkOrderStatusId == 1).ToList();
                //if (openWorkOrder.Count > 0)
                //{
                //    work.CreationDate = openWorkOrder[0].CreationDate;
                //}
                //else
                //{
                //    work.CreationDate = item.FirstOrDefault().CreationDate;
                //}

                var lstWOStatus = _context.WorkOrderTrackings
                     .Include(o => o.WorkOrder).Include(o => o.WorkOrderStatus)
                     .Where(a => a.WorkOrder.RequestId == item.FirstOrDefault().WorkOrder.RequestId)
                     .OrderByDescending(a => a.CreationDate.Value).ToList();

                if (lstWOStatus.Count > 0)
                {
                    foreach (var note in lstWOStatus)
                    {
                        listNotes.Add("-" + note.Notes);
                    }
                    work.ListWorkOrderNotes = string.Join(Environment.NewLine, listNotes);
                }


                work.CreationDate = item.FirstOrDefault().CreationDate;
                work.Note = item.FirstOrDefault().Notes;
                work.CreatedById = item.FirstOrDefault().CreatedById;
                work.CreatedBy = item.FirstOrDefault().User.UserName;
                work.TypeName = item.FirstOrDefault().WorkOrder.WorkOrderType.Name;
                work.TypeNameAr = item.FirstOrDefault().WorkOrder.WorkOrderType.NameAr;
                work.PeriorityName = item.FirstOrDefault().WorkOrder.WorkOrderPeriority != null ? item.FirstOrDefault().WorkOrder.WorkOrderPeriority.Name : "";
                work.PeriorityNameAr = item.FirstOrDefault().WorkOrder.WorkOrderPeriority != null ? item.FirstOrDefault().WorkOrder.WorkOrderPeriority.NameAr : "";
                work.AssignedTo = item.FirstOrDefault().AssignedTo;
                work.ActualStartDate = item.FirstOrDefault().ActualStartDate;
                work.ActualEndDate = item.FirstOrDefault().ActualEndDate;
                work.RequestId = item.FirstOrDefault().WorkOrder.RequestId != null ? (int)item.FirstOrDefault().WorkOrder.RequestId : 0;
                work.HospitalId = item.FirstOrDefault().WorkOrder.Request.AssetDetail.HospitalId;
                list.Add(work);
            }


            var requestsPerPage = list.ToList();
            mainClass.Results = requestsPerPage;
            mainClass.Count = lstWorkOrders.Count;
            return mainClass;


        }




        #region Refactor Functions

        #region Main Functions


        public GeneratedWorkOrderNumberVM GenerateWorOrderNumber()
        {
            GeneratedWorkOrderNumberVM numberObj = new GeneratedWorkOrderNumberVM();
            string WO = "WO";

            var lstIds = _context.WorkOrders.ToList();
            if (lstIds.Count > 0)
            {
                var code = lstIds.LastOrDefault().Id;
                numberObj.WONumber = WO + (code + 1);
            }
            else
            {
                numberObj.WONumber = WO + 1;
            }

            return numberObj;
        }

        public IndexWorkOrderVM GetById(int id)
        {
            IndexWorkOrderVM work = new IndexWorkOrderVM();
            var woObj = _context.WorkOrders.Where(a => a.Id == id).Include(w => w.Request).Include(w => w.Request.AssetDetail).Include(w => w.Request.AssetDetail.Supplier)
                         .Include(w => w.Request.AssetDetail.MasterAsset.brand)
                         .Include(w => w.Request.AssetDetail.Department)
                          .Include(w => w.Request.AssetDetail.Building)
                         .Include(w => w.Request.AssetDetail.Floor)
                         .Include(w => w.Request.AssetDetail.Room)
                        .Include(w => w.Request.AssetDetail.MasterAsset)
                        .Include(w => w.WorkOrderType)
                        .Include(w => w.WorkOrderPeriority).Include(w => w.User).Where(a => a.Id == id).ToList();

            if (woObj.Count > 0)
            {
                work.Id = woObj[0].Id;
                work.Subject = woObj[0].Subject;

                work.BarCode = woObj[0].Request.AssetDetail.Barcode;
                work.SerialNumber = woObj[0].Request.AssetDetail.SerialNumber;
                if (woObj[0].Request.AssetDetail.Supplier != null)
                {
                    work.SupplierName = woObj[0].Request.AssetDetail.Supplier.Name;
                    work.SupplierNameAr = woObj[0].Request.AssetDetail.Supplier.NameAr;
                }
                work.WorkOrderNumber = woObj[0].WorkOrderNumber;
                work.ModelNumber = woObj[0].Request.AssetDetail.MasterAsset.ModelNumber;
                // work.CreationDate = woObj[0].CreationDate;
                work.PlannedStartDate = woObj[0].PlannedStartDate;
                work.PlannedEndDate = woObj[0].PlannedEndDate;
                work.ActualStartDate = woObj[0].ActualStartDate;
                work.ActualEndDate = woObj[0].ActualEndDate;
                work.BrandName = woObj[0].Request.AssetDetail.MasterAsset.brand != null ? woObj[0].Request.AssetDetail.MasterAsset.brand.Name : "";
                work.BrandNameAr = woObj[0].Request.AssetDetail.MasterAsset.brand != null ? woObj[0].Request.AssetDetail.MasterAsset.brand.NameAr : "";
                work.DepartmentName = woObj[0].Request.AssetDetail.Department != null ? woObj[0].Request.AssetDetail.Department.Name : "";
                work.DepartmentNameAr = woObj[0].Request.AssetDetail.Department != null ? woObj[0].Request.AssetDetail.Department.NameAr : "";

                work.Note = woObj[0].Note;
                work.MasterAssetId = woObj[0].Request.AssetDetail.MasterAssetId;
                work.CreatedById = woObj[0].CreatedById;
                work.CreatedBy = woObj[0].User.UserName;
                work.WorkOrderPeriorityId = woObj[0].WorkOrderPeriorityId != null ? (int)woObj[0].WorkOrderPeriorityId : 0;
                work.WorkOrderPeriorityId = woObj[0].WorkOrderPeriority != null ? woObj[0].WorkOrderPeriority.Id : 0;
                work.PeriorityName = woObj[0].WorkOrderPeriority != null ? woObj[0].WorkOrderPeriority.Name : "";
                work.PeriorityNameAr = woObj[0].WorkOrderPeriority != null ? woObj[0].WorkOrderPeriority.NameAr : "";
                work.AssetName = woObj[0].Request.AssetDetail.MasterAsset.Name;
                work.AssetNameAr = woObj[0].Request.AssetDetail.MasterAsset.NameAr;
                work.WorkOrderTypeId = woObj[0].WorkOrderTypeId != null ? (int)woObj[0].WorkOrderTypeId : 0;
                work.WorkOrderTypeName = woObj[0].WorkOrderTypeId != null ? woObj[0].WorkOrderType.Name : "";
                work.WorkOrderTypeNameAr = woObj[0].WorkOrderTypeId != null ? woObj[0].WorkOrderType.NameAr : "";
                work.TypeName = woObj[0].WorkOrderTypeId != null ? woObj[0].WorkOrderType.Name : "";
                work.TypeNameAr = woObj[0].WorkOrderTypeId != null ? woObj[0].WorkOrderType.NameAr : "";
                work.RequestId = woObj[0].RequestId != null ? (int)woObj[0].RequestId : 0;
                work.RequestSubject = woObj[0].Request.Subject;
                work.HospitalId = woObj[0].Request.AssetDetail.HospitalId;
                work.WorkOrderTrackingId = _context.WorkOrderTrackings.Where(t => t.WorkOrderId == id).FirstOrDefault().Id;
                // work.WorkOrderStatusId = _context.WorkOrderTrackings.Where(t => t.WorkOrderId == id).FirstOrDefault().WorkOrderStatusId;

                work.WarrantyStart = woObj[0].Request.AssetDetail.WarrantyStart.ToString();
                work.WarrantyEnd = woObj[0].Request.AssetDetail.WarrantyEnd.ToString();
                work.WarrantyExpires = woObj[0].Request.AssetDetail.WarrantyExpires != null ? woObj[0].Request.AssetDetail.WarrantyExpires.ToString() : "";

                work.PurchaseDate = woObj[0].Request.AssetDetail.PurchaseDate.ToString();
                work.InstallationDate = woObj[0].Request.AssetDetail.InstallationDate.ToString();
                work.OperationDate = woObj[0].Request.AssetDetail.OperationDate.ToString();
                work.ReceivingDate = woObj[0].Request.AssetDetail.ReceivingDate.ToString();

                if (woObj[0].Request.AssetDetail.Building != null)
                {
                    work.BuildingName = woObj[0].Request.AssetDetail.Building.Name;
                    work.BuildingNameAr = woObj[0].Request.AssetDetail.Building.NameAr;
                }
                if (woObj[0].Request.AssetDetail.Floor != null)
                {
                    work.FloorName = woObj[0].Request.AssetDetail.Floor.Name;
                    work.FloorNameAr = woObj[0].Request.AssetDetail.Floor.Name;
                }
                if (woObj[0].Request.AssetDetail.Room != null)
                {
                    work.RoomName = woObj[0].Request.AssetDetail.Room.Name;
                    work.RoomNameAr = woObj[0].Request.AssetDetail.Room.Name;
                }

                work.DepreciationRate = woObj[0].Request.AssetDetail.DepreciationRate != null ? woObj[0].Request.AssetDetail.DepreciationRate.ToString() : "0";
                work.PONumber = woObj[0].Request.AssetDetail.PONumber;
                work.CostCenter = woObj[0].Request.AssetDetail.CostCenter;
                work.Remarks = woObj[0].Request.AssetDetail.Remarks;



                var lstStatus = _context.WorkOrderTrackings.Include(a => a.WorkOrderStatus).Where(t => t.WorkOrderId == id).OrderByDescending(a => a.CreationDate).ToList();
                if (lstStatus.Count > 0)
                {
                    work.Note = lstStatus[0].Notes;
                    work.StatusName = lstStatus[0].WorkOrderStatus.Name;
                    work.StatusNameAr = lstStatus[0].WorkOrderStatus.NameAr;
                    work.WorkOrderStatusId = lstStatus[0].WorkOrderStatusId;
                    if (lstStatus[0].WorkOrderStatusId == 12)
                    {
                        work.ClosedDate = lstStatus[0].CreationDate;
                        work.CreationDate = lstStatus.LastOrDefault().WorkOrder.CreationDate;
                    }
                    else
                        work.CreationDate = lstStatus.LastOrDefault().WorkOrder.CreationDate;


                }
            }
            return work;
        }

        public IEnumerable<IndexWorkOrderVM> GetAll()
        {
            return _context.WorkOrders.Include(w => w.Request).Include(w => w.WorkOrderType).Include(w => w.WorkOrderPeriority).Include(w => w.User)
                .Select(prob => new IndexWorkOrderVM
                {
                    Id = prob.Id,
                    Subject = prob.Subject,
                    BarCode = prob.Request.AssetDetail.Barcode,
                    WorkOrderNumber = prob.WorkOrderNumber,
                    CreationDate = prob.CreationDate,
                    PlannedStartDate = prob.PlannedStartDate,
                    PlannedEndDate = prob.PlannedEndDate,
                    ActualStartDate = prob.ActualStartDate,
                    ActualEndDate = prob.ActualEndDate,
                    Note = prob.Note,
                    CreatedById = prob.CreatedById,
                    CreatedBy = prob.User.UserName,
                    WorkOrderPeriorityId = prob.WorkOrderPeriorityId != null ? (int)prob.WorkOrderPeriorityId : 0,
                    WorkOrderPeriorityName = prob.WorkOrderPeriority.Name,
                    WorkOrderTypeId = prob.WorkOrderTypeId != null ? (int)prob.WorkOrderTypeId : 0,
                    WorkOrderTypeName = prob.WorkOrderType.Name,
                    RequestId = prob.RequestId != null ? (int)prob.RequestId : 0,
                    RequestSubject = prob.Request.Subject,

                }).OrderByDescending(o => o.CreationDate).ToList();
        }

        public PrintWorkOrderVM PrintWorkOrderById(int id)
        {
            List<LstWorkOrderFromTracking> lstTracking = new List<LstWorkOrderFromTracking>();
            PrintWorkOrderVM printWorkObj = new PrintWorkOrderVM();
            var workOrders = _context.WorkOrders.Where(a => a.Id == id).Include(w => w.Request).Include(a => a.Request.AssetDetail)
                .Include(a => a.Request.RequestType).Include(a => a.Request.SubProblem).Include(a => a.Request.SubProblem.Problem)
                .Include(a => a.Request.RequestMode)
                .Include(a => a.Request.AssetDetail.Hospital)
                .Include(a => a.Request.AssetDetail.MasterAsset)
                       .Include(a => a.Request.AssetDetail.MasterAsset.brand)
                .Include(w => w.WorkOrderType).Include(w => w.WorkOrderPeriority).Include(w => w.User).ToList();

            if (workOrders.Count > 0)
            {

                var work = workOrders[0];

                printWorkObj.Id = work.Id;
                printWorkObj.Subject = work.Subject;
                printWorkObj.MasterAssetCode = work.Request.AssetDetail.MasterAsset.Code;
                printWorkObj.AssetCode = work.Request.AssetDetail.Code;
                printWorkObj.BrandName = work.Request.AssetDetail.MasterAsset.brand.Name;
                printWorkObj.BrandNameAr = work.Request.AssetDetail.MasterAsset.brand.NameAr;
                printWorkObj.BarCode = work.Request.AssetDetail.Barcode;
                printWorkObj.ModelNumber = work.Request.AssetDetail.MasterAsset.ModelNumber;
                printWorkObj.WorkOrderNumber = work.WorkOrderNumber;
                printWorkObj.CreationDate = work.CreationDate;
                printWorkObj.PlannedStartDate = work.PlannedStartDate;
                printWorkObj.PlannedEndDate = work.PlannedEndDate;
                printWorkObj.ActualStartDate = work.ActualStartDate;
                printWorkObj.ActualEndDate = work.ActualEndDate;
                printWorkObj.Note = work.Note;
                printWorkObj.CreatedById = work.CreatedById;
                printWorkObj.CreatedBy = work.User.UserName;
                printWorkObj.PeriorityName = work.WorkOrderPeriority != null ? work.WorkOrderPeriority.Name : "";
                printWorkObj.PeriorityNameAr = work.WorkOrderPeriority != null ? work.WorkOrderPeriority.NameAr : "";
                printWorkObj.TypeName = work.WorkOrderType.Name;
                printWorkObj.TypeNameAr = work.WorkOrderType.NameAr;


                printWorkObj.RequestId = work.Request.Id;
                printWorkObj.RequestSubject = work.Request.Subject;
                printWorkObj.RequestDate = work.Request.RequestDate.ToShortDateString();
                printWorkObj.RequestCode = work.Request.RequestCode;
                printWorkObj.RequestTypeName = work.Request.RequestType.Name;
                printWorkObj.RequestTypeNameAr = work.Request.RequestType.NameAr;
                printWorkObj.ModeName = work.Request.RequestMode.Name;
                printWorkObj.ModeNameAr = work.Request.RequestMode.NameAr;


                printWorkObj.SubProblemName = work.Request.SubProblem != null ? work.Request.SubProblem.Name : "";
                printWorkObj.SubProblemNameAr = work.Request.SubProblem != null ? work.Request.SubProblem.NameAr : "";


                printWorkObj.ProblemName = work.Request.SubProblem != null ? work.Request.SubProblem.Problem.Name : "";
                printWorkObj.ProblemNameAr = work.Request.SubProblem != null ? work.Request.SubProblem.Problem.NameAr : "";


                var lstRequests = _context.Request.Where(a => a.Id == work.RequestId).ToList();
                if (lstRequests.Count > 0)
                {
                    var requestObj = lstRequests[0];
                    var lstRequestTracks = _context.RequestTracking.Where(a => a.RequestId == requestObj.Id).ToList();
                    if (lstRequestTracks.Count > 0)
                    {
                        var trackObj = lstRequestTracks[0];
                        printWorkObj.FirstRequest = trackObj.Description;
                    }

                }

                printWorkObj.HospitalId = work.Request.AssetDetail.HospitalId;
                printWorkObj.HospitalName = work.Request.AssetDetail.Hospital.Name;
                printWorkObj.HospitalNameAr = work.Request.AssetDetail.Hospital.NameAr;
                printWorkObj.AssetName = work.Request.AssetDetail.MasterAsset.Name;
                printWorkObj.AssetNameAr = work.Request.AssetDetail.MasterAsset.NameAr;
                printWorkObj.SerialNumber = work.Request.AssetDetail.SerialNumber;

                var lstTracks = _context.WorkOrderTrackings
                                                      .Include(A => A.WorkOrderStatus)
                                                      .Include(A => A.User)
                                                     .Where(t => t.WorkOrderId == id).ToList();

                if (lstTracks.Count > 0)
                {
                    if (lstTracks.Where(a => a.WorkOrderStatusId == 11).ToList().Count > 0)
                    {
                        printWorkObj.LastWorkOrder = lstTracks.Where(a => a.WorkOrderStatusId == 11).Last().Notes;
                    }



                    foreach (var item in lstTracks)
                    {
                        LstWorkOrderFromTracking trackObj = new LstWorkOrderFromTracking();
                        if (item.ActualStartDate != null)
                            trackObj.ActualStartDate = DateTime.Parse(item.ActualStartDate.Value.ToShortDateString());


                        if (item.ActualEndDate != null)
                            trackObj.ActualEndDate = DateTime.Parse(item.ActualEndDate.Value.ToShortDateString());


                        trackObj.Notes = item.Notes;
                        trackObj.CreatedBy = _context.ApplicationUser.Where(a => a.Id == item.CreatedById).ToList().FirstOrDefault().UserName;
                        trackObj.StatusName = item.WorkOrderStatus.Name;
                        trackObj.StatusNameAr = item.WorkOrderStatus.NameAr;

                        if (item.AssignedTo != null && item.AssignedTo != "")
                            trackObj.AssignedToName = _context.ApplicationUser.Where(a => a.Id == item.AssignedTo).ToList().FirstOrDefault().UserName;
                        else
                            trackObj.AssignedToName = "";


                        trackObj.WorkOrderStatusId = item.WorkOrderStatusId;

                        if (item.WorkOrderStatusId == 12)
                        {
                            trackObj.ClosedDate = item.ActualEndDate.Value.ToString();//.Value.ToShortDateString();
                            printWorkObj.ClosedDate = lstTracks.Where(a => a.WorkOrderStatusId == 12).Last().CreationDate.Value.ToString();
                        }
                        else
                        {
                            trackObj.ClosedDate = "";
                            printWorkObj.ClosedDate = "";
                        }



                        lstTracking.Add(trackObj);

                    }
                    printWorkObj.LstWorkOrderTracking = lstTracking;
                }
            }
            return printWorkObj;

        }

        public void Update(int id, EditWorkOrderVM editWorkOrderVM)
        {

            try
            {
                WorkOrder workOrder = new WorkOrder();
                workOrder.Id = editWorkOrderVM.Id;
                workOrder.Subject = editWorkOrderVM.Subject;
                workOrder.WorkOrderNumber = editWorkOrderVM.WorkOrderNumber;
                workOrder.CreationDate = editWorkOrderVM.CreationDate;
                workOrder.PlannedStartDate = editWorkOrderVM.PlannedStartDate;
                workOrder.PlannedEndDate = editWorkOrderVM.PlannedEndDate;
                workOrder.ActualStartDate = editWorkOrderVM.ActualStartDate;
                workOrder.ActualEndDate = editWorkOrderVM.ActualEndDate;
                workOrder.Note = editWorkOrderVM.Note;
                workOrder.CreatedById = editWorkOrderVM.CreatedById;
                workOrder.WorkOrderPeriorityId = editWorkOrderVM.WorkOrderPeriorityId;
                workOrder.WorkOrderTypeId = editWorkOrderVM.WorkOrderTypeId;
                workOrder.RequestId = editWorkOrderVM.RequestId;
                _context.Entry(workOrder).State = EntityState.Modified;
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }
        }

        public int Add(CreateWorkOrderVM createWorkOrderVM)
        {
            try
            {
                if (createWorkOrderVM != null)
                {
                    WorkOrder workOrder = new WorkOrder();
                    workOrder.Subject = createWorkOrderVM.Subject;
                    workOrder.WorkOrderNumber = createWorkOrderVM.WorkOrderNumber;
                    workOrder.CreationDate = DateTime.Now;
                    workOrder.PlannedStartDate = DateTime.Parse(createWorkOrderVM.PlannedStartDate);
                    workOrder.PlannedEndDate = DateTime.Parse(createWorkOrderVM.PlannedEndDate);
                    workOrder.ActualStartDate = DateTime.Parse(createWorkOrderVM.ActualStartDate);
                    workOrder.ActualEndDate = DateTime.Parse(createWorkOrderVM.ActualEndDate);
                    workOrder.Note = createWorkOrderVM.Note;
                    workOrder.CreatedById = createWorkOrderVM.CreatedById;
                    workOrder.WorkOrderPeriorityId = createWorkOrderVM.WorkOrderPeriorityId;
                    workOrder.WorkOrderTypeId = createWorkOrderVM.WorkOrderTypeId;
                    workOrder.RequestId = createWorkOrderVM.RequestId;
                    workOrder.HospitalId = createWorkOrderVM.HospitalId;
                    _context.WorkOrders.Add(workOrder);
                    _context.SaveChanges();
                    createWorkOrderVM.Id = workOrder.Id;
                }
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }
            return createWorkOrderVM.Id;
        }

        public void Delete(int id)
        {
            var workOrder = _context.WorkOrders.Find(id);
            try
            {
                if (workOrder != null)
                {

                    _context.WorkOrders.Remove(workOrder);
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }
        }


        private IQueryable<WorkOrderTracking> GetAllWorkOrders()
        {

            var workOrderTrackings = _context.WorkOrderTrackings.Include(a => a.WorkOrder).Include(a => a.WorkOrderStatus)
                         .Include(a => a.WorkOrder.WorkOrderPeriority).Include(a => a.WorkOrder.WorkOrderType)
                         .Include(a => a.User)
                         .Include(a => a.WorkOrder.Request)
                         .Include(a => a.WorkOrder.Request.AssetDetail)
                         .Include(a => a.WorkOrder.Request.AssetDetail.MasterAsset)
                         .Include(a => a.WorkOrder.Request.AssetDetail.MasterAsset.brand)
                         .Include(a => a.WorkOrder.Request.AssetDetail.Supplier)
                         .Include(a => a.WorkOrder.Request.AssetDetail.Department)
                         .Include(a => a.WorkOrder.Request.AssetDetail.MasterAsset.Origin)
                         .Include(a => a.WorkOrder.Request.AssetDetail.Building)
                         .Include(a => a.WorkOrder.Request.AssetDetail.Floor)
                         .Include(a => a.WorkOrder.Request.AssetDetail.Room)
                         .Include(a => a.WorkOrder.Request.AssetDetail.Hospital).ThenInclude(h => h.Organization)
                         .Include(a => a.WorkOrder.Request.AssetDetail.Hospital).ThenInclude(h => h.Governorate)
                         .Include(a => a.WorkOrder.Request.AssetDetail.Hospital).ThenInclude(h => h.City)
                         .Include(a => a.WorkOrder.Request.AssetDetail.Hospital).ThenInclude(h => h.SubOrganization)
                         .ToList()
                         .GroupBy(track => track.WorkOrderId)
                         .Select(g => g.OrderByDescending(track => track.WorkOrderDate).FirstOrDefault());
            return workOrderTrackings.AsQueryable();
        }

        public IndexWorkOrderVM2 ListWorkOrders(SortAndFilterWorkOrderVM data, int pageNumber, int pageSize)
        {
            #region Initial Variables
            IQueryable<WorkOrderTracking> query = GetAllWorkOrders();

            IndexWorkOrderVM2 mainClass = new IndexWorkOrderVM2();
            List<IndexWorkOrderVM2.GetData> list = new List<IndexWorkOrderVM2.GetData>();
            ApplicationUser userObj = new ApplicationUser();
            List<string> lstRoleNames = new List<string>();
            Employee empObj = new Employee();
            #endregion

            #region User Role

            if (data.SearchObj.UserId != null)
            {
                var getUserById = _context.ApplicationUser.Where(a => a.Id == data.SearchObj.UserId).ToList();
                userObj = getUserById[0];

                var roles = (from userRole in _context.UserRoles
                             join role in _context.ApplicationRole on userRole.RoleId equals role.Id
                             where userRole.UserId == userObj.Id
                             select role);
                foreach (var role in roles)
                {
                    lstRoleNames.Add(role.Name);
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
                if (!lstRoleNames.Any(s => s == "TLHospitalManager" || s == "EngDepManager" || s == "SRReviewer"))
                {
                    query = query.Where(a => a.CreatedById == userObj.Id && a.HospitalId == userObj.HospitalId);
                }
                if (lstRoleNames.Contains("TLHospitalManager") && lstRoleNames.Contains("EngDepManager") && lstRoleNames.Contains("SRReviewer") && lstRoleNames.Contains("SRCreator") && lstRoleNames.Contains("AssetOwner") && lstRoleNames.Contains("SRCreator"))
                {
                    query = query.Where(a => a.HospitalId == userObj.HospitalId);
                }
                if (lstRoleNames.Contains("TLHospitalManager") && !lstRoleNames.Contains("EngDepManager") && !lstRoleNames.Contains("SRReviewer") && !lstRoleNames.Contains("SRCreator") && !lstRoleNames.Contains("AssetOwner") && !lstRoleNames.Contains("SRCreator"))
                {
                    query = query.Where(a => a.HospitalId == userObj.HospitalId);
                }
                if (lstRoleNames.Contains("TLHospitalManager") && lstRoleNames.Contains("EngDepManager") && !lstRoleNames.Contains("SRReviewer") && !lstRoleNames.Contains("SRCreator") && !lstRoleNames.Contains("AssetOwner") && !lstRoleNames.Contains("SRCreator"))
                {
                    query = query.Where(a => a.HospitalId == userObj.HospitalId);
                }
                if (lstRoleNames.Contains("TLHospitalManager") && lstRoleNames.Contains("EngDepManager") && lstRoleNames.Contains("SRReviewer") && !lstRoleNames.Contains("SRCreator") && !lstRoleNames.Contains("AssetOwner") && !lstRoleNames.Contains("SRCreator"))
                {
                    query = query.Where(a => a.HospitalId == userObj.HospitalId);
                }
                if (lstRoleNames.Contains("TLHospitalManager") && lstRoleNames.Contains("EngDepManager") && lstRoleNames.Contains("SRReviewer") && lstRoleNames.Contains("SRCreator") && !lstRoleNames.Contains("AssetOwner") && !lstRoleNames.Contains("SRCreator"))
                {
                    query = query.Where(a => a.HospitalId == userObj.HospitalId);
                }
                if (lstRoleNames.Contains("TLHospitalManager") && lstRoleNames.Contains("EngDepManager") && lstRoleNames.Contains("SRReviewer") && lstRoleNames.Contains("SRCreator") && lstRoleNames.Contains("AssetOwner") && !lstRoleNames.Contains("SRCreator"))
                {
                    query = query.Where(a => a.HospitalId == userObj.HospitalId);
                }
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
            if (data.SearchObj.GovernorateId != 0)
            {
                query = query.Where(x => x.WorkOrder.Hospital.GovernorateId == data.SearchObj.GovernorateId);
            }
            if (data.SearchObj.CityId != 0)
            {
                query = query.Where(x => x.WorkOrder.Hospital.CityId == data.SearchObj.CityId);
            }
            if (data.SearchObj.OrganizationId != 0)
            {
                query = query.Where(x => x.WorkOrder.Hospital.OrganizationId == data.SearchObj.OrganizationId);
            }
            if (data.SearchObj.SubOrganizationId != 0)
            {
                query = query.Where(x => x.WorkOrder.Hospital.SubOrganizationId == data.SearchObj.SubOrganizationId);
            }
            if (data.SearchObj.HospitalId != 0)
            {
                query = query.Where(x => x.WorkOrder.HospitalId == data.SearchObj.HospitalId);
            }
            if (data.SearchObj.MasterAssetId != 0)
            {
                query = query.Where(x => x.WorkOrder.Request.AssetDetail.MasterAssetId == data.SearchObj.MasterAssetId);
            }
            if (!string.IsNullOrEmpty(data.SearchObj.BarCode))
            {
                query = query.Where(x => x.WorkOrder.Request.AssetDetail.Barcode.Contains(data.SearchObj.BarCode));
            }
            if (!string.IsNullOrEmpty(data.SearchObj.SerialNumber))
            {
                query = query.Where(x => x.WorkOrder.Request.AssetDetail.SerialNumber.Contains(data.SearchObj.SerialNumber));
            }
            if (data.SearchObj.ModelNumber != "")
            {
                query = query.Where(x => x.WorkOrder.Request.AssetDetail.MasterAsset.ModelNumber == data.SearchObj.ModelNumber);
            }
            if (data.SearchObj.DepartmentId != 0)
            {
                query = query.Where(x => x.WorkOrder.Request.AssetDetail.DepartmentId == data.SearchObj.DepartmentId);
            }
            if (data.SearchObj.PeriorityId != null)
            {
                query = query.Where(x => x.WorkOrder.Request.AssetDetail.MasterAsset.PeriorityId == data.SearchObj.PeriorityId);
            }
            if (data.SearchObj.WONumber != "")
            {
                query = query.Where(x => x.WorkOrder.WorkOrderNumber == data.SearchObj.WONumber);
            }
            if (data.SearchObj.Subject != "")
            {
                query = query.Where(x => x.WorkOrder.Subject == data.SearchObj.Subject);
            }
            if (data.SearchObj.RequestSubject != "")
            {
                query = query.Where(x => x.WorkOrder.Request.Subject == data.SearchObj.RequestSubject);
            }
            if (data.SearchObj.StatusId != 0)
            {
                query = query.Where(x => x.WorkOrderStatusId == data.SearchObj.StatusId);
            }
            else
            {
                query = query;
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
                query = query.Where(a => a.WorkOrder.CreationDate.Value.Date >= startingFrom.Date && a.WorkOrder.CreationDate.Value.Date <= endingTo.Date);
            }


            #endregion

            #region Sort Criteria

            switch (data.SortObj.SortBy)
            {
                case "BarCode":
                case "الباركود":
                    if (data.SortObj.SortStatus == "ascending")
                    {
                        query = query.OrderBy(x => x.WorkOrder.Request.AssetDetail.Barcode);
                    }
                    else
                    {
                        query = query.OrderByDescending(x => x.WorkOrder.Request.AssetDetail.Barcode);
                    }
                    break;
                case "Serial":
                case "السيريال":
                    if (data.SortObj.SortStatus == "ascending")
                    {
                        query = query.OrderBy(x => x.WorkOrder.Request.AssetDetail.SerialNumber);
                    }
                    else
                    {
                        query = query.OrderByDescending(x => x.WorkOrder.Request.AssetDetail.SerialNumber);
                    }
                    break;
                case "Model":
                case "الموديل":
                    if (data.SortObj.SortStatus == "ascending")
                    {
                        query = query.OrderBy(x => x.WorkOrder.Request.AssetDetail.MasterAsset.ModelNumber);
                    }
                    else
                    {
                        query = query.OrderByDescending(x => x.WorkOrder.Request.AssetDetail.MasterAsset.ModelNumber);
                    }
                    break;
                case "Subject":
                case "الموضوع":
                    if (data.SortObj.SortStatus == "ascending")
                    {
                        query = query.OrderBy(x => x.WorkOrder.Request.Subject);
                    }
                    else
                    {
                        query = query.OrderByDescending(x => x.WorkOrder.Request.Subject);
                    }
                    break;

                case "Date":
                case "التاريخ":
                    if (data.SortObj.SortStatus == "ascending")
                    {
                        query = query.OrderBy(x => x.WorkOrder.Request.RequestDate);
                    }
                    else
                    {
                        query = query.OrderByDescending(x => x.WorkOrder.Request.RequestDate);
                    }
                    break;

                case "Request Code":
                case "رقم الطلب":
                    if (data.SortObj.SortStatus == "ascending")
                    {
                        query = query.OrderBy(x => x.WorkOrder.Request.RequestCode);
                    }
                    else
                    {
                        query = query.OrderByDescending(x => x.WorkOrder.Request.RequestCode);
                    }
                    break;
                case "CreatedBy":
                case "تم بواسطة":
                    if (data.SortObj.SortStatus == "ascending")
                    {
                        query = query.OrderBy(x => x.User.UserName);
                    }
                    else
                    {
                        query = query.OrderByDescending(x => x.User.UserName);
                    }
                    break;
                case "Periority":
                    if (data.SortObj.SortStatus == "ascending")
                    {
                        query.OrderBy(x => x.WorkOrder.Request.RequestPeriority.Name);
                    }
                    else
                    {
                        query.OrderByDescending(x => x.WorkOrder.Request.RequestPeriority.Name);
                    }
                    break;

                case "الأولوية":
                    if (data.SortObj.SortStatus == "ascending")
                    {
                        query = query.OrderBy(x => x.WorkOrder.Request.RequestPeriority.NameAr);
                    }
                    else
                    {
                        query = query.OrderByDescending(x => x.WorkOrder.Request.RequestPeriority.NameAr);
                    }
                    break;
                case "Status":
                    if (data.SortObj.SortStatus == "ascending")
                    {
                        query.OrderBy(x => x.WorkOrderStatus.Name);
                    }
                    else
                    {
                        query.OrderByDescending(x => x.WorkOrderStatus.Name);
                    }
                    break;
                case "الحالة":
                    if (data.SortObj.SortStatus == "ascending")
                    {
                        query.OrderBy(x => x.WorkOrderStatus.NameAr);
                    }
                    else
                    {
                        query = query.OrderByDescending(x => x.WorkOrderStatus.NameAr);
                    }
                    break;
                case "Asset Name":
                    if (data.SortObj.SortStatus == "ascending")
                    {
                        query.OrderBy(x => x.WorkOrder.Request.AssetDetail.MasterAsset.Name);
                    }
                    else
                    {
                        query.OrderByDescending(x => x.WorkOrder.Request.AssetDetail.MasterAsset.Name);
                    }
                    break;
                case "اسم الأصل":
                    if (data.SortObj.SortStatus == "ascending")
                    {
                        query = query.OrderBy(x => x.WorkOrder.Request.AssetDetail.MasterAsset.NameAr);
                    }
                    else
                    {
                        query = query.OrderByDescending(x => x.WorkOrder.Request.AssetDetail.MasterAsset.NameAr);
                    }
                    break;

                case "Brands":
                    if (data.SortObj.SortStatus == "ascending")
                    {
                        query = query.OrderBy(x => x.WorkOrder.Request.AssetDetail.MasterAsset.brand.Name);
                    }
                    else
                    {
                        query = query.OrderByDescending(x => x.WorkOrder.Request.AssetDetail.MasterAsset.brand.Name);
                    }
                    break;
                case "الماركات":
                    if (data.SortObj.SortStatus == "ascending")
                    {
                        query = query.OrderBy(x => x.WorkOrder.Request.AssetDetail.MasterAsset.brand.NameAr);
                    }
                    else
                    {
                        query = query.OrderByDescending(x => x.WorkOrder.Request.AssetDetail.MasterAsset.brand.NameAr);
                    }
                    break;
                case "Department":
                    if (data.SortObj.SortStatus == "ascending")
                    {
                        query = query.OrderBy(x => x.WorkOrder.Request.AssetDetail.Department.Name);
                    }
                    else
                    {
                        query = query.OrderByDescending(x => x.WorkOrder.Request.AssetDetail.Department.Name);
                    }
                    break;
                case "القسم":
                    if (data.SortObj.SortStatus == "ascending")
                    {
                        query = query.OrderBy(x => x.WorkOrder.Request.AssetDetail.Department.NameAr);
                    }
                    else
                    {
                        query = query.OrderByDescending(x => x.WorkOrder.Request.AssetDetail.Department.NameAr);
                    }
                    break;

            }

            #endregion

            #region Count data and fiter By Paging
            IQueryable<WorkOrderTracking> lstResults;
            mainClass.Count = query.Count();
            if (pageNumber == 0 && pageSize == 0)
                lstResults = query;
            else
                lstResults = query.Skip((pageNumber - 1) * pageSize).Take(pageSize);
            #endregion

            #region Loop to get Items after serach and sort

            foreach (var woItem in lstResults.ToList())
            {
                IndexWorkOrderVM2.GetData getDataObj = new IndexWorkOrderVM2.GetData();
                getDataObj.Id = woItem.WorkOrderId;
                getDataObj.Subject = woItem.WorkOrder.Subject;
                getDataObj.BarCode = woItem.WorkOrder.Request.AssetDetail.Barcode;
                getDataObj.ModelNumber = woItem.WorkOrder.Request.AssetDetail.MasterAsset.ModelNumber;
                getDataObj.AssetName = woItem.WorkOrder.Request.AssetDetail.MasterAsset.Name;
                getDataObj.AssetNameAr = woItem.WorkOrder.Request.AssetDetail.MasterAsset.NameAr;
                getDataObj.SerialNumber = woItem.WorkOrder.Request.AssetDetail.SerialNumber;
                getDataObj.WorkOrderNumber = woItem.WorkOrder.WorkOrderNumber;
                if (woItem.WorkOrder.WorkOrderType != null)
                    getDataObj.WorkOrderTypeName = woItem.WorkOrder.WorkOrderType.Name;
                getDataObj.RequestSubject = woItem.WorkOrder.Request.Subject;
                getDataObj.AssetId = woItem.WorkOrder.Request.AssetDetailId;
                getDataObj.CreatedBy = woItem.User.UserName;
                getDataObj.ActualStartDate = woItem.ActualStartDate;
                getDataObj.ActualEndDate = woItem.ActualEndDate;
                getDataObj.HospitalId = woItem.WorkOrder.Request.AssetDetail.HospitalId;
                getDataObj.GovernorateId = woItem.WorkOrder.Request.AssetDetail.Hospital.GovernorateId;
                getDataObj.CityId = woItem.WorkOrder.Request.AssetDetail.Hospital.CityId;
                getDataObj.OrganizationId = woItem.WorkOrder.Request.AssetDetail.Hospital.OrganizationId;
                getDataObj.SubOrganizationId = woItem.WorkOrder.Request.AssetDetail.Hospital.SubOrganizationId;
                getDataObj.CreatedById = woItem.WorkOrder.CreatedById;
                getDataObj.CreationDate = woItem.WorkOrder.CreationDate;
                getDataObj.AssetName = woItem.WorkOrder.Request.AssetDetail.MasterAsset.Name + " - " + woItem.WorkOrder.Request.AssetDetail.SerialNumber;
                getDataObj.AssetNameAr = woItem.WorkOrder.Request.AssetDetail.MasterAsset.NameAr + " - " + woItem.WorkOrder.Request.AssetDetail.SerialNumber;
                getDataObj.MasterAssetId = woItem.WorkOrder.Request.AssetDetail.MasterAssetId;
                getDataObj.WorkOrderStatusId = woItem.WorkOrderStatusId;
                getDataObj.StatusName = woItem.WorkOrderStatus.Name;
                getDataObj.StatusNameAr = woItem.WorkOrderStatus.NameAr;
                getDataObj.statusColor = woItem.WorkOrderStatus.Color;
                getDataObj.statusIcon = woItem.WorkOrderStatus.Icon;
                getDataObj.Note = woItem.Notes;


                var firstTrack = _context.WorkOrderTrackings.Where(a => a.WorkOrderId == woItem.WorkOrder.Id && a.HospitalId == data.SearchObj.HospitalId).OrderBy(a => a.CreationDate.Value.Date).ToList();
                if (firstTrack.Count > 0 && getDataObj.WorkOrderStatusId < 12)
                    getDataObj.FirstTrackDate = firstTrack[0].WorkOrderDate;
                else if (firstTrack.Count > 0 && getDataObj.WorkOrderStatusId == 12)
                    getDataObj.FirstTrackDate = firstTrack.LastOrDefault().CreationDate;


                if (getDataObj.WorkOrderStatusId == 12)
                    getDataObj.ClosedDate = woItem.CreationDate;




                if (woItem.WorkOrder.Request.AssetDetailId != null)
                {
                    getDataObj.AssetName = woItem.WorkOrder.Request.AssetDetail.MasterAsset.Name;
                    getDataObj.AssetNameAr = woItem.WorkOrder.Request.AssetDetail.MasterAsset.NameAr;
                }


                if (woItem.WorkOrder.WorkOrderPeriority != null)
                {
                    getDataObj.WorkOrderPeriorityId = (int)woItem.WorkOrder.WorkOrderPeriorityId;
                    getDataObj.PeriorityName = woItem.WorkOrder.WorkOrderPeriority.Name;
                    getDataObj.PeriorityNameAr = woItem.WorkOrder.WorkOrderPeriority.NameAr;

                }
                if (woItem.WorkOrder.Request.AssetDetail.Department != null)
                {
                    getDataObj.DepartmentId = (int)woItem.WorkOrder.Request.AssetDetail.DepartmentId;
                    getDataObj.DepartmentName = woItem.WorkOrder.Request.AssetDetail.Department.Name;
                    getDataObj.DepartmentNameAr = woItem.WorkOrder.Request.AssetDetail.Department.NameAr;

                }
                list.Add(getDataObj);
            }
            #endregion

            #region Represent data after Paging and count
            mainClass.Results = list;
            #endregion
            return mainClass;

        }



        #endregion

        #region WorkOrder Attachments Functions

        public int CreateWorkOrderAttachments(WorkOrderAttachment attachObj)
        {
            WorkOrderAttachment documentObj = new WorkOrderAttachment();
            documentObj.DocumentName = attachObj.DocumentName;
            documentObj.FileName = attachObj.FileName;
            documentObj.WorkOrderTrackingId = attachObj.WorkOrderTrackingId;
            documentObj.HospitalId = attachObj.HospitalId;
            _context.WorkOrderAttachments.Add(documentObj);
            _context.SaveChanges();
            return attachObj.Id;
        }


        #endregion

        #region WorkOrder Report Functions

        public IEnumerable<IndexWorkOrderVM> GetWorkOrdersByDate(SearchWorkOrderByDateVM woDateObj)
        {
            List<IndexWorkOrderVM> list = new List<IndexWorkOrderVM>();
            ApplicationUser UserObj = new ApplicationUser();
            ApplicationRole roleObj = new ApplicationRole();
            List<string> userRoleNames = new List<string>();

            var obj = _context.ApplicationUser.Where(a => a.Id == woDateObj.UserId).ToList();
            if (obj.Count > 0)
            {
                UserObj = obj[0];

                var roles = (from userRole in _context.UserRoles
                             join role in _context.ApplicationRole on userRole.RoleId equals role.Id
                             where userRole.UserId == woDateObj.UserId
                             select role);
                foreach (var role in roles)
                {
                    userRoleNames.Add(role.Name);
                }
                var lstWorkOrders = _context.WorkOrderTrackings
                     .Include(t => t.WorkOrder)
                     .Include(t => t.WorkOrderStatus)
                      .Include(w => w.WorkOrder.WorkOrderType)
                      .Include(w => w.WorkOrder.WorkOrderPeriority)
                      .Include(w => w.WorkOrder.Request)
                      .Include(w => w.WorkOrder.Request.AssetDetail)
                      .Include(w => w.WorkOrder.Request.AssetDetail.MasterAsset)
                      .Include(w => w.User)
                      .ToList().OrderByDescending(a => a.WorkOrder.CreationDate).ToList().GroupBy(a => a.WorkOrder.Id).ToList();



                foreach (var item in lstWorkOrders)
                {
                    IndexWorkOrderVM work = new IndexWorkOrderVM();
                    work.Id = item.FirstOrDefault().Id;
                    work.WorkOrderNumber = item.FirstOrDefault().WorkOrder.WorkOrderNumber;
                    work.BarCode = item.FirstOrDefault().WorkOrder.Request.AssetDetail.Barcode;
                    work.ModelNumber = item.FirstOrDefault().WorkOrder.Request.AssetDetail.MasterAsset.ModelNumber;
                    work.AssetName = item.FirstOrDefault().WorkOrder.Request.AssetDetail.MasterAsset.Name;
                    work.AssetNameAr = item.FirstOrDefault().WorkOrder.Request.AssetDetail.MasterAsset.NameAr;
                    work.SerialNumber = item.FirstOrDefault().WorkOrder.Request.AssetDetail.SerialNumber;
                    work.Subject = item.FirstOrDefault().WorkOrder.Subject;
                    work.RequestSubject = item.FirstOrDefault().WorkOrder.Request.Subject;
                    work.CreationDate = item.FirstOrDefault().CreationDate;
                    work.Note = item.FirstOrDefault().Notes;
                    work.CreatedById = item.FirstOrDefault().CreatedById;
                    work.CreatedBy = item.FirstOrDefault().User.UserName;
                    work.TypeName = item.FirstOrDefault().WorkOrder.WorkOrderType.Name;
                    work.TypeNameAr = item.FirstOrDefault().WorkOrder.WorkOrderType.NameAr;
                    work.PeriorityName = item.FirstOrDefault().WorkOrder.WorkOrderPeriority != null ? item.FirstOrDefault().WorkOrder.WorkOrderPeriority.Name : "";
                    work.PeriorityNameAr = item.FirstOrDefault().WorkOrder.WorkOrderPeriority != null ? item.FirstOrDefault().WorkOrder.WorkOrderPeriority.NameAr : "";
                    work.AssignedTo = item.FirstOrDefault().AssignedTo;


                    var lstStatus = _context.WorkOrderTrackings
                           .Include(t => t.WorkOrder).Include(t => t.WorkOrderStatus)
                           .Where(a => a.WorkOrderId == item.FirstOrDefault().Id).ToList()
                           .OrderByDescending(a => a.WorkOrderDate).ToList().GroupBy(a => item.FirstOrDefault().Id).ToList();
                    if (lstStatus.Count > 0)
                    {
                        var trackObj = lstStatus[0];
                        // work.WorkOrderStatusId = item.FirstOrDefault().WorkOrderStatus.Id;
                        // work.StatusId = item.LastOrDefault().WorkOrderStatus.Id;
                        //work.StatusName = item.FirstOrDefault().WorkOrderStatus.Name;
                        //work.StatusNameAr = item.FirstOrDefault().WorkOrderStatus.NameAr;
                        //work.statusColor = item.FirstOrDefault().WorkOrderStatus.Color;
                        //work.statusIcon = item.FirstOrDefault().WorkOrderStatus.Icon;


                        work.WorkOrderStatusId = trackObj.FirstOrDefault().WorkOrderStatus.Id;
                        work.StatusId = trackObj.FirstOrDefault().WorkOrderStatusId;
                        work.StatusName = trackObj.FirstOrDefault().WorkOrderStatus.Name;
                        work.StatusNameAr = trackObj.FirstOrDefault().WorkOrderStatus.NameAr;
                        work.statusColor = trackObj.FirstOrDefault().WorkOrderStatus.Color;
                        work.statusIcon = trackObj.FirstOrDefault().WorkOrderStatus.Icon;


                        work.Note = trackObj.FirstOrDefault().Notes;
                        if (work.WorkOrderStatusId == 12)
                        {
                            work.TrackCreationDate = trackObj.FirstOrDefault().CreationDate.ToString();
                            work.ClosedDate = trackObj.FirstOrDefault().CreationDate;
                        }
                        else
                        {
                            work.TrackCreationDate = "";
                            work.ClosedDate = null;
                        }



                    }
                    work.ActualStartDate = item.FirstOrDefault().ActualStartDate;
                    work.ActualEndDate = item.FirstOrDefault().ActualEndDate;
                    work.RequestId = item.FirstOrDefault().WorkOrder.RequestId != null ? (int)item.FirstOrDefault().WorkOrder.RequestId : 0;
                    work.HospitalId = item.FirstOrDefault().WorkOrder.Request.AssetDetail.HospitalId;
                    if (woDateObj.UserId != null)
                    {
                        var lstAssigned = _context.WorkOrderTrackings.Where(a => a.AssignedTo == woDateObj.UserId && a.WorkOrderId == work.Id).ToList();
                        if (lstAssigned.Count > 0)
                        {
                            work.AssignedTo = lstAssigned[0].AssignedTo;
                        }
                    }
                    list.Add(work);
                }


                if (woDateObj.StatusId > 0)
                {
                    list = list.Where(a => a.StatusId == woDateObj.StatusId).ToList();
                }
                else
                {
                    list = list.ToList();
                }

                list = list.Where(a => a.CreationDate.HasValue).ToList();

                DateTime? start = DateTime.Parse(woDateObj.StrStartDate);
                DateTime? end = DateTime.Parse(woDateObj.StrEndDate);
                list = list.Where(a => a.CreationDate.Value.Date >= start.Value.Date && a.CreationDate.Value.Date <= end.Value.Date).ToList();


                if (list.Count > 0)
                {

                    if (userRoleNames.Contains("Admin"))
                    {
                        list = list.ToList();
                    }
                    if (userRoleNames.Contains("EngDepManager") && userRoleNames.Contains("Eng"))
                    {
                        list = list.Where(t => t.HospitalId == UserObj.HospitalId).ToList();
                    }
                    if (userRoleNames.Contains("EngDepManager") && !userRoleNames.Contains("Eng"))
                    {
                        list = list.Where(t => t.HospitalId == UserObj.HospitalId).ToList();
                    }
                    if (userRoleNames.Contains("Eng") && !userRoleNames.Contains("EngDepManager"))
                    {
                        list = list.Where(a => a.HospitalId == woDateObj.HospitalId && a.AssignedTo == woDateObj.UserId).ToList();
                    }
                }

            }
            return list;
        }

        #endregion


        #region Mobile Functions
        public IndexWorkOrderVM GetMobileWorkOrderByRequestUserId(int requestId, string userId)
        {

            IndexWorkOrderVM workOrderVM = new IndexWorkOrderVM();

            ApplicationUser UserObj = new ApplicationUser();
            ApplicationRole roleObj = new ApplicationRole();
            string userRoleName = "";
            var obj = _context.ApplicationUser.Where(a => a.Id == userId).ToList();
            if (obj.Count > 0)
            {
                UserObj = obj[0];

                var lstRoles = _context.ApplicationRole.Where(a => a.Id == UserObj.RoleId).ToList();
                if (lstRoles.Count > 0)
                {
                    roleObj = lstRoles[0];
                    userRoleName = roleObj.Name;
                }
            }

            var lstWorkOrders = _context.WorkOrders
                                .Include(w => w.Request)
                                .Include(w => w.WorkOrderType)
                                .Include(w => w.WorkOrderPeriority)
                                .Include(w => w.User)
                                .Where(a => a.RequestId == requestId).ToList();

            if (lstWorkOrders.Count > 0)
            {
                var workOrderDBObj = lstWorkOrders[0];
                workOrderVM.Id = workOrderDBObj.Id;
                workOrderVM.Subject = workOrderDBObj.Subject;
                workOrderVM.UserName = workOrderDBObj.User.UserName;
                workOrderVM.WorkOrderNumber = workOrderDBObj.WorkOrderNumber;
                workOrderVM.CreationDate = workOrderDBObj.CreationDate;
                workOrderVM.Note = workOrderDBObj.Note;
                workOrderVM.WorkOrderPeriorityName = workOrderDBObj.WorkOrderPeriority.Name;
                workOrderVM.WorkOrderTypeName = workOrderDBObj.WorkOrderType.Name;
                workOrderVM.RequestId = workOrderDBObj.RequestId != null ? (int)workOrderDBObj.RequestId : 0;
                workOrderVM.RequestSubject = workOrderDBObj.Request.Subject;
                workOrderVM.HospitalId = workOrderDBObj.HospitalId;

                workOrderVM.ListTracks = _context.WorkOrderTrackings.Include(a => a.WorkOrderStatus).Where(a => a.WorkOrderId == workOrderDBObj.Id)
                      .ToList().Select(item => new LstWorkOrderFromTracking
                      {
                          Id = item.Id,
                          StatusName = item.WorkOrderStatus.Name,
                          StatusNameAr = item.WorkOrderStatus.NameAr,
                          Notes = item.Notes,
                          WorkOrderDate = DateTime.Parse(item.WorkOrderDate.ToString()),
                          WorkOrderStatusId = item.WorkOrderStatusId,
                          WorkOrderStatusColor = item.WorkOrderStatus.Color
                      }).ToList();
                return workOrderVM;
            }
            return workOrderVM;
        }
        #endregion



        #region Export And Print
        /// <summary>
        /// Print PDF for list of checked WorkOrder
        /// </summary>
        /// <param name="workOrders"></param>
        /// <returns></returns>
        public List<IndexWorkOrderVM2.GetData> PrintListOfWorkOrders(List<ExportWorkOrderVM> workOrders)
        {
            List<IndexWorkOrderVM2.GetData> lstData = new List<IndexWorkOrderVM2.GetData>();

            var list = _context.WorkOrders.Include(w => w.Request).Include(a => a.Request.AssetDetail)
               .Include(a => a.Request.RequestType).Include(a => a.Request.SubProblem).Include(a => a.Request.SubProblem.Problem)
               .Include(a => a.Request.RequestMode)
               .Include(a => a.Request.AssetDetail.Department)
               .Include(a => a.Request.AssetDetail)
               .Include(a => a.Request.AssetDetail.Hospital)
               .Include(a => a.Request.AssetDetail.MasterAsset)
               .Include(w => w.WorkOrderType).Include(w => w.WorkOrderPeriority).Include(w => w.User).ToList();




            foreach (var item in list)
            {
                IndexWorkOrderVM2.GetData getDataObj = new IndexWorkOrderVM2.GetData();
                getDataObj.Id = item.Id;
                getDataObj.Subject = item.Subject;
                getDataObj.BarCode = item.Request.AssetDetail.Barcode;
                getDataObj.ModelNumber = item.Request.AssetDetail.MasterAsset.ModelNumber;
                getDataObj.AssetName = item.Request.AssetDetail.MasterAsset.Name;
                getDataObj.AssetNameAr = item.Request.AssetDetail.MasterAsset.NameAr;
                getDataObj.SerialNumber = item.Request.AssetDetail.SerialNumber;
                getDataObj.WorkOrderNumber = item.WorkOrderNumber;
                getDataObj.WorkOrderTypeName = item.WorkOrderType.Name;
                getDataObj.RequestSubject = item.Request.Subject;
                getDataObj.AssetId = item.Request.AssetDetailId;
                getDataObj.CreatedById = item.CreatedById;
                getDataObj.CreatedBy = item.User.UserName;
                getDataObj.ActualStartDate = item.ActualStartDate;
                getDataObj.ActualEndDate = item.ActualEndDate;
                getDataObj.PlannedStartDate = item.PlannedStartDate;
                getDataObj.PlannedEndDate = item.PlannedEndDate;
                getDataObj.WorkOrderPeriorityId = item.WorkOrderPeriorityId != null ? (int)item.WorkOrderPeriorityId : 0;
                getDataObj.WorkOrderPeriorityName = item.WorkOrderPeriority != null ? item.WorkOrderPeriority.Name : "";
                getDataObj.HospitalId = item.Request.AssetDetail.HospitalId;
                getDataObj.GovernorateId = item.Request.AssetDetail.Hospital.GovernorateId;
                getDataObj.CityId = item.Request.AssetDetail.Hospital.CityId;
                getDataObj.OrganizationId = item.Request.AssetDetail.Hospital.OrganizationId;
                getDataObj.SubOrganizationId = item.Request.AssetDetail.Hospital.SubOrganizationId;
                getDataObj.CreatedById = item.CreatedById;
                getDataObj.CreationDate = item.CreationDate;
                getDataObj.AssetName = item.Request.AssetDetail.MasterAsset.Name + " - " + item.Request.AssetDetail.SerialNumber;
                getDataObj.AssetNameAr = item.Request.AssetDetail.MasterAsset.NameAr + " - " + item.Request.AssetDetail.SerialNumber;
                getDataObj.MasterAssetId = item.Request.AssetDetail.MasterAssetId;
                //   getDataObj.Note = item.Note;
                var lstStatus = _context.WorkOrderTrackings
                            .Include(t => t.WorkOrder).Include(t => t.WorkOrderStatus)
                            .Where(a => a.WorkOrderId == item.Id).ToList().OrderByDescending(a => a.WorkOrderDate).ToList();
                if (lstStatus.Count > 0)
                {

                    getDataObj.WorkOrderStatusId = (int)lstStatus[0].WorkOrderStatus.Id;
                    getDataObj.StatusName = lstStatus[0].WorkOrderStatus.Name;
                    getDataObj.StatusNameAr = lstStatus[0].WorkOrderStatus.NameAr;
                    getDataObj.statusColor = lstStatus[0].WorkOrderStatus.Color;
                    getDataObj.statusIcon = lstStatus[0].WorkOrderStatus.Icon;
                    getDataObj.ClosedDate = lstStatus[0].CreationDate;
                    getDataObj.Note = lstStatus[0].Notes;
                }

                if (item.Request.AssetDetailId != null)
                {
                    getDataObj.AssetName = item.Request.AssetDetail.MasterAsset.Name;
                    getDataObj.AssetNameAr = item.Request.AssetDetail.MasterAsset.NameAr;
                }


                if (item.WorkOrderPeriorityId != null)
                {
                    getDataObj.WorkOrderPeriorityId = (int)item.WorkOrderPeriorityId;
                    getDataObj.PeriorityName = item.WorkOrderPeriority.Name;
                    getDataObj.PeriorityNameAr = item.WorkOrderPeriority.NameAr;

                }
                if (item.Request.AssetDetail.DepartmentId != null)
                {
                    getDataObj.DepartmentId = (int)item.Request.AssetDetail.DepartmentId;
                    getDataObj.DepartmentName = item.Request.AssetDetail.Department.Name;
                    getDataObj.DepartmentNameAr = item.Request.AssetDetail.Department.NameAr;

                }
                lstData.Add(getDataObj);
            }
            return lstData;
        }


        /// <summary>
        /// Print WorkOrder Item
        /// </summary>
        /// <param name="printWorkOrderObj"></param>
        /// <returns></returns>
        public List<IndexWorkOrderVM2.GetData> PrintListOfWorkOrders(PrintWorkOrderVM printWorkOrderObj)
        {
            List<IndexWorkOrderVM2.GetData> lstData = new List<IndexWorkOrderVM2.GetData>();

            var list = _context.WorkOrders.Include(w => w.Request).Include(a => a.Request.AssetDetail)
               .Include(a => a.Request.RequestType).Include(a => a.Request.SubProblem).Include(a => a.Request.SubProblem.Problem)
               .Include(a => a.Request.RequestMode)
               .Include(a => a.Request.AssetDetail.Department)
               .Include(a => a.Request.AssetDetail)
               .Include(a => a.Request.AssetDetail.Hospital)
               .Include(a => a.Request.AssetDetail.MasterAsset)
               .Include(w => w.WorkOrderType).Include(w => w.WorkOrderPeriority).Include(w => w.User).ToList();

            foreach (var item in list)
            {
                IndexWorkOrderVM2.GetData getDataObj = new IndexWorkOrderVM2.GetData();
                getDataObj.Id = item.Id;
                getDataObj.Subject = item.Subject;
                getDataObj.BarCode = item.Request.AssetDetail.Barcode;
                getDataObj.ModelNumber = item.Request.AssetDetail.MasterAsset.ModelNumber;
                getDataObj.AssetName = item.Request.AssetDetail.MasterAsset.Name;
                getDataObj.AssetNameAr = item.Request.AssetDetail.MasterAsset.NameAr;
                getDataObj.SerialNumber = item.Request.AssetDetail.SerialNumber;
                getDataObj.WorkOrderNumber = item.WorkOrderNumber;
                getDataObj.WorkOrderTypeName = item.WorkOrderType.Name;
                getDataObj.RequestSubject = item.Request.Subject;
                getDataObj.AssetId = item.Request.AssetDetailId;
                getDataObj.CreatedById = item.CreatedById;
                getDataObj.CreatedBy = item.User.UserName;
                getDataObj.ActualStartDate = item.ActualStartDate;
                getDataObj.ActualEndDate = item.ActualEndDate;
                getDataObj.PlannedStartDate = item.PlannedStartDate;
                getDataObj.PlannedEndDate = item.PlannedEndDate;
                getDataObj.WorkOrderPeriorityId = item.WorkOrderPeriorityId != null ? (int)item.WorkOrderPeriorityId : 0;
                getDataObj.WorkOrderPeriorityName = item.WorkOrderPeriority != null ? item.WorkOrderPeriority.Name : "";
                getDataObj.HospitalId = item.Request.AssetDetail.HospitalId;
                getDataObj.GovernorateId = item.Request.AssetDetail.Hospital.GovernorateId;
                getDataObj.CityId = item.Request.AssetDetail.Hospital.CityId;
                getDataObj.OrganizationId = item.Request.AssetDetail.Hospital.OrganizationId;
                getDataObj.SubOrganizationId = item.Request.AssetDetail.Hospital.SubOrganizationId;
                getDataObj.CreatedById = item.CreatedById;
                getDataObj.CreationDate = item.CreationDate;
                getDataObj.AssetName = item.Request.AssetDetail.MasterAsset.Name + " - " + item.Request.AssetDetail.SerialNumber;
                getDataObj.AssetNameAr = item.Request.AssetDetail.MasterAsset.NameAr + " - " + item.Request.AssetDetail.SerialNumber;
                getDataObj.MasterAssetId = item.Request.AssetDetail.MasterAssetId;
                //getDataObj.Note = item.Note;
                var lstStatus = _context.WorkOrderTrackings
                            .Include(t => t.WorkOrder).Include(t => t.WorkOrderStatus)
                            .Where(a => a.WorkOrderId == item.Id).ToList().OrderByDescending(a => a.WorkOrderDate).ToList();
                if (lstStatus.Count > 0)
                {

                    getDataObj.WorkOrderStatusId = (int)lstStatus[0].WorkOrderStatus.Id;
                    getDataObj.StatusName = lstStatus[0].WorkOrderStatus.Name;
                    getDataObj.StatusNameAr = lstStatus[0].WorkOrderStatus.NameAr;
                    getDataObj.statusColor = lstStatus[0].WorkOrderStatus.Color;
                    getDataObj.statusIcon = lstStatus[0].WorkOrderStatus.Icon;
                    getDataObj.ClosedDate = lstStatus[0].CreationDate;
                    getDataObj.Note = lstStatus[0].Notes;

                }

                if (item.Request.AssetDetailId != null)
                {
                    getDataObj.AssetName = item.Request.AssetDetail.MasterAsset.Name;
                    getDataObj.AssetNameAr = item.Request.AssetDetail.MasterAsset.NameAr;
                }


                if (item.WorkOrderPeriorityId != null)
                {
                    getDataObj.WorkOrderPeriorityId = (int)item.WorkOrderPeriorityId;
                    getDataObj.PeriorityName = item.WorkOrderPeriority.Name;
                    getDataObj.PeriorityNameAr = item.WorkOrderPeriority.NameAr;

                }
                if (item.Request.AssetDetail.DepartmentId != null)
                {
                    getDataObj.DepartmentId = (int)item.Request.AssetDetail.DepartmentId;
                    getDataObj.DepartmentName = item.Request.AssetDetail.Department.Name;
                    getDataObj.DepartmentNameAr = item.Request.AssetDetail.Department.NameAr;
                }
                lstData.Add(getDataObj);
            }
            return lstData;
        }
        #endregion



        public IEnumerable<IndexWorkOrderVM> GetAllWorkOrdersByHospitalId(int? hospitalId)
        {
            List<IndexWorkOrderVM> list = new List<IndexWorkOrderVM>();
            var lstWorkOrders = _context.WorkOrders
                      .Include(w => w.WorkOrderType)
                      .Include(w => w.WorkOrderPeriority)
                      .Include(w => w.Request)
                      .Include(w => w.Request.AssetDetail)
                      .Include(w => w.Request.AssetDetail.MasterAsset)
                    .Include(w => w.Request.AssetDetail.MasterAsset.brand)
                      .Include(w => w.Request.AssetDetail.Department)
                      .Include(w => w.User).OrderByDescending(a => a.CreationDate).ToList().GroupBy(a => a.Id).ToList();

            if (hospitalId != 0)
            {
                lstWorkOrders = lstWorkOrders.Where(t => t.FirstOrDefault().HospitalId == hospitalId).ToList();
            }
            if (lstWorkOrders.Count > 0)
            {
                foreach (var item in lstWorkOrders)
                {
                    IndexWorkOrderVM work = new IndexWorkOrderVM();
                    work.Id = item.FirstOrDefault().Id;
                    work.AssetName = item.FirstOrDefault().Request.AssetDetail.MasterAsset.Name;
                    work.AssetNameAr = item.FirstOrDefault().Request.AssetDetail.MasterAsset.NameAr;
                    work.SerialNumber = item.FirstOrDefault().Request.AssetDetail.SerialNumber;
                    work.BarCode = item.FirstOrDefault().Request.AssetDetail.Barcode;
                    work.ModelNumber = item.FirstOrDefault().Request.AssetDetail.MasterAsset.ModelNumber;
                    work.WorkOrderNumber = item.FirstOrDefault().WorkOrderNumber;
                    work.ModelNumber = item.FirstOrDefault().Request.AssetDetail.MasterAsset.ModelNumber;
                    work.Subject = item.FirstOrDefault().Subject;
                    work.RequestSubject = item.FirstOrDefault().Request.Subject;
                    work.CreationDate = item.FirstOrDefault().CreationDate;
                    work.Note = item.FirstOrDefault().Note;
                    work.CreatedById = item.FirstOrDefault().CreatedById;
                    work.CreatedBy = item.FirstOrDefault().User.UserName;
                    work.BarCode = item.FirstOrDefault().Request.AssetDetail.Barcode;
                    work.TypeName = item.FirstOrDefault().WorkOrderType != null ? item.FirstOrDefault().WorkOrderType.Name : "";
                    work.TypeNameAr = item.FirstOrDefault().WorkOrderType != null ? item.FirstOrDefault().WorkOrderType.NameAr : "";
                    work.PeriorityName = item.FirstOrDefault().WorkOrderPeriority != null ? item.FirstOrDefault().WorkOrderPeriority.Name : "";
                    work.PeriorityNameAr = item.FirstOrDefault().WorkOrderPeriority != null ? item.FirstOrDefault().WorkOrderPeriority.NameAr : "";

                    if (item.FirstOrDefault().Request.AssetDetail.MasterAsset.brand != null)
                    {
                        work.BrandName = item.FirstOrDefault().Request.AssetDetail.MasterAsset.brand.Name;
                        work.BrandNameAr = item.FirstOrDefault().Request.AssetDetail.MasterAsset.brand.NameAr;
                    }

                    if (item.FirstOrDefault().Request.AssetDetail.Department != null)
                    {
                        work.DepartmentName = item.FirstOrDefault().Request.AssetDetail.Department.Name;
                        work.DepartmentNameAr = item.FirstOrDefault().Request.AssetDetail.Department.NameAr;
                    }

                    var lstAssignTo = _context.WorkOrderTrackings.Where(a => a.WorkOrderId == item.FirstOrDefault().Id).ToList().OrderByDescending(a => a.WorkOrderDate).ToList().GroupBy(a => item.FirstOrDefault().Id).ToList();
                    var lstStatus = _context.WorkOrderTrackings
                           .Include(t => t.WorkOrder).Include(t => t.WorkOrderStatus)
                           .Where(a => a.WorkOrderId == item.FirstOrDefault().Id).ToList().OrderByDescending(a => a.WorkOrderDate).ToList().GroupBy(a => item.FirstOrDefault().Id).ToList();
                    if (lstStatus.Count > 0)
                    {

                        work.AssignedTo = lstStatus[0].FirstOrDefault().AssignedTo;
                        work.WorkOrderStatusId = lstStatus[0].FirstOrDefault().WorkOrderStatus.Id;

                        if (work.WorkOrderStatusId == 3 || work.WorkOrderStatusId == 4 || work.WorkOrderStatusId == 5)
                        {
                            var pendingStatus = _context.WorkOrderStatuses.Where(a => a.Id == 6).ToList().FirstOrDefault();
                            work.WorkOrderStatusId = lstStatus[0].FirstOrDefault().WorkOrderStatus.Id;

                            work.StatusName = lstStatus[0].FirstOrDefault().WorkOrderStatus.Name + " - " + pendingStatus.Name;
                            work.StatusNameAr = lstStatus[0].FirstOrDefault().WorkOrderStatus.NameAr + " - " + pendingStatus.NameAr;
                            work.statusColor = lstStatus[0].FirstOrDefault().WorkOrderStatus.Color;
                            work.statusIcon = lstStatus[0].FirstOrDefault().WorkOrderStatus.Icon;
                        }
                        else
                        {
                            work.WorkOrderStatusId = lstStatus[0].FirstOrDefault().WorkOrderStatus.Id;
                            work.StatusName = lstStatus[0].FirstOrDefault().WorkOrderStatus.Name;
                            work.StatusNameAr = lstStatus[0].FirstOrDefault().WorkOrderStatus.NameAr;
                            work.statusColor = lstStatus[0].FirstOrDefault().WorkOrderStatus.Color;
                            work.statusIcon = lstStatus[0].FirstOrDefault().WorkOrderStatus.Icon;
                        }
                        if (work.WorkOrderStatusId == 12)
                        {
                            work.ClosedDate = lstStatus[0].FirstOrDefault().ActualEndDate;
                        }
                    }


                    var lstClosedDate = _context.WorkOrderTrackings
                        .Include(t => t.WorkOrder).Include(t => t.WorkOrderStatus)
                        .Where(a => a.WorkOrderId == item.FirstOrDefault().Id && a.WorkOrderStatusId == 12).ToList().OrderByDescending(a => a.WorkOrderDate).ToList().GroupBy(a => item.FirstOrDefault().Id).ToList();
                    if (lstClosedDate.Count > 0)
                    {
                        work.ClosedDate = lstClosedDate[0].FirstOrDefault().CreationDate;
                    }
                    work.ActualStartDate = item.FirstOrDefault().ActualStartDate;
                    work.ActualEndDate = item.FirstOrDefault().ActualEndDate;
                    work.RequestId = item.FirstOrDefault().RequestId != null ? (int)item.FirstOrDefault().RequestId : 0;
                    work.HospitalId = item.FirstOrDefault().Request.AssetDetail.HospitalId;
                    work.ListTracks = _context.WorkOrderTrackings.Include(a => a.WorkOrderStatus).Where(a => a.WorkOrderId == work.Id)
                    .ToList().Select(item => new LstWorkOrderFromTracking
                    {
                        Id = item.Id,
                        StatusName = item.WorkOrderStatus.Name,
                        StatusNameAr = item.WorkOrderStatus.NameAr
                    }).ToList();

                    list.Add(work);
                }
            }

            return list;
        }


        #endregion
    }
}
