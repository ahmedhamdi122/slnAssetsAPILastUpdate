using Asset.Domain.Repositories;
using Asset.Models;
using Asset.ViewModels.AssetPeriorityVM;
using Asset.ViewModels.RequestVM;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset.Core.Repositories
{
    public class AssetPeriorityRepositories : IAssetPeriorityRepository
    {

        private ApplicationDbContext _context;


        public AssetPeriorityRepositories(ApplicationDbContext context)
        {
            _context = context;
        }



        public int Add(CreateAssetPeriorityVM model)
        {
            AssetPeriority assetPeriorityObj = new AssetPeriority();
            try
            {
                if (model != null)
                {
                    assetPeriorityObj.Name = model.Name;
                    assetPeriorityObj.NameAr = model.NameAr;
                    _context.AssetPeriorities.Add(assetPeriorityObj);
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                string str = ex.Message;
            }
            return assetPeriorityObj.Id;
        }

        public int Delete(int id)
        {
            var assetPeriorityObj = _context.AssetPeriorities.Find(id);
            try
            {
                if (assetPeriorityObj != null)
                {
                    _context.AssetPeriorities.Remove(assetPeriorityObj);
                    return _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }

            return 0;
        }

        public IEnumerable<IndexAssetPeriorityVM.GetData> GetAll()
        {
            return _context.AssetPeriorities.Where(a => a.Id < 4).ToList().Select(item => new IndexAssetPeriorityVM.GetData
            {
                Id = item.Id,
                Name = item.Name,
                NameAr = item.NameAr,
                Color = item.Color
            });
        }

        public IEnumerable<IndexAssetPeriorityVM.GetData> GetAllByHospitalId(int? hospitalId)
        {
            List<IndexRequestVM.GetData> list = new List<IndexRequestVM.GetData>();
            List<Request> highCount = new List<Request>();
            List<Request> mediumCount = new List<Request>();
            List<Request> normalCount = new List<Request>();
            List<Request> medicalCount = new List<Request>();
            List<Request> productionCount = new List<Request>();


            IQueryable<Request> lstAllRequests;
            if (hospitalId == 0)
            {
                lstAllRequests = _context.Request
                         .Include(t => t.AssetDetail).Include(t => t.AssetDetail.Department)
                         .Include(t => t.AssetDetail.MasterAsset)
                         .Include(t => t.AssetDetail.MasterAsset.brand)
                         .Include(t => t.AssetDetail.MasterAsset.AssetPeriority)
                          .OrderByDescending(a => a.RequestDate);
            }
            else
            {
                lstAllRequests = _context.Request
                             .Include(t => t.AssetDetail).Include(t => t.AssetDetail.Department)
                             .Include(t => t.AssetDetail.MasterAsset)
                             .Include(t => t.AssetDetail.MasterAsset.brand)
                              .Include(t => t.AssetDetail.MasterAsset.AssetPeriority)
                              .OrderByDescending(a => a.RequestDate)
                              .Where(a => a.AssetDetail.HospitalId == hospitalId);
            }
            var lstRequests = lstAllRequests.ToList();
            foreach (var req in lstRequests)
            {
                var lstStatus = _context.RequestTracking.Include(t => t.Request).Include(t => t.Request.AssetDetail)
                    .Include(t => t.Request.AssetDetail.MasterAsset)
                    .Include(t => t.Request.AssetDetail.MasterAsset.AssetPeriority)
                    .Where(a => a.RequestId == req.Id).ToList();
                if (lstStatus.Count == 1)
                {
                    foreach (var item in lstStatus)
                    {
                        IndexRequestVM.GetData getDataObj = new IndexRequestVM.GetData();
                        getDataObj.Id = req.Id;
                        if (req.AssetDetail.MasterAsset.PeriorityId != null)
                        {
                            getDataObj.PeriorityId = req.AssetDetail.MasterAsset != null ? (int)req.AssetDetail.MasterAsset.PeriorityId : 0;
                        }
                        getDataObj.PeriorityName = req.AssetDetail.MasterAsset.AssetPeriority != null ? req.AssetDetail.MasterAsset.AssetPeriority.Name : "";
                        getDataObj.PeriorityNameAr = req.AssetDetail.MasterAsset.AssetPeriority != null ? req.AssetDetail.MasterAsset.AssetPeriority.NameAr : "";
                        getDataObj.Color = req.AssetDetail.MasterAsset.AssetPeriority != null ? req.AssetDetail.MasterAsset.AssetPeriority.Color : "";
                        switch (req.AssetDetail.MasterAsset.PeriorityId)
                        {
                            case 1:
                                highCount.Add(req);
                                break;
                            case 2:
                                mediumCount.Add(req);
                                break;
                            case 3:
                                normalCount.Add(req);
                                break;

                            case 4:
                                medicalCount.Add(req);
                                break;

                            case 5:
                                productionCount.Add(req);
                                break;
                            default:
                                break;
                        }
                        list.Add(getDataObj);
                    }
                }
            }

            var lstPerioroties =  _context.AssetPeriorities.Where(a => a.Id < 4).ToList().Select(item => new IndexAssetPeriorityVM.GetData
            {
                Id = item.Id,
                Name = item.Name,
                NameAr = item.NameAr,
                Color = item.Color,
                HighCount = highCount.Count(),
                MediumCount = mediumCount.Count(),
                NormalCount = normalCount.Count(),
                MedicalCount = medicalCount.Count(),
                ProductionCount = productionCount.Count(),
                TotalCount = highCount.Count() + mediumCount.Count() + normalCount.Count() + medicalCount.Count() + productionCount.Count()
            });




            return lstPerioroties;
        }

        public EditAssetPeriorityVM GetById(int id)
        {
            return _context.AssetPeriorities.Where(a => a.Id == id).Select(item => new EditAssetPeriorityVM
            {
                Id = item.Id,
                Name = item.Name,
                NameAr = item.NameAr
            }).First();
        }

        public int Update(EditAssetPeriorityVM model)
        {
            try
            {

                var assetPeriorityObj = _context.AssetPeriorities.Find(model.Id);
                assetPeriorityObj.Name = model.Name;
                assetPeriorityObj.NameAr = model.NameAr;
                _context.Entry(assetPeriorityObj).State = EntityState.Modified;
                _context.SaveChanges();
                return assetPeriorityObj.Id;



            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }

            return 0;
        }
    }
}
