using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using iText.Forms;
using iText.Forms.Fields;
using iText.IO.Image;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Xobject;
using iText.Layout.Element;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PdfFormFiller.Api.Options;
using PdfFormFiller.Api.ViewModels;
using PdfFormFiller.Core.Interfaces;

namespace PdfFormFiller.Api.Controllers
{
    [Route("pdfform/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1")]
    public class ScanController : Controller
    {
        private readonly PdfFilesOptions _pdfFilesOptions;

        public ScanController(IConfiguration configuration)
        {
            _pdfFilesOptions = configuration.GetSection("PdfFiles").Get<PdfFilesOptions>();
        }

        [HttpGet("{pdfCode}")]
        public ActionResult<PdfScanResult> Get(string pdfCode)
        {
            if (string.IsNullOrEmpty(pdfCode))
            {
                return BadRequest();
            }

            var templateFilePath = $"{pdfCode}.pdf";
            if (!string.IsNullOrEmpty(_pdfFilesOptions.TemplatePath))
            {
                templateFilePath = $"{_pdfFilesOptions.TemplatePath}{Path.DirectorySeparatorChar}{templateFilePath}";
            }

            if (!System.IO.File.Exists(templateFilePath))
            {
                return NotFound();
            }

            using PdfReader reader = new PdfReader(templateFilePath);
            reader.SetUnethicalReading(true);
            using PdfDocument pdf = new PdfDocument(reader);
            
            var scanPages = new PdfScanPage[pdf.GetNumberOfPages()];
            var result = new PdfScanResult { 
                PdfCode = pdfCode,
                Pages = new List<PdfScanPage>(pdf.GetNumberOfPages())
            };

            //PdfPage pdfPage = pdf.GetPage(1);
            //PdfFormXObject pdfPageCopy = pdfPage.CopyAsFormXObject(pdf);
            //Image pageImage = new Image(pdfPageCopy);
            //ImageData b = new ImageData();
            //PdfImageXObject a = new PdfImageXObject()

            var form = PdfAcroForm.GetAcroForm(pdf, false);
            var fields = form.GetFormFields();
            foreach (string field in fields.Keys)
            {
                PdfFormField formField = form.GetField(field);
                if (formField.IsReadOnly()) continue;

                var pdfValue = formField.GetValue();
                if (formField.GetWidgets() == null) continue;
                if (formField.GetWidgets().Count <= 0) continue;

                bool isButton = (formField is PdfButtonFormField);
                bool isText = (formField is PdfTextFormField);

                if (!isText && !isButton) continue;

                var page = formField.GetWidgets().First().GetPage();
                var pageNumber = pdf.GetPageNumber(page);

                PdfScanPage scanPage = scanPages[pageNumber - 1];
                if (scanPage == null)
                {
                    scanPage = new PdfScanPage {
                        Number = pageNumber,
                        FormFields = new List<PdfScanFormField>()
                    };
                }

                double minLowerLeftX = double.MaxValue;
                double minLowerLeftY = double.MaxValue;
                double maxUpperRightX = 0;
                double maxUpperRightY = 0;
                foreach (var widget in formField.GetWidgets())
                {
                    var rectangle = widget.GetRectangle();
                    if (rectangle == null) continue;
                    if (rectangle.Size() < 4) continue;

                    // API links explanations:
                    //https://itextpdf.com/en/resources/faq/technical-support/itext-7/how-find-absolute-position-and-dimension-field
                    //https://itextpdf.com/en/resources/faq/technical-support/itext-7/how-show-image-text-field-position
                    var lowerLeftX = rectangle.GetAsNumber(0).GetValue(); //lower left position x
                    var lowerLeftY = rectangle.GetAsNumber(1).GetValue(); //lower left position y
                    var upperRightX = rectangle.GetAsNumber(2).GetValue(); //upper right position x
                    var upperRightY = rectangle.GetAsNumber(3).GetValue(); //upper right position y

                    if (minLowerLeftX > lowerLeftX) minLowerLeftX = lowerLeftX;
                    if (minLowerLeftY > lowerLeftY) minLowerLeftY = lowerLeftY;

                    if (maxUpperRightX < upperRightX) maxUpperRightX = upperRightX;
                    if (maxUpperRightY < upperRightY) maxUpperRightY = upperRightY;
                }

                var scanFormField = new PdfScanFormField
                {
                    Name = field,
                    Position = new Position
                    {
                        X = minLowerLeftX,
                        Y = minLowerLeftY
                    },
                    Size = new Size
                    {
                        Width = maxUpperRightX - minLowerLeftX,
                        Height = maxUpperRightY - minLowerLeftY
                    }
                };

                scanPage.FormFields.Add(scanFormField);
                scanPages[pageNumber - 1] = scanPage;
            }

            result.Pages = scanPages.Where(sp => sp != null).ToList();
            return Ok(result);
        }
    }
}