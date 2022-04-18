using FatCamel.Host.Core;
using FatCamel.Host.StaticClasses;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FatCamel.Host.Extensions
{
    public static class WebHostBuilderExtensions
    {
        /// <summary>
        /// Настройка <see cref="IWebHostEnvironment.WebRootFileProvider"/> для использования статических файлов предоставляемых модулем
        /// </summary>
        /// <param name="builder"><see cref="IWebHostBuilder"/></param>
        /// <returns><see cref="IWebHostBuilder"/></returns>
        public static IWebHostBuilder UseModulesWebAssets(this IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration((context, configBuilder) =>
            {
                List<string> addedPaths = new List<string>();
                List<IFileProvider> providers = new List<IFileProvider>();
                var modules = StartupManager.Graph.Where(m => !string.IsNullOrWhiteSpace(m.Metadata.WebAssetsPrefix)).OrderBy(m => m.Metadata.Order);

                if (modules?.Any() != true) return;

                foreach (var module in modules)
                {
                    var pfx = module.Metadata.WebAssetsPrefix!;

                    if (addedPaths.Contains(pfx)) continue;
                    addedPaths.Add(pfx);
                    providers.Add(new ModuleWebAssetsFileProvider(module.Metadata.WebAssetsPrefix!, Path.Combine(module.Metadata.ModulePath!, "wwwroot")));

                    StartupLogger.LogInformation("Добавлен обработчик стаических фалов для пути '{0}' из модуля '{1}'", pfx, module.Name);
                }

                var webRootFileProvider = context.HostingEnvironment.WebRootFileProvider;
                providers.Insert(0, webRootFileProvider);
                context.HostingEnvironment.WebRootFileProvider = new CompositeFileProvider(providers);
            });

            return builder;
        }
    }
}