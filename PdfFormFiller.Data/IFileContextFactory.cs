using System;
using System.Collections.Generic;
using System.Text;
using PdfFormFiller.Core.Models;

namespace PdfFormFiller.Data
{
	public interface IFileContextFactory
	{
		IFileContext<T> GetFileContext<T>(string fileName) where T : Entity;
		IDynamicFileContext GetDynamicFileContext(string fileName);
	}
}
