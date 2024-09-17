using System.Collections.Generic;
using System.Threading.Tasks;
using Asset.Models;
using Asset.ViewModels.RoleCategoryVM;


namespace Asset.Domain.Repositories
{
  public  interface IRoleCategoryRepository
    {
         Task<IEnumerable<IndexCategoryVM.GetData>> GetAll();
        Task<RoleCategory> GetById(int id);
        int Add(CreateRoleCategory roleCategory);
        Task<int> Delete(int id);
        int Update(EditRoleCategory roleCategory);

        Task<bool> isRoleCategoryExistsUsingId(int id);


        Task<IndexCategoryVM> LoadRoleCategories(int first, int rows, string SortField, int SortOrder, string search);

        GenerateRoleCategoryOrderVM GenerateRoleCategoryOrderId();
    }
}
