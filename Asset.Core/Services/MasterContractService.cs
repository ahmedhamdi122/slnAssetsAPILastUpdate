using Asset.Domain;
using Asset.Domain.Services;
using Asset.Models;
using Asset.ViewModels.ContractVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset.Core.Services
{
    public class MasterContractService : IMasterContractService
    {
        private IUnitOfWork _unitOfWork;

        public MasterContractService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        public int Add(CreateMasterContractVM masterContractObj)
        {
            return _unitOfWork.MasterContractRepository.Add(masterContractObj);
        }

        public IndexMasterContractVM AlertContractsEndBefore3Months(int hospitalId, int duration, int pageNumber, int pageSize)
        {
            return _unitOfWork.MasterContractRepository.AlertContractsEndBefore3Months(hospitalId, duration, pageNumber, pageSize);
        }

        public int CreateContractAttachments(ContractAttachment attachObj)
        {
            return _unitOfWork.MasterContractRepository.CreateContractAttachments(attachObj);
        }

        public int Delete(int id)
        {
            return _unitOfWork.MasterContractRepository.Delete(id);
        }

        public int DeleteContractAttachment(int attachId)
        {
            return _unitOfWork.MasterContractRepository.DeleteContractAttachment(attachId);
        }

        public GeneratedMasterContractNumberVM GenerateMasterContractSerial()
        {
            return _unitOfWork.MasterContractRepository.GenerateMasterContractSerial();
        }

        public IEnumerable<MasterContract> GetAll()
        {
            return _unitOfWork.MasterContractRepository.GetAll();
        }

        public DetailMasterContractVM GetById(int id)
        {
            return _unitOfWork.MasterContractRepository.GetById(id);
        }

        public IEnumerable<ContractAttachment> GetContractAttachmentByMasterContractId(int masterContractId)
        {
            return _unitOfWork.MasterContractRepository.GetContractAttachmentByMasterContractId(masterContractId).ToList();
        }

        public ContractAttachment GetLastDocumentForMasterContractId(int masterContractId)
        {
            return _unitOfWork.MasterContractRepository.GetLastDocumentForMasterContractId(masterContractId);
        }

        public IndexMasterContractVM ListMasterContracts(SortAndFilterContractVM data, int pageNumber, int pageSize)
        {
            return _unitOfWork.MasterContractRepository.ListMasterContracts(data, pageNumber, pageSize);
        }

        public IndexMasterContractVM GetListBoxMasterContractsByHospitalId(int hospitalId)
        {
            return _unitOfWork.MasterContractRepository.GetListBoxMasterContractsByHospitalId(hospitalId);
        }

        //public IndexMasterContractVM Search(SearchContractVM model, int pageNumber, int pageSize)
        //{
        //    return _unitOfWork.MasterContractRepository.Search(model, pageNumber, pageSize);
        //}

        //public IEnumerable<IndexMasterContractVM.GetData> SortContracts(int hospitalId, SortContractsVM sortObj)
        //{
        //    return _unitOfWork.MasterContractRepository.SortContracts(hospitalId, sortObj);
        //}

        public int Update(MasterContract masterContractObj)
        {
            return _unitOfWork.MasterContractRepository.Update(masterContractObj);
        }
    }
}
