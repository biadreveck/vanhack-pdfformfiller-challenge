using System;
using System.Collections.Generic;
using System.Text;

namespace PdfFormFiller.Data.Exceptions
{
	public class FileContextException : Exception
	{
		public FileContextExceptionCode Code { get; }

		public FileContextException() { }
		public FileContextException(string message) : base(message) { }
		public FileContextException(FileContextExceptionCode code)
		{
			Code = code;
		}
		public FileContextException(string message, FileContextExceptionCode code) : base(message) 
		{
			Code = code;
		}
	}
}
