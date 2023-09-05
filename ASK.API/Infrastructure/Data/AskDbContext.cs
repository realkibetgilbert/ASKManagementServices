using ASK.API.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ASK.API.Infrastructure.Data
{
    public class AskDbContext : IdentityDbContext<AuthUser, IdentityRole<long>, long>
    {
        public AskDbContext(DbContextOptions<AskDbContext> options) : base(options)
        {
        }
    }
}
