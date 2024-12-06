using System.ComponentModel.DataAnnotations;

namespace horolog_api.Features.Users;

public class AccountDto
{
    [Required]
    public required string Username { get; set; }
    
    [Required]
    public required string Password { get; set; }
}