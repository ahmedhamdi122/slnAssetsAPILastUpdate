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
            return await _unitOfWork.Role.hasRoleWithRoleCategoryId(id);
        }
        public Task<IndexRoleVM> getAll(int first, int rows, SortSearchVM sortSearchObj)
        {
            return _unitOfWork.Role.getAll(first, rows, sortSearchObj);
        }
        public async Task AddRoleWithModulePermissionsAsync(CreateRoleVM createRole)
        {
            
            ApplicationRole roleObj=new ApplicationRole() { Name=createRole.Name,DisplayName=createRole.DisplayName,RoleCategoryId=createRole.RolecategoryID};
            var roleId= await _unitOfWork.Role.createRole(roleObj);
            await _unitOfWork.Role.AddModulePermissionsAsync(roleId,createRole.ModuleIdsWithPermissions);
        }
        public async Task<string> ValidateModuleAndPermissionsAsync(IEnumerable<ModuleIdWithPermissionsVM> ModuleIdsWithPermissions)
        {
            return await _unitOfWork.Role.ValidateModuleAndPermissionsAsync(ModuleIdsWithPermissions);
        }
        public async Task<string> CheckRoleExists(string Name, string DisplayName)
        {
            var nameExists =await _unitOfWork.Role.CheckRoleNameExists(Name);
            if (nameExists) return "Name";
            var DisplayNameExists = await _unitOfWork.Role.CheckRoleDisplayNameExists(DisplayName);
            if (DisplayNameExists) return "DisplayName";
            return null;
        }
        public async Task<RoleVM> getById(string roleId)
        {
            return await _unitOfWork.Role.getById(roleId);
        }
        public async Task<ModulesPermissionsResult> getModulesPermissionsbyRoleId(string roleId, int first, int rows, SortSearchVM sortSearchObj)
        {
            return await _unitOfWork.Role.getModulesPermissionsbyRoleId(roleId, first, rows, sortSearchObj);
        }
    }
}
