using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using PdfFormFiller.Core.Models;
using PdfFormFiller.Data.Exceptions;

namespace PdfFormFiller.Data
{
	public class FileContext<T> : IFileContext<T> where T : Entity
	{
		private readonly string _filePath;

		public FileContext(string filePath)
		{
			_filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
		}

		public IList<T> ReadAll()
		{
			return ReadFile();
		}

		public T ReadItem(string id)
		{
			IList<T> entities = ReadFile();
			if (entities == null)
				throw new FileContextException(FileContextExceptionCode.ItemNotFound);

			try
			{
				return entities.First(e => e.Id == id);
			}
			catch (InvalidOperationException)
			{
				throw new FileContextException(FileContextExceptionCode.ItemNotFound);
			}			
		}

		public T CreateItem(T item)
		{
			IList<T> entities = ReadFile();
			if (entities == null) entities = new List<T>();

			try
			{
				entities.First(e => e.Id == item.Id);
				throw new FileContextException(FileContextExceptionCode.ItemConflict);
			}
			catch (InvalidOperationException)
			{
				entities.Add(item);
				if (!SaveFile(entities)) 
					throw new FileContextException(FileContextExceptionCode.SavingError);

				return item;
			}
		}

		public T ReplaceItem(T newItem, string id)
		{
			IList<T> entities = ReadFile();
			if (entities == null)
				throw new FileContextException(FileContextExceptionCode.ItemNotFound);

			T item = null;
			try
			{
				item = entities.First(e => e.Id == id);
			}
			catch (InvalidOperationException)
			{
				throw new FileContextException(FileContextExceptionCode.ItemNotFound);
			}

			if (item == null)
				throw new FileContextException(FileContextExceptionCode.ItemNotFound);

			newItem.Id = id;
			entities[entities.IndexOf(item)] = newItem;

			if (!SaveFile(entities))
				throw new FileContextException(FileContextExceptionCode.SavingError);

			return newItem;
		}

		public T DeleteItem(string id)
		{
			IList<T> entities = ReadFile();
			if (entities == null)
				throw new FileContextException(FileContextExceptionCode.ItemNotFound);

			T item = null;
			try
			{
				item = entities.First(e => e.Id == id);
			}
			catch (InvalidOperationException)
			{
				throw new FileContextException(FileContextExceptionCode.ItemNotFound);
			}

			if (item == null)
				throw new FileContextException(FileContextExceptionCode.ItemNotFound);

			entities.Remove(item);

			if (!SaveFile(entities))
				throw new FileContextException(FileContextExceptionCode.SavingError);

			return item;
		}

		private IList<T> ReadFile()
		{
			try
			{
				using StreamReader r = new StreamReader(_filePath);
				string json = r.ReadToEnd();
				return JsonConvert.DeserializeObject<IList<T>>(json);
			}
			catch 
			{
				return null;
			}
		}
		private bool SaveFile(IList<T> entities)
		{
			try
			{
				//using StreamWriter w = new StreamWriter(_fileName);
				//var serializer = new JsonSerializer();
				//serializer.Serialize(w, entities);
				var jsonContent = JsonConvert.SerializeObject(entities);
				File.WriteAllText(_filePath, jsonContent);
				return true;
			}
			catch
			{
				return false;
			}
		}
	}
}
