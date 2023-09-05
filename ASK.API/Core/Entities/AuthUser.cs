using Microsoft.AspNetCore.Identity;

namespace ASK.API.Core.Entities
{
    public class AuthUser:IdentityUser<long>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool IsActive { get; set; }
        public string? PasswordResetToken { get; set; }
        public DateTimeOffset? PasswordResetTokenExpiration { get; set; }
    }
}
