using System;
using System.Collections.Generic;
using System.Text;

namespace PdfFormFiller.Core.Models
{
	public class PdfFormFieldMap
	{
		public string Name { get; set; }
		public PdfMapCondition Condition { get; set; }
		public PdfMapDynamicValue Value { get; set; }
	}
}
