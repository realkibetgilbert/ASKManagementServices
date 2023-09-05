#nullable disable
namespace Gateway.Mpesa.Models
{
    public class StkCallback
    {
        public string MerchantRequestId { get; set; }
        public string CheckoutRequestId { get; set; }
        public int ResultCode { get; set; } = 1;
        public string ResultDesc { get; set; }
        public CallbackMetadata CallbackMetadata { get; set; }
    }
}