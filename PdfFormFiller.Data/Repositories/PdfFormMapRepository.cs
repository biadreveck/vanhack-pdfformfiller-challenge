using System.Linq;
using PdfFormFiller.Core.Exceptions;
using PdfFormFiller.Core.Interfaces;
using PdfFormFiller.Core.Models;

namespace PdfFormFiller.Data.Repositories
{
	public class PdfFormMapRepository : FileRepository<PdfFormMap>, IPdfFormMapRepository
	{
		public PdfFormMapRepository(IFileContextFactory factory) : base(factory) { }

		public PdfFormMap GetByFormId(string formId)
		{
			var items = Find(pfm => pfm.FormId == formId);
			if (items == null) throw new EntityNotFoundException();
			return items.First();
		}

		public override string FileName { get; } = "pdfformmaps.json";
	}
}
