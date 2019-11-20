using System;
using System.Collections.Generic;
using System.Text;

namespace PdfFormFiller.Data
{
	public interface IDynamicFileContext
	{
		IList<dynamic> ReadAll();
		dynamic ReadItem(string id);
	}
}
