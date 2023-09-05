using System.ComponentModel.DataAnnotations;

namespace ASK.API.Dtos.AccountDtos
{
    public class RoleDto
    {
        [Required]
        public string Name { get; set; }
    }
}
