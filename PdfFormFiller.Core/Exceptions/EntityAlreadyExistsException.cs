using System;
using System.Collections.Generic;
using System.Text;

namespace PdfFormFiller.Core.Exceptions
{
	public class EntityAlreadyExistsException : Exception
	{
		public EntityAlreadyExistsException() { }

		public EntityAlreadyExistsException(string message) : base(message) { }
	}
}
