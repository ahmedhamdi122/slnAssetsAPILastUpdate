﻿using Asset.Domain;
using Asset.Domain.Services;
using Asset.Models;
using Asset.ViewModels.BrandVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset.Core.Services
{
    public class BrandService : IBrandService
    {
        private IUnitOfWork _unitOfWork;

        public BrandService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public int Add(CreateBrandVM brandObj)
        {
            return _unitOfWork.BrandRepository.Add(brandObj);

        }

        public IEnumerable<IndexBrandVM.GetData> AutoCompleteBrandName(string brandName)
        {
            return _unitOfWork.BrandRepository.AutoCompleteBrandName(brandName);
        }

        public int CountBrands()
        {
            return _unitOfWork.BrandRepository.CountBrands();
        }

        public int Delete(int id)
        {
            var brandObj = _unitOfWork.BrandRepository.GetById(id);
            _unitOfWork.BrandRepository.Delete(brandObj.Id);
            _unitOfWork.CommitAsync();

            return brandObj.Id;
        }

        public GenerateBrandCodeVM GenerateBrandCode()
        {
            return _unitOfWork.BrandRepository.GenerateBrandCode();
        }

        public IEnumerable<IndexBrandVM.GetData> GetAll()
        {
            return _unitOfWork.BrandRepository.GetAll();
        }

        public IEnumerable<Brand> GetAllBrands()
        {
            return _unitOfWork.BrandRepository.GetAllBrands();
        }

        public IEnumerable<IndexBrandVM.GetData> GetBrandByName(string brandName)
        {
            return _unitOfWork.BrandRepository.GetBrandByName(brandName);
        }

        public EditBrandVM GetById(int id)
        {
            return _unitOfWork.BrandRepository.GetById(id);
        }

        public IndexBrandVM GetTop10Brands(int hospitalId)
        {
            return _unitOfWork.BrandRepository.GetTop10Brands(hospitalId);
        }

        public IndexBrandVM ListBrands(SortAndFilterBrandVM data, int pageNumber, int pageSize)
        {
            return _unitOfWork.BrandRepository.ListBrands(data, pageNumber, pageSize);
        }

        public IEnumerable<IndexBrandVM.GetData> SortBrands(SortBrandVM sortObj)
        {
            return _unitOfWork.BrandRepository.SortBrands(sortObj);
        }

        public int Update(EditBrandVM brandObj)
        {
            _unitOfWork.BrandRepository.Update(brandObj);
            _unitOfWork.CommitAsync();
            return brandObj.Id;
        }
    }
}
