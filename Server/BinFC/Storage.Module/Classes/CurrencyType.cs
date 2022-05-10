namespace Storage.Module.Classes
{
    public enum CurrencyType
    {
        USDT,
        BUSD
    }

    public static class CurrencyTypeConverter
    {
        public static CurrencyType ConvertStringToCurrencyType(string currencyType) =>
            currencyType switch
            {
                nameof(CurrencyType.USDT) => CurrencyType.USDT,
                nameof(CurrencyType.BUSD) => CurrencyType.BUSD,
                _ => CurrencyType.USDT,
            };
    }
}