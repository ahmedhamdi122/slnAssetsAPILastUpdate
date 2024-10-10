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
using Asset.ViewModels.PermissionVM;

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
             await _context.ApplicationRole.AddAsync(RoleObj);
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
                .Take(rows).OrderBy(r => r.Counter)
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
        public async Task<RoleVM> getById(string roleId)
        {
            var role = await _context.ApplicationRole.Include(r=>r.RoleCategory).FirstOrDefaultAsync(r=>r.Id==roleId);
            if(role!=null)
            {
                return new RoleVM() { Id=role.Id,Name = role.Name, DisplayName = role.DisplayName, RoleCategory = new RoleCategoryVM() {Id=role.RoleCategory.Id,Name=role.RoleCategory.Name,NameAr=role.RoleCategory.NameAr  } };
            }
            return null;
        }
        public async Task<ModulesPermissionsResult> getModulesPermissionsbyRoleId(string roleId,int first, int rows, SortSearchVM sortSearchObj)
        {
            ModulesPermissionsResult mainClass = new ModulesPermissionsResult();
            var query = _context.Modules
               .Include(m => m.RoleModulePermissions).ThenInclude(r=>r.Permission).Where(m => m.RoleModulePermissions.Any(rp => rp.RoleId == roleId))
                .AsQueryable();

            if(!string.IsNullOrEmpty(sortSearchObj.search))
            {
                query = query.Where(m=> m.Name== sortSearchObj.search || m.NameAr== sortSearchObj.search);
            }
            if (sortSearchObj.SortField == "name")
            {
                query = sortSearchObj.SortOrder == 1 ? query.OrderBy(m => m.Name) : query.OrderByDescending(m => m.NameAr);
            }
         
            mainClass.count = await query.CountAsync();

            mainClass.results = await query
                .Skip(first)
                .Take(rows)
                .Select(m => new ModuleWithPermissionsVM()
                {
                    
                    name = m.Name,
                    nameAr = m.NameAr,
                    Permissions = m.RoleModulePermissions.Where(r=>r.RoleId==roleId).Select(r=> new permissionVM() { name=r.Permission.Name })
                }).ToListAsync();
            return mainClass;
        }
        public async Task<ModulesPermissionsWithSelectedPermissionIDsResult> getModulesPermissionsbyRoleIdForEdit(string roleId, int first, int rows, SortSearchVM sortSearchObj)
        {
            ModulesPermissionsWithSelectedPermissionIDsResult mainClass = new ModulesPermissionsWithSelectedPermissionIDsResult();
            var query = _context.Modules.
               Include(m => m.Permissions).Include(m => m.RoleModulePermissions)
                .AsQueryable();

            if (!string.IsNullOrEmpty(sortSearchObj.search))
            {
                query = query.Where(m => m.Name == sortSearchObj.search || m.NameAr == sortSearchObj.search);
            }
            if (sortSearchObj.SortField == "name")
            {
                query = sortSearchObj.SortOrder == 1 ? query.OrderBy(m => m.Name) : query.OrderByDescending(m => m.NameAr);
            }

            mainClass.count = await query.CountAsync();

            mainClass.results = await query
                .Skip(first)
                .Take(rows)
                .Select(m => new ModulePermissionsWithSelectedPermissionIdsVM() { id = m.Id, name = m.Name, nameAr = m.NameAr, Permissions = m.Permissions.Select(p => new permissionVM() { id = p.Id, name = p.Name }), selectedPemrissionIDs = m.RoleModulePermissions.Where(r => r.RoleId == roleId).Select(p => p.PermissionId).ToList() }).ToListAsync();
            return mainClass;
        }

        public async Task<bool> IsRoleAssignedToUsers(string RoleId)
        {
            return await _context.UserRoles.AnyAsync(u=>u.RoleId==RoleId);
        }
        public async Task DeleteRole(string RoleId)
        {
            var Role=await _context.Roles.FindAsync(RoleId);
             _context.Roles.Remove(Role);
            
            await _context.SaveChangesAsync();
        }
    }
}

