using FatCamel.Host.Enum;
using FatCamel.Host.Interfaces;
using FatCamel.Host.Middlewares;
using FatCamel.Host.StaticClasses;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
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
            _modules = StartupManager.Graph?.Select(m => m.CreateInstance(_configuration)).Where(m => m != null).Cast<IModule>().ToArray() ?? new IModule[0];
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        options.RequireHttpsMetadata = false;
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            // укзывает, будет ли валидироваться издатель при валидации токена
                            ValidateIssuer = true,
                            // строка, представляющая издателя
                            ValidIssuer = AuthOptions.ISSUER,

                            // будет ли валидироваться потребитель токена
                            ValidateAudience = true,
                            // установка потребителя токена
                            ValidAudience = AuthOptions.AUDIENCE,
                            // будет ли валидироваться время существования
                            ValidateLifetime = false,

                            // установка ключа безопасности
                            IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
                            // валидация ключа безопасности
                            ValidateIssuerSigningKey = true,
                        };
                    });

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

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder =>
                    {
                        builder
                          .AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                    });
            });

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
            app.UseExceptionHandler("/error");

            app.UseForwardedHeaders();

            app.UseRequestLocalization();

            app.UseMiddleware<CultureMiddleware>();

            app.UseStaticFiles();

            foreach (var m in _modules)
            {
                m.ConfigureAsync(app, hal, env, serviceProvider).Wait();
            }

            app.UseRouting();

            app.UseCors("CorsPolicy");

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}