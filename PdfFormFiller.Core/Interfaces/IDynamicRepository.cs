using System;
using System.Collections.Generic;
using System.Text;

namespace PdfFormFiller.Core.Interfaces
{
	public interface IDynamicRepository
	{
		void SetContext(string fileName);
		dynamic Get(string id);
		IList<dynamic> GetAll();
		IList<dynamic> Find(Func<dynamic, bool> predicate);

		//dynamic Add(dynamic entity);
		//void Update(dynamic entity);
		//void Delete(dynamic entity);
	}
}
