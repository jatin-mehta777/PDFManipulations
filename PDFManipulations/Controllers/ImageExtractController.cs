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
    public class ImageExtractController : Controller
    {
        
        static string rootFolder = Directory.GetCurrentDirectory();
        public static string outPutFilePath = rootFolder + "\\wwwroot\\Merged PDFs\\Extracted" + ".pdf";
        List<PdfReader> readerListpdf = new List<PdfReader>();
        byte[] password = Encoding.ASCII.GetBytes("123456");

        [HttpGet]
        [Route("ExtactImagesFromFile")]
        public IActionResult ExtactImagesFromFile()
        {
            return View();
        }

        [HttpPost]
        [Route("MergePDFController/ExtractImagesFromPDF")]
        public ActionResult ExtractImagesFromPDF( IFormFile file)

        {
            
            String FileExt = Path.GetExtension(file.FileName).ToUpper();

            Stream fileStream = file.OpenReadStream();
            var mStreamer = new MemoryStream();

            mStreamer.SetLength(fileStream.Length);
            fileStream.Read(mStreamer.GetBuffer(), 0, (int)fileStream.Length);
            mStreamer.Seek(0, SeekOrigin.Begin);

            var imgs = new List<System.Drawing.Image>();
            var pdf = new PdfReader(mStreamer.GetBuffer());

            try
            {
                for (int pageNumber = 1; pageNumber <= pdf.NumberOfPages; pageNumber++)
                {
                    PdfDictionary pg = pdf.GetPageN(pageNumber);
                    List<PdfObject> objs = FindImageInPDFDictionary(pg);

                    foreach (var obj in objs)
                    {
                        if (obj != null)
                        {
                            int XrefIndex = Convert.ToInt32(((PrIndirectReference)obj).Number.ToString(System.Globalization.CultureInfo.InvariantCulture));
                            PdfObject pdfObj = pdf.GetPdfObject(XrefIndex);
                            PdfStream pdfStrem = (PdfStream)pdfObj;

                            PdfReader pdfReader = new PdfReader(pdfStrem.ToString(), password);
                            readerListpdf.Add(pdfReader);
                            //MemoryStream ms = new MemoryStream();
                            //st.Read(ms.GetBuffer(), 0, (int)st.Length);
                            //ms.Seek(0, SeekOrigin.Begin);
                            //byte[] imagebytes = ms.GetBuffer();
                            //SaveFileDetails(Fd);

                            //var pdfImage = new iTextSharp.text.pdf.PdfImage((PrStream)pdfStrem);
                            //var img = pdfImage.GetDrawingImage();

                            //imgs.Add(img);
                        }
                    }

                }
                Document document = new Document(PageSize.A4, 0, 0, 0, 0);
                //Create blank output pdf file and get the stream to write on it.
                PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(outPutFilePath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite, 4096, true));
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
            finally
            {
                pdf.Close();
            }


        }

        private List<PdfObject> FindImageInPDFDictionary(PdfDictionary pg)
        {
            var res = (PdfDictionary)PdfReader.GetPdfObject(pg.Get(PdfName.Resources));
            var xobj = (PdfDictionary)PdfReader.GetPdfObject(res.Get(PdfName.Xobject));
            var pdfObgs = new List<PdfObject>();

            if (xobj != null)
            {
                foreach (PdfName name in xobj.Keys)
                {
                    PdfObject obj = xobj.Get(name);
                    if (obj.IsIndirect())
                    {
                        var tg = (PdfDictionary)PdfReader.GetPdfObject(obj);
                        var type = (PdfName)PdfReader.GetPdfObject(tg.Get(PdfName.Subtype));

                        if (PdfName.Image.Equals(type)) // image at the root of the pdf
                        {
                            pdfObgs.Add(obj);
                        }
                        else if (PdfName.Form.Equals(type)) // image inside a form
                        {
                            FindImageInPDFDictionary(tg).ForEach(o => pdfObgs.Add(o));
                        }
                        else if (PdfName.Group.Equals(type)) // image inside a group
                        {
                            FindImageInPDFDictionary(tg).ForEach(o => pdfObgs.Add(o));
                        }
                    }
                }
            }

            return pdfObgs;
        }





    }
}
