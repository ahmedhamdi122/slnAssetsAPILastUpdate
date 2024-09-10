using Asset.Domain.Repositories;
using Asset.Models;
using Asset.ViewModels.RoleCategoryVM;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Asset.Core.Repositories
{
    public class RoleCategoryRepositories : IRoleCategoryRepository
    {
        private ApplicationDbContext _context;
        string msg;

        public RoleCategoryRepositories(ApplicationDbContext context)
        {
            _context = context;
        }


        public RoleCategory GetById(int id)
        {
            var roleCategoryObj = _context.RoleCategories.Find(id);

            if (roleCategoryObj == null)
            {
                return null;
            }
            return roleCategoryObj;
        }




        public async Task<IEnumerable<IndexCategoryVM.GetData>> GetAll()
        {
            List<IndexCategoryVM.GetData> lstRoleCategoriesVM = new List<IndexCategoryVM.GetData>();
            var lstRoleCategories = await _context.RoleCategories.OrderBy(a => a.OrderId).Select(item => new IndexCategoryVM.GetData
            {
                Id = item.Id,
                Name = item.Name,
                NameAr = item.NameAr,
                OrderId = item.OrderId
            }).ToListAsync();

            return lstRoleCategories;
        }

        public int Add(CreateRoleCategory roleCategory)
        {
            RoleCategory roleCategoryObj = new RoleCategory();
            try
            {

                if (roleCategory != null)
                {
                    roleCategoryObj.Name = roleCategory.Name;
                    roleCategoryObj.NameAr = roleCategory.NameAr;
                    roleCategoryObj.OrderId = roleCategory.OrderId;
                    _context.RoleCategories.Add(roleCategoryObj);
                    _context.SaveChanges();

                }
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }
            return roleCategoryObj.Id;
        }

        public int Delete(RoleCategory roleCategory)
        {
            var roleCategoryObj = _context.RoleCategories.Find(roleCategory.Id);
            try
            {
                if (roleCategoryObj != null)
                {
                    _context.RoleCategories.Remove(roleCategory);
                    return _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }

            return 0;
        }

        public int Update(EditRoleCategory roleCategory)
        {
            try
            {
                var roleCategoryObj = _context.RoleCategories.Find(roleCategory.Id);
                roleCategoryObj.Id = roleCategory.Id;
                roleCategoryObj.Name = roleCategory.Name;
                roleCategoryObj.NameAr = roleCategory.NameAr;
                roleCategoryObj.OrderId = roleCategory.OrderId;
                _context.Entry(roleCategoryObj).State = EntityState.Modified;
                _context.SaveChanges();
                return roleCategory.Id;


            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }

            return 0;
        }

        public async Task<IndexCategoryVM> LoadRoleCategories(int first, int rows, string SortField, int SortOrder)
        {
            IndexCategoryVM mainClass = new IndexCategoryVM();
            List<IndexCategoryVM.GetData> list = new List<IndexCategoryVM.GetData>();
            var query = _context.RoleCategories.OrderBy((rc) =>rc.Id).Select(roleCtegory =>
                new IndexCategoryVM.GetData()
                {
                    Id = roleCtegory.Id,
                    Name = roleCtegory.Name,
                    NameAr = roleCtegory.NameAr,
                    OrderId = roleCtegory.OrderId
                }).AsQueryable();
       
      

            if (SortField=="Name")
            {
                if (SortOrder ==1)
                    query = query.OrderBy(d => d.Name);
                else
                    query = query.OrderByDescending(d => d.Name);

            }
            if (SortField == "NameAr")
            {
                if (SortOrder == 1)
                    query = query.OrderBy(d => d.NameAr);
                else
                    query = query.OrderByDescending(d => d.NameAr);
            }
            if (SortField== "OrderId")
            {
                if (SortOrder == 1)
                    query = query.OrderBy(d => d.OrderId);
                else
                    query = query.OrderByDescending(d => d.OrderId);

            }
            mainClass.Count = query.Count();
            var itemsPerPage = await query.Skip(first).Take(rows).ToListAsync();
            mainClass.Results = itemsPerPage;
            return mainClass;
        }

        public GenerateRoleCategoryOrderVM GenerateRoleCategoryOrderId()
        {
            GenerateRoleCategoryOrderVM numberObj = new GenerateRoleCategoryOrderVM();
            int orderId = 0;

            var lastId = _context.RoleCategories.ToList();
            if (lastId.Count > 0)
            {
                var code = lastId.Max(a => a.OrderId);
                var order = (code + 1);
                numberObj.OrderId = order;
            }
            else
            {
                numberObj.OrderId = (orderId + 1);
            }

            return numberObj;
        }
    }
}