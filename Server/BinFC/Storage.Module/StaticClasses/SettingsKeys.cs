using System.Collections.Generic;

namespace Storage.Module.StaticClasses
{
    public static class SettingsKeys
    {
        public const string ApiKey = nameof(ApiKey);
        public const string ApiSecret = nameof(ApiSecret);
        public const string CronExpression = nameof(CronExpression);
        public const string SellCurrency = nameof(SellCurrency);
        public const string IsNotification = nameof(IsNotification);
        public const string NotificationNames = nameof(NotificationNames);
        public const string BinanceSellEnable = nameof(BinanceSellEnable);
        public const string SpotPercent = nameof(SpotPercent);
        public const string FuturesPercent = nameof(FuturesPercent);

        public static List<string> Settings = new()
        {
            ApiKey,
            ApiSecret,
            CronExpression,
            SellCurrency,
            IsNotification,
            NotificationNames,
            BinanceSellEnable,
            SpotPercent,
            FuturesPercent
        };
    }
}