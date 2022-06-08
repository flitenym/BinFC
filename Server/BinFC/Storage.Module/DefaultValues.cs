using Storage.Module.Classes;
using System.Collections.Generic;

namespace Storage.Module
{
    public static class DefaultValues
    {
        public static string AdminName = "admin";
        public static string AdminPassword = "1111";
        public static int UniqueId = 1;
        public static string UniqueName = "Обычный пользователь";
        public static string Cron = "* * * * *";
        public static string SellCurrency = nameof(CurrencyType.USDT);
        public static string SpotPercent = "41";
        public static string FuturesPercent = "30";
        public static List<string> Languages = new() { "ru", "en" };
    }
}