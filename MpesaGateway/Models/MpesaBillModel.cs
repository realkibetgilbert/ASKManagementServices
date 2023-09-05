#nullable disable
namespace Gateway.Mpesa.Models
{
    public class MpesaBillModel
    {
        public string PhoneNumber { get; set; }
        public int Amount { get; set; }
        public string BillRef { get; set; }
    }
}