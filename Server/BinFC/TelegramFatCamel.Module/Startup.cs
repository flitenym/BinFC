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
        public Task ConfigureAsync(IApplicationBuilder app, IHostApplicationLifetime hal, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            return Task.CompletedTask;
        }

        public Task ConfigureServicesAsync(IServiceCollection services)
        {
            services.AddLocalization(options => options.ResourcesPath = "Localization");

            services.AddSingleton<TelegramFatCamelBotService>();
            services.AddHostedService(sp => sp.GetRequiredService<TelegramFatCamelBotService>());

            services.AddScoped<ICommandExecutorService, CommandExecutorService>();
            // Commands
            services.AddScoped<BaseCommand, AcceptEmailCommand>();
            services.AddScoped<BaseCommand, AcceptNameCommand>();
            services.AddScoped<BaseCommand, AcceptPurseCommand>();
            services.AddScoped<BaseCommand, AlreadyInputIdCommand>();
            services.AddScoped<BaseCommand, ChangePurseCommand>();
            services.AddScoped<BaseCommand, ErrorInputIdCommand>();
            services.AddScoped<BaseCommand, GetOperationsCommand>();
            services.AddScoped<BaseCommand, GetPrivateCommand>();            
            services.AddScoped<BaseCommand, InputBepCommand>();
            services.AddScoped<BaseCommand, InputEmailCommand>();
            services.AddScoped<BaseCommand, InputIdAlreadyExistChatCommand>();
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