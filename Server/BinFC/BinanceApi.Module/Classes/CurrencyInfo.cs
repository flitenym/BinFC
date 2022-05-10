namespace BinanceApi.Module.Classes
{
    public class CurrencyInfo
    {
        public CurrencyInfo(string asset, bool isSell, bool isDust)
        {
            Asset = asset;
            IsSell = isSell;
            IsDust = isDust;
        }
        public string Asset { get; set; }
        public bool IsSell { get; set; }
        public bool IsDust { get; set; }
        public bool IsSuccess => IsSell || IsDust;
    }
}