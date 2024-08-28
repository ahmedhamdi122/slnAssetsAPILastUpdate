using Asset.Models;
using Asset.ViewModels.ContractVM;
using System;
using System.Collections.Generic;


namespace Asset.Domain.Repositories
{
    public interface IMasterContractRepository
    {
        IEnumerable<MasterContract> GetAll();

        //IndexMasterContractVM GetListBoxMasterContractsByHospitalId(int hospitalId, int pageNumber, int pageSize);

        IndexMasterContractVM ListMasterContracts(SortAndFilterContractVM data, int pageNumber, int pageSize);


        IndexMasterContractVM GetListBoxMasterContractsByHospitalId(int hospitalId);
  
     //   IndexMasterContractVM Search(SearchContractVM model, int pageNumber, int pageSize);
     //   IEnumerable<IndexMasterContractVM.GetData> SortContracts(int hospitalId, SortContractsVM sortObj);
    
        
        
        
        
        DetailMasterContractVM GetById(int id);
        int Add(CreateMasterContractVM masterContractObj);
        int Update(MasterContract masterContractObj);
        int Delete(int id);
        int CreateContractAttachments(ContractAttachment attachObj);
        GeneratedMasterContractNumberVM GenerateMasterContractSerial();
        IEnumerable<ContractAttachment> GetContractAttachmentByMasterContractId(int masterContractId);
        IndexMasterContractVM AlertContractsEndBefore3Months(int hospitalId,int duration, int pageNumber, int pageSize);
        ContractAttachment GetLastDocumentForMasterContractId(int masterContractId);
        int DeleteContractAttachment(int attachId);

    }
}
