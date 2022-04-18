using FatCamel.Host.Enums;
using FatCamel.Host.StaticClasses;
using Microsoft.Extensions.Localization;
using System.Reflection;

namespace FatCamel.Host.Core
{
    public class StartupDescription
    {
        private readonly IStringLocalizer _localizer = InternalLocalizers.General;

        /// <summary>
        /// Простое название сборки
        /// </summary>
        public string Assembly { get; set; } = string.Empty;

        /// <summary>
        /// Полное название типа
        /// </summary>
        public string Type { get; set; } = string.Empty;

        /// <summary>
        /// Название метода для вызова
        /// </summary>
        public string Method { get; set; } = string.Empty;

        public void Init(StartupStages stage)
        {
            var asm = AssemblyLoadContext.Default.Assemblies.FirstOrDefault(a => (a.GetName().Name == Assembly));
            if (asm == null)
                throw new PluginInitException(_localizer["MISSING_ASSEMBLY", Assembly, Type, Method, stage]);
            var type = asm.GetType(Type, false);
            if (type == null)
                throw new PluginInitException(_localizer["MISSING_TYPE", Assembly, Type, Method, stage]);
            var mtd = type.GetMethod(Method, BindingFlags.Static | BindingFlags.Public);
            if (mtd == null)
                throw new PluginInitException(_localizer["MISSING_METHOD", Assembly, Type, Method, stage]);

            StartupLogger.LogInformation(_localizer["PLUGIN_INIT_CALL", Assembly, Type, Method, stage]);
            mtd.Invoke(null, null);
        }
    }
}