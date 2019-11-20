using System;
using System.Collections.Generic;
using System.Text;
using PdfFormFiller.Core.Enums;

namespace PdfFormFiller.Core.Models
{
	public class PdfMapDynamicValue
	{
		public PdfMapDynamicValueType Type { get; set; }
		public dynamic Value { get; set; }
	}
}
