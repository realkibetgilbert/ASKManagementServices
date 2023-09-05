#nullable disable
namespace Gateway.Mpesa.Models;

public class StkInitiationPayload
{
    public string BusinessShortCode { get; set; }
    public string Password { get; set; }
    public string Timestamp { get; set; }
    public string TransactionType { get; set; }
    public string Amount { get; set; }
    public string PartyA { get; set; }
    public string PartyB { get; set; }
    public string PhoneNumber { get; set; }
    public string CallBackURL { get; set; }
    public string AccountReference { get; set; }
    public string TransactionDesc { get; set; }
}