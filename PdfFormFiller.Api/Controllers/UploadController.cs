using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PdfFormFiller.Api.Options;

namespace PdfFormFiller.Api.Controllers
{
    [Route("pdfform/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1")]
    public class UploadController : Controller
    {
        private readonly PdfFilesOptions _pdfFilesOptions;

        public UploadController(IConfiguration configuration)
        {
            _pdfFilesOptions = configuration.GetSection("PdfFiles").Get<PdfFilesOptions>();
        }

        /// <summary>
        /// Uploads an pdf to the API server and saves it.
        /// </summary>
        /// <param name="pdfCode">Code that will be used to access the file and link it to resources</param>
        /// <returns>204 - No Content</returns>
        [HttpPost("{pdfCode}")]
        public async Task<ActionResult> PostFile(string pdfCode, [FromForm(Name = "file")] IFormFile file)
        {
            if (file == null || file.Length < 0)
            {
                return BadRequest();
            }

            var templateFilePath = $"{pdfCode}.pdf";
            if (!string.IsNullOrEmpty(_pdfFilesOptions.TemplatePath))
            {
                templateFilePath = $"{_pdfFilesOptions.TemplatePath}{Path.DirectorySeparatorChar}{templateFilePath}";
            }

            using (var stream = System.IO.File.Create(templateFilePath))
            {
                await file.CopyToAsync(stream);
            }

            return NoContent();
        }
    }
}