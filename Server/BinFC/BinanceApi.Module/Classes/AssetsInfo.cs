namespace BinanceApi.Module.Classes
{
    public class AssetsInfo
    {
        public AssetsInfo(string fromAsset, string toAsset, decimal quantity, bool isDust)
        {
            FromAsset = fromAsset;
            ToAsset = toAsset;
            Quantity = quantity;
            IsDust = isDust;
        }
        public string FromAsset { get; set; }
        public string ToAsset { get; set; }
        public decimal Quantity { get; set; }
        public bool IsDust { get; set; }
    }
}