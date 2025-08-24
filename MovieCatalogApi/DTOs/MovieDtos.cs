using System.ComponentModel.DataAnnotations;

namespace MovieCatalogApi.DTOs;

public class MovieReadDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int ReleaseYear { get; set; }
    public int DirectorId { get; set; }
    public string? DirectorName { get; set; }
}

public class MovieCreateDto
{
    [Required, StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [Range(1888, 3000)]
    public int ReleaseYear { get; set; }

    [Required]
    public int DirectorId { get; set; }
}

public class MovieUpdateDto
{
    [Required, StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [Range(1888, 3000)]
    public int ReleaseYear { get; set; }

    [Required]
    public int DirectorId { get; set; }
}
