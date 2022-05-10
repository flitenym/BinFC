using System.Collections.Generic;

namespace Storage.Module.StaticClasses
{
    public static class SettingsKeys
    {
        public const string ApiKey = nameof(ApiKey);
        public const string ApiSecret = nameof(ApiSecret);
        public const string CronExpression = nameof(CronExpression);
        public const string SellCurrency = nameof(SellCurrency);

        public static List<string> Settings = new()
        {
            ApiKey,
            ApiSecret,
            CronExpression,
            SellCurrency
        };
    }
}