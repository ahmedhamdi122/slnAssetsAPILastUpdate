using Asset.Domain.Repositories;
using Asset.Models;
using Asset.ViewModels.HospitalApplicationVM;
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
    public class HospitalApplicationRepositories : IHospitalApplicationRepository
    {
        private ApplicationDbContext _context;

        string msg;

        public HospitalApplicationRepositories(ApplicationDbContext context)
        {
            _context = context;
        }

        public EditHospitalApplicationVM GetById(int id)
        {
            EditHospitalApplicationVM hospitalAppVM = new EditHospitalApplicationVM();
            var execludIds = (from execlude in _context.HospitalExecludeReasons
                              join trans in _context.HospitalReasonTransactions on execlude.Id equals trans.ReasonId
                              where trans.HospitalApplicationId == id
                              select execlude.Id).ToList();


            var holdIds = (from execlude in _context.HospitalHoldReasons
                           join trans in _context.HospitalReasonTransactions on execlude.Id equals trans.ReasonId
                           where trans.HospitalApplicationId == id
                           select execlude.Id).ToList();


            var execludNames = (from execlude in _context.HospitalExecludeReasons
                                join trans in _context.HospitalReasonTransactions on execlude.Id equals trans.ReasonId
                                where trans.HospitalApplicationId == id
                                select execlude).ToList();


            var holdNames = (from hold in _context.HospitalHoldReasons
                             join trans in _context.HospitalReasonTransactions on hold.Id equals trans.ReasonId
                             where trans.HospitalApplicationId == id
                             select hold).ToList();

            var lstHospitalApps = _context.HospitalApplications.Include(a => a.User).Include(a => a.ApplicationType)
                  .Include(a => a.User).Include(a => a.AssetDetail)
                  .Include(a => a.AssetDetail.Department)
                  .Include(a => a.AssetDetail.MasterAsset)
                  .Include(a => a.AssetDetail.MasterAsset.brand)
                  .Where(a => a.Id == id).ToList();

            if (lstHospitalApps.Count > 0)
            {
                HospitalApplication item = lstHospitalApps[0];
                hospitalAppVM = new EditHospitalApplicationVM
                {
                    Id = item.Id,
                    AssetId = item.AssetId,
                    StatusId = item.StatusId,
                    AppDate = item.AppDate,
                    DueDate = item.DueDate != null ? item.DueDate.Value.ToString() : null,
                    AppNumber = item.AppNumber,
                    UserId = item.User.UserName,
                    AppTypeId = item.AppTypeId,
                    Comment = item.Comment,
                    ReasonIds = execludIds,
                    HoldReasonIds = holdIds,
                    ActionDate= item.ActionDate.Value.ToShortDateString(),
                    HospitalId= item.HospitalId,
                    assetName = item.AssetDetail.MasterAsset.Name + " - " + item.AssetDetail.MasterAsset.brand.Name,
                    assetNameAr = item.AssetDetail.MasterAsset.NameAr + " - " + item.AssetDetail.MasterAsset.brand.NameAr,
                    AppTypeName = item.ApplicationType.Name,
                    AppTypeNameAr = item.ApplicationType.NameAr,
                    SerialNumber = item.AssetDetail.SerialNumber,
                    BarCode = item.AssetDetail.Barcode,
                    ReasonNames =  execludNames ,
                    HoldReasonNames = holdNames 
                };
            }
            return hospitalAppVM;
        }

        public IEnumerable<IndexHospitalApplicationVM.GetData> GetAll()
        {
            List<IndexHospitalApplicationVM.GetData> list = new List<IndexHospitalApplicationVM.GetData>();
            var lstHospitalApplications = _context.HospitalApplications.Include(a => a.ApplicationType).Include(a => a.User)
                .Include(a => a.AssetDetail).Include(a => a.AssetDetail.MasterAsset).ToList().OrderByDescending(a => a.AppDate.Value.Date).ToList();
            foreach (var item in lstHospitalApplications)
            {

                IndexHospitalApplicationVM.GetData getDataObj = new IndexHospitalApplicationVM.GetData();
                getDataObj.Id = item.Id;
                getDataObj.HospitalId = item.HospitalId;
                getDataObj.AppNumber = item.AppNumber;
                getDataObj.Date = item.AppDate.Value.ToString();
                getDataObj.DueDate = item.DueDate != null ? item.DueDate.Value.ToString() : "";
                getDataObj.AppTypeId = item.AppTypeId;
                getDataObj.UserName = item.User.UserName;
                getDataObj.AssetId = item.AssetDetail.Id;
                getDataObj.AssetName = item.AssetDetail.MasterAsset.Name;// + " - " + item.AssetDetail.SerialNumber;
                getDataObj.AssetNameAr = item.AssetDetail.MasterAsset.NameAr;// + " - " + item.AssetDetail.SerialNumber;
                getDataObj.TypeName = item.ApplicationType.Name;
                getDataObj.TypeNameAr = item.ApplicationType.NameAr;


                getDataObj.SerialNumber = item.AssetDetail.SerialNumber;
                getDataObj.BarCode = item.AssetDetail.Barcode;
                getDataObj.ModelNumber = item.AssetDetail.MasterAsset.ModelNumber;


                getDataObj.DiffMonths = ((item.AppDate.Value.Year - DateTime.Today.Date.Year) * 12) + item.AppDate.Value.Month - DateTime.Today.Date.Month;
                getDataObj.IsMoreThan3Months = getDataObj.DiffMonths <= -3 ? true : false;

                getDataObj.StatusId = item.StatusId;

                var lstStatuses = _context.HospitalSupplierStatuses.Where(a => a.Id == item.StatusId).ToList();
                if (lstStatuses.Count > 0)
                {
                    getDataObj.StatusName = lstStatuses[0].Name;
                    getDataObj.StatusNameAr = lstStatuses[0].NameAr;
                    getDataObj.StatusColor = lstStatuses[0].Color;
                    getDataObj.StatusIcon = lstStatuses[0].Icon;

                }

                var ReasonExTitles = (from execlude in _context.HospitalExecludeReasons
                                      join trans in _context.HospitalReasonTransactions on execlude.Id equals trans.ReasonId
                                      where trans.HospitalApplicationId == item.Id
                                      && item.AppTypeId == 1
                                      select execlude).ToList();
                if (ReasonExTitles.Count > 0)
                {
                    List<string> execludeNames = new List<string>();
                    foreach (var reason in ReasonExTitles)
                    {
                        execludeNames.Add(reason.Name);
                    }
                    getDataObj.ReasonExTitles = string.Join(",", execludeNames);

                    List<string> execludeNamesAr = new List<string>();
                    foreach (var reason in ReasonExTitles)
                    {
                        execludeNamesAr.Add(reason.NameAr);
                    }
                    getDataObj.ReasonExTitlesAr = string.Join(",", execludeNamesAr);
                }

                var ReasonHoldTitles = (from execlude in _context.HospitalHoldReasons
                                        join trans in _context.HospitalReasonTransactions on execlude.Id equals trans.ReasonId
                                        where trans.HospitalApplicationId == item.Id
                                        && item.AppTypeId == 2
                                        select execlude).ToList();
                if (ReasonHoldTitles.Count > 0)
                {
                    List<string> holdNames = new List<string>();
                    foreach (var reason in ReasonHoldTitles)
                    {
                        holdNames.Add(reason.Name);
                    }
                    getDataObj.ReasonHoldTitles = string.Join(",", holdNames);

                    List<string> holdNamesAr = new List<string>();
                    foreach (var reason in ReasonHoldTitles)
                    {
                        holdNamesAr.Add(reason.NameAr);
                    }
                    getDataObj.ReasonHoldTitlesAr = string.Join(",", holdNamesAr);
                }
                list.Add(getDataObj);
            }

            return list;
        }

        public int Add(CreateHospitalApplicationVM model)
        {
            HospitalApplication hospitalApplicationObj = new HospitalApplication();
            try
            {
                if (model != null)
                {
                    hospitalApplicationObj.AssetId = model.AssetId;
                    hospitalApplicationObj.HospitalId = model.HospitalId;
                    hospitalApplicationObj.AppTypeId = model.AppTypeId;
                    hospitalApplicationObj.StatusId = 1;
                    hospitalApplicationObj.AppDate = DateTime.Today.Date;
                    if (model.DueDate != "")
                        hospitalApplicationObj.DueDate = DateTime.Parse(model.DueDate.ToString());
                    if (model.ActionDate != null)
                        hospitalApplicationObj.ActionDate = DateTime.Parse(model.ActionDate.ToString());
                    else
                        hospitalApplicationObj.ActionDate = DateTime.Now;

                    hospitalApplicationObj.AppNumber = model.AppNumber;
                    hospitalApplicationObj.UserId = model.UserId;
                    hospitalApplicationObj.Comment = model.Comment;
                    hospitalApplicationObj.AppDate = DateTime.Now;
                    _context.HospitalApplications.Add(hospitalApplicationObj);
                    _context.SaveChanges();
                    int id = hospitalApplicationObj.Id;
                }
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }
            return hospitalApplicationObj.Id;
        }

        public int Delete(int id)
        {
            var hospitalApplicationObj = _context.HospitalApplications.Find(id);
            try
            {
                if (hospitalApplicationObj != null)
                {
                    var lstTransactions = _context.HospitalReasonTransactions.Where(a => a.HospitalApplicationId == hospitalApplicationObj.Id).ToList();
                    if (lstTransactions.Count > 0)
                    {
                        foreach (var trans in lstTransactions)
                        {
                            var lstAttachments = _context.HospitalApplicationAttachments.Where(a => a.HospitalReasonTransactionId == trans.Id).ToList();
                            foreach (var attach in lstAttachments)
                            {
                                _context.HospitalApplicationAttachments.Remove(attach);
                                _context.SaveChanges();
                            }


                            _context.HospitalReasonTransactions.Remove(trans);
                            _context.SaveChanges();
                        }

                    }
                    _context.HospitalApplications.Remove(hospitalApplicationObj);
                    return _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }

            return 0;
        }

        public int Update(EditHospitalApplicationVM model)
        {
            try
            {

                var hospitalApplicationObj = _context.HospitalApplications.Find(model.Id);
                hospitalApplicationObj.AssetId = model.AssetId;
                hospitalApplicationObj.HospitalId = model.HospitalId;
                hospitalApplicationObj.AppTypeId = model.AppTypeId;
                hospitalApplicationObj.StatusId = 1;
                hospitalApplicationObj.AppDate = DateTime.Today.Date;
                if (model.DueDate != "")
                    hospitalApplicationObj.DueDate = DateTime.Parse(model.DueDate.ToString());
                if (model.ActionDate != "")
                    hospitalApplicationObj.ActionDate = DateTime.Parse(model.ActionDate.ToString());

                hospitalApplicationObj.AppNumber = model.AppNumber;
                hospitalApplicationObj.UserId = model.UserId;
                _context.Entry(hospitalApplicationObj).State = EntityState.Modified;
                _context.SaveChanges();


                if (model.ReasonIds.Count > 0)
                {

                    if (model.AppTypeId == 1)
                    {
                        List<int> reasonIds = new List<int>();
                        var savedReasonIds = (from execlude in _context.HospitalExecludeReasons
                                              join trans in _context.HospitalReasonTransactions on execlude.Id equals trans.ReasonId
                                              where trans.HospitalApplicationId == model.Id
                                                      && model.AppTypeId == 1
                                              select trans).ToList().Select(a => a.ReasonId).ToList();

                        foreach (var sr in savedReasonIds)
                        {
                            reasonIds.Add(sr.Value);
                        }



                        var savedIds = reasonIds.ToList().Except(model.ReasonIds);
                        if (savedIds.Count() > 0)
                        {
                            foreach (var item in savedIds)
                            {
                                var row = _context.HospitalReasonTransactions.Where(a => a.HospitalApplicationId == model.Id && a.ReasonId == item).ToList();
                                if (row.Count > 0)
                                {
                                    var reasonObj = row[0];
                                    _context.HospitalReasonTransactions.Remove(reasonObj);
                                    _context.SaveChanges();
                                }
                            }
                        }
                        var neewIds = model.ReasonIds.Except(reasonIds);
                        if (neewIds.Count() > 0)
                        {
                            foreach (var itm in neewIds)
                            {
                                HospitalReasonTransaction hospitalReasonObj = new HospitalReasonTransaction();
                                hospitalReasonObj.HospitalApplicationId = model.Id;
                                hospitalReasonObj.ReasonId = itm;
                                _context.HospitalReasonTransactions.Add(hospitalReasonObj);
                                _context.SaveChanges();
                            }
                        }

                    }



                    if (model.AppTypeId == 2)
                    {
                        List<int> reasonIds = new List<int>();
                        var savedReasonIds = (from execlude in _context.HospitalHoldReasons
                                              join trans in _context.HospitalReasonTransactions on execlude.Id equals trans.ReasonId
                                              where trans.HospitalApplicationId == model.Id
                                              && model.AppTypeId == 2
                                              select trans).ToList().Select(a => a.ReasonId).ToList();
                        foreach (var sr in savedReasonIds)
                        {
                            reasonIds.Add(sr.Value);
                        }



                        var savedIds = reasonIds.ToList().Except(model.ReasonIds);
                        if (savedIds.Count() > 0)
                        {
                            foreach (var item in savedIds)
                            {
                                var row = _context.HospitalReasonTransactions.Where(a => a.HospitalApplicationId == model.Id && a.ReasonId == item).ToList();
                                if (row.Count > 0)
                                {
                                    var reasonObj = row[0];
                                    _context.HospitalReasonTransactions.Remove(reasonObj);
                                    _context.SaveChanges();
                                }
                            }
                        }
                        var neewIds = model.ReasonIds.Except(reasonIds);
                        if (neewIds.Count() > 0)
                        {
                            foreach (var itm in neewIds)
                            {
                                HospitalReasonTransaction hospitalReasonObj = new HospitalReasonTransaction();
                                hospitalReasonObj.HospitalApplicationId = model.Id;
                                hospitalReasonObj.ReasonId = itm;
                                _context.HospitalReasonTransactions.Add(hospitalReasonObj);
                                _context.SaveChanges();
                            }
                        }

                    }



                    return hospitalApplicationObj.Id;

                }


            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }

            return 0;
        }

        public int CreateHospitalApplicationAttachments(HospitalApplicationAttachment attachObj)
        {
            HospitalApplicationAttachment assetAttachmentObj = new HospitalApplicationAttachment();
            assetAttachmentObj.HospitalReasonTransactionId = attachObj.HospitalReasonTransactionId;
            assetAttachmentObj.Title = "";
            assetAttachmentObj.FileName = attachObj.FileName;
            assetAttachmentObj.HospitalId = attachObj.HospitalId;
            _context.HospitalApplicationAttachments.Add(assetAttachmentObj);
            _context.SaveChanges();
            int Id = assetAttachmentObj.Id;
            return Id;
        }

        public IEnumerable<HospitalApplicationAttachment> GetAttachmentByHospitalApplicationId(int id)
        {
            return _context.HospitalApplicationAttachments.Where(a => a.HospitalReasonTransactionId == id).ToList();
        }

        public int DeleteHospitalApplicationAttachment(int id)
        {
            try
            {
                var attachObj = _context.HospitalApplicationAttachments.Find(id);
                _context.HospitalApplicationAttachments.Remove(attachObj);
                return _context.SaveChanges();
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }

            return 0;

        }

        public int UpdateExcludedDate(EditHospitalApplicationVM model)
        {
            var hospitalApplicationObj = _context.HospitalApplications.Find(model.Id);
            hospitalApplicationObj.StatusId = model.StatusId;

            if (model.StatusId == 2)
                hospitalApplicationObj.DueDate = DateTime.Parse(model.DueDate);
            if (model.StatusId == 3)
                hospitalApplicationObj.DueDate = DateTime.Today.Date;


            hospitalApplicationObj.DueDate = DateTime.Parse(model.DueDate.ToString());

            if (model.ActionDate != "")
                hospitalApplicationObj.ActionDate = DateTime.Today.Date;

            hospitalApplicationObj.UserId = model.UserId;
            hospitalApplicationObj.Comment = model.Comment;
            _context.Entry(hospitalApplicationObj).State = EntityState.Modified;
            _context.SaveChanges();


            if (model.StatusId == 2 && hospitalApplicationObj.AppTypeId == 1)
            {
                try
                {
                    AssetStatusTransaction assetStatusTransactionObj = new AssetStatusTransaction();
                    assetStatusTransactionObj.AssetDetailId = (int)model.AssetId;
                    assetStatusTransactionObj.AssetStatusId = 8;
                    assetStatusTransactionObj.HospitalId = hospitalApplicationObj.HospitalId;
                    var utcDate = DateTime.UtcNow;
                    DateTime localDate = utcDate.ToLocalTime();
                    assetStatusTransactionObj.StatusDate = localDate.Date;
                    _context.AssetStatusTransactions.Add(assetStatusTransactionObj);
                    _context.SaveChanges();
                }
                catch (Exception ex)
                {
                    string error = ex.Message;
                }
            }

            if (model.StatusId == 2 && hospitalApplicationObj.AppTypeId == 2)
            {
                AssetStatusTransaction assetStatusTransactionObj = new AssetStatusTransaction();
                assetStatusTransactionObj.AssetDetailId = (int)model.AssetId;
                assetStatusTransactionObj.AssetStatusId = 9;
                assetStatusTransactionObj.HospitalId = hospitalApplicationObj.HospitalId;

                var utcDate = DateTime.UtcNow;
                DateTime localDate = utcDate.ToLocalTime();
                assetStatusTransactionObj.StatusDate = localDate.Date;
                _context.AssetStatusTransactions.Add(assetStatusTransactionObj);
                _context.SaveChanges();
            }
            if (model.StatusId == 3 && (hospitalApplicationObj.AppTypeId == 1 || hospitalApplicationObj.AppTypeId == 2))
            {
                AssetStatusTransaction assetStatusTransactionObj = new AssetStatusTransaction();
                assetStatusTransactionObj.AssetDetailId = (int)model.AssetId;
                assetStatusTransactionObj.AssetStatusId = 3;
                assetStatusTransactionObj.HospitalId = hospitalApplicationObj.HospitalId;

                var utcDate = DateTime.UtcNow;
                DateTime localDate = utcDate.ToLocalTime();
                assetStatusTransactionObj.StatusDate = localDate.Date;
                _context.AssetStatusTransactions.Add(assetStatusTransactionObj);
                _context.SaveChanges();
            }

            if (model.StatusId == 4 && (hospitalApplicationObj.AppTypeId == 1 || hospitalApplicationObj.AppTypeId == 2))
            {
                AssetStatusTransaction assetStatusTransactionObj = new AssetStatusTransaction();
                assetStatusTransactionObj.AssetDetailId = (int)model.AssetId;
                assetStatusTransactionObj.AssetStatusId = 3;
                assetStatusTransactionObj.HospitalId = hospitalApplicationObj.HospitalId;


                var utcDate = DateTime.UtcNow;
                DateTime localDate = utcDate.ToLocalTime();

                assetStatusTransactionObj.StatusDate = DateTime.Today.Date;
                _context.AssetStatusTransactions.Add(assetStatusTransactionObj);
                _context.SaveChanges();
            }

            return hospitalApplicationObj.Id;
        }
        public GeneratedHospitalApplicationNumberVM GenerateHospitalApplicationNumber()
        {
            GeneratedHospitalApplicationNumberVM numberObj = new GeneratedHospitalApplicationNumberVM();
            string pre = "EXHLD";

            var lstHospitalApplications = _context.HospitalApplications.ToList();
            if (lstHospitalApplications.Count > 0)
            {
                var appNumber = lstHospitalApplications.LastOrDefault().Id;
                numberObj.AppNumber = pre + (appNumber + 1);
            }
            else
            {
                numberObj.AppNumber = pre + 1;
            }

            return numberObj;
        }









        private IQueryable<HospitalApplication> ListHospitalApplications()
        {
            return _context.HospitalApplications.Include(a => a.ApplicationType).Include(a => a.User)
                      .Include(a => a.AssetDetail)
                      .Include(a => a.AssetDetail.Department)
                      .Include(a => a.AssetDetail.MasterAsset)
                      .Include(a => a.AssetDetail.MasterAsset.brand)
                      .OrderByDescending(a => a.AppDate.Value.Date);
        }


        public IndexHospitalApplicationVM ListHospitalApplications(SortAndFilterHospitalApplicationVM data, int pageNumber, int pageSize)
        {

            #region Initial Variables
            IQueryable<HospitalApplication> query = ListHospitalApplications();
            IndexHospitalApplicationVM mainClass = new IndexHospitalApplicationVM();
            List<IndexHospitalApplicationVM.GetData> list = new List<IndexHospitalApplicationVM.GetData>();
            ApplicationUser userObj = new ApplicationUser();
            List<string> userRoleNames = new List<string>();
            Employee empObj = new Employee();
            #endregion

            #region User Role

            if (data.SearchObj.UserId != "")
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
                query = query.Where(a=>a.HospitalId == data.SearchObj.HospitalId);
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
            if (data.SearchObj.MasterAssetId != 0)
            {
                query = query.Where(x => x.AssetDetail.MasterAssetId == data.SearchObj.MasterAssetId);
            }
            if (data.SearchObj.AppTypeId != 0)
            {
                query = query.Where(x => x.AppTypeId == data.SearchObj.AppTypeId);
            }
            if (data.SearchObj.StatusId != 0)
            {
                query = query.Where(x => x.StatusId == data.SearchObj.StatusId);
            }
            if (data.SearchObj.HospitalId != 0)
            {
                query = query.Where(x => x.HospitalId == data.SearchObj.HospitalId);
            }
            if (data.SearchObj.GovernorateId != 0)
            {
                query = query.Where(x => x.AssetDetail.Hospital.GovernorateId == data.SearchObj.GovernorateId);
            }
            if (data.SearchObj.CityId != 0)
            {
                query = query.Where(x => x.AssetDetail.Hospital.CityId == data.SearchObj.CityId);
            }
            if (data.SearchObj.OrganizationId != null)
            {
                query = query.Where(x => x.AssetDetail.Hospital.OrganizationId == data.SearchObj.OrganizationId);
            }
            if (data.SearchObj.SubOrganizationId != null)
            {
                query = query.Where(x => x.AssetDetail.Hospital.SubOrganizationId == data.SearchObj.SubOrganizationId);
            }
            if (!string.IsNullOrEmpty(data.SearchObj.BarCode))
            {
                query = query.Where(x => x.AssetDetail.Barcode.Contains(data.SearchObj.BarCode));
            }
            if (!string.IsNullOrEmpty(data.SearchObj.Serial))
            {
                query = query.Where(x => x.AssetDetail.SerialNumber.Contains(data.SearchObj.Serial));
            }
            if (data.SearchObj.DepartmentId != 0)
            {
                query = query.Where(x => x.AssetDetail.DepartmentId == data.SearchObj.DepartmentId);
            }
            if (data.SearchObj.BrandId != 0)
            {
                query = query.Where(x => x.AssetDetail.MasterAsset.BrandId == data.SearchObj.BrandId);
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
                query = query.Where(a => a.AppDate.Value.Date >= startingFrom.Date && a.AppDate.Value.Date <= endingTo.Date);
            }


            #endregion

            #region Sort Criteria

            switch (data.SortObj.SortBy)
            {
                case "Barcode":
                case "الباركود":
                    if (data.SortObj.SortStatus == "ascending")
                    {
                        query = query.OrderBy(x => x.AssetDetail.Barcode);
                    }
                    else
                    {
                        query = query.OrderByDescending(x => x.AssetDetail.Barcode);
                    }
                    break;
                case "Serial":
                case "السيريال":
                    if (data.SortObj.SortStatus == "ascending")
                    {
                        query = query.OrderBy(x => x.AssetDetail.SerialNumber);
                    }
                    else
                    {
                        query = query.OrderByDescending(x => x.AssetDetail.SerialNumber);
                    }
                    break;
                case "ModelNumber":
                case "الموديل":
                    if (data.SortObj.SortStatus == "ascending")
                    {
                        query = query.OrderBy(x => x.AssetDetail.MasterAsset.ModelNumber);
                    }
                    else
                    {
                        query = query.OrderByDescending(x => x.AssetDetail.MasterAsset.ModelNumber);
                    }
                    break;
                case "Name":
                    if (data.SortObj.SortStatus == "ascending")
                    {
                        query.OrderBy(x => x.AssetDetail.MasterAsset.Name);
                    }
                    else
                    {
                        query.OrderByDescending(x => x.AssetDetail.MasterAsset.Name);
                    }
                    break;
                case "الاسم":
                    if (data.SortObj.SortStatus == "ascending")
                    {
                        query = query.OrderBy(x => x.AssetDetail.MasterAsset.NameAr);
                    }
                    else
                    {
                        query = query.OrderByDescending(x => x.AssetDetail.MasterAsset.NameAr);
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
                        query = query.OrderBy(x => x.AssetDetail.MasterAsset.brand.Name);
                    }
                    else
                    {
                        query = query.OrderByDescending(x => x.AssetDetail.MasterAsset.brand.Name);
                    }
                    break;
                case "الماركات":
                    if (data.SortObj.SortStatus == "ascending")
                    {
                        query = query.OrderBy(x => x.AssetDetail.MasterAsset.brand.NameAr);
                    }
                    else
                    {
                        query = query.OrderByDescending(x => x.AssetDetail.MasterAsset.brand.NameAr);
                    }
                    break;
                case "Department":
                    if (data.SortObj.SortStatus == "ascending")
                    {
                        query = query.OrderBy(x => x.AssetDetail.Department.Name);
                    }
                    else
                    {
                        query = query.OrderByDescending(x => x.AssetDetail.Department.Name);
                    }
                    break;
                case "القسم":
                    if (data.SortObj.SortStatus == "ascending")
                    {
                        query = query.OrderBy(x => x.AssetDetail.Department.NameAr);
                    }
                    else
                    {
                        query = query.OrderByDescending(x => x.AssetDetail.Department.NameAr);
                    }
                    break;
                default:
                    query = query.OrderByDescending(x => x.AppDate);
                    break;
            }

            #endregion

            #region Count data and fiter By Paging
            mainClass.Count = query.Count();
            IQueryable<HospitalApplication> lstResults = null;
            if (pageNumber == 0 && pageSize == 0)
            {
                lstResults = query;
            }
            else
            {
                lstResults = query.Skip((pageNumber - 1) * pageSize).Take(pageSize);
            }
            #endregion


            foreach (var item in lstResults)
            {

                IndexHospitalApplicationVM.GetData getDataObj = new IndexHospitalApplicationVM.GetData();
                getDataObj.Id = item.Id;
                getDataObj.HospitalId = item.HospitalId;
                getDataObj.AppNumber = item.AppNumber;
                getDataObj.Date = item.AppDate.Value.ToString();
                getDataObj.DueDate = item.DueDate != null ? item.DueDate.Value.ToString() : "";
                getDataObj.AppTypeId = item.AppTypeId;
                getDataObj.UserName = item.User.UserName;
                getDataObj.AssetId = item.AssetDetail.Id;
                getDataObj.AssetName = item.AssetDetail.MasterAsset.Name;
                getDataObj.AssetNameAr = item.AssetDetail.MasterAsset.NameAr;
                getDataObj.TypeName = item.ApplicationType.Name;
                getDataObj.TypeNameAr = item.ApplicationType.NameAr;
                getDataObj.SerialNumber = item.AssetDetail.SerialNumber;
                getDataObj.BarCode = item.AssetDetail.Barcode;
                getDataObj.ModelNumber = item.AssetDetail.MasterAsset.ModelNumber;

                getDataObj.BrandName = item.AssetDetail.MasterAsset.brand.Name;
                getDataObj.BrandNameAr = item.AssetDetail.MasterAsset.brand.NameAr;

                getDataObj.DiffMonths = ((item.AppDate.Value.Year - DateTime.Today.Date.Year) * 12) + item.AppDate.Value.Month - DateTime.Today.Date.Month;
                getDataObj.IsMoreThan3Months = getDataObj.DiffMonths <= -3 ? true : false;

                getDataObj.StatusId = item.StatusId;

                var lstStatuses = _context.HospitalSupplierStatuses.Where(a => a.Id == item.StatusId).ToList();
                if (lstStatuses.Count > 0)
                {
                    getDataObj.StatusName = lstStatuses[0].Name;
                    getDataObj.StatusNameAr = lstStatuses[0].NameAr;
                    getDataObj.StatusColor = lstStatuses[0].Color;
                    getDataObj.StatusIcon = lstStatuses[0].Icon;

                }

                var ReasonExTitles = (from execlude in _context.HospitalExecludeReasons
                                      join trans in _context.HospitalReasonTransactions on execlude.Id equals trans.ReasonId
                                      where trans.HospitalApplicationId == item.Id
                                      && item.AppTypeId == 1
                                      select execlude).ToList();
                if (ReasonExTitles.Count > 0)
                {
                    List<string> execludeNames = new List<string>();
                    foreach (var reason in ReasonExTitles)
                    {
                        execludeNames.Add(reason.Name);
                    }
                    getDataObj.ReasonExTitles = string.Join(",", execludeNames);

                    List<string> execludeNamesAr = new List<string>();
                    foreach (var reason in ReasonExTitles)
                    {
                        execludeNamesAr.Add(reason.NameAr);
                    }
                    getDataObj.ReasonExTitlesAr = string.Join(",", execludeNamesAr);
                }

                var ReasonHoldTitles = (from execlude in _context.HospitalHoldReasons
                                        join trans in _context.HospitalReasonTransactions on execlude.Id equals trans.ReasonId
                                        where trans.HospitalApplicationId == item.Id
                                        && item.AppTypeId == 2
                                        select execlude).ToList();
                if (ReasonHoldTitles.Count > 0)
                {
                    List<string> holdNames = new List<string>();
                    foreach (var reason in ReasonHoldTitles)
                    {
                        holdNames.Add(reason.Name);
                    }
                    getDataObj.ReasonHoldTitles = string.Join(",", holdNames);

                    List<string> holdNamesAr = new List<string>();
                    foreach (var reason in ReasonHoldTitles)
                    {
                        holdNamesAr.Add(reason.NameAr);
                    }
                    getDataObj.ReasonHoldTitlesAr = string.Join(",", holdNamesAr);
                }
                list.Add(getDataObj);
            }


            #region Represent data after Paging and count
            mainClass.Results = list;
            #endregion

            return mainClass;
        }



        //public ViewHospitalApplicationVM GetHospitalApplicationById(int id)
        //{
        //    var execludNames = (from execlude in _context.HospitalExecludeReasons
        //                        join trans in _context.HospitalReasonTransactions on execlude.Id equals trans.ReasonId
        //                        where trans.HospitalApplicationId == id
        //                        select execlude).ToList();


        //    var holdNames = (from hold in _context.HospitalHoldReasons
        //                     join trans in _context.HospitalReasonTransactions on hold.Id equals trans.ReasonId
        //                     where trans.HospitalApplicationId == id
        //                     select hold).ToList();



        //    ViewHospitalApplicationVM hospitalApplicationObj = new ViewHospitalApplicationVM();
        //    var lstHostApplication = _context.HospitalApplications.Include(a => a.User).Include(a => a.ApplicationType)
        //        .Include(a => a.User).Include(a => a.AssetDetail).Include(a => a.AssetDetail.MasterAsset)
        //        .Where(a => a.Id == id).ToList();
        //    if (lstHostApplication.Count > 0)
        //    {
        //        var item = lstHostApplication[0];

        //        hospitalApplicationObj.Id = item.Id;
        //        hospitalApplicationObj.StatusId = item.StatusId;
        //        hospitalApplicationObj.AppTypeId = item.AppTypeId;
        //        hospitalApplicationObj.AppDate = item.AppDate;
        //        if (item.DueDate != null)
        //            hospitalApplicationObj.DueDate = item.DueDate.Value.ToString();

        //        hospitalApplicationObj.AppNumber = item.AppNumber;
        //        hospitalApplicationObj.AppTypeName = item.ApplicationType.Name;
        //        hospitalApplicationObj.AppTypeNameAr = item.ApplicationType.NameAr;
        //        hospitalApplicationObj.HospitalId = (int)item.AssetDetail.HospitalId;
        //        hospitalApplicationObj.Comment = item.Comment;
        //        if (item.AppTypeId == 1)
        //        {
        //            hospitalApplicationObj.ReasonNames = execludNames;
        //        }
        //        else
        //        {
        //            hospitalApplicationObj.HoldReasonNames = holdNames;
        //        }
        //        hospitalApplicationObj.AssetId = item.AssetDetail.Id;
        //        hospitalApplicationObj.assetName = item.AssetDetail.MasterAsset.Name;// + " - " + item.AssetDetail.SerialNumber;
        //        hospitalApplicationObj.assetNameAr = item.AssetDetail.MasterAsset.NameAr;// + " - " + item.AssetDetail.SerialNumber;
        //        hospitalApplicationObj.SerialNumber = item.AssetDetail.SerialNumber;
        //        hospitalApplicationObj.BarCode = item.AssetDetail.Barcode;
        //    }


        //    return hospitalApplicationObj;



        //}

        //public int GetAssetHospitalId(int assetId)
        //{
        //    int hospitalId = 0;
        //    var hospitalAppObj = _context.HospitalApplications.Include(a => a.AssetDetail).Where(a => a.AssetId == assetId).FirstOrDefault();
        //    if (hospitalAppObj != null)
        //    {
        //        hospitalId = int.Parse(hospitalAppObj.AssetDetail.HospitalId.ToString());
        //    }

        //    return hospitalId;
        //}

        //public IEnumerable<IndexHospitalApplicationVM.GetData> GetAllByStatusId(int statusId, int hospitalId)
        //{
        //    List<IndexHospitalApplicationVM.GetData> list = new List<IndexHospitalApplicationVM.GetData>();
        //    var lstHospitalApplications = _context.HospitalApplications.Include(a => a.ApplicationType).Include(a => a.User).Include(a => a.AssetDetail).Include(a => a.AssetDetail.MasterAsset).ToList();

        //    if (statusId != 0)
        //    {
        //        lstHospitalApplications = lstHospitalApplications.Where(a => a.StatusId == statusId).ToList();
        //    }
        //    if (hospitalId != 0)
        //    {
        //        lstHospitalApplications = lstHospitalApplications.Where(a => a.AssetDetail.HospitalId == hospitalId).ToList();
        //    }

        //    foreach (var item in lstHospitalApplications)
        //    {

        //        IndexHospitalApplicationVM.GetData getDataObj = new IndexHospitalApplicationVM.GetData();
        //        getDataObj.Id = item.Id;
        //        getDataObj.AppNumber = item.AppNumber;
        //        getDataObj.AssetId = item.AssetDetail.Id;
        //        getDataObj.Date = item.AppDate.Value.ToString();
        //        getDataObj.DueDate = item.DueDate != null ? item.DueDate.Value.ToString() : "";
        //        getDataObj.AppTypeId = item.AppTypeId;
        //        getDataObj.UserName = item.User.UserName;
        //        getDataObj.AssetId = item.AssetDetail.Id;
        //        getDataObj.AssetName = item.AssetDetail.MasterAsset.Name + " - " + item.AssetDetail.SerialNumber;
        //        getDataObj.AssetNameAr = item.AssetDetail.MasterAsset.NameAr + " - " + item.AssetDetail.SerialNumber;
        //        getDataObj.TypeName = item.ApplicationType.Name;
        //        getDataObj.TypeNameAr = item.ApplicationType.NameAr;
        //        getDataObj.DiffMonths = ((item.AppDate.Value.Year - DateTime.Today.Date.Year) * 12) + item.AppDate.Value.Month - DateTime.Today.Date.Month;
        //        getDataObj.IsMoreThan3Months = getDataObj.DiffMonths <= -3 ? true : false;
        //        getDataObj.StatusId = item.StatusId;

        //        foreach (var itm in lstHospitalApplications)
        //        {
        //            if (item.StatusId == 1)
        //            {

        //                //  lstHospitalApplications = lstHospitalApplications.Where(a => a.StatusId == itm.StatusId).ToList();
        //                getDataObj.OpenStatus = lstHospitalApplications.Count;
        //                getDataObj.StatusName = _context.HospitalSupplierStatuses.FirstOrDefault(a => a.Id == itm.StatusId).Name;
        //                getDataObj.StatusNameAr = _context.HospitalSupplierStatuses.FirstOrDefault(a => a.Id == itm.StatusId).NameAr;
        //            }
        //            if (item.StatusId == 2)
        //            {
        //                getDataObj.StatusName = _context.HospitalSupplierStatuses.FirstOrDefault(a => a.Id == itm.StatusId).Name;
        //                getDataObj.StatusNameAr = _context.HospitalSupplierStatuses.FirstOrDefault(a => a.Id == itm.StatusId).NameAr;
        //                // lstHospitalApplications = lstHospitalApplications.Where(a => a.StatusId == itm.StatusId).ToList();
        //                getDataObj.ApproveStatus = lstHospitalApplications.Count;
        //            }
        //            if (item.StatusId == 3)
        //            {
        //                //  lstHospitalApplications = lstHospitalApplications.Where(a => a.StatusId == itm.StatusId).ToList();
        //                getDataObj.StatusName = _context.HospitalSupplierStatuses.FirstOrDefault(a => a.Id == itm.StatusId).Name;
        //                getDataObj.StatusNameAr = _context.HospitalSupplierStatuses.FirstOrDefault(a => a.Id == itm.StatusId).NameAr;

        //                getDataObj.RejectStatus = lstHospitalApplications.Count;
        //            }
        //            if (item.StatusId == 4)
        //            {
        //                getDataObj.StatusName = _context.HospitalSupplierStatuses.FirstOrDefault(a => a.Id == itm.StatusId).Name;
        //                getDataObj.StatusNameAr = _context.HospitalSupplierStatuses.FirstOrDefault(a => a.Id == itm.StatusId).NameAr;
        //                //   lstHospitalApplications = lstHospitalApplications.Where(a => a.StatusId == itm.StatusId).ToList();
        //                getDataObj.SystemRejectStatus = lstHospitalApplications.Count;
        //            }
        //        }

        //        var ReasonExTitles = (from execlude in _context.HospitalExecludeReasons
        //                              join trans in _context.HospitalReasonTransactions on execlude.Id equals trans.ReasonId
        //                              where trans.HospitalApplicationId == item.Id
        //                              && item.AppTypeId == 1
        //                              select execlude).ToList();
        //        if (ReasonExTitles.Count > 0)
        //        {
        //            List<string> execludeNames = new List<string>();// { "John", "Anna", "Monica" };
        //            foreach (var reason in ReasonExTitles)
        //            {
        //                execludeNames.Add(reason.Name);
        //            }

        //            getDataObj.ReasonExTitles = string.Join(",", execludeNames);


        //            List<string> execludeNamesAr = new List<string>();
        //            foreach (var reason in ReasonExTitles)
        //            {
        //                execludeNamesAr.Add(reason.NameAr);
        //            }
        //            getDataObj.ReasonExTitlesAr = string.Join(",", execludeNamesAr);

        //        }

        //        var ReasonHoldTitles = (from execlude in _context.HospitalHoldReasons
        //                                join trans in _context.HospitalReasonTransactions on execlude.Id equals trans.ReasonId
        //                                where trans.HospitalApplicationId == item.Id
        //                                && item.AppTypeId == 2
        //                                select execlude).ToList();
        //        if (ReasonHoldTitles.Count > 0)
        //        {
        //            List<string> holdNames = new List<string>();
        //            foreach (var reason in ReasonHoldTitles)
        //            {
        //                holdNames.Add(reason.Name);
        //            }
        //            getDataObj.ReasonHoldTitles = string.Join(",", holdNames);

        //            List<string> holdNamesAr = new List<string>();
        //            foreach (var reason in ReasonHoldTitles)
        //            {
        //                holdNamesAr.Add(reason.NameAr);
        //            }
        //            getDataObj.ReasonHoldTitlesAr = string.Join(",", holdNamesAr);
        //        }


        //        list.Add(getDataObj);
        //    }

        //    return list;
        //}

        //public IEnumerable<IndexHospitalApplicationVM.GetData> SortHospitalApp(SortHospitalApplication sortObj)
        //{
        //    var list = GetAll();
        //    if (sortObj.AssetName != "")
        //    {
        //        if (sortObj.SortStatus == "descending")
        //            list = list.OrderByDescending(d => d.AssetName).ToList();
        //        else
        //            list = list.OrderBy(d => d.AssetName).ToList();
        //    }
        //    else if (sortObj.AssetNameAr != "")
        //    {
        //        if (sortObj.SortStatus == "descending")
        //            list = list.OrderByDescending(d => d.AssetNameAr).ToList();
        //        else
        //            list = list.OrderBy(d => d.AssetNameAr).ToList();
        //    }
        //    else if (sortObj.AssetName != "")
        //    {
        //        if (sortObj.SortStatus == "descending")
        //            list = list.OrderByDescending(d => d.AssetName).ToList();
        //        else
        //            list = list.OrderBy(d => d.AssetName).ToList();
        //    }
        //    else if (sortObj.TypeName != "")
        //    {
        //        if (sortObj.SortStatus == "descending")
        //            list = list.OrderByDescending(d => d.TypeName).ToList();
        //        else
        //            list = list.OrderBy(d => d.TypeName).ToList();
        //    }
        //    else if (sortObj.TypeNameAr != "")
        //    {
        //        if (sortObj.SortStatus == "descending")
        //            list = list.OrderByDescending(d => d.TypeNameAr).ToList();
        //        else
        //            list = list.OrderBy(d => d.TypeNameAr).ToList();
        //    }
        //    else if (sortObj.ReasonExTitles != "" || sortObj.ReasonHoldTitles != "")
        //    {
        //        if (sortObj.SortStatus == "descending")
        //            list = list.OrderByDescending(d => d.ReasonExTitles).ThenByDescending(d => d.ReasonHoldTitles).ToList();
        //        else
        //            list = list.OrderBy(d => d.ReasonExTitles).ThenBy(d => d.ReasonHoldTitles).ToList();
        //    }
        //    else if (sortObj.ReasonExTitlesAr != "" || sortObj.ReasonHoldTitlesAr != "")
        //    {
        //        if (sortObj.SortStatus == "descending")
        //            list = list.OrderByDescending(d => d.ReasonExTitlesAr).ThenByDescending(d => d.ReasonHoldTitlesAr).ToList();
        //        else
        //            list = list.OrderBy(d => d.ReasonExTitlesAr).ThenBy(d => d.ReasonHoldTitlesAr).ToList();
        //    }
        //    else if (sortObj.Date != "")
        //    {
        //        if (sortObj.SortStatus == "descending")
        //            list = list.OrderByDescending(d => d.Date).ToList();
        //        else
        //            list = list.OrderBy(d => d.Date).ToList();
        //    }
        //    else if (sortObj.StatusName != "")
        //    {
        //        if (sortObj.SortStatus == "descending")
        //            list = list.OrderByDescending(d => d.StatusName).ToList();
        //        else
        //            list = list.OrderBy(d => d.StatusName).ToList();
        //    }
        //    else if (sortObj.StatusNameAr != "")
        //    {
        //        if (sortObj.SortStatus == "descending")
        //            list = list.OrderByDescending(d => d.StatusNameAr).ToList();
        //        else
        //            list = list.OrderBy(d => d.StatusNameAr).ToList();
        //    }
        //    else if (sortObj.DueDate != "")
        //    {
        //        if (sortObj.SortStatus == "descending")
        //            list = list.OrderByDescending(d => d.DueDate).ToList();
        //        else
        //            list = list.OrderBy(d => d.DueDate).ToList();
        //    }
        //    else if (sortObj.AppNumber != "")
        //    {
        //        if (sortObj.SortStatus == "descending")
        //            list = list.OrderByDescending(d => d.AppNumber).ToList();
        //        else
        //            list = list.OrderBy(d => d.AppNumber).ToList();
        //    }

        //    else if (sortObj.SerialNumber != "")
        //    {
        //        if (sortObj.SortStatus == "descending")
        //            list = list.OrderByDescending(d => d.SerialNumber).ToList();
        //        else
        //            list = list.OrderBy(d => d.SerialNumber).ToList();
        //    }
        //    else if (sortObj.ModelNumber != "")
        //    {
        //        if (sortObj.SortStatus == "descending")
        //            list = list.OrderByDescending(d => d.ModelNumber).ToList();
        //        else
        //            list = list.OrderBy(d => d.ModelNumber).ToList();
        //    }
        //    else if (sortObj.BarCode != "")
        //    {
        //        if (sortObj.SortStatus == "descending")
        //            list = list.OrderByDescending(d => d.BarCode).ToList();
        //        else
        //            list = list.OrderBy(d => d.BarCode).ToList();
        //    }
        //    return list;
        //}

        //public IEnumerable<IndexHospitalApplicationVM.GetData> GetAllByHospitalId(int hospitalId)
        //{
        //    List<IndexHospitalApplicationVM.GetData> list = new List<IndexHospitalApplicationVM.GetData>();
        //    var lstHospitalApplications = _context.HospitalApplications.Include(a => a.ApplicationType).Include(a => a.User)
        //        .Include(a => a.AssetDetail).Include(a => a.AssetDetail.MasterAsset).ToList().OrderByDescending(a => a.AppDate).ToList();
        //    foreach (var item in lstHospitalApplications)
        //    {

        //        IndexHospitalApplicationVM.GetData getDataObj = new IndexHospitalApplicationVM.GetData();
        //        getDataObj.Id = item.Id;
        //        getDataObj.AppNumber = item.AppNumber;
        //        getDataObj.Date = item.AppDate.Value.ToString();
        //        getDataObj.DueDate = item.DueDate != null ? item.DueDate.Value.ToString() : "";
        //        getDataObj.AppTypeId = item.AppTypeId;
        //        getDataObj.UserName = item.User.UserName;
        //        getDataObj.AssetId = item.AssetDetail.Id;
        //        getDataObj.AssetName = item.AssetDetail.MasterAsset.Name + " - " + item.AssetDetail.SerialNumber;
        //        getDataObj.AssetNameAr = item.AssetDetail.MasterAsset.NameAr + " - " + item.AssetDetail.SerialNumber;
        //        getDataObj.TypeName = item.ApplicationType.Name;
        //        getDataObj.TypeNameAr = item.ApplicationType.NameAr;

        //        getDataObj.DiffMonths = ((item.AppDate.Value.Year - DateTime.Today.Date.Year) * 12) + item.AppDate.Value.Month - DateTime.Today.Date.Month;
        //        getDataObj.IsMoreThan3Months = getDataObj.DiffMonths <= -3 ? true : false;
        //        getDataObj.HospitalId = item.AssetDetail.HospitalId;

        //        var lstStatuses = _context.HospitalSupplierStatuses.Where(a => a.Id == item.StatusId).ToList();
        //        if (lstStatuses.Count > 0)
        //        {
        //            getDataObj.StatusName = lstStatuses[0].Name;
        //            getDataObj.StatusNameAr = lstStatuses[0].NameAr;
        //            getDataObj.StatusColor = lstStatuses[0].Color;
        //            getDataObj.StatusIcon = lstStatuses[0].Icon;
        //        }


        //        var ReasonExTitles = (from execlude in _context.HospitalExecludeReasons
        //                              join trans in _context.HospitalReasonTransactions on execlude.Id equals trans.ReasonId
        //                              where trans.HospitalApplicationId == item.Id
        //                              && item.AppTypeId == 1
        //                              select execlude).ToList();
        //        if (ReasonExTitles.Count > 0)
        //        {
        //            List<string> execludeNames = new List<string>();// { "John", "Anna", "Monica" };
        //            foreach (var reason in ReasonExTitles)
        //            {
        //                execludeNames.Add(reason.Name);
        //            }

        //            getDataObj.ReasonExTitles = string.Join(",", execludeNames);


        //            List<string> execludeNamesAr = new List<string>();
        //            foreach (var reason in ReasonExTitles)
        //            {
        //                execludeNamesAr.Add(reason.NameAr);
        //            }
        //            getDataObj.ReasonExTitlesAr = string.Join(",", execludeNamesAr);

        //        }

        //        var ReasonHoldTitles = (from execlude in _context.HospitalHoldReasons
        //                                join trans in _context.HospitalReasonTransactions on execlude.Id equals trans.ReasonId
        //                                where trans.HospitalApplicationId == item.Id
        //                                && item.AppTypeId == 2
        //                                select execlude).ToList();
        //        if (ReasonHoldTitles.Count > 0)
        //        {
        //            List<string> holdNames = new List<string>();
        //            foreach (var reason in ReasonHoldTitles)
        //            {
        //                holdNames.Add(reason.Name);
        //            }
        //            getDataObj.ReasonHoldTitles = string.Join(",", holdNames);

        //            List<string> holdNamesAr = new List<string>();
        //            foreach (var reason in ReasonHoldTitles)
        //            {
        //                holdNamesAr.Add(reason.NameAr);
        //            }
        //            getDataObj.ReasonHoldTitlesAr = string.Join(",", holdNamesAr);
        //        }


        //        list.Add(getDataObj);
        //    }
        //    if (hospitalId == 0)
        //    {
        //        list = list.ToList();
        //    }
        //    else
        //    {
        //        list = list.Where(a => a.HospitalId == hospitalId).ToList();
        //    }

        //    return list;
        //}

        //public IEnumerable<IndexHospitalApplicationVM.GetData> GetAllByAppTypeId(int appTypeId)
        //{
        //    List<IndexHospitalApplicationVM.GetData> list = new List<IndexHospitalApplicationVM.GetData>();
        //    var lstHospitalApplications = _context.HospitalApplications.Include(a => a.ApplicationType).Include(a => a.User)
        //        .Include(a => a.AssetDetail).Include(a => a.AssetDetail.MasterAsset).ToList().OrderByDescending(a => a.AppDate.Value.Date).ToList();

        //    if (appTypeId != 0)
        //        lstHospitalApplications = lstHospitalApplications.Where(a => a.AppTypeId == appTypeId).ToList();


        //    foreach (var item in lstHospitalApplications)
        //    {

        //        IndexHospitalApplicationVM.GetData getDataObj = new IndexHospitalApplicationVM.GetData();
        //        getDataObj.Id = item.Id;
        //        getDataObj.HospitalId = item.HospitalId;
        //        getDataObj.AppNumber = item.AppNumber;
        //        getDataObj.Date = item.AppDate.Value.ToString();
        //        getDataObj.DueDate = item.DueDate != null ? item.DueDate.Value.ToString() : "";
        //        getDataObj.AppTypeId = item.AppTypeId;
        //        getDataObj.UserName = item.User.UserName;
        //        getDataObj.AssetId = item.AssetDetail.Id;
        //        getDataObj.AssetName = item.AssetDetail.MasterAsset.Name + " - " + item.AssetDetail.SerialNumber;
        //        getDataObj.AssetNameAr = item.AssetDetail.MasterAsset.NameAr + " - " + item.AssetDetail.SerialNumber;
        //        getDataObj.TypeName = item.ApplicationType.Name;
        //        getDataObj.TypeNameAr = item.ApplicationType.NameAr;

        //        getDataObj.DiffMonths = ((item.AppDate.Value.Year - DateTime.Today.Date.Year) * 12) + item.AppDate.Value.Month - DateTime.Today.Date.Month;
        //        getDataObj.IsMoreThan3Months = getDataObj.DiffMonths <= -3 ? true : false;

        //        getDataObj.StatusId = item.StatusId;

        //        var lstStatuses = _context.HospitalSupplierStatuses.Where(a => a.Id == item.StatusId).ToList();
        //        if (lstStatuses.Count > 0)
        //        {
        //            getDataObj.StatusName = lstStatuses[0].Name;
        //            getDataObj.StatusNameAr = lstStatuses[0].NameAr;
        //            getDataObj.StatusColor = lstStatuses[0].Color;
        //            getDataObj.StatusIcon = lstStatuses[0].Icon;
        //        }




        //        var ReasonExTitles = (from execlude in _context.HospitalExecludeReasons
        //                              join trans in _context.HospitalReasonTransactions on execlude.Id equals trans.ReasonId
        //                              where trans.HospitalApplicationId == item.Id
        //                              && item.AppTypeId == 1
        //                              select execlude).ToList();
        //        if (ReasonExTitles.Count > 0)
        //        {
        //            List<string> execludeNames = new List<string>();// { "John", "Anna", "Monica" };
        //            foreach (var reason in ReasonExTitles)
        //            {
        //                execludeNames.Add(reason.Name);
        //            }

        //            getDataObj.ReasonExTitles = string.Join(",", execludeNames);


        //            List<string> execludeNamesAr = new List<string>();
        //            foreach (var reason in ReasonExTitles)
        //            {
        //                execludeNamesAr.Add(reason.NameAr);
        //            }
        //            getDataObj.ReasonExTitlesAr = string.Join(",", execludeNamesAr);

        //        }

        //        var ReasonHoldTitles = (from execlude in _context.HospitalHoldReasons
        //                                join trans in _context.HospitalReasonTransactions on execlude.Id equals trans.ReasonId
        //                                where trans.HospitalApplicationId == item.Id
        //                                && item.AppTypeId == 2
        //                                select execlude).ToList();
        //        if (ReasonHoldTitles.Count > 0)
        //        {
        //            List<string> holdNames = new List<string>();
        //            foreach (var reason in ReasonHoldTitles)
        //            {
        //                holdNames.Add(reason.Name);
        //            }
        //            getDataObj.ReasonHoldTitles = string.Join(",", holdNames);

        //            List<string> holdNamesAr = new List<string>();
        //            foreach (var reason in ReasonHoldTitles)
        //            {
        //                holdNamesAr.Add(reason.NameAr);
        //            }
        //            getDataObj.ReasonHoldTitlesAr = string.Join(",", holdNamesAr);
        //        }


        //        list.Add(getDataObj);
        //    }

        //    return list;
        //}

        //public IEnumerable<IndexHospitalApplicationVM.GetData> GetAllByAppTypeIdAndStatusId(int statusId, int appTypeId, int hospitalId)
        //{
        //    List<IndexHospitalApplicationVM.GetData> list = new List<IndexHospitalApplicationVM.GetData>();
        //    var lstHospitalApplications = _context.HospitalApplications.Include(a => a.ApplicationType).Include(a => a.User)
        //        .Include(a => a.HospitalSupplierStatus).Include(a => a.ApplicationType)
        //        .Include(a => a.AssetDetail).Include(a => a.AssetDetail.Hospital).Include(a => a.AssetDetail.MasterAsset).ToList()
        //        .OrderByDescending(a => a.AppDate.Value.Date).ToList();


        //    if (lstHospitalApplications.Count > 0)
        //    {
        //        if (hospitalId != 0)
        //            lstHospitalApplications = lstHospitalApplications.Where(a => a.AssetDetail.HospitalId == hospitalId).ToList();

        //        if (appTypeId != 0)
        //            lstHospitalApplications = lstHospitalApplications.Where(a => a.AppTypeId == appTypeId).ToList();

        //        if (statusId != 0)
        //            lstHospitalApplications = lstHospitalApplications.Where(a => a.StatusId == statusId).ToList();



        //        foreach (var item in lstHospitalApplications)
        //        {

        //            IndexHospitalApplicationVM.GetData getDataObj = new IndexHospitalApplicationVM.GetData();
        //            getDataObj.Id = item.Id;
        //            getDataObj.HospitalId = item.HospitalId;
        //            getDataObj.AppNumber = item.AppNumber;
        //            getDataObj.Date = item.AppDate.Value.ToString();
        //            getDataObj.DueDate = item.DueDate != null ? item.DueDate.Value.ToString() : "";
        //            getDataObj.AppTypeId = item.AppTypeId;
        //            getDataObj.UserName = item.User.UserName;
        //            getDataObj.AssetId = item.AssetDetail.Id;
        //            getDataObj.AssetName = item.AssetDetail.MasterAsset.Name;// + " - " + item.AssetDetail.SerialNumber;
        //            getDataObj.AssetNameAr = item.AssetDetail.MasterAsset.NameAr;// + " - " + item.AssetDetail.SerialNumber;
        //            getDataObj.TypeName = item.ApplicationType.Name;
        //            getDataObj.TypeNameAr = item.ApplicationType.NameAr;


        //            getDataObj.SerialNumber = item.AssetDetail.SerialNumber;
        //            getDataObj.BarCode = item.AssetDetail.Barcode;
        //            getDataObj.ModelNumber = item.AssetDetail.MasterAsset.ModelNumber;


        //            getDataObj.DiffMonths = ((item.AppDate.Value.Year - DateTime.Today.Date.Year) * 12) + item.AppDate.Value.Month - DateTime.Today.Date.Month;
        //            getDataObj.IsMoreThan3Months = getDataObj.DiffMonths <= -3 ? true : false;

        //            getDataObj.StatusId = item.StatusId;
        //            getDataObj.StatusName = item.HospitalSupplierStatus.Name;
        //            getDataObj.StatusNameAr = item.HospitalSupplierStatus.NameAr;
        //            getDataObj.StatusColor = item.HospitalSupplierStatus.Color;
        //            getDataObj.StatusIcon = item.HospitalSupplierStatus.Icon;


        //            if (appTypeId == 1)
        //            {

        //                var ReasonExTitles = (from execlude in _context.HospitalExecludeReasons
        //                                      join trans in _context.HospitalReasonTransactions on execlude.Id equals trans.ReasonId
        //                                      where trans.HospitalApplicationId == item.Id
        //                                      && item.AppTypeId == 1
        //                                      select execlude).ToList();
        //                if (ReasonExTitles.Count > 0)
        //                {
        //                    List<string> execludeNames = new List<string>();// { "John", "Anna", "Monica" };
        //                    foreach (var reason in ReasonExTitles)
        //                    {
        //                        execludeNames.Add(reason.Name);
        //                    }

        //                    getDataObj.ReasonExTitles = string.Join(",", execludeNames);


        //                    List<string> execludeNamesAr = new List<string>();
        //                    foreach (var reason in ReasonExTitles)
        //                    {
        //                        execludeNamesAr.Add(reason.NameAr);
        //                    }
        //                    getDataObj.ReasonExTitlesAr = string.Join(",", execludeNamesAr);

        //                }
        //            }
        //            if (appTypeId == 2)
        //            {
        //                var ReasonHoldTitles = (from execlude in _context.HospitalHoldReasons
        //                                        join trans in _context.HospitalReasonTransactions on execlude.Id equals trans.ReasonId
        //                                        where trans.HospitalApplicationId == item.Id
        //                                        && item.AppTypeId == 2
        //                                        select execlude).ToList();
        //                if (ReasonHoldTitles.Count > 0)
        //                {
        //                    List<string> holdNames = new List<string>();
        //                    foreach (var reason in ReasonHoldTitles)
        //                    {
        //                        holdNames.Add(reason.Name);
        //                    }
        //                    getDataObj.ReasonHoldTitles = string.Join(",", holdNames);

        //                    List<string> holdNamesAr = new List<string>();
        //                    foreach (var reason in ReasonHoldTitles)
        //                    {
        //                        holdNamesAr.Add(reason.NameAr);
        //                    }
        //                    getDataObj.ReasonHoldTitlesAr = string.Join(",", holdNamesAr);
        //                }
        //            }

        //            list.Add(getDataObj);
        //        }
        //    }
        //    return list;
        //}
        //public IndexHospitalApplicationVM GetAllHospitalExecludes(SearchHospitalApplicationVM searchObj, int statusId, int appTypeId, int hospitalId, int pageNumber, int pageSize)
        //{

        //    IndexHospitalApplicationVM mainClass = new IndexHospitalApplicationVM();
        //    List<IndexHospitalApplicationVM.GetData> list = new List<IndexHospitalApplicationVM.GetData>();

        //    var lstHospitalApplications = _context.HospitalApplications
        //        .Include(a => a.ApplicationType).Include(a => a.User)
        //        .Include(a => a.HospitalSupplierStatus).Include(a => a.ApplicationType)
        //        .Include(a => a.AssetDetail).Include(a => a.AssetDetail.Hospital).Include(a => a.AssetDetail.MasterAsset)
        //        .Include(a => a.AssetDetail.MasterAsset.brand).ToList()
        //        .OrderByDescending(a => a.AppDate.Value.Date)
        //        .Where(a => (a.StatusId == statusId || a.StatusId == 4) && a.AppTypeId == appTypeId).ToList();


        //    if (lstHospitalApplications.Count > 0)
        //    {
        //        if (hospitalId != 0)
        //            lstHospitalApplications = lstHospitalApplications.Where(a => a.AssetDetail.HospitalId == hospitalId).ToList();

        //        if (appTypeId != 0)
        //            lstHospitalApplications = lstHospitalApplications.Where(a => a.AppTypeId == appTypeId).ToList();

        //        if (statusId != 0)
        //            lstHospitalApplications = lstHospitalApplications.Where(a => a.StatusId == statusId || a.StatusId == 4).ToList();



        //        if (searchObj.strStartDate != "")
        //            searchObj.StartDate = DateTime.Parse(searchObj.strStartDate);


        //        if (searchObj.strEndDate != "")
        //            searchObj.EndDate = DateTime.Parse(searchObj.strEndDate);




        //        if (searchObj.StartDate != null && searchObj.EndDate != null)
        //        {
        //            lstHospitalApplications = lstHospitalApplications.Where(a => a.AppDate.Value.Date >= searchObj.StartDate.Value.Date && a.AppDate.Value.Date <= searchObj.EndDate.Value.Date).ToList();
        //        }
        //        else
        //        {
        //            lstHospitalApplications = lstHospitalApplications.ToList();
        //        }



        //        foreach (var item in lstHospitalApplications)
        //        {

        //            IndexHospitalApplicationVM.GetData getDataObj = new IndexHospitalApplicationVM.GetData();
        //            getDataObj.Id = item.Id;
        //            getDataObj.HospitalId = item.HospitalId;
        //            getDataObj.AppNumber = item.AppNumber;
        //            getDataObj.Date = item.AppDate.Value.ToString();
        //            getDataObj.DueDate = item.DueDate != null ? item.DueDate.Value.ToString() : "";
        //            getDataObj.AppTypeId = item.AppTypeId;
        //            getDataObj.UserName = item.User.UserName;
        //            getDataObj.AssetId = item.AssetDetail.Id;
        //            getDataObj.AssetName = item.AssetDetail.MasterAsset.Name;
        //            getDataObj.AssetNameAr = item.AssetDetail.MasterAsset.NameAr;
        //            getDataObj.TypeName = item.ApplicationType.Name;
        //            getDataObj.TypeNameAr = item.ApplicationType.NameAr;

        //            if (item.AssetDetail.MasterAsset.brand != null)
        //            {
        //                getDataObj.BrandName = item.AssetDetail.MasterAsset.brand.Name;
        //                getDataObj.BrandNameAr = item.AssetDetail.MasterAsset.brand.NameAr;
        //            }

        //            getDataObj.FixCost = item.AssetDetail.FixCost;
        //            getDataObj.CostPerDay = item.AssetDetail.FixCost != null ? Math.Round((((decimal)item.AssetDetail.FixCost) / 365)) : 0;
        //            if (searchObj.EndDate != null)
        //            {
        //                getDataObj.AllDays = Math.Round((DateTime.Parse(searchObj.EndDate.ToString()) - DateTime.Parse(item.AppDate.ToString()).Date).TotalDays);
        //            }
        //            else
        //            {
        //                getDataObj.AllDays = 0;
        //            }


        //            if (getDataObj.CostPerDay != 0 && getDataObj.AllDays != 0)
        //                getDataObj.TotalCost = Math.Round((decimal)getDataObj.CostPerDay * (decimal)getDataObj.AllDays);
        //            else
        //                getDataObj.TotalCost = 0;





        //            getDataObj.SerialNumber = item.AssetDetail.SerialNumber;
        //            getDataObj.BarCode = item.AssetDetail.Barcode;
        //            getDataObj.ModelNumber = item.AssetDetail.MasterAsset.ModelNumber;


        //            getDataObj.DiffMonths = ((item.AppDate.Value.Year - DateTime.Today.Date.Year) * 12) + item.AppDate.Value.Month - DateTime.Today.Date.Month;
        //            getDataObj.IsMoreThan3Months = getDataObj.DiffMonths <= -3 ? true : false;

        //            getDataObj.StatusId = item.StatusId;
        //            getDataObj.StatusName = item.HospitalSupplierStatus.Name;
        //            getDataObj.StatusNameAr = item.HospitalSupplierStatus.NameAr;
        //            getDataObj.StatusColor = item.HospitalSupplierStatus.Color;
        //            getDataObj.StatusIcon = item.HospitalSupplierStatus.Icon;


        //            if (appTypeId == 1)
        //            {

        //                var ReasonExTitles = (from execlude in _context.HospitalExecludeReasons
        //                                      join trans in _context.HospitalReasonTransactions on execlude.Id equals trans.ReasonId
        //                                      where trans.HospitalApplicationId == item.Id
        //                                      && item.AppTypeId == 1
        //                                      select execlude).ToList();
        //                if (ReasonExTitles.Count > 0)
        //                {
        //                    List<string> execludeNames = new List<string>();// { "John", "Anna", "Monica" };
        //                    foreach (var reason in ReasonExTitles)
        //                    {
        //                        execludeNames.Add(reason.Name);
        //                    }

        //                    getDataObj.ReasonExTitles = string.Join(",", execludeNames);


        //                    List<string> execludeNamesAr = new List<string>();
        //                    foreach (var reason in ReasonExTitles)
        //                    {
        //                        execludeNamesAr.Add(reason.NameAr);
        //                    }
        //                    getDataObj.ReasonExTitlesAr = string.Join(",", execludeNamesAr);

        //                }
        //            }
        //            if (appTypeId == 2)
        //            {
        //                var ReasonHoldTitles = (from execlude in _context.HospitalHoldReasons
        //                                        join trans in _context.HospitalReasonTransactions on execlude.Id equals trans.ReasonId
        //                                        where trans.HospitalApplicationId == item.Id
        //                                        && item.AppTypeId == 2
        //                                        select execlude).ToList();
        //                if (ReasonHoldTitles.Count > 0)
        //                {
        //                    List<string> holdNames = new List<string>();
        //                    foreach (var reason in ReasonHoldTitles)
        //                    {
        //                        holdNames.Add(reason.Name);
        //                    }
        //                    getDataObj.ReasonHoldTitles = string.Join(",", holdNames);

        //                    List<string> holdNamesAr = new List<string>();
        //                    foreach (var reason in ReasonHoldTitles)
        //                    {
        //                        holdNamesAr.Add(reason.NameAr);
        //                    }
        //                    getDataObj.ReasonHoldTitlesAr = string.Join(",", holdNamesAr);
        //                }
        //            }

        //            list.Add(getDataObj);
        //        }




        //        var requestsPerPage = list.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
        //        mainClass.Results = requestsPerPage;
        //        mainClass.Count = lstHospitalApplications.Count();
        //        //  return mainClass;
        //    }
        //    return mainClass;

        //}
        //public IndexHospitalApplicationVM GetAllHospitalHolds(SearchHospitalApplicationVM searchObj, int statusId, int appTypeId, int hospitalId, int pageNumber, int pageSize)
        //{
        //    IndexHospitalApplicationVM mainClass = new IndexHospitalApplicationVM();
        //    List<IndexHospitalApplicationVM.GetData> list = new List<IndexHospitalApplicationVM.GetData>();

        //    var lstHospitalApplications = _context.HospitalApplications.Include(a => a.ApplicationType).Include(a => a.User)
        //        .Include(a => a.HospitalSupplierStatus).Include(a => a.ApplicationType)
        //        .Include(a => a.AssetDetail).Include(a => a.AssetDetail.Hospital).Include(a => a.AssetDetail.MasterAsset).Include(a => a.AssetDetail.MasterAsset.brand).ToList()
        //        .OrderByDescending(a => a.AppDate.Value.Date).Where(a => a.StatusId == statusId && a.AppTypeId == appTypeId).ToList();


        //    if (lstHospitalApplications.Count > 0)
        //    {
        //        if (hospitalId != 0)
        //            lstHospitalApplications = lstHospitalApplications.Where(a => a.AssetDetail.HospitalId == hospitalId).ToList();

        //        if (appTypeId != 0)
        //            lstHospitalApplications = lstHospitalApplications.Where(a => a.AppTypeId == appTypeId).ToList();

        //        if (statusId != 0)
        //            lstHospitalApplications = lstHospitalApplications.Where(a => a.StatusId == statusId).ToList();


        //        if (searchObj.strStartDate != "")
        //            searchObj.StartDate = DateTime.Parse(searchObj.strStartDate);


        //        if (searchObj.strEndDate != "")
        //            searchObj.EndDate = DateTime.Parse(searchObj.strEndDate);


        //        if (searchObj.StartDate != null || searchObj.EndDate != null)
        //        {
        //            lstHospitalApplications = lstHospitalApplications.Where(a => a.AppDate.Value.Date >= searchObj.StartDate.Value.Date && a.AppDate.Value.Date <= searchObj.EndDate.Value.Date).ToList();
        //        }
        //        else
        //        {
        //            lstHospitalApplications = lstHospitalApplications.ToList();
        //        }


        //        foreach (var item in lstHospitalApplications)
        //        {

        //            IndexHospitalApplicationVM.GetData getDataObj = new IndexHospitalApplicationVM.GetData();
        //            getDataObj.Id = item.Id;
        //            getDataObj.HospitalId = item.HospitalId;
        //            getDataObj.AppNumber = item.AppNumber;
        //            getDataObj.Date = item.AppDate.Value.ToString();
        //            getDataObj.DueDate = item.DueDate != null ? item.DueDate.Value.ToString() : "";
        //            getDataObj.AppTypeId = item.AppTypeId;
        //            getDataObj.UserName = item.User.UserName;
        //            getDataObj.AssetId = item.AssetDetail.Id;
        //            getDataObj.AssetName = item.AssetDetail.MasterAsset.Name;// + " - " + item.AssetDetail.SerialNumber;
        //            getDataObj.AssetNameAr = item.AssetDetail.MasterAsset.NameAr;// + " - " + item.AssetDetail.SerialNumber;
        //            getDataObj.TypeName = item.ApplicationType.Name;
        //            getDataObj.TypeNameAr = item.ApplicationType.NameAr;


        //            if (item.AssetDetail.MasterAsset.brand != null)
        //            {
        //                getDataObj.BrandName = item.AssetDetail.MasterAsset.brand.Name;
        //                getDataObj.BrandNameAr = item.AssetDetail.MasterAsset.brand.NameAr;
        //            }

        //            getDataObj.FixCost = item.AssetDetail.FixCost;
        //            getDataObj.CostPerDay = item.AssetDetail.FixCost != null ? Math.Round((((decimal)item.AssetDetail.FixCost) / 365)) : 0;
        //            if (searchObj.EndDate != null)
        //            {
        //                getDataObj.AllDays = Math.Round((DateTime.Parse(searchObj.EndDate.ToString()) - DateTime.Parse(item.AppDate.ToString()).Date).TotalDays);
        //            }
        //            else
        //            {
        //                getDataObj.AllDays = 0;
        //            }


        //            if (getDataObj.CostPerDay != 0 && getDataObj.AllDays != 0)
        //                getDataObj.TotalCost = Math.Round((decimal)getDataObj.CostPerDay * (decimal)getDataObj.AllDays);
        //            else
        //                getDataObj.TotalCost = 0;


        //            getDataObj.SerialNumber = item.AssetDetail.SerialNumber;
        //            getDataObj.BarCode = item.AssetDetail.Barcode;
        //            getDataObj.ModelNumber = item.AssetDetail.MasterAsset.ModelNumber;


        //            getDataObj.DiffMonths = ((item.AppDate.Value.Year - DateTime.Today.Date.Year) * 12) + item.AppDate.Value.Month - DateTime.Today.Date.Month;
        //            getDataObj.IsMoreThan3Months = getDataObj.DiffMonths <= -3 ? true : false;

        //            getDataObj.StatusId = item.StatusId;
        //            getDataObj.StatusName = item.HospitalSupplierStatus.Name;
        //            getDataObj.StatusNameAr = item.HospitalSupplierStatus.NameAr;
        //            getDataObj.StatusColor = item.HospitalSupplierStatus.Color;
        //            getDataObj.StatusIcon = item.HospitalSupplierStatus.Icon;


        //            if (appTypeId == 2)
        //            {
        //                var ReasonHoldTitles = (from execlude in _context.HospitalHoldReasons
        //                                        join trans in _context.HospitalReasonTransactions on execlude.Id equals trans.ReasonId
        //                                        where trans.HospitalApplicationId == item.Id
        //                                        && item.AppTypeId == appTypeId
        //                                        select execlude).ToList();
        //                if (ReasonHoldTitles.Count > 0)
        //                {
        //                    List<string> holdNames = new List<string>();
        //                    foreach (var reason in ReasonHoldTitles)
        //                    {
        //                        holdNames.Add(reason.Name);
        //                    }
        //                    getDataObj.ReasonHoldTitles = string.Join(",", holdNames);

        //                    List<string> holdNamesAr = new List<string>();
        //                    foreach (var reason in ReasonHoldTitles)
        //                    {
        //                        holdNamesAr.Add(reason.NameAr);
        //                    }
        //                    getDataObj.ReasonHoldTitlesAr = string.Join(",", holdNamesAr);
        //                }
        //            }

        //            list.Add(getDataObj);
        //        }




        //        var requestsPerPage = list.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
        //        mainClass.Results = requestsPerPage;
        //        mainClass.Count = lstHospitalApplications.Count();
        //        return mainClass;
        //    }

        //    mainClass.Results = new List<IndexHospitalApplicationVM.GetData>();
        //    mainClass.Count = 0;
        //    return mainClass;
        //}
        //public IndexHospitalApplicationVM GetAllHospitalExecludes(SearchHospitalApplicationVM searchObj)
        //{

        //    IndexHospitalApplicationVM mainClass = new IndexHospitalApplicationVM();
        //    List<IndexHospitalApplicationVM.GetData> list = new List<IndexHospitalApplicationVM.GetData>();

        //    var lstHospitalApplications = _context.HospitalApplications.Include(a => a.ApplicationType).Include(a => a.User)
        //        .Include(a => a.HospitalSupplierStatus).Include(a => a.ApplicationType)
        //        .Include(a => a.AssetDetail).Include(a => a.AssetDetail.Hospital).Include(a => a.AssetDetail.MasterAsset).Include(a => a.AssetDetail.MasterAsset.brand).ToList()
        //        .OrderByDescending(a => a.AppDate.Value.Date).Where(a => a.StatusId == searchObj.StatusId && a.AppTypeId == searchObj.AppTypeId).ToList();
        //    if (lstHospitalApplications.Count > 0)
        //    {

        //        if (lstHospitalApplications.Count > 0)
        //        {
        //            if (searchObj.HospitalId != 0)
        //                lstHospitalApplications = lstHospitalApplications.Where(a => a.AssetDetail.HospitalId == searchObj.HospitalId).ToList();

        //            if (searchObj.AppTypeId != 0)
        //                lstHospitalApplications = lstHospitalApplications.Where(a => a.AppTypeId == searchObj.AppTypeId).ToList();

        //            if (searchObj.StatusId != 0)
        //                lstHospitalApplications = lstHospitalApplications.Where(a => a.StatusId == searchObj.StatusId).ToList();



        //            if (searchObj.strStartDate != "")
        //                searchObj.StartDate = DateTime.Parse(searchObj.strStartDate);


        //            if (searchObj.strEndDate != "")
        //                searchObj.EndDate = DateTime.Parse(searchObj.strEndDate);




        //            if (searchObj.StartDate != null || searchObj.EndDate != null)
        //            {
        //                lstHospitalApplications = lstHospitalApplications.Where(a => a.AppDate.Value.Date >= searchObj.StartDate.Value.Date && a.AppDate.Value.Date <= searchObj.EndDate.Value.Date).ToList();
        //            }
        //            else
        //            {
        //                lstHospitalApplications = lstHospitalApplications.ToList();
        //            }



        //            foreach (var item in lstHospitalApplications)
        //            {

        //                IndexHospitalApplicationVM.GetData getDataObj = new IndexHospitalApplicationVM.GetData();
        //                getDataObj.Id = item.Id;
        //                getDataObj.HospitalId = item.HospitalId;
        //                getDataObj.AppNumber = item.AppNumber;
        //                getDataObj.Date = item.AppDate.Value.ToString();
        //                getDataObj.DueDate = item.DueDate != null ? item.DueDate.Value.ToString() : "";
        //                getDataObj.AppTypeId = item.AppTypeId;
        //                getDataObj.UserName = item.User.UserName;
        //                getDataObj.AssetId = item.AssetDetail.Id;
        //                getDataObj.AssetName = item.AssetDetail.MasterAsset.Name;
        //                getDataObj.AssetNameAr = item.AssetDetail.MasterAsset.NameAr;
        //                getDataObj.TypeName = item.ApplicationType.Name;
        //                getDataObj.TypeNameAr = item.ApplicationType.NameAr;

        //                if (item.AssetDetail.MasterAsset.brand != null)
        //                {
        //                    getDataObj.BrandName = item.AssetDetail.MasterAsset.brand.Name;
        //                    getDataObj.BrandNameAr = item.AssetDetail.MasterAsset.brand.NameAr;
        //                }

        //                getDataObj.FixCost = item.AssetDetail.FixCost;
        //                getDataObj.CostPerDay = item.AssetDetail.FixCost != null ? Math.Round((((decimal)item.AssetDetail.FixCost) / 365)) : 0;
        //                if (searchObj.EndDate != null)
        //                {
        //                    getDataObj.AllDays = Math.Round((DateTime.Parse(searchObj.EndDate.ToString()) - DateTime.Parse(item.AppDate.ToString()).Date).TotalDays);
        //                }
        //                else
        //                {
        //                    getDataObj.AllDays = 0;
        //                }


        //                if (getDataObj.CostPerDay != 0 && getDataObj.AllDays != 0)
        //                    getDataObj.TotalCost = Math.Round((decimal)getDataObj.CostPerDay * (decimal)getDataObj.AllDays);
        //                else
        //                    getDataObj.TotalCost = 0;

        //                getDataObj.SerialNumber = item.AssetDetail.SerialNumber;
        //                getDataObj.BarCode = item.AssetDetail.Barcode;
        //                getDataObj.ModelNumber = item.AssetDetail.MasterAsset.ModelNumber;
        //                list.Add(getDataObj);
        //            }



        //            mainClass.Results = list;
        //            mainClass.Count = list.Count();
        //            return mainClass;
        //        }
        //    }

        //    mainClass.Results = new List<IndexHospitalApplicationVM.GetData>();
        //    mainClass.Count = 0;
        //    return mainClass;

        //}
        //public IndexHospitalApplicationVM GetAllHospitalHolds(SearchHospitalApplicationVM searchObj)
        //{
        //    IndexHospitalApplicationVM mainClass = new IndexHospitalApplicationVM();
        //    List<IndexHospitalApplicationVM.GetData> list = new List<IndexHospitalApplicationVM.GetData>();

        //    var lstHospitalApplications = _context.HospitalApplications.Include(a => a.ApplicationType).Include(a => a.User)
        //        .Include(a => a.HospitalSupplierStatus).Include(a => a.ApplicationType)
        //        .Include(a => a.AssetDetail).Include(a => a.AssetDetail.Hospital).Include(a => a.AssetDetail.MasterAsset).Include(a => a.AssetDetail.MasterAsset.brand).ToList()
        //        .OrderByDescending(a => a.AppDate.Value.Date).Where(a => a.StatusId == searchObj.StatusId && a.AppTypeId == searchObj.AppTypeId).ToList();

        //    if (lstHospitalApplications.Count > 0)
        //    {
        //        if (lstHospitalApplications.Count > 0)
        //        {
        //            if (searchObj.HospitalId != 0)
        //                lstHospitalApplications = lstHospitalApplications.Where(a => a.AssetDetail.HospitalId == searchObj.HospitalId).ToList();

        //            if (searchObj.AppTypeId != 0)
        //                lstHospitalApplications = lstHospitalApplications.Where(a => a.AppTypeId == searchObj.AppTypeId).ToList();

        //            if (searchObj.StatusId != 0)
        //                lstHospitalApplications = lstHospitalApplications.Where(a => a.StatusId == searchObj.StatusId).ToList();



        //            if (searchObj.strStartDate != "")
        //                searchObj.StartDate = DateTime.Parse(searchObj.strStartDate);


        //            if (searchObj.strEndDate != "")
        //                searchObj.EndDate = DateTime.Parse(searchObj.strEndDate);




        //            if (searchObj.StartDate != null || searchObj.EndDate != null)
        //            {
        //                lstHospitalApplications = lstHospitalApplications.Where(a => a.AppDate.Value.Date >= searchObj.StartDate.Value.Date && a.AppDate.Value.Date <= searchObj.EndDate.Value.Date).ToList();
        //            }
        //            else
        //            {
        //                lstHospitalApplications = lstHospitalApplications.ToList();
        //            }



        //            foreach (var item in lstHospitalApplications)
        //            {

        //                IndexHospitalApplicationVM.GetData getDataObj = new IndexHospitalApplicationVM.GetData();
        //                getDataObj.Id = item.Id;
        //                getDataObj.HospitalId = item.HospitalId;
        //                getDataObj.AppNumber = item.AppNumber;
        //                getDataObj.Date = item.AppDate.Value.ToString();
        //                getDataObj.DueDate = item.DueDate != null ? item.DueDate.Value.ToString() : "";
        //                getDataObj.AppTypeId = item.AppTypeId;
        //                getDataObj.UserName = item.User.UserName;
        //                getDataObj.AssetId = item.AssetDetail.Id;
        //                getDataObj.AssetName = item.AssetDetail.MasterAsset.Name;
        //                getDataObj.AssetNameAr = item.AssetDetail.MasterAsset.NameAr;
        //                getDataObj.TypeName = item.ApplicationType.Name;
        //                getDataObj.TypeNameAr = item.ApplicationType.NameAr;

        //                if (item.AssetDetail.MasterAsset.brand != null)
        //                {
        //                    getDataObj.BrandName = item.AssetDetail.MasterAsset.brand.Name;
        //                    getDataObj.BrandNameAr = item.AssetDetail.MasterAsset.brand.NameAr;
        //                }

        //                getDataObj.FixCost = item.AssetDetail.FixCost;
        //                getDataObj.CostPerDay = item.AssetDetail.FixCost != null ? Math.Round((((decimal)item.AssetDetail.FixCost) / 365)) : 0;
        //                if (searchObj.EndDate != null)
        //                {
        //                    getDataObj.AllDays = Math.Round((DateTime.Parse(searchObj.EndDate.ToString()) - DateTime.Parse(item.AppDate.ToString()).Date).TotalDays);
        //                }
        //                else
        //                {
        //                    getDataObj.AllDays = 0;
        //                }


        //                if (getDataObj.CostPerDay != 0 && getDataObj.AllDays != 0)
        //                    getDataObj.TotalCost = Math.Round((decimal)getDataObj.CostPerDay * (decimal)getDataObj.AllDays);
        //                else
        //                    getDataObj.TotalCost = 0;

        //                getDataObj.SerialNumber = item.AssetDetail.SerialNumber;
        //                getDataObj.BarCode = item.AssetDetail.Barcode;
        //                getDataObj.ModelNumber = item.AssetDetail.MasterAsset.ModelNumber;
        //                list.Add(getDataObj);
        //            }

        //            mainClass.Results = list;
        //            mainClass.Count = list.Count();
        //            return mainClass;
        //        }
        //    }
        //    mainClass.Results = new List<IndexHospitalApplicationVM.GetData>();
        //    mainClass.Count = 0;
        //    return mainClass;
        //}





        //public IEnumerable<IndexHospitalApplicationVM.GetData> GetHospitalApplicationByDate(SearchHospitalApplicationVM searchObj)
        //{

        //    if (searchObj.strStartDate != "")
        //        searchObj.StartDate = DateTime.Parse(searchObj.strStartDate);
        //    else
        //        searchObj.StartDate = DateTime.Today.Date;


        //    if (searchObj.strEndDate != "")
        //        searchObj.EndDate = DateTime.Parse(searchObj.strEndDate);
        //    else
        //        searchObj.EndDate = DateTime.Today.Date;


        //    List<IndexHospitalApplicationVM.GetData> list = new List<IndexHospitalApplicationVM.GetData>();

        //    var lstHospitalApplications = _context.HospitalApplications.Include(a => a.ApplicationType).Include(a => a.User)
        //         .Include(a => a.HospitalSupplierStatus).Include(a => a.ApplicationType)
        //         .Include(a => a.AssetDetail).Include(a => a.AssetDetail.Hospital).Include(a => a.AssetDetail.MasterAsset).ToList()
        //         .Where(a => a.AppDate >= searchObj.StartDate.Value.Date && a.AppDate <= searchObj.EndDate.Value.Date)
        //         .OrderByDescending(a => a.AppDate.Value.Date).ToList();



        //    foreach (var item in lstHospitalApplications)
        //    {

        //        IndexHospitalApplicationVM.GetData getDataObj = new IndexHospitalApplicationVM.GetData();
        //        getDataObj.Id = item.Id;
        //        getDataObj.HospitalId = item.HospitalId;
        //        getDataObj.AppNumber = item.AppNumber;
        //        getDataObj.Date = item.AppDate.Value.ToString();
        //        getDataObj.DueDate = item.DueDate != null ? item.DueDate.Value.ToString() : "";
        //        getDataObj.AppTypeId = item.AppTypeId;
        //        getDataObj.UserName = item.User.UserName;
        //        getDataObj.AssetId = item.AssetDetail.Id;
        //        getDataObj.AssetName = item.AssetDetail.MasterAsset.Name;// + " - " + item.AssetDetail.SerialNumber;
        //        getDataObj.AssetNameAr = item.AssetDetail.MasterAsset.NameAr;// + " - " + item.AssetDetail.SerialNumber;
        //        getDataObj.SerialNumber = item.AssetDetail.SerialNumber;
        //        getDataObj.BarCode = item.AssetDetail.Barcode;
        //        getDataObj.ModelNumber = item.AssetDetail.MasterAsset.ModelNumber;

        //        getDataObj.TypeName = item.ApplicationType.Name;
        //        getDataObj.TypeNameAr = item.ApplicationType.NameAr;

        //        getDataObj.DiffMonths = ((item.AppDate.Value.Year - DateTime.Today.Date.Year) * 12) + item.AppDate.Value.Month - DateTime.Today.Date.Month;
        //        getDataObj.IsMoreThan3Months = getDataObj.DiffMonths <= -3 ? true : false;

        //        getDataObj.StatusId = item.StatusId;
        //        getDataObj.StatusName = item.HospitalSupplierStatus.Name;
        //        getDataObj.StatusNameAr = item.HospitalSupplierStatus.NameAr;
        //        getDataObj.StatusColor = item.HospitalSupplierStatus.Color;
        //        getDataObj.StatusIcon = item.HospitalSupplierStatus.Icon;
        //        switch (item.AppTypeId)
        //        {
        //            case 1:
        //                var ReasonExTitles = (from execlude in _context.HospitalExecludeReasons
        //                                      join trans in _context.HospitalReasonTransactions on execlude.Id equals trans.ReasonId
        //                                      where trans.HospitalApplicationId == item.Id
        //                                      && item.AppTypeId == 1
        //                                      select execlude).ToList();
        //                if (ReasonExTitles.Count > 0)
        //                {
        //                    List<string> execludeNames = new List<string>();
        //                    foreach (var reason in ReasonExTitles)
        //                    {
        //                        execludeNames.Add(reason.Name);
        //                    }

        //                    getDataObj.ReasonExTitles = string.Join(",", execludeNames);


        //                    List<string> execludeNamesAr = new List<string>();
        //                    foreach (var reason in ReasonExTitles)
        //                    {
        //                        execludeNamesAr.Add(reason.NameAr);
        //                    }
        //                    getDataObj.ReasonExTitlesAr = string.Join(",", execludeNamesAr);

        //                }
        //                break;
        //            case 2:
        //                var ReasonHoldTitles = (from execlude in _context.HospitalHoldReasons
        //                                        join trans in _context.HospitalReasonTransactions on execlude.Id equals trans.ReasonId
        //                                        where trans.HospitalApplicationId == item.Id
        //                                        && item.AppTypeId == 2
        //                                        select execlude).ToList();
        //                if (ReasonHoldTitles.Count > 0)
        //                {
        //                    List<string> holdNames = new List<string>();
        //                    foreach (var reason in ReasonHoldTitles)
        //                    {
        //                        holdNames.Add(reason.Name);
        //                    }
        //                    getDataObj.ReasonHoldTitles = string.Join(",", holdNames);

        //                    List<string> holdNamesAr = new List<string>();
        //                    foreach (var reason in ReasonHoldTitles)
        //                    {
        //                        holdNamesAr.Add(reason.NameAr);
        //                    }
        //                    getDataObj.ReasonHoldTitlesAr = string.Join(",", holdNamesAr);
        //                }
        //                break;
        //        }

        //        list.Add(getDataObj);
        //    }

        //    return list;
        //}
    }
}