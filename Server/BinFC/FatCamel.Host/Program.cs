using System;
using System.Diagnostics;
using FatCamel.Host.Core;
using FatCamel.Host.Extensions;
using FatCamel.Host.StaticClasses;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Localization;

namespace FatCamel.Host
{
    public class Program
    {
        public static void Main()
        {
            try
            {
                CreateHostBuilder().Build().Run();
            }
            catch (Exception ex)
            {
                StartupLogger.LogError(ex, InternalLocalizers.General["GLOBAL_ERROR"]);

                Console.WriteLine(InternalLocalizers.General["SERVER_STOP"]!);
                if (!Debugger.IsAttached)
                {
                    Console.WriteLine(InternalLocalizers.General["PRESS_ENTER_KEY"]!);
                    Console.Read();
                }
            }
        }

        public static IHostBuilder CreateHostBuilder() =>
            Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder()
                .ConfigureModules()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}