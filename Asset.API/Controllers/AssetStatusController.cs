using Asset.API.Helpers;
using Asset.Domain.Services;
using Asset.Models;
using Asset.ViewModels.AssetStatusVM;
using Asset.ViewModels.PagingParameter;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection.Metadata;
using System.Net;
using Microsoft.AspNetCore.Hosting;
using Document = iTextSharp.text.Document;

namespace Asset.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssetStatusController : ControllerBase
    {

        private IAssetStatusService _assetStatusService;
        private IPagingService _pagingService;
        IWebHostEnvironment _webHostingEnvironment;
        private readonly ISettingService _settingService;


        string strInsitute, strInsituteAr, strLogo = "";

        public AssetStatusController(IAssetStatusService assetStatusService, IWebHostEnvironment webHostingEnvironment, IPagingService pagingService, ISettingService settingService)
        {
            _assetStatusService = assetStatusService;
            _pagingService = pagingService;
            _webHostingEnvironment = webHostingEnvironment;
            _settingService = settingService;
        }


        [HttpGet]
        [Route("ListAssetStatus")]
        public IEnumerable<IndexAssetStatusVM.GetData> GetAll()
        {
            return _assetStatusService.GetAll();
        }


        [HttpPut]
        [Route("GetAssetStatusWithPaging")]
        public IEnumerable<IndexAssetStatusVM.GetData> GetAssetStatusWithPaging(PagingParameter pageInfo)
        {
            var HospitalAssets = _assetStatusService.GetAll().ToList();
            return _pagingService.GetAll<IndexAssetStatusVM.GetData>(pageInfo, HospitalAssets);
        }


        [HttpGet]
        [Route("GetAssetStatusCount")]
        public int GetAssetStatusCount()
        {
            return _assetStatusService.GetAll().ToList().Count;// _AssetDetailService.GetAll().ToList().Count();
        }


        [HttpPost]
        [Route("SortAssetStatuses/{pagenumber}/{pagesize}")]
        public IEnumerable<IndexAssetStatusVM.GetData> SortAssetStatuses(int pagenumber, int pagesize, SortAssetStatusVM sortObj)
        {
            PagingParameter pageInfo = new PagingParameter();
            pageInfo.PageNumber = pagenumber;
            pageInfo.PageSize = pagesize;
            var list = _assetStatusService.SortAssetStatuses(sortObj);
            return _pagingService.GetAll<IndexAssetStatusVM.GetData>(pageInfo, list.ToList());
        }


 


        [HttpPost]
        [Route("GetHospitalAssetStatus/{statusId}/{userId}/{hospitalId}")]
        public IndexAssetStatusVM GetHospitalAssetStatus(int statusId, string userId, int hospitalId)
        {
            return _assetStatusService.GetHospitalAssetStatus(statusId, userId, hospitalId);
        }


        [HttpGet]
        [Route("GetById/{id}")]
        public ActionResult<EditAssetStatusVM> GetById(int id)
        {
            return _assetStatusService.GetById(id);
        }
        [HttpPut]
        [Route("UpdateAssetStatus")]
        public IActionResult Update(EditAssetStatusVM AssetStatusVM)
        {
            try
            {
                int id = AssetStatusVM.Id;
                if (AssetStatusVM.Code.Length > 5)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "codelen", Message = "code length exceed 5 characters", MessageAr = "هذا الكود لا يتعدى  خمس أرقام " });

                }
                var lstCode = _assetStatusService.GetAll().ToList().Where(a => a.Code == AssetStatusVM.Code && a.Id != id).ToList();
                if (lstCode.Count > 0)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "code", Message = "AssetStatus code already exist", MessageAr = "هذا الكود مسجل سابقاً" });
                }
                var lstNames = _assetStatusService.GetAll().ToList().Where(a => a.Name == AssetStatusVM.Name && a.Id != id).ToList();
                if (lstNames.Count > 0)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "name", Message = "AssetStatus name already exist", MessageAr = "هذا الاسم مسجل سابقاً" });
                }
                var lstArNames = _assetStatusService.GetAll().ToList().Where(a => a.NameAr == AssetStatusVM.NameAr && a.Id != id).ToList();
                if (lstArNames.Count > 0)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "nameAr", Message = "AssetStatus arabic name already exist", MessageAr = "هذا الاسم مسجل سابقاً" });
                }
                else
                {
                    int updatedRow = _assetStatusService.Update(AssetStatusVM);
                }
            }
            catch (DbUpdateConcurrencyException ex)
            {
                string msg = ex.Message;
                return BadRequest("Error in update");
            }

            return Ok();
        }


        [HttpPost]
        [Route("AddAssetStatus")]
        public ActionResult<AssetStatu> Add(CreateAssetStatusVM AssetStatusVM)
        {
            if (AssetStatusVM.Code.Length > 5)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "codelen", Message = "code length exceed 5 characters", MessageAr = "هذا الكود لا يتعدى  خمس أرقام " });

            }

            var lstCode = _assetStatusService.GetAll().ToList().Where(a => a.Code == AssetStatusVM.Code).ToList();
            if (lstCode.Count > 0)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "code", Message = "AssetStatus code already exist", MessageAr = "هذا الكود مسجل سابقاً" });
            }
            var lstNames = _assetStatusService.GetAll().ToList().Where(a => a.Name == AssetStatusVM.Name).ToList();
            if (lstNames.Count > 0)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "name", Message = "AssetStatus name already exist", MessageAr = "هذا الاسم مسجل سابقاً" });
            }
            var lstArNames = _assetStatusService.GetAll().ToList().Where(a => a.NameAr == AssetStatusVM.NameAr).ToList();
            if (lstArNames.Count > 0)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "name", Message = "AssetStatus arabic name already exist", MessageAr = "هذا الاسم مسجل سابقاً" });
            }
            else
            {
                var savedId = _assetStatusService.Add(AssetStatusVM);
                return CreatedAtAction("GetById", new { id = savedId }, AssetStatusVM);
            }
        }

        [HttpDelete]
        [Route("DeleteAssetStatus/{id}")]
        public ActionResult<AssetStatu> Delete(int id)
        {
            try
            {

                int deletedRow = _assetStatusService.Delete(id);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                string msg = ex.Message;
                return BadRequest("Error in delete");
            }
            return Ok();
        }








        [HttpPost]
        [Route("CreateAssetStatusPDF/{lang}")]
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
            System.IO.File.WriteAllBytes(_webHostingEnvironment.ContentRootPath + "/UploadedAttachments/AssetStatus/AssetStatusList.pdf", bytes);
            memoryStream = new MemoryStream();
            PdfReader reader = new PdfReader(bytes);
            using (PdfStamper stamper = new PdfStamper(reader, memoryStream))
            {
                int pages = reader.NumberOfPages;
                //Footer
                for (int i = 1; i <= pages; i++)
                {
                    ColumnText.ShowTextAligned(stamper.GetUnderContent(i), Element.ALIGN_LEFT, new Phrase(ArabicNumeralHelper.toArabicNumber(pages.ToString()) + "/" + ArabicNumeralHelper.toArabicNumber(i.ToString()), font), 500f, 5f, 0);
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
                    if (lang == "en")
                        titleTable.AddCell(new PdfPCell(new Phrase("Asset Statuses", titlefont)) { PaddingBottom = 5, Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_CENTER });
                    else
                        titleTable.AddCell(new PdfPCell(new Phrase("حالات الأصل", titlefont)) { PaddingBottom = 5, Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_CENTER });
                    titleTable.WriteSelectedRows(0, -1, 5, 770, stamper.GetOverContent(i));
                }

                for (int i = 1; i <= pages; i++)
                {
                    PdfPTable bodytable2 = new PdfPTable(3);
                    bodytable2.SetTotalWidth(new float[] { 200f, 200f, 175f });
                    bodytable2.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
                    bodytable2.HorizontalAlignment = Element.ALIGN_RIGHT;
                    bodytable2.WidthPercentage = 100;
                    bodytable2.PaddingTop = 200;
                    bodytable2.HeaderRows = 1;
                    bodytable2.SetWidths(new int[] { 100, 100, 100 });

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
            System.IO.File.WriteAllBytes(_webHostingEnvironment.ContentRootPath + "/UploadedAttachments/AssetStatus/AssetStatusList.pdf", bytes);

            memoryStream.Close();
            document.Close();

        }
        public PdfPTable CreateAssetStatusTable(string lang)
        {
            var lstData = _assetStatusService.GetAll().ToList();


            PdfPTable table = new PdfPTable(3);
            table.SetTotalWidth(new float[] { 200f, 200f, 175f });
            table.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
            table.HorizontalAlignment = Element.ALIGN_RIGHT;
            table.WidthPercentage = 100;
            table.PaddingTop = 200;
            table.HeaderRows = 1;
            table.SetWidths(new int[] { 100, 100, 100 });


            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            string ARIALUNI_TFF = _webHostingEnvironment.ContentRootPath + "/Font/adobearabic.ttf";
            BaseFont bfArialUniCode = BaseFont.CreateFont(ARIALUNI_TFF, BaseFont.IDENTITY_H, true);
            iTextSharp.text.Font font = new iTextSharp.text.Font(bfArialUniCode, 12);
            if (lang == "ar")
            {
                string[] col = {  "الاسم بالعربي", "الاسم", "الكود" };
                for (int i = col.Length - 1; i >= 0; i--)
                {
                    PdfPCell cell = new PdfPCell(new Phrase(col[i], font));
                    cell.BackgroundColor = new iTextSharp.text.BaseColor(153, 204, 255);
                    cell.PaddingBottom = 10;
                    table.AddCell(cell);
                }
                int index = 0;
                foreach (var item in lstData)
                {
                    ++index;
                 
                    if (item.Code.ToString() != "")
                        table.AddCell(new PdfPCell(new Phrase(ArabicNumeralHelper.toArabicNumber(item.Code), font)) { PaddingBottom = 5 });
                    else
                        table.AddCell(new PdfPCell(new Phrase(" ")) { PaddingBottom = 5 });

                    if (item.Name != null)
                        table.AddCell(new PdfPCell(new Phrase(item.Name, font)) { PaddingBottom = 5 });
                    else
                        table.AddCell(new PdfPCell(new Phrase(" ")) { PaddingBottom = 5 });

                    if (item.NameAr != null)
                        table.AddCell(new PdfPCell(new Phrase(item.NameAr, font)) { PaddingBottom = 5 });
                    else
                        table.AddCell(new PdfPCell(new Phrase(" ")) { PaddingBottom = 5 });
                }
            }
            else
            {
                string[] col = { "Code", "Name", "NameAr"};
                for (int i = 0; i <= col.Length - 1; i++)
                {
                    PdfPCell cell = new PdfPCell(new Phrase(col[i], font));
                    cell.BackgroundColor = new iTextSharp.text.BaseColor(153, 204, 255);
                    cell.PaddingBottom = 10;
                    table.AddCell(cell);
                }
                int index = 0;
                foreach (var item in lstData)
                {
                    ++index;
                   // table.AddCell(new PdfPCell(new Phrase(ArabicNumeralHelper.toArabicNumber(index.ToString()), font)) { PaddingBottom = 5 });
                    if (item.Code.ToString() != "")
                        table.AddCell(new PdfPCell(new Phrase(ArabicNumeralHelper.toArabicNumber(item.Code), font)) { PaddingBottom = 5 });
                    else
                        table.AddCell(new PdfPCell(new Phrase(" ")) { PaddingBottom = 5 });

                    if (item.Name != null)
                        table.AddCell(new PdfPCell(new Phrase(item.Name, font)) { PaddingBottom = 5 });
                    else
                        table.AddCell(new PdfPCell(new Phrase(" ")) { PaddingBottom = 5 });

                    if (item.NameAr != null)
                        table.AddCell(new PdfPCell(new Phrase(item.NameAr, font)) { PaddingBottom = 5 });
                    else
                        table.AddCell(new PdfPCell(new Phrase(" ")) { PaddingBottom = 5 });
                }
            }

            return table;
        }
        [HttpGet]
        [Route("DownloadAssetStatusPDF/{fileName}")]
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
