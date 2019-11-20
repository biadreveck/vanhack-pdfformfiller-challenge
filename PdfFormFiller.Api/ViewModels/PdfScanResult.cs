using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PdfFormFiller.Api.ViewModels
{
    public class PdfScanResult
    {
        public string PdfCode { get; set; }
        public List<PdfScanPage> Pages { get; set; }
    }
}
