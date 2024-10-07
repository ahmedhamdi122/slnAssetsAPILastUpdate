using Asset.Domain.Repositories;
using Asset.Models;
using Asset.ViewModels.RoleCategoryVM;
using Asset.ViewModels.RoleVM;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Asset.ViewModels.ModuleVM;
using Asset.Models.Models;

namespace Asset.Core.Repositories
{
    public class RoleRepository: IRoleRepository
    {
        private ApplicationDbContext _context;
        public RoleRepository(ApplicationDbContext context)
        {
            _context=context;
        }
        public async Task<string> createRole(ApplicationRole RoleObj)
        {
            var Counter= await _context.ApplicationRole.CountAsync();
            RoleObj.Counter = Counter+1;
             await _context.Roles.AddAsync(RoleObj);
            await _context.SaveChangesAsync();
            return RoleObj.Id;
        }
        public async Task<IndexRoleVM> getAll(int first, int rows, SortSearchVM sortSearchObj)
        {
            IndexRoleVM mainClass = new IndexRoleVM();

     
            var query =  _context.ApplicationRole
               .Include(r => r.RoleCategory)
                .AsQueryable();


            if (sortSearchObj.SortField == "name")
            {
                query = sortSearchObj.SortOrder == 1 ? query.OrderBy(r => r.Name) : query.OrderByDescending(r => r.Name);
            }
            else if (sortSearchObj.SortField == "displayName")
            {
                query = sortSearchObj.SortOrder == 1 ? query.OrderBy(r => r.DisplayName): query.OrderByDescending(r => r.DisplayName);
            }
            else if (sortSearchObj.SortField == "categoryName")
            {
                query = sortSearchObj.SortOrder == 1? query.OrderBy(r => r.RoleCategory.Name) : query.OrderByDescending(r => r.RoleCategory.Name);
            }
            mainClass.Count = await query.CountAsync();

            mainClass.Results = await query
                .Skip(first)
                .Take(rows)
                .Select(role => new IndexRoleVM.GetData
                {
                    Id = role.Id,
                    Name = role.Name,
                    DisplayName = role.DisplayName,
                    CategoryName = role.RoleCategory.Name
                }).ToListAsync();
                return mainClass;
        }
        public async Task<bool> hasRoleWithRoleCategoryId(int id)
        {
            return await _context.ApplicationRole.AnyAsync(r=> r.RoleCategoryId == id);
             
        }
        public  async Task AddModulePermissionsAsync(string roleId,IEnumerable<ModuleIdWithPermissionsVM> ModuleIdsWithPermissions)
        {
            var RoleModulePermissions = ModuleIdsWithPermissions.SelectMany(mwp => mwp.permissionIDs.Select(permissionId=>new RoleModulePermission() { RoleId=roleId,ModuleId=mwp.moduleId,PermissionId= permissionId }));
            await _context.roleModulePermission.AddRangeAsync(RoleModulePermissions);
            await _context.SaveChangesAsync();  
        }
        public async Task<string?> ValidateModuleAndPermissionsAsync(IEnumerable<ModuleIdWithPermissionsVM> ModuleIdsWithPermissions)
        {
            foreach (var moduleIdWithPermissions in ModuleIdsWithPermissions)
            {
                var ModuleExists = await _context.Modules.AnyAsync(m => m.Id == moduleIdWithPermissions.moduleId);
                if (!ModuleExists)
                {
                    return $"ModuleId : {moduleIdWithPermissions.moduleId} not found";
                }
                foreach (var permissionId in moduleIdWithPermissions.permissionIDs)
                {
                    var permissionIdExists = await _context.Permissions.AnyAsync(p => p.Id == permissionId);
                    if (!permissionIdExists)
                    {
                        return $"permissionId : {permissionId} not found";
                    }

                 }
            }
            return null;

        }
        public async Task<bool> CheckRoleDisplayNameExists(string DisplayName)
        {
            return await _context.ApplicationRole.AnyAsync(r=>r.DisplayName==DisplayName);
        }
        public async Task<bool> CheckRoleNameExists(string Name)
        {
            return await _context.ApplicationRole.AnyAsync(r => r.Name==Name);
        }


    }
}

