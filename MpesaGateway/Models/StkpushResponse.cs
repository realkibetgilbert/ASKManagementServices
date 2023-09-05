#nullable disable
namespace Gateway.Mpesa.Models
{
    public class StkpushResponse
    {
        public string MerchantRequestId { get; set; }
        public string CheckoutRequestId { get; set; }
        public string ResponseCode { get; set; }
        public string ResponseDescription { get; set; }
        public string CustomerMessage { get; set; }
    }
}