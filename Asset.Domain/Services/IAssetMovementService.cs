﻿using System.Collections.Generic;
using Asset.Models;
using Asset.ViewModels.AssetMovementVM;


namespace Asset.Domain.Services
{
  public  interface IAssetMovementService
    {
        IEnumerable<AssetMovement> GetAllAssetMovements();
        IndexAssetMovementVM GetAll(int pageNumber, int pageSize);
        IndexAssetMovementVM GetAll(SortAndFilterAssetMovementVM data, int pageNumber, int pageSize);
        IEnumerable<IndexAssetMovementVM.GetData> GetMovementByAssetDetailId(int assetId);
        EditAssetMovementVM GetById(int id);
        int Add(CreateAssetMovementVM movementObj);
        int Update(EditAssetMovementVM movementObj);
        int Delete(int id);

        IndexAssetMovementVM SearchAssetMovement(SearchAssetMovementVM searchObj, int pageNumber, int pageSize);
    }
}
