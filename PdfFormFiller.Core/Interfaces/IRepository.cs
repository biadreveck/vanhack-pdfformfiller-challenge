using System;
using System.Collections.Generic;
using PdfFormFiller.Core.Models;

namespace PdfFormFiller.Core.Interfaces
{
	public interface IRepository<T> where T : Entity
	{
		T Get(string id);
		IList<T> GetAll();
		IList<T> Find(Func<T, bool> predicate);

		T Add(T entity);
		void Update(T entity);
		void Delete(T entity);
	}
}
