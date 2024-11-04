
using Asset.Domain;
using Asset.Domain.Services;
using Asset.Models;
using Asset.ViewModels.AssetDetailAttachmentVM;
using Asset.ViewModels.AssetDetailVM;
using Asset.ViewModels.BrandVM;
using Asset.ViewModels.CityVM;
using Asset.ViewModels.GovernorateVM;
using Asset.ViewModels.HospitalVM;
using Asset.ViewModels.OrganizationVM;
using Asset.ViewModels.PMAssetTaskScheduleVM;
using Asset.ViewModels.PmAssetTimeVM;
using Asset.ViewModels.RequestVM;
using Asset.ViewModels.SupplierVM;
using Asset.ViewModels.WorkOrderVM;
using Org.BouncyCastle.Crypto;
using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Linq;
using System.Threading.Tasks;

namespace Asset.Core.Services
{
    public class AssetDetailService : IAssetDetailService
    {
        private IUnitOfWork _unitOfWork;
        public bool hasAssetWithMasterId(int id)
        {
            return _unitOfWork.AssetDetailRepository.hasAssetWithMasterId(id);
        }
        public AssetDetailService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public bool CheckAssetDetailCodeExists(string code)
        {
            return _unitOfWork.AssetDetailRepository.CheckAssetDetailCodeExists(code);
        }
        public int Add(CreateAssetDetailVM assetDetailObj)
        {
            return _unitOfWork.AssetDetailRepository.Add(assetDetailObj);
            //  return _unitOfWork.CommitAsync();

        }
        public AssetDetail QueryAssetDetailById(int assetId)
        {
            return _unitOfWork.AssetDetailRepository.QueryAssetDetailById(assetId);
        }
        public int Delete(int id)
        {
            var assetDetailObj = _unitOfWork.AssetDetailRepository.GetById(id);
            _unitOfWork.AssetDetailRepository.Delete(assetDetailObj.Id);
            _unitOfWork.CommitAsync();

            return assetDetailObj.Id;
        }

        public IEnumerable<IndexAssetDetailVM.GetData> GetAll()
        {
            return _unitOfWork.AssetDetailRepository.GetAll();
        }

        public IEnumerable<IndexAssetDetailVM.GetData> GetAssetDetailsByAssetId(int assetId)
        {
            return _unitOfWork.AssetDetailRepository.GetAssetDetailsByAssetId(assetId);
        }


        public EditAssetDetailVM GetById(int id)
        {
            return _unitOfWork.AssetDetailRepository.GetById(id);
        }

        public int Update(EditAssetDetailVM assetDetailObj)
        {
            _unitOfWork.AssetDetailRepository.Update(assetDetailObj);
            return assetDetailObj.Id;
        }

        public int DeleteAssetDetailAttachment(int id)
        {
            return _unitOfWork.AssetDetailRepository.DeleteAssetDetailAttachment(id);
        }

        public IEnumerable<AssetDetailAttachment> GetAttachmentByAssetDetailId(int assetId)
        {
            return _unitOfWork.AssetDetailRepository.GetAttachmentByAssetDetailId(assetId);
        }

        public int CreateAssetDetailDocuments(CreateAssetDetailAttachmentVM attachObj)
        {
            return _unitOfWork.AssetDetailRepository.CreateAssetDetailDocuments(attachObj);
        }

        public ViewAssetDetailVM ViewAssetDetailByMasterId(int masterId)
        {
            return _unitOfWork.AssetDetailRepository.ViewAssetDetailByMasterId(masterId);

        }


        public IEnumerable<IndexPMAssetTaskScheduleVM.GetData> GetAllPMAssetTaskSchedules(int? hospitalId)
        {
            return _unitOfWork.AssetDetailRepository.GetAllPMAssetTaskSchedules(hospitalId);
        }


        public IEnumerable<AssetDetail> ViewAllAssetDetailByMasterId(int MasterAssetId)
        {
            return _unitOfWork.AssetDetailRepository.ViewAllAssetDetailByMasterId(MasterAssetId);
        }



        public IEnumerable<AssetDetail> GetAllAssetDetailsByHospitalId(int hospitalId)
        {
            return _unitOfWork.AssetDetailRepository.GetAllAssetDetailsByHospitalId(hospitalId);

        }


        public List<CountAssetVM> CountAssetsByHospital()
        {
            return _unitOfWork.AssetDetailRepository.CountAssetsByHospital();
        }

        public List<PmDateGroupVM> GetAllwithgrouping(int assetId)
        {
            return _unitOfWork.AssetDetailRepository.GetAllwithgrouping(assetId);
        }
        public List<IndexAssetDetailVM.GetData> FilterAsset(filterDto data)
        {
            return _unitOfWork.AssetDetailRepository.FilterAsset(data);
        }

        public List<BrandGroupVM> GetAssetByBrands(List<IndexAssetDetailVM.GetData> AssetModel)
        {
            return _unitOfWork.AssetDetailRepository.GetAssetByBrands(AssetModel);
        }

        public List<GroupHospitalVM> GetAssetByHospital(List<IndexAssetDetailVM.GetData> AssetModel)
        {
            return _unitOfWork.AssetDetailRepository.GetAssetByHospital(AssetModel);
        }

        public List<GroupGovernorateVM> GetAssetByGovernorate(List<IndexAssetDetailVM.GetData> AssetModel)
        {
            return _unitOfWork.AssetDetailRepository.GetAssetByGovernorate(AssetModel);
        }
        public List<GroupCityVM> GetAssetByCity(List<IndexAssetDetailVM.GetData> AssetModel)
        {
            return _unitOfWork.AssetDetailRepository.GetAssetByCity(AssetModel);
        }
        public List<GroupSupplierVM> GetAssetBySupplier(List<IndexAssetDetailVM.GetData> AssetModel)
        {
            return _unitOfWork.AssetDetailRepository.GetAssetBySupplier(AssetModel);
        }
        public List<GroupOrganizationVM> GetAssetByOrganization(List<IndexAssetDetailVM.GetData> AssetModel)
        {
            return _unitOfWork.AssetDetailRepository.GetAssetByOrganization(AssetModel);
        }




        public List<HospitalAssetAge> GetAssetsByAgeGroup(int hospitalId)
        {
            return _unitOfWork.AssetDetailRepository.GetAssetsByAgeGroup(hospitalId);
        }

        public List<HospitalAssetAge> GetGeneralAssetsByAgeGroup(FilterHospitalAssetAge model)
        {
            return _unitOfWork.AssetDetailRepository.GetGeneralAssetsByAgeGroup(model);
        }

        public IEnumerable<IndexAssetDetailVM.GetData> AutoCompleteAssetBarCode(string barcode, int hospitalId)
        {
            return _unitOfWork.AssetDetailRepository.AutoCompleteAssetBarCode(barcode, hospitalId);
        }



        public IEnumerable<IndexAssetDetailVM.GetData> AutoCompleteAssetSerial(string serial, int hospitalId)
        {
            return _unitOfWork.AssetDetailRepository.AutoCompleteAssetSerial(serial, hospitalId);
        }

        public IEnumerable<ViewAssetDetailVM> GetListOfAssetDetailsByHospitalNotInContract(int hospitalId)
        {
            return _unitOfWork.AssetDetailRepository.GetListOfAssetDetailsByHospitalNotInContract(hospitalId);
        }

        public IEnumerable<ViewAssetDetailVM> GetNoneExcludedAssetsByHospitalId(int hospitalId)
        {
            return _unitOfWork.AssetDetailRepository.GetNoneExcludedAssetsByHospitalId(hospitalId);
        }

        public IEnumerable<ViewAssetDetailVM> GetSupplierNoneExcludedAssetsByHospitalId(int hospitalId)
        {
            return _unitOfWork.AssetDetailRepository.GetSupplierNoneExcludedAssetsByHospitalId(hospitalId);
        }


        public AssetDetailAttachment GetLastDocumentForAssetDetailId(int assetDetailId)
        {
            return _unitOfWork.AssetDetailRepository.GetLastDocumentForAssetDetailId(assetDetailId);
        }

        public IEnumerable<ViewAssetDetailVM> GetAutoCompleteSupplierNoneExcludedAssetsByHospitalId(string barcode, int hospitalId)
        {
            return _unitOfWork.AssetDetailRepository.GetAutoCompleteSupplierNoneExcludedAssetsByHospitalId(barcode, hospitalId);
        }

        public int CountAssetsByHospitalId(int hospitalId)
        {
            return _unitOfWork.AssetDetailRepository.CountAssetsByHospitalId(hospitalId);
        }

        public List<CountAssetVM> ListTopAssetsByHospitalId(int hospitalId)
        {
            return _unitOfWork.AssetDetailRepository.ListTopAssetsByHospitalId(hospitalId);
        }

        public List<CountAssetVM> ListAssetsByGovernorateIds()
        {
            return _unitOfWork.AssetDetailRepository.ListAssetsByGovernorateIds();
        }

        public List<CountAssetVM> ListAssetsByCityIds()
        {
            return _unitOfWork.AssetDetailRepository.ListAssetsByCityIds();
        }

        public List<CountAssetVM> CountAssetsInHospitalByHospitalId(int hospitalId)
        {
            return _unitOfWork.AssetDetailRepository.CountAssetsInHospitalByHospitalId(hospitalId);
        }


        public IndexAssetDetailVM GetAllAssetsByStatusId(int pageNumber, int pageSize, int statusId, string userId)
        {
            return _unitOfWork.AssetDetailRepository.GetAllAssetsByStatusId(pageNumber, pageSize, statusId, userId);
        }



        public IndexAssetDetailVM SearchHospitalAssetsByDepartmentId(int departmentId, string userId, int pageNumber, int pageSize)
        {
            return _unitOfWork.AssetDetailRepository.SearchHospitalAssetsByDepartmentId(departmentId, userId, pageNumber, pageSize);
        }

        public ViewAssetDetailVM GetAssetHistoryById(int assetId)
        {
            return _unitOfWork.AssetDetailRepository.GetAssetHistoryById(assetId);
        }

        public List<IndexAssetDetailVM.GetData> FilterDataByDepartmentBrandSupplierId(FilterHospitalAsset data)
        {
            return _unitOfWork.AssetDetailRepository.FilterDataByDepartmentBrandSupplierId(data);
        }

        public List<DepartmentGroupVM> GetAssetByDepartment(List<IndexAssetDetailVM.GetData> AssetModel)
        {
            return _unitOfWork.AssetDetailRepository.GetAssetByDepartment(AssetModel);
        }



        public IEnumerable<ViewAssetDetailVM> GetListOfAssetDetailsByHospitalNotInContract(string barcode, int hospitalId)
        {
            return _unitOfWork.AssetDetailRepository.GetListOfAssetDetailsByHospitalNotInContract(barcode, hospitalId);
        }

        public IEnumerable<ViewAssetDetailVM> GetListOfAssetDetailsByHospitalNotInContractBySerialNumber(string serialNumber, int hospitalId)
        {
            return _unitOfWork.AssetDetailRepository.GetListOfAssetDetailsByHospitalNotInContractBySerialNumber(serialNumber, hospitalId);
        }

        public GeneratedAssetDetailBCVM GenerateAssetDetailBarcode()
        {
            return _unitOfWork.AssetDetailRepository.GenerateAssetDetailBarcode();
        }

        public IEnumerable<IndexPMAssetTaskScheduleVM.GetData> GetAllPMAssetTaskScheduleByAssetId(int? assetId)
        {
            return _unitOfWork.AssetDetailRepository.GetAllPMAssetTaskScheduleByAssetId(assetId);

        }

        public MobileAssetDetailVM GetAssetDetailById(string userId, int assetId)
        {
            return _unitOfWork.AssetDetailRepository.GetAssetDetailById(userId, assetId);
        }

        public bool GenerateQrCodeForAllAssets(string domainName)
        {
            return _unitOfWork.AssetDetailRepository.GenerateQrCodeForAllAssets(domainName);
        }

        public IndexAssetDetailVM MobSearchAssetInHospital(SearchMasterAssetVM searchObj, int pageNumber, int pageSize)
        {
            return _unitOfWork.AssetDetailRepository.MobSearchAssetInHospital(searchObj, pageNumber, pageSize);
        }

        public IndexAssetDetailVM AlertAssetsWarrantyEndBefore3Monthes(int hospitalId, int duration, int pageNumber, int pageSize)
        {
            return _unitOfWork.AssetDetailRepository.AlertAssetsWarrantyEndBefore3Monthes(hospitalId,duration, pageNumber, pageSize);
        }



        public MobileAssetDetailVM2 GetAssetDetailByIdOnly(string userId, int assetId)
        {
            return _unitOfWork.AssetDetailRepository.GetAssetDetailByIdOnly(userId, assetId);
        }

        public IEnumerable<ViewAssetDetailVM> GetAutoCompleteSupplierExcludedAssetsByHospitalId(string barcode, int hospitalId)
        {
            return _unitOfWork.AssetDetailRepository.GetAutoCompleteSupplierExcludedAssetsByHospitalId(barcode, hospitalId);
        }

        public IndexAssetDetailVM GetHospitalAssetsByGovIdAndDeptIdAndHospitalId(int departmentId, int govId, int hospitalId, string userId, int pageNumber, int pageSize)
        {
            return _unitOfWork.AssetDetailRepository.GetHospitalAssetsByGovIdAndDeptIdAndHospitalId(departmentId, govId, hospitalId, userId, pageNumber, pageSize);
        }


        public IndexAssetDetailVM GetHospitalAssetsBySupplierId(int supplierId, int pageNumber, int pageSize)
        {
            return _unitOfWork.AssetDetailRepository.GetHospitalAssetsBySupplierId(supplierId, pageNumber, pageSize);

        }

        //public IndexAssetDetailVM SearchHospitalAssetsBySupplierId(SearchAssetDetailVM searchObj, int pageNumber, int pageSize)
        //{
        //    return _unitOfWork.AssetDetailRepository.SearchHospitalAssetsBySupplierId(searchObj, pageNumber, pageSize);
        //}

        public IndexAssetDetailVM SortHospitalAssetsBySupplierId(Sort sortObj, int pageNumber, int pageSize)
        {
            return _unitOfWork.AssetDetailRepository.SortHospitalAssetsBySupplierId(sortObj, pageNumber, pageSize);
        }


        public IndexAssetDetailVM FilterDataByDepartmentBrandSupplierIdAndPaging(FilterHospitalAsset data, string userId, int pageNumber, int pageSize)
        {
            return _unitOfWork.AssetDetailRepository.FilterDataByDepartmentBrandSupplierIdAndPaging(data, userId, pageNumber, pageSize);
        }



        public IndexAssetDetailVM GetAssetsByBrandId(int brandId)
        {
            return _unitOfWork.AssetDetailRepository.GetAssetsByBrandId(brandId);
        }

        public IndexAssetDetailVM GetAssetsByDepartmentId(int departmentId)
        {
            return _unitOfWork.AssetDetailRepository.GetAssetsByDepartmentId(departmentId);
        }

        public List<IndexAssetDetailVM.GetData> GetAssetsBySupplierId(int supplierId)
        {

            return _unitOfWork.AssetDetailRepository.GetAssetsBySupplierId(supplierId);
        }

        public IndexAssetDetailVM GetAssetsBySupplierIdWithPaging(int supplierId, int pageNumber, int pageSize)
        {
            return _unitOfWork.AssetDetailRepository.GetAssetsBySupplierIdWithPaging(supplierId, pageNumber, pageSize);
        }

      public IndexAssetDetailVM GetAssetsByGovernorateIdWithPaging(int governorateId, int pageNumber, int pageSize)
        {
            return _unitOfWork.AssetDetailRepository.GetAssetsByGovernorateIdWithPaging(governorateId, pageNumber, pageSize);
        }

        public IndexAssetDetailVM GetAssetsByGovernorateIdWithPaging(int governorateId)
        {
            return _unitOfWork.AssetDetailRepository.GetAssetsByGovernorateIdWithPaging(governorateId);
        }






        public IndexAssetDetailVM GetAssetsByCityIdWithPaging(int cityId, int pageNumber, int pageSize)
        {
            return _unitOfWork.AssetDetailRepository.GetAssetsByCityIdWithPaging(cityId, pageNumber, pageSize);
        }

        public IndexAssetDetailVM GetAssetsByCityIdWithPaging(int cityId)
        {
            return _unitOfWork.AssetDetailRepository.GetAssetsByCityIdWithPaging(cityId);
        }







        public IndexAssetDetailVM GetAssetsByHospitalIdWithPaging(int hospitalId, int pageNumber, int pageSize)
        {
            return _unitOfWork.AssetDetailRepository.GetAssetsByHospitalIdWithPaging(hospitalId, pageNumber, pageSize);
        }

        public IndexAssetDetailVM GetAssetsByHospitalIdWithPaging(int hospitalId)
        {
            return _unitOfWork.AssetDetailRepository.GetAssetsByHospitalIdWithPaging(hospitalId);
        }




        public List<IndexAssetDetailVM.GetData> FindAllFilteredAssetsForGrouping(FilterHospitalAsset data)
        {
            return _unitOfWork.AssetDetailRepository.FindAllFilteredAssetsForGrouping(data);
        }

        public List<BrandGroupVM> GroupAssetDetailsByBrand(FilterHospitalAsset data)
        {
            return _unitOfWork.AssetDetailRepository.GroupAssetDetailsByBrand(data);
        }

        public List<SupplierGroupVM> GroupAssetDetailsBySupplier(FilterHospitalAsset data)
        {
            return _unitOfWork.AssetDetailRepository.GroupAssetDetailsBySupplier(data);
        }

        public List<DepartmentGroupVM> GroupAssetDetailsByDepartment(FilterHospitalAsset data)
        {
            return _unitOfWork.AssetDetailRepository.GroupAssetDetailsByDepartment(data);
        }

        public IndexAssetDetailVM ListHospitalAssets(SortAndFilterVM data,int  first,int rows)
        {
            return _unitOfWork.AssetDetailRepository.ListHospitalAssets(data, first, rows);
        }

        public IndexAssetDetailVM GenericReportGetAssetsByUserIdAndPaging(string userId, int pageNumber, int pageSize)
        {
            return _unitOfWork.AssetDetailRepository.GenericReportGetAssetsByUserIdAndPaging(userId, pageNumber, pageSize);

        }

        public IndexRequestVM GetRequestsForAssetId(int assetId, int pageNumber, int pageSize)
        {
            return _unitOfWork.AssetDetailRepository.GetRequestsForAssetId(assetId, pageNumber, pageSize);
        }


        public IndexAssetDetailVM SortAssetDetailAfterSearch(SortAndFilterDataModel data, int pageNumber, int pageSize)
        {
            return _unitOfWork.AssetDetailRepository.SortAssetDetailAfterSearch(data, pageNumber, pageSize);
        }

        public IndexAssetDetailVM GeoSortAssetsWithoutSearch(Sort sortObj, int pageNumber, int pageSize)
        {
            return _unitOfWork.AssetDetailRepository.GeoSortAssetsWithoutSearch(sortObj, pageNumber, pageSize);
        }

        public IEnumerable<AssetDetail> GetAssetNameByMasterAssetIdAndHospitalId(int masterAssetId, int hospitalId)
        {
            return _unitOfWork.AssetDetailRepository.GetAssetNameByMasterAssetIdAndHospitalId(masterAssetId, hospitalId);         
        }

        public IEnumerable<IndexAssetDetailVM.GetData> AutoCompleteAssetBarCodeByDepartmentId(string barcode, int hospitalId, int departmentId)
        {
            return _unitOfWork.AssetDetailRepository.AutoCompleteAssetBarCodeByDepartmentId(barcode, hospitalId, departmentId);
        }

        public List<GovernorateGroupVM> GroupAssetDetailsByGovernorate(FilterHospitalAsset data)
        {
            return _unitOfWork.AssetDetailRepository.GroupAssetDetailsByGovernorate(data);
        }

        public List<GroupCityVM> GroupAssetDetailsByCity(FilterHospitalAsset data)
        {
            return _unitOfWork.AssetDetailRepository.GroupAssetDetailsByCity(data);
        }


        public async Task<IEnumerable<IndexAssetDetailVM.GetData>> GetAssetsByUserId(string userId)
        {
            return await _unitOfWork.AssetDetailRepository.GetAssetsByUserId(userId);
        }

        public List<GroupHospitalVM> GroupAssetDetailsByHospital(FilterHospitalAsset data)
        {
            return  _unitOfWork.AssetDetailRepository.GroupAssetDetailsByHospital(data);
        }




        public List<IndexAssetDetailVM.GetData> PrintListOfPMWorkOrders(List<IndexAssetDetailVM.GetData> assets)
        {
            return _unitOfWork.AssetDetailRepository.PrintListOfPMWorkOrders(assets);
        }

        public List<IndexAssetDetailVM.GetData> PrintListOfPMWorkOrders(SortAndFilterVM assetSortAndFilterVM)
        {
            return _unitOfWork.AssetDetailRepository.PrintListOfPMWorkOrders(assetSortAndFilterVM);
        }
    }
}