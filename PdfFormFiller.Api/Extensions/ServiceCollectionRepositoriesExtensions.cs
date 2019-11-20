using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using PdfFormFiller.Core.Interfaces;
using PdfFormFiller.Data.Repositories;

namespace PdfFormFiller.Api.Extensions
{
	public static class ServiceCollectionRepositoriesExtensions
	{
		public static IServiceCollection AddRepositories(this IServiceCollection services)
		{
			services.AddScoped<IPdfFormMapRepository, PdfFormMapRepository>()
				.AddScoped<IDynamicRepository, DynamicFileRepository>();

			return services;
		}
	}
}
