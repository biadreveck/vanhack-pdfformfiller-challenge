using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using PdfFormFiller.Core.Models;

namespace PdfFormFiller.Data
{
	public class FileContextFactory : IFileContextFactory
	{
		private readonly string _filePath;

		public FileContextFactory(string filePath)
		{
			_filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
		}

		public IFileContext<T> GetFileContext<T>(string fileName) where T : Entity
		{	
			if (string.IsNullOrEmpty(_filePath))
				return new FileContext<T>(fileName);
			else
				return new FileContext<T>(_filePath + Path.DirectorySeparatorChar + fileName);
		}

		public IDynamicFileContext GetDynamicFileContext(string fileName)
		{
			if (string.IsNullOrEmpty(_filePath))
				return new DynamicFileContext(fileName);
			else
				return new DynamicFileContext(_filePath + Path.DirectorySeparatorChar + fileName);
		}
	}
}
