using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieCatalogApi.Data;
using MovieCatalogApi.DTOs;
using MovieCatalogApi.Models;

namespace MovieCatalogApi.Controllers;

[ApiController]
[Route("api/movies")]
public class MoviesController : ControllerBase
{
    private readonly AppDbContext _db;
    public MoviesController(AppDbContext db) => _db = db;

    // GET: api/movies
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MovieReadDto>>> GetMovies()
    {
        var movies = await _db.Movies
            .AsNoTracking()
            .Include(m => m.Director)
            .Select(m => new MovieReadDto
            {
                Id = m.Id,
                Title = m.Title,
                ReleaseYear = m.ReleaseYear,
                DirectorId = m.DirectorId,
                DirectorName = m.Director != null ? m.Director.Name : null
            })
            .ToListAsync();

        return Ok(movies);
    }

    // GET: api/movies/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<MovieReadDto>> GetMovieById(int id)
    {
        var movie = await _db.Movies
            .Include(m => m.Director)
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id);

        if (movie == null) return NotFound(new { message = "Movie not found" });

        var dto = new MovieReadDto
        {
            Id = movie.Id,
            Title = movie.Title,
            ReleaseYear = movie.ReleaseYear,
            DirectorId = movie.DirectorId,
            DirectorName = movie.Director?.Name
        };
        return Ok(dto);
    }

    // POST: api/movies
    [HttpPost]
    public async Task<ActionResult<MovieReadDto>> CreateMovie([FromBody] MovieCreateDto dto)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);

        var directorExists = await _db.Directors.AnyAsync(d => d.Id == dto.DirectorId);
        if (!directorExists)
            return NotFound(new { message = $"Director {dto.DirectorId} not found" });

        var movie = new Movie
        {
            Title = dto.Title,
            ReleaseYear = dto.ReleaseYear,
            DirectorId = dto.DirectorId
        };

        _db.Movies.Add(movie);
        await _db.SaveChangesAsync();

        // Load director name for response
        await _db.Entry(movie).Reference(m => m.Director).LoadAsync();

        var readDto = new MovieReadDto
        {
            Id = movie.Id,
            Title = movie.Title,
            ReleaseYear = movie.ReleaseYear,
            DirectorId = movie.DirectorId,
            DirectorName = movie.Director?.Name
        };

        return CreatedAtAction(nameof(GetMovieById), new { id = movie.Id }, readDto);
    }

    // PUT: api/movies/5
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateMovie(int id, [FromBody] MovieUpdateDto dto)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);

        var movie = await _db.Movies.FindAsync(id);
        if (movie == null) return NotFound(new { message = "Movie not found" });

        var directorExists = await _db.Directors.AnyAsync(d => d.Id == dto.DirectorId);
        if (!directorExists)
            return NotFound(new { message = $"Director {dto.DirectorId} not found" });

        movie.Title = dto.Title;
        movie.ReleaseYear = dto.ReleaseYear;
        movie.DirectorId = dto.DirectorId;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    // DELETE: api/movies/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteMovie(int id)
    {
        var movie = await _db.Movies.FindAsync(id);
        if (movie == null) return NotFound(new { message = "Movie not found" });

        _db.Movies.Remove(movie);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
