using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Configuration;
using System;
using System.IO;

namespace FatCamel.Host.Core
{
    sealed class SectionSettings : ILoggerSettings
    {
        class FilteredContexts
        {
            public string[]? Enabled { get; set; }
            public string[]? Ignored { get; set; }
        }

        public SectionSettings(IConfiguration configuration)
        {
            var section = configuration.GetSection("Project:Filtered Contexts").Get<FilteredContexts>();
            LogSettings.SourceSwitches = section?.Enabled?.Select(s => new LogSwitch(s)).ToArray();
            LogSettings.IgnoredContexts = section?.Ignored;

            try
            {
                if (File.Exists(LogSettings.FilePath))
                    LogSettings.LoadAsync(File.OpenRead(LogSettings.FilePath)).Wait();
            }
            catch (Exception ex)
            {
                Util.StartupLogger.LogError(ex, "Ошибка загрузки натроек журналирования");
            }
        }

        public void Configure(LoggerConfiguration loggerConfiguration)
        {
            var cfgMinLvl = loggerConfiguration.MinimumLevel;
            cfgMinLvl.ControlledBy(LogSettings.MinimumLevelSwitch);
            if (LogSettings.SourceSwitches != null)
                foreach (var @switch in LogSettings.SourceSwitches)
                    cfgMinLvl.Override(@switch.Key, @switch);
        }
    }
}