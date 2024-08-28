﻿using Asset.Models;
using Asset.ViewModels.AssetStatusVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset.Domain.Services
{
    public interface IAssetStatusService
    {

        IEnumerable<IndexAssetStatusVM.GetData> GetAll();
        EditAssetStatusVM GetById(int id);
        IEnumerable<AssetStatu> GetAllAssetStatus();
        IEnumerable<IndexAssetStatusVM.GetData> GetAssetStatusByName(string AssetStatusName);
        IEnumerable<IndexAssetStatusVM.GetData> SortAssetStatuses(SortAssetStatusVM sortObj);
      
        IndexAssetStatusVM GetHospitalAssetStatus(int statusId, string userId, int hospitalId);

        int Add(CreateAssetStatusVM AssetStatusVM);
        int Update(EditAssetStatusVM AssetStatusVM);
        int Delete(int id);
    }
}
