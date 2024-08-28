using Asset.Domain.Repositories;
using Asset.Models;
using Asset.ViewModels.AssetDetailVM;
using Asset.ViewModels.ContractVM;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contract.Core.Repositories
{
    public class MasterContractRepositories : IMasterContractRepository
    {

        private ApplicationDbContext _context;

        public MasterContractRepositories(ApplicationDbContext context)
        {
            _context = context;
        }



        public IEnumerable<MasterContract> GetAll()
        {
            return _context.MasterContracts.ToList();
        }

        public DetailMasterContractVM GetById(int id)
        {
            var lstContracts = _context.MasterContracts
                 .Include(a => a.Supplier)
                                   .Include(a => a.Hospital)
                                   .Include(a => a.Supplier)
                                   .Where(a => a.Id == id).ToList();

            DetailMasterContractVM detailMasterObj = new DetailMasterContractVM();

            if (lstContracts.Count > 0)
            {
                var masterObj = lstContracts[0];
                detailMasterObj.Id = masterObj.Id;
                detailMasterObj.SupplierId = masterObj.SupplierId;
                detailMasterObj.HospitalId = masterObj.HospitalId;
                detailMasterObj.Subject = masterObj.Subject;
                detailMasterObj.Serial = masterObj.Serial;
                detailMasterObj.Cost = masterObj.Cost;
                detailMasterObj.ContractDate = masterObj.ContractDate.Value;
                detailMasterObj.From = masterObj.From.Value;
                detailMasterObj.To = masterObj.To.Value;
                detailMasterObj.Notes = masterObj.Notes;
                if (masterObj.Supplier != null)
                {
                    detailMasterObj.SupplierName = masterObj.Supplier.Name;
                    detailMasterObj.SupplierNameAr = masterObj.Supplier.NameAr;
                }
                else
                {
                    detailMasterObj.SupplierName = "";
                    detailMasterObj.SupplierNameAr = "";
                }


                detailMasterObj.ListDetails = _context.ContractDetails.Include(a => a.AssetDetail).Include(a => a.AssetDetail.Department)
                    .Include(a => a.AssetDetail.MasterAsset).Include(a => a.AssetDetail.MasterAsset.brand)
                    .Where(a => a.MasterContractId == masterObj.Id).ToList().Select(detail => new ContractDetailVM
                    {
                        AssetDetailId =  detail.AssetDetailId,
                        AssetName = detail.AssetDetail.MasterAsset.Name,
                        AssetNameAr = detail.AssetDetail.MasterAsset.NameAr,
                        BarCode = detail.AssetDetail.Barcode,
                        SerialNumber = detail.AssetDetail.SerialNumber,
                        HasSpareParts = detail.HasSpareParts,
                        ResponseTime = detail.ResponseTime,
                        BrandNameAr = detail.AssetDetail.MasterAsset.brand.NameAr,
                        BrandName = detail.AssetDetail.MasterAsset.brand.Name,
                        DepartmentNameAr = detail.AssetDetail.Department.NameAr,
                        DepartmentName = detail.AssetDetail.Department.Name
                    }).ToList();
            }

            return detailMasterObj;


            ;
        }

        public int Add(CreateMasterContractVM model)
        {
            MasterContract masterContractObj = new MasterContract();
            try
            {
                if (model != null)
                {
                    masterContractObj.ContractDate = model.ContractDate;
                    masterContractObj.From = model.From;
                    masterContractObj.To = model.To;
                    masterContractObj.Serial = model.Serial;
                    masterContractObj.Subject = model.Subject;
                    masterContractObj.Cost = model.Cost;
                    masterContractObj.HospitalId = model.HospitalId;
                    masterContractObj.SupplierId = model.SupplierId;
                    masterContractObj.Notes = model.Notes;
                    masterContractObj.TotalVisits = model.TotalVisits;

                    _context.MasterContracts.Add(masterContractObj);
                    _context.SaveChanges();
                    var masterId = masterContractObj.Id;

                    if (model.lstDetails.Count > 0)
                    {
                        foreach (var item in model.lstDetails)
                        {
                            ContractDetail detailObj = new ContractDetail();
                            detailObj.ContractDate = model.ContractDate;
                            detailObj.MasterContractId = masterId;
                            detailObj.AssetDetailId = item.AssetDetailId;
                            detailObj.ResponseTime = item.ResponseTime;
                            detailObj.HasSpareParts = item.HasSpareParts;
                            detailObj.HospitalId = item.HospitalId;
                            _context.ContractDetails.Add(detailObj);
                            _context.SaveChanges();
                        }

                        foreach (var item in model.lstDetails)
                        {
                            var assetDetailObj = _context.AssetDetails.Find(item.AssetDetailId);
                            assetDetailObj.MasterContractId = masterId;
                            _context.Entry(assetDetailObj).State = EntityState.Modified;
                            _context.SaveChanges();
                        }
                    }
                    return masterContractObj.Id;




                }
            }
            catch (Exception ex)
            {
                string str = ex.Message;
            }
            return 0;
        }
        public int Delete(int id)
        {
            var masterContractObj = _context.MasterContracts.Find(id);
            try
            {
                if (masterContractObj != null)
                {
                    var lstDetails = _context.ContractDetails.Where(a => a.MasterContractId == masterContractObj.Id).ToList();
                    foreach (var item in lstDetails)
                    {
                        _context.ContractDetails.Remove(item);
                        _context.SaveChanges();
                    }
                    _context.MasterContracts.Remove(masterContractObj);
                    return _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }

            return 0;
        }
        public int Update(MasterContract model)
        {
            try
            {
                var masterContractObj = _context.MasterContracts.Find(model.Id);
                masterContractObj.Id = model.Id;
                masterContractObj.ContractDate = model.ContractDate;
                masterContractObj.From = model.From;
                masterContractObj.To = model.To;
                masterContractObj.Subject = model.Subject;
                masterContractObj.Serial = model.Serial;
                masterContractObj.Cost = model.Cost;
                masterContractObj.SupplierId = model.SupplierId;
                masterContractObj.HospitalId = model.HospitalId;
                masterContractObj.Notes = model.Notes;
                masterContractObj.TotalVisits = model.TotalVisits;
                _context.Entry(masterContractObj).State = EntityState.Modified;
                _context.SaveChanges();
                return masterContractObj.Id;
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }

            return 0;
        }




        private IQueryable<ContractDetail> ListMasterContracts()
        {
            var lstMasterContracts = _context.ContractDetails.Include(a => a.MasterContract)
                                               .Include(a => a.AssetDetail)
                                               .Include(a => a.AssetDetail.Hospital)
                                               .Include(a => a.AssetDetail.Hospital.Governorate)
                                               .Include(a => a.AssetDetail.Hospital.City)
                                               .Include(a => a.AssetDetail.Hospital.Organization)
                                               .Include(a => a.AssetDetail.Hospital.SubOrganization)
                                               .Include(a => a.MasterContract.Supplier)
                                               .Include(a => a.AssetDetail.MasterAsset)
                                               .Include(a => a.AssetDetail.MasterAsset.brand)
                                               .Include(a => a.AssetDetail.MasterAsset.Origin);
            //.ToList()
            //.GroupBy(a => a.MasterContractId)
            //.Select(g => g.OrderByDescending(track => track.MasterContract.Serial).FirstOrDefault());
            return lstMasterContracts.AsQueryable();
        }

        public IndexMasterContractVM ListMasterContracts(SortAndFilterContractVM data, int pageNumber, int pageSize)
        {
            #region Initial Variables
            IQueryable<ContractDetail> query = ListMasterContracts();

            IndexMasterContractVM mainClass = new IndexMasterContractVM();
            List<IndexMasterContractVM.GetData> list = new List<IndexMasterContractVM.GetData>();
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
                query = query.Where(x => x.AssetDetail.Hospital.GovernorateId == data.SearchObj.GovernorateId);
            }
            if (data.SearchObj.CityId != 0)
            {
                query = query.Where(x => x.AssetDetail.Hospital.CityId == data.SearchObj.CityId);
            }
            if (data.SearchObj.OrganizationId != 0)
            {
                query = query.Where(x => x.AssetDetail.Hospital.OrganizationId == data.SearchObj.OrganizationId);
            }
            if (data.SearchObj.SubOrganizationId != 0)
            {
                query = query.Where(x => x.AssetDetail.Hospital.SubOrganizationId == data.SearchObj.SubOrganizationId);
            }
            if (data.SearchObj.HospitalId != 0)
            {
                query = query.Where(x => x.HospitalId == data.SearchObj.HospitalId);
            }
            if (data.SearchObj.MasterAssetId != 0)
            {
                query = query.Where(x => x.AssetDetail.MasterAssetId == data.SearchObj.MasterAssetId);
            }
            if (!string.IsNullOrEmpty(data.SearchObj.BarCode))
            {
                query = query.Where(x => x.AssetDetail.Barcode == data.SearchObj.BarCode);
            }
            if (!string.IsNullOrEmpty(data.SearchObj.SerialNumber))
            {
                query = query.Where(x => x.AssetDetail.SerialNumber.Contains(data.SearchObj.SerialNumber));
            }
            if (data.SearchObj.Model != "")
            {
                query = query.Where(x => x.AssetDetail.MasterAsset.ModelNumber == data.SearchObj.Model);
            }
            if (data.SearchObj.DepartmentId != 0)
            {
                query = query.Where(x => x.AssetDetail.DepartmentId == data.SearchObj.DepartmentId);
            }
            if (data.SearchObj.BrandId != 0)
            {
                query = query.Where(x => x.AssetDetail.MasterAsset.BrandId == data.SearchObj.BrandId);
            }
            if (data.SearchObj.SupplierId != 0)
            {
                query = query.Where(x => x.MasterContract.SupplierId == data.SearchObj.SupplierId);
            }
            if (data.SearchObj.OriginId != 0)
            {
                query = query.Where(x => x.AssetDetail.MasterAsset.OriginId == data.SearchObj.OriginId);
            }
            if (data.SearchObj.Subject != "")
            {
                query = query.Where(x => x.MasterContract.Subject == data.SearchObj.Subject);
            }
            if (data.SearchObj.ContractNumber != "")
            {
                query = query.Where(x => x.MasterContract.Serial == data.SearchObj.ContractNumber);
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
                query = query.Where(a => a.MasterContract.ContractDate >= startingFrom.Date && a.MasterContract.ContractDate <= endingTo.Date);
            }




            if (data.SearchObj.SelectedContractType == 1)
            {
                query = query.Where(b => b.MasterContract.To != null);
                query = query.Where(b => b.MasterContract.To.Value.Date >= DateTime.Today.Date);
            }
            if (data.SearchObj.SelectedContractType == 2)
            {
                query = query.Where(b => b.MasterContract.To != null);
                query = query.Where(b => b.MasterContract.To.Value.Date <= DateTime.Today.Date);
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
                case "Model":
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
                case "Subject":
                case "الموضوع":
                    if (data.SortObj.SortStatus == "ascending")
                    {
                        query = query.OrderBy(x => x.MasterContract.Subject);
                    }
                    else
                    {
                        query = query.OrderByDescending(x => x.MasterContract.Subject);
                    }
                    break;

                case "Contract Date":
                case "تاريخ العقد":
                    if (data.SortObj.SortStatus == "ascending")
                    {
                        query = query.OrderBy(x => x.MasterContract.ContractDate);
                    }
                    else
                    {
                        query = query.OrderByDescending(x => x.MasterContract.ContractDate);
                    }
                    break;


                case "Start Date":
                case "تاريخ البداية":
                    if (data.SortObj.SortStatus == "ascending")
                    {
                        query = query.OrderBy(x => x.MasterContract.ContractDate);
                    }
                    else
                    {
                        query = query.OrderByDescending(x => x.MasterContract.ContractDate);
                    }
                    break;



                case "End Date":
                case "تاريخ النهاية":
                    if (data.SortObj.SortStatus == "ascending")
                    {
                        query = query.OrderBy(x => x.MasterContract.ContractDate);
                    }
                    else
                    {
                        query = query.OrderByDescending(x => x.MasterContract.ContractDate);
                    }
                    break;
                case "Contract Number":
                case "رقم العقد":
                    if (data.SortObj.SortStatus == "ascending")
                    {
                        query = query.OrderBy(x => x.MasterContract.Serial);
                    }
                    else
                    {
                        query = query.OrderByDescending(x => x.MasterContract.Serial);
                    }
                    break; ;



                    //case "Asset Name":
                    //    if (data.SortObj.SortStatus == "ascending")
                    //    {
                    //        query.OrderBy(x => x.AssetDetail.MasterAsset.Name);
                    //    }
                    //    else
                    //    {
                    //        query.OrderByDescending(x => x.AssetDetail.MasterAsset.Name);
                    //    }
                    //    break;
                    //case "اسم الأصل":
                    //    if (data.SortObj.SortStatus == "ascending")
                    //    {
                    //        query = query.OrderBy(x => x.AssetDetail.MasterAsset.NameAr);
                    //    }
                    //    else
                    //    {
                    //        query = query.OrderByDescending(x => x.AssetDetail.MasterAsset.NameAr);
                    //    }
                    //    break;
                    //case "Hospital":
                    //    if (data.SortObj.SortStatus == "ascending")
                    //    {
                    //        query = query.OrderBy(x => x.Hospital.Name);
                    //    }
                    //    else
                    //    {
                    //        query = query.OrderByDescending(x => x.Hospital.Name);
                    //    }
                    //    break;
                    //case "المستشفى":
                    //    if (data.SortObj.SortStatus == "ascending")
                    //    {
                    //        query = query.OrderBy(x => x.Hospital.NameAr);
                    //    }
                    //    else
                    //    {
                    //        query = query.OrderByDescending(x => x.Hospital.NameAr);
                    //    }
                    //    break;
                    //case "Brands":
                    //    if (data.SortObj.SortStatus == "ascending")
                    //    {
                    //        query = query.OrderBy(x => x.AssetDetail.MasterAsset.brand.Name);
                    //    }
                    //    else
                    //    {
                    //        query = query.OrderByDescending(x => x.AssetDetail.MasterAsset.brand.Name);
                    //    }
                    //    break;
                    //case "الماركات":
                    //    if (data.SortObj.SortStatus == "ascending")
                    //    {
                    //        query = query.OrderBy(x => x.AssetDetail.MasterAsset.brand.NameAr);
                    //    }
                    //    else
                    //    {
                    //        query = query.OrderByDescending(x => x.AssetDetail.MasterAsset.brand.NameAr);
                    //    }
                    //    break;
                    //case "Department":
                    //    if (data.SortObj.SortStatus == "ascending")
                    //    {
                    //        query = query.OrderBy(x => x.AssetDetail.Department.Name);
                    //    }
                    //    else
                    //    {
                    //        query = query.OrderByDescending(x => x.AssetDetail.Department.Name);
                    //    }
                    //    break;
                    //case "القسم":
                    //    if (data.SortObj.SortStatus == "ascending")
                    //    {
                    //        query = query.OrderBy(x => x.AssetDetail.Department.NameAr);
                    //    }
                    //    else
                    //    {
                    //        query = query.OrderByDescending(x => x.AssetDetail.Department.NameAr);
                    //    }
                    //    break;
                    //case "Governorate":
                    //    if (data.SortObj.SortStatus == "ascending")
                    //    {
                    //        query = query.OrderBy(x => x.Hospital.Governorate.Name);
                    //    }
                    //    else
                    //    {
                    //        query = query.OrderByDescending(x => x.Hospital.Governorate.Name);
                    //    }
                    //    break;
                    //case "المحافظة":
                    //    if (data.SortObj.SortStatus == "ascending")
                    //    {
                    //        query = query.OrderBy(x => x.Hospital.Governorate.NameAr);
                    //    }
                    //    else
                    //    {
                    //        query = query.OrderByDescending(x => x.Hospital.Governorate.NameAr);
                    //    }
                    //    break;
                    //case "City":
                    //    if (data.SortObj.SortStatus == "ascending")
                    //    {
                    //        query = query.OrderBy(x => x.Hospital.City.Name);
                    //    }
                    //    else
                    //    {
                    //        query = query.OrderByDescending(x => x.Hospital.City.Name);
                    //    }
                    //    break;
                    //case "المدينة":
                    //    if (data.SortObj.SortStatus == "ascending")
                    //    {
                    //        query = query.OrderBy(x => x.Hospital.City.NameAr);
                    //    }
                    //    else
                    //    {
                    //        query = query.OrderByDescending(x => x.Hospital.City.NameAr);
                    //    }
                    //    break;
                    //case "Organization":
                    //    if (data.SortObj.SortStatus == "ascending")
                    //    {
                    //        query = query.OrderBy(x => x.Hospital.Organization.Name);
                    //    }
                    //    else
                    //    {
                    //        query = query.OrderByDescending(x => x.Hospital.Organization.Name);
                    //    }
                    //    break;

                    //case "الهيئة":
                    //    if (data.SortObj.SortStatus == "ascending")
                    //    {
                    //        query = query.OrderBy(x => x.Hospital.Organization.NameAr);
                    //    }
                    //    else
                    //    {
                    //        query = query.OrderByDescending(x => x.Hospital.Organization.NameAr);
                    //    }
                    //    break;
                    //case "SubOrganization":
                    //    if (data.SortObj.SortStatus == "ascending")
                    //    {
                    //        query = query.OrderBy(x => x.Hospital.SubOrganization.Name);
                    //    }
                    //    else
                    //    {
                    //        query = query.OrderByDescending(x => x.Hospital.SubOrganization.Name);
                    //    }
                    //    break;
                    //case "هيئة فرعية":
                    //    if (data.SortObj.SortStatus == "ascending")
                    //    {
                    //        query = query.OrderBy(x => x.Hospital.SubOrganization.NameAr);
                    //    }
                    //    else
                    //    {
                    //        query = query.OrderByDescending(x => x.Hospital.SubOrganization.NameAr);
                    //    }
                    //    break;
            }

            #endregion

            #region Count data and fiter By Paging
            mainClass.Count = query.Count();
            var lstGroupResults = query.ToList().GroupBy(a => a.MasterContractId).ToList();
            var lstResults = lstGroupResults.Skip((pageNumber - 1) * pageSize).Take(pageSize);
            #endregion

            #region Loop to get Items after serach and sort

            foreach (var item in lstResults.ToList())
            {
                IndexMasterContractVM.GetData getDataObj = new IndexMasterContractVM.GetData();
                getDataObj.Id = item.FirstOrDefault().MasterContract.Id;
                getDataObj.Subject = item.FirstOrDefault().MasterContract.Subject;
                getDataObj.Cost = item.FirstOrDefault().MasterContract.Cost.ToString();
                getDataObj.Notes = item.FirstOrDefault().MasterContract.Notes;
                getDataObj.ContractNumber = item.FirstOrDefault().MasterContract.Serial;
                getDataObj.ContractDate = item.FirstOrDefault().MasterContract.ContractDate;
                getDataObj.StartDate = item.FirstOrDefault().MasterContract.From;
                getDataObj.EndDate = item.FirstOrDefault().MasterContract.To;
                getDataObj.HospitalId = item.FirstOrDefault().MasterContract.HospitalId;
                getDataObj.SupplierName = item.FirstOrDefault().MasterContract.Supplier != null ? item.FirstOrDefault().MasterContract.Supplier.Name : "";
                getDataObj.SupplierNameAr = item.FirstOrDefault().MasterContract.Supplier != null ? item.FirstOrDefault().MasterContract.Supplier.NameAr : "";
                list.Add(getDataObj);
            }
            #endregion

            #region Represent data after Paging and count
            mainClass.Results = list;
            #endregion




            return mainClass;
        }



        //public IndexMasterContractVM GetMasterContractsByHospitalId(int hospitalId, int pageNumber, int pageSize)
        //{
        //    IndexMasterContractVM mainClass = new IndexMasterContractVM();
        //    List<IndexMasterContractVM.GetData> list = new List<IndexMasterContractVM.GetData>();
        //    var lstMasters = _context.ContractDetails
        //                 .Include(a => a.MasterContract)
        //                   .Include(a => a.MasterContract.Supplier)
        //                 .Include(a => a.AssetDetail)
        //                    .Include(a => a.AssetDetail.Hospital)
        //                 .Include(a => a.MasterContract.Supplier).Select(item => new IndexMasterContractVM.GetData
        //                 {
        //                     Id = item.MasterContract.Id,
        //                     SupplierId = item.MasterContract.SupplierId,
        //                     HospitalId = item.HospitalId,
        //                     Subject = item.MasterContract.Subject,
        //                     Cost = item.MasterContract.Cost.ToString(),
        //                     ContractNumber = item.MasterContract.Serial,
        //                     ContractDate = item.MasterContract.ContractDate.Value,
        //                     StartDate = item.MasterContract.From.Value,
        //                     EndDate = item.MasterContract.To.Value,

        //                     SupplierName = item.MasterContract.Supplier != null?item.MasterContract.Supplier.Name:"",
        //                     SupplierNameAr = item.MasterContract.Supplier != null ? item.MasterContract.Supplier.NameAr:"",
        //                     Notes = item.MasterContract.Notes

        //                 }).ToList().GroupBy(a => a.Id);


        //    foreach (var item in lstMasters)
        //    {
        //        IndexMasterContractVM.GetData getDataObj = new IndexMasterContractVM.GetData();
        //        getDataObj.Id = item.FirstOrDefault().Id;
        //        getDataObj.Subject = item.FirstOrDefault().Subject;
        //        getDataObj.Cost = item.FirstOrDefault().Cost;
        //        getDataObj.Notes = item.FirstOrDefault().Notes;
        //        getDataObj.ContractNumber = item.FirstOrDefault().ContractNumber;
        //        getDataObj.ContractDate = item.FirstOrDefault().ContractDate;
        //        getDataObj.StartDate = item.FirstOrDefault().StartDate;
        //        getDataObj.EndDate = item.FirstOrDefault().EndDate;
        //        getDataObj.HospitalId = item.FirstOrDefault().HospitalId;
        //        getDataObj.SupplierName = item.FirstOrDefault().SupplierId != null ? item.FirstOrDefault().SupplierName : "";
        //        getDataObj.SupplierNameAr = item.FirstOrDefault().SupplierId != null ? item.FirstOrDefault().SupplierNameAr : "";
        //        list.Add(getDataObj);
        //    }

        //    if (hospitalId > 0)
        //    {
        //        list = list.Where(a => a.HospitalId == hospitalId).ToList();
        //    }
        //    else
        //    {
        //        list = list.ToList();
        //    }


        //    var contractPerPage = list.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
        //    mainClass.Results = contractPerPage;
        //    mainClass.Count = list.Count();
        //    return mainClass;

        //}


















        //public IndexMasterContractVM Search(SearchContractVM searchObj, int pageNumber, int pageSize)
        //{

        //    IndexMasterContractVM mainClass = new IndexMasterContractVM();
        //    List<IndexMasterContractVM.GetData> lstData = new List<IndexMasterContractVM.GetData>();

        //    string setstartcontractday, setstartcontractmonth, setendcontractday, setendcontractmonth = "";
        //    // List<IndexMasterContractVM.GetData> lstData = new List<IndexMasterContractVM.GetData>();

        //    var lstContracts = _context.ContractDetails
        //                     .Include(a => a.MasterContract)
        //                     .Include(a => a.AssetDetail)
        //                     .Include(a => a.MasterContract.Supplier)
        //                     .Select(item => new
        //                     {
        //                         Id = item.MasterContract.Id,
        //                         Subject = item.MasterContract.Subject,
        //                         Notes = item.MasterContract.Notes,
        //                         Cost = item.MasterContract.Cost,
        //                         ContractNumber = item.MasterContract.Serial,
        //                         ContractDate = item.MasterContract.ContractDate,
        //                         StartDate = item.MasterContract.From,
        //                         EndDate = item.MasterContract.To,
        //                         SupplierId = item.MasterContract.SupplierId,
        //                         SupplierName = item.MasterContract.Supplier.Name,
        //                         SupplierNameAr = item.MasterContract.Supplier.NameAr,
        //                         HospitalId = item.AssetDetail.HospitalId
        //                     }).ToList().GroupBy(a => a.Id).ToList();



        //    foreach (var cntrct in lstContracts)
        //    {
        //        IndexMasterContractVM.GetData getDataObj = new IndexMasterContractVM.GetData();
        //        getDataObj.Id = cntrct.FirstOrDefault().Id;
        //        getDataObj.Subject = cntrct.FirstOrDefault().Subject;
        //        getDataObj.Notes = cntrct.FirstOrDefault().Notes;
        //        getDataObj.Cost = cntrct.FirstOrDefault().Cost.ToString();
        //        getDataObj.ContractNumber = cntrct.FirstOrDefault().ContractNumber;
        //        getDataObj.ContractDate = cntrct.FirstOrDefault().ContractDate.Value;
        //        getDataObj.StartDate = cntrct.FirstOrDefault().StartDate;
        //        getDataObj.EndDate = cntrct.FirstOrDefault().EndDate;
        //        getDataObj.SupplierName = cntrct.FirstOrDefault().SupplierName;
        //        getDataObj.SupplierNameAr = cntrct.FirstOrDefault().SupplierNameAr;
        //        lstData.Add(getDataObj);
        //    }


        //    if (searchObj.SelectedContractType == 1)
        //    {
        //        lstData = lstData.Where(b => b.EndDate != null).ToList();
        //        lstData = lstData.Where(b => b.EndDate.Value.Date >= DateTime.Today.Date).ToList();
        //    }
        //    if (searchObj.SelectedContractType == 2)
        //    {
        //        lstData = lstData.Where(b => b.EndDate != null).ToList();
        //        lstData = lstData.Where(b => b.EndDate.Value.Date <= DateTime.Today.Date).ToList();
        //    }




        //    //if (searchObj.HospitalId != 0)
        //    //{
        //    //    lstData = lstData.Where(b => b.HospitalId == searchObj.HospitalId).ToList();
        //    //}
        //    if (searchObj.Subject != "")
        //    {
        //        lstData = lstData.Where(b => b.Subject == searchObj.Subject).ToList();
        //    }
        //    if (searchObj.ContractNumber != "")
        //    {
        //        lstData = lstData.Where(b => b.ContractNumber == searchObj.ContractNumber).ToList();
        //    }




        //    if (searchObj.StartDate == null)
        //    {
        //        //   searchObj.StartDate = DateTime.Parse("01/01/1900");
        //    }
        //    else
        //    {
        //        //   searchObj.StartDate = searchObj.StartDate;

        //        var startyear = searchObj.StartDate.Value.Year;
        //        var startmonth = searchObj.StartDate.Value.Month;
        //        var startday = searchObj.StartDate.Value.Day;
        //        if (startday < 10)
        //            setstartcontractday = searchObj.StartDate.Value.Day.ToString().PadLeft(2, '0');
        //        else
        //            setstartcontractday = searchObj.StartDate.Value.Day.ToString();

        //        if (startmonth < 10)
        //            setstartcontractmonth = searchObj.StartDate.Value.Month.ToString().PadLeft(2, '0');
        //        else
        //            setstartcontractmonth = searchObj.StartDate.Value.Month.ToString();

        //        var sDate = startyear + "-" + setstartcontractmonth + "-" + setstartcontractday;
        //        var startingFrom = DateTime.Parse(sDate);
        //    }

        //    if (searchObj.EndDate == null)
        //    {
        //        //  searchObj.EndDate = DateTime.Today.Date;
        //    }
        //    else
        //    {
        //        //  searchObj.EndDate = searchObj.EndDate;


        //        var endyear = searchObj.EndDate.Value.Year;
        //        var endmonth = searchObj.EndDate.Value.Month;
        //        var endday = searchObj.EndDate.Value.Day;
        //        if (endday < 10)
        //            setendcontractday = searchObj.EndDate.Value.Day.ToString().PadLeft(2, '0');
        //        else
        //            setendcontractday = searchObj.EndDate.Value.Day.ToString();

        //        if (endmonth < 10)
        //            setendcontractmonth = searchObj.EndDate.Value.Month.ToString().PadLeft(2, '0');
        //        else
        //            setendcontractmonth = searchObj.EndDate.Value.Month.ToString();

        //        var eDate = endyear + "-" + setendcontractmonth + "-" + setendcontractday;
        //        var endingTo = DateTime.Parse(eDate);
        //    }







        //    // lstData = lstData.Where(a => a.StartDate >= startingFrom && a.EndDate <= endingTo).ToList();

        //    if (searchObj.StartDate != null && searchObj.EndDate != null)
        //        lstData = lstData.Where(a => a.StartDate.Value.Date >= searchObj.StartDate.Value.Date && a.EndDate.Value.Date <= searchObj.EndDate.Value.Date).ToList();






        //    if (searchObj.ContractDate != null)
        //    {
        //        string setcontractday, setcontractmonth = "";
        //        var year = searchObj.ContractDate.Value.Year;
        //        var month = searchObj.ContractDate.Value.Month;
        //        var day = searchObj.ContractDate.Value.Day;
        //        if (day < 10)
        //            setcontractday = searchObj.ContractDate.Value.Day.ToString().PadLeft(2, '0');
        //        else
        //            setcontractday = searchObj.ContractDate.Value.Day.ToString();

        //        if (month < 10)
        //            setcontractmonth = searchObj.ContractDate.Value.Month.ToString().PadLeft(2, '0');
        //        else
        //            setcontractmonth = searchObj.ContractDate.Value.Month.ToString();

        //        var contrctDate = year + "-" + setcontractmonth + "-" + setcontractday;
        //        var conDate = DateTime.Parse(contrctDate);


        //        lstData = lstData.Where(a => a.ContractDate >= conDate && a.ContractDate <= conDate).ToList();
        //    }


        //    var contractPerPage = lstData.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
        //    mainClass.Results = contractPerPage;
        //    mainClass.Count = lstData.Count();
        //    return mainClass;
        //}



        public int CreateContractAttachments(ContractAttachment attachObj)
        {
            ContractAttachment documentObj = new ContractAttachment();
            documentObj.DocumentName = attachObj.DocumentName;
            documentObj.FileName = attachObj.FileName;
            documentObj.MasterContractId = attachObj.MasterContractId;
            documentObj.MasterContractId = attachObj.MasterContractId;
            documentObj.HospitalId = attachObj.HospitalId;
            _context.ContractAttachments.Add(documentObj);
            _context.SaveChanges();
            return attachObj.Id;
        }

        public GeneratedMasterContractNumberVM GenerateMasterContractSerial()
        {
            GeneratedMasterContractNumberVM numberObj = new GeneratedMasterContractNumberVM();
            string strContract = "Cntrct";

            var lstContracts = _context.MasterContracts.ToList();
            if (lstContracts.Count > 0)
            {
                var code = lstContracts.LastOrDefault().Id;
                numberObj.ContractSerial = strContract + (code + 1);
            }
            else
            {
                numberObj.ContractSerial = strContract + 1;
            }

            return numberObj;
        }

        public IEnumerable<ContractAttachment> GetContractAttachmentByMasterContractId(int masterContractId)
        {
            return _context.ContractAttachments.Where(a => a.MasterContractId == masterContractId).ToList();
        }




        public IndexMasterContractVM GetListBoxMasterContractsByHospitalId(int hospitalId)
        {
            IndexMasterContractVM mainClass = new IndexMasterContractVM();
            List<IndexMasterContractVM.GetData> list = new List<IndexMasterContractVM.GetData>();
            var lstMasters = _context.ContractDetails
                         .Include(a => a.MasterContract)
                         .Include(a => a.AssetDetail)
                            .Include(a => a.AssetDetail.Hospital)
                         .Include(a => a.MasterContract.Supplier).Select(item => new IndexMasterContractVM.GetData
                         {
                             Id = item.MasterContract.Id,
                             SupplierId = item.MasterContract.SupplierId,
                             HospitalId = item.HospitalId,
                             Subject = item.MasterContract.Subject,
                             Cost = item.MasterContract.Cost.ToString(),
                             ContractNumber = item.MasterContract.Serial,
                             ContractDate = item.MasterContract.ContractDate.Value,
                             StartDate = item.MasterContract.From.Value,
                             EndDate = item.MasterContract.To.Value,
                             SupplierName = item.MasterContract.Supplier != null ? item.MasterContract.Supplier.Name : "",
                             SupplierNameAr = item.MasterContract.Supplier != null ? item.MasterContract.Supplier.NameAr : "",
                             Notes = item.MasterContract.Notes
                         }).ToList().GroupBy(a => a.Id);

            foreach (var item in lstMasters)
            {
                IndexMasterContractVM.GetData getDataObj = new IndexMasterContractVM.GetData();
                getDataObj.Id = item.FirstOrDefault().Id;
                getDataObj.Subject = item.FirstOrDefault().Subject;
                getDataObj.Cost = item.FirstOrDefault().Cost;
                getDataObj.Notes = item.FirstOrDefault().Notes;
                getDataObj.ContractNumber = item.FirstOrDefault().ContractNumber;
                getDataObj.ContractDate = item.FirstOrDefault().ContractDate;
                getDataObj.StartDate = item.FirstOrDefault().StartDate;
                getDataObj.EndDate = item.FirstOrDefault().EndDate;
                getDataObj.HospitalId = item.FirstOrDefault().HospitalId;
                getDataObj.SupplierName = item.FirstOrDefault().SupplierId != null ? item.FirstOrDefault().SupplierName : "";
                getDataObj.SupplierNameAr = item.FirstOrDefault().SupplierId != null ? item.FirstOrDefault().SupplierNameAr : "";
                list.Add(getDataObj);
            }

            if (hospitalId > 0)
            {
                list = list.Where(a => a.HospitalId == hospitalId).ToList();
            }
            else
            {
                list = list.ToList();
            }
            var contractPerPage = list.ToList();
            mainClass.Results = contractPerPage;
            mainClass.Count = list.Count();
            return mainClass;
        }

        public ContractAttachment GetLastDocumentForMasterContractId(int masterContractId)
        {
            ContractAttachment documentObj = new ContractAttachment();
            var lstDocuments = _context.ContractAttachments.Where(a => a.MasterContractId == masterContractId).ToList();
            if (lstDocuments.Count > 0)
            {
                documentObj = lstDocuments.Last();
            }
            return documentObj;
        }

        public int DeleteContractAttachment(int attachId)
        {
            var contractAttachObj = _context.ContractAttachments.Find(attachId);
            try
            {
                if (contractAttachObj != null)
                {
                    _context.ContractAttachments.Remove(contractAttachObj);
                    return _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }
            return 0;
        }



        #region Refactor Functions


        public IndexMasterContractVM AlertContractsEndBefore3Months(int hospitalId, int duration, int pageNumber, int pageSize)
        {
            int durationDays = 0;
            if (duration == 1)
                durationDays = 70;
            if (duration == 2)
                durationDays = 80;
            if (duration == 3)
                durationDays = 90;

            List<MasterContract> allContracts = _context.MasterContracts.Include(a => a.Supplier).ToList();
            IndexMasterContractVM mainClass = new IndexMasterContractVM();
            List<IndexMasterContractVM.GetData> lstContracts = new List<IndexMasterContractVM.GetData>();
            if (hospitalId != 0)
                allContracts = allContracts.Where(a => a.HospitalId == hospitalId).ToList();
            else
                allContracts = allContracts.ToList();


            if (allContracts.Count > 0)
            {
                foreach (var itm in allContracts)
                {

                    if (itm.To.HasValue && itm.To != null && itm.To != DateTime.Parse("01/01/1900"))
                    {
                        IndexMasterContractVM.GetData item = new IndexMasterContractVM.GetData();
                        item.Id = itm.Id;
                        item.ContractNumber = itm.Serial;
                        item.ContractDate = itm.ContractDate;
                        item.Subject = itm.Subject;
                        item.EndDate = itm.To;
                        item.SupplierName = itm.Supplier != null ? itm.Supplier.Name : "";
                        item.SupplierNameAr = itm.Supplier != null ? itm.Supplier.NameAr : "";
                        item.Cost = itm.Cost.ToString();
                        var isPassed = CheckIfDateHasPasseddurationDays(itm.To.Value,durationDays);
                        if (isPassed)
                        {
                            lstContracts.Add(item);
                        }
                    }
                }
            }
            lstContracts.RemoveAll(s => s.EndDate == null);
            mainClass.Count = lstContracts.Count;
            var contractPerPage = lstContracts.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            mainClass.Results = contractPerPage;
            return mainClass;
        }


        static bool CheckIfDateHasPasseddurationDays(DateTime targetDate,int durationDays)
        {
            DateTime currentDate = DateTime.Now;
            DateTime daysAgo = currentDate.AddDays(-durationDays);
            return targetDate <= daysAgo;
        }


        #endregion


    }
}
