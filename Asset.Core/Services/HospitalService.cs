﻿using Asset.Domain;
using Asset.Domain.Services;
using Asset.Models;
using Asset.ViewModels.HospitalVM;
using Asset.ViewModels.RoleCategoryVM;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Asset.Core.Services
{
    public class HospitalService : IHospitalService
    {

        private IUnitOfWork _unitOfWork;

        public HospitalService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        public int Add(CreateHospitalVM HospitalVM)
        {
            _unitOfWork.HospitalRepository.Add(HospitalVM);
            return _unitOfWork.CommitAsync();
        }

        public int CountHospitals()
        {
            return _unitOfWork.HospitalRepository.CountHospitals();
        }

        public List<CountHospitalVM> CountHospitalsByCities()
        {
            return _unitOfWork.HospitalRepository.CountHospitalsByCities();


        }

        public int Delete(int id)
        {
            var HospitalObj = _unitOfWork.HospitalRepository.GetById(id);
            _unitOfWork.HospitalRepository.Delete(HospitalObj.Id);
            _unitOfWork.CommitAsync();

            return HospitalObj.Id;
        }

        public IEnumerable<IndexHospitalVM.GetData> GetAll()
        {
            return _unitOfWork.HospitalRepository.GetAll();
        }

        public IEnumerable<Hospital> GetAllHospitals()
        {
            return _unitOfWork.HospitalRepository.GetAllHospitals();
        }

        public EditHospitalVM GetById(int id)
        {
            return _unitOfWork.HospitalRepository.GetById(id);
        }

        public List<HospitalDepartment> GetHospitalDepartmentByHospitalId(int hospitalId)
        {
            return _unitOfWork.HospitalRepository.GetHospitalDepartmentByHospitalId(hospitalId);
        }

        public List<IndexHospitalDepartmentVM.GetData> GetHospitalDepartmentByHospitalId2(int hospitalId)
        {
            return _unitOfWork.HospitalRepository.GetHospitalDepartmentByHospitalId2(hospitalId);
        }

        public DetailHospitalVM GetHospitalDetailById(int id)
        {
            return _unitOfWork.HospitalRepository.GetHospitalDetailById(id);
        }

        public IEnumerable<Hospital> GetHospitalsByCityId(int cityId)
        {
            return _unitOfWork.HospitalRepository.GetHospitalsByCityId(cityId);
        }

        public IEnumerable<Hospital> GetHospitalsBySubOrganizationId(int subOrgId)
        {
            return _unitOfWork.HospitalRepository.GetHospitalsBySubOrganizationId(subOrgId);
        }


        public IEnumerable<IndexHospitalVM.GetData> GetHospitalsByUserId(string userId)
        {
            return _unitOfWork.HospitalRepository.GetHospitalsByUserId(userId);
        }

        public List<SubOrganization> GetSubOrganizationsByHospitalId(int hospitalId)
        {
            return _unitOfWork.HospitalRepository.GetSubOrganizationsByHospitalId(hospitalId);
        }

        public IndexHospitalVM GetTop10Hospitals(int hospitalId)
        {
            return _unitOfWork.HospitalRepository.GetTop10Hospitals(hospitalId);
        }

        public IEnumerable<IndexHospitalVM.GetData> SearchHospitals(SearchHospitalVM searchObj)
        {
            return _unitOfWork.HospitalRepository.SearchHospitals(searchObj);
        }

        public IEnumerable<IndexHospitalVM.GetData> SortHospitals(SortVM sortObj)
        {
            return _unitOfWork.HospitalRepository.SortHospitals(sortObj);
        }
        public int Update(EditHospitalVM HospitalVM)
        {
            _unitOfWork.HospitalRepository.Update(HospitalVM);
            _unitOfWork.CommitAsync();
            return HospitalVM.Id;
        }

        public int UpdateHospitalDepartment(EditHospitalDepartmentVM hospitalDepartmentVM)
        {
            return _unitOfWork.HospitalRepository.UpdateHospitalDepartment(hospitalDepartmentVM);
        }
        public IEnumerable<HospitalWithAssetVM> GetHospitalsWithAssets()
        {
            return _unitOfWork.HospitalRepository.GetHospitalsWithAssets();
        }

        public int CountDepartmentsByHospitalId(int hospitalId)
        {
            return _unitOfWork.HospitalRepository.CountDepartmentsByHospitalId(hospitalId);
        }

        public IndexHospitalDepartmentVM.GetData GetSelectedHospitalDepartmentByDepartmentId(int hospitalId, int departmentId)
        {
            return _unitOfWork.HospitalRepository.GetSelectedHospitalDepartmentByDepartmentId(hospitalId, departmentId);
        }


        public IEnumerable<Hospital> GetHospitalByGovId(int govId)
        {
            return _unitOfWork.HospitalRepository.GetHospitalByGovId(govId);
        }

        public GenerateHospitalCodeVM GenerateHospitalCode()
        {
            return _unitOfWork.HospitalRepository.GenerateHospitalCode();
        }

        public IEnumerable<IndexHospitalVM.GetData> GetHospitalsByGovCityOrgSubOrgId(int govId, int cityId, int orgId, int subOrgId)
        {
            return _unitOfWork.HospitalRepository.GetHospitalsByGovCityOrgSubOrgId(govId, cityId, orgId, subOrgId);
        }
    }
}
