using FatCamel.Host.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using TelegramFatCamel.Module.Extensions;

namespace TelegramFatCamel
{
    public class Startup : IModule
    {
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddServices();
        }
    }
}
