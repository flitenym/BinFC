using System;
using System.Diagnostics;
using Host.Core;
using Host.Extensions;
using Host.StaticClasses;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Web;

namespace Host
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
                })
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                })
                .UseNLog();
    }
}