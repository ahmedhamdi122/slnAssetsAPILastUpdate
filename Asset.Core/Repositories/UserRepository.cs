using Asset.Domain.Repositories;
using Asset.Models;
using Asset.ViewModels.RoleCategoryVM;
using Asset.ViewModels.RoleVM;
using Asset.ViewModels.UserVM;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset.Core.Repositories
{
    public class UserRepository : IUserRepository
    {
        private ApplicationDbContext _context;
        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<UserResultVM> GetAll(int first, int rows, string SortField, int SortOrder, string search)
        {
            UserResultVM UserResult = new UserResultVM();
            List<UserVM> list = new List<UserVM>();
            var query = _context.Users.OrderBy((rc) => rc.Id).Select(  user =>
                new UserVM()
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    RoleCategory =  _context.RoleCategories.Select(rc => new ReadRoleCategoryVM() { Id = rc.Id, Name = rc.Name, NameAr = rc.NameAr }).FirstOrDefault(rc => rc.Id == user.RoleCategoryId),
                    DisplayName=user.UserName,
                    PhoneNumber=user.PhoneNumber,
                    Email=user.Email,
                }).AsQueryable();



            if (SortField == "UserName")
            {
                if (SortOrder == 1)
                    query = query.OrderBy(d => d.UserName);
                else
                    query = query.OrderByDescending(d => d.UserName);

            }
            if (SortField == "RoleCategoryName")
            {
                if (SortOrder == 1)
                    query = query.OrderBy(d => d.RoleCategory.Name);
                else
                    query = query.OrderByDescending(d => d.RoleCategory.Name);
            }
            if (SortField == "RoleCategoryNameAr")
            {
                if (SortOrder == 1)
                    query = query.OrderBy(d => d.RoleCategory.NameAr);
                else
                    query = query.OrderByDescending(d => d.RoleCategory.NameAr);

            }
            if (SortField == "DisplayName")
            {
                if (SortOrder == 1)
                    query = query.OrderBy(d => d.DisplayName);
                else
                    query = query.OrderByDescending(d => d.DisplayName);

            }
            if (SortField == "PhoneNumber")
            {
                if (SortOrder == 1)
                    query = query.OrderBy(d => d.PhoneNumber);
                else
                    query = query.OrderByDescending(d => d.PhoneNumber);

            }
            if (SortField == "Email")
            {
                if (SortOrder == 1)
                    query = query.OrderBy(d => d.Email);
                else
                    query = query.OrderByDescending(d => d.Email);

            }
            UserResult.count = query.Count();
            UserResult.results = await query.Skip(first).Take(rows).ToListAsync();
            return UserResult;
        }
    }
}
