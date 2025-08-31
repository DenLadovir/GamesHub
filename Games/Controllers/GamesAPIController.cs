using System.Diagnostics;
using Games.Database;
using Games.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class GamesApiController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly ILogger<GamesApiController> _logger;

    public GamesApiController(AppDbContext db, ILogger<GamesApiController> logger)
    {
        _db = db;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Game>>> GetGames()
    {
        return await _db.Games.ToListAsync();
    }

    // API: Получение конкретной игры
    [HttpGet("{id}")]
    public async Task<ActionResult<Game>> GetGame(int id)
    {
        var game = await _db.Games.FindAsync(id);
        if (game == null)
        {
            return NotFound();
        }
        return game;
    }

    // API: Создание игры
    [HttpPost]
    public async Task<ActionResult<Game>> CreateGame([FromBody] Game game)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        game.ReleaseDate = DateTime.SpecifyKind(game.ReleaseDate, DateTimeKind.Utc);
        _db.Games.Add(game);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetGame), new { id = game.Id }, game);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateGame(int id, [FromBody] Game game)
    {
        if (id != game.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        game.ReleaseDate = DateTime.SpecifyKind(game.ReleaseDate, DateTimeKind.Utc);
        _db.Entry(game).State = EntityState.Modified;

        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_db.Games.Any(e => e.Id == id))
            {
                return NotFound();
            }
            throw;
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteGameApi(int id)
    {
        var game = await _db.Games.FindAsync(id);
        if (game == null)
        {
            _logger.LogWarning($"API: Игра с ID {id} не найдена");
            return NotFound();
        }

        try
        {
            _db.Games.Remove(game);
            await _db.SaveChangesAsync();
            _logger.LogInformation($"API: Игра {game.Title} (ID: {id}) удалена");
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"API: Ошибка при удалении игры ID {id}");
            return StatusCode(500, "Internal Server Error");
        }
    }
}