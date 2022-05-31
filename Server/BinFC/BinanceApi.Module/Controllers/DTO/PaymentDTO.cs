namespace BinanceApi.Module.Controllers.DTO
{
    public class PaymentDTO
    {
        public long UserId { get; set; }
        public string UserName { get; set; }
        public decimal Usdt { get; set; }
        public string TrcAddress { get; set; }
        public string BepAddress { get; set; }
    }
}