using Asset.ViewModels.RoleCategoryVM;
using Asset.ViewModels.RoleVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset.Domain.Repositories
{
    public interface IRoleRepository
    {
         Task<IndexRoleVM> getAll(int first, int rows, SortSearchVM sortSearchObj);
        Task<bool> hasRoleWithRoleCategoryId(int id);
    }
}
