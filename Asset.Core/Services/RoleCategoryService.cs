﻿using Asset.Domain;
using Asset.Domain.Services;
using Asset.Models;
using Asset.ViewModels.RoleCategoryVM;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace Asset.Core.Services
{
    public class RoleCategoryService : IRoleCategoryService

    {
        private IUnitOfWork _unitOfWork;

        public RoleCategoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public int Add(CreateRoleCategory roleCategory)
        {
            _unitOfWork.RoleCategory.Add(roleCategory);
            return _unitOfWork.CommitAsync();

        }

        public int Delete(int id)
        {
            var roleCategory = _unitOfWork.RoleCategory.GetById(id);
            _unitOfWork.RoleCategory.Delete(roleCategory);
            _unitOfWork.CommitAsync();

            return roleCategory.Id;
        }

        public RoleCategory GetById(int id)
        {
            return _unitOfWork.RoleCategory.GetById(id);
        }

        public async Task<IEnumerable<IndexCategoryVM.GetData>> GetAll()
        {
            return await _unitOfWork.RoleCategory.GetAll();
        }


        public int Update(EditRoleCategory roleCategory)
        {
            _unitOfWork.RoleCategory.Update(roleCategory);
            _unitOfWork.CommitAsync();
            return roleCategory.Id;
        }

        public async Task<IndexCategoryVM> LoadRoleCategories(int first, int rows, string SortField, int SortOrder)
        {
           return await _unitOfWork.RoleCategory.LoadRoleCategories( first,rows, SortField,  SortOrder);
        }
        
        public GenerateRoleCategoryOrderVM GenerateRoleCategoryOrderId()
        {
            return _unitOfWork.RoleCategory.GenerateRoleCategoryOrderId();
        }
    }
}
