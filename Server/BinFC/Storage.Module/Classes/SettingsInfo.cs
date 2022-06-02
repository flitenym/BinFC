using Storage.Module.Entities;
using Storage.Module.StaticClasses;
using System.Collections.Generic;
using System.Linq;

namespace Storage.Module.Classes
{
    public class SettingsInfo
    {
        public string ApiKey { get; set; }
        public string ApiSecret { get; set; }
        public string CronExpression { get; set; }
        public string SellCurrency { get; set; }
        public bool IsNotification { get; set; }
        public bool BinanceSellEnable { get; set; }
        public string NotificationNames { get; set; }        
        public int SpotPercent { get; set; }
        public int FuturesPercent { get; set; }


        public List<long> AdminsChatId { get; set; } = new();

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

        public List<string> GetNotificationNames(string notificationNames)
        {
            List<string> names = new();
            List<string> repaired = new();
            for (int i = 0; i < notificationNames.Length; i++)
            {
                if (char.IsPunctuation(notificationNames[i]))
                {
                    names = notificationNames.Split(',').Select(x => x.Trim()).ToList();
                    break;
                }
            }

            if (!names.Any())
            {
                names.Add(notificationNames);
            }

            foreach (var notificationName in names)
            {
                if (notificationName.StartsWith('@'))
                {
                    repaired.Add(notificationName[1..]);
                    continue;
                }
            }

            return repaired;
        }

        public void SetFieldsBySettings(List<Settings> settings)
        {
            foreach(Settings item in settings)
            {
                switch (item.Key)
                {
                    case SettingsKeys.ApiKey:
                        {
                            ApiKey = item.Value;
                            break;
                        }
                    case SettingsKeys.ApiSecret:
                        {
                            ApiSecret = item.Value;
                            break;
                        }
                    case SettingsKeys.CronExpression:
                        {
                            CronExpression = item.Value;
                            break;
                        }
                    case SettingsKeys.SellCurrency:
                        {
                            SellCurrency = item.Value;
                            break;
                        }
                    case SettingsKeys.IsNotification:
                        {
                            IsNotification = bool.Parse(item.Value);
                            break;
                        }
                    case SettingsKeys.BinanceSellEnable:
                        {
                            BinanceSellEnable = bool.Parse(item.Value);
                            break;
                        }
                    case SettingsKeys.SpotPercent:
                        {
                            SpotPercent = int.Parse(item.Value);
                            break;
                        }
                    case SettingsKeys.FuturesPercent:
                        {
                            FuturesPercent = int.Parse(item.Value);
                            break;
                        }
                    case SettingsKeys.NotificationNames:
                        {
                            NotificationNames = item.Value;
                            break;
                        }
                }
            }
        }
    }
}