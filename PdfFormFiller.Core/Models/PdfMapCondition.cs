using System;
using System.Collections.Generic;
using System.Text;
using PdfFormFiller.Core.Enums;

namespace PdfFormFiller.Core.Models
{
	public class PdfMapCondition
	{
		public PdfMapConditionType Type { get; set; }
		public PdfMapDynamicValue Left { get; set; }
		public PdfMapDynamicValue Right { get; set; }
	}
}
