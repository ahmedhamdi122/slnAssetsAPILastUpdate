using Asset.Models;
using Asset.ViewModels.MasterAssetAttachmentVM;
using Asset.ViewModels.MasterAssetVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset.Domain.Repositories
{
    public interface IMasterAssetRepository
    {
        #region Main Functions
        IndexMasterAssetVM GetAll(SortAndFilterMasterAssetVM data, int pageNumber, int pageSize);
        IEnumerable<MasterAsset> GetAllMasterAssets();
        IEnumerable<IndexMasterAssetVM.GetData> GetListMasterAsset();
        EditMasterAssetVM GetById(int id);
        int Add(CreateMasterAssetVM masterAssetObj);
        int Update(EditMasterAssetVM masterAssetObj);
        int Delete(int id);
        int CountMasterAssets();
        GeneratedMasterAssetCodeVM GenerateMasterAssetcode();
        ViewMasterAssetVM ViewMasterAsset(int id);
        #endregion

        #region Search Auto Complete 
        IEnumerable<MasterAsset> DistinctAutoCompleteMasterAssetName(string name);

       // IEnumerable<string> GetDistintMasterAssetModels(string name);
        IEnumerable<string> GetDistintMasterAssetModels(int brandId, string name);
        IEnumerable<Brand> GetDistintMasterAssetBrands(string name);
        IEnumerable<MasterAsset> AutoCompleteMasterAssetName(string name);
        IEnumerable<IndexMasterAssetVM.GetData> AutoCompleteMasterAssetName2(string name);
        IEnumerable<IndexMasterAssetVM.GetData> AutoCompleteMasterAssetName3(string name, int hospitalId);
        #endregion


       #region Master Asset Attachments
        int CreateMasterAssetDocuments(CreateMasterAssetAttachmentVM attachObj);
        IEnumerable<MasterAssetAttachment> GetAttachmentByMasterAssetId(int assetId);
        int DeleteMasterAssetAttachment(int id);
        MasterAssetAttachment GetLastDocumentForMsterAssetId(int masterId);
        #endregion

        IQueryable<MasterAsset> RetrieveQueryableMasterAssets();
        IEnumerable<IndexMasterAssetVM.GetData> GetTop10MasterAsset(int hospitalId);
        IEnumerable<MasterAsset> GetAllMasterAssetsByHospitalId(int hospitalId, string userId);
        IEnumerable<MasterAsset> GetAllMasterAssetsByHospitalId(int hospitalId);
        int UpdateMasterAssetImageAfterInsert(CreateMasterAssetVM masterAssetObj);
         List<CountMasterAssetBrands> CountMasterAssetsByBrand(int hospitalId);
        List<CountMasterAssetSuppliers> CountMasterAssetsBySupplier(int hospitalId);


        List<MasterAsset> GetMasterAssetIdByNameBrandModel(string name,int brandId, string model);

    }
}
