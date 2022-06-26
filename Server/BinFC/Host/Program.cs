using System;
using System.Diagnostics;
using HostLibrary.Core;
using HostLibrary.StaticClasses;
using HostLibrary;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

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
            Initial.CreateHostBuilder()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}