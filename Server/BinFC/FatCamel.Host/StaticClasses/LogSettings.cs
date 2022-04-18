using Serilog.Core;
using Serilog.Events;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace FatCamel.Host.StaticClasses
{
    public static class LogSettings
    {
        /// <summary>
        /// Путь к файлу с настройками журналирования
        /// </summary>
        public static string FilePath => Path.Combine("App_Data", "ProjectLogSettings.txt");

        /// <summary>
        /// Минимальный уровень журналирования
        /// </summary>
        public static LoggingLevelSwitch MinimumLevelSwitch { get; } = new LoggingLevelSwitch(LogEventLevel.Information);

        /// <string>
        /// Фильтры для пользователей
        /// </string>
        public static List<LogFilterSettings>? Filters { get; set; }

        /// <summary>
        /// Список ключей для управления уровнем журналирования для контекстов
        /// </summary>
        public static LogSwitch[]? SourceSwitches { get; set; }

        /// <summary>
        /// Список названий контекстов для которых нужно игнорировать фильтрацию
        /// </summary>
        public static string[]? IgnoredContexts { get; set; }

        /// <summary>
        /// Загрузка настроек журналирования из потока
        /// </summary>
        /// <param name="stream">Поток с данными настроек</param>
        /// <exception cref="JsonException"></exception>
        public static async Task LoadAsync(Stream stream)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                IncludeFields = true
            };

            options.Converters.Add(new LoggingLevelSwitchJsonConverter());

            using (var reader = new StreamReader(stream))
            {
                var data = await reader.ReadToEndAsync();
                var res = JsonSerializer.Deserialize(data, typeof((LoggingLevelSwitch?, List<LogFilterSettings>?)), options) as (LoggingLevelSwitch?, List<LogFilterSettings>?)?;

                if (res.HasValue)
                {
                    MinimumLevelSwitch.MinimumLevel = res.Value.Item1?.MinimumLevel ?? LogEventLevel.Information;
                    Filters = res.Value.Item2;
                    if (Filters != null)
                        foreach (var filter in Filters)
                        {
                            filter.Contexts = filter.Contexts?.Where(c => SourceSwitches?.Any(ss => ss.Key == c.Key) == true).ToList();
                            if (!string.IsNullOrWhiteSpace(filter.Expression) && SerilogExpression.TryCompile(filter.Expression, out var cmp, out var error))
                                filter.Compiled = cmp;
                        }

                    SetSwitches(Filters?.Where(f => f.Contexts != null).SelectMany(f => f.Contexts!).GroupBy(c => c.Key).ToDictionary(g => g.Key, g => g.Min(c => c.Level)));
                }
            }
        }

        /// <summary>
        /// Запись настроек журналирования в переданный поток
        /// </summary>
        /// <param name="stream">Поток данных в который надо записать настройки</param>
        public static async Task SaveAsync(Stream stream)
        {
            (LoggingLevelSwitch MinimumLevel, List<LogFilterSettings>? Filters) values = (MinimumLevelSwitch, Filters);

            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                IncludeFields = true
            };

            options.Converters.Add(new LoggingLevelSwitchJsonConverter());

            var res = JsonSerializer.Serialize(values, options);

            using (StreamWriter writer = new StreamWriter(stream, leaveOpen: true))
                await writer.WriteLineAsync(res);
        }

        /// <summary>
        /// Изменяет уровень журналирования для источников данных
        /// </summary>
        /// <param name="minLevels">Набор названий контекстов и уровней журналирования</param>
        public static void SetSwitches(IDictionary<string, LogEventLevel>? minLevels)
        {
            if (SourceSwitches == null) return;

            foreach (var @switch in SourceSwitches)
            {
                if (minLevels != null && minLevels.TryGetValue(@switch.Key, out var lvl))
                    @switch.MinimumLevel = lvl;
                else
                    @switch.MinimumLevel = LogEventLevel.Fatal;
            }
        }
    }
}