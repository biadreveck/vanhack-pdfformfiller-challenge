using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using PdfFormFiller.Api.Options;
using PdfFormFiller.Data;

namespace PdfFormFiller.Api.Extensions
{
	public static class ServiceCollectionFileDbExtensions
	{
		public static IServiceCollection AddFileDb(this IServiceCollection services, FileDbOptions options)
		{
			var fileContextFactory = new FileContextFactory(options.DbPath);
			services.AddSingleton<IFileContextFactory>(fileContextFactory);
			return services;
		}
	}
}
