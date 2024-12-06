using horolog_api.Features.Users;

namespace horolog_api.Features.Tokens;

public interface ITokenService
{
    string CreateToken(User user);
}