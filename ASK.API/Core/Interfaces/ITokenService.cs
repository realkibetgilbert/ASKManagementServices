using ASK.API.Core.Entities;

namespace ASK.API.Core.Interfaces
{
    public interface ITokenService
    {
        string CreateJwtToken(AuthUser user, List<string> roles);
    }
}
