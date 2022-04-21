using System;
using System.Diagnostics;
using FatCamel.Host.Core;
using FatCamel.Host.Enums;
using FatCamel.Host.Extensions;
using FatCamel.Host.StaticClasses;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Localization;

namespace FatCamel.Host
{
    public class Program
    {
        private static readonly IStringLocalizer _localizer = InternalLocalizers.General;

        public static void Main()
        {
            try
            {
                //System.Diagnostics.Debugger.Launch();
                var host = CreateHostBuilder().Build();
                var lifetime = host.Services.GetService(typeof(IHostApplicationLifetime)) as IHostApplicationLifetime;
                lifetime?.ApplicationStarted.Register(() => StartupManager.InitComponents(StartupStages.ApplicationStart));
                host.Run();
            }
            catch (Exception ex)
            {
                StartupLogger.LogError(ex, _localizer["GLOBAL_ERROR"]);

                Console.WriteLine(_localizer["SERVER_STOP"]!);
                if (!(Environment.GetEnvironmentVariable("APP_POOL_ID") is string) && !Debugger.IsAttached)
                {
                    Console.WriteLine(_localizer["PRESS_ENTER_KEY"]!);
                    Console.Read();
                }
            }
        }

        public static IHostBuilder CreateHostBuilder() =>
            Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder()
                .ConfigureModules()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseModulesWebAssets();
                    webBuilder.UseStartup<Startup>();
                })
                .FinalConfiguration();
    }
}