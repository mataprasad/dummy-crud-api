using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using DummyCrudApi.Controllers;
using DummyCrudApi.Fx;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace DummyCrudApi
{
    public class Startup
    {
        public const string API_KEY_NAME = "X-API-KEY";

        private IWebHostEnvironment env;

        public Startup(IWebHostEnvironment env)
        {
            this.env = env;
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            this.Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; private set; }

        public ILifetimeScope AutofacContainer { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
            services.AddHttpClient();
            services.AddScoped<ApiKeyFilter>();
            services.AddControllersWithViews();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Dummy CRUD API",
                    Description = "Made with &#x2764;&#xfe0f; &nbsp;using ASP.NET Core",
                    Contact = new OpenApiContact
                    {
                        Name = "Mata Prasad Chauhan",
                        Email = string.Empty,
                        Url = new Uri("https://github.com/mataprasad"),
                    }
                });
                c.AddSecurityDefinition(API_KEY_NAME, new OpenApiSecurityScheme
                {
                    Name = API_KEY_NAME,
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = ""
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "X-API-KEY"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
                c.OperationFilter<SwaggerUnauthorizedResponse>();
            });
            services.AddDynamicRegistartion(Configuration, "IDbConnectionBuilder");
            services.AddDynamicRegistartion(Configuration, "IDbContext");
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            // Register your own things directly with Autofac, like:
            StartupHelper.LoadDynamicAutofacModule(this.env, Configuration, builder, "IDbConnectionBuilder");
            StartupHelper.LoadDynamicAutofacModule(this.env, Configuration, builder, "IDbContext");
        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // If, for some reason, you need a reference to the built container, you
            // can use the convenience extension method GetAutofacRoot.
            this.AutofacContainer = app.ApplicationServices.GetAutofacRoot();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseSwagger(c =>
            {
                c.SerializeAsV2 = true;
            });

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Dummy CRUD API");
                c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
            });

            app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
