using System.Collections.Generic;
using System.Threading.Tasks;
using Asset.Models;
using Asset.ViewModels.RoleCategoryVM;


namespace Asset.Domain.Repositories
{
  public  interface IRoleCategoryRepository
    {
         Task<IEnumerable<IndexCategoryVM.GetData>> GetAll();
        RoleCategory GetById(int id);
        int Add(CreateRoleCategory roleCategory);
        int Delete(RoleCategory roleCategory);
        int Update(EditRoleCategory roleCategory);



        Task<IndexCategoryVM> LoadRoleCategories(int first, int rows, string SortField, int SortOrder);

        GenerateRoleCategoryOrderVM GenerateRoleCategoryOrderId();
    }
}
