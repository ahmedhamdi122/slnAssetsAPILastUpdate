﻿using Asset.Domain.Repositories;
using Asset.Models;
using Asset.ViewModels.HospitalVM;
using Asset.ViewModels.RoleCategoryVM;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Asset.Core.Repositories
{
    public class HospitalRepositories : IHospitalRepository
    {
        private ApplicationDbContext _context;
        string msg;

        public HospitalRepositories(ApplicationDbContext context)
        {
            _context = context;
        }


        public EditHospitalVM GetById(int id)
        {
            var lstHospitals = _context.Hospitals.Where(a => a.Id == id).ToList();

            if (lstHospitals.Count > 0)
            {
                Hospital item = lstHospitals[0];
                EditHospitalVM hospitalObj = new EditHospitalVM();
                hospitalObj.Id = item.Id;
                hospitalObj.Code = item.Code;
                hospitalObj.Name = item.Name;
                hospitalObj.NameAr = item.NameAr;
                hospitalObj.Address = item.Address;
                hospitalObj.AddressAr = item.AddressAr;
                hospitalObj.Email = item.Email;
                hospitalObj.Mobile = item.Mobile;
                hospitalObj.Latitude = item.Latitude != null ? decimal.Parse(item.Latitude.ToString()) : 0;
                hospitalObj.Longtitude = item.Longtitude != null ? decimal.Parse(item.Longtitude.ToString()) : 0;
                hospitalObj.ManagerName = item.ManagerName;
                hospitalObj.ManagerNameAr = item.ManagerNameAr;
                hospitalObj.GovernorateId = item.GovernorateId != null ? item.GovernorateId : 0;
                hospitalObj.CityId = item.CityId != null ? item.CityId : 0;
                hospitalObj.OrganizationId = item.OrganizationId != null ? item.OrganizationId : 0;
                hospitalObj.SubOrganizationId = item.SubOrganizationId != null ? item.SubOrganizationId : 0;
                hospitalObj.ContractName = item.ContractName;
                hospitalObj.StrContractStart = item.ContractStart.ToString();
                hospitalObj.StrContractEnd = item.ContractEnd.ToString();
                hospitalObj.ContractStart = item.ContractStart;
                hospitalObj.ContractEnd = item.ContractEnd;
                hospitalObj.Departments = _context.HospitalDepartments.Where(a => a.HospitalId == item.Id).Select(a => a.DepartmentId).ToList();
                hospitalObj.EnableDisableDepartments = _context.HospitalDepartments.Where(a => a.HospitalId == item.Id).ToList().Count > 0 ?
                 _context.HospitalDepartments.Where(a => a.HospitalId == item.Id).Select(item => new EnableDisableDepartment
                 {
                     DepartmentId = item.DepartmentId,
                     IsActive = item.IsActive
                 }).ToList() : null;

                return hospitalObj;
            }

            return new EditHospitalVM();
        }




        public IEnumerable<IndexHospitalVM.GetData> GetAll()
        {
            var lstHospitals = _context.Hospitals.Include(a => a.Governorate)
                         .Include(a => a.City).Include(a => a.Organization).Include(a => a.SubOrganization)
                         .ToList().Select(item => new IndexHospitalVM.GetData
                         {
                             Id = item.Id,
                             Code = item.Code,
                             Name = item.Name,
                             NameAr = item.NameAr,
                             CityName = (item.City != null) ? item.City.Name : "",
                             CityNameAr = (item.City != null) ? item.City.NameAr : "",
                             GovernorateName = (item.Governorate != null) ? item.Governorate.Name : "",
                             GovernorateNameAr = (item.Governorate != null) ? item.Governorate.NameAr : "",
                             OrgName = (item.Organization != null) ? item.Organization.Name : "",
                             OrgNameAr = (item.Organization != null) ? item.Organization.NameAr : "",
                             SubOrgName = (item.SubOrganization != null) ? item.SubOrganization.Name : "",
                             SubOrgNameAr = (item.SubOrganization != null) ? item.SubOrganization.NameAr : "",
                             ContractName = item.ContractName,
                             StrContractStart = item.ContractStart.ToString(),
                             StrContractEnd = item.ContractEnd.ToString(),
                             CountAssets = _context.AssetDetails.Include(a => a.Hospital).Where(a => a.HospitalId == item.Id).Count()

                         });

            return lstHospitals;
        }

        public int Add(CreateHospitalVM HospitalVM)
        {
            Hospital HospitalObj = new Hospital();
            try
            {
                if (HospitalVM != null)
                {
                    HospitalObj.Code = HospitalVM.Code;
                    HospitalObj.Name = HospitalVM.Name;
                    HospitalObj.NameAr = HospitalVM.NameAr;
                    HospitalObj.Address = HospitalVM.Address;
                    HospitalObj.AddressAr = HospitalVM.AddressAr;
                    HospitalObj.Email = HospitalVM.Email;
                    HospitalObj.Mobile = HospitalVM.Mobile;
                    HospitalObj.ManagerName = HospitalVM.ManagerName;
                    HospitalObj.ManagerNameAr = HospitalVM.ManagerNameAr;
                    HospitalObj.Latitude = float.Parse(HospitalVM.Latitude.ToString());
                    HospitalObj.Longtitude = float.Parse(HospitalVM.Longtitude.ToString());
                    HospitalObj.GovernorateId = HospitalVM.GovernorateId;
                    HospitalObj.CityId = HospitalVM.CityId;
                    HospitalObj.OrganizationId = HospitalVM.OrganizationId;
                    HospitalObj.SubOrganizationId = (int)HospitalVM.SubOrganizationId;


                    HospitalObj.ContractName = HospitalVM.ContractName;
                    if (HospitalVM.StrContractStart != "")
                        HospitalObj.ContractStart = DateTime.Parse(HospitalVM.StrContractStart);
                    if (HospitalVM.StrContractEnd != "")
                        HospitalObj.ContractEnd = DateTime.Parse(HospitalVM.StrContractEnd);

                    _context.Hospitals.Add(HospitalObj);
                    _context.SaveChanges();


                    int hospitalId = HospitalObj.Id;
                    if (HospitalVM.Departments.Count > 0)
                    {
                        foreach (var depart in HospitalVM.Departments)
                        {
                            HospitalDepartment hospitalDepartmentObj = new HospitalDepartment();
                            hospitalDepartmentObj.DepartmentId = depart;
                            hospitalDepartmentObj.HospitalId = hospitalId;
                            hospitalDepartmentObj.IsActive = true;
                            _context.HospitalDepartments.Add(hospitalDepartmentObj);
                            int hdId = _context.SaveChanges();
                        }
                    }


                }
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }
            return HospitalObj.Id;
        }

        public int Delete(int id)
        {
            var HospitalObj = _context.Hospitals.Find(id);
            try
            {
                if (HospitalObj != null)
                {

                    _context.Hospitals.Remove(HospitalObj);
                    return _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }

            return 0;
        }

        public int Update(EditHospitalVM HospitalVM)
        {
            try
            {

                var HospitalObj = _context.Hospitals.Find(HospitalVM.Id);
                HospitalObj.Id = HospitalVM.Id;
                HospitalObj.Code = HospitalVM.Code;
                HospitalObj.Name = HospitalVM.Name;
                HospitalObj.NameAr = HospitalVM.NameAr;
                HospitalObj.Address = HospitalVM.Address;
                HospitalObj.AddressAr = HospitalVM.AddressAr;
                HospitalObj.Email = HospitalVM.Email;
                HospitalObj.Mobile = HospitalVM.Mobile;
                HospitalObj.ManagerName = HospitalVM.ManagerName;
                HospitalObj.ManagerNameAr = HospitalVM.ManagerNameAr;
                HospitalObj.Latitude = float.Parse(HospitalVM.Latitude.ToString());
                HospitalObj.Longtitude = float.Parse(HospitalVM.Longtitude.ToString());
                HospitalObj.GovernorateId = HospitalVM.GovernorateId;
                HospitalObj.CityId = HospitalVM.CityId;
                HospitalObj.OrganizationId = HospitalVM.OrganizationId;
                HospitalObj.SubOrganizationId = HospitalVM.SubOrganizationId;


                HospitalObj.ContractName = HospitalVM.ContractName;
                if (HospitalVM.StrContractStart != "")
                    HospitalObj.ContractStart = DateTime.Parse(HospitalVM.StrContractStart);
                if (HospitalVM.StrContractEnd != "")
                    HospitalObj.ContractEnd = DateTime.Parse(HospitalVM.StrContractEnd);


                _context.Entry(HospitalObj).State = EntityState.Modified;
                _context.SaveChanges();


                if (HospitalVM.Departments.Count > 0 && HospitalVM.Departments != null)
                {
                    var savedId = _context.HospitalDepartments.Where(a => a.HospitalId == HospitalVM.Id).ToList().Select(x => x.DepartmentId).ToList();
                    var newIds = HospitalVM.Departments.ToList().Except(savedId);
                    if (newIds.Count() > 0)
                    {
                        foreach (var newId in newIds)
                        {
                            HospitalDepartment hospitalDepartmentObj = new HospitalDepartment();
                            hospitalDepartmentObj.DepartmentId = newId;
                            hospitalDepartmentObj.HospitalId = HospitalObj.Id;
                            hospitalDepartmentObj.IsActive = true;
                            _context.HospitalDepartments.Add(hospitalDepartmentObj);
                            int hdId = _context.SaveChanges();
                        }
                    }

                    var removedIds = savedId.ToList().Except(HospitalVM.Departments);
                    if (removedIds.Count() > 0)
                    {
                        foreach (var removedId in removedIds)
                        {
                            var hospitalDepartmentObj = _context.HospitalDepartments.Where(a => a.HospitalId == HospitalObj.Id && a.DepartmentId == removedId).First();
                            _context.HospitalDepartments.Remove(hospitalDepartmentObj);
                            int hdId = _context.SaveChanges();
                        }
                    }
                }

                return HospitalObj.Id;
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }

            return 0;
        }

        public IEnumerable<Hospital> GetAllHospitals()
        {
            return _context.Hospitals.ToList();
        }

        public IEnumerable<IndexHospitalVM.GetData> GetHospitalsByUserId(string userId)
        {
            List<IndexHospitalVM.GetData> lstHospitals = new List<IndexHospitalVM.GetData>();
            if (userId != null)
            {
                var userObj = _context.ApplicationUser.Find(userId);


                lstHospitals = _context.Hospitals.Include(a => a.Governorate).Include(a => a.City).Include(a => a.Organization).Include(a => a.SubOrganization)
                     .Select(item => new IndexHospitalVM.GetData
                     {
                         Id = item.Id,
                         Code = item.Code,
                         Name = item.Name,
                         NameAr = item.NameAr,
                         CityId = item.City != null ? item.City.Id : 0,
                         CityName = (item.City != null) ? item.City.Name : "",
                         CityNameAr = (item.City != null) ? item.City.NameAr : "",
                         GovernorateId = item.Governorate != null ? item.Governorate.Id : 0,
                         GovernorateName = (item.Governorate != null) ? item.Governorate.Name : "",
                         GovernorateNameAr = (item.Governorate != null) ? item.Governorate.NameAr : "",
                         OrganizationId = item.Organization != null ? item.Organization.Id : 0,
                         OrgName = (item.Organization != null) ? item.Organization.Name : "",
                         OrgNameAr = (item.Organization != null) ? item.Organization.NameAr : "",
                         SubOrganizationId = item.SubOrganization != null ? item.SubOrganization.Id : 0,
                         SubOrgName = (item.SubOrganization != null) ? item.SubOrganization.Name : "",
                         SubOrgNameAr = (item.SubOrganization != null) ? item.SubOrganization.NameAr : "",
                         ContractName = item.ContractName,
                         StrContractStart = item.ContractStart.ToString(),
                         StrContractEnd = item.ContractEnd.ToString(),
                     }).ToList();

                if (userObj.GovernorateId == 0 && userObj.CityId == 0 && userObj.OrganizationId == 0 && userObj.SubOrganizationId == 0 && userObj.HospitalId == 0)
                {
                    lstHospitals = lstHospitals.ToList();
                }
                if (userObj.GovernorateId > 0 && userObj.CityId == 0 && userObj.HospitalId == 0)
                {
                    lstHospitals = lstHospitals.Where(a => a.GovernorateId == userObj.GovernorateId).ToList();
                }
                if (userObj.GovernorateId > 0 && userObj.CityId > 0 && userObj.HospitalId == 0)
                {
                    lstHospitals = lstHospitals.Where(a => a.GovernorateId == userObj.GovernorateId && a.CityId == userObj.CityId).ToList();
                }
                if (userObj.GovernorateId > 0 && userObj.CityId > 0 && userObj.HospitalId > 0)
                {
                    lstHospitals = lstHospitals.Where(a => a.GovernorateId == userObj.GovernorateId && a.CityId == userObj.CityId && a.Id == userObj.HospitalId).ToList();
                }
                if (userObj.OrganizationId > 0 && userObj.SubOrganizationId == 0)
                {
                    lstHospitals = lstHospitals.Where(a => a.OrganizationId == userObj.OrganizationId).ToList();
                }
                if (userObj.OrganizationId > 0 && userObj.SubOrganizationId > 0 && userObj.HospitalId == 0)
                {
                    lstHospitals = lstHospitals.Where(a => a.OrganizationId == userObj.OrganizationId && a.SubOrganizationId == userObj.SubOrganizationId).ToList();
                }
                if (userObj.OrganizationId > 0 && userObj.SubOrganizationId > 0 && userObj.HospitalId > 0)
                {
                    lstHospitals = lstHospitals.Where(a => a.OrganizationId == userObj.OrganizationId && a.SubOrganizationId == userObj.SubOrganizationId && a.Id == userObj.HospitalId).ToList();
                }
            }
            return lstHospitals;
        }

        public IEnumerable<Hospital> GetHospitalsByCityId(int cityId)
        {
            var lstHospitals = _context.Hospitals.ToList().Where(a => a.CityId == cityId).OrderBy(a => a.Id).ToList();
            return lstHospitals;

        }

        public IEnumerable<Hospital> GetHospitalsBySubOrganizationId(int subOrgId)
        {
            return _context.Hospitals.ToList().Where(a => a.SubOrganizationId == subOrgId).ToList();
        }

        public DetailHospitalVM GetHospitalDetailById(int id)
        {
            var HospitalObj = _context.Hospitals
                                .Include(a => a.Governorate)
                                .Include(a => a.City)
                                .Include(a => a.Organization)
                                .Include(a => a.SubOrganization)
                                .Where(a => a.Id == id).Select(item => new DetailHospitalVM
                                {
                                    Id = item.Id,
                                    Code = item.Code,
                                    Name = item.Name,
                                    NameAr = item.NameAr,
                                    Address = item.Address,
                                    AddressAr = item.AddressAr,
                                    Email = item.Email,
                                    Mobile = item.Mobile,
                                    Latitude = item.Latitude != null ? decimal.Parse(item.Latitude.ToString()) : 0,
                                    Longtitude = item.Longtitude != null ? decimal.Parse(item.Longtitude.ToString()) : 0,
                                    ManagerName = item.ManagerName,
                                    ManagerNameAr = item.ManagerNameAr,
                                    Departments = _context.HospitalDepartments.Where(a => a.HospitalId == item.Id).Select(a => a.DepartmentId).ToList().Count > 0 ? _context.HospitalDepartments.Where(a => a.HospitalId == item.Id).Select(a => a.DepartmentId).ToList() : null,
                                    EnableDisableDepartments = _context.HospitalDepartments.Where(a => a.HospitalId == item.Id).Select(item => new EnableDisableDepartment
                                    {
                                        DepartmentId = item.DepartmentId,
                                        IsActive = item.IsActive
                                    }).ToList(),

                                    GovernorateName = (item.GovernorateId != null || item.GovernorateId != 0) ? item.Governorate.Name : "",
                                    GovernorateNameAr = (item.GovernorateId != null || item.GovernorateId != 0) ? item.Governorate.NameAr : "",
                                    CityName = (item.CityId != null || item.CityId != 0) ? item.City.Name : "",
                                    CityNameAr = (item.CityId != null || item.CityId != 0) ? item.City.NameAr : "",
                                    SubOrganizationName = (item.SubOrganizationId != null || item.SubOrganizationId != 0) ? item.SubOrganization.Name : "",
                                    SubOrganizationNameAr = (item.SubOrganizationId != null || item.SubOrganizationId != 0) ? item.SubOrganization.NameAr : "",
                                    OrganizationName = (item.OrganizationId != null || item.OrganizationId != 0) ? item.Organization.Name : "",
                                    OrganizationNameAr = (item.OrganizationId != null || item.OrganizationId != 0) ? item.Organization.NameAr : "",


                                    ContractName = item.ContractName,
                                    StrContractStart = item.ContractStart.ToString(),
                                    StrContractEnd = item.ContractEnd.ToString(),
                                    ContractStart = item.ContractStart,
                                    ContractEnd = item.ContractEnd,

                                }).First();

            return HospitalObj;
        }

        public int UpdateHospitalDepartment(EditHospitalDepartmentVM hospitalDepartmentVM)
        {
            try
            {

                var HospitalDepartmentObj = _context.HospitalDepartments.Where(a => a.HospitalId == hospitalDepartmentVM.HospitalId && a.DepartmentId == hospitalDepartmentVM.DepartmentId).First();
                HospitalDepartmentObj.Id = HospitalDepartmentObj.Id;

                if (HospitalDepartmentObj.IsActive == false)
                    HospitalDepartmentObj.IsActive = true;
                if (HospitalDepartmentObj.IsActive == true)
                    HospitalDepartmentObj.IsActive = false;
                _context.Entry(HospitalDepartmentObj).State = EntityState.Modified;
                _context.SaveChanges();
                return HospitalDepartmentObj.Id;
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }

            return 0;
        }

        public List<HospitalDepartment> GetHospitalDepartmentByHospitalId(int hospitalId)
        {
            var lstHospitalDepartments = _context.HospitalDepartments.Where(a => a.HospitalId == hospitalId).ToList();
            return lstHospitalDepartments;
        }

        public List<IndexHospitalDepartmentVM.GetData> GetHospitalDepartmentByHospitalId2(int hospitalId)
        {
            var lstHospitalDepartments = _context.HospitalDepartments.Include(a => a.Department).Where(a => a.HospitalId == hospitalId).ToList()
                .Select(item => new IndexHospitalDepartmentVM.GetData
                {
                    Id = item.Id,
                    DepartmentName = item.Department != null ? item.Department.Name : "",
                    DepartmentNameAr = item.Department != null ? item.Department.NameAr : "",
                    IsActive = item.IsActive,
                    HospitalId = item.HospitalId,
                    DepartmentId = item.DepartmentId
                }).ToList();

            return lstHospitalDepartments;
        }

        public List<SubOrganization> GetSubOrganizationsByHospitalId(int hospitalId)
        {
            return _context.Hospitals.Include(a => a.SubOrganization).Where(a => a.Id == hospitalId).Select(a => a.SubOrganization).ToList();
            //join sub in _context.SubOrganizations on hospital.SubOrganizationId equals sub.Id
            //where hospital.Id == hospitalId
            //select sub).ToList();
        }

        public List<CountHospitalVM> CountHospitalsByCities()
        {
            List<CountHospitalVM> list = new List<CountHospitalVM>();


            var lstCities = _context.Cities.ToList().Take(5);
            foreach (var item in lstCities)
            {
                CountHospitalVM countHospitalObj = new CountHospitalVM();
                countHospitalObj.CityName = item.Name;
                countHospitalObj.CityNameAr = item.NameAr;
                countHospitalObj.CountOfHospitals = _context.Hospitals.Where(a => a.CityId == item.Id).ToList().Count;
                list.Add(countHospitalObj);
            }

            return list;
        }

        public int CountHospitals()
        {
            return _context.Hospitals.Count();
        }

        public IEnumerable<IndexHospitalVM.GetData> SearchHospitals(SearchHospitalVM searchObj)
        {
            List<IndexHospitalVM.GetData> lstData = new List<IndexHospitalVM.GetData>();
            ApplicationUser UserObj = new ApplicationUser();
            var obj = _context.ApplicationUser.Where(a => a.Id == searchObj.UserId).ToList();
            if (obj.Count > 0)
            {
                UserObj = obj[0];
            }
            var list = _context.Hospitals
                        .Include(a => a.Governorate)
                        .Include(a => a.City)
                        .Include(a => a.Organization)
                        .Include(a => a.SubOrganization)
                        .ToList();


            if (list.Count > 0)
            {
                if (UserObj.GovernorateId == 0 && UserObj.CityId == 0 && UserObj.HospitalId == 0)
                {
                    list = list.ToList();
                }

                if (UserObj.GovernorateId > 0 && UserObj.CityId == 0 && UserObj.HospitalId == 0)
                {
                    list = list.Where(t => t.GovernorateId == UserObj.GovernorateId).ToList();
                }
                if (UserObj.GovernorateId > 0 && UserObj.CityId > 0 && UserObj.HospitalId == 0)
                {
                    list = list.Where(t => t.GovernorateId == UserObj.GovernorateId && t.CityId == UserObj.CityId).ToList();
                }
                if (UserObj.GovernorateId > 0 && UserObj.CityId > 0 && UserObj.HospitalId > 0)
                {
                    list = list.Where(t => t.GovernorateId == UserObj.GovernorateId && t.CityId == UserObj.CityId && t.Id == UserObj.HospitalId).ToList();
                }
                if (UserObj.OrganizationId > 0 && UserObj.SubOrganizationId == 0 && UserObj.HospitalId == 0)
                {
                    list = list.Where(t => t.OrganizationId == UserObj.OrganizationId).ToList();
                }
                if (UserObj.OrganizationId > 0 && UserObj.SubOrganizationId > 0 && UserObj.HospitalId == 0)
                {
                    list = list.Where(t => t.OrganizationId == UserObj.OrganizationId && t.SubOrganizationId == UserObj.SubOrganizationId).ToList();
                }

                if (UserObj.OrganizationId > 0 && UserObj.SubOrganizationId > 0 && UserObj.HospitalId > 0)
                {
                    list = list.Where(t => t.SubOrganizationId == UserObj.SubOrganizationId && t.Id == UserObj.HospitalId).ToList();
                }
            }

            foreach (var item in list)
            {
                IndexHospitalVM.GetData getDataObj = new IndexHospitalVM.GetData();
                getDataObj.Id = item.Id;
                getDataObj.Code = item.Code;
                getDataObj.Name = item.Name;
                getDataObj.NameAr = item.NameAr;
                getDataObj.GovernorateId = item.GovernorateId != null ? (int)item.GovernorateId : 0;
                getDataObj.GovernorateName = item.GovernorateId != null ? item.Governorate.Name : "";
                getDataObj.GovernorateNameAr = item.GovernorateId != null ? item.Governorate.NameAr : "";
                getDataObj.CityId = item.CityId != null ? (int)item.CityId : 0;
                getDataObj.CityName = item.CityId != null ? item.City.Name : "";
                getDataObj.CityNameAr = item.CityId != null ? item.City.NameAr : "";
                getDataObj.OrganizationId = item.OrganizationId != null ? (int)item.OrganizationId : 0;
                getDataObj.OrgName = item.OrganizationId != null ? item.Organization.Name : "";
                getDataObj.OrgNameAr = item.OrganizationId != null ? item.Organization.NameAr : "";
                getDataObj.SubOrganizationId = item.SubOrganizationId != null ? (int)item.SubOrganizationId : 0;
                getDataObj.SubOrgName = item.SubOrganizationId != null ? item.SubOrganization.Name : "";
                getDataObj.SubOrgNameAr = item.SubOrganizationId != null ? item.SubOrganization.NameAr : "";
                lstData.Add(getDataObj);
            }


            if (searchObj.GovernorateId != 0)
            {
                lstData = lstData.Where(a => a.GovernorateId == searchObj.GovernorateId).ToList();
            }
            else
                lstData = lstData.ToList();



            if (searchObj.CityId != 0)
            {
                lstData = lstData.Where(a => a.CityId == searchObj.CityId).ToList();
            }
            else
                lstData = lstData.ToList();

            if (searchObj.OrganizationId != 0)
            {
                lstData = lstData.Where(a => a.OrganizationId == searchObj.OrganizationId).ToList();
            }
            else
                lstData = lstData.ToList();



            if (searchObj.SubOrganizationId != 0)
            {
                lstData = lstData.Where(a => a.SubOrganizationId == searchObj.SubOrganizationId).ToList();
            }
            else
                lstData = lstData.ToList();



            if (searchObj.Code != "")
            {
                lstData = lstData.Where(b => b.Code.Contains(searchObj.Code)).ToList();
            }
            else
                lstData = lstData.ToList();


            if (searchObj.Name != "")
            {
                lstData = lstData.Where(b => b.Name.Contains(searchObj.Name)).ToList();
            }
            else
                lstData = lstData.ToList();


            if (searchObj.NameAr != "")
            {
                lstData = lstData.Where(b => b.NameAr.Contains(searchObj.NameAr)).ToList();
            }
            else
                lstData = lstData.ToList();

            return lstData;
        }
        public IEnumerable<IndexHospitalVM.GetData> SortHospitals(SortVM sortObj)
        {
            List<IndexHospitalVM.GetData> lstHospital = new List<IndexHospitalVM.GetData>();

            var lstHospitals = _context.Hospitals.Include(a => a.Governorate)
                   .Include(a => a.City)
                   .Include(a => a.Organization)
                   .Include(a => a.SubOrganization).ToList();

            foreach (var item in lstHospitals)
            {
                IndexHospitalVM.GetData hospitalobj = new IndexHospitalVM.GetData();
                hospitalobj.Id = item.Id;
                hospitalobj.Code = item.Code;
                hospitalobj.Name = item.Name;
                hospitalobj.NameAr = item.NameAr;
                hospitalobj.GovernorateId = item.GovernorateId != null ? (int)item.GovernorateId : 0;
                hospitalobj.GovernorateName = item.GovernorateId != null ? item.Governorate.Name : "";
                hospitalobj.GovernorateNameAr = item.GovernorateId != null ? item.Governorate.NameAr : "";
                hospitalobj.CityId = item.CityId != null ? (int)item.CityId : 0;
                hospitalobj.CityName = item.CityId != null ? item.City.Name : "";
                hospitalobj.CityNameAr = item.CityId != null ? item.City.NameAr : "";
                hospitalobj.OrganizationId = item.OrganizationId != null ? (int)item.OrganizationId : 0;
                hospitalobj.OrgName = item.OrganizationId != null ? item.Organization.Name : "";
                hospitalobj.OrgNameAr = item.OrganizationId != null ? item.Organization.NameAr : "";
                hospitalobj.SubOrganizationId = item.SubOrganizationId != null ? (int)item.SubOrganizationId : 0;
                hospitalobj.SubOrgName = item.SubOrganizationId != null ? item.SubOrganization.Name : "";
                hospitalobj.SubOrgNameAr = item.SubOrganizationId != null ? item.SubOrganization.NameAr : "";
                lstHospital.Add(hospitalobj);
            }

            if (sortObj.UserId != null)
            {
                var userObj = _context.ApplicationUser.Find(sortObj.UserId);
                if (userObj.GovernorateId == 0 && userObj.CityId == 0 && userObj.OrganizationId == 0 && userObj.SubOrganizationId == 0 && userObj.HospitalId == 0)
                {
                    lstHospital = lstHospital.ToList();
                }
                if (userObj.GovernorateId > 0 && userObj.CityId == 0 && userObj.HospitalId == 0)
                {
                    lstHospital = lstHospital.Where(a => a.GovernorateId == userObj.GovernorateId).ToList();
                }
                if (userObj.GovernorateId > 0 && userObj.CityId > 0 && userObj.HospitalId == 0)
                {
                    lstHospital = lstHospital.Where(a => a.GovernorateId == userObj.GovernorateId && a.CityId == userObj.CityId).ToList();
                }
                if (userObj.GovernorateId > 0 && userObj.CityId > 0 && userObj.HospitalId > 0)
                {
                    lstHospital = lstHospital.Where(a => a.GovernorateId == userObj.GovernorateId && a.CityId == userObj.CityId && a.Id == userObj.HospitalId).ToList();
                }
                if (userObj.OrganizationId > 0 && userObj.SubOrganizationId == 0)
                {
                    lstHospital = lstHospital.Where(a => a.OrganizationId == userObj.OrganizationId).ToList();
                }
                if (userObj.OrganizationId > 0 && userObj.SubOrganizationId > 0 && userObj.HospitalId == 0)
                {
                    lstHospital = lstHospital.Where(a => a.OrganizationId == userObj.OrganizationId && a.SubOrganizationId == userObj.SubOrganizationId).ToList();
                }
                if (userObj.OrganizationId > 0 && userObj.SubOrganizationId > 0 && userObj.HospitalId > 0)
                {
                    lstHospital = lstHospital.Where(a => a.OrganizationId == userObj.OrganizationId && a.SubOrganizationId == userObj.SubOrganizationId && a.Id == userObj.HospitalId).ToList();
                }




                if (sortObj.GovernorateName != "")
                {
                    if (sortObj.SortStatus == "descending")
                        lstHospital = lstHospital.OrderByDescending(d => d.GovernorateName).ToList();
                    else
                        lstHospital = lstHospital.OrderBy(d => d.GovernorateName).ToList();
                }
                else if (sortObj.GovernorateNameAr != "")
                {
                    if (sortObj.SortStatus == "descending")
                        lstHospital = lstHospital.OrderByDescending(d => d.GovernorateNameAr).ToList();
                    else
                        lstHospital = lstHospital.OrderBy(d => d.GovernorateNameAr).ToList();
                }

                else if (sortObj.CityName != "")
                {
                    if (sortObj.SortStatus == "descending")
                        lstHospital = lstHospital.OrderByDescending(d => d.CityName).ToList();
                    else
                        lstHospital = lstHospital.OrderBy(d => d.CityName).ToList();
                }
                else if (sortObj.CityNameAr != "")
                {
                    if (sortObj.SortStatus == "descending")
                        lstHospital = lstHospital.OrderByDescending(d => d.CityNameAr).ToList();
                    else
                        lstHospital = lstHospital.OrderBy(d => d.CityNameAr).ToList();
                }


                else if (sortObj.Name != "")
                {
                    if (sortObj.SortStatus == "descending")
                        lstHospital = lstHospital.OrderByDescending(d => d.Name).ToList();
                    else
                        lstHospital = lstHospital.OrderBy(d => d.Name).ToList();
                }
                else if (sortObj.NameAr != "")
                {
                    if (sortObj.SortStatus == "descending")
                        lstHospital = lstHospital.OrderByDescending(d => d.NameAr).ToList();
                    else
                        lstHospital = lstHospital.OrderBy(d => d.NameAr).ToList();
                }

                else if (sortObj.OrgName != "")
                {
                    if (sortObj.SortStatus == "descending")
                        lstHospital = lstHospital.OrderByDescending(d => d.OrgName).ToList();
                    else
                        lstHospital = lstHospital.OrderBy(d => d.OrgName).ToList();
                }
                else if (sortObj.OrgNameAr != "")
                {
                    if (sortObj.SortStatus == "descending")
                        lstHospital = lstHospital.OrderByDescending(d => d.OrgNameAr).ToList();
                    else
                        lstHospital = lstHospital.OrderBy(d => d.OrgNameAr).ToList();
                }
                else if (sortObj.SubOrgName != "")
                {
                    if (sortObj.SortStatus == "descending")
                        lstHospital = lstHospital.OrderByDescending(d => d.SubOrgName).ToList();
                    else
                        lstHospital = lstHospital.OrderBy(d => d.SubOrgName).ToList();
                }
                else if (sortObj.SubOrgNameAr != "")
                {
                    if (sortObj.SortStatus == "descending")
                        lstHospital = lstHospital.OrderByDescending(d => d.SubOrgNameAr).ToList();
                    else
                        lstHospital = lstHospital.OrderBy(d => d.SubOrgNameAr).ToList();
                }
                else if (sortObj.Code != "")
                {
                    if (sortObj.SortStatus == "descending")
                        lstHospital = lstHospital.OrderByDescending(d => d.Code).ToList();
                    else
                        lstHospital = lstHospital.OrderBy(d => d.Code).ToList();
                }
            }
            return lstHospital;
        }


        public IEnumerable<HospitalWithAssetVM> GetHospitalsWithAssets()
        {

            //   var byAge = _context.Hospitals.ToList().GroupBy(x => 10 * (x.Id / 10));
            decimal? price = 0;
            var hosWithAsset = _context.Hospitals.ToList().Select(item => new HospitalWithAssetVM
            {
                Id = item.Id,
                Name = item.Name,
                NameAr = item.NameAr,
                AssetCount = _context.AssetDetails.Where(a => a.HospitalId == item.Id).ToList().Count(),
                Assetprice = 0
            }).ToList();
            if (hosWithAsset != null)
            {
                for (var i = 0; i < hosWithAsset.Count; i++)
                {
                    if (hosWithAsset[i].AssetCount != 0)
                    {
                        var Assets = _context.AssetDetails.Where(a => a.HospitalId == hosWithAsset[i].Id).ToList();
                        foreach (var Ass in Assets)
                        {
                            price += Ass.Price;
                        }
                        hosWithAsset[i].Assetprice = price;
                        price = 0;
                    }
                }
            }
            return hosWithAsset;
        }

        public int CountDepartmentsByHospitalId(int hospitalId)
        {
            return _context.HospitalDepartments.Include(a => a.Hospital).Where(a => a.HospitalId == hospitalId).ToList().Count;
        }

        public IndexHospitalDepartmentVM.GetData GetSelectedHospitalDepartmentByDepartmentId(int hospitalId, int departmentId)
        {
            IndexHospitalDepartmentVM.GetData getDataObj = new IndexHospitalDepartmentVM.GetData();

            var lstHospitalDepartments = _context.HospitalDepartments
                .Include(a => a.Department)
                .Where(a => a.HospitalId == hospitalId && a.DepartmentId == departmentId).ToList();
            if (lstHospitalDepartments.Count > 0)
            {
                var hospitalDepartmentObj = lstHospitalDepartments[0];
                getDataObj.Id = hospitalDepartmentObj.Id;
                getDataObj.DepartmentName = hospitalDepartmentObj.Department != null ? hospitalDepartmentObj.Department.Name : "";
                getDataObj.DepartmentNameAr = hospitalDepartmentObj.Department != null ? hospitalDepartmentObj.Department.NameAr : "";
                getDataObj.IsActive = hospitalDepartmentObj.IsActive;
                getDataObj.HospitalId = hospitalDepartmentObj.HospitalId;
                getDataObj.DepartmentId = hospitalDepartmentObj.DepartmentId;
                return getDataObj;

            }
            return getDataObj;
        }


        public IEnumerable<Hospital> GetHospitalByGovId(int govId)
        {
            return _context.Hospitals.Where(h => h.GovernorateId == govId).ToList();
        }

        public GenerateHospitalCodeVM GenerateHospitalCode()
        {
            GenerateHospitalCodeVM numberObj = new GenerateHospitalCodeVM();
            int code = 0;

            var lastId = _context.Hospitals.ToList();
            if (lastId.Count > 0)
            {
                var lastHospitalCode = lastId.Max(a => a.Code);
                var hospitalCode = (int.Parse(lastHospitalCode) + 1).ToString();
                var lastcode = hospitalCode.ToString().PadLeft(3, '0');
                numberObj.Code = lastcode;
            }
            else
            {
                numberObj.Code = (code + 1).ToString();
            }

            return numberObj;
        }


        #region Refactor Functions

        public IndexHospitalVM GetTop10Hospitals(int hospitalId)
        {
            IndexHospitalVM mainClass = new IndexHospitalVM();
            List<IndexHospitalVM.GetData> list = new List<IndexHospitalVM.GetData>();
            List<Hospital> lstHospitals = new List<Hospital>();
            if (hospitalId != 0)
            {
                lstHospitals = _context.Hospitals.Include(a => a.Governorate)
                 .Include(a => a.City).Include(a => a.Organization).Include(a => a.SubOrganization)
                 .Where(a => a.Id == hospitalId).ToList();
            }
            else
            {
                lstHospitals = _context.Hospitals.Include(a => a.Governorate)
                              .Include(a => a.City).Include(a => a.Organization).Include(a => a.SubOrganization)
                             .ToList().ToList();
            }

            if (lstHospitals.Count > 0)
            {
                foreach (var item in lstHospitals)
                {
                    IndexHospitalVM.GetData itemObj = new IndexHospitalVM.GetData();
                    itemObj.Id = item.Id;
                    itemObj.Code = item.Code;
                    itemObj.Name = item.Name;
                    itemObj.NameAr = item.NameAr;
                    itemObj.CityName = (item.City != null) ? item.City.Name : "";
                    itemObj.CityNameAr = (item.City != null) ? item.City.NameAr : "";
                    itemObj.GovernorateName = (item.Governorate != null) ? item.Governorate.Name : "";
                    itemObj.GovernorateNameAr = (item.Governorate != null) ? item.Governorate.NameAr : "";
                    itemObj.OrgName = (item.Organization != null) ? item.Organization.Name : "";
                    itemObj.OrgNameAr = (item.Organization != null) ? item.Organization.NameAr : "";
                    itemObj.SubOrgName = (item.SubOrganization != null) ? item.SubOrganization.Name : "";
                    itemObj.SubOrgNameAr = (item.SubOrganization != null) ? item.SubOrganization.NameAr : "";
                    list.Add(itemObj);
                }
            }
            mainClass.Results = list;
            mainClass.Count = lstHospitals.Count;
            return mainClass;
        }

        public IEnumerable<IndexHospitalVM.GetData> GetHospitalsByGovCityOrgSubOrgId(int govId, int cityId, int orgId, int subOrgId)
        {

            List<IndexHospitalVM.GetData> lstData = new List<IndexHospitalVM.GetData>();
            var list = _context.Hospitals
                         .Include(a => a.Governorate)
                         .Include(a => a.City)
                         .Include(a => a.Organization)
                         .Include(a => a.SubOrganization)
                         .ToList();

            if (govId != 0)
            {
                list = list.Where(a => a.GovernorateId == govId).ToList();
            }
            else
                list = list.ToList();

            if (cityId != 0)
            {
                list = list.Where(a => a.CityId == cityId).ToList();
            }
            else
                list = list.ToList();


            if (orgId != 0)
            {
                list = list.Where(a => a.OrganizationId == orgId).ToList();
            }
            else
                list = list.ToList();



            if (subOrgId != 0)
            {
                list = list.Where(a => a.SubOrganizationId == subOrgId).ToList();
            }
            else
                list = list.ToList();


            foreach (var item in list)
            {
                IndexHospitalVM.GetData getDataObj = new IndexHospitalVM.GetData();
                getDataObj.Id = item.Id;
                getDataObj.Code = item.Code;
                getDataObj.Name = item.Name;
                getDataObj.NameAr = item.NameAr;
                lstData.Add(getDataObj);
            }



            return lstData;
        }


        #endregion

    }
}