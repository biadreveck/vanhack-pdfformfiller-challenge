using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PdfFormFiller.Api.Extensions;
using PdfFormFiller.Api.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace PdfFormFiller.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
			services.AddCors(o => o.AddPolicy("AllowAll", builder =>
			{
				builder.AllowAnyOrigin()
					   .AllowAnyMethod()
					   .AllowAnyHeader()
					   .AllowCredentials();
			}));
			services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

			services.AddApiVersioning(p =>
			{
				p.DefaultApiVersion = new ApiVersion(1, 0);
				p.ReportApiVersions = true;
				p.AssumeDefaultVersionWhenUnspecified = true;
			});

			services.AddVersionedApiExplorer(p =>
			{
				p.GroupNameFormat = "'v'VVV";
				p.SubstituteApiVersionInUrl = true;
			});

			services.AddTransient<IConfigureOptions<SwaggerGenOptions>, SwaggerOptions>();
			services.AddSwaggerGen();

			// Api Extensions
			var fileDbOptions = Configuration.GetSection("FileDb").Get<FileDbOptions>();
			services.AddRepositories();
			services.AddFileDb(fileDbOptions);
		}

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApiVersionDescriptionProvider provider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

			app.UseCors("AllowAll");
            app.UseHttpsRedirection();
            app.UseMvc();
			app.UseApiVersioning();
			app.UseMvcWithDefaultRoute();

			app.UseSwagger();
			app.UseSwaggerUI(options =>
			{
				foreach (var description in provider.ApiVersionDescriptions)
				{
					options.SwaggerEndpoint(
					$"/swagger/{description.GroupName}/swagger.json",
					description.GroupName.ToUpperInvariant());
				}

				options.DocExpansion(DocExpansion.List);
			});
		}
    }
}
