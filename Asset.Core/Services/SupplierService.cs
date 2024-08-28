﻿using Asset.Domain;
using Asset.Domain.Services;
using Asset.Models;
using Asset.ViewModels.SupplierVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset.Core.Services
{
   public class SupplierService: ISupplierService
    {
        private IUnitOfWork _unitOfWork;

        public SupplierService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public int Add(CreateSupplierVM supplierObj)
        {
            return _unitOfWork.SupplierRepository.Add(supplierObj);
           // return _unitOfWork.CommitAsync();
        }

        public int CountSuppliers()
        {
            return _unitOfWork.SupplierRepository.CountSuppliers();
        }

        public int Delete(int id)
        {
            var supplierObj = _unitOfWork.SupplierRepository.GetById(id);
            _unitOfWork.SupplierRepository.Delete(supplierObj.Id);
            _unitOfWork.CommitAsync();
            return supplierObj.Id;
        }

        public IEnumerable<IndexSupplierVM.GetData> GetAll()
        {
            return _unitOfWork.SupplierRepository.GetAll();
        }

        public IEnumerable<Supplier> GetAllSuppliers()
        {
            return _unitOfWork.SupplierRepository.GetAllSuppliers();
        }

        public EditSupplierVM GetById(int id)
        {
            return _unitOfWork.SupplierRepository.GetById(id);
        }

        public IEnumerable<IndexSupplierVM.GetData> GetSupplierByName(string supplierName)
        {
            return _unitOfWork.SupplierRepository.GetSupplierByName(supplierName);
        }

        public IEnumerable<IndexSupplierVM.GetData> GetTop10Suppliers(int hospitalId)
        {
            return _unitOfWork.SupplierRepository.GetTop10Suppliers(hospitalId);
        }

        public int Update(EditSupplierVM supplierObj)
        {
            _unitOfWork.SupplierRepository.Update(supplierObj);
            _unitOfWork.CommitAsync();
            return supplierObj.Id;
        }

        public IEnumerable<IndexSupplierVM.GetData> SortSuppliers(SortSupplierVM sortObj)
        {
            return _unitOfWork.SupplierRepository.SortSuppliers(sortObj);
        }

        public IndexSupplierVM FindSupplier(string strText, int pageNumber, int pageSize)
        {
            return _unitOfWork.SupplierRepository.FindSupplier(strText,pageNumber,pageSize);
        }

        public IEnumerable<IndexSupplierVM.GetData> FindSupplierByText(string strText)
        {
            return _unitOfWork.SupplierRepository.FindSupplierByText(strText);
        }

        public IndexSupplierVM GetAllSuppliersWithPaging(int pageNumber, int pageSize)
        {
            return _unitOfWork.SupplierRepository.GetAllSuppliersWithPaging(pageNumber, pageSize);
        }

        public int CreateSupplierAttachment(SupplierAttachment attachObj)
        {
            return _unitOfWork.SupplierRepository.CreateSupplierAttachment(attachObj);
        }

        public List<SupplierAttachment> GetSupplierAttachmentsBySupplierId(int supplierId)
        {
            return _unitOfWork.SupplierRepository.GetSupplierAttachmentsBySupplierId(supplierId);
        }

        public GenerateSupplierCodeVM GenerateSupplierCode()
        {
            return _unitOfWork.SupplierRepository.GenerateSupplierCode();
        }

        public SupplierAttachment GetLastDocumentForSupplierId(int supplierId)
        {
            return _unitOfWork.SupplierRepository.GetLastDocumentForSupplierId(supplierId);
        }

        public int DeleteSupplierAttachment(int id)
        {
            return _unitOfWork.SupplierRepository.DeleteSupplierAttachment(id);
        }
    }
}
