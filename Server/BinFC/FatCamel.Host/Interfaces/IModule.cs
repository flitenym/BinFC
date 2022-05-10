using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;

namespace FatCamel.Host.Interfaces
{
    public interface IModule
    {
        Task ConfigureServicesAsync(IServiceCollection services);
        Task ConfigureAsync(IApplicationBuilder app, IHostApplicationLifetime hal, IWebHostEnvironment env, IServiceProvider serviceProvider);
    }
}