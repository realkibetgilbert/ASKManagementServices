using System.ComponentModel.DataAnnotations;

namespace ASK.API.Dtos.AccountDtos
{
    public class RegisterUserDto
    {

        [Required(ErrorMessage = "FullName is Required")]
        public string FullName { get; set; }
        [Required(ErrorMessage = "Phone is Required"),Phone]
        public string PhoneNumber { get; set; }
        [Required(ErrorMessage = "Email is Required"),EmailAddress]
        public string Email { get; set; }
        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
