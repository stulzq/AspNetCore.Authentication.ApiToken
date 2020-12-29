using System;
using System.IO;
using AspNetCore.ApiToken.SampleApp.Store;
using AspNetCore.Authentication.ApiToken;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

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
                .AddApiToken(op => op.UseCache = false)
                // .AddRedisCache(op => op.ConnectionString = "192.168.3.57:6379")
                .AddProfileService<MyApiTokenProfileService>()
                .AddTokenStore<MyApiTokenStore>()
                .AddCleanService();
            // .AddRedisCache(op=>op.ConnectionString="xxx");

            services.AddControllers().AddNewtonsoftJson(op =>
            {
                op.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Local;
                op.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm";
                op.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            });

            //Swagger
            services.AddSwaggerGen(op =>
            {
                op.UseInlineDefinitionsForEnums();
                op.SwaggerDoc("v1",
                    new OpenApiInfo { Title = typeof(Startup).Namespace, Version = "v1" });
                op.DocInclusionPredicate((docName, description) => true);

                op.AddSecurityDefinition("ApiToken", new OpenApiSecurityScheme
                {
                    Description = "Input Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "ApiToken"
                });
                op.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "ApiToken"
                            }
                        },
                        new string[] { }
                    }
                });

            });
            services.AddSwaggerGenNewtonsoftSupport();

            services.AddDbContext<ApiTokenDbContext>(options => options.UseMySql(
                Configuration.GetConnectionString("DefaultConnection"),
                ServerVersion.FromString("5.7-mysql"),
                mySqlOptions => mySqlOptions
                    .CharSetBehavior(CharSetBehavior.NeverAppend)));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                app.UseSwagger();
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
