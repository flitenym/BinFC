using System.Collections.Generic;

namespace Storage.Module.StaticClasses
{
    public static class SettingsKeys
    {
        public const string ApiKey = nameof(ApiKey);
        public const string CronExpression = nameof(CronExpression);

        public static List<string> Settings = new()
        {
            ApiKey,
            CronExpression
        };
    }
}