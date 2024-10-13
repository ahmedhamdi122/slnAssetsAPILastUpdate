using Asset.Models;
using Asset.ViewModels.WorkOrderVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset.Domain.Repositories
{
    public interface IWorkOrderRepository
    {
       
        IEnumerable<IndexWorkOrderVM> GetAllWorkOrdersByHospitalId(int? hospitalId);
        List<IndexWorkOrderVM> GetAllWorkOrdersByHospitalIdAndPaging(int? hospitalId, string userId, int statusId, int pageNumber, int pageSize);
        IndexWorkOrderVM2 GetAllWorkOrdersByHospitalIdAndPaging2(int? hospitalId, string userId, int statusId, int pageNumber, int pageSize);
        int GetWorkOrdersCountByStatusIdAndPaging(int? hospitalId, string userId, int statusId);
        IEnumerable<IndexWorkOrderVM> ExportWorkOrdersByStatusId(int? hospitalId, string userId, int statusId);
        IEnumerable<IndexWorkOrderVM> GetAllWorkOrdersByHospitalId(int? hospitalId, string userId, int statusId);
        //IEnumerable<IndexWorkOrderVM> GetworkOrderByUserId(int requestId, string userId);
        //IndexWorkOrderVM GetMobileWorkOrderByRequestUserId(int requestId, string userId);
       // IEnumerable<IndexWorkOrderVM> GetworkOrderByUserAssetId(int assetId, string userId);
       // IEnumerable<IndexWorkOrderVM> GetworkOrder(string userId);
        List<IndexWorkOrderVM> GetLastRequestAndWorkOrderByAssetId(int assetId);
        List<IndexWorkOrderVM> GetLastRequestAndWorkOrderByAssetId(int assetId, int requestId);
        IEnumerable<IndexWorkOrderVM> SearchWorkOrders(SearchWorkOrderVM searchObj);
        IndexWorkOrderVM2 SearchWorkOrders(SearchWorkOrderVM searchObj, int pageNumber, int pageSize);
        // GetWorkOrderByRequestId(int requestId);
     
 
        int GetTotalWorkOrdersForAssetInHospital(int assetDetailId);
       
        IEnumerable<IndexWorkOrderVM> SortWorkOrders(int hosId, string userId, SortWorkOrderVM sortObj, int statusId);
        IEnumerable<IndexWorkOrderVM> GetWorkOrdersByDate(SearchWorkOrderByDateVM woDateObj);
        IndexWorkOrderVM2 GetWorkOrdersByDateAndStatus(SearchWorkOrderByDateVM woDateObj, int pageNumber, int pageSize);
        IndexWorkOrderVM2 GetWorkOrdersByDateAndStatus(SearchWorkOrderByDateVM woDateObj);
        int CountWorkOrdersByHospitalId(int hospitalId, string userId);
        int CreateWorkOrderAttachments(WorkOrderAttachment attachObj);
        List<IndexWorkOrderVM2.GetData> PrintListOfWorkOrders(List<ExportWorkOrderVM> workOrders);
        List<IndexWorkOrderVM2.GetData> PrintListOfWorkOrders(PrintWorkOrderVM printWorkOrderObj);




        #region Main Functions
        IEnumerable<IndexWorkOrderVM> GetAll();
        IndexWorkOrderVM2 ListWorkOrders(SortAndFilterWorkOrderVM data, int pageNumber, int pageSize);
        GeneratedWorkOrderNumberVM GenerateWorOrderNumber();
        int Add(CreateWorkOrderVM createWorkOrderVM);
        void Update(int id, EditWorkOrderVM editWorkOrderVM);
        void Delete(int id);
        IndexWorkOrderVM GetById(int id);
        PrintWorkOrderVM PrintWorkOrderById(int id);
        #endregion
    }
}
