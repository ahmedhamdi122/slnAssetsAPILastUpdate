using Asset.ViewModels.RoleCategoryVM;
using Asset.ViewModels.RoleVM;
using Org.BouncyCastle.Crypto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset.Domain.Services
{
    public interface IRoleService
    {
        Task<IndexRoleVM> getAll(int first, int rows, SortSearchVM sortSearchObj);
        Task<bool> hasRoleWithRoleCategoryId(int id);
        Task<string> add(CreateRoleVM createRoleVM); 
    }
}
