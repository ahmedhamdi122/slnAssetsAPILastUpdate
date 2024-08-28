﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Asset.Models;
using Asset.ViewModels.HospitalVM;


namespace Asset.Domain.Services
{
  public  interface IHospitalService
    {
        IEnumerable<Hospital> GetAllHospitals();
        IEnumerable<IndexHospitalVM.GetData> GetAll(); 
        IndexHospitalVM GetTop10Hospitals(int hospitalId);
        EditHospitalVM GetById(int id);
        DetailHospitalVM GetHospitalDetailById(int id);
        IEnumerable<IndexHospitalVM.GetData> GetHospitalsByUserId(string userId);
        IEnumerable<IndexHospitalVM.GetData> SearchHospitals(SearchHospitalVM searchObj);
        IEnumerable<Hospital> GetHospitalsByCityId(int cityId);
        IEnumerable<Hospital> GetHospitalsBySubOrganizationId(int subOrgId);
        IEnumerable<IndexHospitalVM.GetData> GetHospitalsByGovCityOrgSubOrgId(int govId, int cityId, int orgId, int subOrgId);
        List<SubOrganization> GetSubOrganizationsByHospitalId(int hospitalId);
        int Add(CreateHospitalVM Hospital); 
        int Update(EditHospitalVM Hospital);
        int UpdateHospitalDepartment(EditHospitalDepartmentVM hospitalDepartmentVM);
        List<HospitalDepartment> GetHospitalDepartmentByHospitalId(int hospitalId);
        List<IndexHospitalDepartmentVM.GetData> GetHospitalDepartmentByHospitalId2(int hospitalId);
        IndexHospitalDepartmentVM.GetData GetSelectedHospitalDepartmentByDepartmentId(int hospitalId,int departmentId);
        List<CountHospitalVM> CountHospitalsByCities();
        int CountHospitals();
        int Delete(int id);
        IEnumerable<IndexHospitalVM.GetData> SortHospitals(SortVM sortObj);
        public IEnumerable<HospitalWithAssetVM> GetHospitalsWithAssets();
        int CountDepartmentsByHospitalId(int hospitalId);
        IEnumerable<Hospital> GetHospitalByGovId(int govId);

        GenerateHospitalCodeVM GenerateHospitalCode();
    }
}
