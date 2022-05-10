using System.Collections.Generic;

namespace Storage.Module.StaticClasses
{
    public static class SettingsKeys
    {
        public const string ApiKey = nameof(ApiKey);
        public const string ApiSecret = nameof(ApiSecret);
        public const string Emails = nameof(Emails);
        public const string EmailLogin = nameof(EmailLogin);
        public const string EmailPassword = nameof(EmailPassword);
        public const string CronExpression = nameof(CronExpression);
        public const string SellCurrency = nameof(SellCurrency);

        public static List<string> Settings = new()
        {
            ApiKey,
            ApiSecret,
            Emails,
            EmailLogin,
            EmailPassword,
            CronExpression,
            SellCurrency
        };
    }
}