﻿using Asset.Domain;
using Asset.Domain.Services;
using Asset.Models;
using Asset.ViewModels.ScrapVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset.Core.Services
{
    public class ScrapService : IScrapService
    {
        private IUnitOfWork _unitOfWork;

        public ScrapService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public int Add(CreateScrapVM createScrapVM)
        {
            return _unitOfWork.scrapRepository.Add(createScrapVM);
        }

        public int CreateScrapAttachments(ScrapAttachment attachObj)
        {
            return _unitOfWork.scrapRepository.CreateScrapAttachments(attachObj);
        }

        public List<IndexScrapVM.GetData> GetAll()
        {
            return _unitOfWork.scrapRepository.GetAll();
        }
        public ViewScrapVM ViewScrapById(int id)
        {
            return _unitOfWork.scrapRepository.ViewScrapById(id);
        }
        public Scrap GetById(int id)
        {
            return _unitOfWork.scrapRepository.GetById(id);
        }


        public IndexScrapVM.GetData GetById2(int id)
        {
            return _unitOfWork.scrapRepository.GetById2(id);
        }
        public GeneratedScrapNumberVM GenerateScrapNumber()
        {
            return _unitOfWork.scrapRepository.GenerateScrapNumber();
        }
        public IEnumerable<ScrapAttachment> GetScrapAttachmentByScrapId(int scrapId)
        {
            return _unitOfWork.scrapRepository.GetScrapAttachmentByScrapId(scrapId);
        }

        public IEnumerable<IndexScrapVM.GetData> SearchInScraps(SearchScrapVM searchObj)
        {
            return _unitOfWork.scrapRepository.SearchInScraps(searchObj);
        }
        public int Delete(int id)
        {
            var scrapObj = _unitOfWork.scrapRepository.GetById(id);
            _unitOfWork.scrapRepository.Delete(id);
            _unitOfWork.CommitAsync();

            return scrapObj.Id;

        }

        public IndexScrapVM ListScraps(SortAndFilterScrapVM data, int pageNumber, int pageSize)
        {
            return _unitOfWork.scrapRepository.ListScraps(data, pageNumber,pageSize);
        }

        public List<ViewScrapVM> GetScrapReasonByScrapId(int scrapId)
        {
            return _unitOfWork.scrapRepository.GetScrapReasonByScrapId(scrapId);
        }

        public IndexScrapVM SearchInScraps(SearchScrapVM searchObj, int pageNumber, int pageSize)
        {
            return _unitOfWork.scrapRepository.SearchInScraps(searchObj, pageNumber, pageSize);
        }

        public IndexScrapVM GetAllScraps(int pageNumber, int pageSize)
        {
            return _unitOfWork.scrapRepository.GetAllScraps(pageNumber, pageSize);
        }

        public List<IndexScrapVM.GetData> PrintListOfScraps(List<IndexScrapVM.GetData> scraps)
        {
            return _unitOfWork.scrapRepository.PrintListOfScraps(scraps);
        }
    }
}
