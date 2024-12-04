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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Asset.Domain.Repositories
{
    public interface IAssetDetailRepository
    {
        bool CheckAssetDetailCodeExists(string code);
        bool hasAssetWithMasterId(int id);
        IEnumerable<IndexAssetDetailVM.GetData> GetAll();
        EditAssetDetailVM GetById(int id);
        int Add(CreateAssetDetailVM assetDetailObj);
        int Update(EditAssetDetailVM assetDetailObj);
        int Delete(int id);
        int DeleteAssetDetailAttachment(int id);
        IEnumerable<IndexAssetDetailVM.GetData> GetAssetDetailsByAssetId(int assetId);
        IndexAssetDetailVM GetAllAssetsByStatusId(int pageNumber, int pageSize, int statusId, string userId);
        IndexAssetDetailVM ListHospitalAssets(SortAndFilterVM data, int pageNumber, int pageSize);
        IndexAssetDetailVM AlertAssetsWarrantyEndBefore3Monthes(int hospitalId,int duration, int pageNumber, int pageSize);
        IndexRequestVM GetRequestsForAssetId(int assetId, int pageNumber, int pageSize);
        IndexAssetDetailVM SortAssetDetailAfterSearch(SortAndFilterDataModel data, int pageNumber, int pageSize);
        IndexAssetDetailVM GeoSortAssetsWithoutSearch(Sort sortObj, int pageNumber, int pageSize);


        IEnumerable<AssetDetail> GetAssetNameByMasterAssetIdAndHospitalId(int masterAssetId, int hospitalId);



        //IEnumerable<IndexAssetDetailVM.GetData> GetAllAssetsByStatusId(int statusId, string userId);
        //Task<IEnumerable<IndexAssetDetailVM.GetData>> GetAssetDetailsByUserId(string userId);
        //Task<IndexAssetDetailVM> GetAssetDetailsByUserId2(int pageNumber, int pageSize, string userId);
        Task<IEnumerable<IndexAssetDetailVM.GetData>> GetAssetsByUserId(string userId);
        //IndexAssetDetailVM GetAssetsByUserId(string userId, int pageNumber, int pageSize);
        //IndexAssetDetailVM SearchAssetInHospital(int pagenumber, int pagesize, SearchMasterAssetVM searchObj);
        //IndexAssetDetailVM SearchAssetWarrantyInHospital(int pagenumber, int pagesize, SearchMasterAssetVM searchObj);
        //IEnumerable<IndexAssetDetailVM.GetData> SearchAssetInHospitalByHospitalId(SearchMasterAssetVM searchObj);
        //IndexAssetDetailVM SearchHospitalAssetsByHospitalId(SearchMasterAssetVM searchObj);
        //IEnumerable<IndexAssetDetailVM.GetData> SortAssets(Sort sortObj);
        //IndexAssetDetailVM SortAssets(Sort sortObj,  int statusId, string userId);
        //IndexAssetDetailVM SortAssets2(Sort sortObj, int pageNumber, int pageSize);
        //
        //
        //IndexAssetDetailVM GetAssetsByUserIdAndPaging(string userId, int pageNumber, int pageSize);
        //List<DrawChart> DrawingChart();




        AssetDetail QueryAssetDetailById(int assetId);


        IndexAssetDetailVM SearchHospitalAssetsByDepartmentId(int departmentId, string userId, int pageNumber, int pageSize);


        IEnumerable<AssetDetail> GetAllAssetDetailsByHospitalId(int hospitalId);
   
        IEnumerable<ViewAssetDetailVM> GetNoneExcludedAssetsByHospitalId(int hospitalId);
        IEnumerable<ViewAssetDetailVM> GetSupplierNoneExcludedAssetsByHospitalId(int hospitalId);
        IEnumerable<ViewAssetDetailVM> GetAutoCompleteSupplierNoneExcludedAssetsByHospitalId(string barcode, int hospitalId);
        IEnumerable<ViewAssetDetailVM> GetAutoCompleteSupplierExcludedAssetsByHospitalId(string barcode, int hospitalId);
        IEnumerable<ViewAssetDetailVM> GetListOfAssetDetailsByHospitalNotInContract(int hospitalId);
        IEnumerable<ViewAssetDetailVM> GetListOfAssetDetailsByHospitalNotInContract(string barcode, int hospitalId);
        IEnumerable<ViewAssetDetailVM> GetListOfAssetDetailsByHospitalNotInContractBySerialNumber(string serialNumber, int hospitalId);
      
        ViewAssetDetailVM ViewAssetDetailByMasterId(int masterId);
        ViewAssetDetailVM GetAssetHistoryById(int assetId);
        IEnumerable<AssetDetail> ViewAllAssetDetailByMasterId(int MasterAssetId);
        int CreateAssetDetailDocuments(CreateAssetDetailAttachmentVM attachObj);
        IEnumerable<AssetDetailAttachment> GetAttachmentByAssetDetailId(int assetId);


        IEnumerable<IndexPMAssetTaskScheduleVM.GetData> GetAllPMAssetTaskSchedules(int? hospitalId);
        IEnumerable<IndexPMAssetTaskScheduleVM.GetData> GetAllPMAssetTaskScheduleByAssetId(int? assetId);
        List<CountAssetVM> CountAssetsByHospital();
        List<CountAssetVM> ListTopAssetsByHospitalId(int hospitalId);
        List<CountAssetVM> ListAssetsByGovernorateIds();
        List<CountAssetVM> ListAssetsByCityIds();
        List<CountAssetVM> CountAssetsInHospitalByHospitalId(int hospitalId);
        int CountAssetsByHospitalId(int hospitalId);
        List<PmDateGroupVM> GetAllwithgrouping(int assetId);
        List<IndexAssetDetailVM.GetData> FilterAsset(filterDto data);
        List<IndexAssetDetailVM.GetData> FilterDataByDepartmentBrandSupplierId(FilterHospitalAsset data);
        List<DepartmentGroupVM> GetAssetByDepartment(List<IndexAssetDetailVM.GetData> AssetModel);
        List<BrandGroupVM> GetAssetByBrands(List<IndexAssetDetailVM.GetData> AssetModel);
        List<GroupHospitalVM> GetAssetByHospital(List<IndexAssetDetailVM.GetData> AssetModel);
        List<GroupGovernorateVM> GetAssetByGovernorate(List<IndexAssetDetailVM.GetData> AssetModel);
        List<GroupCityVM> GetAssetByCity(List<IndexAssetDetailVM.GetData> AssetModel);


        List<GroupSupplierVM> GetAssetBySupplier(List<IndexAssetDetailVM.GetData> AssetModel);
        List<GroupOrganizationVM> GetAssetByOrganization(List<IndexAssetDetailVM.GetData> AssetModel);




        List<HospitalAssetAge> GetAssetsByAgeGroup(int hospitalId);
        List<HospitalAssetAge> GetGeneralAssetsByAgeGroup(FilterHospitalAssetAge model);
        Task<IEnumerable<IndexAssetDetailVM.GetData>> AutoCompleteAssetBarCode(string barcode, int hospitalId, string UserId);
        Task<IEnumerable<IndexAssetDetailVM.GetData>> AutoCompleteAssetSerial(string serial, int hospitalId, string UserId);
        IEnumerable<IndexAssetDetailVM.GetData> AutoCompleteAssetBarCodeByDepartmentId(string barcode, int hospitalId, int departmentId);
        AssetDetailAttachment GetLastDocumentForAssetDetailId(int assetDetailId);
        GeneratedAssetDetailBCVM GenerateAssetDetailBarcode();
        MobileAssetDetailVM GetAssetDetailById(string userId, int assetId);
        MobileAssetDetailVM2 GetAssetDetailByIdOnly(string userId, int assetId);
        bool GenerateQrCodeForAllAssets(string domainName);
        IndexAssetDetailVM MobSearchAssetInHospital(SearchMasterAssetVM searchObj, int pageNumber, int pageSize);
        IndexAssetDetailVM GetHospitalAssetsByGovIdAndDeptIdAndHospitalId(int departmentId, int govId, int hospitalId, string userId, int pageNumber, int pageSize);
        IndexAssetDetailVM GetHospitalAssetsBySupplierId(int supplierId, int pageNumber, int pageSize);
        //IndexAssetDetailVM SearchHospitalAssetsBySupplierId(SearchAssetDetailVM searchObj, int pageNumber, int pageSize);
        IndexAssetDetailVM SortHospitalAssetsBySupplierId(Sort sortObj, int pageNumber, int pageSize);




        IndexAssetDetailVM FilterDataByDepartmentBrandSupplierIdAndPaging(FilterHospitalAsset data, string userId, int pageNumber, int pageSize);
        IndexAssetDetailVM GetAssetsByBrandId(int brandId);
        IndexAssetDetailVM GetAssetsByDepartmentId(int departmentId);
        List<IndexAssetDetailVM.GetData> GetAssetsBySupplierId(int supplierId);
        IndexAssetDetailVM GetAssetsBySupplierIdWithPaging(int supplierId, int pageNumber, int pageSize);

        IndexAssetDetailVM GetAssetsByGovernorateIdWithPaging(int governorateId, int pageNumber, int pageSize);

        IndexAssetDetailVM GetAssetsByGovernorateIdWithPaging(int governorateId);


       IndexAssetDetailVM GetAssetsByCityIdWithPaging(int cityId, int pageNumber, int pageSize);

        IndexAssetDetailVM GetAssetsByCityIdWithPaging(int cityId);


        IndexAssetDetailVM GetAssetsByHospitalIdWithPaging(int hospitalId, int pageNumber, int pageSize);
        IndexAssetDetailVM GetAssetsByHospitalIdWithPaging(int hospitalId);

        List<IndexAssetDetailVM.GetData> FindAllFilteredAssetsForGrouping(FilterHospitalAsset data);
        List<BrandGroupVM> GroupAssetDetailsByBrand(FilterHospitalAsset data);
        List<SupplierGroupVM> GroupAssetDetailsBySupplier(FilterHospitalAsset data);
        List<DepartmentGroupVM> GroupAssetDetailsByDepartment(FilterHospitalAsset data);

        List<GovernorateGroupVM> GroupAssetDetailsByGovernorate(FilterHospitalAsset data);
        List<GroupCityVM> GroupAssetDetailsByCity(FilterHospitalAsset data);
      
        List<GroupHospitalVM> GroupAssetDetailsByHospital(FilterHospitalAsset data);





        IndexAssetDetailVM GenericReportGetAssetsByUserIdAndPaging(string userId, int pageNumber, int pageSize);




        List<IndexAssetDetailVM.GetData> PrintListOfPMWorkOrders(List<IndexAssetDetailVM.GetData> workOrders);


           List<IndexAssetDetailVM.GetData> PrintListOfPMWorkOrders(SortAndFilterVM data);

    }
}
