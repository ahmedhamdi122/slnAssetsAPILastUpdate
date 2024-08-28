﻿using Asset.Models;
using Asset.ViewModels.SupplierExecludeAssetVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset.Domain.Repositories
{
    public interface ISupplierExecludeAssetRepository
    {

        IEnumerable<IndexSupplierExecludeAssetVM.GetData> GetAll();
        IEnumerable<IndexSupplierExecludeAssetVM.GetData> GetAllByStatusId(int statusId);
        IEnumerable<IndexSupplierExecludeAssetVM.GetData> GetAllByAppTypeId(int appTypeId);

        IndexSupplierExecludeAssetVM ListSupplierExecludeAssets(SortAndFilterSupplierExecludeAssetVM data, int pageNumber, int pageSize);

        IEnumerable<IndexSupplierExecludeAssetVM.GetData> PrintSearchSupplierExecludes(SortAndFilterSupplierExecludeAssetVM data);


        EditSupplierExecludeAssetVM GetById(int id);
        ViewSupplierExecludeAssetVM GetSupplierExecludeAssetDetailById(int id);
        int Add(CreateSupplierExecludeAssetVM execludeAssetObj);
        int Update(EditSupplierExecludeAssetVM execludeAssetObj);
        int UpdateExcludedDate(EditSupplierExecludeAssetVM execludeAssetObj);
        int Delete(int id);
        int DeleteSupplierExecludeAttachment(int id);

        GenerateSupplierExecludeAssetNumberVM GenerateSupplierExecludeAssetNumber();

        int CreateSupplierExecludAttachments(SupplierExecludeAttachment attachObj);
        IEnumerable<SupplierExecludeAttachment> GetAttachmentBySupplierExecludeAssetId(int assetId);

        IEnumerable<IndexSupplierExecludeAssetVM.GetData> GetSupplierExecludeAssetByDate(SearchSupplierExecludeAssetVM searchObj);

        IndexSupplierExecludeAssetVM GetAllSupplierExecludes(SearchSupplierExecludeAssetVM searchObj, int statusId, int appTypeId, int hospitalId, int pageNumber, int pageSize);
        IndexSupplierExecludeAssetVM GetAllSupplierHoldes(SearchSupplierExecludeAssetVM searchObj, int statusId, int appTypeId, int hospitalId, int pageNumber, int pageSize);
        IndexSupplierExecludeAssetVM GetAllSupplierExecludes(SearchSupplierExecludeAssetVM searchObj);
        IndexSupplierExecludeAssetVM GetAllSupplierHoldes(SearchSupplierExecludeAssetVM searchObj);
        //  IEnumerable<IndexSupplierExecludeAssetVM.GetData> PrintSearchSupplierExecludes(SearchSupplierExecludeAssetVM searchObj);
    }
}
