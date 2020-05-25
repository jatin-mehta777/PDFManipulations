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


        //private string[] filesPath = files;
        public string outPutFilePath = "C:\\PDF\\Merged.pdf";
        

        [HttpGet]
        [Route("MergePDFController")]
        public IActionResult MergeFiles()
        {
            return View();
        }
        [HttpGet]
        [Route("MergePDFController/MergeFiless")]
        public ActionResult MergeFiless()
        {
            byte[] password = Encoding.ASCII.GetBytes("123456");
            List<PdfReader> readerList = new List<PdfReader>();
            foreach (string filePath in filesPath)
            {
                PdfReader pdfReader = new PdfReader(filePath, password);
                readerList.Add(pdfReader);
            }
            Document document = new Document(PageSize.A4, 0, 0, 0, 0);
            PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(outPutFilePath, FileMode.Create,FileAccess.ReadWrite));
            document.Open();
            foreach (PdfReader reader in readerList)
            {
                for (int i = 1; i <= reader.NumberOfPages; i++)
                {
                    PdfImportedPage page = writer.GetImportedPage(reader, i);
                    document.Add(iTextSharp.text.Image.GetInstance(page));
                }
            }
            document.Close();
            return View();
        }


    }
}
