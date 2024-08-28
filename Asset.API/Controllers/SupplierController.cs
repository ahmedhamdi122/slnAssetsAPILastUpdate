using Asset.API.Helpers;
using Asset.Domain.Services;
using Asset.Models;
using Asset.ViewModels.PagingParameter;
using Asset.ViewModels.SupplierVM;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Syncfusion.DocIO;
using Syncfusion.DocIO.DLS;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Asset.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SupplierController : ControllerBase
    {
        private IAssetDetailService _assetDetailService;
        private IMasterContractService _masterContractService;
        private ISupplierService _SupplierService;
        private IPagingService _pagingService;
        IWebHostEnvironment _webHostingEnvironment;
        private readonly ISettingService _settingService;
        string strInsitute, strInsituteAr, strLogo = "";
        int i = 1;

        public SupplierController(ISupplierService SupplierService, IAssetDetailService assetDetailService, ISettingService settingService,
            IMasterContractService masterContractService, IPagingService pagingService, IWebHostEnvironment webHostingEnvironment)
        {
            _masterContractService = masterContractService;
            _assetDetailService = assetDetailService;
            _SupplierService = SupplierService;
            _pagingService = pagingService;
            _webHostingEnvironment = webHostingEnvironment;
            _settingService = settingService;
        }


        [HttpGet]
        [Route("ListSuppliers")]
        public IEnumerable<IndexSupplierVM.GetData> GetAll()
        {
            return _SupplierService.GetAll();
        }


        [HttpPost]
        [Route("GetAllSuppliersWithPaging/{pagenumber}/{pagesize}")]
        public IndexSupplierVM GetAllSuppliersWithPaging(int pagenumber, int pagesize)
        {
            return _SupplierService.GetAllSuppliersWithPaging(pagenumber, pagesize);
        }

        [HttpGet]
        [Route("GetTop10Suppliers/{hospitalId}")]
        public IEnumerable<IndexSupplierVM.GetData> GetTop10Suppliers(int hospitalId)
        {
            return _SupplierService.GetTop10Suppliers(hospitalId);
        }
        [HttpGet]
        [Route("GetTop10SuppliersCount/{hospitalId}")]
        public int GetTop10SuppliersCount(int hospitalId)
        {
            return _SupplierService.GetTop10Suppliers(hospitalId).ToList().Count;
        }


        [HttpPost]
        [Route("SortSuppliers/{pagenumber}/{pagesize}")]
        public IEnumerable<IndexSupplierVM.GetData> SortAssets(int pagenumber, int pagesize, SortSupplierVM sortObj)
        {
            PagingParameter pageInfo = new PagingParameter();
            pageInfo.PageNumber = pagenumber;
            pageInfo.PageSize = pagesize;
            var list = _SupplierService.SortSuppliers(sortObj);
            return _pagingService.GetAll<IndexSupplierVM.GetData>(pageInfo, list.ToList());
        }

        [HttpPut]
        [Route("GetSuppliersWithPaging")]
        public IEnumerable<IndexSupplierVM.GetData> GetAll(PagingParameter pageInfo)
        {
            var lstSuppliers = _SupplierService.GetAll().ToList();
            return _pagingService.GetAll<IndexSupplierVM.GetData>(pageInfo, lstSuppliers);
        }


        [HttpPost]
        [Route("FindSupplier/{pagenumber}/{pagesize}")]
        public IndexSupplierVM FindSupplier(string strText, int pageNumber, int pageSize)
        {
            var lstSuppliers = _SupplierService.FindSupplier(strText, pageNumber, pageSize);
            return lstSuppliers;
        }



        [HttpGet]
        [Route("FindSupplierByText")]
        public IEnumerable<IndexSupplierVM.GetData> FindSupplierByText(string strText)
        {
            var lstSuppliers = _SupplierService.FindSupplierByText(strText);
            return lstSuppliers;
        }



        [HttpGet]
        [Route("getcount")]
        public int count()
        {
            return _SupplierService.GetAll().ToList().Count;
        }



        [HttpGet]
        [Route("CountSuppliers")]
        public int CountSuppliers()
        {
            return _SupplierService.CountSuppliers();
        }




        [HttpGet]
        [Route("GetById/{id}")]
        public ActionResult<EditSupplierVM> GetById(int id)
        {
            return _SupplierService.GetById(id);
        }



        [HttpPut]
        [Route("UpdateSupplier")]
        public IActionResult Update(EditSupplierVM SupplierVM)
        {
            try
            {
                int id = SupplierVM.Id;
                var lstCitiesCode = _SupplierService.GetAllSuppliers().ToList().Where(a => a.Code == SupplierVM.Code && a.Id != id).ToList();
                if (lstCitiesCode.Count > 0)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "code", Message = "Supplier code already exist", MessageAr = "هذا الكود مسجل سابقاً" });
                }
                var lstCitiesNames = _SupplierService.GetAllSuppliers().ToList().Where(a => a.Name == SupplierVM.Name && a.Id != id).ToList();
                if (lstCitiesNames.Count > 0)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "name", Message = "Supplier name already exist", MessageAr = "هذا الاسم مسجل سابقاً" });
                }
                var lstCitiesArNames = _SupplierService.GetAllSuppliers().ToList().Where(a => a.NameAr == SupplierVM.NameAr && a.Id != id).ToList();
                if (lstCitiesArNames.Count > 0)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "name", Message = "Supplier arabic name already exist", MessageAr = "هذا الاسم مسجل سابقاً" });
                }

                else
                {
                    int updatedRow = _SupplierService.Update(SupplierVM);
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
        [Route("AddSupplier")]
        public ActionResult Add(CreateSupplierVM SupplierVM)
        {
            if (SupplierVM.Code.Length > 5)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "codelen", Message = "Supplier code must not exceed 5 characters", MessageAr = "هذا الكود لا يتعدي 5 حروف وأرقام" });

            }
            var lstOrgCode = _SupplierService.GetAllSuppliers().ToList().Where(a => a.Code == SupplierVM.Code).ToList();
            if (lstOrgCode.Count > 0)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "code", Message = "Supplier code already exist", MessageAr = "هذا الكود مسجل سابقاً" });
            }
            var lstOrgNames = _SupplierService.GetAllSuppliers().ToList().Where(a => a.Name == SupplierVM.Name).ToList();
            if (lstOrgNames.Count > 0)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "name", Message = "Supplier name already exist", MessageAr = "هذا الاسم مسجل سابقاً" });
            }
            var lstCitiesArNames = _SupplierService.GetAllSuppliers().ToList().Where(a => a.NameAr == SupplierVM.NameAr).ToList();
            if (lstCitiesArNames.Count > 0)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "nameAr", Message = "Supplier arabic name already exist", MessageAr = "هذا الاسم مسجل سابقاً" });
            }
            else
            {
                var savedId = _SupplierService.Add(SupplierVM);
                return Ok(savedId);// CreatedAtAction("GetById", new { id = savedId }, SupplierVM);
            }
        }


        [HttpPost]
        [Route("CreateSupplierAttachment")]
        public int CreateRequestAttachments(SupplierAttachment attachObj)
        {
            return _SupplierService.CreateSupplierAttachment(attachObj);
        }


        [HttpGet]
        [Route("GetSupplierAttachmentsBySupplierId/{supplierId}")]
        public List<SupplierAttachment> GetSupplierAttachmentsBySupplierId(int supplierId)
        {
            return _SupplierService.GetSupplierAttachmentsBySupplierId(supplierId);
        }




        [HttpGet]
        [Route("GetLastDocumentForSupplierId/{supplierId}")]
        public SupplierAttachment GetLastDocumentForSupplierId(int supplierId)
        {
            return _SupplierService.GetLastDocumentForSupplierId(supplierId);
        }



        [HttpPost]
        [Route("UploadSupplierFile")]
        public ActionResult UploadSupplierFile(IFormFile file)
        {
            var folderPath = _webHostingEnvironment.ContentRootPath + "/UploadedAttachments/SupplierAttachments";
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


        [HttpDelete]
        [Route("DeleteSupplier/{id}")]
        public ActionResult<Supplier> Delete(int id)
        {
            try
            {
                var supplierObj = _SupplierService.GetById(id);
                var lstHospitalAssets = _assetDetailService.GetAll().Where(a => a.SupplierId == supplierObj.Id).ToList();
                if (lstHospitalAssets.Count > 0)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "hostassets", Message = "Hospital Assets has this supplier", MessageAr = "أصول المستشفى بها منتجات من هذا المورد" });
                }
                var lstMasterContracts = _masterContractService.GetAll().Where(a => a.SupplierId == supplierObj.Id).ToList();
                if (lstMasterContracts.Count > 0)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "contract", Message = "Contract has this supplier", MessageAr = "العقد به هذا المورد" });
                }
                else
                {
                    int deletedRow = _SupplierService.Delete(id);
                }
            }
            catch (DbUpdateConcurrencyException ex)
            {
                string msg = ex.Message;
                return BadRequest("Error in delete");
            }

            return Ok();
        }

        [HttpDelete]
        [Route("DeleteSupplierAttachment/{id}")]
        public ActionResult<SupplierAttachment> DeleteSupplierAttachment(int id)
        {
            try
            {
                int deletedRow = _SupplierService.DeleteSupplierAttachment(id);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                string msg = ex.Message;
                return BadRequest("Error in delete");
            }

            return Ok();
        }

        [HttpGet("GenerateSupplierCode")]
        public GenerateSupplierCodeVM GenerateSupplierCode()
        {
            return _SupplierService.GenerateSupplierCode();
        }




        [HttpPost]
        [Route("CreateSupplierPDF/{lang}")]
        public void CreateSupplierPDF(string lang)
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
            Document document = new Document(iTextSharp.text.PageSize.A4.Rotate(), 10f, 10f, 10f, 10f);
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

            PdfPTable bodytable = CreateSupplierTable(lang);
            int countnewpages = bodytable.Rows.Count / 20;
            for (int i = 1; i <= countnewpages; i++)
            {
                document.NewPage();
                writer.PageEmpty = false;
            }

            document.Close();
            byte[] bytes = memoryStream.ToArray();
            System.IO.File.WriteAllBytes(_webHostingEnvironment.ContentRootPath + "/UploadedAttachments/SupplierPDF/SupplierList.pdf", bytes);
            memoryStream = new MemoryStream();
            PdfReader reader = new PdfReader(bytes);
            using (PdfStamper stamper = new PdfStamper(reader, memoryStream))
            {
                int pages = reader.NumberOfPages;
                //Footer
                for (int i = 1; i <= pages; i++)
                {
                    ColumnText.ShowTextAligned(stamper.GetUnderContent(i), Element.ALIGN_LEFT, new Phrase(ArabicNumeralHelper.toArabicNumber(pages.ToString()) + "/" + ArabicNumeralHelper.toArabicNumber(i.ToString()), font), 400f, 15f, 0);
                }
                //Header
                for (int i = 1; i <= pages; i++)
                {
                    string imageURL = _webHostingEnvironment.ContentRootPath + "/Images/" + strLogo;
                    iTextSharp.text.Image jpg = iTextSharp.text.Image.GetInstance(imageURL);
                    jpg.ScaleAbsolute(70f, 50f);
                    PdfPTable headertable = new PdfPTable(2);
                    headertable.SetTotalWidth(new float[] { 350f, 50f });
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


                    headertable.WriteSelectedRows(0, -1, 420, 580, stamper.GetOverContent(i));

                    System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                    string adobearabicheaderTitle = _webHostingEnvironment.ContentRootPath + "/Font/adobearabic.ttf";
                    BaseFont bfUniCodeheaderTitle = BaseFont.CreateFont(adobearabicheaderTitle, BaseFont.IDENTITY_H, true);
                    iTextSharp.text.Font titlefont = new iTextSharp.text.Font(bfUniCodeheaderTitle, 16);
                    titlefont.SetStyle("bold");


                    PdfPTable titleTable = new PdfPTable(1);
                    titleTable.SetTotalWidth(new float[] { 800f });
                    titleTable.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
                    titleTable.WidthPercentage = 100;
                    if (lang == "ar")
                        titleTable.AddCell(new PdfPCell(new Phrase("الموردين", titlefont)) { PaddingBottom = 5, Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_CENTER });
                    else
                        titleTable.AddCell(new PdfPCell(new Phrase("Suppliers", titlefont)) { PaddingBottom = 5, Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_CENTER });


                    titleTable.WriteSelectedRows(0, -1, 5, 520, stamper.GetOverContent(i));
                }

                for (int i = 1; i <= pages; i++)
                {
                    PdfPTable bodytable2 = new PdfPTable(11);
                    bodytable2.SetTotalWidth(new float[] { 90f, 60f, 80f, 80f, 80f, 80f, 80f, 80f, 80f, 80f, 30f });
                    bodytable2.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
                    bodytable2.HorizontalAlignment = Element.ALIGN_RIGHT;
                    bodytable2.WidthPercentage = 100;
                    bodytable2.PaddingTop = 200;
                    bodytable2.HeaderRows = 1;
                    bodytable2.SetWidths(new int[] { 90, 60, 80, 80, 80, 80, 80, 80, 80, 80, 30 });

                    int countRows = bodytable.Rows.Count;
                    if (countRows > 20)
                    {
                        countRows = 20;
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
                    bodytable2.WriteSelectedRows(0, -1, 10, 480, stamper.GetUnderContent(i));

                }
            }
            bytes = memoryStream.ToArray();
            System.IO.File.WriteAllBytes(_webHostingEnvironment.ContentRootPath + "/UploadedAttachments/SupplierPDF/SupplierList.pdf", bytes);

            memoryStream.Close();
            document.Close();

        }
        public PdfPTable CreateSupplierTable(string lang)
        {
            var lstData = _SupplierService.GetAllSuppliers().ToList();


            PdfPTable table = new PdfPTable(11);
            table.SetTotalWidth(new float[] { 90f, 60f, 80f, 80f, 80f, 80f, 80f, 80f, 80f, 80f, 30f });
            table.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
            table.HorizontalAlignment = Element.ALIGN_RIGHT;
            table.WidthPercentage = 100;
            table.PaddingTop = 200;
            table.HeaderRows = 1;
            table.SetWidths(new int[] { 90, 60, 80, 80, 80, 80, 80, 80, 80, 80, 30 });


            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            string ARIALUNI_TFF = _webHostingEnvironment.ContentRootPath + "/Font/adobearabic.ttf";
            BaseFont bfArialUniCode = BaseFont.CreateFont(ARIALUNI_TFF, BaseFont.IDENTITY_H, true);
            iTextSharp.text.Font font = new iTextSharp.text.Font(bfArialUniCode, 12);
            if (lang == "ar")
            {
                string[] col = { "ملاحظات", "فاكس", "الاتصال", "العنوان بالعربي", "العنوان", "البريد الالكتروني", "الموقع الالكتروني", "محمول", "الاسم بالعربي", "الاسم", "الكود" };
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

                    if (item.Code != null)
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


                    if (item.Mobile != null)
                        table.AddCell(new PdfPCell(new Phrase(item.Mobile, font)) { PaddingBottom = 5 });
                    else
                        table.AddCell(new PdfPCell(new Phrase(" ")) { PaddingBottom = 5 });

                    if (item.Website != null)
                        table.AddCell(new PdfPCell(new Phrase(item.Website, font)) { PaddingBottom = 5 });
                    else
                        table.AddCell(new PdfPCell(new Phrase(" ")) { PaddingBottom = 5 });


                    if (item.EMail != null)
                        table.AddCell(new PdfPCell(new Phrase(item.EMail, font)) { PaddingBottom = 5 });
                    else
                        table.AddCell(new PdfPCell(new Phrase(" ")) { PaddingBottom = 5 });



                    if (item.Address != null)
                        table.AddCell(new PdfPCell(new Phrase(item.Address, font)) { PaddingBottom = 5 });
                    else
                        table.AddCell(new PdfPCell(new Phrase(" ")) { PaddingBottom = 5 });

                    if (item.AddressAr != null)
                        table.AddCell(new PdfPCell(new Phrase(item.AddressAr, font)) { PaddingBottom = 5 });
                    else
                        table.AddCell(new PdfPCell(new Phrase(" ")) { PaddingBottom = 5 });






                    if (item.ContactPerson != null)
                        table.AddCell(new PdfPCell(new Phrase(item.ContactPerson, font)) { PaddingBottom = 5 });
                    else
                        table.AddCell(new PdfPCell(new Phrase(" ")) { PaddingBottom = 5 });

                    if (item.Fax != null)
                        table.AddCell(new PdfPCell(new Phrase(item.Fax, font)) { PaddingBottom = 5 });
                    else
                        table.AddCell(new PdfPCell(new Phrase(" ")) { PaddingBottom = 5 });

                    if (item.Notes != null)
                        table.AddCell(new PdfPCell(new Phrase(item.Notes, font)) { PaddingBottom = 5 });
                    else
                        table.AddCell(new PdfPCell(new Phrase(" ")) { PaddingBottom = 5 });



                }
            }
            else
            {


                string[] col = { "Code", "Name", "NameAr", "Mobile", "WebSite", "Email", "Address", "AddressAr", "ContactPerson", "Fax", "Notes" };
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
                    if (item.Code != null)
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




                    if (item.Mobile != null)
                        table.AddCell(new PdfPCell(new Phrase(item.Mobile, font)) { PaddingBottom = 5 });
                    else
                        table.AddCell(new PdfPCell(new Phrase(" ")) { PaddingBottom = 5 });

                    if (item.Website != null)
                        table.AddCell(new PdfPCell(new Phrase(item.Website, font)) { PaddingBottom = 5 });
                    else
                        table.AddCell(new PdfPCell(new Phrase(" ")) { PaddingBottom = 5 });


                    if (item.EMail != null)
                        table.AddCell(new PdfPCell(new Phrase(item.EMail, font)) { PaddingBottom = 5 });
                    else
                        table.AddCell(new PdfPCell(new Phrase(" ")) { PaddingBottom = 5 });



                    if (item.Address != null)
                        table.AddCell(new PdfPCell(new Phrase(item.Address, font)) { PaddingBottom = 5 });
                    else
                        table.AddCell(new PdfPCell(new Phrase(" ")) { PaddingBottom = 5 });

                    if (item.AddressAr != null)
                        table.AddCell(new PdfPCell(new Phrase(item.AddressAr, font)) { PaddingBottom = 5 });
                    else
                        table.AddCell(new PdfPCell(new Phrase(" ")) { PaddingBottom = 5 });


                    if (item.ContactPerson != null)
                        table.AddCell(new PdfPCell(new Phrase(item.ContactPerson, font)) { PaddingBottom = 5 });
                    else
                        table.AddCell(new PdfPCell(new Phrase(" ")) { PaddingBottom = 5 });

                    if (item.Fax != null)
                        table.AddCell(new PdfPCell(new Phrase(item.Fax, font)) { PaddingBottom = 5 });
                    else
                        table.AddCell(new PdfPCell(new Phrase(" ")) { PaddingBottom = 5 });

                    if (item.Notes != null)
                        table.AddCell(new PdfPCell(new Phrase(item.Notes, font)) { PaddingBottom = 5 });
                    else
                        table.AddCell(new PdfPCell(new Phrase(" ")) { PaddingBottom = 5 });



                }
            }

            return table;
        }
        [HttpGet]
        [Route("DownloadSupplierPDF/{fileName}")]
        public HttpResponseMessage DownloadSupplierPDF(string fileName)
        {
            var file = _webHostingEnvironment.ContentRootPath + "/UploadedAttachments/SupplierPDF/" + fileName;
            HttpResponseMessage response = null;
            if (!System.IO.File.Exists(file))
            {

                var folder = Directory.CreateDirectory(_webHostingEnvironment.ContentRootPath + "/UploadedAttachments/SupplierPDF/");
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




        [HttpPost]
        [Route("GenerateWordForAllSupplier/{lang}")]
        public ActionResult GenerateWordForAllSupplier(string lang)
        {
            string strTemplateFile = "";
            string strExportFile = "";
            using (WordDocument document = new WordDocument())
            {
                //Opens the Word template document
                if (lang == "ar")
                    strTemplateFile = _webHostingEnvironment.ContentRootPath + @"\UploadedAttachments\SupplierTemplates\ArabicSupplierTemplate.dotx";
                else
                    strTemplateFile = _webHostingEnvironment.ContentRootPath + @"\UploadedAttachments\SupplierTemplates\EnglishSupplierTemplate.dotx";

                Stream docStream = System.IO.File.OpenRead(strTemplateFile);
                document.Open(docStream, FormatType.Docx);
                docStream.Dispose();


                var allSuppliers = _SupplierService.GetAll().ToList();
                MailMergeDataTable dataTable = new MailMergeDataTable("Asset_QrCode", allSuppliers);
                document.MailMerge.MergeField += new MergeFieldEventHandler(MergeField_InsertPageBreak);
                // document.MailMerge.MergeImageField += new MergeImageFieldEventHandler(InsertQRBarcode);
                document.MailMerge.MergeField += new MergeFieldEventHandler(MergeField_Event);
                document.MailMerge.RemoveEmptyGroup = true;
                document.MailMerge.ExecuteGroup(dataTable);


                //Saves the file in the given path
                if (lang == "ar")
                    strExportFile = _webHostingEnvironment.ContentRootPath + @"\UploadedAttachments\SupplierTemplates\ArabicSupplierCards.docx";
                else
                    strExportFile = _webHostingEnvironment.ContentRootPath + @"\UploadedAttachments\SupplierTemplates\EnglishSupplierCards.docx";


                docStream = System.IO.File.Create(strExportFile);
                document.Save(docStream, FormatType.Docx);
                docStream.Dispose();
                document.Close();



            }
            return Ok();
        }
        [HttpPost]
        [Route("GenerateWordForSelectedSupplier/{lang}")]
        public ActionResult GenerateWordForSelectedSupplier(List<Supplier> selectedSuppliers, string lang)
        {
            string strTemplateFile = "";
            string strExportFile = "";
            using (WordDocument document = new WordDocument())
            {
                //Opens the Word template document
                if (lang == "ar")
                    strTemplateFile = _webHostingEnvironment.ContentRootPath + @"\UploadedAttachments\SupplierTemplates\ArabicSupplierTemplate.dotx";
                else
                    strTemplateFile = _webHostingEnvironment.ContentRootPath + @"\UploadedAttachments\SupplierTemplates\EnglishSupplierTemplate.dotx";


                Stream docStream = System.IO.File.OpenRead(strTemplateFile);
                document.Open(docStream, FormatType.Docx);
                docStream.Dispose();


                var allSelectedSuppliers = ListSuppliers(selectedSuppliers).ToList();
                MailMergeDataTable dataTable = new MailMergeDataTable("Asset_QrCode", allSelectedSuppliers);
                document.MailMerge.MergeField += new MergeFieldEventHandler(MergeField_InsertPageBreak);
                // document.MailMerge.MergeImageField += new MergeImageFieldEventHandler(InsertQRBarcode);
                document.MailMerge.MergeField += new MergeFieldEventHandler(MergeField_Event);
                document.MailMerge.RemoveEmptyGroup = true;
                document.MailMerge.ExecuteGroup(dataTable);


                //Saves the file in the given path
                if (lang == "ar")
                    strExportFile = _webHostingEnvironment.ContentRootPath + @"\UploadedAttachments\SupplierTemplates\ArabicSupplierCards.docx";
                else
                    strExportFile = _webHostingEnvironment.ContentRootPath + @"\UploadedAttachments\SupplierTemplates\EnglishSupplierCards.docx";

                docStream = System.IO.File.Create(strExportFile);
                document.Save(docStream, FormatType.Docx);
                docStream.Dispose();
                document.Close();



            }
            return Ok();
        }
        private static void MergeField_Event(object sender, MergeFieldEventArgs args)
        {
            string fieldValue = args.FieldValue.ToString();
            //When field value is Null or empty, then remove the field owner paragraph.
            if (string.IsNullOrEmpty(fieldValue))
            {
                //Get the merge field owner paragraph and remove it from its owner text body.
                WParagraph ownerParagraph = args.CurrentMergeField.OwnerParagraph;
                WTextBody ownerTextBody = ownerParagraph.OwnerTextBody;
                ownerTextBody.ChildEntities.Remove(ownerParagraph);
            }
        }
        private void MergeField_InsertPageBreak(object sender, MergeFieldEventArgs args)
        {
            if (args.FieldName == "Notes")
            {
                //Gets the owner paragraph 
                WParagraph paragraph = args.CurrentMergeField.OwnerParagraph;
                //Appends the page break 
                paragraph.AppendBreak(BreakType.PageBreak);
                i++;
            }

        }
        [HttpGet]
        [Route("DownloadSupplierCardPDF/{fileName}")]
        public HttpResponseMessage DownloadSupplierCardPDF(string fileName)
        {
            var file = _webHostingEnvironment.ContentRootPath + "/UploadedAttachments/SupplierTemplates/" + fileName;
            HttpResponseMessage response = null;
            if (!System.IO.File.Exists(file))
            {
                // return new HttpResponseMessage(HttpStatusCode.Gone);
                var folder = Directory.CreateDirectory(_webHostingEnvironment.ContentRootPath + "/UploadedAttachments/SupplierTemplates/");
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




        private List<Supplier> ListSuppliers(List<Supplier> selectedSuppliers)
        {
            return selectedSuppliers.ToList();
        }






        #region Create Supplier selected Items as PDF
        [HttpPost]
        [Route("CreateSelectedSupplierPDF")]
        public void CreateSupplierPDF(List<Supplier> suppliers)
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
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            string adobearabic = _webHostingEnvironment.ContentRootPath + "/Font/adobearabic.ttf";
            BaseFont bfUniCode = BaseFont.CreateFont(adobearabic, BaseFont.IDENTITY_H, true);
            iTextSharp.text.Font font = new iTextSharp.text.Font(bfUniCode, 14);

            Phrase ph = new Phrase(" ", font);
            document.Add(ph);

            PdfPTable bodytable = CreateSupplierTable(suppliers);
            int countnewpages = bodytable.Rows.Count / 12;
            for (int i = 1; i <= countnewpages; i++)
            {
                document.NewPage();
                writer.PageEmpty = false;
            }

            document.Close();
            byte[] bytes = memoryStream.ToArray();

            System.IO.File.WriteAllBytes(_webHostingEnvironment.ContentRootPath + "/UploadedAttachments/SupplierReport.pdf", bytes);
            memoryStream = new MemoryStream();
            PdfReader reader = new PdfReader(bytes);
            using (PdfStamper stamper = new PdfStamper(reader, memoryStream))
            {
                int pages = reader.NumberOfPages;
                //Footer
                for (int i = 1; i <= pages; i++)
                {
                    ColumnText.ShowTextAligned(stamper.GetUnderContent(i), Element.ALIGN_LEFT, new Phrase(ArabicNumeralHelper.toArabicNumber(pages.ToString()) + "/" + ArabicNumeralHelper.toArabicNumber(i.ToString()), font), 800f, 15f, 0);
                    ColumnText.ShowTextAligned(stamper.GetUnderContent(i), Element.ALIGN_RIGHT, new Phrase("تمت الطباعة بواسطة  " + suppliers[0].PrintedBy, font), 150f, 15f, 0, PdfWriter.RUN_DIRECTION_RTL, ColumnText.AR_LIG);
                }
                //Header
                for (int i = 1; i <= pages; i++)
                {
                    string imageURL = _webHostingEnvironment.ContentRootPath + "/Images/" + strLogo;
                    iTextSharp.text.Image jpg = iTextSharp.text.Image.GetInstance(imageURL);
                    jpg.ScaleAbsolute(70f, 50f);
                    PdfPTable headertable = new PdfPTable(2);
                    headertable.SetTotalWidth(new float[] { 350f, 50f });
                    headertable.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
                    headertable.WidthPercentage = 100;
                    PdfPCell cell = new PdfPCell(new PdfPCell(jpg));
                    cell.PaddingTop = 5;
                    cell.Border = Rectangle.NO_BORDER;
                    cell.PaddingRight = 15;
                    headertable.AddCell(cell);
                    if (suppliers[0].Lang == "ar")
                        headertable.AddCell(new PdfPCell(new Phrase(strInsituteAr + "\n" , font)) { Border = Rectangle.NO_BORDER, PaddingTop = 15 });
                    else
                        headertable.AddCell(new PdfPCell(new Phrase(strInsitute + "\n" , font)) { Border = Rectangle.NO_BORDER, PaddingTop = 10 });


                    headertable.WriteSelectedRows(0, -1, 420, 580, stamper.GetOverContent(i));

                    System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                    string adobearabicheaderTitle = _webHostingEnvironment.ContentRootPath + "/Font/adobearabic.ttf";
                    BaseFont bfUniCodeheaderTitle = BaseFont.CreateFont(adobearabicheaderTitle, BaseFont.IDENTITY_H, true);
                    iTextSharp.text.Font titlefont = new iTextSharp.text.Font(bfUniCodeheaderTitle, 16);
                    titlefont.SetStyle("bold");


                    PdfPTable titleTable = new PdfPTable(1);
                    titleTable.SetTotalWidth(new float[] { 800f });
                    titleTable.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
                    titleTable.WidthPercentage = 100;
                    titleTable.AddCell(new PdfPCell(new Phrase("الموردين", titlefont)) { PaddingBottom = 5, Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_CENTER });

                    titleTable.WriteSelectedRows(0, -1, 5, 550, stamper.GetOverContent(i));
                }

                for (int i = 1; i <= pages; i++)
                {
                    PdfPTable bodytable2 = new PdfPTable(11);
                    bodytable2.SetTotalWidth(new float[] { 90f, 80f, 80f, 80f, 80f, 80f, 80f, 80f, 80f, 80f, 15f });
                    bodytable2.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
                    bodytable2.HorizontalAlignment = Element.ALIGN_RIGHT;
                    bodytable2.WidthPercentage = 100;
                    bodytable2.PaddingTop = 200;
                    bodytable2.HeaderRows = 1;
                    bodytable2.SetWidths(new int[] { 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 7 });

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
                    bodytable2.WriteSelectedRows(0, -1, 10, 500, stamper.GetUnderContent(i));

                }
            }
            bytes = memoryStream.ToArray();
            System.IO.File.WriteAllBytes(_webHostingEnvironment.ContentRootPath + "/UploadedAttachments/SupplierReport.pdf", bytes);

            memoryStream.Close();
            document.Close();

        }
        public PdfPTable CreateSupplierTable(List<Supplier> suppliers)
        {
            var lstData = ListSuppliers(suppliers);
            PdfPTable table = new PdfPTable(11);
            table.SetTotalWidth(new float[] { 90f, 80f, 80f, 80f, 80f, 80f, 80f, 80f, 80f, 80f, 15f });
            table.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
            table.HorizontalAlignment = Element.ALIGN_RIGHT;
            table.WidthPercentage = 100;
            table.PaddingTop = 200;
            table.HeaderRows = 1;
            table.SetWidths(new int[] { 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 7 });


            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            string ARIALUNI_TFF = _webHostingEnvironment.ContentRootPath + "/Font/adobearabic.ttf";
            BaseFont bfArialUniCode = BaseFont.CreateFont(ARIALUNI_TFF, BaseFont.IDENTITY_H, true);
            iTextSharp.text.Font font = new iTextSharp.text.Font(bfArialUniCode, 12);
            if (suppliers[0].Lang == "ar")
            {
                string[] col = { "ملاحظات", "فاكس", "الشخص المسؤول", "العنوان بالعربي", "العنوان", "البريد الالكتروني", "الموقع", "المحمول", "الاسم بالعربي", "الاسم", "الكود" };
                for (int i = col.Length - 1; i >= 0; i--)
                {
                    PdfPCell cell = new PdfPCell(new Phrase(col[i], font));
                    cell.BackgroundColor = new iTextSharp.text.BaseColor(153, 204, 255);
                    cell.PaddingBottom = 10;
                    table.AddCell(cell);
                }
            }
            if (suppliers[0].Lang == "en")
            {
                string[] col = { "Code", "Name", "NameAr", "Mobile", "WebSite", "Email", "Address", "AddressAr", "Contact Person", "Fax", "Notes" };
                for (i = 0; i < col.Length - 1; i++)
                {
                    PdfPCell cell = new PdfPCell(new Phrase(col[i], font));
                    cell.BackgroundColor = new iTextSharp.text.BaseColor(153, 204, 255);
                    cell.PaddingBottom = 10;
                    table.AddCell(cell);
                }
            }
            int index = 0;
            foreach (var item in lstData)
            {
                ++index;
                //   table.AddCell(new PdfPCell(new Phrase(ArabicNumeralHelper.toArabicNumber(index.ToString()), font)) { PaddingBottom = 5 });
                if (item.Code != null)
                    table.AddCell(new PdfPCell(new Phrase(item.Code, font)) { PaddingBottom = 5 });
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


                if (item.Mobile != null)
                    table.AddCell(new PdfPCell(new Phrase(ArabicNumeralHelper.toArabicNumber(item.Mobile), font)) { PaddingBottom = 5 });
                else
                    table.AddCell(new PdfPCell(new Phrase(" ")) { PaddingBottom = 5 });


                if (item.Website != null)
                {
                    table.AddCell(new PdfPCell(new Phrase(item.Website, font)) { PaddingBottom = 5 });
                }
                else
                    table.AddCell(new PdfPCell(new Phrase(" ")) { PaddingBottom = 5 });



                if (item.EMail != null)
                    table.AddCell(new PdfPCell(new Phrase(item.EMail, font)) { PaddingBottom = 5 });
                else
                    table.AddCell(new PdfPCell(new Phrase(" ")) { PaddingBottom = 5 });

                if (item.Address != null)
                    table.AddCell(new PdfPCell(new Phrase(item.Address, font)) { PaddingBottom = 5 });
                else
                    table.AddCell(new PdfPCell(new Phrase(" ")) { PaddingBottom = 5 });

                if (item.AddressAr != null)
                    table.AddCell(new PdfPCell(new Phrase(item.AddressAr, font)) { PaddingBottom = 5 });
                else
                    table.AddCell(new PdfPCell(new Phrase(" ")) { PaddingBottom = 5 });

                if (item.ContactPerson != null)
                    table.AddCell(new PdfPCell(new Phrase(item.ContactPerson, font)) { PaddingBottom = 5 });
                else
                    table.AddCell(new PdfPCell(new Phrase(" ")) { PaddingBottom = 5 });

                if (item.Fax != null)
                    table.AddCell(new PdfPCell(new Phrase(item.Fax, font)) { PaddingBottom = 5 });
                else
                    table.AddCell(new PdfPCell(new Phrase(" ")) { PaddingBottom = 5 });


                if (item.Notes != null)
                    table.AddCell(new PdfPCell(new Phrase(item.Notes, font)) { PaddingBottom = 5 });
                else
                    table.AddCell(new PdfPCell(new Phrase(" ")) { PaddingBottom = 5 });
            }
            return table;
        }
        [HttpGet]
        [Route("DownloadSupplierCheckBoxPDF/{fileName}")]
        public HttpResponseMessage DownloadServiceRequestCheckBoxPDF(string fileName)
        {
            var file = _webHostingEnvironment.ContentRootPath + "/UploadedAttachments/" + fileName;
            HttpResponseMessage response = null;
            if (!System.IO.File.Exists(file))
            {
                var folder = Directory.CreateDirectory(_webHostingEnvironment.ContentRootPath + "/UploadedAttachments/");
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

        #endregion

    }
}
