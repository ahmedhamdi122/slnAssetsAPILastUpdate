using Asset.Domain.Repositories;
using Asset.Models;
using Asset.ViewModels.AssetDetailVM;
using Asset.ViewModels.MasterAssetAttachmentVM;
using Asset.ViewModels.MasterAssetVM;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset.Core.Repositories
{
    public class MasterAssetRepositories : IMasterAssetRepository
    {

        private ApplicationDbContext _context;
        public MasterAssetRepositories(ApplicationDbContext context)
        {
            _context = context;
        }
        public bool ExistsByNameModelAndVersion(string Name, string ModelNumber, string VersionNumber)
        {
            return _context.MasterAssets.Any(asset => asset.Name == Name && asset.ModelNumber == ModelNumber && asset.VersionNumber == VersionNumber);
        }
        public bool ExistsByNameArModelAndVersion(string NameAr, string ModelNumber, string VersionNumber)
        {
            return _context.MasterAssets.Any(asset => asset.NameAr == NameAr && asset.ModelNumber == ModelNumber && asset.VersionNumber == VersionNumber);
        }
        public bool isMasterAssetExistsUsingId(int id)
        {
            return _context.MasterAssets.Any(x => x.Id == id);
        }
        public int CountMasterAssets()
        {
            return _context.MasterAssets.Count();
        }
        public IQueryable<MasterAsset> RetrieveQueryableMasterAssets()
        {
            return _context.MasterAssets;
        }
        public List<CountMasterAssetBrands> CountMasterAssetsByBrand(int hospitalId)
        {
            List<CountMasterAssetBrands> list = new List<CountMasterAssetBrands>();

            if (hospitalId != 0)
            {
                //var countsByGroup = _context.AssetDetails.Include(a => a.MasterAsset).Include(a => a.MasterAsset.brand)
                //                 .Where(a => a.HospitalId == hospitalId).ToList()
                //                 .GroupBy(item => item.MasterAssetId)
                //                 .ToDictionary(group => group.Key, group => group.Count()).OrderByDescending(pair => pair.Value).Take(10)
                //                 .ToList();


                var lstBrands = _context.AssetDetails.Include(a => a.MasterAsset).Include(a => a.MasterAsset.brand)
                                           .Where(a => a.MasterAsset.BrandId != null && a.HospitalId == hospitalId).ToList()
                                           .GroupBy(item => item.MasterAsset.BrandId)
                                           .ToDictionary(group => group.Key, group => new CountMasterAssetBrands
                                           {
                                               Key = int.Parse(group.Key.ToString()),
                                               Value = group.Count(),
                                               BrandName = group.FirstOrDefault().MasterAsset.brand.Name,
                                               BrandNameAr = group.FirstOrDefault().MasterAsset.brand.NameAr,
                                               CountOfMasterAssets = group.Count()
                                           }).OrderByDescending(pair => pair.Value.Value).Take(10).ToList();
                foreach (var item in lstBrands)
                {
                    CountMasterAssetBrands countHospitalObj = new CountMasterAssetBrands();
                    countHospitalObj.BrandName = item.Value.BrandName;
                    countHospitalObj.BrandNameAr = item.Value.BrandNameAr;
                    countHospitalObj.CountOfMasterAssets = item.Value.Value;
                    list.Add(countHospitalObj);
                }
            }
            else
            {
                var lstBrands = _context.AssetDetails.Include(a => a.MasterAsset).Include(a => a.MasterAsset.brand).ToList()
                                          .Where(a => a.MasterAsset.BrandId != null)
                                          .GroupBy(item => item.MasterAssetId)
                                          .ToDictionary(group => group.Key, group => new CountMasterAssetBrands
                                          {
                                              Key = group.Key,
                                              Value = group.Count(),
                                              BrandName = group.FirstOrDefault().MasterAsset.brand.Name,
                                              BrandNameAr = group.FirstOrDefault().MasterAsset.brand.NameAr,
                                              CountOfMasterAssets = group.Count()
                                          }).OrderByDescending(pair => pair.Value.Value).Take(10).ToList();


                foreach (var item in lstBrands)
                {
                    CountMasterAssetBrands countHospitalObj = new CountMasterAssetBrands();
                    countHospitalObj.BrandName = item.Value.BrandName;
                    countHospitalObj.BrandNameAr = item.Value.BrandNameAr;
                    countHospitalObj.CountOfMasterAssets = item.Value.Value;
                    list.Add(countHospitalObj);
                }
            }

            return list;


            //if (hospitalId != 0)
            //{
            //    var lstBrands = _context.AssetDetails.Include(a => a.MasterAsset).Include(a => a.MasterAsset.brand)
            //            .Where(a => a.HospitalId == hospitalId)
            //            .OrderBy(a => a.MasterAsset.brand).Take(10).ToList();
            //    foreach (var item in lstBrands)
            //    {
            //        CountMasterAssetBrands countHospitalObj = new CountMasterAssetBrands();
            //        countHospitalObj.BrandName = item.MasterAsset.brand != null ? item.MasterAsset.brand.Name : "";
            //        countHospitalObj.BrandNameAr = item.MasterAsset.brand != null ? item.MasterAsset.brand.NameAr : "";
            //        countHospitalObj.CountOfMasterAssets = _context.AssetDetails.Include(a => a.MasterAsset)
            //            .Where(a => a.MasterAsset.BrandId == item.MasterAsset.BrandId && a.HospitalId == hospitalId).ToList().Count();
            //        list.Add(countHospitalObj);
            //    }
            //}
            //else
            //{
            //    var lstBrands = _context.Brands.ToList().Take(10);
            //    foreach (var item in lstBrands)
            //    {
            //        CountMasterAssetBrands countHospitalObj = new CountMasterAssetBrands();
            //        countHospitalObj.BrandName = item.Name;
            //        countHospitalObj.BrandNameAr = item.NameAr;
            //        countHospitalObj.CountOfMasterAssets = _context.MasterAssets.Where(a => a.BrandId == item.Id).ToList().Count();
            //        list.Add(countHospitalObj);
            //    }
            //}
            //return list.OrderBy(a => a.CountOfMasterAssets).ToList();
        }

        public List<CountMasterAssetSuppliers> CountMasterAssetsBySupplier(int hospitalId)
        {
            List<CountMasterAssetSuppliers> list = new List<CountMasterAssetSuppliers>();
            var lstSuppliers = _context.Suppliers.ToList().Take(10);
            if (hospitalId != 0)
            {
                foreach (var item in lstSuppliers)
                {
                    CountMasterAssetSuppliers countHospitalObj = new CountMasterAssetSuppliers();
                    countHospitalObj.SupplierName = item.Name;
                    countHospitalObj.SupplierNameAr = item.NameAr;
                    countHospitalObj.CountOfMasterAssets = _context.AssetDetails.Where(a => a.SupplierId == item.Id && a.HospitalId == hospitalId).ToList().GroupBy(a => a.SupplierId).Count();
                    list.Add(countHospitalObj);
                }
            }
            else
            {
                foreach (var item in lstSuppliers)
                {
                    CountMasterAssetSuppliers countHospitalObj = new CountMasterAssetSuppliers();
                    countHospitalObj.SupplierName = item.Name;
                    countHospitalObj.SupplierNameAr = item.NameAr;
                    countHospitalObj.CountOfMasterAssets = _context.AssetDetails.Where(a => a.SupplierId == item.Id).ToList().GroupBy(a => a.SupplierId).Count();
                    list.Add(countHospitalObj);
                }
            }
            return list;

        }

        public IEnumerable<MasterAsset> GetAllMasterAssets()
        {
            return _context.MasterAssets.ToList();
        }

        public IEnumerable<MasterAsset> GetAllMasterAssetsByHospitalId(int hospitalId, string userId)
        {
            List<MasterAsset> list = new List<MasterAsset>();
            ApplicationUser UserObj = new ApplicationUser();
            int employeeId = 0;

            var obj = _context.ApplicationUser.Where(a => a.Id == userId).ToList();
            if (obj.Count > 0)
            {
                UserObj = obj[0];
                var lstEmployees = _context.Employees.Where(a => a.Email == UserObj.Email).ToList();
                if (lstEmployees.Count > 0)
                {
                    var employeeObj = lstEmployees[0];
                    employeeId = employeeObj.Id;
                }
            }
            var lstMasterAssets = (from asset in _context.MasterAssets
                                   join detail in _context.AssetDetails on asset.Id equals detail.MasterAssetId
                                   //  join owner in _context.AssetOwners on detail.Id equals owner.AssetDetailId
                                   where detail.HospitalId == hospitalId
                                   // where owner.EmployeeId == employeeId
                                   select asset).ToList().GroupBy(a => a.Id).ToList();

            foreach (var item in lstMasterAssets)
            {
                MasterAsset masterAssetObj = new MasterAsset();
                masterAssetObj.Id = item.FirstOrDefault().Id;
                masterAssetObj.Name = item.FirstOrDefault().Name;
                masterAssetObj.NameAr = item.FirstOrDefault().NameAr;
                list.Add(masterAssetObj);
            }
            return list;
        }

        public IEnumerable<MasterAsset> GetAllMasterAssetsByHospitalId(int hospitalId)
        {
            List<MasterAsset> list = new List<MasterAsset>();
            var lstMasterAssets = _context.AssetDetails.Include(a => a.MasterAsset)
                                   .Where(a => a.HospitalId == hospitalId).ToList().GroupBy(a => a.MasterAsset.Id).ToList();


            foreach (var item in lstMasterAssets)
            {
                MasterAsset masterAssetObj = new MasterAsset();
                masterAssetObj.Id = item.FirstOrDefault().MasterAsset.Id;
                masterAssetObj.Name = item.FirstOrDefault().MasterAsset.Name;
                masterAssetObj.NameAr = item.FirstOrDefault().MasterAsset.NameAr;
                list.Add(masterAssetObj);
            }
            return list;
        }

        public ViewMasterAssetVM ViewMasterAsset(int id)
        {
            ViewMasterAssetVM model = new ViewMasterAssetVM();
            var masterAssetObj = _context.MasterAssets.Find(id);
            model.Id = masterAssetObj.Id;
            model.Code = masterAssetObj.Code;
            model.Name = masterAssetObj.Name;
            model.NameAr = masterAssetObj.NameAr;
            model.Description = masterAssetObj.Description;
            model.DescriptionAr = masterAssetObj.DescriptionAr;
            model.ExpectedLifeTime = masterAssetObj.ExpectedLifeTime != null ? (int)masterAssetObj.ExpectedLifeTime : 0;
            model.Height = masterAssetObj.Height;
            model.Length = masterAssetObj.Length;
            model.ModelNumber = masterAssetObj.ModelNumber;
            model.VersionNumber = masterAssetObj.VersionNumber;
            model.Weight = masterAssetObj.Weight;
            model.Width = masterAssetObj.Width;

            model.Power = masterAssetObj.Power;
            model.Voltage = masterAssetObj.Voltage;
            model.Ampair = masterAssetObj.Ampair;
            model.Frequency = masterAssetObj.Frequency;
            model.ElectricRequirement = masterAssetObj.ElectricRequirement;
            //  model.PMTimeId = masterAssetObj.PMTimeId;
            model.AssetImg = masterAssetObj.AssetImg;
            var lstECRIs = _context.ECRIS.Where(a => a.Id == masterAssetObj.ECRIId).ToList();
            if (lstECRIs.Count > 0)
            {
                model.ECRIId = lstECRIs[0].Id;
                model.ECRIName = lstECRIs[0].Name;
                model.ECRINameAr = lstECRIs[0].NameAr;
            }
            var lstBrands = _context.Brands.Where(a => a.Id == masterAssetObj.BrandId).ToList();
            if (lstBrands.Count > 0)
            {
                model.BrandId = lstBrands[0].Id;
                model.BrandName = lstBrands[0].Name;
                model.BrandNameAr = lstBrands[0].NameAr;
            }

            var lstPeriorities = _context.AssetPeriorities.Where(a => a.Id == masterAssetObj.PeriorityId).ToList();
            if (lstPeriorities.Count > 0)
            {
                model.PeriorityId = lstPeriorities[0].Id;
                model.PeriorityName = lstPeriorities[0].Name;
                model.PeriorityNameAr = lstPeriorities[0].NameAr;
            }

            var lstOrigins = _context.AssetPeriorities.Where(a => a.Id == masterAssetObj.OriginId).ToList();
            if (lstOrigins.Count > 0)
            {
                model.OriginId = lstOrigins[0].Id;
                model.OriginName = lstOrigins[0].Name;
                model.OriginNameAr = lstOrigins[0].NameAr;
            }

            var lstCategories = _context.Categories.Where(a => a.Id == masterAssetObj.CategoryId).ToList();
            if (lstCategories.Count > 0)
            {
                model.CategoryId = lstCategories[0].Id;
                model.CategoryName = lstCategories[0].Name;
                model.CategoryNameAr = lstCategories[0].NameAr;
            }


            var lstSubCategories = _context.SubCategories.Where(a => a.Id == masterAssetObj.SubCategoryId).ToList();
            if (lstSubCategories.Count > 0)
            {
                model.SubCategoryId = lstSubCategories[0].Id;
                model.SubCategoryName = lstSubCategories[0].Name;
                model.SubCategoryNameAr = lstSubCategories[0].NameAr;
            }
            return model;
        }


        public IEnumerable<IndexMasterAssetVM.GetData> GetTop10MasterAsset(int hospitalId)
        {
            List<IndexMasterAssetVM.GetData> list = new List<IndexMasterAssetVM.GetData>();

            if (hospitalId != 0)
            {
                var lstAssets = _context.AssetDetails
                    .Include(a => a.MasterAsset)
                    .Include(a => a.MasterAsset.brand)
                    .Include(a => a.MasterAsset.ECRIS)
                    .Include(a => a.MasterAsset.Origin)
                    .Where(a => a.HospitalId == hospitalId).ToList().OrderBy(a => a.MasterAsset.Name).ToList();//.GroupBy(a=>a.MasterAsset.Id);
                foreach (var item in lstAssets)
                {
                    IndexMasterAssetVM.GetData getDataObj = new IndexMasterAssetVM.GetData();
                    getDataObj.Id = item.MasterAsset.Id;
                    getDataObj.Code = item.MasterAsset.Code;
                    getDataObj.Model = item.MasterAsset.ModelNumber;
                    getDataObj.PMColor = item.MasterAsset.PMColor;
                    getDataObj.PMBGColor = item.MasterAsset.PMBGColor;
                    getDataObj.ECRIName = item.MasterAsset.ECRIId != null ? item.MasterAsset.ECRIS.Name : "";
                    getDataObj.ECRINameAr = item.MasterAsset.ECRIId != null ? item.MasterAsset.ECRIS.NameAr : "";
                    getDataObj.Name = item.MasterAsset.Name;
                    getDataObj.NameAr = item.MasterAsset.NameAr;
                    getDataObj.OriginName = item.MasterAsset.OriginId != null ? item.MasterAsset.Origin.Name : "";
                    getDataObj.OriginNameAr = item.MasterAsset.OriginId != null ? item.MasterAsset.Origin.NameAr : "";
                    getDataObj.BrandName = item.MasterAsset.BrandId != null ? item.MasterAsset.brand.Name : "";
                    getDataObj.BrandNameAr = item.MasterAsset.BrandId != null ? item.MasterAsset.brand.NameAr : "";
                    list.Add(getDataObj);
                }
            }
            else
            {
                var lstMasters = _context.MasterAssets.Include(a => a.brand).Include(a => a.ECRIS).Include(a => a.Origin).OrderBy(a => a.Name).ToList();
                foreach (var item in lstMasters)
                {
                    IndexMasterAssetVM.GetData getDataObj = new IndexMasterAssetVM.GetData();
                    getDataObj.Id = item.Id;
                    getDataObj.Code = item.Code;
                    getDataObj.Model = item.ModelNumber;
                    getDataObj.PMColor = item.PMColor;
                    getDataObj.PMBGColor = item.PMBGColor;
                    getDataObj.ECRIName = item.ECRIId != null ? item.ECRIS.Name : "";
                    getDataObj.ECRINameAr = item.ECRIId != null ? item.ECRIS.NameAr : "";
                    getDataObj.Name = item.Name;
                    getDataObj.NameAr = item.NameAr;
                    getDataObj.OriginName = item.OriginId != null ? item.Origin.Name : "";
                    getDataObj.OriginNameAr = item.OriginId != null ? item.Origin.NameAr : "";
                    getDataObj.BrandName = item.brand != null ? item.brand.Name : "";
                    getDataObj.BrandNameAr = item.brand != null ? item.brand.NameAr : "";
                    list.Add(getDataObj);
                }
            }
            return list;
        }

        public int UpdateMasterAssetImageAfterInsert(CreateMasterAssetVM masterAssetObj)
        {
            var masterObj = _context.MasterAssets.Find(masterAssetObj.Id);
            masterObj.AssetImg = masterAssetObj.AssetImg;
            _context.Entry(masterObj).State = EntityState.Modified;
            _context.SaveChanges();
            return masterAssetObj.Id;
        }


        #region Refactor Functions

        /// <summary>
        /// List MasterAsset
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IndexMasterAssetVM.GetData> GetListMasterAsset()
        {
            List<IndexMasterAssetVM.GetData> list = new List<IndexMasterAssetVM.GetData>();
            var lstMasterAssets = _context.AssetDetails.Include(a => a.MasterAsset).Include(a => a.MasterAsset.brand)
                                   .ToList().GroupBy(a => a.MasterAssetId).ToList();


            foreach (var item in lstMasterAssets)
            {
                IndexMasterAssetVM.GetData masterAssetObj = new IndexMasterAssetVM.GetData();
                masterAssetObj.Id = item.FirstOrDefault().MasterAsset.Id;
                masterAssetObj.Model = item.FirstOrDefault().MasterAsset != null ? item.FirstOrDefault().MasterAsset.ModelNumber : "";
                masterAssetObj.Name = item.FirstOrDefault().MasterAsset != null ? item.FirstOrDefault().MasterAsset.Name : "";
                masterAssetObj.NameAr = item.FirstOrDefault().MasterAsset != null ? item.FirstOrDefault().MasterAsset.NameAr : "";
                masterAssetObj.BrandName = item.FirstOrDefault().MasterAsset.brand != null ? item.FirstOrDefault().MasterAsset.brand.Name : "";
                masterAssetObj.BrandNameAr = item.FirstOrDefault().MasterAsset.brand != null ? item.FirstOrDefault().MasterAsset.brand.NameAr : "";
                list.Add(masterAssetObj);
            }
            return list;
        }

        /// <summary>
        /// List MasterAsset By Sort and Search Criteria with Paging
        /// </summary>
        /// <returns></returns>
        public async Task<IndexMasterAssetVM> GetAll(int First, int Rows, SearchSortMasterAssetVM SearchSortObj)
        {
            #region Initial Variables
            IQueryable<MasterAsset> lstMasters = _context.MasterAssets.Include(a => a.brand).Include(a => a.Category)
                  .Include(a => a.SubCategory).Include(a => a.ECRIS).Include(a => a.Origin);
            IndexMasterAssetVM mainClass = new IndexMasterAssetVM();
            List<IndexMasterAssetVM.GetData> list = new List<IndexMasterAssetVM.GetData>();
            #endregion

            #region Search Criteria

            if (!string.IsNullOrEmpty(SearchSortObj.AssetName))
            {
                lstMasters = lstMasters.Where(x => x.Name.Contains(SearchSortObj.AssetName));
            }
            if (!string.IsNullOrEmpty(SearchSortObj.AssetNameAr))
            {
                lstMasters = lstMasters.Where(x => x.NameAr.Contains(SearchSortObj.AssetNameAr));
            }
            if (SearchSortObj.BrandId != 0)
            {
                lstMasters = lstMasters.Where(x => x.BrandId == SearchSortObj.BrandId);
            }
            if (SearchSortObj.OriginId != 0)
            {
                lstMasters = lstMasters.Where(x => x.OriginId == SearchSortObj.OriginId);
            }
            if (!string.IsNullOrEmpty(SearchSortObj.ModelNumber))
            {
                lstMasters = lstMasters.Where(x => x.ModelNumber == SearchSortObj.ModelNumber);
            }
            if (SearchSortObj.CategoryId != 0)
            {
                lstMasters = lstMasters.Where(x => x.CategoryId == SearchSortObj.CategoryId);
            }
            if (SearchSortObj.SubCategoryId != 0)
            {
                lstMasters = lstMasters.Where(x => x.SubCategoryId == SearchSortObj.SubCategoryId);
            }



            #endregion

            #region Sort Criteria

            if (SearchSortObj.SortFiled != null)
            {
                switch (SearchSortObj.SortFiled)
                {
                    case "Code":
                    case "الكود":
                        if (SearchSortObj.SortOrder == 1)
                        {
                            lstMasters = lstMasters.OrderBy(x => x.Code);
                        }
                        else
                        {
                            lstMasters = lstMasters.OrderByDescending(x => x.Code);
                        }

                        break;
                    case "Model Number":
                    case "ModelNumber":
                    case "الموديل":
                        if (SearchSortObj.SortOrder == 1)
                        {
                            lstMasters = lstMasters.OrderBy(x => x.ModelNumber);
                        }
                        else
                        {
                            lstMasters = lstMasters.OrderByDescending(x => x.ModelNumber);
                        }
                        break;
                    case "ECRI":
                        if (SearchSortObj.SortOrder == 1)
                        {
                            lstMasters = lstMasters.OrderBy(x => x.ECRIS.Name);
                        }
                        else
                        {
                            lstMasters = lstMasters.OrderByDescending(x => x.ECRIS.Name);
                        }
                        break;
                    case "Name":
                        if (SearchSortObj.SortOrder == 1)
                        {
                            lstMasters = lstMasters.OrderBy(x => x.Name);
                        }
                        else
                        {
                            lstMasters = lstMasters.OrderByDescending(x => x.Name);
                        }
                        break;

                    case "الاسم":
                        if (SearchSortObj.SortOrder == 1)
                        {
                            lstMasters = lstMasters.OrderBy(x => x.NameAr);
                        }
                        else
                        {
                            lstMasters = lstMasters.OrderByDescending(x => x.NameAr);
                        }
                        break;


                    case "Origin":
                        if (SearchSortObj.SortOrder == 1)
                        {
                            lstMasters = lstMasters.OrderBy(x => x.Origin.Name);
                        }
                        else
                        {
                            lstMasters = lstMasters.OrderByDescending(x => x.Origin.Name);
                        }
                        break;

                    case "بلد المنشأ":
                        if (SearchSortObj.SortOrder == 1)
                        {
                            lstMasters = lstMasters.OrderBy(x => x.Origin.NameAr);
                        }
                        else
                        {
                            lstMasters = lstMasters.OrderByDescending(x => x.Origin.NameAr);
                        }
                        break;
                    case "Brand":
                    case "Manufacture":
                        if (SearchSortObj.SortOrder == 1)
                        {
                            lstMasters = lstMasters.OrderBy(x => x.brand.Name);
                        }
                        else
                        {
                            lstMasters = lstMasters.OrderByDescending(x => x.brand.Name);
                        }
                        break;
                    case "الماركات":
                        if (SearchSortObj.SortOrder == 1)
                        {
                            lstMasters = lstMasters.OrderBy(x => x.brand.NameAr);
                        }
                        else
                        {
                            lstMasters = lstMasters.OrderByDescending(x => x.brand.NameAr);
                        }
                        break;
                    case "الماركة":
                        if (SearchSortObj.SortOrder == 1)
                        {
                            lstMasters = lstMasters.OrderBy(x => x.brand.NameAr);
                        }
                        else
                        {
                            lstMasters = lstMasters.OrderByDescending(x => x.brand.NameAr);
                        }
                        break;
                    default:
                        lstMasters = lstMasters.OrderBy(x => x.Code);
                        break;
                }
            }


            #endregion

            #region Represent data by Paging and count
            mainClass.Count = await lstMasters.CountAsync();
            lstMasters = lstMasters.Skip(First).Take(Rows);


            #endregion

            #region Loop to get Items after serach and sort

            mainClass.Results = await lstMasters.Select(item =>
                 new IndexMasterAssetVM.GetData()
                 {
                     Id = item.Id,
                     Code = item.Code,
                     Model = item.ModelNumber,
                     CategoryId = item.CategoryId,
                     SubCategoryId = item.SubCategoryId,
                     ECRIName = item.ECRIId != null ? item.ECRIS.Name : "",
                     ECRINameAr = item.ECRIId != null ? item.ECRIS.NameAr : "",
                     Name = item.Name,
                     NameAr = item.NameAr,
                     OriginName = item.OriginId != null ? item.Origin.Name : "",
                     OriginNameAr = item.OriginId != null ? item.Origin.NameAr : "",
                     BrandName = item.brand != null ? item.brand.Name : "",
                     BrandNameAr = item.brand != null ? item.brand.NameAr : "",
                 }).ToListAsync();

            #endregion

           

            return mainClass;
        }

        /// <summary>
        /// Get MasterAssetObj By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public EditMasterAssetVM GetById(int id)
        {
            var lstMasterAssets = _context.MasterAssets.Include(a => a.brand).FirstOrDefault(a=>a.Id==id);

            if (lstMasterAssets!=null)
            {
                MasterAsset item = lstMasterAssets;
                EditMasterAssetVM masterAssetObj = new EditMasterAssetVM();
                masterAssetObj.Id = item.Id;
                masterAssetObj.Name = item.Name;
                masterAssetObj.NameAr = item.NameAr;
                masterAssetObj.Code = item.Code;
                masterAssetObj.ECRIId = item.ECRIId != null ? (int)item.ECRIId : null;
                masterAssetObj.BrandId = item.brand != null ? item.BrandId : null;
                masterAssetObj.CategoryId = item.CategoryId != null ? item.CategoryId : null;
                masterAssetObj.SubCategoryId = item.SubCategoryId != null ? item.SubCategoryId : null;
                masterAssetObj.Description = item.Description;
                masterAssetObj.DescriptionAr = item.DescriptionAr;
                masterAssetObj.ExpectedLifeTime = item.ExpectedLifeTime != null ? (int)item.ExpectedLifeTime : 0;
                masterAssetObj.Height = item.Height;
                masterAssetObj.Length = item.Length;
                masterAssetObj.ModelNumber = item.ModelNumber;
                masterAssetObj.Model = item.ModelNumber;
                masterAssetObj.VersionNumber = item.VersionNumber;
                masterAssetObj.Weight = item.Weight;
                masterAssetObj.Width = item.Width;
                masterAssetObj.PeriorityId = item.PeriorityId;
                masterAssetObj.OriginId = item.OriginId != null ? item.OriginId : null;
                masterAssetObj.Power = item.Power;
                masterAssetObj.Voltage = item.Voltage;
                masterAssetObj.Ampair = item.Ampair;
                masterAssetObj.Frequency = item.Frequency;
                masterAssetObj.ElectricRequirement = item.ElectricRequirement;
                masterAssetObj.PMTimeId = item.PMTimeId;
                masterAssetObj.AssetImg = item.AssetImg;
                if (item.BrandId != null)
                {
                    masterAssetObj.BrandName = item.brand.Name;
                    masterAssetObj.BrandNameAr = item.brand.NameAr;
                }
                return masterAssetObj;
            }

            return null;
        }

        /// <summary>
        ///  Update MasterAsset Object
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Update(EditMasterAssetVM model)
        {
            try
            {

                var masterAssetObj = _context.MasterAssets.Find(model.Id);
                masterAssetObj.Id = model.Id;
                masterAssetObj.Code = model.Code;
                masterAssetObj.Name = model.Name;
                masterAssetObj.NameAr = model.NameAr;
                masterAssetObj.BrandId = model.BrandId;
                masterAssetObj.CategoryId = model.CategoryId;
                masterAssetObj.SubCategoryId = model.SubCategoryId;
                masterAssetObj.Description = model.Description;
                masterAssetObj.DescriptionAr = model.DescriptionAr;
                masterAssetObj.ExpectedLifeTime = model.ExpectedLifeTime;
                masterAssetObj.Height = model.Height;
                masterAssetObj.Length = model.Length;
                masterAssetObj.ModelNumber = model.ModelNumber;
                masterAssetObj.VersionNumber = model.VersionNumber;
                masterAssetObj.Weight = model.Weight;
                masterAssetObj.Width = model.Width;
                masterAssetObj.ECRIId = model.ECRIId;
                masterAssetObj.PeriorityId = model.PeriorityId;
                masterAssetObj.OriginId = model.OriginId;
                masterAssetObj.Power = model.Power;
                masterAssetObj.Voltage = model.Voltage;
                masterAssetObj.Ampair = model.Ampair;
                masterAssetObj.Frequency = model.Frequency;
                masterAssetObj.ElectricRequirement = model.ElectricRequirement;
                masterAssetObj.PMTimeId = model.PMTimeId;
                masterAssetObj.AssetImg = model.AssetImg;

                _context.Entry(masterAssetObj).State = EntityState.Modified;
                _context.SaveChanges();
                return masterAssetObj.Id;
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }

            return 0;
        }

        /// <summary>
        /// Create MasterAsset Object
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Add(CreateMasterAssetVM model)
        {
            MasterAsset masterAssetObj = new MasterAsset();
            try
            {
                if (model != null)
                {
                    masterAssetObj.Code = model.Code;
                    masterAssetObj.Name = model.Name;
                    masterAssetObj.NameAr = model.NameAr;
                    if (model.BrandId != null)
                        masterAssetObj.BrandId = model.BrandId;
                    if (model.CategoryId != null)
                        masterAssetObj.CategoryId = model.CategoryId;
                    if (model.SubCategoryId != null)
                        masterAssetObj.SubCategoryId = model.SubCategoryId;
                    masterAssetObj.Description = model.Description;
                    masterAssetObj.DescriptionAr = model.DescriptionAr;
                    masterAssetObj.ExpectedLifeTime = model.ExpectedLifeTime;
                    masterAssetObj.Height = model.Height;
                    masterAssetObj.Length = model.Length;
                    masterAssetObj.ModelNumber = model.ModelNumber;
                    masterAssetObj.VersionNumber = model.VersionNumber;
                    masterAssetObj.Weight = model.Weight;
                    masterAssetObj.Width = model.Width;
                    if (model.ECRIId != null)
                        masterAssetObj.ECRIId = model.ECRIId;

                    if (model.PeriorityId != null)
                        masterAssetObj.PeriorityId = model.PeriorityId;
                    if (model.OriginId != null)
                        masterAssetObj.OriginId = model.OriginId;
                    masterAssetObj.Power = model.Power;
                    masterAssetObj.Voltage = model.Voltage;
                    masterAssetObj.Ampair = model.Ampair;
                    masterAssetObj.Frequency = model.Frequency;
                    masterAssetObj.ElectricRequirement = model.ElectricRequirement;
                    if (model.PMTimeId != 0)
                        masterAssetObj.PMTimeId = model.PMTimeId;
                    if(model.AssetImg != "")
                    masterAssetObj.AssetImg = model.AssetImg;

                    _context.MasterAssets.Add(masterAssetObj);
                    _context.SaveChanges();
                    return masterAssetObj.Id;
                }
            }
            catch (Exception ex)
            {
                string str = ex.Message;
            }
            return masterAssetObj.Id;
        }

        /// <summary>
        /// Delete MasterAsset by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int Delete(int id)
        {
            
            try
            {

                    var masterAssetObj=_context.MasterAssets.Find(id);
                    _context.MasterAssets.Remove(masterAssetObj);
                    return _context.SaveChanges();
                
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }

            return 0;
        }


        #region Search Auto Complete 


        /// <summary>
        /// Distinct AutoComplete Master Asset Name for Generic Report
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IEnumerable<MasterAsset> DistinctAutoCompleteMasterAssetName(string name)
        {
            var lstMasters = _context.MasterAssets.Where(a => a.Name.Contains(name) || a.NameAr.Contains(name))
                   .ToList().GroupBy(a => a.Name).ToList();
            if (lstMasters.Count > 0)
            {
                var lstNames = lstMasters.Select(x => x.FirstOrDefault()).ToList();
                return lstNames;
            }

            return null;
        }

        public IEnumerable<MasterAsset> AutoCompleteMasterAssetName(string name)
        {
            var lst = _context.MasterAssets.Where(a => a.Name.Contains(name) || a.NameAr.Contains(name)).ToList();
            return lst;
        }

        public IEnumerable<IndexMasterAssetVM.GetData> AutoCompleteMasterAssetName2(string name)
        {
            var lst = _context.MasterAssets.Include(a => a.brand).Where(a => a.Name.Contains(name) || a.NameAr.Contains(name))
                .Select(item => new IndexMasterAssetVM.GetData
                {
                    Id = item.Id,
                    Code = item.Code,
                    Name = item.Name,
                    NameAr = item.NameAr,
                    Model = item.ModelNumber,
                    ModelNumber = item.ModelNumber,
                    BrandName = item.brand != null ? item.brand.Name : "",
                    BrandNameAr = item.brand != null ? item.brand.NameAr : ""
                }).ToList();
            return lst;
        }

        public async Task<IEnumerable<AssetDetailsWithMasterAssetVM>> AutoCompleteMasterAssetName(string name, int hospitalId, string UserId)
        {

            IQueryable<AssetDetail> query = _context.AssetDetails.Include(a => a.MasterAsset).Include(a => a.MasterAsset.brand);
            List<AssetDetailsWithMasterAssetVM> lstAsset = new List<AssetDetailsWithMasterAssetVM>();


            var locationIds = await _context.Users.Select(u => new { UserId = u.Id, hospitalId = u.HospitalId, CityId = u.CityId, GovernorateId = u.GovernorateId, OrganizationId = u.OrganizationId, SubOrganizationId = u.SubOrganizationId }).FirstOrDefaultAsync(u => u.UserId == UserId);

            if (locationIds.hospitalId == 0 && locationIds.CityId == 0 && locationIds.GovernorateId > 0 && locationIds.OrganizationId == 0 && locationIds.SubOrganizationId == 0)
            {
                query = query.Where(a => a.Hospital.GovernorateId == locationIds.GovernorateId);
            }
            else if (locationIds.hospitalId == 0 && locationIds.CityId > 0 && locationIds.GovernorateId > 0 && locationIds.OrganizationId == 0 && locationIds.SubOrganizationId == 0)
            {
                query = query.Where(a => a.Hospital.CityId == locationIds.CityId && a.Hospital.OrganizationId == locationIds.OrganizationId);
            }
            else if (locationIds.hospitalId == 0 && locationIds.CityId == 0 && locationIds.GovernorateId == 0 && locationIds.OrganizationId > 0 && locationIds.SubOrganizationId == 0)
            {
                query = query.Where(a => a.Hospital.OrganizationId == locationIds.OrganizationId);
            }
            else if (locationIds.hospitalId == 0 && locationIds.CityId == 0 && locationIds.GovernorateId > 0 && locationIds.OrganizationId > 0 && locationIds.SubOrganizationId > 0)
            {
                query = query.Where(a => a.Hospital.OrganizationId == locationIds.OrganizationId && a.Hospital.SubOrganizationId == locationIds.SubOrganizationId);
            }
            if (hospitalId != 0)
            {
                query = query.Where(a => a.Hospital.Id == hospitalId);
            }
            
           lstAsset=await query.Where(a => a.MasterAsset.Name.Contains(name) || a.MasterAsset.NameAr.Contains(name) && a.HospitalId == hospitalId).Select(item => new AssetDetailsWithMasterAssetVM
           {
               Id = item.Id,
               MasterAssetCode = item.MasterAsset.Code,
               MasterAsseName = item.MasterAsset != null ? item.MasterAsset.Name : "",
               MasterAsseNameAr = item.MasterAsset != null ? item.MasterAsset.NameAr : "",
               MasterAsseModelNumbe = item.MasterAsset != null ? item.MasterAsset.ModelNumber : "",
               MasterAsseBrandName = item.MasterAsset.brand != null ? item.MasterAsset.brand.Name : "",
               MasterAsseBrandNameAr = item.MasterAsset.brand != null ? item.MasterAsset.brand.NameAr : "",
               SerialNumber = item.SerialNumber != null ? item.SerialNumber : "",
               Barcode = item.Barcode != null ? item.Barcode : "",
           }).ToListAsync();
            
            return lstAsset;
        }

        #endregion
















        /// <summary>
        /// Generate MasterAsset Code When Create new item
        /// </summary>
        /// <returns></returns>
        public GeneratedMasterAssetCodeVM GenerateMasterAssetcode()
        {
            GeneratedMasterAssetCodeVM numberObj = new GeneratedMasterAssetCodeVM();
            int barCode = 0;

            var lastId = _context.MasterAssets.ToList();
            if (lastId.Count > 0)
            {
                var code = lastId.Max(a => a.Id);
                var barcode = (code + 1).ToString();
                var lastcode = barcode.ToString().PadLeft(5, '0');
                numberObj.Code = lastcode;
            }
            else
            {
                numberObj.Code = (barCode + 1).ToString();
            }
            return numberObj;
        }


        #region Master Assets Attachments
        /// <summary>
        /// List Attachments depend on Master Asset Id
        /// </summary>
        /// <param name="assetId"></param>
        /// <returns></returns>
        public IEnumerable<MasterAssetAttachment> GetAttachmentByMasterAssetId(int assetId)
        {
            return _context.MasterAssetAttachments.Where(a => a.MasterAssetId == assetId).ToList();
        }

        /// <summary>
        /// Add AttachmentObj for master asset Id
        /// </summary>
        /// <param name="attachObj"></param>
        /// <returns></returns>
        public int CreateMasterAssetDocuments(CreateMasterAssetAttachmentVM attachObj)
        {
            MasterAssetAttachment MasterAssetAttachmentObj = new MasterAssetAttachment();
            MasterAssetAttachmentObj.MasterAssetId = attachObj.MasterAssetId;
            MasterAssetAttachmentObj.Title = attachObj.Title;
            MasterAssetAttachmentObj.FileName = attachObj.FileName;
            _context.MasterAssetAttachments.Add(MasterAssetAttachmentObj);
            _context.SaveChanges();
            int Id = MasterAssetAttachmentObj.Id;
            return Id;
        }

        /// <summary>
        /// Delete Attachment Item for Master Asset
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int DeleteMasterAssetAttachment(int id)
        {
            try
            {
                var attachObj = _context.MasterAssetAttachments.Find(id);
                _context.MasterAssetAttachments.Remove(attachObj);
                return _context.SaveChanges();
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }

            return 0;
        }


        /// <summary>
        /// Get Last Document For Master Asset by Id If we need in Edit mode to add more attachments
        /// </summary>
        /// <param name="masterId"></param>
        /// <returns></returns>
        public MasterAssetAttachment GetLastDocumentForMsterAssetId(int masterId)
        {
            MasterAssetAttachment documentObj = new MasterAssetAttachment();
            var lstDocuments = _context.MasterAssetAttachments.Where(a => a.MasterAssetId == masterId).OrderBy(a => a.FileName).ToList();
            if (lstDocuments.Count > 0)
            {
                documentObj = lstDocuments.Last();
            }
            return documentObj;
        }

        public IEnumerable<string> GetDistintMasterAssetModels(int brandId, string name)
        {


            // Filter the assets based on name
            var filteredAssets = _context.MasterAssets.Include(a=>a.brand)
                .Where(a => (a.Name.Contains(name) || a.NameAr.Contains(name)) && a.BrandId == brandId)
                .ToList();

            // Extract brand names
            var brandList = filteredAssets
                 .Select(b => b.ModelNumber)  // Assuming `Name` is the property in `Brand` entity
                .ToList();

            return brandList;
        }

        public IEnumerable<Brand> GetDistintMasterAssetBrands(string name)
        {
            // Filter the assets based on name
            var filteredAssets = _context.MasterAssets
                .Include(a => a.brand)
                .Where(a => a.Name.Contains(name) || a.NameAr.Contains(name))
                .Select(a => a.brand)
                .Distinct()
                .ToList();

            // Extract brand names
            var brandList = filteredAssets
               // .Select(b => b.Name)  // Assuming `Name` is the property in `Brand` entity
                .ToList();

            return brandList;
           // return null;
        }

        public List<MasterAsset> GetMasterAssetIdByNameBrandModel(string name, int brandId, string model)
        {
           var lstAssets = _context.MasterAssets
             //.Include(a => a.brand)
             .Where(a => (a.Name.Contains(name) || a.NameAr.Contains(name)) && a.BrandId == brandId && a.ModelNumber == model)
             .ToList();

            return lstAssets;
        }



        #endregion

        #endregion

    }
}
