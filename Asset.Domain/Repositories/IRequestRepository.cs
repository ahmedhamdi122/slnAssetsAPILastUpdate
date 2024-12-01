using Asset.Models;
using Asset.ViewModels.RequestTrackingVM;
using Asset.ViewModels.RequestVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset.Domain.Repositories
{
    public interface IRequestRepository
    {
        bool ValidateDate(int AssetDetailId);
        IEnumerable<IndexRequestVM.GetData> GetAllRequestsWithTrackingByUserId(string userId);
        IEnumerable<IndexRequestVM.GetData> GetAllRequestsByStatusId(string userId, int statusId);
        IEnumerable<IndexRequestVM.GetData> GetAllRequestsByStatusId(string userId, int statusId, int page, int pageSize);
 
        //IEnumerable<IndexRequestVM.GetData> GetAllRequestsByHospitalAssetId(int assetId);
        List<ReportRequestVM> GetRequestEstimationById(int id);
        List<ReportRequestVM> GetRequestEstimations(SearchRequestDateVM searchRequestDateObj);
        IndexRequestsVM GetRequestByWorkOrderId(int workOrderId);
        int GetTotalRequestForAssetInHospital(int assetDetailId);
        int GetTotalOpenRequest(string userId);
        List<Request> ListOpenRequests(int hospitalId);
        List<IndexRequestVM.GetData> ListNewRequests(int hospitalId);
        List<IndexRequestTracking> ListOpenRequestTracks(int hospitalId);
        int UpdateOpenedRequest(int requestId);
        int UpdateOpenedRequestTrack(int trackId);
        IndexRequestsVM GetByRequestCode(string code);


        IEnumerable<IndexRequestVM.GetData> GetAllRequestsByAssetId(int assetId, int hospitalId);
        IEnumerable<IndexRequestVM.GetData> SearchRequests(SearchRequestVM searchObj);
        IndexRequestVM SearchInRequests(SearchRequestVM searchObj, int pageNumber, int pageSize);
        Task<IEnumerable<IndexRequestsVM>> SortRequests(SortRequestVM sortObj, int statusId);
        IEnumerable<IndexRequestsVM> SortRequestsByAssetId(SortRequestVM sortObj);
        Task<List<IndexRequestsVM>> SortRequestsByPaging(SortRequestVM sortObj, int statusId, int pageNumber, int pageSize);
        IndexRequestVM GetRequestsByStatusIdAndPagingV2(string userId, int statusId, int pageNumber, int pageSize);
        List<IndexRequestVM.GetData> GetRequestsByStatusIdAndPaging(string userId, int statusId, int pageNumber, int pageSize);
        IndexRequestVM GetAllRequestsByStatusIdAndPaging(string userId, int statusId, int pageNumber, int pageSize);
        IndexRequestVM GetAllRequestsByStatusIdAndPaging2(string userId, int statusId, int pageNumber, int pageSize);






        List<IndexRequestVM.GetData> GetRequestsByDateAndStatus(SearchRequestDateVM requestDateObj);
        int CountRequestsByHospitalId(int hospitalId, string userId);
        int GetRequestsCountByStatusIdAndPaging(string userId, int statusId);
        IndexRequestVM AlertOpenedRequestAssetsAndHighPeriority(int periorityId, int hospitalId, int pageNumber, int pageSize);
        OpenRequestVM ListOpenRequests(SearchOpenRequestVM searchOpenRequestObj, int pageNumber, int pageSize);
        List<OpenRequestVM.GetData> ListOpenRequestsPDF(SearchOpenRequestVM searchOpenRequestObj);


        #region Main Functions
        IEnumerable<IndexRequestsVM> GetAll();
        IndexRequestsVM GetById(int id);
        int Add(CreateRequestVM createRequestVM);
        void Update(EditRequestVM editRequestVM);
        void Delete(int id);
        GeneratedRequestNumberVM GenerateRequestNumber();
        PrintServiceRequestVM PrintServiceRequestById(int id);
        IEnumerable<IndexRequestVM.GetData> GetOldRequestsByHospitalAssetId(int hospitalAssetId);

        #region Create Request Attachments
        int CreateRequestAttachments(RequestDocument attachObj);
        #endregion

        #endregion
        #region Report and Print Functions
        IEnumerable<IndexRequestVM.GetData> ExportRequestsByStatusId(SortAndFilterRequestVM data);
        List<IndexRequestVM.GetData> PrintListOfRequests(List<ExportRequestVM> requests);
        IEnumerable<IndexRequestVM.GetData> GetRequestsByDate(SearchRequestDateVM requestDateObj);
        IndexRequestVM GetRequestsByDateAndStatus(SearchRequestDateVM requestDateObj, int pageNumber, int pageSize);
        #endregion


        #region List Requests
        Task<IndexRequestVM> ListRequests(SortAndFilterRequestVM data, int first, int rows);
        #endregion
    }
}
