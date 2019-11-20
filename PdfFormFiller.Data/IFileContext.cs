using System;
using System.Collections.Generic;
using System.Text;
using PdfFormFiller.Core.Models;

namespace PdfFormFiller.Data
{
	public interface IFileContext<T> where T : Entity
	{
		IList<T> ReadAll();
		T ReadItem(string id);
		T CreateItem(T item);
		T ReplaceItem(T newItem, string id);
		T DeleteItem(string id);
	}
}
