using Asset.Domain.Repositories;
using Asset.Models;
using Asset.ViewModels.RoleCategoryVM;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

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




        public IEnumerable<IndexCategoryVM.GetData> GetAll()
        {
            List<IndexCategoryVM.GetData> lstRoleCategoriesVM = new List<IndexCategoryVM.GetData>();
            var lstRoleCategories = _context.RoleCategories.OrderBy(a => a.OrderId).ToList().Select(item => new IndexCategoryVM.GetData
            {
                Id = item.Id,
                Name = item.Name,
                NameAr = item.NameAr,
                OrderId = item.OrderId
            });

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

        public IndexCategoryVM SortRoleCategories(int pagenumber, int pagesize, SortRoleCategoryVM sortObj)
        {
            IndexCategoryVM mainClass = new IndexCategoryVM();
            List<IndexCategoryVM.GetData> list = new List<IndexCategoryVM.GetData>();
            IQueryable<RoleCategory> roleCategories = _context.RoleCategories;
            List<RoleCategory> lstRoleCategories = _context.RoleCategories.ToList();
            foreach (var item in lstRoleCategories)
            {
                IndexCategoryVM.GetData Assetobj = new IndexCategoryVM.GetData();
                Assetobj.Id = item.Id;
                Assetobj.Name = item.Name;
                Assetobj.NameAr = item.NameAr;
                Assetobj.OrderId = item.OrderId;
                list.Add(Assetobj);
            }

            if (sortObj.Name != "")
            {
                if (sortObj.SortStatus == "descending")
                    list = list.OrderByDescending(d => d.Name).ToList();
                else
                    list = list.OrderBy(d => d.Name).ToList();
            }
            if (sortObj.NameAr != "")
            {
                if (sortObj.SortStatus == "descending")
                    list = list.OrderByDescending(d => d.NameAr).ToList();
                else
                    list = list.OrderBy(d => d.NameAr).ToList();
            }
            if (sortObj.OrderId != "0")
            {
                if (sortObj.SortStatus == "descending")
                    list = list.OrderByDescending(d => d.OrderId).ToList();
                else
                    list = list.OrderBy(d => d.OrderId).ToList();
            }
            if (sortObj.Id != "0")
            {
                if (sortObj.SortStatus == "descending")
                    list = list.OrderByDescending(d => d.Id).ToList();
                else
                    list = list.OrderBy(d => d.Id).ToList();
            }

            var itemsPerPage = list.Skip((pagenumber - 1) * pagesize).Take(pagesize).ToList();
            mainClass.Results = itemsPerPage;
            mainClass.Count = list.Count;
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