#nullable disable
namespace Gateway.Mpesa.Models
{
    public class TokenResult
    {
        public string access_token { get; set; }
        public int expires_in { get; set; }
    }
}