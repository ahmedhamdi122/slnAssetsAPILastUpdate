using Asset.Models;
using Asset.ViewModels.ModuleVM;
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
        Task<string> createRole(ApplicationRole createRoleVM);
        Task AddModulePermissionsAsync(string roleId,IEnumerable<ModuleIdWithPermissionsVM> ModuleIdsWithPermissions);
        Task<string> ValidateModuleAndPermissionsAsync(IEnumerable<ModuleIdWithPermissionsVM> ModuleIdsWithPermissions);
        Task<bool> CheckRoleNameExists(string Name);
        Task<bool> CheckRoleDisplayNameExists(string DisplayName);
        Task<RoleVM> getById(string roleId);
        Task<ModulesPermissionsResult> getModulesPermissionsbyRoleId(string roleId,int first, int rows, SortSearchVM sortSearchObj);
        Task<ModulesPermissionsWithSelectedPermissionIDsResult> getModulesPermissionsbyRoleIdForEdit(string roleId, int first, int rows, SortSearchVM sortSearchObj);

    }
}
