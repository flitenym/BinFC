using Storage.Module.Classes;

namespace Storage.Module
{
    public static class DefaultValues
    {
        public static string AdminName = "admin";
        public static string AdminPassword = "1111";
        public static int UniqueId = 0;
        public static string UniqueName = "Обычный пользователь";
        public static string Cron = "* * * * *";
        public static string SellCurrency = nameof(CurrencyType.USDT);
    }
}