using System.ComponentModel.DataAnnotations;

namespace MongoDbCrudOperations.Dto;

public sealed record UpdateBookDto
{
    [Required]
    [StringLength(200, MinimumLength = 1)]
    public string Title { get; set; } = string.Empty;

    [StringLength(100, MinimumLength = 1)]
    public string? Author { get; set; }

    [StringLength(1000)]
    public string? Description { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Page count must be greater than 0")]
    public int? PageCount { get; set; }

    public DateTime? PublishDate { get; set; }
}