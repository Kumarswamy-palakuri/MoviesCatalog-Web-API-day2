using Microsoft.EntityFrameworkCore;
using MovieCatalogApi.Models;

namespace MovieCatalogApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Movie> Movies => Set<Movie>();
    public DbSet<Director> Directors => Set<Director>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Relationships
        modelBuilder.Entity<Movie>()
            .HasOne(m => m.Director)
            .WithMany(d => d.Movies)
            .HasForeignKey(m => m.DirectorId)
            .OnDelete(DeleteBehavior.Cascade);

        // Seed sample data (optional)
        modelBuilder.Entity<Director>().HasData(
            new Director { Id = 1, Name = "Christopher Nolan", Bio = "Known for complex narratives." },
            new Director { Id = 2, Name = "Greta Gerwig", Bio = "Acclaimed director and writer." }
        );

        modelBuilder.Entity<Movie>().HasData(
            new Movie { Id = 1, Title = "Inception", ReleaseYear = 2010, DirectorId = 1 },
            new Movie { Id = 2, Title = "Oppenheimer", ReleaseYear = 2023, DirectorId = 1 },
            new Movie { Id = 3, Title = "Lady Bird", ReleaseYear = 2017, DirectorId = 2 }
        );
    }
}
