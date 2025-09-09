using System.ComponentModel.DataAnnotations;

namespace MongoDbCrudOperations.Dto;

public sealed record CreateBookDto
{
    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string Author { get; init; } = string.Empty;

    [Required]
    [StringLength(200, MinimumLength = 1)]
    public string Title { get; init; } = string.Empty;

    [StringLength(1000)]
    public string Description { get; init; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "Page count must be greater than 0")]
    public int PageCount { get; init; }

    public DateTime PublishDate { get; init; }
}
