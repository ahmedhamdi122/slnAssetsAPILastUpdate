using Asset.Domain.Repositories;
using Asset.Models;
using Asset.ViewModels.BrandVM;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset.Core.Repositories
{
    public class BrandRepositories : IBrandRepository
    {
        private ApplicationDbContext _context;

        public BrandRepositories(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Boolean> CheckNameBrandExistsBeforeAsync(string Name)
        {
            return await _context.Brands.AnyAsync(b => b.Name == Name);
        }
        public async Task<Boolean> CheckNameArBrandExistsBeforeAsync(string NameAr)
        {
            return await _context.Brands.AnyAsync(b => b.NameAr == NameAr);
        }
        public async Task<Boolean> CheckCodeBrandExistsBeforeAsync(string code)
        {
            return await _context.Brands.AnyAsync(b=>b.Code==code);
        }
        public async Task<int> Add(CreateBrandVM model)
        {
            Brand brandObj = new Brand();
            try
            {
                if (model != null)
                {
                    brandObj.Code = model.Code;
                    brandObj.Name = model.Name;
                    brandObj.NameAr = model.NameAr;
                    await _context.Brands.AddAsync(brandObj);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                string str = ex.Message;
            }
            return brandObj.Id;
        }

        public IEnumerable<IndexBrandVM.GetData> AutoCompleteBrandName(string brandName)
        {
            var lst = _context.Brands.Where(a => a.Name.Contains(brandName) || a.NameAr.Contains(brandName)).ToList()
               .Select(item => new IndexBrandVM.GetData
               {
                   Id = item.Id,
                   Code = item.Code,
                   Name = item.Name,
                   NameAr = item.NameAr
               }).ToList();
            return lst;
        }

        public int CountBrands()
        {
            return _context.Brands.Count();
        }

        public int Delete(int id)
        {
            var brandObj = _context.Brands.Find(id);
            try
            {
                if (brandObj != null)
                {
                    _context.Brands.Remove(brandObj);
                    return _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }
            return 0;
        }

        public GenerateBrandCodeVM GenerateBrandCode()
        {
            GenerateBrandCodeVM numberObj = new GenerateBrandCodeVM();
            int code = 0;

            var lastId = _context.Brands.ToList();
            if (lastId.Count > 0)
            {

                var lastBrandCode = lastId.Max(a => a.Code);
                if (lastBrandCode == null)
                {
                    numberObj.Code = (code + 1).ToString();
                    var lastcode = numberObj.Code.PadLeft(3, '0');
                    numberObj.Code = lastcode;
                }
                else
                {
                    var hospitalCode = (int.Parse(lastBrandCode) + 1).ToString();
                    var lastcode = hospitalCode.ToString().PadLeft(3, '0');
                    numberObj.Code = lastcode;
                }
            }
            else
            {
                numberObj.Code = (code + 1).ToString();
                var lastcode = numberObj.Code.PadLeft(3, '0');
                numberObj.Code = lastcode;
            }

            return numberObj;
        }

        public IEnumerable<IndexBrandVM.GetData> GetAll()
        {
            return _context.Brands.ToList().Select(item => new IndexBrandVM.GetData
            {
                Id = item.Id,
                Code = item.Code,
                Name = item.Name,
                NameAr = item.NameAr
            });
        }

        public IEnumerable<Brand> GetAllBrands()
        {
            return _context.Brands.ToList();
        }

        public IEnumerable<IndexBrandVM.GetData> GetBrandByName(string brandName)
        {
            return _context.Brands.Where(a => (a.Name == brandName || a.NameAr == brandName)).ToList().Select(item => new IndexBrandVM.GetData
            {

                Id = item.Id,
                Code = item.Code,
                Name = item.Name,
                NameAr = item.NameAr
            });
        }

        public EditBrandVM GetById(int id)
        {
            return _context.Brands.Where(a => a.Id == id).Select(item => new EditBrandVM
            {
                Id = item.Id,
                Code = item.Code,
                Name = item.Name,
                NameAr = item.NameAr
            }).First();
        }

        public IndexBrandVM GetTop10Brands(int hospitalId)
        {
            IndexBrandVM mainClass = new IndexBrandVM();
            List<IndexBrandVM.GetData> list = new List<IndexBrandVM.GetData>();
            List<IndexBrandVM.GetData> lstBrands = new List<IndexBrandVM.GetData>();
            if (hospitalId != 0)
            {
                lstBrands = _context.AssetDetails.Include(a => a.MasterAsset).Include(a => a.MasterAsset.brand).Include(a => a.Hospital)
                   .Where(a => a.HospitalId == hospitalId)
                   .ToList().GroupBy(a => new { a.MasterAssetId, a.MasterAsset.BrandId }).Select(item => new IndexBrandVM.GetData
                   {
                       Id = item.FirstOrDefault().MasterAsset.brand != null ? item.FirstOrDefault().MasterAsset.brand.Id : 0,
                       Code = item.FirstOrDefault().MasterAsset.brand != null ? item.FirstOrDefault().MasterAsset.brand.Code : "",
                       Name = item.FirstOrDefault().MasterAsset.brand != null ? item.FirstOrDefault().MasterAsset.brand.Name : "",
                       NameAr = item.FirstOrDefault().MasterAsset.brand != null ? item.FirstOrDefault().MasterAsset.brand.NameAr : ""
                   }).OrderBy(a=>a.Code).ToList();
            }
            else
            {
                lstBrands = _context.MasterAssets.Include(a => a.brand)
                    .ToList().GroupBy(a => new { a.BrandId }).ToList().Select(item => new IndexBrandVM.GetData
                    {
                        Id = item.FirstOrDefault().brand != null ? item.FirstOrDefault().brand.Id : 0,
                        Code = item.FirstOrDefault().brand != null ? item.FirstOrDefault().brand.Code : "",
                        Name = item.FirstOrDefault().brand != null ? item.FirstOrDefault().brand.Name : "",
                        NameAr = item.FirstOrDefault().brand != null ? item.FirstOrDefault().brand.NameAr : ""
                    }).OrderBy(a => a.Code).ToList();
            }

            mainClass.Results = lstBrands;
            mainClass.Count = lstBrands.Count;
            return mainClass;
        }



        public IndexBrandVM ListBrands(SortAndFilterBrandVM data, int pageNumber, int pageSize)
        {
            #region Initial Variables
            IQueryable<Brand> query = _context.Brands;
            IndexBrandVM mainClass = new IndexBrandVM();
            List<IndexBrandVM.GetData> list = new List<IndexBrandVM.GetData>();
            #endregion

            #region Search Criteria
            if (data.SearchObj.Id != 0)
            {
                query = query.Where(x => x.Id == data.SearchObj.Id);
            }

            #endregion

            #region Sort Criteria

            switch (data.SortObj.SortBy)
            {
                case "Code":
                case "الكود":
                    if (data.SortObj.SortStatus == "ascending")
                    {
                        query.OrderBy(x => x.Code);
                    }
                    else
                    {
                        query.OrderByDescending(x => x.Code);
                    }
                    break;
                case "Name":
                    if (data.SortObj.SortStatus == "ascending")
                    {
                        query.OrderBy(x => x.Name);
                    }
                    else
                    {
                        query.OrderByDescending(x => x.Name);
                    }
                    break;

                case "الاسم":
                    if (data.SortObj.SortStatus == "ascending")
                    {
                        query = query.OrderBy(x => x.NameAr);
                    }
                    else
                    {
                        query = query.OrderByDescending(x => x.NameAr);
                    }
                    break;
            }

            #endregion

            #region Count data and fiter By Paging
            mainClass.Count = query.Count();

            var lstResults = query.Skip((pageNumber - 1) * pageSize).Take(pageSize);
            #endregion

            #region Loop to get Items after serach and sort
            foreach (var item in lstResults.ToList())
            {

                IndexBrandVM.GetData getDataobj = new IndexBrandVM.GetData();
                getDataobj.Id = item.Id;
                getDataobj.Code = item.Code;
                getDataobj.Name = item.Name;
                getDataobj.NameAr = item.NameAr;
                list.Add(getDataobj);
            }
            #endregion

            #region Represent data after Paging and count
            mainClass.Results = list;
            #endregion

            return mainClass;
        }

        public IEnumerable<IndexBrandVM.GetData> SortBrands(SortBrandVM sortObj)
        {
            var lstBrands = GetAll().ToList();
            if (sortObj.Code != "")
            {
                if (sortObj.SortStatus == "descending")
                    lstBrands = lstBrands.OrderByDescending(d => d.Code).ToList();
                else
                    lstBrands = lstBrands.OrderBy(d => d.Code).ToList();
            }
            else if (sortObj.Name != "")
            {
                if (sortObj.SortStatus == "descending")
                    lstBrands = lstBrands.OrderByDescending(d => d.Name).ToList();
                else
                    lstBrands = lstBrands.OrderBy(d => d.Name).ToList();
            }

            else if (sortObj.NameAr != "")
            {
                if (sortObj.SortStatus == "descending")
                    lstBrands = lstBrands.OrderByDescending(d => d.NameAr).ToList();
                else
                    lstBrands = lstBrands.OrderBy(d => d.NameAr).ToList();
            }

            return lstBrands;
        }

        public int Update(EditBrandVM model)
        {
            try
            {
                var brandObj = _context.Brands.Find(model.Id);
                brandObj.Code = model.Code;
                brandObj.Name = model.Name;
                brandObj.NameAr = model.NameAr;
                _context.Entry(brandObj).State = EntityState.Modified;
                _context.SaveChanges();
                return brandObj.Id;
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }

            return 0;
        }
    }
}
