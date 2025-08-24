using System.ComponentModel.DataAnnotations;

namespace MovieCatalogApi.DTOs;

public class DirectorReadDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Bio { get; set; }
    public int MoviesCount { get; set; }
}

public class DirectorCreateDto
{
    [Required, StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Bio { get; set; }
}

public class DirectorUpdateDto
{
    [Required, StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Bio { get; set; }
}
