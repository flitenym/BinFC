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

            services.AddSingleton<ICommandExecutorService, CommandExecutorService>();
            services.AddSingleton<ITelegramSettingsService, TelegramSettingsService>();
            services.AddSingleton<ITelegramFatCamelBotService, TelegramFatCamelBotService>();

            // Commands
            services.AddSingleton<BaseCommand, AcceptEmailCommand>();
            services.AddSingleton<BaseCommand, AcceptNameCommand>();
            services.AddSingleton<BaseCommand, AcceptPurseCommand>();
            services.AddSingleton<BaseCommand, ChangePurseCommand>();
            services.AddSingleton<BaseCommand, ErrorInputIdCommand>();
            services.AddSingleton<BaseCommand, GetOperationsCommand>();
            services.AddSingleton<BaseCommand, InputBepCommand>();
            services.AddSingleton<BaseCommand, InputEmailCommand>();
            services.AddSingleton<BaseCommand, InputIdCommand>();
            services.AddSingleton<BaseCommand, InputNameCommand>();
            services.AddSingleton<BaseCommand, InputTrcCommand>();
            services.AddSingleton<BaseCommand, NotExistIdCommand>();
            services.AddSingleton<BaseCommand, SelectPurseCommand>();
            services.AddSingleton<BaseCommand, StartCommand>();

            return Task.CompletedTask;
        }
    }
}