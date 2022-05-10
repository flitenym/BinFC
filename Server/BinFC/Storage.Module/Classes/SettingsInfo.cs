namespace Storage.Module.Classes
{
    public class SettingsInfo
    {
        public string ApiKey { get; set; }
        public string ApiSecret { get; set; }
        public string CronExpression { get; set; }
        public string SellCurrency { get; set; }

        public (bool IsValid, string ValidError) IsValid()
        {
            if (string.IsNullOrEmpty(ApiKey))
            {
                return (false, "ApiKey не задан");
            }

            if (string.IsNullOrEmpty(ApiSecret))
            {
                return (false, "ApiSecret не задан");
            }

            if (string.IsNullOrEmpty(SellCurrency))
            {
                return (false, "Валюта для продажи не задана");
            }

            return (true, null);
        }
    }
}