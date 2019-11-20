using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PdfFormFiller.Api.ViewModels
{
    public class PdfScanFormField
    {
        public string Name { get; set; }
        public Position Position { get; set; }
        public Size Size { get; set; }
    }
}
