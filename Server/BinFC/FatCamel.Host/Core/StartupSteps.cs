using FatCamel.Host.Enums;
using System.Collections.Generic;
using System.Linq;

namespace FatCamel.Host.Core
{
    public class StartupSteps
    {
        /// <summary>
        /// Список плагинов иницилизируемых до выполнения метода ConfigureServices
        /// </summary>
        public List<StartupDescription> PreConfigureServices { get; set; }

        /// <summary>
        /// Список плагинов иницилизируемых во время выполнения метода ConfigureServices
        /// </summary>
        public List<StartupDescription> ConfigureServices { get; set; }

        /// <summary>
        /// Список плагинов иницилизируемых во время выполнения метода Configure
        /// </summary>
        public List<StartupDescription> Configure { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<StartupDescription> ApplicationStart { get; set; }

        public IEnumerable<StartupDescription> GetComponents(StartupStages stage)
        {
            IEnumerable<StartupDescription> result = null;
            switch (stage)
            {
                case StartupStages.PreConfigureServices:
                    result = PreConfigureServices;
                    break;
                case StartupStages.ConfigureServices:
                    result = ConfigureServices;
                    break;
                case StartupStages.Configure:
                    result = Configure;
                    break;
                case StartupStages.ApplicationStart:
                    result = ApplicationStart;
                    break;
            }

            return result ?? Enumerable.Empty<StartupDescription>();
        }
    }
}
