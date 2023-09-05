#nullable disable
namespace Gateway.Mpesa.Models
{
    public class StkPushRequestError
    {
        public string RequestId { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
    }
}