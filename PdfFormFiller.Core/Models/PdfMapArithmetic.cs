using System;
using System.Collections.Generic;
using System.Text;
using PdfFormFiller.Core.Enums;

namespace PdfFormFiller.Core.Models
{
	public class PdfMapArithmetic
	{
		public PdfMapArithmeticType Type { get; set; }
		public PdfMapDynamicValue Left { get; set; }
		public PdfMapDynamicValue Right { get; set; }
	}
}
