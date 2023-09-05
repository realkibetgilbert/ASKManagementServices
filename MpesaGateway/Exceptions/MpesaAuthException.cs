#nullable disable
namespace Gateway.Mpesa.Exceptions
{
    public class MpesaAuthException : Exception
    {
        public MpesaAuthException(string message) : base(message) { }
    }
}