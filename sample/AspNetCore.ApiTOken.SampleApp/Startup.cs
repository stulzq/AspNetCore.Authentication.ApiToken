using System;
using System.IO;
using AspNetCore.Authentication.ApiToken;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace AspNetCore.ApiToken.SampleApp
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
            services.AddAuthentication(ApiTokenDefaults.AuthenticationScheme)
                .AddApiToken(op => op.ParseType = ApiTokenParseType.Both)
                .AddProfileService<MyApiTokenProfileService>()
                .AddTokenStore<MyApiTokenStore>();
            services.AddControllers().AddNewtonsoftJson(op =>
            {
                //设置时区
                op.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Local;
                op.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm";
                //序列化采用驼峰命名
                op.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            });

            //Swagger
            services.AddSwaggerGen(op =>
            {
                op.UseInlineDefinitionsForEnums();

                op.SwaggerDoc("v1",
                    new OpenApiInfo { Title = typeof(Startup).Namespace, Version = "v1" });
                op.DocInclusionPredicate((docName, description) => true);

                var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                foreach (var file in Directory.GetFiles(baseDirectory))
                {
                    if (Path.GetExtension(file).ToLower() == ".xml")
                    {
                        op.IncludeXmlComments(file, true);
                    }
                }

            });
            services.AddSwaggerGenNewtonsoftSupport();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                app.UseSwagger(c =>
                    {
                        c.RouteTemplate = "swagger/{documentName}/swagger.json";

                    }
                );
                app.UseSwaggerUI(op =>
                {
                    op.SwaggerEndpoint($"/swagger/v1/swagger.json",
                        $"{typeof(Startup).Namespace} v1");
                });
            }

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
