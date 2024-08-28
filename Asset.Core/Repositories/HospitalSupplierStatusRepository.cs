﻿using Asset.Domain.Repositories;
using Asset.Models;
using Asset.ViewModels.HospitalSupplierStatusVM;
using Asset.ViewModels.RequestStatusVM;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset.Core.Repositories
{
    public class HospitalSupplierStatusRepository : IHospitalSupplierStatusRepository
    {
        private ApplicationDbContext _context;

        public HospitalSupplierStatusRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public IndexHospitalSupplierStatusVM GetAllByHospitals()
        {
            IndexHospitalSupplierStatusVM ItemObj = new IndexHospitalSupplierStatusVM();
            var list = _context.HospitalSupplierStatuses.ToList();
            ItemObj.ListStatus = list;
            foreach (var itm in list)
            {
                var lstHospitalStatus = _context.HospitalApplications.Include(a => a.AssetDetail).Where(a => a.StatusId == itm.Id).ToList();

                if (itm.Id == 1)
                {
                    ItemObj.OpenStatus = lstHospitalStatus.Count;
                }
                if (itm.Id == 2)
                {
                    ItemObj.ApproveStatus = lstHospitalStatus.Count;
                }
                if (itm.Id == 3)
                {
                    ItemObj.RejectStatus = lstHospitalStatus.Count;
                }
                if (itm.Id == 4)
                {
                    ItemObj.SystemRejectStatus = lstHospitalStatus.Count;
                }
            }
            ItemObj.TotalStatus = ItemObj.OpenStatus + ItemObj.ApproveStatus + ItemObj.RejectStatus + ItemObj.SystemRejectStatus;
            return ItemObj;
        }

        public IndexHospitalSupplierStatusVM GetAllByHospitals(int statusId, int appTypeId, int? hospitalId)
        {
            IndexHospitalSupplierStatusVM ItemObj = new IndexHospitalSupplierStatusVM();
            var list = _context.HospitalSupplierStatuses.ToList();

            List<HospitalApplication> lstHospitalStatus = new List<HospitalApplication>();
            if (hospitalId != 0)
            {

                foreach (var itm in list)
                {

                    lstHospitalStatus = _context.HospitalApplications.Include(a => a.AssetDetail).Include(a => a.AssetDetail.Hospital)
                    .Where(a => a.AssetDetail.HospitalId == hospitalId && a.AppTypeId == appTypeId && a.StatusId == itm.Id).ToList();
                    if (itm.Id == 1)
                    {
                        ItemObj.OpenStatus = lstHospitalStatus.Count;
                    }
                    if (itm.Id == 2)
                    {
                        ItemObj.ApproveStatus = lstHospitalStatus.Count;
                    }
                    if (itm.Id == 3)
                    {
                        ItemObj.RejectStatus = lstHospitalStatus.Count;
                    }
                    if (itm.Id == 4)
                    {
                        ItemObj.SystemRejectStatus = lstHospitalStatus.Count;
                    }
                }

                ItemObj.TotalStatus = ItemObj.OpenStatus + ItemObj.ApproveStatus + ItemObj.RejectStatus + ItemObj.SystemRejectStatus;

            }
            else
            {

                foreach (var itm in list)
                {
                    lstHospitalStatus = _context.HospitalApplications.Include(a => a.AssetDetail).Include(a => a.AssetDetail.Hospital).ToList().Where(a => a.StatusId == itm.Id && a.AppTypeId == appTypeId).ToList();

                    if (itm.Id == 1)
                    {
                        ItemObj.OpenStatus = lstHospitalStatus.Count;
                    }
                    if (itm.Id == 2)
                    {
                        ItemObj.ApproveStatus = lstHospitalStatus.Count;
                    }
                    if (itm.Id == 3)
                    {
                        ItemObj.RejectStatus = lstHospitalStatus.Count;
                    }
                    if (itm.Id == 4)
                    {
                        ItemObj.SystemRejectStatus = lstHospitalStatus.Count;
                    }
                }
                ItemObj.TotalStatus = ItemObj.OpenStatus + ItemObj.ApproveStatus + ItemObj.RejectStatus + ItemObj.SystemRejectStatus;
            }
            ItemObj.ListStatus = list;
            return ItemObj;
        }




        public IndexHospitalSupplierStatusVM GetAll(int statusId, int appTypeId, int? hospitalId)
        {

            IndexHospitalSupplierStatusVM ItemObj = new IndexHospitalSupplierStatusVM();
            var list = _context.HospitalSupplierStatuses.ToList();
            ItemObj.ListStatus = list;


            ApplicationUser UserObj = new ApplicationUser();
            List<string> userRoleNames = new List<string>();

            var obj = _context.ApplicationUser.Where(a => a.Id == "").ToList();
            if (obj.Count > 0)
            {
                UserObj = obj[0];

                var roles = (from userRole in _context.UserRoles
                             join role in _context.ApplicationRole on userRole.RoleId equals role.Id
                             where userRole.UserId == UserObj.Id
                             select role);
                foreach (var role in roles)
                {
                    userRoleNames.Add(role.Name);
                }
            }


            if (hospitalId == null || hospitalId == 0)
            {
                if (list.Count > 0)
                {
                    foreach (var itm in list)
                    {
                        var lstStatus = _context.SupplierExecludeAssets.Where(a => a.StatusId == itm.Id && a.AppTypeId == appTypeId).ToList();
                        if (itm.Id == 1)
                        {
                            ItemObj.OpenStatus = lstStatus.Count;
                        }
                        if (itm.Id == 2)
                        {
                            ItemObj.ApproveStatus = lstStatus.Count;
                        }
                        if (itm.Id == 3)
                        {
                            ItemObj.RejectStatus = lstStatus.Count;
                        }
                        if (itm.Id == 4)
                        {
                            ItemObj.SystemRejectStatus = lstStatus.Count;
                        }
                    }

                    ItemObj.TotalStatus = ItemObj.OpenStatus + ItemObj.ApproveStatus + ItemObj.RejectStatus + ItemObj.SystemRejectStatus;

                }
            }
            else
            {

                if (list.Count > 0)
                {
                    foreach (var itm in list)
                    {
                        var lstHospitalStatus = _context.HospitalApplications.Include(a => a.AssetDetail).Where(a => a.AssetDetail.HospitalId == hospitalId && a.StatusId == itm.Id && a.AppTypeId == appTypeId).ToList();

                        if (itm.Id == 1)
                        {
                            ItemObj.OpenStatus = lstHospitalStatus.Count;
                        }
                        if (itm.Id == 2)
                        {
                            ItemObj.ApproveStatus = lstHospitalStatus.Count;
                        }
                        if (itm.Id == 3)
                        {
                            ItemObj.RejectStatus = lstHospitalStatus.Count;
                        }
                        if (itm.Id == 4)
                        {
                            ItemObj.SystemRejectStatus = lstHospitalStatus.Count;
                        }
                    }
                    ItemObj.TotalStatus = ItemObj.OpenStatus + ItemObj.ApproveStatus + ItemObj.RejectStatus + ItemObj.SystemRejectStatus;
                }
            }
            return ItemObj;
        }


        public HospitalSupplierStatus GetById(int id)
        {
            return _context.HospitalSupplierStatuses.Find(id);
        }

      
    }
}
