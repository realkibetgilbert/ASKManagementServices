#nullable disable
namespace Gateway.Mpesa.Models
{
	public class PaybillTillConfiguration
	{
		public string ShortCode { get; set; }
		public bool IsPayBill { get; set; }
		public string PartyB { get; set; }
		public string ClientKey { get; set; }
		public string ClientSecret { get; set; }
		public string PassKey { get; set; }
		public string CallbackUrl { get; set; }
		public string Initiator { get; set; }
		public string InitiatorPassword { get; set; }
		public string StatusResultUrl { get; set; }
		public string StatusTimeOutUrl { get; set; }
	}
}