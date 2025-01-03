﻿using Asset.Models;
using Asset.ViewModels.AssetDetailVM;
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
        Task<IndexMasterAssetVM> GetAll(int First, int Rows,SearchSortMasterAssetVM? SearchSortObj);
        IEnumerable<MasterAsset> GetAllMasterAssets();
        IEnumerable<IndexMasterAssetVM.GetData> GetListMasterAsset();
        EditMasterAssetVM GetById(int id);
        int Add(CreateMasterAssetVM masterAssetObj);
        int Update(EditMasterAssetVM masterAssetObj);
        int Delete(int id);
        int CountMasterAssets();
        GeneratedMasterAssetCodeVM GenerateMasterAssetcode();
        ViewMasterAssetVM ViewMasterAsset(int id);
        bool ExistsByNameModelAndVersion(string Name, string ModelNumber, string VersionNumber);
        bool ExistsByNameArModelAndVersion(string NameAr, string ModelNumber, string VersionNumber);
        #endregion

        #region Search Auto Complete 
        IEnumerable<MasterAsset> DistinctAutoCompleteMasterAssetName(string name);

       // IEnumerable<string> GetDistintMasterAssetModels(string name);
        IEnumerable<string> GetDistintMasterAssetModels(int brandId, string name);
        IEnumerable<Brand> GetDistintMasterAssetBrands(string name);
        IEnumerable<MasterAsset> AutoCompleteMasterAssetName(string name);
        IEnumerable<IndexMasterAssetVM.GetData> AutoCompleteMasterAssetName2(string name);
        Task<IEnumerable<AssetDetailsWithMasterAssetVM>> AutoCompleteMasterAssetName(string name, int hospitalId, string UserId);
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

        bool isMasterAssetExistsUsingId(int id);
        List<MasterAsset> GetMasterAssetIdByNameBrandModel(string name,int brandId, string model);

    }
}
