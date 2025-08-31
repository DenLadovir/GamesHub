using Games.Controllers;
using Games.Database;
using Games.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("Games")]
public class GamesController : Controller
{
    private readonly ILogger<GamesController> _logger;
    private readonly AppDbContext _db;

    public GamesController(ILogger<GamesController> logger, AppDbContext db)
    {
        _logger = logger;
        _db = db;
    }

    // Просмотр игры
    [HttpGet("{id}")]
    public async Task<IActionResult> ViewGame(int id)
    {
        var game = await _db.Games
            .Include(g => g.GameGenres)
                .ThenInclude(gg => gg.Genre)
            .FirstOrDefaultAsync(g => g.Id == id);

        if (game == null) return NotFound();
        return View(game);
    }

    // Добавление игры - GET
    [HttpGet("AddGame")]
    [Authorize(Policy = "ModeratorOrAdmin")]
    public async Task<IActionResult> AddGame()
    {
        await LoadGenresToViewBag();

        var model = new Game
        {
            SelectedGenreIds = new List<int>() // пустой список жанров
        };

        return View(model);
    }

    // Добавление игры - POST
    [HttpPost("AddGame")]
    [Authorize(Policy = "ModeratorOrAdmin")]
    public async Task<IActionResult> AddGame(Game game, int[] selectedGenres)
    {
        if (ModelState.IsValid)
        {
            game.ReleaseDate = DateTime.SpecifyKind(
                game.ReleaseDate == default ? DateTime.Today : game.ReleaseDate,
                DateTimeKind.Utc);

            _db.Games.Add(game);
            await _db.SaveChangesAsync();

            await SaveGameGenres(game.Id, selectedGenres);

            return RedirectToAction("ViewGame", new { id = game.Id });
        }

        await LoadGenresToViewBag();
        game.SelectedGenreIds = selectedGenres?.ToList() ?? new List<int>();
        return View(game);
    }

    // Добавление жанра
    [HttpGet("AddGenre")]
    [Authorize(Policy = "ModeratorOrAdmin")]
    public IActionResult AddGenre() => View();

    [HttpPost("AddGenre")]
    [Authorize(Policy = "ModeratorOrAdmin")]
    public async Task<IActionResult> AddGenre(Genre genre)
    {
        if (ModelState.IsValid)
        {
            _db.Genres.Add(genre);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }
        return View(genre);
    }

    // Редактирование игры - GET
    [HttpGet("Edit/{id}")]
    [Authorize(Policy = "ModeratorOrAdmin")]
    public async Task<IActionResult> EditGame(int id)
    {
        var game = await _db.Games
            .Include(g => g.GameGenres)
                .ThenInclude(gg => gg.Genre)
            .FirstOrDefaultAsync(g => g.Id == id);

        if (game == null) return NotFound();

        game.SelectedGenreIds = game.GameGenres.Select(gg => gg.GenreId).ToList();
        await LoadGenresToViewBag();
        return View(game);
    }

    // Редактирование игры - POST
    [HttpPost("Edit/{id}")]
    [Authorize(Policy = "ModeratorOrAdmin")]
    public async Task<IActionResult> EditGame(int id, Game updatedGame, int[] selectedGenres)
    {
        if (id != updatedGame.Id) return NotFound();

        if (ModelState.IsValid)
        {
            var game = await _db.Games
                .Include(g => g.GameGenres)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (game == null) return NotFound();

            game.Title = updatedGame.Title;
            game.Description = updatedGame.Description;
            game.ReleaseDate = DateTime.SpecifyKind(
                updatedGame.ReleaseDate == default ? DateTime.Today : updatedGame.ReleaseDate,
                DateTimeKind.Utc);

            // Обновляем жанры
            _db.Set<GameGenre>().RemoveRange(game.GameGenres);
            await _db.SaveChangesAsync();

            await SaveGameGenres(game.Id, selectedGenres);

            return RedirectToAction("ViewGame", new { id = game.Id });
        }

        await LoadGenresToViewBag();
        updatedGame.SelectedGenreIds = selectedGenres?.ToList() ?? new List<int>();
        return View(updatedGame);
    }

    // Удаление игры - GET
    [HttpGet("Delete/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteGame(int id)
    {
        var game = await _db.Games
            .Include(g => g.GameGenres)
                .ThenInclude(gg => gg.Genre)
            .FirstOrDefaultAsync(g => g.Id == id);

        if (game == null)
        {
            _logger.LogWarning($"Игра с ID {id} не найдена");
            return NotFound();
        }

        return View(game);
    }

    // Удаление игры - POST
    [HttpPost("Delete/{id}")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteGameConfirmed(int id)
    {
        var game = await _db.Games.FindAsync(id);
        if (game == null) return NotFound();

        _db.Games.Remove(game);
        await _db.SaveChangesAsync();
        return RedirectToAction("Index", "Home");
    }

    [HttpGet("Filter")]
    public async Task<IActionResult> Filtering(GameFilterViewModel filter)
    {
        var query = _db.Games
            .Include(g => g.GameGenres)
                .ThenInclude(gg => gg.Genre)
            .AsQueryable();

        // Фильтрация по названию
        if (!string.IsNullOrEmpty(filter.Title))
        {
            query = query.Where(g => g.Title.Contains(filter.Title));
        }

        // Фильтрация по жанрам
        if (filter.SelectedGenreIds != null && filter.SelectedGenreIds.Count > 0)
        {
            query = query.Where(g => g.GameGenres.Any(gg => filter.SelectedGenreIds.Contains(gg.GenreId)));
        }

        // Фильтрация по году
        if (filter.Year.HasValue)
        {
            //var startDate = new DateTime(filter.Year.Value, 1, 1);
            //var endDate = startDate.AddYears(1);

            //query = query.Where(g => g.ReleaseDate >= startDate && g.ReleaseDate < endDate);
            query = query.Where(g => g.ReleaseDate.Year == filter.Year.Value);
        }

        filter.Games = await query.ToListAsync();
        filter.AllGenres = await _db.Genres.ToListAsync();

        return View(filter);
    }

    #region Вспомогательные методы

    private async Task LoadGenresToViewBag()
    {
        ViewBag.AllGenres = await _db.Genres.ToListAsync();
    }

    private async Task SaveGameGenres(int gameId, int[] selectedGenres)
    {
        if (selectedGenres == null || selectedGenres.Length == 0) return;

        var genres = await _db.Genres.Where(g => selectedGenres.Contains(g.Id)).ToListAsync();
        foreach (var genre in genres)
        {
            _db.Set<GameGenre>().Add(new GameGenre
            {
                GameId = gameId,
                GenreId = genre.Id
            });
        }

        await _db.SaveChangesAsync();
    }

    #endregion
}