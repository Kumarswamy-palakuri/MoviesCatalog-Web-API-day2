using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieCatalogApi.Data;
using MovieCatalogApi.DTOs;
using MovieCatalogApi.Models;

namespace MovieCatalogApi.Controllers;

[ApiController]
[Route("api/directors")]
public class DirectorsController : ControllerBase
{
    private readonly AppDbContext _db;
    public DirectorsController(AppDbContext db) => _db = db;

    // GET: api/directors
    [HttpGet]
    public async Task<ActionResult<IEnumerable<DirectorReadDto>>> GetDirectors()
    {
        var list = await _db.Directors
            .AsNoTracking()
            .Select(d => new DirectorReadDto
            {
                Id = d.Id,
                Name = d.Name,
                Bio = d.Bio,
                MoviesCount = d.Movies.Count
            })
            .ToListAsync();

        return Ok(list);
    }

    // GET: api/directors/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<DirectorReadDto>> GetDirectorById(int id)
    {
        var director = await _db.Directors
            .Include(d => d.Movies)
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.Id == id);

        if (director == null) return NotFound(new { message = "Director not found" });

        var dto = new DirectorReadDto
        {
            Id = director.Id,
            Name = director.Name,
            Bio = director.Bio,
            MoviesCount = director.Movies.Count
        };

        return Ok(dto);
    }

    // POST: api/directors
    [HttpPost]
    public async Task<ActionResult<DirectorReadDto>> CreateDirector([FromBody] DirectorCreateDto dto)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);

        var director = new Director
        {
            Name = dto.Name,
            Bio = dto.Bio
        };

        _db.Directors.Add(director);
        await _db.SaveChangesAsync();

        var readDto = new DirectorReadDto
        {
            Id = director.Id,
            Name = director.Name,
            Bio = director.Bio,
            MoviesCount = 0
        };

        return CreatedAtAction(nameof(GetDirectorById), new { id = director.Id }, readDto);
    }

    // PUT: api/directors/5
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateDirector(int id, [FromBody] DirectorUpdateDto dto)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);

        var director = await _db.Directors.FindAsync(id);
        if (director == null) return NotFound(new { message = "Director not found" });

        director.Name = dto.Name;
        director.Bio = dto.Bio;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    // DELETE: api/directors/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteDirector(int id)
    {
        var director = await _db.Directors
            .Include(d => d.Movies)
            .FirstOrDefaultAsync(d => d.Id == id);

        if (director == null) return NotFound(new { message = "Director not found" });

        // Cascade delete: movies will be removed due to FK OnDelete.Cascade
        _db.Directors.Remove(director);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    // GET: api/directors/5/movies
    [HttpGet("{directorId:int}/movies")]
    public async Task<ActionResult<IEnumerable<MovieReadDto>>> GetMoviesByDirector(int directorId)
    {
        var directorExists = await _db.Directors.AnyAsync(d => d.Id == directorId);
        if (!directorExists) return NotFound(new { message = "Director not found" });

        var movies = await _db.Movies
            .AsNoTracking()
            .Where(m => m.DirectorId == directorId)
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
}
