using Asset.API.Helpers;
using Asset.Domain.Services;
using Asset.Models;
using Asset.ViewModels.PagingParameter;
using Asset.ViewModels.RequestStatusVM;
using Asset.ViewModels.RequestVM;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Asset.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RequestStatusController : ControllerBase
    {
        private IRequestStatusService _requestStatusService;
        private IRequestTrackingService _requestTrackingService;
        private IPagingService _pagingService;
        IWebHostEnvironment _webHostingEnvironment;
        private readonly ISettingService _settingService;

        string strInsitute, strInsituteAr, strLogo = "";
        public RequestStatusController(ISettingService settingService,IWebHostEnvironment webHostingEnvironment,IRequestStatusService requestStatusService, IRequestTrackingService requestTrackingService, IPagingService pagingService)
        {
            _requestStatusService = requestStatusService;
            _requestTrackingService = requestTrackingService;
            _pagingService = pagingService;
            _webHostingEnvironment=webHostingEnvironment;
            _settingService = settingService;
    }
      
        [HttpGet]
        public IEnumerable<IndexRequestStatusVM.GetData> Get()
        {
            return _requestStatusService.GetAllRequestStatus();
        }



        [HttpGet]
        [Route("GetRequestStatusByUserId/{userId}")]
        public async Task<List<RequestStatusVM>> GetRequestStatusByUserId(string userId)
        {
            return await _requestStatusService.GetRequestStatusByUserId(userId);
        }



        [HttpGet]
        [Route("GetAllForReport")]
        public IndexRequestStatusVM.GetData GetAllForReport()
        {
            return _requestStatusService.GetAllForReport();
        }




        [HttpPost]
        [Route("GetAllForReportByDate")]
        public IndexRequestStatusVM.GetData GetAllForReportByDate(SearchRequestDateVM requestDateObj)
        {
            return _requestStatusService.GetAllForReport(requestDateObj);
        }



        [HttpGet]
        [Route("GetAll/{userId}")]
        public IndexRequestStatusVM.GetData GetAll(string userId)
        {
            return _requestStatusService.GetAll(userId);
        }


        [HttpGet]
        [Route("GetAllByHospitalId/{userId}/{hospitalId}")]
        public IndexRequestStatusVM.GetData GetAllByHospitalId(string userId,int hospitalId)
        {
            return _requestStatusService.GetAllByHospitalId(userId,hospitalId);
        }



        [HttpPut]
        [Route("GetRequestStatusesWithPaging")]
        public IEnumerable<IndexRequestStatusVM.GetData> GetAll(PagingParameter pageInfo)
        {
            var lstBrands = _requestStatusService.GetAllRequestStatus().ToList();
            return _pagingService.GetAll<IndexRequestStatusVM.GetData>(pageInfo, lstBrands);
        }

        [HttpGet]
        [Route("GetCount")]
        public int count()
        {
            return _requestStatusService.GetAllRequestStatus().ToList().Count;
        }


        [HttpPost]
        [Route("SortRequestStatuses/{pagenumber}/{pagesize}")]
        public IEnumerable<IndexRequestStatusVM.GetData> SortWorkOrderTypes(int pagenumber, int pagesize, SortRequestStatusVM sortObj)
        {
            PagingParameter pageInfo = new PagingParameter();
            pageInfo.PageNumber = pagenumber;
            pageInfo.PageSize = pagesize;
            var list = _requestStatusService.SortRequestStatuses(sortObj);
            return _pagingService.GetAll<IndexRequestStatusVM.GetData>(pageInfo, list.ToList());
        }





        [HttpGet]
        [Route("GetById/{id}")]
        public ActionResult<RequestStatus> GetById(int id)
        {
            return _requestStatusService.GetById(id);
        }

        // POST api/<RequestStatusController>
        public ActionResult<IndexRequestStatusVM> Post(RequestStatus createRequestStatusVM)
        {
           
            var lstNames = _requestStatusService.GetAllRequestStatus().ToList().Where(a => a.Name == createRequestStatusVM.Name).ToList();
            if (lstNames.Count > 0)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "name", Message = "Request status name already exist", MessageAr = "هذا الاسم مسجل سابقاً" });
            }
            var lstArNames = _requestStatusService.GetAllRequestStatus().ToList().Where(a => a.NameAr == createRequestStatusVM.NameAr).ToList();
            if (lstArNames.Count > 0)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "name", Message = "Request status arabic name already exist", MessageAr = "هذا الاسم مسجل سابقاً" });
            }
            else
            {
                _requestStatusService.Add(createRequestStatusVM);
                return Ok();
            }

        }

        [HttpPut]
        [Route("UpdateRequestStatus")]
        public IActionResult PutRequestStatus(RequestStatus editRequestStatus)
        {
           
            var lstNames = _requestStatusService.GetAllRequestStatus().ToList().Where(a => a.Name == editRequestStatus.Name && a.Id != editRequestStatus.Id).ToList();
            if (lstNames.Count > 0)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "name", Message = " name already exist", MessageAr = "هذا الاسم مسجل سابقاً" });
            }
            var lstArNames = _requestStatusService.GetAllRequestStatus().ToList().Where(a => a.NameAr == editRequestStatus.NameAr && a.Id != editRequestStatus.Id).ToList();
            if (lstArNames.Count > 0)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "nameAr", Message = " arabic name already exist", MessageAr = "هذا الاسم مسجل سابقاً" });
            }
            else
            {
                _requestStatusService.Update(editRequestStatus);
                return Ok();
            }
        }

        // DELETE api/<RequestStatusController>/5

        [HttpDelete]
        [Route("DeleteReqStatus/{id}")]
        public ActionResult<IndexRequestStatusVM> Delete(int id)
        {
            try
            {
                var lstRequestTracking = _requestTrackingService.GetAll().Where(a => a.RequestStatusId == id).ToList();
                if (lstRequestTracking.Count > 0)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "reqStatus", Message = " arabic name already exist", MessageAr = "هذا الاسم مسجل سابقاً" });
                }
                else
                {
                   int deletedRow = _requestStatusService.Delete(id);
                }
            }
            catch (DbUpdateConcurrencyException ex)
            {
                string msg = ex.Message;
                return BadRequest("Error in delete");
            }

            return Ok();
        }


        [HttpPost]
        [Route("CreateRequestStatusPDF/{lang}")]
        public void CreateAssetStatusPDF(string lang)
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
            Document document = new Document(PageSize.A4, 10f, 10f, 10f, 10f);
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

            PdfPTable bodytable = CreateAssetStatusTable(lang);
            int countnewpages = bodytable.Rows.Count / 12;
            for (int i = 1; i <= countnewpages; i++)
            {
                document.NewPage();
                writer.PageEmpty = false;
            }

            document.Close();
            byte[] bytes = memoryStream.ToArray();
            System.IO.File.WriteAllBytes(_webHostingEnvironment.ContentRootPath + "/UploadedAttachments/RequestStatus/RequestStatusList.pdf", bytes);
            memoryStream = new MemoryStream();
            PdfReader reader = new PdfReader(bytes);
            using (PdfStamper stamper = new PdfStamper(reader, memoryStream))
            {
                int pages = reader.NumberOfPages;
                //Footer
                for (int i = 1; i <= pages; i++)
                {
                    ColumnText.ShowTextAligned(stamper.GetUnderContent(i), Element.ALIGN_LEFT, new Phrase(ArabicNumeralHelper.toArabicNumber(pages.ToString()) + "/" + ArabicNumeralHelper.toArabicNumber(i.ToString()), font), 500f, 5f, 0);
                    //  ColumnText.ShowTextAligned(stamper.GetUnderContent(i), Element.ALIGN_RIGHT, new Phrase("تمت الطباعة بواسطة  " + "PrintedBy", font), 150f, 15f, 0, PdfWriter.RUN_DIRECTION_RTL, ColumnText.AR_LIG);
                }
                //Header
                for (int i = 1; i <= pages; i++)
                {
                    string imageURL = _webHostingEnvironment.ContentRootPath + "/Images/" + strLogo;
                    iTextSharp.text.Image jpg = iTextSharp.text.Image.GetInstance(imageURL);
                    jpg.ScaleAbsolute(70f, 50f);
                    PdfPTable headertable = new PdfPTable(2);
                    headertable.SetTotalWidth(new float[] { 80f, 50f });
                    headertable.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
                    headertable.WidthPercentage = 100;
                    PdfPCell cell = new PdfPCell(new PdfPCell(jpg));
                    cell.PaddingTop = 5;
                    cell.Border = Rectangle.NO_BORDER;
                    cell.PaddingRight = 5;
                    headertable.AddCell(cell);
                    if (lang == "ar")
                        headertable.AddCell(new PdfPCell(new Phrase(strInsituteAr + "\n" + "", font)) { Border = Rectangle.NO_BORDER, PaddingTop = 15 });
                    else
                        headertable.AddCell(new PdfPCell(new Phrase(strInsitute + "\n" + "", font)) { Border = Rectangle.NO_BORDER, PaddingTop = 15 });


                    headertable.WriteSelectedRows(0, -1, 420, 820, stamper.GetOverContent(i));

                    System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                    string adobearabicheaderTitle = _webHostingEnvironment.ContentRootPath + "/Font/adobearabic.ttf";
                    BaseFont bfUniCodeheaderTitle = BaseFont.CreateFont(adobearabicheaderTitle, BaseFont.IDENTITY_H, true);
                    iTextSharp.text.Font titlefont = new iTextSharp.text.Font(bfUniCodeheaderTitle, 16);
                    titlefont.SetStyle("bold");


                    PdfPTable titleTable = new PdfPTable(1);
                    titleTable.SetTotalWidth(new float[] { 500f });
                    titleTable.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
                    titleTable.WidthPercentage = 100;
                    titleTable.AddCell(new PdfPCell(new Phrase("نوع بلاغ العطل", titlefont)) { PaddingBottom = 5, Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_CENTER });
                    titleTable.WriteSelectedRows(0, -1, 5, 770, stamper.GetOverContent(i));
                }

                for (int i = 1; i <= pages; i++)
                {
                    PdfPTable bodytable2 = new PdfPTable(4);
                    bodytable2.SetTotalWidth(new float[] { 120f, 150f, 150f, 150f });
                    bodytable2.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
                    bodytable2.HorizontalAlignment = Element.ALIGN_RIGHT;
                    bodytable2.WidthPercentage = 100;
                    bodytable2.PaddingTop = 200;
                    bodytable2.HeaderRows = 1;
                    bodytable2.SetWidths(new int[] { 80, 80, 80 ,80});

                    int countRows = bodytable.Rows.Count;
                    if (countRows > 12)
                    {
                        countRows = 12;
                    }

                    bodytable2.Rows.Insert(0, bodytable.Rows[0]);
                    for (int j = 1; j <= countRows - 1; j++)
                    {
                        bodytable2.Rows.Add(bodytable.Rows[j]);
                    }
                    for (int k = 1; k <= bodytable2.Rows.Count - 1; k++)
                    {
                        bodytable.DeleteRow(1);
                    }
                    bodytable2.WriteSelectedRows(0, -1, 10, 730, stamper.GetUnderContent(i));

                }
            }
            bytes = memoryStream.ToArray();
            System.IO.File.WriteAllBytes(_webHostingEnvironment.ContentRootPath + "/UploadedAttachments/RequestStatus/RequestStatusList.pdf", bytes);

            memoryStream.Close();
            document.Close();

        }
        public PdfPTable CreateAssetStatusTable(string lang)
        {
            var lstData = _requestStatusService.GetAllForReport();


            PdfPTable table = new PdfPTable(4);
            table.SetTotalWidth(new float[] { 120f, 150f, 150f, 150f });
            table.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
            table.HorizontalAlignment = Element.ALIGN_RIGHT;
            table.WidthPercentage = 100;
            table.PaddingTop = 200;
            table.HeaderRows = 1;
            table.SetWidths(new int[] { 80, 80, 80, 80 });


            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            string ARIALUNI_TFF = _webHostingEnvironment.ContentRootPath + "/Font/adobearabic.ttf";
            BaseFont bfArialUniCode = BaseFont.CreateFont(ARIALUNI_TFF, BaseFont.IDENTITY_H, true);
            iTextSharp.text.Font font = new iTextSharp.text.Font(bfArialUniCode, 12);
            if (lang == "ar")
            {
                string[] col = { "اللون", "الأيقون", "الاسم بالعربي", "الاسم" };
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
                string[] col = { "Name", "NameAr", "Icon", "Color" };
                for (int i = 0; i <= col.Length - 1; i++)
                {
                    PdfPCell cell = new PdfPCell(new Phrase(col[i], font));
                    cell.BackgroundColor = new iTextSharp.text.BaseColor(153, 204, 255);
                    cell.PaddingBottom = 10;
                    table.AddCell(cell);
                }
            }
            int index = 0;
            foreach (var item in lstData.ListStatus)
                {
                    ++index;

                
                    if (item.Name != null)
                        table.AddCell(new PdfPCell(new Phrase(item.Name, font)) { PaddingBottom = 5 });
                    else
                        table.AddCell(new PdfPCell(new Phrase(" ")) { PaddingBottom = 5 });

                    if (item.NameAr != null)
                        table.AddCell(new PdfPCell(new Phrase(item.NameAr, font)) { PaddingBottom = 5 });
                    else
                        table.AddCell(new PdfPCell(new Phrase(" ")) { PaddingBottom = 5 });

                    if (item.Icon.ToString() != "")
                        table.AddCell(new PdfPCell(new Phrase(item.Icon, font)) { PaddingBottom = 5 });
                    else
                        table.AddCell(new PdfPCell(new Phrase(" ")) { PaddingBottom = 5 });


                    if (item.Color != null)
                        table.AddCell(new PdfPCell(new Phrase(item.Color, font)) { PaddingBottom = 5 });
                    else
                        table.AddCell(new PdfPCell(new Phrase(" ")) { PaddingBottom = 5 });
                }
            
            

            return table;
        }
        [HttpGet]
        [Route("DownloadRequestStatusPDF/{fileName}")]
        public HttpResponseMessage DownloadAssetStatusPDF(string fileName)
        {
            var file = _webHostingEnvironment.ContentRootPath + "/UploadedAttachments/AssetStatus/" + fileName;
            HttpResponseMessage response = null;
            if (!System.IO.File.Exists(file))
            {

                var folder = Directory.CreateDirectory(_webHostingEnvironment.ContentRootPath + "/UploadedAttachments/AssetStatus/");
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
