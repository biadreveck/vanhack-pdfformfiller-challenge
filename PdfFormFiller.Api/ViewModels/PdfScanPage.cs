using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PdfFormFiller.Api.ViewModels
{
    public class PdfScanPage
    {
        public int Number { get; set; }
        public string Image { get; set; }
        public List<PdfScanFormField> FormFields { get; set; }
    }
}
