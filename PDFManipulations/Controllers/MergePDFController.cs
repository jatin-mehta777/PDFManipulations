using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PDFManipulations.Models;
using iTextSharp.text.pdf;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using Dapper;
using System.Data.SqlClient;
using System.Data;
using static System.Net.Mime.MediaTypeNames;
using iTextSharp.text;
using System.Text;

namespace PDFManipulations.Controllers
{
    public class MergePDFController : Controller
    {

        //System.IO.DirectoryInfo files = new DirectoryInfo("C:\\PDF");
        string[] filesPath = Directory.GetFiles("C:\\PDF");
        byte[] password = Encoding.ASCII.GetBytes("123456");
        List<PdfReader> readerList = new List<PdfReader>();
        List<byte[]> combineBytes = new List<byte[]>();
        byte[] finalbytes;
        //private string[] filesPath = files;
        static string rootFolder = Directory.GetCurrentDirectory();
        public static string outPutFilePath = rootFolder +"\\wwwroot\\Merged PDFs\\Merged"  +".pdf";
        List<PdfReader> readerListpdf = new List<PdfReader>();

        [HttpGet]
        [Route("MergePDFController")]
        public IActionResult MergeFiles()
        {
            return View();
        }
        [HttpPost]
        [Route("MergePDFController/MergeFiless")]
        public ActionResult MergeFiless(IFormFile[] files)
        {
            byte[] password = Encoding.ASCII.GetBytes("123456");

            try {
                for (int i = 0; i < files.Length; i++)
                {
                    Stream fileStream = files[i].OpenReadStream();
                    var mStreamer = new MemoryStream();

                    mStreamer.SetLength(fileStream.Length);
                    fileStream.Read(mStreamer.GetBuffer(), 0, (int)fileStream.Length);
                    mStreamer.Seek(0, SeekOrigin.Begin);
                    //combineBytes[i] = mStreamer.GetBuffer();
                    //combineBytes.Add(combineBytes[i]);
                    ////PdfReader pdf = new PdfReader();
                    ////pdf.AddPdfObject(pdf)
                    PdfReader pdfReader = new PdfReader(mStreamer, password);
                    readerListpdf.Add(pdfReader);
                    mStreamer.Flush();
                    mStreamer.Dispose();
                    //finalbytes =  concatAndAddContent(combineBytes);
                }

                Document document = new Document(PageSize.A4, 0, 0, 0, 0);
                //Create blank output pdf file and get the stream to write on it.
                PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(outPutFilePath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite,  4096, true));
                document.Open();

                foreach (PdfReader reader in readerListpdf)
                {
                    for (int i = 1; i <= reader.NumberOfPages; i++)
                    {
                        PdfImportedPage page = writer.GetImportedPage(reader, i);
                        document.Add(iTextSharp.text.Image.GetInstance(page));
                    }
                }
                writer.Close();
                document.Close();
                return View();
            }
           catch (Exception ex)
            {
                string error = ex.Message;
                return View(ex.Message);
            }
}

        [HttpGet]
        public  async Task<IActionResult> DownloadFile()
        {

            //     string rootFolder = Directory.GetCurrentDirectory();
            //public  string outPutFilePath = rootFolder + "\\wwwroot\\Merged PDFs\\Merged.pdf";
            

            var path = outPutFilePath;
            
            var memory = new MemoryStream();
            using (var stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite, 4096, true))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return File(memory, "application/pdf", Path.GetFileName(path));
        }
    }
}

        //using (MemoryStream inputData = new MemoryStream(finalbytes))
        //{
        //    using (MemoryStream outputData = new MemoryStream())
        //    {
        //        //iTextSharp.text.pdf.PdfReader reader = new iTextSharp.text.pdf.PdfReader(bytes, password);
        //        var font = BaseFont.CreateFont(BaseFont.TIMES_BOLD, BaseFont.WINANSI, BaseFont.EMBEDDED);
        //        byte[] watemarkedbytes = AddWatermark(finalbytes, font);

        //        iTextSharp.text.pdf.PdfReader awatemarkreader = new iTextSharp.text.pdf.PdfReader(watemarkedbytes);
        //        PdfEncryptor.Encrypt(awatemarkreader, outputData, true, "123456", "123456", PdfWriter.ALLOW_SCREENREADERS);
        //        finalbytes = outputData.ToArray();
        //        FileDetailsModel Fd = new Models.FileDetailsModel();
        //        Fd.FileName = "Merged"+DateTime.Now;
        //        Fd.FileContent = finalbytes;
        //        SaveFileDetails(Fd);
        //        return File(finalbytes, "application/pdf");

        //    }
        //}
    
        //private void SaveFileDetails(FileDetailsModel objDet)
        //{

        //    DynamicParameters Parm = new DynamicParameters();
        //    Parm.Add("@FileName", objDet.FileName);
        //    Parm.Add("@FileContent", objDet.FileContent);
        //    DbConnection();
        //    con.Open();
        //    con.Execute("AddFileDetails", Parm, commandType: System.Data.CommandType.StoredProcedure);
        //    con.Close();


        //}


        //private SqlConnection con;
        //private string constr;
        //private void DbConnection()
        //{
        //    //constr =ConfigurationManager.ConnectionStrings["dbcon"].ToString();
        //    constr = @"Server = (localdb)\MSSQLLocalDB; Database = PDFFIles; Trusted_Connection = True";
        //    con = new SqlConnection(constr);

        //}


        //public byte[] AddWatermark(byte[] bytes, BaseFont bf)
        //{
        //    using (var ms = new MemoryStream(1000 * 1024))
        //    {
        //        PdfReader reader = new PdfReader(bytes, password);
                
        //        PdfStamper stamper = new PdfStamper(reader, ms);
        //        PdfContentByte waterMark;
        //        int times = reader.NumberOfPages;


        //        for (int pageIndex = 1; pageIndex <= reader.NumberOfPages; pageIndex++)
        //        {
        //            waterMark = stamper.GetOverContent(pageIndex);
        //            iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(@"C:\Users\jmehta7\source\repos\PDFManipulations\PDFManipulations\wwwroot\images\12097871.jpg");
        //            img.SetAbsolutePosition(100, 100);
        //            waterMark.AddImage(img);
        //        }
        //        stamper.FormFlattening = true;
        //        stamper.Close();

        //        return ms.ToArray();
        //    }
        //}

        //public static byte[] concatAndAddContent(List<byte[]> pdfByteContent)
        //{
        //    byte[] allBytes;

        //    using (MemoryStream ms = new MemoryStream())
        //    {
        //        Document doc = new Document();
        //        PdfWriter writer = PdfWriter.GetInstance(doc, ms);

        //        doc.SetPageSize(PageSize.Letter);
        //        doc.Open();
        //        PdfContentByte cb = writer.DirectContent;
        //        PdfImportedPage page;

        //        PdfReader reader;
        //        foreach (byte[] p in pdfByteContent)
        //        {
        //            reader = new PdfReader(p);
        //            int pages = reader.NumberOfPages;

        //            // loop over document pages
        //            for (int i = 1; i <= pages; i++)
        //            {
        //                doc.SetPageSize(PageSize.Letter);
        //                doc.NewPage();
        //                page = writer.GetImportedPage(reader, i);
        //                cb.AddTemplate(page, 0, 0);

        //            }
        //        }

        //        doc.Close();
        //        allBytes = ms.GetBuffer();
        //        ms.Flush();
        //        ms.Dispose();
        //    }

        //    return allBytes;
        //}

    
