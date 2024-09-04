using Asset.API.Helpers;
using Asset.Domain.Services;
using Asset.Models;
using Asset.ViewModels.MasterAssetAttachmentVM;
using Asset.ViewModels.MasterAssetComponentVM;
using Asset.ViewModels.MasterAssetVM;
using Asset.ViewModels.PagingParameter;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Asset.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MasterAssetController : ControllerBase
    {
        private IMasterAssetService _MasterAssetService;
        private IAssetDetailService _assetDetailService;
        private IMasterAssetComponentService _masterAssetComponentService;
        IWebHostEnvironment _webHostEnvironment;
        [Obsolete]
        IHostingEnvironment _webHostingEnvironment;

        [Obsolete]
        public MasterAssetController(IMasterAssetService MasterAssetService, IMasterAssetComponentService masterAssetComponentService,
            IAssetDetailService assetDetailService, IHostingEnvironment webHostingEnvironment, IWebHostEnvironment webHostEnvironment)
        {
            _MasterAssetService = MasterAssetService;
            _assetDetailService = assetDetailService;
            _masterAssetComponentService = masterAssetComponentService;
            _webHostingEnvironment = webHostingEnvironment;
            _webHostEnvironment = webHostEnvironment;
        }


        [HttpPost]
        [Route("ListMasterAssets/{First}/{Rows}")]
        public ActionResult<IndexMasterAssetVM> GetAll(int First, int Rows, [FromBody] SearchSortMasterAssetVM SearchSortObj)
        {
            return _MasterAssetService.GetAll(First, Rows,SearchSortObj);
        }

        [HttpGet]
        [Route("GetTop10MasterAsset/{hospitalId}")]
        public IEnumerable<IndexMasterAssetVM.GetData> GetTop10MasterAsset(int hospitalId)
        {
            return _MasterAssetService.GetTop10MasterAsset(hospitalId);
        }

        [HttpGet]
        [Route("GetTop10MasterAssetCount/{hospitalId}")]
        public int GetTop10MasterAssetCount(int hospitalId)
        {
            var total = _MasterAssetService.GetTop10MasterAsset(hospitalId).ToList().Count();
            return total;
        }


        [HttpGet]
        [Route("ListMasterAssetsByHospitalUserId/{hospitalId}/{userId}")]
        public IEnumerable<MasterAsset> GetAllMasterAssetsByHospitalUserId(int hospitalId, string userId)
        {
            return _MasterAssetService.GetAllMasterAssetsByHospitalId(hospitalId, userId);
        }


        [HttpGet]
        [Route("GetListMasterAsset")]
        public IEnumerable<IndexMasterAssetVM.GetData> GetListMasterAsset()
        {
            return _MasterAssetService.GetListMasterAsset();
        }




        [HttpGet]
        [Route("GetMasterAssetIdByNameBrandModel/{name}/{brandId}/{model}")]
        public List<MasterAsset> GetMasterAssetIdByNameBrandModel(string name, int brandId, string model)
        {
            return _MasterAssetService.GetMasterAssetIdByNameBrandModel(name,brandId,model);
        }



        [HttpGet]
        [Route("AutoCompleteMasterAssetName/{name}")]
        public IEnumerable<MasterAsset> AutoCompleteMasterAssetName(string name)
        {
            return _MasterAssetService.AutoCompleteMasterAssetName(name);
        }
        [HttpGet]
        [Route("AutoCompleteMasterAssetName2/{name}")]
        public IEnumerable<IndexMasterAssetVM.GetData> AutoCompleteMasterAssetName2(string name)
        {
            return _MasterAssetService.AutoCompleteMasterAssetName2(name);
        }
        [HttpGet]
        [Route("AutoCompleteMasterAssetName3/{name}/{hospitalId}")]
        public IEnumerable<IndexMasterAssetVM.GetData> AutoCompleteMasterAssetName3(string name, int hospitalId)
        {
            return _MasterAssetService.AutoCompleteMasterAssetName3(name, hospitalId);
        }


        [HttpGet]
        [Route("DistinctAutoCompleteMasterAssetName/{name}")]
        public IEnumerable<MasterAsset> DistinctAutoCompleteMasterAssetName(string name)
        {
            return _MasterAssetService.DistinctAutoCompleteMasterAssetName(name);
        }


        [HttpGet]
        [Route("GetDistintMasterAssetModels/{brandId}/{name}")]
        public IEnumerable<string> GetDistintMasterAssetModels(int brandId,string name)
        {
            return _MasterAssetService.GetDistintMasterAssetModels(brandId, name);
        }


        [HttpGet]
        [Route("GetDistintMasterAssetBrands/{name}")]
        public IEnumerable<Brand> GetDistintMasterAssetBrands(string name)
        {
            return _MasterAssetService.GetDistintMasterAssetBrands(name);
        }


        [HttpGet]
        [Route("ListMasterAssetsByHospitalId/{hospitalId}")]
        public IEnumerable<MasterAsset> GetAllMasterAssetsByHospitalId(int hospitalId)
        {
            return _MasterAssetService.GetAllMasterAssetsByHospitalId(hospitalId);
        }

        [HttpGet]
        [Route("GetById/{id}")]
        public ActionResult<EditMasterAssetVM> GetById(int id)
        {
            return _MasterAssetService.GetById(id);
        }


        [HttpGet]
        [Route("ViewMasterAsset/{id}")]
        public ActionResult<ViewMasterAssetVM> ViewMasterAsset(int id)
        {
            return _MasterAssetService.ViewMasterAsset(id);
        }



        [Route("GetLastDocumentForMsterAssetId/{masterId}")]
        public MasterAssetAttachment GetLastDocumentForMsterAssetId(int masterId)
        {
            return _MasterAssetService.GetLastDocumentForMsterAssetId(masterId);
        }



        [HttpPut]
        [Route("UpdateMasterAsset")]
        public IActionResult Update(EditMasterAssetVM MasterAssetVM)
        {
            try
            {
                int id = MasterAssetVM.Id;
                if (MasterAssetVM.Code != null)
                {
                    var lstCode = _MasterAssetService.GetAllMasterAssets().Where(a => (a.Code == MasterAssetVM.Code && a.Code != null) && a.Id != id).ToList();
                    if (lstCode.Count > 0)
                    {
                        return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "code", Message = "MasterAsset code already exist", MessageAr = "هذا الكود مسجل سابقاً" });
                    }
                }
                var lstNames = _MasterAssetService.GetAllMasterAssets().ToList().Where(a => a.Name == MasterAssetVM.Name && a.ModelNumber == MasterAssetVM.ModelNumber && (a.VersionNumber == MasterAssetVM.VersionNumber && a.VersionNumber != null) && a.Id != id).ToList();
                if (lstNames.Count > 0)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "name", Message = "MasterAsset name already exist", MessageAr = "هذا الاسم مسجل سابقاً" });
                }
                var lstArNames = _MasterAssetService.GetAllMasterAssets().ToList().Where(a => a.NameAr == MasterAssetVM.NameAr && a.ModelNumber == MasterAssetVM.ModelNumber && (a.VersionNumber == MasterAssetVM.VersionNumber && a.VersionNumber != null) && a.Id != id).ToList();
                if (lstArNames.Count > 0)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "nameAr", Message = "MasterAsset arabic name already exist", MessageAr = "هذا الاسم مسجل سابقاً" });
                }
                else
                {
                    int updatedRow = _MasterAssetService.Update(MasterAssetVM);

                }
            }
            catch (DbUpdateConcurrencyException ex)
            {
                string msg = ex.Message;
                return BadRequest("Error in update");
            }

            return Ok(MasterAssetVM.Id);
        }


        [HttpPut]
        [Route("UpdateMasterAssetImageAfterInsert")]
        public IActionResult Update(CreateMasterAssetVM masterAssetObj)
        {
            try
            {
                int updatedRow = _MasterAssetService.UpdateMasterAssetImageAfterInsert(masterAssetObj);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                string msg = ex.Message;
                return BadRequest("Error in update");
            }

            return Ok(masterAssetObj.Id);
        }


        [HttpPost]
        [Route("AddMasterAsset")]
        public ActionResult Add(CreateMasterAssetVM MasterAssetVM)
        { 
            if(MasterAssetVM.Code==null)       
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "CodeRequired", Message = "Code Required", MessageAr = "لابد من كتابه كود" });
            }

            if (MasterAssetVM.Code.Length > 5)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "codelen", Message = "code must be maximum 5  charchters", MessageAr = "هذا الكود اقصى حد  له 5 حروف وأرقام " });
            }
            if (MasterAssetVM.Name==null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "nameRequired", Message = "Name Required", MessageAr = "يجب ادخال اسم" });
            }
            if (MasterAssetVM.NameAr == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "nameAr", Message = "NameAr Required", MessageAr = "يجب ادخال اسم بالعربي" });
            }
            if (MasterAssetVM.BrandId == 0 || MasterAssetVM.BrandId==null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "brand", Message = "You should select Brand", MessageAr = "لابد من اختيار الماركة" });
            }
            var CodeExists = _MasterAssetService.CheckMasterAssetCodeExists(MasterAssetVM.Code);
            if (CodeExists)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "codeExists", Message = "MasterAsset code already exist", MessageAr = "هذا الكود مسجل سابقاً" });
            }
            var ExistsByNameModelAndVersion=false;
            if (MasterAssetVM.Name!=null)
            {
                ExistsByNameModelAndVersion= _MasterAssetService.ExistsByNameModelAndVersion(MasterAssetVM.Name, MasterAssetVM.ModelNumber, MasterAssetVM.VersionNumber);
            }
            if (ExistsByNameModelAndVersion)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "ExistsByNameModelAndVersion", Message = "Can't add because an asset with the same asset name, model number, and version number already exists.", MessageAr = "لا يمكن الاضافة بسبب وجود جهاز له اسم الأصل ورقم الموديل ورقم الإصدار بالفعل" });
                }
            var ExistsByNameArModelAndVersion = false;
            if (MasterAssetVM.NameAr != null)
            {
                ExistsByNameArModelAndVersion = _MasterAssetService.ExistsByNameArModelAndVersion(MasterAssetVM.NameAr, MasterAssetVM.ModelNumber, MasterAssetVM.VersionNumber);
            }
            if (ExistsByNameArModelAndVersion)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "ExistsByNameArModelAndVersion", Message = "Can't add because an asset with the same asset nameAr, model number, and version number already exists.", MessageAr = "لا يمكن الاضافة بسبب وجود جهاز له اسم الأصل بالعربي ورقم الموديل ورقم الإصدار بالفعل" });
            }
         
                var savedId = _MasterAssetService.Add(MasterAssetVM);
                return Ok(savedId);
        }

        [HttpDelete]
        [Route("DeleteMasterAsset/{id}")]
        public ActionResult<MasterAsset> Delete(int id)
        {
            try
            {
                var isMasterAssetExists=_MasterAssetService.isMasterAssetExistsUsingId(id);
                if(!isMasterAssetExists)
                {
                    return StatusCode(StatusCodes.Status404NotFound, new Response { Status = "masterAsset", Message = "Can't delete becuase not found", MessageAr = "لا يوجد " });
                }
                var hasAssetDetailsWithMasterId = _assetDetailService.hasAssetWithMasterId(id);
                if (hasAssetDetailsWithMasterId)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "code", Message = "Cannot delete asset,There are existing assets associated with the specified master asset", MessageAr = "لا يمكن مسح الأصل الرئيسي بسبب وجود أصل مستشفى او أكثر مرتبط به " });
                }

                   int deletedRow = _MasterAssetService.Delete(id);
                   
                
            }
            catch (DbUpdateConcurrencyException ex)
            {
                string msg = ex.Message;
                return BadRequest("Error in delete");
            }

            return Ok();
        }


        [HttpPost]
        [Route("CreateMasterAssetAttachments")]
        public int CreateMasterAssetAttachments(CreateMasterAssetAttachmentVM attachObj)
        {
            return _MasterAssetService.CreateMasterAssetDocuments(attachObj);
        }
        [HttpPost]
        [Route("UploadMasterAssetFiles")]
        [Obsolete]
        public ActionResult UploadInFiles(IFormFile file)
        {
            var folderPath = _webHostingEnvironment.ContentRootPath + "/UploadedAttachments/MasterAssets";
            bool exists = System.IO.Directory.Exists(folderPath);
            if (!exists)
                System.IO.Directory.CreateDirectory(folderPath);
            string filePath = folderPath + "/" + file.FileName;
            if (System.IO.File.Exists(filePath))
            {
            }
            else
            {
                Stream stream = new FileStream(filePath, FileMode.Create);
                file.CopyTo(stream);
                stream.Close();
            }
            return StatusCode(StatusCodes.Status201Created);
        }


        [HttpPost]
        [Route("UploadMasterAssetImage")]
        [Obsolete]
        public ActionResult UploadMasterAssetImage(IFormFile file)
        {

            var folderPath = _webHostingEnvironment.ContentRootPath + "/UploadedAttachments/MasterAssets/UploadMasterAssetImage";
            bool exists = System.IO.Directory.Exists(folderPath);
            if (!exists)
                System.IO.Directory.CreateDirectory(folderPath);

            string filePath = folderPath + "/" + file.FileName;
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
                Stream stream = new FileStream(filePath, FileMode.Create);
                file.CopyTo(stream);
                stream.Close();
            }
            else
            {
                Stream stream = new FileStream(filePath, FileMode.Create);
                file.CopyTo(stream);
                stream.Close();
            }

            return StatusCode(StatusCodes.Status201Created);
        }
     
        [HttpGet]
        [Route("GetAttachmentByMasterAssetId/{assetId}")]
        public IEnumerable<MasterAssetAttachment> GetAttachmentByMasterAssetId(int assetId)
        {
            return _MasterAssetService.GetAttachmentByMasterAssetId(assetId);
        }



        [HttpDelete]
        [Route("DeleteMasterAssetAttachment/{id}")]
        public int DeleteMasterAssetAttachment(int id)
        {
            return _MasterAssetService.DeleteMasterAssetAttachment(id);
        }



        [HttpGet]
        [Route("CountMasterAssets")]
        public int CountMasterAssets()
        {
            return _MasterAssetService.CountMasterAssets();
        }

        [HttpGet]
        [Route("CountMasterAssetsByBrand/{hospitalId}")]
        public List<CountMasterAssetBrands> CountMasterAssetsByBrand(int hospitalId)
        {
            return _MasterAssetService.CountMasterAssetsByBrand(hospitalId);
        }

        [HttpGet]
        [Route("CountMasterAssetsBySupplier/{hospitalId}")]
        public List<CountMasterAssetSuppliers> CountMasterAssetsBySupplier(int hospitalId)
        {
            return _MasterAssetService.CountMasterAssetsBySupplier(hospitalId);
        }


        [HttpDelete]
        [Route("DeleteMasterAssetImage/{id}")]
        public ActionResult DeleteMasterAssetImage(int id)
        {
            var masterAssetObj = _MasterAssetService.GetById(id);
            var folderPath = _webHostEnvironment.ContentRootPath + "/UploadedAttachments/MasterAssets/UploadMasterAssetImage/" + masterAssetObj.AssetImg;
            bool exists = System.IO.File.Exists(folderPath);
            if (exists)
            {
                System.IO.File.Delete(folderPath);
                masterAssetObj.AssetImg = "";
                _MasterAssetService.Update(masterAssetObj);
            }
            return Ok();
        }


        [HttpGet]
        [Route("GenerateMasterAssetcode")]
        public GeneratedMasterAssetCodeVM GenerateMasterAssetcode()
        {
            return _MasterAssetService.GenerateMasterAssetcode();
        }



    }
}
