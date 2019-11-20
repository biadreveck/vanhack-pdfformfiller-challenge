using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using PdfFormFiller.Data.Exceptions;

namespace PdfFormFiller.Data
{
	public class DynamicFileContext : IDynamicFileContext
	{
		private readonly string _filePath;

		public DynamicFileContext(string filePath)
		{
			_filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
		}

		public IList<dynamic> ReadAll()
		{
			return ReadFile();
		}

		public dynamic ReadItem(string id)
		{
			IList<dynamic> entities = ReadFile();
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

		//public dynamic CreateItem(dynamic item)
		//{
		//	IList<dynamic> entities = ReadFile();
		//	if (entities == null) entities = new List<dynamic>();

		//	try
		//	{
		//		entities.First(e => e.Id == item.Id);
		//		throw new FileContextException(FileContextExceptionCode.ItemConflict);
		//	}
		//	catch (InvalidOperationException)
		//	{
		//		entities.Add(item);
		//		if (!SaveFile(entities))
		//			throw new FileContextException(FileContextExceptionCode.SavingError);

		//		return item;
		//	}
		//}

		//public dynamic ReplaceItem(dynamic newItem, string id)
		//{
		//	IList<dynamic> entities = ReadFile();
		//	if (entities == null)
		//		throw new FileContextException(FileContextExceptionCode.ItemNotFound);

		//	dynamic item = null;
		//	try
		//	{
		//		item = entities.First(e => e.Id == id);
		//	}
		//	catch (InvalidOperationException)
		//	{
		//		throw new FileContextException(FileContextExceptionCode.ItemNotFound);
		//	}

		//	if (item == null)
		//		throw new FileContextException(FileContextExceptionCode.ItemNotFound);

		//	newItem.Id = id;
		//	entities[entities.IndexOf(item)] = newItem;

		//	if (!SaveFile(entities))
		//		throw new FileContextException(FileContextExceptionCode.SavingError);

		//	return newItem;
		//}

		//public dynamic DeleteItem(string id)
		//{
		//	IList<dynamic> entities = ReadFile();
		//	if (entities == null)
		//		throw new FileContextException(FileContextExceptionCode.ItemNotFound);

		//	dynamic item = null;
		//	try
		//	{
		//		item = entities.First(e => e.Id == id);
		//	}
		//	catch (InvalidOperationException)
		//	{
		//		throw new FileContextException(FileContextExceptionCode.ItemNotFound);
		//	}

		//	if (item == null)
		//		throw new FileContextException(FileContextExceptionCode.ItemNotFound);

		//	entities.Remove(item);

		//	if (!SaveFile(entities))
		//		throw new FileContextException(FileContextExceptionCode.SavingError);

		//	return item;
		//}

		private IList<dynamic> ReadFile()
		{
			try
			{
				using StreamReader r = new StreamReader(_filePath);
				string json = r.ReadToEnd();
				return JsonConvert.DeserializeObject<IList<dynamic>>(json);
			}
			catch
			{
				return null;
			}
		}
		//private bool SaveFile(IList<dynamic> entities)
		//{
		//	try
		//	{
		//		//using StreamWriter w = new StreamWriter(_fileName);
		//		//var serializer = new JsonSerializer();
		//		//serializer.Serialize(w, entities);
		//		var jsonContent = JsonConvert.SerializeObject(entities);
		//		File.WriteAllText(_filePath, jsonContent);
		//		return true;
		//	}
		//	catch
		//	{
		//		return false;
		//	}
		//}
	}
}
