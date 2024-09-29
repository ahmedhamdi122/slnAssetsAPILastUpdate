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
            // return await _context.ApplicationRole.AnyAsync(r=>r.RoleCategoryId==id);
            return await _context.ApplicationRole.AnyAsync(r=> r.RoleCategoryId == id);
             
        }
    }
}

