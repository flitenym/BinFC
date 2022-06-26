using HostLibrary;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace Host
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            Initial.ConfigureServices(services, _configuration);
        }

        public void Configure(IApplicationBuilder app, IHostApplicationLifetime hal, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            Initial.Configure(app, hal, env, serviceProvider, _configuration);
        }
    }
}