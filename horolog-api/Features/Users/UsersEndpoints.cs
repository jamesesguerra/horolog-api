using System.Security.Cryptography;
using System.Text;
using horolog_api.Features.Tokens;
using Microsoft.AspNetCore.Authorization;

namespace horolog_api.Features.Users;

public static class UsersEndpoints
{
    public static IEndpointRouteBuilder MapUsers(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("api/users")
            .WithTags("Users")
            .WithOpenApi();

        group.MapPost("/register", [AllowAnonymous] async (
            ITokenService tokenService, IUsersService service, [AsParameters] AccountDto newUser) =>
        {
            using var hmac = new HMACSHA512();

            var user = new User()
            {
                Username = newUser.Username.ToLower(),
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(newUser.Password)),
                PasswordSalt = hmac.Key
            };

            await service.AddUser(user);
            return new UserDto()
            {
                Username = user.Username,
                Token = tokenService.CreateToken(user)
            };
        });

        group.MapPost("/login", [AllowAnonymous] async (
            ITokenService tokenService, IUsersService service, AccountDto credentials) =>
        {
            var user = await service.GetUserByUsername(credentials.Username.ToLower());

            if (user is null) return TypedResults.BadRequest("Invalid username");

            using var hmac = new HMACSHA512(user.PasswordSalt);

            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(credentials.Password));

            if (computedHash.Where((t, i) => t != user.PasswordHash[i]).Any()) return TypedResults.Unauthorized();

            return Results.Ok(new UserDto()
            {
                Username = user.Username,
                Token = tokenService.CreateToken(user)
            });
        });

        return group;
    }
}