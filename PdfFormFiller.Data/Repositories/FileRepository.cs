using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using PdfFormFiller.Core.Exceptions;
using PdfFormFiller.Core.Interfaces;
using PdfFormFiller.Core.Models;
using PdfFormFiller.Data.Exceptions;

namespace PdfFormFiller.Data.Repositories
{
	public abstract class FileRepository<T> : IRepository<T> where T : Entity
	{
		private readonly IFileContext<T> _fileContext;

		protected FileRepository(IFileContextFactory fileContextFactory)
		{
			_fileContext = fileContextFactory.GetFileContext<T>(FileName);
		}

		public T Get(string id)
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

		public IList<T> GetAll()
		{
			return _fileContext.ReadAll();
		}

		public IList<T> Find(Func<T, bool> predicate)
		{
			var items = _fileContext.ReadAll();
			if (items == null) return null;
			return items.Where(predicate).ToList();
		}

		public T Add(T entity)
		{
			try
			{
				entity.Id = GenerateId(entity);
				return _fileContext.CreateItem(entity);
			}
			catch (FileContextException e)
			{
				if (e.Code == FileContextExceptionCode.ItemConflict)
				{
					throw new EntityAlreadyExistsException();
				}

				throw;
			}
		}

		public void Update(T entity)
		{
			try
			{
				_fileContext.ReplaceItem(entity, entity.Id);
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

		public void Delete(T entity)
		{
			try
			{
				_fileContext.DeleteItem(entity.Id);
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

		public abstract string FileName { get; }
		public virtual string GenerateId(T entity) => Guid.NewGuid().ToString();
	}
}
