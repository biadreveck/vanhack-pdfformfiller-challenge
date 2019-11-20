using System;
using System.Collections.Generic;
using System.Linq;
using PdfFormFiller.Core.Exceptions;
using PdfFormFiller.Core.Interfaces;
using PdfFormFiller.Data.Exceptions;

namespace PdfFormFiller.Data.Repositories
{
	public class DynamicFileRepository : IDynamicRepository
	{		
		private readonly IFileContextFactory _fileContextFactory;
		private IDynamicFileContext _fileContext;

		public DynamicFileRepository(IFileContextFactory fileContextFactory)
		{
			_fileContextFactory = fileContextFactory;
		}

		public void SetContext(string fileName)
		{
			_fileContext = _fileContextFactory.GetDynamicFileContext(fileName);
		}

		public dynamic Get(string id)
		{
			try
			{
				return _fileContext.ReadItem(id) ?? throw new EntityNotFoundException();
			}
			catch (FileContextException e)
			{
				if (e.Code == FileContextExceptionCode.ItemNotFound)
				{
					throw new EntityNotFoundException();
				}

				throw;
			}
		}

		public IList<dynamic> GetAll()
		{
			return _fileContext.ReadAll();
		}

		public IList<dynamic> Find(Func<dynamic, bool> predicate)
		{
			var items = _fileContext.ReadAll();
			if (items == null) return null;
			return items.Where(predicate).ToList();
		}

		//public dynamic Add(dynamic entity)
		//{
		//	try
		//	{
		//		entity.Id = GenerateId(entity);
		//		return _fileContext.CreateItem(entity);
		//	}
		//	catch (FileContextException e)
		//	{
		//		if (e.Code == FileContextExceptionCode.ItemConflict)
		//		{
		//			throw new EntityAlreadyExistsException();
		//		}

		//		throw;
		//	}
		//}

		//public void Update(dynamic entity)
		//{
		//	try
		//	{
		//		_fileContext.ReplaceItem(entity, entity.Id);
		//	}
		//	catch (FileContextException e)
		//	{
		//		if (e.Code == FileContextExceptionCode.ItemNotFound)
		//		{
		//			throw new EntityNotFoundException();
		//		}

		//		throw;
		//	}
		//}

		//public void Delete(dynamic entity)
		//{
		//	try
		//	{
		//		_fileContext.DeleteItem(entity.Id);
		//	}
		//	catch (FileContextException e)
		//	{
		//		if (e.Code == FileContextExceptionCode.ItemNotFound)
		//		{
		//			throw new EntityNotFoundException();
		//		}

		//		throw;
		//	}
		//}
	}
}
