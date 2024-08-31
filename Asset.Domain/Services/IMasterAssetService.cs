using Asset.Models;
using Asset.ViewModels.MasterAssetAttachmentVM;
using Asset.ViewModels.MasterAssetVM;
using Microsoft.Data.SqlClient;
using Org.BouncyCastle.Crypto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset.Domain.Services
{
    public interface IMasterAssetService
    {
        #region Main Functions
        IndexMasterAssetVM GetAll(int First,int Rows,SearchSortMasterAssetVM? SearchSortObj);
        IEnumerable<MasterAsset> GetAllMasterAssets();
        IEnumerable<IndexMasterAssetVM.GetData> GetListMasterAsset();
        EditMasterAssetVM GetById(int id);
        int Add(CreateMasterAssetVM masterAssetObj);
        int Update(EditMasterAssetVM masterAssetObj);
        int Delete(int id);
        int CountMasterAssets();
        GeneratedMasterAssetCodeVM GenerateMasterAssetcode();
        ViewMasterAssetVM ViewMasterAsset(int id);
        bool CheckMasterAssetCodeExists(string Code);
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
        public bool isMasterAssetExistsUsingId(int id);
        public bool ExistsByNameModelAndVersion(string Name,string ModelNumber,string VersionNumber);
        public bool ExistsByNameArModelAndVersion(string NameAr, string ModelNumber, string VersionNumber);


        IEnumerable<IndexMasterAssetVM.GetData> GetTop10MasterAsset(int hospitalId);
        IEnumerable<MasterAsset> GetAllMasterAssetsByHospitalId(int hospitalId, string userId);
        IEnumerable<MasterAsset> GetAllMasterAssetsByHospitalId(int hospitalId);
        int UpdateMasterAssetImageAfterInsert(CreateMasterAssetVM masterAssetObj);
        List<CountMasterAssetBrands> CountMasterAssetsByBrand(int hospitalId);
        List<CountMasterAssetSuppliers> CountMasterAssetsBySupplier(int hospitalId);
        List<MasterAsset> GetMasterAssetIdByNameBrandModel(string name, int brandId, string model);
        
    }
}
