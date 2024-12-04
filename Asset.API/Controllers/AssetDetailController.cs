using Asset.Domain.Services;
using Asset.Models;
using Asset.ViewModels.AssetDetailAttachmentVM;
using Asset.ViewModels.AssetDetailVM;
using Asset.ViewModels.BrandVM;
using Asset.ViewModels.CityVM;
using Asset.ViewModels.GovernorateVM;
using Asset.ViewModels.HospitalVM;
using Asset.ViewModels.OrganizationVM;
using Asset.ViewModels.PMAssetTaskScheduleVM;
using Asset.ViewModels.PmAssetTimeVM;
using Asset.ViewModels.SupplierVM;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Asset.API.Helpers;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Globalization;
using System.Net.Http;
using System.Net;
using System.Net.Http.Headers;
using System.Data;
//using Syncfusion.DocIO.DLS;
//using Syncfusion.DocIO;
//using Syncfusion.Pdf.Barcode;
//using Syncfusion.Pdf.Graphics;
using System.Drawing;
using Rectangle = iTextSharp.text.Rectangle;
using System.Drawing.Imaging;
using Asset.ViewModels.RequestVM;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Threading.Tasks;
using Asset.ViewModels.WorkOrderVM;
using Asset.ViewModels.PMAssetTaskVM;
using System.Threading;

namespace Asset.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssetDetailController : ControllerBase
    {
        #region Members
        private IAssetDetailService _AssetDetailService;
        private IWorkOrderService _workOrderService;
        private IAssetOwnerService _assetOwnerService;
        private IAssetStatusTransactionService _assetStatusTransactionService;
        private IAssetMovementService _assetMovementService;
        private IRequestService _requestService;
        private IPMAssetTimeService _pMAssetTimeService;
        private IPagingService _pagingService;
        private QrController _qrController;
        IWebHostEnvironment _webHostingEnvironment;
        string strInsitute, strInsituteAr, strLogo = "";
        bool isAgency = false;
        private readonly ISettingService _settingService;
        IHttpContextAccessor _httpContextAccessor;
        int i = 1;
        #endregion


        [Obsolete]
        public AssetDetailController(IAssetDetailService AssetDetailService, IAssetOwnerService assetOwnerService, IWorkOrderService workOrderService,
            IPMAssetTimeService pMAssetTimeService, IPagingService pagingService, IAssetMovementService assetMovementService, IAssetStatusTransactionService assetStatusTransactionService,
            QrController qrController, IRequestService requestService, IWebHostEnvironment webHostingEnvironment, ISettingService settingService, IHttpContextAccessor httpContextAccessor)
        {
            _AssetDetailService = AssetDetailService;
            _assetMovementService = assetMovementService;
            _requestService = requestService;
            _webHostingEnvironment = webHostingEnvironment;
            _assetOwnerService = assetOwnerService;
            _pMAssetTimeService = pMAssetTimeService;
            _pagingService = pagingService;
            _qrController = qrController;
            _settingService = settingService;
            _workOrderService = workOrderService;
            _assetStatusTransactionService = assetStatusTransactionService;
            _httpContextAccessor = httpContextAccessor;
        }


        #region Main Funtions of Assets


        /// <summary>
        /// List All Hospital Assets
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("ListAssetDetails")]
        public IEnumerable<IndexAssetDetailVM.GetData> GetAll()
        {
            return _AssetDetailService.GetAll();
        }




        /// <summary>
        /// Get Hospital Asset By Id
        /// </summary>
        /// <param name="assetId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetAssetHistoryById/{assetId}")]
        public ViewAssetDetailVM GetAssetHistoryById(int assetId)
        {
            return _AssetDetailService.GetAssetHistoryById(assetId);
        }

        /// <summary>
        /// Get Hospital Asset By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetById/{id}")]
        public EditAssetDetailVM GetById(int id)
        {
            return _AssetDetailService.GetById(id);
        }


        /// <summary>
        /// Get Hospital Asset By assetId
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetAssetDetailsByAssetId/{assetId}")]
        public IEnumerable<IndexAssetDetailVM.GetData> GetAssetDetailsByAssetId(int assetId)
        {
            return _AssetDetailService.GetAssetDetailsByAssetId(assetId);
        }

        /// <summary>
        /// Update  Hospital Asset 
        /// </summary>
        /// <param name="AssetDetailVM"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("UpdateAssetDetail")]
        public IActionResult Update(EditAssetDetailVM AssetDetailVM)
        {
            try
            {
                //a.BarCode == AssetDetailVM.Barcode && a.SerialNumber == AssetDetailVM.SerialNumber
                int id = AssetDetailVM.Id;
                if (!string.IsNullOrEmpty(AssetDetailVM.Code))
                {
                    var lstCode = _AssetDetailService.GetAll().Where(a => a.Code == AssetDetailVM.Code && a.Id != id).ToList();
                    if (lstCode.Count > 0)
                    {
                        return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "code", Message = "Asset code already exist", MessageAr = "هذا الكود مسجل سابقاً" });
                    }
                }
                var lstNames = _AssetDetailService.GetAll().ToList().Where(a => a.BarCode == AssetDetailVM.Barcode && a.SerialNumber == AssetDetailVM.SerialNumber && a.Id != id).ToList();
                if (lstNames.Count > 0)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "serial", Message = "Asset serial already exist", MessageAr = "هذا السيريال مسجل سابقاً" });
                }

                else
                {
                    // var domainName = "http://" + HttpContext.Request.Host.Value;
                    var domainName = "http://" + _httpContextAccessor.HttpContext.Request.Host.Value;
                    AssetDetailVM.DomainName = domainName;
                    int updatedRow = _AssetDetailService.Update(AssetDetailVM);
                }
            }
            catch (DbUpdateConcurrencyException ex)
            {
                string msg = ex.Message;
                return BadRequest("Error in update");
            }
            return Ok();
        }

        /// <summary>
        /// Create Barcode for new asset Detail
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GenerateAssetDetailBarcode")]
        public GeneratedAssetDetailBCVM GenerateAssetDetailBarcode()
        {
            return _AssetDetailService.GenerateAssetDetailBarcode();
        }

        /// <summary>
        /// Create Hospital Asset
        /// </summary>
        /// <param name="AssetDetailVM"></param>
        /// <returns></returns>

        

        [HttpPost]
        [Route("AddAssetDetail")]
        public ActionResult Add(CreateAssetDetailVM AssetDetailVM)
        {
            var CodeExists = _AssetDetailService.CheckAssetDetailCodeExists(AssetDetailVM.Code);
            if (CodeExists)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "code", Message = "Asset code already exist", MessageAr = "هذا الكود مسجل سابقاً" });
            }
            var lstNames = _AssetDetailService.GetAll().ToList().Where(a => a.BarCode == AssetDetailVM.Barcode && a.SerialNumber == AssetDetailVM.SerialNumber).ToList();
            if (lstNames.Count > 0)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "name", Message = "Asset already exist with this data", MessageAr = "هذا الجهاز مسجل سابقاً" });
            }
            else
            {
                var savedId = _AssetDetailService.Add(AssetDetailVM);
                _qrController.Index(AssetDetailVM.Id);

                //CreateAssetDetailAttachmentVM qrAttach = new CreateAssetDetailAttachmentVM();
                //qrAttach.AssetDetailId = AssetDetailVM.Id;
                //qrAttach.FileName = "asset-" + AssetDetailVM.Id + ".png";
               // CreateAssetDetailAttachments(qrAttach);
                return Ok(savedId);
            }
        }

        /// <summary>
        /// Delete Hospital Asset by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("DeleteAssetDetail/{id}")]
        public ActionResult<AssetDetail> Delete(int id)
        {
            try
            {
                var assetObj = _AssetDetailService.GetById(id);
                var lstMovements = _assetMovementService.GetMovementByAssetDetailId(id).ToList();
                if (lstMovements.Count > 0)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "move", Message = "You cannot delete this asset it has movement", MessageAr = "لا يمكن مسح هذا الأصل لأن له حركات في المستشفى" });
                }
                var lstRequests = _requestService.GetAllRequestsByAssetId(id, int.Parse(assetObj.HospitalId.ToString())).ToList();
                if (lstRequests.Count > 0)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "request", Message = "You cannot delete this asset it has requests", MessageAr = "لا يمكن مسح هذا الأصل لأن له بلاغات أعطال " });
                }
                var lstWO = _workOrderService.GetLastRequestAndWorkOrderByAssetId(id).ToList();
                if (lstWO.Count > 0)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "wo", Message = "You cannot delete this asset it has workorders", MessageAr = "لا يمكن مسح هذا الأصل لأن له  أوامر شغل" });
                }
                else
                {

                    var lstOwners = _assetOwnerService.GetOwnersByAssetDetailId(id).ToList();
                    if (lstOwners.Count > 0)
                    {
                        foreach (var item in lstOwners)
                        {
                            _assetOwnerService.Delete(item.Id);
                        }
                    }

                    var lstAssetTransactions = _assetStatusTransactionService.GetAssetStatusByAssetDetailId(id).ToList();
                    if (lstAssetTransactions.Count > 0)
                    {
                        foreach (var item in lstAssetTransactions)
                        {
                            _assetStatusTransactionService.Delete(item.Id);
                        }
                    }


                    int deletedRow = _AssetDetailService.Delete(id);
                }
            }
            catch (DbUpdateConcurrencyException ex)
            {
                string msg = ex.Message;
                return BadRequest("Error in delete");
            }

            return Ok();
        }

        /// <summary>
        /// Get asset owners for each device 
        /// </summary>
        /// <param name="assetDetailId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetOwnersByAssetDetailId/{assetDetailId}")]
        public List<AssetOwner> GetOwnersByAssetDetailId(int assetDetailId)
        {
            return _assetOwnerService.GetOwnersByAssetDetailId(assetDetailId).ToList();
        }

        /// <summary>
        /// Save Attachments by Hospital Asset Id
        /// </summary>
        /// <param name="attachObj"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("CreateAssetDetailAttachments")]
        public int CreateAssetDetailAttachments(CreateAssetDetailAttachmentVM attachObj)
        {
            return _AssetDetailService.CreateAssetDetailDocuments(attachObj);
        }



        /// <summary>
        ///  Upload Attachments by Hospital Asset Id to Folder 'AssetDetails' in Application
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>       
        [HttpPost]
        [Route("UploadAssetDetailFiles")]
        [Obsolete]
        public ActionResult UploadInFiles(IFormFile file)
        {
            string path = _webHostingEnvironment.ContentRootPath + "/UploadedAttachments/AssetDetails/" + file.FileName;
            if (!System.IO.File.Exists(path))
            {
                Stream stream = new FileStream(path, FileMode.Create);
                file.CopyTo(stream);
                stream.Close();
            }
            return StatusCode(StatusCodes.Status201Created);
        }

        /// <summary>
        /// Get all attachments related by asset detail Id
        /// </summary>
        /// <param name="assetId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetAttachmentByAssetDetailId/{assetId}")]
        public IEnumerable<AssetDetailAttachment> GetAttachmentByAssetDetailId(int assetId)
        {
            return _AssetDetailService.GetAttachmentByAssetDetailId(assetId);
        }

        /// <summary>
        /// Delete attachment by Id depend on asset Detail Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("DeleteAssetDetailAttachment/{id}")]
        public int DeleteAssetDetailAttachment(int id)
        {
            return _AssetDetailService.DeleteAssetDetailAttachment(id);
        }

        /// <summary>
        /// View AssetDetail By MasterId
        /// </summary>
        /// <param name="masterId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("ViewAssetDetailByMasterId/{masterId}")]
        public ActionResult<ViewAssetDetailVM> ViewAssetDetailByMasterId(int masterId)
        {
            return _AssetDetailService.ViewAssetDetailByMasterId(masterId);
        }



        /// <summary>
        ///  View All AssetDetail By MasterId
        /// </summary>
        /// <param name="MasterAssetId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("ViewAllAssetDetailByMasterId/{MasterAssetId}")]
        public IEnumerable<AssetDetail> ViewAllAssetDetailByMasterId(int MasterAssetId)
        {
            return _AssetDetailService.ViewAllAssetDetailByMasterId(MasterAssetId);
        }


        //[HttpPost]
        //[Route("GetAssetDetailsByUserIdWithPaging/{pagenumber}/{pagesize}/{userId}")]
        //public async Task<IndexAssetDetailVM> GetAssetDetailsByUserId2(int pageNumber, int pageSize, string userId)
        //{
        //    var lstAssetDetails = await _AssetDetailService.GetAssetDetailsByUserId2(pageNumber, pageSize, userId);
        //    return lstAssetDetails;
        //}


        #endregion

        #region AutoComplete
        /// <summary>
        /// AutoComplete Asset Barcode depend on HospitalId
        /// </summary>
        /// <param name="barcode"></param>
        /// <param name="hospitalId"></param>
        /// <returns>List of Assets that contains barcode</returns>
        [HttpGet]
        [Route("AutoCompleteAssetBarCode/{barcode}/{hospitalId}/{UserId}")]
        public async Task<IEnumerable<IndexAssetDetailVM.GetData>> AutoCompleteAssetBarCode(string barcode, int hospitalId,string UserId)
        {
            return await _AssetDetailService.AutoCompleteAssetBarCode(barcode, hospitalId, UserId);
        }


        /// <summary>
        /// AutoComplete Asset serial depend on HospitalId
        /// </summary>
        /// <param name="serial"></param>
        /// <param name="hospitalId"></param>
        /// <returns>List of Assets that contains SerialNumber</returns>
        [HttpGet]
        [Route("AutoCompleteAssetSerial/{serial}/{hospitalId}")]
        public IEnumerable<IndexAssetDetailVM.GetData> AutoCompleteAssetSerial(string serial, int hospitalId)
        {
            return _AssetDetailService.AutoCompleteAssetSerial(serial, hospitalId);
        }



        /// <summary>
        /// AutoComplete Asset Barcode depend on HospitalId
        /// </summary>
        /// <param name="barcode"></param>
        /// <param name="hospitalId"></param>
        /// <returns>List of Assets that contains barcode</returns>
        [HttpGet]
        [Route("AutoCompleteAssetBarCodeByDepartmentId/{barcode}/{hospitalId}/{departmentId}")]
        public IEnumerable<IndexAssetDetailVM.GetData> AutoCompleteAssetBarCodeByDepartmentId(string barcode, int hospitalId, int departmentId)
        {
            return _AssetDetailService.AutoCompleteAssetBarCodeByDepartmentId(barcode, hospitalId, departmentId);
        }





        [HttpGet]
        [Route("DownloadAssetHistory/{fileName}")]
        public HttpResponseMessage DownloadSRReportWithInProgressPDF(string fileName)
        {
            var file = _webHostingEnvironment.ContentRootPath + "/UploadedAttachments/AssetDetails/" + fileName;
            HttpResponseMessage response = null;
            if (!System.IO.File.Exists(file))
            {
                // return new HttpResponseMessage(HttpStatusCode.Gone);
                var folder = Directory.CreateDirectory(_webHostingEnvironment.ContentRootPath + "/UploadedAttachments/AssetDetails/");
                var openFile = System.IO.File.Create(folder + fileName);
                openFile.Close();


                var file2 = folder + fileName;
                var fStream = new FileStream(file2, FileMode.Open, FileAccess.Read);
                response = new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StreamContent(fStream)
                };
                response.Content.Headers.ContentDisposition =
                                            new ContentDispositionHeaderValue("attachment")
                                            {
                                                FileName = Path.GetFileName(fStream.Name)
                                            };
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            }
            else
            {
                //if file present than read file 
                var fStream = new FileStream(file, FileMode.Open, FileAccess.Read);
                //compose response and include file as content in it
                response = new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StreamContent(fStream)
                };
                response.Content.Headers.ContentDisposition =
                                            new ContentDispositionHeaderValue("attachment")
                                            {
                                                FileName = Path.GetFileName(fStream.Name)
                                            };
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            }
            return response;
        }




        [HttpGet]
        [Route("PrintAssetHistory/{assetId}/{lang}")]
        public void PrintAssetHistory(int assetId, string lang)
        {


            var lstSettings = _settingService.GetAll().ToList();
            if (lstSettings.Count > 0)
            {
                foreach (var item in lstSettings)
                {
                    if (item.KeyName == "Institute")
                    {
                        strInsitute = item.KeyValue;
                        strInsituteAr = item.KeyValueAr;
                    }
                    if (item.KeyName == "Logo")
                        strLogo = item.KeyValue;

                }
            }

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            Document document = new Document(iTextSharp.text.PageSize.A4.Rotate(), 20f, 20f, 30f, 20f);
            System.IO.MemoryStream memoryStream = new System.IO.MemoryStream();
            PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);
            document.NewPage();
            document.Open();

            //  document.SetPageSize(iTextSharp.text.PageSize.A4); 

            string adobearabic = _webHostingEnvironment.ContentRootPath + "/Font/adobearabic.ttf";
            BaseFont bfUniCode = BaseFont.CreateFont(adobearabic, BaseFont.IDENTITY_H, true);
            iTextSharp.text.Font font = new iTextSharp.text.Font(bfUniCode, 14);
            iTextSharp.text.Font headerfont = new iTextSharp.text.Font(bfUniCode, 22);
            iTextSharp.text.Font titlefont = new iTextSharp.text.Font(bfUniCode, 16);



            var assetDetailObj = _AssetDetailService.QueryAssetDetailById(assetId);

            string imageURL = _webHostingEnvironment.ContentRootPath + "/Images/" + strLogo;
            iTextSharp.text.Image jpg = iTextSharp.text.Image.GetInstance(imageURL);
            jpg.ScaleAbsolute(80f, 70f);
            PdfPTable headertable = new PdfPTable(3);

            if (lang == "ar")
            {

                headertable.SetTotalWidth(new float[] { 40f, 85f, 30f });
                headertable.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
                headertable.WidthPercentage = 100;
                PdfPCell headerCell = new PdfPCell(new PdfPCell(jpg));
                headerCell.PaddingTop = 5;
                headerCell.Border = Rectangle.NO_BORDER;
                headerCell.PaddingRight = 30;
                headerCell.HorizontalAlignment = 2; //0=Left, 1=Centre, 2=Right
                headertable.AddCell(headerCell);

            }
            else
            {
                headertable.SetTotalWidth(new float[] { 30f, 100f, 40f });
                headertable.WidthPercentage = 100;
                PdfPCell headerCell = new PdfPCell(new PdfPCell(jpg));
                headerCell.PaddingTop = 5;
                headerCell.Border = Rectangle.NO_BORDER;
                headerCell.PaddingLeft = 60;
                headerCell.HorizontalAlignment = 0; //0=Left, 1=Centre, 2=Right
                headertable.AddCell(headerCell);
            }

            if (lang == "ar")
                headertable.AddCell(new PdfPCell(new Phrase("\n\t\t\t\t " + strInsituteAr + "\n" + assetDetailObj.Hospital.NameAr + "", font)) { Border = Rectangle.NO_BORDER, PaddingTop = 5 });
            else
                headertable.AddCell(new PdfPCell(new Phrase("\n" + strInsitute + "\n" + assetDetailObj.Hospital.Name + "", font)) { Border = Rectangle.NO_BORDER, PaddingTop = 10 });



            PdfPCell imagesCell = new PdfPCell();
            PdfContentByte cb = new PdfContentByte(writer);
            Barcode128 code128 = new Barcode128();
            code128.CodeType = Barcode.CODE128;
            code128.Code = assetDetailObj.Barcode;
            code128.BarHeight = 35f;
            code128.X = 1f;
            iTextSharp.text.Image image128 = code128.CreateImageWithBarcode(cb, null, null);
            image128.Border = Rectangle.NO_BORDER;
            image128.PaddingTop = 80;



            if (!string.IsNullOrEmpty(assetDetailObj.MasterAsset.AssetImg))
            {
                string assetImagePath = _webHostingEnvironment.ContentRootPath + "/UploadedAttachments/MasterAssets/UploadMasterAssetImage/" + assetDetailObj.MasterAsset.AssetImg;
                iTextSharp.text.Image assetImage = iTextSharp.text.Image.GetInstance(assetImagePath);
                assetImage.ScaleAbsolute(40f, 40f);
                Paragraph p = new Paragraph();
                p.Add(new Chunk(image128, 10, 30, true));
                p.Add(new Chunk(assetImage, 0, 0, true));
                imagesCell.AddElement(p);

            }
            else
            {
                string assetNullImagePath = _webHostingEnvironment.ContentRootPath + "/UploadedAttachments/MasterAssets/UploadMasterAssetImage/UnknownAsset.png";
                iTextSharp.text.Image assetImage = iTextSharp.text.Image.GetInstance(assetNullImagePath);
                if (lang == "ar")
                {
                    assetImage.ScaleAbsolute(50f, 50f);
                    Paragraph p = new Paragraph();
                    p.PaddingTop = 50;
                    p.Add(new Chunk(image128, 10, 0, true));
                    p.Add(new Chunk(assetImage, 0, 0, true));
                    imagesCell.AddElement(p);
                }
                else
                {
                    assetImage.ScaleAbsolute(50f, 50f);
                    Paragraph p = new Paragraph();
                    p.PaddingTop = 50;
                    p.Add(new Chunk(image128, 0, 0, true));
                    p.Add(new Chunk(assetImage, 15, 0, true));
                    imagesCell.AddElement(p);
                }
            }
            headertable.AddCell(new PdfPCell(new PdfPCell(imagesCell) { Border = Rectangle.NO_BORDER }));
            document.Add(headertable);



            PdfPTable titleTable = new PdfPTable(1);
            titleTable.SetTotalWidth(new float[] { 800f });
            titleTable.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
            titleTable.WidthPercentage = 100;
            if (lang == "ar")
                titleTable.AddCell(new PdfPCell(new Phrase("تاريخ الأصل", headerfont)) { PaddingBottom = 10, Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_CENTER });

            else
                titleTable.AddCell(new PdfPCell(new Phrase("Asset History")) { PaddingBottom = 10, Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_CENTER });
            document.Add(titleTable);

            if (lang == "ar")
            {
                PdfPTable table = new PdfPTable(4);
                table.DefaultCell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                table.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
                PdfPCell cell = new PdfPCell(new Phrase("البيانات الرئيسة", titlefont));
                cell.BackgroundColor = new BaseColor(153, 204, 255);
                cell.PaddingBottom = 10;
                cell.Border = 0;
                cell.Colspan = 4;
                cell.HorizontalAlignment = 0; //0=Left, 1=Centre, 2=Right
                table.AddCell(cell);
                table.AddCell(new Phrase("المحافظة", font));
                table.AddCell(new Phrase(assetDetailObj.Hospital.Governorate.NameAr, font));
                table.AddCell(new Phrase("المدينة", font));
                table.AddCell(new Phrase(assetDetailObj.Hospital.City.NameAr, font));
                table.AddCell(new Phrase("الهيئة", font));
                table.AddCell(new Phrase(assetDetailObj.Hospital.Organization.NameAr, font));
                table.AddCell(new Phrase("الهيئة الفرعية", font));
                table.AddCell(new Phrase(assetDetailObj.Hospital.SubOrganization.NameAr, font));
                table.AddCell(new Phrase("الفئة", font));
                if (assetDetailObj.MasterAsset.Category != null)
                    table.AddCell(new Phrase(assetDetailObj.MasterAsset.Category.NameAr, font));
                else
                    table.AddCell(new Phrase(" ", font));
                table.AddCell(new Phrase("الفئة الفرعية", font));
                if (assetDetailObj.MasterAsset.SubCategory != null)
                    table.AddCell(new Phrase(assetDetailObj.MasterAsset.SubCategory.NameAr, font));
                else
                    table.AddCell(new Phrase(" ", font));
                table.AddCell(new Phrase("الأولوية", font));
                table.AddCell(new Phrase(assetDetailObj.MasterAsset.AssetPeriority.NameAr, font));
                table.AddCell(new Phrase("القسم", font));
                table.AddCell(new Phrase(assetDetailObj.Department.NameAr, font));
                table.AddCell(new Phrase("الباركود", font));
                table.AddCell(new Phrase(ArabicNumeralHelper.toArabicNumber(assetDetailObj.Barcode), font));
                table.AddCell(new Phrase("السيريال", font));
                table.AddCell(new Phrase(assetDetailObj.SerialNumber, font));
                table.AddCell(new Phrase("الموديل", font));
                table.AddCell(new Phrase(assetDetailObj.MasterAsset.ModelNumber, font));
                table.AddCell(new Phrase("الماركة", font));
                table.AddCell(new Phrase(assetDetailObj.MasterAsset.brand.NameAr, font));
                table.AddCell(new Phrase("المورد", font));
                table.AddCell(new Phrase(assetDetailObj.Supplier.NameAr, font));
                table.AddCell(new Phrase("بلد المنشأ", font));

                if (assetDetailObj.MasterAsset.Origin != null)
                    table.AddCell(new Phrase(assetDetailObj.MasterAsset.Origin.NameAr, font));
                else
                    table.AddCell(new Phrase(" ", font));

                document.Add(table);


                Phrase ph = new Phrase(" ", font);
                ph.Leading = 10;
                document.Add(ph);


                PdfPTable assetLocationTable = new PdfPTable(6);
                assetLocationTable.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
                assetLocationTable.DefaultCell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                PdfPCell assetLocationCell = new PdfPCell(new Phrase("الموقع", titlefont));
                assetLocationCell.BackgroundColor = new BaseColor(153, 204, 255);
                assetLocationCell.Border = 0;
                assetLocationCell.Colspan = 6;
                assetLocationCell.PaddingBottom = 10;
                assetLocationCell.HorizontalAlignment = 0; //0=Left, 1=Centre, 2=Right
                assetLocationTable.AddCell(assetLocationCell);

                assetLocationTable.AddCell(new Phrase("المبنى", font));
                if (assetDetailObj.Building != null)
                    assetLocationTable.AddCell(new Phrase(assetDetailObj.Building.NameAr, font));
                else
                    assetLocationTable.AddCell(new Phrase(" ", font));

                assetLocationTable.AddCell(new Phrase("الدور", font));
                if (assetDetailObj.Floor != null)
                    assetLocationTable.AddCell(new Phrase(assetDetailObj.Floor.NameAr, font));
                else
                    assetLocationTable.AddCell(new Phrase(" ", font));

                assetLocationTable.AddCell(new Phrase("الغرفة", font));
                if (assetDetailObj.Room != null)
                    assetLocationTable.AddCell(new Phrase(assetDetailObj.Room.NameAr, font));
                else
                    assetLocationTable.AddCell(new Phrase(" ", font));
                document.Add(assetLocationTable);


                Phrase ph2 = new Phrase(" ", font);
                ph2.Leading = 10;
                document.Add(ph2);



                PdfPTable assetWarrantyTable = new PdfPTable(6);
                assetWarrantyTable.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
                assetWarrantyTable.DefaultCell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                PdfPCell assetWarrantyCell = new PdfPCell(new Phrase("الضمان", titlefont));
                assetWarrantyCell.BackgroundColor = new BaseColor(153, 204, 255);
                assetWarrantyCell.Border = 0;
                assetWarrantyCell.Colspan = 6;
                assetWarrantyCell.PaddingBottom = 10;
                assetWarrantyCell.HorizontalAlignment = 0; //0=Left, 1=Centre, 2=Right
                assetWarrantyTable.AddCell(assetWarrantyCell);

                assetWarrantyTable.AddCell(new Phrase("يبدأ الضمان", font));
                if (assetDetailObj.WarrantyStart != null)
                {
                    if (assetDetailObj.WarrantyStart.Value.Date.Year != 1900)
                        assetWarrantyTable.AddCell(new Phrase(ConvertDateTimeToArabicNumerals.ConvertToArabicNumerals(DateTime.Parse(assetDetailObj.WarrantyStart.ToString()).ToShortDateString()), font));
                    else
                        assetWarrantyTable.AddCell(new Phrase(" ", font));
                }
                else
                    assetWarrantyTable.AddCell(new Phrase(" ", font));


                assetWarrantyTable.AddCell(new Phrase("ينتهي الضمان", font));
                if (assetDetailObj.WarrantyEnd != null)
                {
                    if (assetDetailObj.WarrantyEnd.Value.Date.Year != 1900)
                        assetWarrantyTable.AddCell(new Phrase(ConvertDateTimeToArabicNumerals.ConvertToArabicNumerals(DateTime.Parse(assetDetailObj.WarrantyEnd.ToString()).ToShortDateString()), font));
                    else
                        assetWarrantyTable.AddCell(new Phrase(" ", font));
                }
                else
                    assetWarrantyTable.AddCell(new Phrase(" ", font));



                assetWarrantyTable.AddCell(new Phrase("مدة الضمان", font));
                if (assetDetailObj.WarrantyExpires != "")
                    assetWarrantyTable.AddCell(new Phrase(assetDetailObj.WarrantyExpires + "  شهر", font));
                else
                    assetWarrantyTable.AddCell(new Phrase(" ", font));


                assetWarrantyTable.AddCell(new Phrase("الضمان ينتهي بعد", font));
                if (assetDetailObj.WarrantyEnd != null)
                {
                    var resultAr = Asset.Core.Helpers.DateTimeExtensions.ToDateStringAr(DateTime.Today.Date, DateTime.Parse(assetDetailObj.WarrantyEnd.Value.Date.ToString()));
                    if (assetDetailObj.WarrantyEnd.Value.Date.Year != 1900)
                    {
                        assetWarrantyTable.AddCell(new Phrase(Helpers.ArabicNumeralHelper.ConvertNumerals(resultAr.ToString()), font));
                    }
                    else
                        assetWarrantyTable.AddCell(new Phrase(" ", font));

                }
                else
                    assetWarrantyTable.AddCell(new Phrase(" ", font));
                document.Add(assetWarrantyTable);




                Phrase ph3 = new Phrase(" ", font);
                ph3.Leading = 10;
                document.Add(ph3);


                PdfPTable assetPurchaseTable = new PdfPTable(8);
                assetPurchaseTable.DefaultCell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                assetPurchaseTable.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
                PdfPCell assetPurchaseCell = new PdfPCell(new Phrase("الشراء", titlefont));
                assetPurchaseCell.BackgroundColor = new BaseColor(153, 204, 255);
                assetPurchaseCell.Border = 0;
                assetPurchaseCell.Colspan = 8;
                assetPurchaseCell.PaddingBottom = 10;
                assetPurchaseCell.HorizontalAlignment = 0;
                assetPurchaseTable.AddCell(assetPurchaseCell);


                assetPurchaseTable.AddCell(new Phrase("الشراء", font));
                if (assetDetailObj.PurchaseDate != null)
                    assetPurchaseTable.AddCell(new Phrase(ConvertDateTimeToArabicNumerals.ConvertToArabicNumerals(DateTime.Parse(assetDetailObj.PurchaseDate.ToString()).ToShortDateString()), font));
                else
                    assetPurchaseTable.AddCell(new Phrase(" ", font));



                assetPurchaseTable.AddCell(new Phrase("تاريخ توريد", font));
                if (assetDetailObj.ReceivingDate != null)
                    assetPurchaseTable.AddCell(new Phrase(ConvertDateTimeToArabicNumerals.ConvertToArabicNumerals(DateTime.Parse(assetDetailObj.ReceivingDate.ToString()).ToShortDateString()), font));
                else
                    assetPurchaseTable.AddCell(new Phrase(" ", font));



                assetPurchaseTable.AddCell(new Phrase("تاريخ التركيب", font));
                if (assetDetailObj.InstallationDate != null)
                    assetPurchaseTable.AddCell(new Phrase(ConvertDateTimeToArabicNumerals.ConvertToArabicNumerals(DateTime.Parse(assetDetailObj.InstallationDate.ToString()).ToShortDateString()), font));
                else
                    assetPurchaseTable.AddCell(new Phrase(" ", font));


                assetPurchaseTable.AddCell(new Phrase("تاريخ التشغيل", font));
                if (assetDetailObj.OperationDate != null)
                    assetPurchaseTable.AddCell(new Phrase(ConvertDateTimeToArabicNumerals.ConvertToArabicNumerals(DateTime.Parse(assetDetailObj.OperationDate.ToString()).ToShortDateString()), font));
                else
                    assetPurchaseTable.AddCell(new Phrase(" ", font));



                assetPurchaseTable.AddCell(new Phrase("رقم أمر الشراء", font));
                if (assetDetailObj.PONumber != "")
                    assetPurchaseTable.AddCell(new Phrase(assetDetailObj.PONumber, font));
                else
                    assetPurchaseTable.AddCell(new Phrase(" ", font));



                assetPurchaseTable.AddCell(new Phrase("السعر", font));
                if (assetDetailObj.Price != null)
                    assetPurchaseTable.AddCell(new Phrase(ArabicNumeralHelper.toArabicNumber(assetDetailObj.Price.ToString()), font));
                else
                    assetPurchaseTable.AddCell(new Phrase(" ", font));


                assetPurchaseTable.AddCell(new Phrase("رقم الحساب", font));
                if (assetDetailObj.CostCenter != "")
                    assetPurchaseTable.AddCell(new Phrase(assetDetailObj.CostCenter, font));
                else
                    assetPurchaseTable.AddCell(new Phrase(" ", font));

                assetPurchaseTable.AddCell(new Phrase(" ", font));
                assetPurchaseTable.AddCell(new Phrase(" ", font));

                document.Add(assetPurchaseTable);



                Phrase ph4 = new Phrase(" ", font);
                ph4.Leading = 10;
                document.Add(ph4);

                var lstRequests = _requestService.GetAllRequestsByAssetId(assetId, int.Parse(assetDetailObj.HospitalId.ToString())).ToList();
                PdfPTable assetRequestTable = new PdfPTable(7);
                assetRequestTable.SetTotalWidth(new float[] { 20f, 20f, 20f, 20f, 20f, 20f, 20f });
                assetRequestTable.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
                assetRequestTable.HorizontalAlignment = Element.ALIGN_CENTER;
                assetRequestTable.WidthPercentage = 100;
                assetRequestTable.PaddingTop = 200;
                assetRequestTable.HeaderRows = 1;
                assetRequestTable.SetWidths(new int[] { 10, 10, 10, 10, 10, 10, 10 });

                string[] col = { "الحالة", "الموضوع", "التاريخ", "رقم أمر الشغل", "الموضوع", "التاريخ", "رقم بلاغ العطل" };
                for (int i = col.Length - 1; i >= 0; i--)
                {
                    PdfPCell reqCell = new PdfPCell(new Phrase(col[i], font));
                    reqCell.BackgroundColor = new iTextSharp.text.BaseColor(153, 204, 255);
                    reqCell.PaddingBottom = 10;
                    assetRequestTable.AddCell(reqCell);
                }

                int index = 0;
                foreach (var item in lstRequests)
                {
                    ++index;

                    if (item.RequestCode != "")
                        assetRequestTable.AddCell(new Phrase(item.RequestCode, font));
                    else
                        assetRequestTable.AddCell(new Phrase(" ", font));

                    if (item.Subject != "")
                        assetRequestTable.AddCell(new Phrase(item.Subject, font));
                    else
                        assetRequestTable.AddCell(new Phrase(" ", font));


                    if (item.RequestDate != null)
                        assetRequestTable.AddCell(new Phrase(ConvertDateTimeToArabicNumerals.ConvertToArabicNumerals(DateTime.Parse(item.RequestDate.ToString()).ToShortDateString()), font));


                    else
                        assetRequestTable.AddCell(new Phrase(" ", font));




                    if (item.WorkOrderNumber != "")
                        assetRequestTable.AddCell(new Phrase(item.WorkOrderNumber, font));
                    else
                        assetRequestTable.AddCell(new Phrase(" ", font));



                    if (item.WorkOrderDate != null)
                        assetRequestTable.AddCell(new Phrase(item.WorkOrderDate.Value.ToShortDateString(), font));
                    else
                        assetRequestTable.AddCell(new Phrase(" ", font));


                    if (item.WorkOrderSubject != "")
                        assetRequestTable.AddCell(new Phrase(item.WorkOrderSubject, font));
                    else
                        assetRequestTable.AddCell(new Phrase(" ", font));




                    if (item.StatusNameAr != "")
                        assetRequestTable.AddCell(new Phrase(item.StatusNameAr, font));
                    else
                        assetRequestTable.AddCell(new Phrase(" ", font));
                }
                document.Add(assetRequestTable);
            }
            else
            {

                PdfPTable table = new PdfPTable(4);
                table.DefaultCell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                //table.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
                PdfPCell cell = new PdfPCell(new Phrase("Main Data", titlefont));
                cell.BackgroundColor = new BaseColor(153, 204, 255);
                cell.PaddingBottom = 10;
                cell.Border = 0;
                cell.Colspan = 4;
                cell.HorizontalAlignment = 0; //0=Left, 1=Centre, 2=Right
                table.AddCell(cell);
                table.AddCell(new Phrase("Governorate", font));
                table.AddCell(new Phrase(assetDetailObj.Hospital.Governorate.Name, font));
                table.AddCell(new Phrase("City", font));
                table.AddCell(new Phrase(assetDetailObj.Hospital.City.Name, font));
                table.AddCell(new Phrase("Organization", font));
                table.AddCell(new Phrase(assetDetailObj.Hospital.Organization.Name, font));
                table.AddCell(new Phrase("Sub Organization", font));
                table.AddCell(new Phrase(assetDetailObj.Hospital.SubOrganization.Name, font));
                table.AddCell(new Phrase("Category", font));
                if (assetDetailObj.MasterAsset.Category != null)
                    table.AddCell(new Phrase(assetDetailObj.MasterAsset.Category.Name, font));
                else
                    table.AddCell(new Phrase(" ", font));
                table.AddCell(new Phrase("Sub Category", font));
                if (assetDetailObj.MasterAsset.SubCategory != null)
                    table.AddCell(new Phrase(assetDetailObj.MasterAsset.SubCategory.Name, font));
                else
                    table.AddCell(new Phrase(" ", font));
                table.AddCell(new Phrase("Periority", font));
                table.AddCell(new Phrase(assetDetailObj.MasterAsset.AssetPeriority.Name, font));
                table.AddCell(new Phrase("Department", font));
                table.AddCell(new Phrase(assetDetailObj.Department.Name, font));
                table.AddCell(new Phrase("BarCode", font));
                table.AddCell(new Phrase(assetDetailObj.Barcode, font));
                table.AddCell(new Phrase("Serial", font));
                table.AddCell(new Phrase(assetDetailObj.SerialNumber, font));
                table.AddCell(new Phrase("Model", font));
                table.AddCell(new Phrase(assetDetailObj.MasterAsset.ModelNumber, font));
                table.AddCell(new Phrase("Brand", font));
                table.AddCell(new Phrase(assetDetailObj.MasterAsset.brand.Name, font));
                table.AddCell(new Phrase("Supplier", font));
                table.AddCell(new Phrase(assetDetailObj.Supplier.Name, font));
                table.AddCell(new Phrase("Origin", font));
                if (assetDetailObj.MasterAsset.Origin != null)
                    table.AddCell(new Phrase(assetDetailObj.MasterAsset.Origin.Name, font));
                else
                    table.AddCell(new Phrase(" ", font));

                document.Add(table);


                Phrase ph = new Phrase(" ", font);
                ph.Leading = 10;
                document.Add(ph);


                PdfPTable assetLocationTable = new PdfPTable(6);
                // assetLocationTable.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
                assetLocationTable.DefaultCell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                PdfPCell assetLocationCell = new PdfPCell(new Phrase("Location", titlefont));
                assetLocationCell.BackgroundColor = new BaseColor(153, 204, 255);
                assetLocationCell.Border = 0;
                assetLocationCell.Colspan = 6;
                assetLocationCell.PaddingBottom = 10;
                assetLocationCell.HorizontalAlignment = 0; //0=Left, 1=Centre, 2=Right
                assetLocationTable.AddCell(assetLocationCell);

                assetLocationTable.AddCell(new Phrase("Building", font));
                if (assetDetailObj.Building != null)
                    assetLocationTable.AddCell(new Phrase(assetDetailObj.Building.Name, font));
                else
                    assetLocationTable.AddCell(new Phrase(" ", font));

                assetLocationTable.AddCell(new Phrase("Floor", font));
                if (assetDetailObj.Floor != null)
                    assetLocationTable.AddCell(new Phrase(assetDetailObj.Floor.Name, font));
                else
                    assetLocationTable.AddCell(new Phrase(" ", font));

                assetLocationTable.AddCell(new Phrase("Room", font));
                if (assetDetailObj.Room != null)
                    assetLocationTable.AddCell(new Phrase(assetDetailObj.Room.Name, font));
                else
                    assetLocationTable.AddCell(new Phrase(" ", font));
                document.Add(assetLocationTable);


                Phrase ph2 = new Phrase(" ", font);
                ph2.Leading = 10;
                document.Add(ph2);



                PdfPTable assetWarrantyTable = new PdfPTable(6);
                // assetWarrantyTable.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
                assetWarrantyTable.DefaultCell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                PdfPCell assetWarrantyCell = new PdfPCell(new Phrase("Warranty", titlefont));
                assetWarrantyCell.BackgroundColor = new BaseColor(153, 204, 255);
                assetWarrantyCell.Border = 0;
                assetWarrantyCell.Colspan = 6;
                assetWarrantyCell.PaddingBottom = 10;
                assetWarrantyCell.HorizontalAlignment = 0; //0=Left, 1=Centre, 2=Right
                assetWarrantyTable.AddCell(assetWarrantyCell);

                assetWarrantyTable.AddCell(new Phrase("Warranty Start", font));
                if (assetDetailObj.WarrantyStart != null)
                {
                    if (assetDetailObj.WarrantyStart.Value.Date.Year != 1900)
                        assetWarrantyTable.AddCell(new Phrase(assetDetailObj.WarrantyStart.Value.ToShortDateString(), font));
                    else
                        assetWarrantyTable.AddCell(new Phrase(" ", font));
                }
                else
                    assetWarrantyTable.AddCell(new Phrase(" ", font));


                assetWarrantyTable.AddCell(new Phrase("Warranty End", font));
                if (assetDetailObj.WarrantyEnd != null)
                {
                    if (assetDetailObj.WarrantyEnd.Value.Date.Year != 1900)
                        assetWarrantyTable.AddCell(new Phrase(assetDetailObj.WarrantyEnd.Value.ToShortDateString(), font));
                    else
                        assetWarrantyTable.AddCell(new Phrase(" ", font));
                }
                else
                    assetWarrantyTable.AddCell(new Phrase(" ", font));



                assetWarrantyTable.AddCell(new Phrase("Warranty Expires", font));
                if (assetDetailObj.WarrantyExpires != "")
                    assetWarrantyTable.AddCell(new Phrase(assetDetailObj.WarrantyExpires + "  Months", font));
                else
                    assetWarrantyTable.AddCell(new Phrase(" ", font));


                assetWarrantyTable.AddCell(new Phrase("Warranty End After", font));
                if (assetDetailObj.WarrantyEnd != null)
                {
                    var result = Asset.Core.Helpers.DateTimeExtensions.ToDateString(DateTime.Parse(assetDetailObj.WarrantyEnd.Value.Date.ToString()), DateTime.Today.Date);
                    if (assetDetailObj.WarrantyEnd.Value.Date.Year != 1900)
                    {
                        assetWarrantyTable.AddCell(new Phrase(result.ToString()));
                    }
                    else
                        assetWarrantyTable.AddCell(new Phrase(" ", font));

                }
                else
                    assetWarrantyTable.AddCell(new Phrase(" ", font));
                document.Add(assetWarrantyTable);




                Phrase ph3 = new Phrase(" ", font);
                ph3.Leading = 10;
                document.Add(ph3);


                PdfPTable assetPurchaseTable = new PdfPTable(8);
                assetPurchaseTable.DefaultCell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                // assetPurchaseTable.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
                PdfPCell assetPurchaseCell = new PdfPCell(new Phrase("Purchase", titlefont));
                assetPurchaseCell.BackgroundColor = new BaseColor(153, 204, 255);
                assetPurchaseCell.Border = 0;
                assetPurchaseCell.Colspan = 8;
                assetPurchaseCell.PaddingBottom = 10;
                assetPurchaseCell.HorizontalAlignment = 0;
                assetPurchaseTable.AddCell(assetPurchaseCell);


                assetPurchaseTable.AddCell(new Phrase("Purchase", font));
                if (assetDetailObj.PurchaseDate != null)
                    assetPurchaseTable.AddCell(new Phrase(assetDetailObj.PurchaseDate.Value.ToShortDateString(), font));
                else
                    assetPurchaseTable.AddCell(new Phrase(" ", font));



                assetPurchaseTable.AddCell(new Phrase("Receiving Date", font));
                if (assetDetailObj.ReceivingDate != null)
                    assetPurchaseTable.AddCell(new Phrase(assetDetailObj.ReceivingDate.Value.ToShortDateString(), font));
                else
                    assetPurchaseTable.AddCell(new Phrase(" ", font));



                assetPurchaseTable.AddCell(new Phrase("Installation Date", font));
                if (assetDetailObj.InstallationDate != null)
                    assetPurchaseTable.AddCell(new Phrase(assetDetailObj.InstallationDate.Value.ToShortDateString(), font));
                else
                    assetPurchaseTable.AddCell(new Phrase(" ", font));


                assetPurchaseTable.AddCell(new Phrase("Operation Date", font));
                if (assetDetailObj.OperationDate != null)
                    assetPurchaseTable.AddCell(new Phrase(assetDetailObj.OperationDate.Value.ToShortDateString(), font));
                else
                    assetPurchaseTable.AddCell(new Phrase(" ", font));



                assetPurchaseTable.AddCell(new Phrase("PONumber", font));
                if (assetDetailObj.PONumber != "")
                    assetPurchaseTable.AddCell(new Phrase(assetDetailObj.PONumber, font));
                else
                    assetPurchaseTable.AddCell(new Phrase(" ", font));



                assetPurchaseTable.AddCell(new Phrase("Price", font));
                if (assetDetailObj.Price != null)
                    assetPurchaseTable.AddCell(new Phrase(assetDetailObj.Price.ToString(), font));
                else
                    assetPurchaseTable.AddCell(new Phrase(" ", font));


                assetPurchaseTable.AddCell(new Phrase("CostCenter", font));
                if (assetDetailObj.CostCenter != "")
                    assetPurchaseTable.AddCell(new Phrase(assetDetailObj.CostCenter, font));
                else
                    assetPurchaseTable.AddCell(new Phrase(" ", font));

                assetPurchaseTable.AddCell(new Phrase(" ", font));
                assetPurchaseTable.AddCell(new Phrase(" ", font));

                document.Add(assetPurchaseTable);



                Phrase ph4 = new Phrase(" ", font);
                ph4.Leading = 10;
                document.Add(ph4);

                var lstRequests = _requestService.GetAllRequestsByAssetId(assetId, int.Parse(assetDetailObj.HospitalId.ToString())).ToList();
                PdfPTable assetRequestTable = new PdfPTable(7);
                assetRequestTable.SetTotalWidth(new float[] { 20f, 20f, 20f, 20f, 20f, 20f, 20f });
                //  assetRequestTable.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
                assetRequestTable.HorizontalAlignment = Element.ALIGN_CENTER;
                assetRequestTable.WidthPercentage = 100;
                assetRequestTable.PaddingTop = 200;
                assetRequestTable.HeaderRows = 1;
                assetRequestTable.SetWidths(new int[] { 10, 10, 10, 10, 10, 10, 10 });
                string[] col = { "Req No", "Req Date", "Req Subject", "WorkOrder No.", "WO. Date", "Wo. Subject", "Status" };
                for (int i = col.Length - 1; i >= 0; i--)
                {
                    PdfPCell reqCell = new PdfPCell(new Phrase(col[i], font));
                    reqCell.BackgroundColor = new iTextSharp.text.BaseColor(153, 204, 255);
                    reqCell.PaddingBottom = 10;
                    assetRequestTable.AddCell(reqCell);
                }

                int index = 0;
                foreach (var item in lstRequests)
                {
                    ++index;

                    if (item.RequestCode != "")
                        assetRequestTable.AddCell(new Phrase(item.RequestCode, font));
                    else
                        assetRequestTable.AddCell(new Phrase(" ", font));

                    if (item.Subject != "")
                        assetRequestTable.AddCell(new Phrase(item.Subject, font));
                    else
                        assetRequestTable.AddCell(new Phrase(" ", font));


                    if (item.RequestDate != null)
                        assetRequestTable.AddCell(new Phrase(item.RequestDate.ToShortDateString(), font));
                    else
                        assetRequestTable.AddCell(new Phrase(" ", font));




                    if (item.WorkOrderNumber != "")
                        assetRequestTable.AddCell(new Phrase(item.WorkOrderNumber, font));
                    else
                        assetRequestTable.AddCell(new Phrase(" ", font));



                    if (item.WorkOrderDate != null)
                        assetRequestTable.AddCell(new Phrase(item.WorkOrderDate.Value.ToShortDateString(), font));
                    else
                        assetRequestTable.AddCell(new Phrase(" ", font));


                    if (item.WorkOrderSubject != "")
                        assetRequestTable.AddCell(new Phrase(item.WorkOrderSubject, font));
                    else
                        assetRequestTable.AddCell(new Phrase(" ", font));




                    if (item.StatusNameAr != "")
                        assetRequestTable.AddCell(new Phrase(item.StatusName, font));
                    else
                        assetRequestTable.AddCell(new Phrase(" ", font));
                }
                document.Add(assetRequestTable);
            }


            document.Close();
            byte[] bytes = memoryStream.ToArray();
            System.IO.File.WriteAllBytes(_webHostingEnvironment.ContentRootPath + "/UploadedAttachments/AssetDetails/AssetHistory.pdf", bytes);
            memoryStream.Close();
            document.Close();



        }







        #endregion

        #region Exclude Hospital Assets Functions


        [HttpGet]
        [Route("GetNoneExcludedAssetsByHospitalId/{hospitalId}")]
        public IEnumerable<ViewAssetDetailVM> GetNoneExcludedAssetsByHospitalId(int hospitalId)
        {
            return _AssetDetailService.GetNoneExcludedAssetsByHospitalId(hospitalId);
        }
        [HttpGet]
        [Route("GetSupplierNoneExcludedAssetsByHospitalId/{hospitalId}")]
        public IEnumerable<ViewAssetDetailVM> GetSupplierNoneExcludedAssetsByHospitalId(int hospitalId)
        {
            return _AssetDetailService.GetSupplierNoneExcludedAssetsByHospitalId(hospitalId);
        }


        [HttpGet]
        [Route("GetAutoCompleteSupplierExcludedAssetsByHospitalId/{barcode}/{hospitalId}")]
        public ActionResult<IEnumerable<ViewAssetDetailVM>> GetAutoCompleteSupplierExcludedAssetsByHospitalId(string barcode, int hospitalId)
        {
            return _AssetDetailService.GetAutoCompleteSupplierExcludedAssetsByHospitalId(barcode, hospitalId).ToList();
        }

        [HttpGet]
        [Route("GetAutoCompleteSupplierNoneExcludedAssetsByHospitalId/{barcode}/{hospitalId}")]
        public IEnumerable<ViewAssetDetailVM> GetAutoCompleteSupplierNoneExcludedAssetsByHospitalId(string barcode, int hospitalId)
        {
            return _AssetDetailService.GetAutoCompleteSupplierNoneExcludedAssetsByHospitalId(barcode, hospitalId);
        }



        #endregion

        #region Search Functions

        //[HttpPost]
        //[Route("SearchHospitalAssetsBySupplierId/{pageNumber}/{pageSize}")]
        //public IndexAssetDetailVM SearchHospitalAssetsBySupplierId(SearchAssetDetailVM searchObj, int pageNumber, int pageSize)
        //{
        //    return _AssetDetailService.SearchHospitalAssetsBySupplierId(searchObj, pageNumber, pageSize);
        //}



        [HttpPost]
        [Route("SearchHospitalAssetsByDepartmentId/{departmentId}/{userId}/{pageNumber}/{pageSize}")]
        public IndexAssetDetailVM SearchHospitalAssetsByDepartmentId(int departmentId, string userId, int pageNumber, int pageSize)
        {
            var lstAssets = _AssetDetailService.SearchHospitalAssetsByDepartmentId(departmentId, userId, pageNumber, pageSize);
            return lstAssets;
        }




        [HttpPost]
        [Route("GeoSortAssetsWithoutSearch/{pageNumber}/{pageSize}")]
        public IndexAssetDetailVM GeoSortAssetsWithoutSearch(Sort sortObj, int pageNumber, int pageSize)
        {
            return _AssetDetailService.GeoSortAssetsWithoutSearch(sortObj, pageNumber, pageSize);
        }
        #endregion

        #region Sort Functions

        [HttpPost]
        [Route("SortHospitalAssetsBySupplierId/{pageNumber}/{pageSize}")]
        public IndexAssetDetailVM SortHospitalAssetsBySupplierId(Sort sortObj, int pageNumber, int pageSize)
        {
            var assetDetailData = _AssetDetailService.SortHospitalAssetsBySupplierId(sortObj, pageNumber, pageSize);
            return assetDetailData;
        }


        [HttpPost]
        [Route("SortAssetDetailAfterSearch/{pageNumber}/{pageSize}")]
        public IndexAssetDetailVM SortAssetDetailAfterSearch(SortAndFilterDataModel data, int pageNumber, int pageSize)
        {
            return _AssetDetailService.SortAssetDetailAfterSearch(data, pageNumber, pageSize);
        }



        [HttpPost]
        [Route("ListHospitalAssets/{first}/{rows}")]
        public IndexAssetDetailVM ListHospitalAssets(SortAndFilterVM? data, int first, int rows)
        {
            return _AssetDetailService.ListHospitalAssets(data, first, rows);
        }

        [HttpPost]
        [Route("GetRequestsForAssetId/{assetId}/{pageNumber}/{pageSize}")]
        public IndexRequestVM GetRequestsForAssetId(int assetId, int pageNumber, int pageSize)
        {
            return _AssetDetailService.GetRequestsForAssetId(assetId, pageNumber, pageSize);
        }


        #endregion

        #region Generic Report Functions

        [HttpPost]
        [Route("GenericReportGetAssetsByUserIdAndPaging/{userId}/{pageNumber}/{pageSize}")]
        public IndexAssetDetailVM GenericReportGetAssetsByUserIdAndPaging(string userId, int pageNumber, int pageSize)
        {
            return _AssetDetailService.GenericReportGetAssetsByUserIdAndPaging(userId, pageNumber, pageSize);
        }

        /// <summary>
        /// Get Hospital Assets (Asset ) By GovernorateId,DepartmentId, And HospitalId in GeoSearch Component
        /// </summary>
        /// <param name="departmentId"></param>
        /// <param name="govId"></param>
        /// <param name="hospitalId"></param>
        /// <param name="userId"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GetHospitalAssetsByGovIdAndDeptIdAndHospitalId/{departmentId}/{govId}/{hospitalId}/{userId}/{pageNumber}/{pageSize}")]
        public ActionResult<IndexAssetDetailVM> GetHospitalAssetsByGovIdAndDeptIdAndHospitalId2(int departmentId, int govId, int hospitalId, string userId, int pageNumber, int pageSize)
        {
            return _AssetDetailService.GetHospitalAssetsByGovIdAndDeptIdAndHospitalId(departmentId, govId, hospitalId, userId, pageNumber, pageSize);
        }

        #endregion

        #region Generate QR

        /////////////////////////////////////////////////////
        /// HospitalQr - 1
        /////////////////////////////////////////
        [HttpPost]
        [Route("GenerateQrCodeForHospitalAssets")]
        public bool GenerateQrCodeForHospitalAssets(string domainName)
        {
            // domainName = "http://" + HttpContext.Request.Host.Value;
            domainName = "http://" + _httpContextAccessor.HttpContext.Request.Host.Value;
            return _AssetDetailService.GenerateQrCodeForAllAssets(domainName);
        }
        //[HttpPost]
        //[Route("GenerateWordForQrCodeForHospitalAssets")]
        //public ActionResult GenerateWordForQrCodeForHospitalAssets()
        //{

        //    using (WordDocument document = new WordDocument())
        //    {
        //        //Opens the Word template document
        //        string strTemplateFile = _webHostingEnvironment.ContentRootPath + @"\UploadedAttachments\QrTemplates\HospitalCardTemplate.dotx";

        //        Stream docStream = System.IO.File.OpenRead(strTemplateFile);
        //        document.Open(docStream, FormatType.Docx);
        //        docStream.Dispose();


        //        var allAssets = ListAssets().ToList();
        //        MailMergeDataTable dataTable = new MailMergeDataTable("Asset_QrCode", allAssets);
        //        document.MailMerge.MergeField += new MergeFieldEventHandler(MergeField2_InsertPageBreak);
        //        document.MailMerge.MergeImageField += new MergeImageFieldEventHandler(InsertQRBarcode);
        //        document.MailMerge.MergeField += new MergeFieldEventHandler(MergeField2_Event);
        //        document.MailMerge.RemoveEmptyGroup = true;

        //        document.MailMerge.ExecuteGroup(dataTable);


        //        //Saves the file in the given path
        //        string strExportFile = _webHostingEnvironment.ContentRootPath + @"\UploadedAttachments\QrTemplates\HospitalCards.docx";
        //        docStream = System.IO.File.Create(strExportFile);
        //        document.Save(docStream, FormatType.Docx);
        //        docStream.Dispose();
        //        document.Close();

        //    }
        //    return Ok();
        //}
        //private static void MergeField2_Event(object sender, MergeFieldEventArgs args)
        //{
        //    string fieldValue = args.FieldValue.ToString();
        //    //When field value is Null or empty, then remove the field owner paragraph.
        //    if (string.IsNullOrEmpty(fieldValue))
        //    {
        //        //Get the merge field owner paragraph and remove it from its owner text body.
        //        WParagraph ownerParagraph = args.CurrentMergeField.OwnerParagraph;
        //        WTextBody ownerTextBody = ownerParagraph.OwnerTextBody;
        //        ownerTextBody.ChildEntities.Remove(ownerParagraph);
        //    }
        //}
        //private void MergeField2_InsertPageBreak(object sender, MergeFieldEventArgs args)
        //{
        //    if (args.FieldName == "DepartmentName")
        //    {
        //        //Gets the owner paragraph 
        //        WParagraph paragraph = args.CurrentMergeField.OwnerParagraph;
        //        //Appends the page break 
        //        paragraph.AppendBreak(BreakType.PageBreak);
        //        i++;
        //    }

        //}



        ///////////////////////////////////////////////////////
        ///// PoliceQr - 2
        ///////////////////////////////////////////
        //[HttpPost]
        //[Route("GenerateQrCodeForAllAssets")]
        //public bool GenerateQrCodeForAllAssets(string domainName)
        //{
        //    domainName = "http://" + _httpContextAccessor.HttpContext.Request.Host.Value;
        //    return _AssetDetailService.GenerateQrCodeForAllAssets(domainName);
        //}
        //[Route("GenerateWordForQrCodeForPoliceAssets")]
        //public ActionResult GenerateWordForQrCodeForAllAssets()
        //{

        //    using (WordDocument document = new WordDocument())
        //    {
        //        //Opens the Word template document
        //        string strTemplateFile = _webHostingEnvironment.ContentRootPath + @"\UploadedAttachments\QrTemplates\PoliceCardTemplate.dotx";

        //        Stream docStream = System.IO.File.OpenRead(strTemplateFile);
        //        document.Open(docStream, FormatType.Docx);
        //        docStream.Dispose();


        //        var allAssets = ListAssets().ToList();
        //        // DataTable dtAssets = GetAssetsAsDataTable();
        //        MailMergeDataTable dataTable = new MailMergeDataTable("Asset_QrCode", allAssets);
        //        document.MailMerge.MergeField += new MergeFieldEventHandler(MergeField_InsertPageBreak);
        //        document.MailMerge.MergeImageField += new MergeImageFieldEventHandler(InsertQRBarcode);
        //        document.MailMerge.MergeField += new MergeFieldEventHandler(MergeField_Event);
        //        document.MailMerge.RemoveEmptyGroup = true;

        //        document.MailMerge.ExecuteGroup(dataTable);


        //        //Saves the file in the given path
        //        string strExportFile = _webHostingEnvironment.ContentRootPath + @"\UploadedAttachments\QrTemplates\PoliceCards.docx";
        //        docStream = System.IO.File.Create(strExportFile);
        //        document.Save(docStream, FormatType.Docx);
        //        docStream.Dispose();
        //        document.Close();

        //    }
        //    return Ok();
        //}
        //private static void MergeField_Event(object sender, MergeFieldEventArgs args)
        //{
        //    string fieldValue = args.FieldValue.ToString();
        //    //When field value is Null or empty, then remove the field owner paragraph.
        //    if (string.IsNullOrEmpty(fieldValue))
        //    {
        //        //Get the merge field owner paragraph and remove it from its owner text body.
        //        WParagraph ownerParagraph = args.CurrentMergeField.OwnerParagraph;
        //        WTextBody ownerTextBody = ownerParagraph.OwnerTextBody;
        //        ownerTextBody.ChildEntities.Remove(ownerParagraph);
        //    }
        //}
        //private void MergeField_InsertPageBreak(object sender, MergeFieldEventArgs args)
        //{
        //    if (args.FieldName == "DepartmentName")
        //    {
        //        //Gets the owner paragraph 
        //        WParagraph paragraph = args.CurrentMergeField.OwnerParagraph;
        //        //Appends the page break 
        //        paragraph.AppendBreak(BreakType.PageBreak);
        //        i++;
        //    }

        //}
        //private void InsertQRBarcode(object sender, MergeImageFieldEventArgs args)
        //{
        //    if (args.FieldName == "QrFilePath")
        //    {
        //        ////Generates barcode image for field value.
        //        System.Drawing.Image barcodeImage = GenerateQRBarcodeImage(args.FieldValue.ToString());
        //        var stream = FormatImage.ToStream(barcodeImage, ImageFormat.Png);
        //        args.ImageStream = stream;
        //    }
        //}
        //private System.Drawing.Image GenerateQRBarcodeImage(string qrBarcodeText)
        //{
        //    //Drawing QR Barcode
        //    PdfQRBarcode barcode = new PdfQRBarcode();
        //    //Set Error Correction Level
        //    barcode.ErrorCorrectionLevel = PdfErrorCorrectionLevel.Low;
        //    //Set XDimension
        //    barcode.XDimension = 4;
        //    barcode.Text = qrBarcodeText;
        //    PdfColor pdfColor = new PdfColor();
        //    //pdfColor.
        //    barcode.ForeColor = pdfColor;


        //    //Convert the barcode to image
        //    System.Drawing.Image barcodeImage = barcode.ToImage(new SizeF(88f, 88f));
        //    return barcodeImage;
        //}



        ///////////////////////////////////////////////////////
        ///// UniversityQr - 3
        ///////////////////////////////////////////
        //[HttpPost]
        //[Route("GenerateQrCodeForUniversityAssets")]
        //public bool GenerateQrCodeForUniversityAssets(string domainName)
        //{
        //    // domainName = "http://" + HttpContext.Request.Host.Value;
        //    domainName = "http://" + _httpContextAccessor.HttpContext.Request.Host.Value;

        //    return _AssetDetailService.GenerateQrCodeForAllAssets(domainName);
        //}
        //[Route("GenerateWordForQrCodeForUniversityAssets")]
        //public ActionResult GenerateWordForQrCodeForUniversityAssets()
        //{

        //    using (WordDocument document = new WordDocument())
        //    {
        //        //Opens the Word template document
        //        string strTemplateFile = _webHostingEnvironment.ContentRootPath + @"\UploadedAttachments\QrTemplates\UniversityCardTemplate.dotx";

        //        Stream docStream = System.IO.File.OpenRead(strTemplateFile);
        //        document.Open(docStream, FormatType.Docx);
        //        docStream.Dispose();


        //        var allAssets = ListAssets().ToList();
        //        MailMergeDataTable dataTable = new MailMergeDataTable("Asset_QrCode", allAssets);
        //        document.MailMerge.MergeField += new MergeFieldEventHandler(MergeField1_InsertPageBreak);
        //        document.MailMerge.MergeImageField += new MergeImageFieldEventHandler(InsertQRBarcode);
        //        document.MailMerge.MergeField += new MergeFieldEventHandler(MergeField1_Event);
        //        document.MailMerge.RemoveEmptyGroup = true;

        //        document.MailMerge.ExecuteGroup(dataTable);


        //        //Saves the file in the given path
        //        string strExportFile = _webHostingEnvironment.ContentRootPath + @"\UploadedAttachments\QrTemplates\UniversityCards.docx";
        //        docStream = System.IO.File.Create(strExportFile);
        //        document.Save(docStream, FormatType.Docx);
        //        docStream.Dispose();
        //        document.Close();

        //    }
        //    return Ok();
        //}
        //private static void MergeField1_Event(object sender, MergeFieldEventArgs args)
        //{
        //    string fieldValue = args.FieldValue.ToString();
        //    //When field value is Null or empty, then remove the field owner paragraph.
        //    if (string.IsNullOrEmpty(fieldValue))
        //    {
        //        //Get the merge field owner paragraph and remove it from its owner text body.
        //        WParagraph ownerParagraph = args.CurrentMergeField.OwnerParagraph;
        //        WTextBody ownerTextBody = ownerParagraph.OwnerTextBody;
        //        ownerTextBody.ChildEntities.Remove(ownerParagraph);
        //    }
        //}
        //private void MergeField1_InsertPageBreak(object sender, MergeFieldEventArgs args)
        //{


        //    if (args.FieldName == "DepartmentName")
        //    {
        //        //Gets the owner paragraph 
        //        WParagraph paragraph = args.CurrentMergeField.OwnerParagraph;
        //        //Appends the page break 
        //        paragraph.AppendBreak(BreakType.PageBreak);
        //        i++;
        //    }

        //}



        private List<IndexAssetDetailVM.GetData> ListAssets()
        {

            var allAssets = _AssetDetailService.GetAll().OrderBy(a => a.Barcode).ToList();
            if (allAssets.Count > 0)
            {
                return allAssets;
            }
            return new List<IndexAssetDetailVM.GetData>();
        }


        #endregion

        #region Draw Charts and DashBoard

        [HttpGet]
        [Route("ListTopAssetsByHospitalId/{hospitalId}")]
        public IEnumerable<CountAssetVM> ListTopAssetsByHospitalId(int hospitalId)
        {
            return _AssetDetailService.ListTopAssetsByHospitalId(hospitalId);
        }

        [HttpGet]
        [Route("CountAssetsInHospitalByHospitalId/{hospitalId}")]
        public IEnumerable<CountAssetVM> CountAssetsInHospitalByHospitalId(int hospitalId)
        {
            return _AssetDetailService.CountAssetsInHospitalByHospitalId(hospitalId);
        }

        /// <summary>
        /// List Assets By GovernorateId
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("ListAssetsByGovernorateIds")]
        public IEnumerable<CountAssetVM> ListAssetsByGovernorateIds()
        {
            return _AssetDetailService.ListAssetsByGovernorateIds();
        }


        /// <summary>
        /// List Assets By CityId
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("ListAssetsByCityIds")]
        public IEnumerable<CountAssetVM> ListAssetsByCityIds()
        {
            return _AssetDetailService.ListAssetsByCityIds();
        }

        /// <summary>
        /// Count Assets By hospitalId
        /// </summary>
        /// <param name="hospitalId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("CountAssetsByHospitalId/{hospitalId}")]
        public int CountAssetsByHospitalId(int hospitalId)
        {
            return _AssetDetailService.CountAssetsByHospitalId(hospitalId);
        }


        /// <summary>
        ///  Alert Assets WarrantyEnd Before 3 Monthes of Expire In DashBoard Page
        /// </summary>
        /// <param name="duration"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("AlertAssetsWarrantyEndBefore3Monthes/{hospitalId}/{duration}/{pageNumber}/{pageSize}")]
        public IndexAssetDetailVM AlertAssetsWarrantyEndBefore3Monthes(int hospitalId, int duration, int pageNumber, int pageSize)
        {
            return _AssetDetailService.AlertAssetsWarrantyEndBefore3Monthes(hospitalId, duration, pageNumber, pageSize);
        }


        #endregion

        #region PM Tasks

        [HttpGet]
        [Route("GetAllPMAssetTaskSchedules/{hospitalId}")]
        public IEnumerable<IndexPMAssetTaskScheduleVM.GetData> GetAllPMAssetTaskSchedules(int? hospitalId)
        {
            return _AssetDetailService.GetAllPMAssetTaskSchedules(hospitalId);
        }

        [HttpGet]
        [Route("GetAllPMAssetTaskScheduleByAssetId/{assetId}")]
        public IEnumerable<IndexPMAssetTaskScheduleVM.GetData> GetAllPMAssetTaskScheduleByAssetId(int? assetId)
        {
            return _AssetDetailService.GetAllPMAssetTaskScheduleByAssetId(assetId);
        }

        #endregion

        #region Contract Functions

        [HttpGet]
        [Route("GetListOfAssetDetailsByHospitalNotInContract/{hospitalId}")]
        public IEnumerable<ViewAssetDetailVM> GetListOfAssetDetailsByHospitalNotInContract(int hospitalId)
        {
            return _AssetDetailService.GetListOfAssetDetailsByHospitalNotInContract(hospitalId);
        }
        [HttpGet]
        [Route("GetListOfAssetDetailsByHospitalNotInContract2/{barcode}/{hospitalId}")]
        public IEnumerable<ViewAssetDetailVM> GetListOfAssetDetailsByHospitalNotInContract2(string barcode, int hospitalId)
        {
            return _AssetDetailService.GetListOfAssetDetailsByHospitalNotInContract(barcode, hospitalId);
        }
        [HttpGet]
        [Route("GetListOfAssetDetailsByHospitalNotInContractBySerialNumber/{serialNumber}/{hospitalId}")]
        public IEnumerable<ViewAssetDetailVM> GetListOfAssetDetailsByHospitalNotInContractBySerialNumber(string serialNumber, int hospitalId)
        {
            return _AssetDetailService.GetListOfAssetDetailsByHospitalNotInContractBySerialNumber(serialNumber, hospitalId);
        }

        #endregion

        #region Remain Functions

        ///// <summary>
        ///// List AssetDetail In Carousel Control By UserId
        ///// </summary>
        ///// <param name="userId"></param>
        ///// <returns></returns>
        //[HttpGet]
        //[Route("ListAssetDetailCarouselByUserId/{userId}")]
        //public async Task<IEnumerable<IndexAssetDetailVM.GetData>> ListAssetDetailCarouselByUserId(string userId)
        //{
        //    return await _AssetDetailService.GetAssetDetailsByUserId(userId);
        //}



        [HttpGet]
        [Route("GetAssetNameByMasterAssetIdAndHospitalId/{masterAssetId}/{hospitalId}")]
        public IEnumerable<AssetDetail> GetAssetNameByMasterAssetIdAndHospitalId(int masterAssetId, int hospitalId)
        {
            return _AssetDetailService.GetAssetNameByMasterAssetIdAndHospitalId(masterAssetId, hospitalId);
        }




        [HttpGet]
        [Route("GetAllAssetDetailsByHospitalId/{hospitalId}")]
        public IEnumerable<AssetDetail> GetAllAssetDetailsByHospitalId(int hospitalId)
        {
            return _AssetDetailService.GetAllAssetDetailsByHospitalId(hospitalId);
        }

        //[HttpPost]
        //[Route("ExportAssetsByStatusId/{statusId}/{userId}")]
        //public IEnumerable<IndexAssetDetailVM.GetData> ExportAssetsByStatusId(int statusId, string userId)
        //{
        //    var lstAssets = _AssetDetailService.GetAllAssetsByStatusId(statusId, userId).ToList();
        //    return lstAssets;
        //}
        //[HttpPost]
        //[Route("GetAllAssetsCountByStatusId/{statusId}/{userId}")]
        //public int GetCountByStatusId(int statusId, string userId)
        //{
        //    return _AssetDetailService.GetAllAssetsByStatusId(statusId, userId).ToList().Count;
        //}
        //[HttpGet]
        //[Route("GetAssetDetailsByUserId/{userId}")]
        //public async Task<IEnumerable<IndexAssetDetailVM.GetData>> GetAssetDetailsByUserId(string userId)
        //{
        //    return await _AssetDetailService.GetAssetDetailsByUserId(userId);
        //}


        [HttpGet]
        [Route("GetAssetsByUserId/{userId}")]
        public async Task<IEnumerable<IndexAssetDetailVM.GetData>> GetAssetsByUserId(string userId)
        {
            return await _AssetDetailService.GetAssetsByUserId(userId);
        }

        //[HttpPut]
        //[Route("GetAssetDetailsByUserIdWithPaging/{userId}")]
        //public IEnumerable<IndexAssetDetailVM.GetData> GetAssetDetailsByUserId(string userId, PagingParameter pageInfo)
        //{
        //    var AssetDetail = _AssetDetailService.GetAssetDetailsByUserId(userId).Result.ToList();
        //    return _pagingService.GetAll<IndexAssetDetailVM.GetData>(pageInfo, AssetDetail);
        //}
        [HttpGet]
        [Route("CountAssetsByHospital")]
        public IEnumerable<CountAssetVM> CountAssetsByHospital()
        {
            return _AssetDetailService.CountAssetsByHospital();
        }
        [HttpGet]
        [Route("Group/{assetId}")]
        public IEnumerable<PmDateGroupVM> GetEquimentswithgrouping(int assetId)
        {
            return _AssetDetailService.GetAllwithgrouping(assetId);
        }
        [Route("FilterAsset")]
        [HttpPost]
        public ActionResult<List<IndexAssetDetailVM.GetData>> FilterAsset(filterDto data)
        {
            return _AssetDetailService.FilterAsset(data);
        }
        [HttpPost]
        [Route("GetAssetByDepartment")]
        public ActionResult<List<DepartmentGroupVM>> GetAssetByDepartment(List<IndexAssetDetailVM.GetData> AssetModel)
        {
            return _AssetDetailService.GetAssetByDepartment(AssetModel);
        }







        [HttpPost]
        [Route("GetAssetByBrands")]
        public ActionResult<List<BrandGroupVM>> GetAssetByBrands(List<IndexAssetDetailVM.GetData> AssetModel)
        {
            return _AssetDetailService.GetAssetByBrands(AssetModel);
        }
        [HttpPost]
        [Route("GetAssetByHospital")]
        public ActionResult<List<GroupHospitalVM>> GetAssetByHospital(List<IndexAssetDetailVM.GetData> AssetModel)
        {
            return _AssetDetailService.GetAssetByHospital(AssetModel);
        }
        [HttpPost]
        [Route("GetAssetByGovernorate")]
        public ActionResult<List<GroupGovernorateVM>> GetAssetByGovernorate(List<IndexAssetDetailVM.GetData> AssetModel)
        {
            return _AssetDetailService.GetAssetByGovernorate(AssetModel);
        }
        [HttpPost]
        [Route("GetAssetByCity")]
        public ActionResult<List<GroupCityVM>> GetAssetByCity(List<IndexAssetDetailVM.GetData> AssetModel)
        {
            return _AssetDetailService.GetAssetByCity(AssetModel);
        }








        [HttpPost]
        [Route("GetAssetBySupplier")]
        public ActionResult<List<GroupSupplierVM>> GetAssetBySupplier(List<IndexAssetDetailVM.GetData> AssetModel)
        {
            return _AssetDetailService.GetAssetBySupplier(AssetModel);
        }
        [HttpPost]
        [Route("GetAssetByOrganization")]
        public ActionResult<List<GroupOrganizationVM>> GetAssetByOrganization(List<IndexAssetDetailVM.GetData> AssetModel)
        {
            return _AssetDetailService.GetAssetByOrganization(AssetModel);
        }
        [HttpGet]
        [Route("GetAssetsByAgeGroup/{hospitalId}")]
        public List<HospitalAssetAge> GetAssetsByAgeGroup(int hospitalId)
        {
            var list = _AssetDetailService.GetAssetsByAgeGroup(hospitalId);
            return list;
        }
        [HttpPost]
        [Route("GetGeneralAssetsByAgeGroup")]
        public List<HospitalAssetAge> GetGeneralAssetsByAgeGroup(FilterHospitalAssetAge model)
        {
            var list = _AssetDetailService.GetGeneralAssetsByAgeGroup(model);
            return list;
        }
        [Route("GetLastDocumentForAssetDetailId/{assetDetailId}")]
        public AssetDetailAttachment GetLastDocumentForWorkOrderTrackingId(int assetDetailId)
        {
            return _AssetDetailService.GetLastDocumentForAssetDetailId(assetDetailId);
        }
        [HttpPost]
        [Route("CreateAssetDepartmentBrandSupplierPDF")]
        public void CreateAssetDepartmentBrandSupplierPDF(FilterHospitalAsset filterHospitalAssetObj)
        {

            var lstSettings = _settingService.GetAll().ToList();
            if (lstSettings.Count > 0)
            {
                foreach (var item in lstSettings)
                {
                    if (item.KeyName == "Institute")
                    {
                        strInsitute = item.KeyValue;
                        strInsituteAr = item.KeyValueAr;
                    }

                    if (item.KeyName == "Logo")
                        strLogo = item.KeyValue;

                    if (item.KeyName == "PMAgency")
                        isAgency = Convert.ToBoolean(item.KeyValue);
                }
            }

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            iTextSharp.text.Document document = new iTextSharp.text.Document();
            System.IO.MemoryStream memoryStream = new System.IO.MemoryStream();
            PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);
            document.NewPage();
            document.Open();
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            string adobearabic = _webHostingEnvironment.ContentRootPath + "/Font/adobearabic.ttf";
            BaseFont bfUniCode = BaseFont.CreateFont(adobearabic, BaseFont.IDENTITY_H, true);
            iTextSharp.text.Font font = new iTextSharp.text.Font(bfUniCode, 14);

            Phrase ph = new Phrase(" ", font);
            document.Add(ph);

            PdfPTable bodytable = AssetDepartmentBrandSupplier(filterHospitalAssetObj);
            int countnewpages = bodytable.Rows.Count / 25;
            for (int i = 1; i <= countnewpages; i++)
            {
                document.NewPage();
                writer.PageEmpty = false;
            }

            document.Close();
            byte[] bytes = memoryStream.ToArray();
            System.IO.File.WriteAllBytes(_webHostingEnvironment.ContentRootPath + "/UploadedAttachments/AssetDetails/FilterAssetDetails/FilterAssetDetails.pdf", bytes);


            memoryStream = new MemoryStream();
            PdfReader reader = new PdfReader(bytes);
            using (PdfStamper stamper = new PdfStamper(reader, memoryStream))
            {
                int pages = reader.NumberOfPages;
                //Footer
                for (int i = 1; i <= pages; i++)
                {
                    ColumnText.ShowTextAligned(stamper.GetUnderContent(i), Element.ALIGN_LEFT, new Phrase(ArabicNumeralHelper.toArabicNumber(pages.ToString()) + "/" + ArabicNumeralHelper.toArabicNumber(i.ToString()), font), 800f, 15f, 0);
                    ColumnText.ShowTextAligned(stamper.GetUnderContent(i), Element.ALIGN_RIGHT, new Phrase("تمت الطباعة بواسطة  " + filterHospitalAssetObj.PrintedBy, font), 150f, 15f, 0, PdfWriter.RUN_DIRECTION_RTL, ColumnText.AR_LIG);
                }
                //Header
                for (int i = 1; i <= pages; i++)
                {
                    string imageURL = _webHostingEnvironment.ContentRootPath + "/Images/" + strLogo;
                    iTextSharp.text.Image jpg = iTextSharp.text.Image.GetInstance(imageURL);
                    jpg.ScaleAbsolute(70f, 50f);
                    PdfPTable headertable = new PdfPTable(2);
                    headertable.SetTotalWidth(new float[] { 250f, 50f });
                    headertable.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
                    headertable.WidthPercentage = 100;
                    PdfPCell cell = new PdfPCell(new PdfPCell(jpg));
                    //cell.Rowspan = 2;
                    cell.PaddingTop = 5;
                    cell.Border = Rectangle.NO_BORDER;
                    cell.PaddingRight = 10;
                    //cell.HorizontalAlignment = 2; //0=Left, 1=Centre, 2=Right
                    headertable.AddCell(cell);

                    if (filterHospitalAssetObj.Lang == "ar")
                    {
                        headertable.AddCell(new PdfPCell(new Phrase("\t\t\t\t " + strInsituteAr + "\n" + filterHospitalAssetObj.HospitalNameAr + "", font)) { Border = Rectangle.NO_BORDER, PaddingTop = 10 });
                    }
                    else
                        headertable.AddCell(new PdfPCell(new Phrase(" " + strInsitute + "\n" + filterHospitalAssetObj.HospitalName + "", font)) { Border = Rectangle.NO_BORDER, PaddingTop = 10 });
                    headertable.WriteSelectedRows(0, -1, 270, 830, stamper.GetOverContent(i));

                    System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                    string adobearabicheaderTitle = _webHostingEnvironment.ContentRootPath + "/Font/adobearabic.ttf";
                    BaseFont bfUniCodeheaderTitle = BaseFont.CreateFont(adobearabicheaderTitle, BaseFont.IDENTITY_H, true);
                    iTextSharp.text.Font titlefont = new iTextSharp.text.Font(bfUniCodeheaderTitle, 13);
                    titlefont.SetStyle("bold");


                    PdfPTable titleTable = new PdfPTable(1);
                    titleTable.SetTotalWidth(new float[] { 600f });
                    titleTable.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
                    titleTable.WidthPercentage = 100;
                    titleTable.AddCell(new PdfPCell(new Phrase("تقرير الأجهزة بالأقسام والموردين والماركات", titlefont)) { PaddingBottom = 5, Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_CENTER });

                    if (filterHospitalAssetObj.Start == "")
                        filterHospitalAssetObj.Start = "01/01/1900";

                    var sDate = DateTime.Parse(filterHospitalAssetObj.Start);
                    var sday = ArabicNumeralHelper.toArabicNumber(sDate.Day.ToString());
                    var smonth = ArabicNumeralHelper.toArabicNumber(sDate.Month.ToString());
                    var syear = ArabicNumeralHelper.toArabicNumber(sDate.Year.ToString());
                    var strStart = sday + "/" + smonth + "/" + syear;

                    if (filterHospitalAssetObj.End == "")
                        filterHospitalAssetObj.End = DateTime.Today.Date.ToShortDateString();

                    var eDate = DateTime.Parse(filterHospitalAssetObj.End);
                    var eday = ArabicNumeralHelper.toArabicNumber(eDate.Day.ToString());
                    var emonth = ArabicNumeralHelper.toArabicNumber(eDate.Month.ToString());
                    var eyear = ArabicNumeralHelper.toArabicNumber(eDate.Year.ToString());
                    var strEnd = eday + "/" + emonth + "/" + eyear;

                    titleTable.AddCell(new PdfPCell(new Phrase("خلال الفترة من" + strStart + " إلى " + strEnd, titlefont)) { PaddingBottom = 5, Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_CENTER });
                    titleTable.WriteSelectedRows(0, -1, 0, 760, stamper.GetOverContent(i));
                }
                for (int i = 1; i <= pages; i++)
                {
                    PdfPTable bodytable2 = new PdfPTable(8);
                    bodytable2.SetTotalWidth(new float[] { 70f, 70f, 70f, 70f, 70f, 70f, 70f, 70f });
                    bodytable2.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
                    bodytable2.HorizontalAlignment = Element.ALIGN_RIGHT;
                    bodytable2.WidthPercentage = 100;
                    bodytable2.PaddingTop = 200;
                    bodytable2.HeaderRows = 1;

                    bodytable2.SetWidths(new int[] { 25, 25, 25, 25, 25, 25, 25, 7 });
                    int countRows = bodytable.Rows.Count;
                    if (countRows > 25)
                    {
                        countRows = 25;
                    }
                    bodytable2.Rows.Add(bodytable.Rows[0]);
                    for (int j = 1; j <= countRows - 1; j++)
                    {
                        bodytable2.Rows.Add(bodytable.Rows[j]);
                    }
                    for (int k = 1; k <= bodytable2.Rows.Count; k++)
                    {
                        bodytable.DeleteRow(1);
                    }
                    bodytable2.WriteSelectedRows(0, -1, 10, 700, stamper.GetUnderContent(i));
                }
            }
            bytes = memoryStream.ToArray();
            System.IO.File.WriteAllBytes(_webHostingEnvironment.ContentRootPath + "/UploadedAttachments/AssetDetails/FilterAssetDetails/FilterAssetDetails.pdf", bytes);
            memoryStream.Close();
            document.Close();

        }
        public PdfPTable AssetDepartmentBrandSupplier(FilterHospitalAsset filterHospitalAssetObj)
        {

            var lstData = _AssetDetailService.FilterDataByDepartmentBrandSupplierId(filterHospitalAssetObj).ToList();
            PdfPTable table = new PdfPTable(8);
            table.SetTotalWidth(new float[] { 70f, 70f, 70f, 70f, 70f, 70f, 70f, 70f });
            table.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
            table.HorizontalAlignment = Element.ALIGN_RIGHT;
            table.WidthPercentage = 100;
            table.PaddingTop = 200;
            table.HeaderRows = 1;
            table.SetWidths(new int[] { 25, 25, 25, 25, 25, 25, 25, 7 });
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            string ARIALUNI_TFF = _webHostingEnvironment.ContentRootPath + "/Font/adobearabic.ttf";
            BaseFont bfArialUniCode = BaseFont.CreateFont(ARIALUNI_TFF, BaseFont.IDENTITY_H, true);
            iTextSharp.text.Font font = new iTextSharp.text.Font(bfArialUniCode, 10);


            if (filterHospitalAssetObj.selectedElement == "supplier" || filterHospitalAssetObj.selectedElement == "المورد")
            {
                var lstAssetsByBrand = _AssetDetailService.GetAssetBySupplier(lstData).ToList();
                foreach (var item in lstAssetsByBrand)
                {
                    // table.AddCell(new PdfPCell(new Phrase(item.NameAr, font)) { PaddingBottom = 5, Colspan = 8 });

                    PdfPCell c1 = new PdfPCell(new Phrase(item.NameAr, font));
                    c1.Colspan = 8;
                    table.AddCell(c1);



                    string[] col = { "المورد", "الماركة", "القسم", "الموديل", "السيريال", "الباركود", "الاسم", "م" };
                    string[] encol = { "No.", "Name", "Barcode", "Serial", "Model", "Department", "Brand", "Supplier" };
                    if (filterHospitalAssetObj.Lang == "ar")
                    {
                        for (int i = col.Length - 1; i >= 0; i--)
                        {
                            PdfPCell cell = new PdfPCell(new Phrase(col[i], font));
                            cell.BackgroundColor = new iTextSharp.text.BaseColor(153, 204, 255);
                            cell.PaddingBottom = 10;
                            table.AddCell(cell);
                        }
                    }
                    else
                    {
                        for (int i = 0; i <= encol.Length - 1; i++)
                        {
                            PdfPCell cell = new PdfPCell(new Phrase(encol[i]));
                            cell.BackgroundColor = new iTextSharp.text.BaseColor(153, 204, 255);
                            cell.PaddingBottom = 10;
                            table.AddCell(cell);
                        }
                    }
                    if (item.AssetList.Count > 0)
                    {
                        int index = 0;
                        foreach (var groupItems in item.AssetList)
                        {
                            ++index;
                            table.AddCell(new PdfPCell(new Phrase(ArabicNumeralHelper.toArabicNumber(index.ToString()), font)) { PaddingBottom = 5 });
                            table.AddCell(new PdfPCell(new Phrase(groupItems.AssetNameAr, font)) { PaddingBottom = 5 });
                            table.AddCell(new PdfPCell(new Phrase(groupItems.Barcode, font)) { PaddingBottom = 5 });
                            table.AddCell(new PdfPCell(new Phrase(groupItems.SerialNumber, font)) { PaddingBottom = 5 });
                            table.AddCell(new PdfPCell(new Phrase(groupItems.Model, font)) { PaddingBottom = 5 });
                            table.AddCell(new PdfPCell(new Phrase(groupItems.DepartmentNameAr, font)) { PaddingBottom = 5 });
                            table.AddCell(new PdfPCell(new Phrase(groupItems.BrandNameAr, font)) { PaddingBottom = 5 });
                            table.AddCell(new PdfPCell(new Phrase(groupItems.SupplierNameAr, font)) { PaddingBottom = 5 });
                            if (groupItems.PurchaseDate != null)
                                table.AddCell(new PdfPCell(new Phrase(ConvertDateTimeToArabicNumerals.ConvertToArabicNumerals(DateTime.Parse(groupItems.PurchaseDate.ToString()).ToString("g", new CultureInfo("ar-AE"))), font)) { PaddingBottom = 5 });
                            else
                                table.AddCell(new PdfPCell(new Phrase(" ")) { PaddingBottom = 5 });
                        }
                    }
                }
            }
            //if (filterHospitalAssetObj.selectedElement == "brand" || filterHospitalAssetObj.selectedElement == "الصانع")
            //{
            //    var lstAssetsByBrand = _AssetDetailService.GetAssetByBrands(lstData).ToList();
            //    foreach (var item in lstAssetsByBrand)
            //    {
            //        // table.AddCell(new PdfPCell(new Phrase(item.NameAr, font)) { PaddingBottom = 5, Colspan = 8 });

            //        PdfPCell c1 = new PdfPCell(new Phrase(item.NameAr, font));
            //        c1.Colspan = 8;
            //        table.AddCell(c1);

            //        foreach (var groupItems in item.AssetList)
            //        {
            //           // ++index;
            //           // table.AddCell(new PdfPCell(new Phrase(ArabicNumeralHelper.toArabicNumber(index.ToString()), font)) { PaddingBottom = 5 });
            //            table.AddCell(new PdfPCell(new Phrase(groupItems.AssetNameAr, font)) { PaddingBottom = 5 });
            //            table.AddCell(new PdfPCell(new Phrase(groupItems.Barcode, font)) { PaddingBottom = 5 });
            //            table.AddCell(new PdfPCell(new Phrase(groupItems.SerialNumber, font)) { PaddingBottom = 5 });
            //            table.AddCell(new PdfPCell(new Phrase(groupItems.Model, font)) { PaddingBottom = 5 });
            //            table.AddCell(new PdfPCell(new Phrase(groupItems.DepartmentNameAr, font)) { PaddingBottom = 5 });
            //            table.AddCell(new PdfPCell(new Phrase(groupItems.BrandNameAr, font)) { PaddingBottom = 5 });
            //            table.AddCell(new PdfPCell(new Phrase(groupItems.SupplierNameAr, font)) { PaddingBottom = 5 });
            //            if (groupItems.PurchaseDate != null)
            //                table.AddCell(new PdfPCell(new Phrase(ConvertDateTimeToArabicNumerals.ConvertToArabicNumerals(DateTime.Parse(groupItems.PurchaseDate.ToString()).ToString("g", new CultureInfo("ar-AE"))), font)) { PaddingBottom = 5 });
            //            else
            //                table.AddCell(new PdfPCell(new Phrase(" ")) { PaddingBottom = 5 });
            //        }
            //    }
            //}
            //if (filterHospitalAssetObj.selectedElement == "Department" || filterHospitalAssetObj.selectedElement == "القسم")
            //{
            //    var lstAssetsByBrand = _AssetDetailService.GetAssetByDepartment(lstData).ToList();
            //    foreach (var item in lstAssetsByBrand)
            //    {
            //        // table.AddCell(new PdfPCell(new Phrase(item.NameAr, font)) { PaddingBottom = 5, Colspan = 8 });

            //        PdfPCell c1 = new PdfPCell(new Phrase(item.NameAr, font));
            //        c1.Colspan = 8;
            //        table.AddCell(c1);

            //        foreach (var groupItems in item.AssetList)
            //        {
            //            ++index;
            //            table.AddCell(new PdfPCell(new Phrase(ArabicNumeralHelper.toArabicNumber(index.ToString()), font)) { PaddingBottom = 5 });
            //            table.AddCell(new PdfPCell(new Phrase(groupItems.AssetNameAr, font)) { PaddingBottom = 5 });
            //            table.AddCell(new PdfPCell(new Phrase(groupItems.Barcode, font)) { PaddingBottom = 5 });
            //            table.AddCell(new PdfPCell(new Phrase(groupItems.SerialNumber, font)) { PaddingBottom = 5 });
            //            table.AddCell(new PdfPCell(new Phrase(groupItems.Model, font)) { PaddingBottom = 5 });
            //            table.AddCell(new PdfPCell(new Phrase(groupItems.DepartmentNameAr, font)) { PaddingBottom = 5 });
            //            table.AddCell(new PdfPCell(new Phrase(groupItems.BrandNameAr, font)) { PaddingBottom = 5 });
            //            table.AddCell(new PdfPCell(new Phrase(groupItems.SupplierNameAr, font)) { PaddingBottom = 5 });
            //            if (groupItems.PurchaseDate != null)
            //                table.AddCell(new PdfPCell(new Phrase(ConvertDateTimeToArabicNumerals.ConvertToArabicNumerals(DateTime.Parse(groupItems.PurchaseDate.ToString()).ToString("g", new CultureInfo("ar-AE"))), font)) { PaddingBottom = 5 });
            //            else
            //                table.AddCell(new PdfPCell(new Phrase(" ")) { PaddingBottom = 5 });
            //        }
            //    }
            //}
            //else
            //{

            //    foreach (var item in lstData)
            //    {
            //        //  table.AddCell(new PdfPCell(new Phrase("R3C1-4")) { Colspan = 8 });
            //        ++index;
            //        if (filterHospitalAssetObj.Lang == "ar")
            //        {
            //            table.AddCell(new PdfPCell(new Phrase(ArabicNumeralHelper.toArabicNumber(index.ToString()), font)) { PaddingBottom = 5 });
            //            table.AddCell(new PdfPCell(new Phrase(item.AssetNameAr, font)) { PaddingBottom = 5 });
            //            table.AddCell(new PdfPCell(new Phrase(item.Barcode, font)) { PaddingBottom = 5 });
            //            table.AddCell(new PdfPCell(new Phrase(item.SerialNumber, font)) { PaddingBottom = 5 });
            //            table.AddCell(new PdfPCell(new Phrase(item.Model, font)) { PaddingBottom = 5 });
            //            table.AddCell(new PdfPCell(new Phrase(item.DepartmentNameAr, font)) { PaddingBottom = 5 });
            //            table.AddCell(new PdfPCell(new Phrase(item.BrandNameAr, font)) { PaddingBottom = 5 });
            //            table.AddCell(new PdfPCell(new Phrase(item.SupplierNameAr, font)) { PaddingBottom = 5 });
            //            if (item.PurchaseDate != null)
            //                table.AddCell(new PdfPCell(new Phrase(ConvertDateTimeToArabicNumerals.ConvertToArabicNumerals(DateTime.Parse(item.PurchaseDate.ToString()).ToString("g", new CultureInfo("ar-AE"))), font)) { PaddingBottom = 5 });
            //            else
            //                table.AddCell(new PdfPCell(new Phrase(" ")) { PaddingBottom = 5 });
            //        }
            //        else
            //        {
            //            table.AddCell(new PdfPCell(new Phrase(index.ToString(), font)) { PaddingBottom = 5 });
            //            table.AddCell(new PdfPCell(new Phrase(item.AssetName, font)) { PaddingBottom = 5 });
            //            table.AddCell(new PdfPCell(new Phrase(item.Barcode, font)) { PaddingBottom = 5 });
            //            table.AddCell(new PdfPCell(new Phrase(item.SerialNumber, font)) { PaddingBottom = 5 });
            //            table.AddCell(new PdfPCell(new Phrase(item.Model, font)) { PaddingBottom = 5 });
            //            table.AddCell(new PdfPCell(new Phrase(item.DepartmentName, font)) { PaddingBottom = 5 });
            //            table.AddCell(new PdfPCell(new Phrase(item.BrandName, font)) { PaddingBottom = 5 });
            //            table.AddCell(new PdfPCell(new Phrase(item.SupplierName, font)) { PaddingBottom = 5 });
            //            if (item.PurchaseDate != null)
            //                table.AddCell(new PdfPCell(new Phrase(DateTime.Parse(item.PurchaseDate.ToString()).ToString("g", new CultureInfo("en-US")), font)) { PaddingBottom = 5 });
            //            else
            //                table.AddCell(new PdfPCell(new Phrase(" ")) { PaddingBottom = 5 });
            //        }
            //    }
            //}

            return table;
        }
        [HttpGet]
        [Route("DownloadAssetDepartmentBrandSupplierPDF")]
        public HttpResponseMessage DownloadFile()
        {
            var file = _webHostingEnvironment.ContentRootPath + "/UploadedAttachments/AssetDetails/FilterAssetDetails/FilterAssetDetails.pdf";
            HttpResponseMessage response = null;
            if (!System.IO.File.Exists(file))
                System.IO.Directory.CreateDirectory(file);
            //return new HttpResponseMessage(HttpStatusCode.Gone);
            else
            {
                //if file present than read file 
                var fStream = new FileStream(file, FileMode.Open, FileAccess.Read);
                //compose response and include file as content in it
                response = new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StreamContent(fStream)
                };
                response.Content.Headers.ContentDisposition =
                                            new ContentDispositionHeaderValue("attachment")
                                            {
                                                FileName = Path.GetFileName(fStream.Name)
                                            };
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            }
            return response;
        }
        [HttpPost]
        [Route("GetHospitalAssetsBySupplierId/{supplierId}/{pageNumber}/{pageSize}")]
        public IndexAssetDetailVM GetHospitalAssetsBySupplierId(int supplierId, int pageNumber, int pageSize)
        {
            return _AssetDetailService.GetHospitalAssetsBySupplierId(supplierId, pageNumber, pageSize);
        }
        [HttpGet]
        [Route("GetAssetsByBrandId/{brandId}")]
        public IndexAssetDetailVM GetAssetsByBrandId(int brandId)
        {
            IndexAssetDetailVM result = new IndexAssetDetailVM();
            result = _AssetDetailService.GetAssetsByBrandId(brandId);
            return result;
        }
        [HttpGet]
        [Route("GetAssetsByDepartmentId/{departmentId}")]
        public IndexAssetDetailVM GetAssetsByDepartmentId(int departmentId)
        {
            IndexAssetDetailVM result = new IndexAssetDetailVM();
            result = _AssetDetailService.GetAssetsByDepartmentId(departmentId);
            return result;

        }
        [HttpGet]
        [Route("GetAssetsBySupplierId/{supplierId}")]
        public List<IndexAssetDetailVM.GetData> GetAssetsBySupplierId(int supplierId)
        {
            List<IndexAssetDetailVM.GetData> result = new List<IndexAssetDetailVM.GetData>();
            result = _AssetDetailService.GetAssetsBySupplierId(supplierId);
            return result;
        }
        [HttpGet]
        [Route("GetAssetsBySupplierIdWithPaging/{supplierId}/{pageNumber}/{pageSize}")]
        public IndexAssetDetailVM GetAssetsBySupplierIdWithPaging(int supplierId, int pageNumber, int pageSize)
        {
            IndexAssetDetailVM result = _AssetDetailService.GetAssetsBySupplierIdWithPaging(supplierId, pageNumber, pageSize);
            return result;
        }


        [HttpGet]
        [Route("GetAssetsByGovernorateIdWithPaging/{governorateId}/{pageNumber}/{pageSize}")]
        public IndexAssetDetailVM GetAssetsByGovernorateIdWithPaging(int governorateId, int pageNumber, int pageSize)
        {
            return _AssetDetailService.GetAssetsByGovernorateIdWithPaging(governorateId, pageNumber, pageSize);
        }
        [HttpGet]
        [Route("GetAssetsByGovernorateIdWithPaging/{governorateId}")]
        public IndexAssetDetailVM GetAssetsByGovernorateIdWithPaging(int governorateId)
        {
            return _AssetDetailService.GetAssetsByGovernorateIdWithPaging(governorateId);
        }



        [HttpGet]
        [Route("GetAssetsByCityIdWithPaging/{cityId}/{pageNumber}/{pageSize}")]
        public IndexAssetDetailVM GetAssetsByCityIdWithPaging(int cityId, int pageNumber, int pageSize)
        {
            return _AssetDetailService.GetAssetsByCityIdWithPaging(cityId, pageNumber, pageSize);
        }
        [HttpGet]
        [Route("GetAssetsByCityIdWithPaging/{cityId}")]
        public IndexAssetDetailVM GetAssetsByCityIdWithPaging(int cityId)
        {
            return _AssetDetailService.GetAssetsByCityIdWithPaging(cityId);
        }




        [HttpGet]
        [Route("GetAssetsByHospitalIdWithPaging/{cityId}")]
        public IndexAssetDetailVM GetAssetsByHospitalIdWithPaging(int cityId)
        {
            return _AssetDetailService.GetAssetsByHospitalIdWithPaging(cityId);
        }





        [HttpPost]
        [Route("GroupAssetDetailsByBrand")]
        public List<BrandGroupVM> GroupAssetDetailsByBrand(FilterHospitalAsset data)
        {
            return _AssetDetailService.GroupAssetDetailsByBrand(data);
        }
        [HttpPost]
        [Route("GroupAssetDetailsBySupplier")]
        public List<SupplierGroupVM> GroupAssetDetailsBySupplier(FilterHospitalAsset data)
        {
            return _AssetDetailService.GroupAssetDetailsBySupplier(data);
        }
        [HttpPost]
        [Route("GroupAssetDetailsByDepartment")]
        public List<DepartmentGroupVM> GroupAssetDetailsByDepartment(FilterHospitalAsset data)
        {
            return _AssetDetailService.GroupAssetDetailsByDepartment(data);
        }





        [HttpPost]
        [Route("GroupAssetDetailsByGovernorate")]
        public List<GovernorateGroupVM> GroupAssetDetailsByGovernorate(FilterHospitalAsset data)
        {
            return _AssetDetailService.GroupAssetDetailsByGovernorate(data);
        }



        [HttpPost]
        [Route("GroupAssetDetailsByHospital")]
        public List<GroupHospitalVM> GroupAssetDetailsByHospital(FilterHospitalAsset data)
        {
            return _AssetDetailService.GroupAssetDetailsByHospital(data);
        }









        [HttpPost]
        [Route("FilterDataByDepartmentBrandSupplierIdAndPaging/{userId}/{pageNumber}/{pageSize}")]
        public IndexAssetDetailVM FilterDataByDepartmentBrandSupplierIdAndPaging(FilterHospitalAsset data, string userId, int pageNumber, int pageSize)
        {
            var list = _AssetDetailService.FilterDataByDepartmentBrandSupplierIdAndPaging(data, userId, pageNumber, pageSize);
            return list;
        }

        #endregion









        /////////////////////////////////////////////////////
        /// WO PM List
        /// //PrintListOfPMWorkOrders
        /////////////////////////////////////////
        ///


        [HttpPost]
        [Route("GenerateWOPMDocument")]
        public void GenerateWOPMDocument(List<IndexAssetDetailVM.GetData> assetsPM)
        {
         
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            string adobearabic = _webHostingEnvironment.ContentRootPath + "/Font/adobearabic.ttf";
            BaseFont bfUniCode = BaseFont.CreateFont(adobearabic, BaseFont.IDENTITY_H, true);
            iTextSharp.text.Font font = new iTextSharp.text.Font(bfUniCode, 13);
            iTextSharp.text.Font englishfont = new iTextSharp.text.Font(bfUniCode, 12);
            iTextSharp.text.Font reportfont = new iTextSharp.text.Font(bfUniCode, 8);

            List<IndexAssetDetailVM.GetData> lstPrintWO = _AssetDetailService.PrintListOfPMWorkOrders(assetsPM);


            //int countPages = lstPrintWO.Count;

            Document document = new Document();

            try
            {
                PdfWriter.GetInstance(document, new FileStream(_webHostingEnvironment.ContentRootPath + "/UploadedAttachments/WOReports/WOPM2.pdf", FileMode.Create));
                document.Open();

                for (int i = 0; i < lstPrintWO.Count; i++)
                {
                    PdfPTable headertable = new PdfPTable(3);
                    headertable.DefaultCell.Border = 0;
                    headertable.SetTotalWidth(new float[] { 100f, 300f, 150f });
                    headertable.WidthPercentage = 100;

                    headertable.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
                    headertable.AddCell(new Phrase("ﻗطﺎع ﻣﺷروﻋﺎت اﻟﺻﺣﺔ", font));
                    headertable.AddCell(new PdfPCell(new Phrase("ﺗﻘرﯾر ﻋﻣل / Sheet Job", font)) { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE });
                    headertable.AddCell(new PdfPCell(new Phrase("Report No. 1", reportfont)) { Border = Rectangle.NO_BORDER, RunDirection = PdfWriter.RUN_DIRECTION_LTR });
                    document.Add(headertable);

                    Paragraph emptySpace = new Paragraph();
                    emptySpace.Add(new Chunk("\n")); // Adjust the font size as needed
                    document.Add(emptySpace);

                    PdfPTable outerCustomerTable = new PdfPTable(1);
                    outerCustomerTable.SetTotalWidth(new float[] { 570f });
                    outerCustomerTable.WidthPercentage = 100;
                    outerCustomerTable.DefaultCell.Border = 0;
                    outerCustomerTable.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
                    PdfPTable customerTable = new PdfPTable(3);
                    customerTable.SetTotalWidth(new float[] { 100f, 350f, 100f });
                    customerTable.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
                    customerTable.WidthPercentage = 100;
                    customerTable.AddCell(new PdfPCell(new Phrase("العميل", font)) { Border = Rectangle.NO_BORDER });
                    customerTable.AddCell(new PdfPCell(new Phrase(lstPrintWO[i].HospitalNameAr, font)) { Border = Rectangle.NO_BORDER });
                    customerTable.AddCell(new PdfPCell(new Phrase("Customer", englishfont)) { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_LEFT, RunDirection = PdfWriter.RUN_DIRECTION_LTR });
                    customerTable.AddCell(new PdfPCell(new Phrase("العنوان", font)) { Border = Rectangle.NO_BORDER });
                    customerTable.AddCell(new PdfPCell(new Phrase("", font)) { Border = Rectangle.NO_BORDER });
                    customerTable.AddCell(new PdfPCell(new Phrase("Address", englishfont)) { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_LEFT, RunDirection = PdfWriter.RUN_DIRECTION_LTR });
                    customerTable.AddCell(new PdfPCell(new Phrase("التاريخ", font)) { Border = Rectangle.NO_BORDER });
                    //if (lstPrintWO[i].CreationDate != null)
                    //    customerTable.AddCell(new PdfPCell(new Phrase(lstPrintWO[i].CreationDate.Value.ToShortDateString(), font)) { Border = Rectangle.NO_BORDER });
                    //else
                    customerTable.AddCell(new PdfPCell(new Phrase(DateTime.Today.Date.ToShortDateString(), font)) { Border = Rectangle.NO_BORDER });

                    customerTable.AddCell(new PdfPCell(new Phrase("Date", englishfont)) { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_LEFT, RunDirection = PdfWriter.RUN_DIRECTION_LTR });
                    PdfPCell customerCell = new PdfPCell(customerTable);
                    customerCell.Border = PdfPCell.BOX;
                    customerCell.PaddingBottom = 10;
                    outerCustomerTable.AddCell(customerCell);
                    document.Add(outerCustomerTable);

                    Paragraph emptySpace2 = new Paragraph();
                    emptySpace2.Add(new Chunk("\n")); // Adjust the font size as needed
                    document.Add(emptySpace2);

                    PdfPTable outerDepartmentTable = new PdfPTable(1);
                    outerDepartmentTable.SetTotalWidth(new float[] { 570f });
                    outerDepartmentTable.WidthPercentage = 100;
                    outerDepartmentTable.DefaultCell.Border = 0;
                    outerDepartmentTable.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
                    PdfPTable departmentTable = new PdfPTable(3);
                    departmentTable.SetTotalWidth(new float[] { 100f, 300f, 150f });
                    departmentTable.WidthPercentage = 100;
                    departmentTable.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
                    departmentTable.AddCell(new PdfPCell(new Phrase("القسم", font)) { Border = Rectangle.NO_BORDER });
                    departmentTable.AddCell(new PdfPCell(new Phrase(lstPrintWO[i].DepartmentNameAr, font)) { Border = Rectangle.NO_BORDER });
                    departmentTable.AddCell(new PdfPCell(new Phrase("Department", englishfont)) { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_LEFT, RunDirection = PdfWriter.RUN_DIRECTION_LTR });
                    departmentTable.AddCell(new PdfPCell(new Phrase("غرفة رقم", font)) { Border = Rectangle.NO_BORDER });
                    departmentTable.AddCell(new PdfPCell(new Phrase("", font)) { Border = Rectangle.NO_BORDER });
                    departmentTable.AddCell(new PdfPCell(new Phrase("Room No.", englishfont)) { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_LEFT, RunDirection = PdfWriter.RUN_DIRECTION_LTR });
                    departmentTable.AddCell(new PdfPCell(new Phrase("اسم المستخدم", font)) { Border = Rectangle.NO_BORDER });
                    departmentTable.AddCell(new PdfPCell(new Phrase(lstPrintWO[i].UserName, font)) { Border = Rectangle.NO_BORDER });
                    departmentTable.AddCell(new PdfPCell(new Phrase("UserName", englishfont)) { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_LEFT, RunDirection = PdfWriter.RUN_DIRECTION_LTR });
                    PdfPCell departmentCell = new PdfPCell(departmentTable);
                    departmentCell.Border = PdfPCell.BOX;
                    departmentCell.PaddingBottom = 10;
                    outerDepartmentTable.AddCell(departmentCell);
                    document.Add(outerDepartmentTable);

                    Paragraph emptySpace3 = new Paragraph();
                    emptySpace3.Add(new Chunk("\n")); // Adjust the font size as needed
                    document.Add(emptySpace3);

                    PdfPTable outerItemTable = new PdfPTable(1);
                    outerItemTable.SetTotalWidth(new float[] { 570f });
                    outerItemTable.WidthPercentage = 100;
                    outerItemTable.DefaultCell.Border = 0;
                    outerItemTable.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
                    PdfPTable itemTable = new PdfPTable(3);
                    itemTable.SetTotalWidth(new float[] { 100f, 300f, 150f });
                    itemTable.WidthPercentage = 100;
                    itemTable.DefaultCell.BorderColor = BaseColor.BLACK;
                    itemTable.DefaultCell.Border = 1;
                    itemTable.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
                    itemTable.AddCell(new PdfPCell(new Phrase("اﻟﻣﺻﻧﻊ / اﻟﻣﻧﺗﺞ", font)) { Border = Rectangle.NO_BORDER });
                    itemTable.AddCell(new PdfPCell(new Phrase(lstPrintWO[i].BrandNameAr, font)) { Border = Rectangle.NO_BORDER });
                    itemTable.AddCell(new PdfPCell(new Phrase("Manufacture", englishfont)) { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_LEFT, RunDirection = PdfWriter.RUN_DIRECTION_LTR });
                  
                    itemTable.AddCell(new PdfPCell(new Phrase("اﻟﺟﮭﺎز", font)) { Border = Rectangle.NO_BORDER });
                    itemTable.AddCell(new PdfPCell(new Phrase(lstPrintWO[i].AssetNameAr, font)) { Border = Rectangle.NO_BORDER });
                    itemTable.AddCell(new PdfPCell(new Phrase("Equipment", englishfont)) { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_LEFT, RunDirection = PdfWriter.RUN_DIRECTION_LTR });
                   
                    itemTable.AddCell(new PdfPCell(new Phrase("رﻗم ﻣﺳﻠﺳل", font)) { Border = Rectangle.NO_BORDER });
                    itemTable.AddCell(new PdfPCell(new Phrase(lstPrintWO[i].SerialNumber, font)) { Border = Rectangle.NO_BORDER });
                    itemTable.AddCell(new PdfPCell(new Phrase("S.N.", englishfont)) { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_LEFT, RunDirection = PdfWriter.RUN_DIRECTION_LTR });
                 
                    itemTable.AddCell(new PdfPCell(new Phrase("اﻟﻣودﯾل", font)) { Border = Rectangle.NO_BORDER });
                    itemTable.AddCell(new PdfPCell(new Phrase(lstPrintWO[i].Model, font)) { Border = Rectangle.NO_BORDER });
                    itemTable.AddCell(new PdfPCell(new Phrase("Model", englishfont)) { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_LEFT, RunDirection = PdfWriter.RUN_DIRECTION_LTR });
                  
                    itemTable.AddCell(new PdfPCell(new Phrase("اﻟرﻗم اﻟﻛودي", font)) { Border = Rectangle.NO_BORDER });
                    itemTable.AddCell(new PdfPCell(new Phrase(lstPrintWO[i].Barcode, font)) { Border = Rectangle.NO_BORDER });
                    itemTable.AddCell(new PdfPCell(new Phrase("Code", font)) { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_LEFT, RunDirection = PdfWriter.RUN_DIRECTION_LTR });
                    PdfPCell itemCell = new PdfPCell(itemTable);
                    itemCell.Border = PdfPCell.BOX;
                    itemCell.PaddingBottom = 10;
                    outerItemTable.AddCell(itemCell);
                    document.Add(outerItemTable);

                    Paragraph emptySpace4 = new Paragraph();
                    emptySpace4.Add(new Chunk("\n")); // Adjust the font size as needed
                    document.Add(emptySpace4);




                    PdfPTable pmTitleTable = new PdfPTable(1);
                    pmTitleTable.SetTotalWidth(new float[] { 500f });
                    pmTitleTable.DefaultCell.BorderColor = BaseColor.BLACK;
                    pmTitleTable.WidthPercentage = 100;
                    pmTitleTable.DefaultCell.BorderWidth = 1f;
                    pmTitleTable.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
                    pmTitleTable.AddCell(new PdfPCell(new Phrase("ﺧطوات اﻟﺻﯾﺎﻧﺔ / PM Steps", font)) { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_CENTER });
                    document.Add(pmTitleTable);





                    if (lstPrintWO[i].ListPMTasks != null)
                    {
                        PdfPTable pmTable = new PdfPTable(1);
                        pmTable.SetTotalWidth(new float[] { 550f });
                        pmTable.SetWidths(new int[] { 200 });
                        pmTable.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
                        pmTable.HorizontalAlignment = Element.ALIGN_RIGHT;
                        pmTable.WidthPercentage = 100;
                        foreach (var item in lstPrintWO[i].ListPMTasks)
                        {
                            pmTable.AddCell(new PdfPCell(new Phrase(item.TaskNameAr, font)) { Border = Rectangle.NO_BORDER });
                        }

                        PdfPTable pmTable2 = new PdfPTable(1);
                        pmTable2.SetTotalWidth(new float[] { 550f });
                        pmTable2.SetWidths(new int[] { 200 });
                        pmTable2.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
                        pmTable2.HorizontalAlignment = Element.ALIGN_RIGHT;
                        pmTable2.WidthPercentage = 100;
                        int countRows = Math.Min(40, pmTable.Rows.Count);
                        for (int j = 0; j < countRows; j++)
                        {
                            pmTable2.Rows.Add(pmTable.Rows[j]);
                        }
                        for (int k = 1; k <= pmTable2.Rows.Count - 1; k++)
                        {
                            pmTable.DeleteRow(1);
                        }

                        float startY = 0;

                        float endPosition = startY - pmTable2.TotalHeight;
                        endPosition -= 20;
                        if (endPosition >= document.BottomMargin)
                        {
                            document.NewPage();
                        }
                        else
                        {
                            startY = endPosition;
                        }

                        document.Add(pmTable2);
                    }

                    Paragraph listSpace = new Paragraph();
                    listSpace.Add(new Chunk("\n")); // Adjust the font size as needed
                    document.Add(listSpace);


                    PdfPTable mainTable = new PdfPTable(1);
                    mainTable.SetTotalWidth(new float[] { 570f });
                    mainTable.WidthPercentage = 100;

                    //  Create main table cell
                    PdfPCell mainCell = new PdfPCell();
                    mainCell.Border = Rectangle.NO_BORDER;

                    //  Create inner table
                    PdfPTable innerTable2 = new PdfPTable(4);
                    innerTable2.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
                    innerTable2.SetTotalWidth(new float[] { 150f, 100f, 100f, 100f });
                    innerTable2.WidthPercentage = 100;
                    string[] innerCellContent = { "الكمية / Qty", "الوصف / Description", "الكود / Code", "قطع الغيار / Spare Parts" };

                    foreach (string content in innerCellContent)
                    {
                        PdfPCell innerCell = new PdfPCell(new Phrase(content, font));
                        innerCell.HorizontalAlignment = Element.ALIGN_CENTER;
                        innerCell.VerticalAlignment = Element.ALIGN_MIDDLE;

                        string cellText = innerCell.Phrase.Content;

                        // Check if the cell contains "قطع الغيار / Spare Parts"
                        if (cellText.Contains("قطع الغيار / Spare Parts"))
                        {
                            // Create the inner table
                            PdfPTable nestedTable = new PdfPTable(2);
                            nestedTable.SetTotalWidth(new float[] { 100f, 100f });
                            nestedTable.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
                            nestedTable.WidthPercentage = 100;

                            // Add cells to the inner table
                            nestedTable.AddCell(new PdfPCell(new Phrase("ﻗطﻊ اﻟﻐﯾﺎر / Parts Spare", font))
                            {
                                Colspan = 2, // Merge across all two columns
                                HorizontalAlignment = Element.ALIGN_CENTER // Center the content
                            });
                            nestedTable.AddCell(new PdfPCell(new Phrase("ﻣطﻠوﺑﺔ / Ordered", font)));
                            nestedTable.AddCell(new PdfPCell(new Phrase("ﻣﺗوﻓر / Used", font)));

                            // Clear the content of the main cell
                            innerCell.Phrase = new Phrase();

                            // Add the nested table to the main cell
                            innerCell.AddElement(nestedTable);
                        }

                        innerTable2.AddCell(innerCell);
                    }

                    innerTable2.AddCell(new Phrase(" ", font));
                    innerTable2.AddCell(new Phrase(" ", font));
                    innerTable2.AddCell(new Phrase(" ", font));
                    innerTable2.AddCell(new Phrase(" ", font));
                    innerTable2.AddCell(new Phrase(" ", font));
                    innerTable2.AddCell(new Phrase(" ", font));
                    innerTable2.AddCell(new Phrase(" ", font));
                    innerTable2.AddCell(new Phrase(" ", font));
                    innerTable2.AddCell(new Phrase(" ", font));
                    innerTable2.AddCell(new Phrase(" ", font));
                    innerTable2.AddCell(new Phrase(" ", font));
                    innerTable2.AddCell(new Phrase(" ", font));
                    innerTable2.AddCell(new Phrase(" ", font));
                    innerTable2.AddCell(new Phrase(" ", font));
                    innerTable2.AddCell(new Phrase(" ", font));
                    innerTable2.AddCell(new Phrase(" ", font));
                    innerTable2.AddCell(new Phrase(" ", font));
                    mainCell.AddElement(innerTable2);
                    mainTable.AddCell(mainCell);
                    document.Add(mainTable);

                    Paragraph emptySpace5 = new Paragraph();
                    emptySpace5.Add(new Chunk("\n")); // Adjust the font size as needed
                    document.Add(emptySpace5);


                    PdfPTable outerTable = new PdfPTable(1);
                    outerTable.SetTotalWidth(new float[] { 570f });
                    outerTable.WidthPercentage = 100;
                    outerTable.DefaultCell.Border = 0;
                    outerTable.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
                    //  Create a nested table
                    PdfPTable innerTable = new PdfPTable(4);
                    innerTable.SetTotalWidth(new float[] { 100f, 100f, 100f, 100f });
                    innerTable.WidthPercentage = 100;
                    innerTable.DefaultCell.Border = 0;
                    innerTable.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
                    PdfPCell cell1 = new PdfPCell(new Phrase("ﻋن اﻟﻣﺳﺗﺷﻔﻰ", font));
                    cell1.Border = 0;
                    cell1.Colspan = 2; // Merge 2 cells horizontally
                    innerTable.AddCell(new PdfPCell(cell1) { HorizontalAlignment = Element.ALIGN_CENTER });
                    PdfPCell cell2 = new PdfPCell(new Phrase("ﻣﮭﻧدس اﻟﺻﯾﺎﻧﺔ", font));
                    cell2.Border = 0;
                    cell2.Colspan = 2; // Merge 2 cells horizontally
                    innerTable.AddCell(new PdfPCell(cell2) { HorizontalAlignment = Element.ALIGN_CENTER });
                    innerTable.AddCell(new PdfPCell(new Phrase("اﻻﺳم", font)) { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.SECTION });
                    innerTable.AddCell("");
                    innerTable.AddCell(new PdfPCell(new Phrase("اﻻﺳم", font)) { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.SECTION });
                    innerTable.AddCell("");
                    innerTable.AddCell(new PdfPCell(new Phrase("التوقيع", font)) { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.SECTION });
                    innerTable.AddCell("");
                    innerTable.AddCell(new PdfPCell(new Phrase("التوقيع", font)) { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.SECTION });
                    innerTable.AddCell("");
                    innerTable.AddCell(new PdfPCell(new Phrase("التاريخ", font)) { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.SECTION });
                    innerTable.AddCell("");
                    innerTable.AddCell(new PdfPCell(new Phrase("التاريخ", font)) { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.SECTION });
                    innerTable.AddCell("");
                    PdfPCell cell = new PdfPCell(innerTable);
                    cell.Border = PdfPCell.BOX;
                    cell.PaddingBottom = 10;
                    outerTable.AddCell(cell);
                    document.Add(outerTable);











                    document.NewPage();
                }
            }
            catch (DocumentException de)
            {
                Console.Error.WriteLine(de.Message);
            }
            catch (IOException ioe)
            {
                Console.Error.WriteLine(ioe.Message);
            }
            finally
            {
                // Close the document
                document.Close();
            }
        }

        [HttpPost]
        [Route("GenerateWOPMDocumentByDepartment")]
        public void GenerateWOPMDocumentByDepartment(SortAndFilterVM workOrders)
        {
            List<IndexAssetDetailVM.GetData> lstPrintWO = new List<IndexAssetDetailVM.GetData>();

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            string adobearabic = _webHostingEnvironment.ContentRootPath + "/Font/adobearabic.ttf";
            BaseFont bfUniCode = BaseFont.CreateFont(adobearabic, BaseFont.IDENTITY_H, true);
            iTextSharp.text.Font font = new iTextSharp.text.Font(bfUniCode, 13);
            iTextSharp.text.Font englishfont = new iTextSharp.text.Font(bfUniCode, 12);
            iTextSharp.text.Font reportfont = new iTextSharp.text.Font(bfUniCode, 8);

            lstPrintWO = _AssetDetailService.PrintListOfPMWorkOrders(workOrders);



            int countPages = lstPrintWO.Count;

            Document document = new Document();

            try
            {
                if (lstPrintWO.Count > 0)
                {
                    PdfWriter.GetInstance(document, new FileStream(_webHostingEnvironment.ContentRootPath + "/UploadedAttachments/WOReports/WOPM2.pdf", FileMode.Create));
                    document.Open();
                    for (int i = 0; i < lstPrintWO.Count; i++)
                    {
                        PdfPTable headertable = new PdfPTable(3);
                        headertable.DefaultCell.Border = 0;
                        headertable.SetTotalWidth(new float[] { 100f, 300f, 150f });
                        headertable.WidthPercentage = 100;

                        headertable.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
                        headertable.AddCell(new Phrase("ﻗطﺎع ﻣﺷروﻋﺎت اﻟﺻﺣﺔ", font));
                        headertable.AddCell(new PdfPCell(new Phrase("ﺗﻘرﯾر ﻋﻣل / Sheet Job", font)) { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE });
                        headertable.AddCell(new PdfPCell(new Phrase("Report No. 1", reportfont)) { Border = Rectangle.NO_BORDER, RunDirection = PdfWriter.RUN_DIRECTION_LTR });
                        document.Add(headertable);

                        Paragraph emptySpace = new Paragraph();
                        emptySpace.Add(new Chunk("\n")); // Adjust the font size as needed
                        document.Add(emptySpace);

                        PdfPTable outerCustomerTable = new PdfPTable(1);
                        outerCustomerTable.SetTotalWidth(new float[] { 570f });
                        outerCustomerTable.WidthPercentage = 100;
                        outerCustomerTable.DefaultCell.Border = 0;
                        outerCustomerTable.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
                        PdfPTable customerTable = new PdfPTable(3);
                        customerTable.SetTotalWidth(new float[] { 100f, 350f, 100f });
                        customerTable.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
                        customerTable.WidthPercentage = 100;
                        customerTable.AddCell(new PdfPCell(new Phrase("العميل", font)) { Border = Rectangle.NO_BORDER });
                        customerTable.AddCell(new PdfPCell(new Phrase(lstPrintWO[i].HospitalNameAr, font)) { Border = Rectangle.NO_BORDER });
                        customerTable.AddCell(new PdfPCell(new Phrase("Customer", englishfont)) { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_LEFT, RunDirection = PdfWriter.RUN_DIRECTION_LTR });
                        customerTable.AddCell(new PdfPCell(new Phrase("العنوان", font)) { Border = Rectangle.NO_BORDER });
                        customerTable.AddCell(new PdfPCell(new Phrase("", font)) { Border = Rectangle.NO_BORDER });
                        customerTable.AddCell(new PdfPCell(new Phrase("Address", englishfont)) { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_LEFT, RunDirection = PdfWriter.RUN_DIRECTION_LTR });
                        customerTable.AddCell(new PdfPCell(new Phrase("التاريخ", font)) { Border = Rectangle.NO_BORDER });
                        //if (lstPrintWO[i].CreationDate != null)
                        //    customerTable.AddCell(new PdfPCell(new Phrase(lstPrintWO[i].CreationDate.Value.ToShortDateString(), font)) { Border = Rectangle.NO_BORDER });
                        //else
                        customerTable.AddCell(new PdfPCell(new Phrase(ConvertDateTimeToArabicNumerals.ConvertToArabicNumerals(DateTime.Today.Date.ToShortDateString()), font)) { Border = Rectangle.NO_BORDER });

                        customerTable.AddCell(new PdfPCell(new Phrase("Date", englishfont)) { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_LEFT, RunDirection = PdfWriter.RUN_DIRECTION_LTR });
                        PdfPCell customerCell = new PdfPCell(customerTable);
                        customerCell.Border = PdfPCell.BOX;
                        customerCell.PaddingBottom = 10;
                        outerCustomerTable.AddCell(customerCell);
                        document.Add(outerCustomerTable);

                        Paragraph emptySpace2 = new Paragraph();
                        emptySpace2.Add(new Chunk("\n")); // Adjust the font size as needed
                        document.Add(emptySpace2);

                        PdfPTable outerDepartmentTable = new PdfPTable(1);
                        outerDepartmentTable.SetTotalWidth(new float[] { 570f });
                        outerDepartmentTable.WidthPercentage = 100;
                        outerDepartmentTable.DefaultCell.Border = 0;
                        outerDepartmentTable.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
                        PdfPTable departmentTable = new PdfPTable(3);
                        departmentTable.SetTotalWidth(new float[] { 100f, 300f, 150f });
                        departmentTable.WidthPercentage = 100;
                        departmentTable.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
                        departmentTable.AddCell(new PdfPCell(new Phrase("القسم", font)) { Border = Rectangle.NO_BORDER });
                        departmentTable.AddCell(new PdfPCell(new Phrase(lstPrintWO[i].DepartmentNameAr, font)) { Border = Rectangle.NO_BORDER });
                        departmentTable.AddCell(new PdfPCell(new Phrase("Department", englishfont)) { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_LEFT, RunDirection = PdfWriter.RUN_DIRECTION_LTR });
                        departmentTable.AddCell(new PdfPCell(new Phrase("غرفة رقم", font)) { Border = Rectangle.NO_BORDER });
                        departmentTable.AddCell(new PdfPCell(new Phrase("", font)) { Border = Rectangle.NO_BORDER });
                        departmentTable.AddCell(new PdfPCell(new Phrase("Room No.", englishfont)) { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_LEFT, RunDirection = PdfWriter.RUN_DIRECTION_LTR });
                        departmentTable.AddCell(new PdfPCell(new Phrase("اسم المستخدم", font)) { Border = Rectangle.NO_BORDER });
                        departmentTable.AddCell(new PdfPCell(new Phrase(lstPrintWO[i].UserName, font)) { Border = Rectangle.NO_BORDER });
                        departmentTable.AddCell(new PdfPCell(new Phrase("UserName", englishfont)) { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_LEFT, RunDirection = PdfWriter.RUN_DIRECTION_LTR });
                        PdfPCell departmentCell = new PdfPCell(departmentTable);
                        departmentCell.Border = PdfPCell.BOX;
                        departmentCell.PaddingBottom = 10;
                        outerDepartmentTable.AddCell(departmentCell);
                        document.Add(outerDepartmentTable);

                        Paragraph emptySpace3 = new Paragraph();
                        emptySpace3.Add(new Chunk("\n")); // Adjust the font size as needed
                        document.Add(emptySpace3);

                        PdfPTable outerItemTable = new PdfPTable(1);
                        outerItemTable.SetTotalWidth(new float[] { 570f });
                        outerItemTable.WidthPercentage = 100;
                        outerItemTable.DefaultCell.Border = 0;
                        outerItemTable.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
                        PdfPTable itemTable = new PdfPTable(3);
                        itemTable.SetTotalWidth(new float[] { 100f, 300f, 150f });
                        itemTable.WidthPercentage = 100;
                        itemTable.DefaultCell.BorderColor = BaseColor.BLACK;
                        itemTable.DefaultCell.Border = 1;
                        itemTable.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
                        itemTable.AddCell(new PdfPCell(new Phrase("اﻟﻣﺻﻧﻊ / اﻟﻣﻧﺗﺞ", font)) { Border = Rectangle.NO_BORDER });
                        itemTable.AddCell(new PdfPCell(new Phrase(lstPrintWO[i].BrandNameAr, font)) { Border = Rectangle.NO_BORDER });
                        itemTable.AddCell(new PdfPCell(new Phrase("Manufacture", englishfont)) { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_LEFT, RunDirection = PdfWriter.RUN_DIRECTION_LTR });
                        itemTable.AddCell(new PdfPCell(new Phrase("اﻟﺟﮭﺎز", font)) { Border = Rectangle.NO_BORDER });
                        itemTable.AddCell(new PdfPCell(new Phrase(lstPrintWO[i].AssetNameAr, font)) { Border = Rectangle.NO_BORDER });
                        itemTable.AddCell(new PdfPCell(new Phrase("Equipment", englishfont)) { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_LEFT, RunDirection = PdfWriter.RUN_DIRECTION_LTR });
                        itemTable.AddCell(new PdfPCell(new Phrase("رﻗم ﻣﺳﻠﺳل", font)) { Border = Rectangle.NO_BORDER });
                        itemTable.AddCell(new PdfPCell(new Phrase(lstPrintWO[i].SerialNumber, font)) { Border = Rectangle.NO_BORDER });
                        itemTable.AddCell(new PdfPCell(new Phrase("S.N.", englishfont)) { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_LEFT, RunDirection = PdfWriter.RUN_DIRECTION_LTR });
                        itemTable.AddCell(new PdfPCell(new Phrase("اﻟﻣودﯾل", font)) { Border = Rectangle.NO_BORDER });
                        itemTable.AddCell(new PdfPCell(new Phrase(lstPrintWO[i].Model, font)) { Border = Rectangle.NO_BORDER });
                        itemTable.AddCell(new PdfPCell(new Phrase("Model", englishfont)) { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_LEFT, RunDirection = PdfWriter.RUN_DIRECTION_LTR });
                        itemTable.AddCell(new PdfPCell(new Phrase("اﻟرﻗم اﻟﻛودي", font)) { Border = Rectangle.NO_BORDER });
                        itemTable.AddCell(new PdfPCell(new Phrase(lstPrintWO[i].Barcode, font)) { Border = Rectangle.NO_BORDER });
                        itemTable.AddCell(new PdfPCell(new Phrase("Code", englishfont)) { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_LEFT, RunDirection = PdfWriter.RUN_DIRECTION_LTR });

                        PdfPCell itemCell = new PdfPCell(itemTable);
                        itemCell.Border = PdfPCell.BOX;
                        itemCell.PaddingBottom = 10;
                        outerItemTable.AddCell(itemCell);
                        document.Add(outerItemTable);

                        Paragraph emptySpace4 = new Paragraph();
                        emptySpace4.Add(new Chunk("\n")); // Adjust the font size as needed
                        document.Add(emptySpace4);




                        PdfPTable pmTitleTable = new PdfPTable(1);
                        pmTitleTable.SetTotalWidth(new float[] { 500f });
                        pmTitleTable.DefaultCell.BorderColor = BaseColor.BLACK;
                        pmTitleTable.WidthPercentage = 100;
                        pmTitleTable.DefaultCell.BorderWidth = 1f;
                        pmTitleTable.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
                        pmTitleTable.AddCell(new PdfPCell(new Phrase("ﺧطوات اﻟﺻﯾﺎﻧﺔ / PM Steps", font)) { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_CENTER });
                        document.Add(pmTitleTable);





                        if (lstPrintWO[i].ListPMTasks != null)
                        {
                            PdfPTable pmTable = new PdfPTable(1);
                            pmTable.SetTotalWidth(new float[] { 550f });
                            pmTable.SetWidths(new int[] { 200 });
                            pmTable.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
                            pmTable.HorizontalAlignment = Element.ALIGN_RIGHT;
                            pmTable.WidthPercentage = 100;
                            foreach (var item in lstPrintWO[i].ListPMTasks)
                            {
                                pmTable.AddCell(new PdfPCell(new Phrase(item.TaskNameAr, font)) { Border = Rectangle.NO_BORDER });
                            }

                            PdfPTable pmTable2 = new PdfPTable(1);
                            pmTable2.SetTotalWidth(new float[] { 550f });
                            pmTable2.SetWidths(new int[] { 200 });
                            pmTable2.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
                            pmTable2.HorizontalAlignment = Element.ALIGN_RIGHT;
                            pmTable2.WidthPercentage = 100;
                            int countRows = Math.Min(40, pmTable.Rows.Count);
                            for (int j = 0; j < countRows; j++)
                            {
                                pmTable2.Rows.Add(pmTable.Rows[j]);
                            }
                            for (int k = 1; k <= pmTable2.Rows.Count - 1; k++)
                            {
                                pmTable.DeleteRow(1);
                            }

                            float startY = 0;

                            float endPosition = startY - pmTable2.TotalHeight;
                            endPosition -= 20;
                            if (endPosition >= document.BottomMargin)
                            {
                                document.NewPage();
                            }
                            else
                            {
                                startY = endPosition;
                            }

                            document.Add(pmTable2);
                        }

                        Paragraph listSpace = new Paragraph();
                        listSpace.Add(new Chunk("\n")); // Adjust the font size as needed
                        document.Add(listSpace);


                        PdfPTable mainTable = new PdfPTable(1);
                        mainTable.SetTotalWidth(new float[] { 570f });
                        mainTable.WidthPercentage = 100;

                        //  Create main table cell
                        PdfPCell mainCell = new PdfPCell();
                        mainCell.Border = Rectangle.NO_BORDER;

                        //  Create inner table
                        PdfPTable innerTable2 = new PdfPTable(4);
                        innerTable2.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
                        innerTable2.SetTotalWidth(new float[] { 150f, 100f, 100f, 100f });
                        innerTable2.WidthPercentage = 100;
                        string[] innerCellContent = { "الكمية / Qty", "الوصف / Description", "الكود / Code", "قطع الغيار / Spare Parts" };

                        foreach (string content in innerCellContent)
                        {
                            PdfPCell innerCell = new PdfPCell(new Phrase(content, font));
                            innerCell.HorizontalAlignment = Element.ALIGN_CENTER;
                            innerCell.VerticalAlignment = Element.ALIGN_MIDDLE;

                            string cellText = innerCell.Phrase.Content;

                            // Check if the cell contains "قطع الغيار / Spare Parts"
                            if (cellText.Contains("قطع الغيار / Spare Parts"))
                            {
                                // Create the inner table
                                PdfPTable nestedTable = new PdfPTable(2);
                                nestedTable.SetTotalWidth(new float[] { 100f, 100f });
                                nestedTable.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
                                nestedTable.WidthPercentage = 100;

                                // Add cells to the inner table
                                nestedTable.AddCell(new PdfPCell(new Phrase("ﻗطﻊ اﻟﻐﯾﺎر / Parts Spare", font))
                                {
                                    Colspan = 2, // Merge across all two columns
                                    HorizontalAlignment = Element.ALIGN_CENTER // Center the content
                                });
                                nestedTable.AddCell(new PdfPCell(new Phrase("ﻣطﻠوﺑﺔ / Ordered", font)));
                                nestedTable.AddCell(new PdfPCell(new Phrase("ﻣﺗوﻓر / Used", font)));

                                // Clear the content of the main cell
                                innerCell.Phrase = new Phrase();

                                // Add the nested table to the main cell
                                innerCell.AddElement(nestedTable);
                            }

                            innerTable2.AddCell(innerCell);
                        }

                        innerTable2.AddCell(new Phrase(" ", font));
                        innerTable2.AddCell(new Phrase(" ", font));
                        innerTable2.AddCell(new Phrase(" ", font));
                        innerTable2.AddCell(new Phrase(" ", font));
                        innerTable2.AddCell(new Phrase(" ", font));
                        innerTable2.AddCell(new Phrase(" ", font));
                        innerTable2.AddCell(new Phrase(" ", font));
                        innerTable2.AddCell(new Phrase(" ", font));
                        innerTable2.AddCell(new Phrase(" ", font));
                        innerTable2.AddCell(new Phrase(" ", font));
                        innerTable2.AddCell(new Phrase(" ", font));
                        innerTable2.AddCell(new Phrase(" ", font));
                        innerTable2.AddCell(new Phrase(" ", font));
                        innerTable2.AddCell(new Phrase(" ", font));
                        innerTable2.AddCell(new Phrase(" ", font));
                        innerTable2.AddCell(new Phrase(" ", font));
                        innerTable2.AddCell(new Phrase(" ", font));
                        mainCell.AddElement(innerTable2);
                        mainTable.AddCell(mainCell);
                        document.Add(mainTable);

                        Paragraph emptySpace5 = new Paragraph();
                        emptySpace5.Add(new Chunk("\n")); // Adjust the font size as needed
                        document.Add(emptySpace5);


                        PdfPTable outerTable = new PdfPTable(1);
                        outerTable.SetTotalWidth(new float[] { 570f });
                        outerTable.WidthPercentage = 100;
                        outerTable.DefaultCell.Border = 0;
                        outerTable.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
                        //  Create a nested table
                        PdfPTable innerTable = new PdfPTable(4);
                        innerTable.SetTotalWidth(new float[] { 100f, 100f, 100f, 100f });
                        innerTable.WidthPercentage = 100;
                        innerTable.DefaultCell.Border = 0;
                        innerTable.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
                        PdfPCell cell1 = new PdfPCell(new Phrase("ﻋن اﻟﻣﺳﺗﺷﻔﻰ", font));
                        cell1.Border = 0;
                        cell1.Colspan = 2; // Merge 2 cells horizontally
                        innerTable.AddCell(new PdfPCell(cell1) { HorizontalAlignment = Element.ALIGN_CENTER });
                        PdfPCell cell2 = new PdfPCell(new Phrase("ﻣﮭﻧدس اﻟﺻﯾﺎﻧﺔ", font));
                        cell2.Border = 0;
                        cell2.Colspan = 2; // Merge 2 cells horizontally
                        innerTable.AddCell(new PdfPCell(cell2) { HorizontalAlignment = Element.ALIGN_CENTER });
                        innerTable.AddCell(new PdfPCell(new Phrase("اﻻﺳم", font)) { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.SECTION });
                        innerTable.AddCell("");
                        innerTable.AddCell(new PdfPCell(new Phrase("اﻻﺳم", font)) { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.SECTION });
                        innerTable.AddCell("");
                        innerTable.AddCell(new PdfPCell(new Phrase("التوقيع", font)) { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.SECTION });
                        innerTable.AddCell("");
                        innerTable.AddCell(new PdfPCell(new Phrase("التوقيع", font)) { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.SECTION });
                        innerTable.AddCell("");
                        innerTable.AddCell(new PdfPCell(new Phrase("التاريخ", font)) { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.SECTION });
                        innerTable.AddCell("");
                        innerTable.AddCell(new PdfPCell(new Phrase("التاريخ", font)) { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.SECTION });
                        innerTable.AddCell("");
                        PdfPCell cell = new PdfPCell(innerTable);
                        cell.Border = PdfPCell.BOX;
                        cell.PaddingBottom = 10;
                        outerTable.AddCell(cell);
                        document.Add(outerTable);


                        document.NewPage();
                    }
                }
            }
            catch (DocumentException de)
            {
                Console.Error.WriteLine(de.Message);
            }
            catch (IOException ioe)
            {
                Console.Error.WriteLine(ioe.Message);
            }
            finally
            {
                // Close the document
                document.Close();
            }
        }


        [HttpGet]
        [Route("DownloadWOPMPDF/{fileName}")]
        public HttpResponseMessage DownloadWOPMPDF(string fileName)
        {
            var file = _webHostingEnvironment.ContentRootPath + "/UploadedAttachments/WOReports/" + fileName;
            HttpResponseMessage response = null;
            if (!System.IO.File.Exists(file))
            {
                var folder = Directory.CreateDirectory(_webHostingEnvironment.ContentRootPath + "/UploadedAttachments/WOReports/");
                var openFile = System.IO.File.Create(folder + fileName);
                openFile.Close();

                var file2 = folder + fileName;
                var fStream = new FileStream(file2, FileMode.Open, FileAccess.Read);
                response = new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StreamContent(fStream)
                };
                response.Content.Headers.ContentDisposition =
                                            new ContentDispositionHeaderValue("attachment")
                                            {
                                                FileName = Path.GetFileName(fStream.Name)
                                            };
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            }
            else
            {
                //if file present than read file 
                var fStream = new FileStream(file, FileMode.Open, FileAccess.Read);
                //compose response and include file as content in it
                response = new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StreamContent(fStream)
                };
                response.Content.Headers.ContentDisposition =
                                            new ContentDispositionHeaderValue("attachment")
                                            {
                                                FileName = Path.GetFileName(fStream.Name)
                                            };
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            }
            return response;
        }


    }
}
