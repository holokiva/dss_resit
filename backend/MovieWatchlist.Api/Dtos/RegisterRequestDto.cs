using System.ComponentModel.DataAnnotations;

namespace MovieWatchlist.Api.Dtos;

public sealed class RegisterRequestDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(6)]
    public string Password { get; set; } = string.Empty;

    [Required]
    [MinLength(6)]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Required]
    [StringLength(40, MinimumLength = 2)]
    public string DisplayName { get; set; } = string.Empty;
}
