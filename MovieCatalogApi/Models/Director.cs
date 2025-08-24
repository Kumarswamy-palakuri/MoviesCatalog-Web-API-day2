using System.ComponentModel.DataAnnotations;

namespace MovieCatalogApi.Models;

public class Director
{
    public int Id { get; set; }

    [Required, StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Bio { get; set; }

    // Navigation
    public List<Movie> Movies { get; set; } = new();
}
