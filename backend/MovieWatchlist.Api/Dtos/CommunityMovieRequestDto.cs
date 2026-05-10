using System.ComponentModel.DataAnnotations;

namespace MovieWatchlist.Api.Dtos;

public sealed class CommunityMovieRequestDto : IValidatableObject
{
    [Required]
    [StringLength(120, MinimumLength = 1)]
    public string Title { get; set; } = string.Empty;

    [StringLength(80)]
    public string? Director { get; set; }

    [StringLength(40)]
    public string? Genre { get; set; }

    [StringLength(1500)]
    public string? Description { get; set; }

    public int ReleaseYear { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var maxYear = DateTime.UtcNow.Year + 2;
        if (ReleaseYear < 1888 || ReleaseYear > maxYear)
        {
            yield return new ValidationResult(
                $"ReleaseYear must be between 1888 and {maxYear}.",
                [nameof(ReleaseYear)]);
        }
    }
}
