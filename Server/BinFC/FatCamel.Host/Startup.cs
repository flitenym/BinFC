using FatCamel.Host.Enum;
using FatCamel.Host.Interfaces;
using FatCamel.Host.Middlewares;
using FatCamel.Host.StaticClasses;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace FatCamel.Host
{
    public class Startup
    {
        private IModule[] _modules;
        private IConfiguration _configuration { get; }

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
            System.Diagnostics.Debugger.Launch();
            _modules = StartupManager.Graph?.Select(m => m.CreateInstance(_configuration)).Where(m => m != null).Cast<IModule>().ToArray() ?? new IModule[0];
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<RequestLocalizationOptions>(options =>
            {
                options.DefaultRequestCulture = new RequestCulture(nameof(CultureType.ru));
                options.SupportedCultures = new List<CultureInfo>() { 
                    new CultureInfo(nameof(CultureType.ru)),
                    new CultureInfo(nameof(CultureType.en))
                };
                options.SupportedUICultures = new List<CultureInfo>() {
                    new CultureInfo(nameof(CultureType.ru)),
                    new CultureInfo(nameof(CultureType.en))
                };
            });

            services.AddControllers();

            foreach (var m in _modules)
            {
                m.ConfigureServicesAsync(services).Wait();
            }

            services.Configure<ExceptionHandlerOptions>(options =>
            {
                options.ExceptionHandlingPath = "/Error";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostApplicationLifetime hal, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            app.UseForwardedHeaders();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler();
            }

            app.UseRequestLocalization();

            app.UseMiddleware<CultureMiddleware>();

            app.UseStaticFiles();

            foreach (var m in _modules)
            {
                m.ConfigureAsync(app, hal, env, serviceProvider).Wait();
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