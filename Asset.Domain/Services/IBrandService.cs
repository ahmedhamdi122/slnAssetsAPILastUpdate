using Asset.Models;
using Asset.ViewModels.BrandVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset.Domain.Services
{
  public  interface IBrandService
    {
        Task<Boolean> CheckNameArBrandExistsBeforeAsync(string NameAr);
        Task<bool> CheckNameBrandExistsBeforeAsync(string Name);

        Task<bool> CheckCodeBrandExistsBeforeAsync(string code);
        IEnumerable<IndexBrandVM.GetData> GetAll();
        IndexBrandVM GetTop10Brands(int hospitalId);
        EditBrandVM GetById(int id);
        IEnumerable<Brand> GetAllBrands();
        IEnumerable<IndexBrandVM.GetData> GetBrandByName(string brandName);
        IEnumerable<IndexBrandVM.GetData> AutoCompleteBrandName(string brandName);
        IndexBrandVM ListBrands(SortAndFilterBrandVM data, int pageNumber, int pageSize);
        Task<int> Add(CreateBrandVM brandObj);
        int Update(EditBrandVM brandObj);
        int Delete(int id);
        int CountBrands();
        IEnumerable<IndexBrandVM.GetData> SortBrands(SortBrandVM sortObj);
        GenerateBrandCodeVM GenerateBrandCode();
    }
}
