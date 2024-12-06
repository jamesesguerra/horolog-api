using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using horolog_api.Features.Users;
using Microsoft.IdentityModel.Tokens;

namespace horolog_api.Features.Tokens;

public class TokenService(IConfiguration config) : ITokenService
{
    public string CreateToken(User user)
    {
        var tokenKey = config["TokenKey"] ?? throw new Exception("Cannot access token key");
        if (tokenKey.Length < 64) throw new Exception("Token key needs to be longer than 64 characters");
        
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey));

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Username)
        };
        
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = credentials
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        
        return tokenHandler.WriteToken(token);
    }
}