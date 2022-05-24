using FatCamel.Host.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;
using TelegramFatCamel.Module.Commands;
using TelegramFatCamel.Module.Commands.Base;
using TelegramFatCamel.Module.Services;
using TelegramFatCamel.Module.Services.Interfaces;

namespace TelegramFatCamel.Module
{
    public class Startup : IModule
    {
        public async Task ConfigureAsync(IApplicationBuilder app, IHostApplicationLifetime hal, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            await serviceProvider.GetRequiredService<ITelegramFatCamelBotService>().GetTelegramBotAsync();
            hal.ApplicationStopping.Register(async () => await OnShutdownAsync(serviceProvider));
        }

        private async Task OnShutdownAsync(IServiceProvider serviceProvider)
        {
            await serviceProvider.GetRequiredService<ITelegramFatCamelBotService>().StopTelegramBotAsync();
        }

        public Task ConfigureServicesAsync(IServiceCollection services)
        {
            services.AddLocalization(options => options.ResourcesPath = "Localization");

            services.AddSingleton<ITelegramSettingsService, TelegramSettingsService>();
            services.AddSingleton<ITelegramFatCamelBotService, TelegramFatCamelBotService>();

            services.AddScoped<ICommandExecutorService, CommandExecutorService>();
            // Commands
            services.AddScoped<BaseCommand, AcceptEmailCommand>();
            services.AddScoped<BaseCommand, AcceptNameCommand>();
            services.AddScoped<BaseCommand, AcceptPurseCommand>();
            services.AddScoped<BaseCommand, ChangePurseCommand>();
            services.AddScoped<BaseCommand, ErrorInputIdCommand>();
            services.AddScoped<BaseCommand, GetOperationsCommand>();
            services.AddScoped<BaseCommand, GetPrivateCommand>();            
            services.AddScoped<BaseCommand, InputBepCommand>();
            services.AddScoped<BaseCommand, InputEmailCommand>();
            services.AddScoped<BaseCommand, InputIdCommand>();
            services.AddScoped<BaseCommand, InputNameCommand>();
            services.AddScoped<BaseCommand, InputTrcCommand>();
            services.AddScoped<BaseCommand, NotExistIdCommand>();
            services.AddScoped<BaseCommand, SelectPurseCommand>();
            services.AddScoped<BaseCommand, StartCommand>();

            return Task.CompletedTask;
        }
    }
}