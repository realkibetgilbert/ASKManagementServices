#nullable disable
namespace Gateway.Mpesa.Exceptions
{
    public class MpesaSystemException : Exception
    {
        public MpesaSystemException(string message) : base(message) { }
    }
}