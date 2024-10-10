using Asset.Domain;
using Asset.Domain.Services;
using Asset.Models;
using Asset.ViewModels.ModuleVM;
using Asset.ViewModels.RoleCategoryVM;
using Asset.ViewModels.RoleVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset.Core.Services
{
    public class RoleService: IRoleService
    {
        private IUnitOfWork _unitOfWork;
        public RoleService(IUnitOfWork unitOfWork)
        {
            _unitOfWork=unitOfWork;
        }
        public async Task<bool> hasRoleWithRoleCategoryId(int id)
        {
            return await _unitOfWork.RoleRepository.hasRoleWithRoleCategoryId(id);
        }
        public Task<IndexRoleVM> getAll(int first, int rows, SortSearchVM sortSearchObj)
        {
            return _unitOfWork.RoleRepository.getAll(first, rows, sortSearchObj);
        }
        public async Task AddRoleWithModulePermissionsAsync(CreateRoleVM createRole)
        {
            
            ApplicationRole roleObj=new ApplicationRole() { Name=createRole.Name,DisplayName=createRole.DisplayName,RoleCategoryId=createRole.RolecategoryID};
            var roleId= await _unitOfWork.RoleRepository.createRole(roleObj);
            await _unitOfWork.RoleRepository.AddModulePermissionsAsync(roleId,createRole.ModuleIdsWithPermissions);
        }
        public async Task<string> ValidateModuleAndPermissionsAsync(IEnumerable<ModuleIdWithPermissionsVM> ModuleIdsWithPermissions)
        {
            return await _unitOfWork.RoleRepository.ValidateModuleAndPermissionsAsync(ModuleIdsWithPermissions);
        }
        public async Task<string> CheckRoleExists(string Name, string DisplayName)
        {
            var nameExists =await _unitOfWork.RoleRepository.CheckRoleNameExists(Name);
            if (nameExists) return "Name";
            var DisplayNameExists = await _unitOfWork.RoleRepository.CheckRoleDisplayNameExists(DisplayName);
            if (DisplayNameExists) return "DisplayName";
            return null;
        }
        public async Task<RoleVM> getById(string roleId)
        {
            return await _unitOfWork.RoleRepository.getById(roleId);
        }
        public async Task<ModulesPermissionsResult> getModulesPermissionsbyRoleId(string roleId, int first, int rows, SortSearchVM sortSearchObj)
        {
            return await _unitOfWork.RoleRepository.getModulesPermissionsbyRoleId(roleId, first, rows, sortSearchObj);
        }
        public async Task<ModulesPermissionsWithSelectedPermissionIDsResult> getModulesPermissionsbyRoleIdForEdit(string roleId, int first, int rows, SortSearchVM sortSearchObj)
        {
            return await _unitOfWork.RoleRepository.getModulesPermissionsbyRoleIdForEdit(roleId, first, rows, sortSearchObj);
        }
        public async Task<bool> IsRoleAssignedToUsersService(string RoleId)
        {
            return await _unitOfWork.RoleRepository.IsRoleAssignedToUsers(RoleId);
        }
        public async Task DeleteRoleService(string RoleId)
        {
            await _unitOfWork.RoleRepository.DeleteRole(RoleId);
        }
        

    }
}
