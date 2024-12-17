using Asset.Models;
using Asset.ViewModels.RequestTrackingVM;
using Asset.ViewModels.RequestVM;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset.Domain.Services
{
    public interface IRequestService
    {
        IEnumerable<IndexRequestsVM> GetAllRequests();
        IEnumerable<IndexRequestVM.GetData> GetAllRequestsWithTrackingByUserId(string userId);
        IEnumerable<IndexRequestVM.GetData> GetAllRequestsByStatusId(string userId, int statusId);
        //IEnumerable<IndexRequestVM.GetData> GetAllRequestsByHospitalAssetId(int assetId);
        List<ReportRequestVM> GetRequestEstimationById(int id);
        List<ReportRequestVM> GetRequestEstimations(SearchRequestDateVM searchRequestDateObj);
        int GetTotalOpenRequest(string userId);
        List<Request> ListOpenRequests(int hospitalId);
        List<IndexRequestVM.GetData> ListNewRequests(int hospitalId);
        List<IndexRequestTracking> ListOpenRequestTracks(int hospitalId);
        int UpdateOpenedRequest(int requestId);
        int UpdateOpenedRequestTrack(int trackId);
        IndexRequestsVM GetRequestByWorkOrderId(int workOrderId);
        int GetTotalRequestForAssetInHospital(int assetDetailId);
        Task<ActionResult<IndexRequestsVM>> GetRequestById(int id);
        int AddRequest(CreateRequestVM createRequestVM);
        void UpdateRequest(EditRequestVM editRequestVM);
        void DeleteRequest(int id);
        IndexRequestsVM GetByRequestCode(string code);
        bool ValidateDate(int AssetDetailId);



        IEnumerable<IndexRequestVM.GetData> GetAllRequestsByAssetId(int assetId, int hospitalId);
        IEnumerable<IndexRequestVM.GetData> SearchRequests(SearchRequestVM searchObj);
        IndexRequestVM SearchInRequests(SearchRequestVM searchObj, int pageNumber, int pageSize);
        Task<IEnumerable<IndexRequestsVM>> SortRequests(SortRequestVM sortObj,int statusId);
        IEnumerable<IndexRequestsVM> SortRequestsByAssetId(SortRequestVM sortObj);
        Task<List<IndexRequestsVM>> SortRequestsByPaging(SortRequestVM sortObj, int statusId, int pageNumber, int pageSize);



        IEnumerable<IndexRequestVM.GetData> GetRequestsByDate(SearchRequestDateVM requestDateObj);
        IndexRequestVM GetRequestsByDateAndStatus(SearchRequestDateVM requestDateObj,int pageNumber, int pageSize);
        List<IndexRequestVM.GetData> GetRequestsByDateAndStatus(SearchRequestDateVM requestDateObj);
        int CountRequestsByHospitalId(int hospitalId, string userId);
        int CreateRequestAttachments(RequestDocument attachObj);
      
        List<IndexRequestVM.GetData> GetRequestsByStatusIdAndPaging(string userId, int statusId, int pageNumber, int pageSize);
        IndexRequestVM GetAllRequestsByStatusIdAndPaging(string userId, int statusId, int pageNumber, int pageSize);
        IndexRequestVM GetAllRequestsByStatusIdAndPaging2(string userId, int statusId, int pageNumber, int pageSize);

        int GetRequestsCountByStatusIdAndPaging(string userId, int statusId);
    
        List<IndexRequestVM.GetData> PrintListOfRequests(List<ExportRequestVM> requests);
        OpenRequestVM ListOpenRequests(SearchOpenRequestVM searchOpenRequestObj, int pageNumber, int pageSize);
        List<OpenRequestVM.GetData> ListOpenRequestsPDF(SearchOpenRequestVM searchOpenRequestObj);
        IndexRequestVM GetRequestsByStatusIdAndPagingV2(string userId, int statusId, int pageNumber, int pageSize);



        
        #region Main Functions
        GeneratedRequestNumberVM GenerateRequestNumber();
        PrintServiceRequestVM PrintServiceRequestById(int id);
        IEnumerable<IndexRequestVM.GetData> GetOldRequestsByHospitalAssetId(int hospitalAssetId);
        #endregion
        #region Report and Print Functions
        IEnumerable<IndexRequestVM.GetData> ExportRequestsByStatusId(SortAndFilterRequestVM data);
        #endregion
        #region List Requests
        Task<IndexRequestVM> ListRequests(SortAndFilterRequestVM data, int first, int rows);
        #endregion

        #region Dashboard Functions
        IndexRequestVM AlertOpenedRequestAssetsAndHighPeriority(int periorityId, int hospitalId, int pageNumber, int pageSize);
        #endregion
    }
}
