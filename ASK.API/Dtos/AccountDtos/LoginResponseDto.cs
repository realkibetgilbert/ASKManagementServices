namespace ASK.API.Dtos.AccountDtos
{
    public class LoginResponseDto
    {

        public long id { get; set; }
        public string userName { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }

        public string phoneNumber { get; set; }
        public string token { get; set; }
    }
}
