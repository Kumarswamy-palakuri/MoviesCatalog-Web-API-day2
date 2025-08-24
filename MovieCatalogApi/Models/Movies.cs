using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieCatalogApi.Models;

public class Movie
{
    public int Id { get; set; }

    [Required, StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [Range(1888, 3000)]
    public int ReleaseYear { get; set; }

    // FK
    [Required]
    public int DirectorId { get; set; }

    // Navigation
    public Director? Director { get; set; }
}
