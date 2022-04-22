﻿using FatCamel.Host.Core;
using FatCamel.Host.Core.Classes;
using FatCamel.Host.StaticClasses;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Linq;

namespace FatCamel.Host.Extensions
{
    public static class HostBuilderExtensions
    {
        private static IConfiguration ModifyConfiguration(IConfigurationBuilder configBuilder)
        {
            string settingsPath = null;

            var cfg = configBuilder.Build();    // Информация о пути стандартного appsettings.json будет доступна только если вызвать метод Build
             
            var prov = configBuilder.Sources
                .OfType<JsonConfigurationSource>()
                .FirstOrDefault(s => s.Path.StartsWith("appsettings.json"))?.FileProvider as PhysicalFileProvider;
            if (prov != null)
                settingsPath = Path.Combine(prov.Root, "appsettings.json");

            if (settingsPath != null)
                StartupLogger.LogInformation(InternalLocalizers.General["SETTINGS_PATH", settingsPath]);
            StartupManager.SettingsPath = settingsPath;
            return cfg;
        }

        /// <summary>
        /// Регистрация модулей в системе
        /// </summary>
        /// <param name="settingsPath">Путь к настройкам указанный в командной строке</param>
        public static IHostBuilder ConfigureModules(this IHostBuilder builder)
        {
            return builder.ConfigureAppConfiguration((context, configBuilder) =>
            {
                var cfg = ModifyConfiguration(configBuilder);

                var options = cfg.GetSection("Project:Hosting").Get<HostingOptions>();

                if (options == null)
                    throw new ArgumentNullException(nameof(options), InternalLocalizers.General["MISSING_SECTION", "Project:Hosting"]);

                StartupManager.Register(options, configBuilder);
            });
        }

        /// <summary>
        /// Добавляет файл настройки для тех. поддержки
        /// </summary>
        /// <remarks>Метод должен вызываться последним во время настроки хоста</remarks>
        public static IHostBuilder FinalConfiguration(this IHostBuilder builder)
        {
            return builder.ConfigureAppConfiguration((context, configBuilder) =>
            {
                var customPath = Path.Join(Path.GetDirectoryName(StartupManager.SettingsPath), "custom.appsettings.json");
                configBuilder.AddJsonFile(customPath, true, true);
            });
        }
    }
}