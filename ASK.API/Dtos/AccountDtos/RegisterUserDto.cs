using System.ComponentModel.DataAnnotations;

namespace ASK.API.Dtos.AccountDtos
{
    public class RegisterUserDto
    {
        [Required(ErrorMessage = "First Name is Required")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is Required")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Phone is Required")]
        public string PhoneNumber { get; set; }
        [Required(ErrorMessage = "Email is Required"),EmailAddress]
        public string Email { get; set; }
        [Required(ErrorMessage = "Username is required")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required,Compare("Password")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
        public bool IsActive { get; set; }
        public string[] Roles { get; set; }
    }
}
