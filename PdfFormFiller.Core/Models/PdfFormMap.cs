using System;
using System.Collections.Generic;
using System.Text;

namespace PdfFormFiller.Core.Models
{
	public class PdfFormMap : Entity
	{
		public string PdfCode { get; set; }
		public IList<string> Collections { get; set; }
		public IList<PdfFormFieldMap> FieldMaps { get; set; }
	}
}
