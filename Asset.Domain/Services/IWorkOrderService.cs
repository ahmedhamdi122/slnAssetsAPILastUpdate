﻿using Asset.Models;
using Asset.ViewModels.WorkOrderVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset.Domain.Services
{
    public interface IWorkOrderService
    {
        IEnumerable<IndexWorkOrderVM> GetAllWorkOrders();
        IndexWorkOrderVM2 ListWorkOrders(SortAndFilterWorkOrderVM data, int pageNumber, int pageSize);
        IEnumerable<IndexWorkOrderVM> GetAllWorkOrdersByHospitalId(int? hospitalId);
        List<IndexWorkOrderVM> GetAllWorkOrdersByHospitalIdAndPaging(int? hospitalId, string userId, int statusId, int pageNumber, int pageSize);
        IndexWorkOrderVM2 GetAllWorkOrdersByHospitalIdAndPaging2(int? hospitalId, string userId, int statusId, int pageNumber, int pageSize);
        int GetWorkOrdersCountByStatusIdAndPaging(int? hospitalId, string userId, int statusId);
        IEnumerable<IndexWorkOrderVM> ExportWorkOrdersByStatusId(int? hospitalId, string userId, int statusId);
        IEnumerable<IndexWorkOrderVM> GetAllWorkOrdersByHospitalId(int? hospitalId, string userId, int statusId);
        IEnumerable<IndexWorkOrderVM> GetworkOrderByUserId(int requestId, string userId);

        IndexWorkOrderVM GetMobileWorkOrderByRequestUserId(int requestId, string userId);

        IEnumerable<IndexWorkOrderVM> GetworkOrderByUserAssetId(int assetId, string userId);
        IEnumerable<IndexWorkOrderVM> GetworkOrder(string userId);
        List<IndexWorkOrderVM> GetLastRequestAndWorkOrderByAssetId(int assetId);
        List<IndexWorkOrderVM> GetLastRequestAndWorkOrderByAssetId(int assetId, int requestId);
        IEnumerable<IndexWorkOrderVM> SearchWorkOrders(SearchWorkOrderVM searchObj);
        IndexWorkOrderVM2 SearchWorkOrders(SearchWorkOrderVM searchObj, int pageNumber, int pageSize);
        IndexWorkOrderVM GetWorkOrderById(int id);
        IndexWorkOrderVM GetWorkOrderByRequestId(int requestId);
        int AddWorkOrder(CreateWorkOrderVM createWorkOrderVM);
        void UpdateWorkOrder(int id, EditWorkOrderVM editWorkOrderVM);
        void DeleteWorkOrder(int id);
        GeneratedWorkOrderNumberVM GenerateWorOrderNumber();
        int GetTotalWorkOrdersForAssetInHospital(int assetDetailId);
        PrintWorkOrderVM PrintWorkOrderById(int id);
        IEnumerable<IndexWorkOrderVM> SortWorkOrders(int hosId, string userId, SortWorkOrderVM sortObj, int statusId);
        IEnumerable<IndexWorkOrderVM> GetWorkOrdersByDate(SearchWorkOrderByDateVM woDateObj);
        IndexWorkOrderVM2 GetWorkOrdersByDateAndStatus(SearchWorkOrderByDateVM woDateObj, int pageNumber, int pageSize);
        IndexWorkOrderVM2 GetWorkOrdersByDateAndStatus(SearchWorkOrderByDateVM woDateObj);
        int CountWorkOrdersByHospitalId(int hospitalId, string userId);
        int CreateWorkOrderAttachments(WorkOrderAttachment attachObj);
        List<IndexWorkOrderVM2.GetData> PrintListOfWorkOrders(List<ExportWorkOrderVM> workOrders);
        List<IndexWorkOrderVM2.GetData> PrintListOfWorkOrders(PrintWorkOrderVM printWorkOrderObj);




    }
}
