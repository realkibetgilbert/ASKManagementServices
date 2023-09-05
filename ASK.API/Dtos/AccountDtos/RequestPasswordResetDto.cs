using System.ComponentModel.DataAnnotations;

namespace ASK.API.Dtos.AccountDtos
{
    public class RequestPasswordResetDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
