using System;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
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
        private static string _envName = "Production";

        private static string _settingsPath = null;

        private static readonly IStringLocalizer _localizer = InternalLocalizers.General;

        static int ParseArgs(string[] args)
        {
            var cmd = new RootCommand(_localizer["APP_DESC"]!)
            {
                new Option("--environment", _localizer["ENV_DESC"]!)
                {
                    Argument = new Argument<string>(_localizer["ENV_ARG_DESC"]!, () => "Production")
                },
                new Option("--config", _localizer["CONFIG_DESC"]!)
                {
                    Argument = new Argument<string>(_localizer["CONFIG_ARG_DESC"]!)
                },
                new Option<bool>("--no-start-log", _localizer["STSRT_LOG_DESC"]!)
            };

            cmd.TreatUnmatchedTokensAsErrors = false;

            cmd.Handler = CommandHandler.Create<string, string, bool>((environment, config, noStartOut) =>
            {
                _envName = environment;
                _settingsPath = config;

                if (noStartOut)
                    StartupLogger.Disable();

                return 1;
            });

            return new CommandLineBuilder(cmd).Build().Invoke(args);
        }

        public static void Main(string[] args)
        {
            try
            {
                if (ParseArgs(args) == 1)
                {
                    var host = CreateHostBuilder(args).Build();
                    var lifetime = host.Services.GetService(typeof(IHostApplicationLifetime)) as IHostApplicationLifetime;
                    lifetime?.ApplicationStarted.Register(() => StartupManager.InitComponents(StartupStages.ApplicationStart));
                    host.Run();
                }
            }
            catch (Exception ex)
            {
                StartupLogger.LogError(ex, _localizer["GLOBAL_ERROR"]);

                Console.WriteLine(_localizer["SERVER_STOP"]!);
                if (!(System.Environment.GetEnvironmentVariable("APP_POOL_ID") is string) && !Debugger.IsAttached)
                {
                    Console.WriteLine(_localizer["PRESS_ENTER_KEY"]!);
                    Console.Read();
                }
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(args)
                .ConfigureModules(_settingsPath)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseModulesWebAssets();
                    webBuilder.UseStartup<Startup>();
                })
                .FinalConfiguration();
    }
}