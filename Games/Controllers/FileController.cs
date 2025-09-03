using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.Json;
using CsvHelper;
using CsvHelper.Configuration;
using Games.Constants;
using Games.Database;
using Games.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class FileController : Controller
{
    private readonly AppDbContext _context;

    public FileController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost("upload/games/json")]
    public async Task<IActionResult> UploadGamesJson(IFormFile file)
    {
        if(file == null || file.Length == 0)
        {
            TempData["Error"] = Messages.FileNotLoaded;
            return RedirectToAction("Index", "Admin");
        }

        using Stream stream = file.OpenReadStream();
        try
        {
            var games = await JsonSerializer.DeserializeAsync<List<Game>>(stream);
            if (games == null || !games.Any())
            {
                TempData["Error"] = Messages.JsonParseError;
                return RedirectToAction("Index", "Admin");
            }

            HashSet<string> existingTitles = _context.Games.Select(g => g.Title).ToHashSet(StringComparer.OrdinalIgnoreCase);
            List<Game> newGames = games
            .Where(g => !existingTitles.Contains(g.Title))
            .ToList();

            if (!newGames.Any())
            {
                TempData["Error"] = Messages.GamesAlreadyExist;
                return RedirectToAction("Index", "Admin");
            }

            _context.Games.AddRange(newGames);
            await _context.SaveChangesAsync();

            TempData["Success"] = string.Format(Messages.GamesUploaded, newGames.Count);
            return RedirectToAction("Index", "Admin");
        }
        catch (JsonException ex)
        {
            TempData["Error"] = Messages.JsonFormatError + ex.Message;
            return RedirectToAction("Index", "Admin");
        }
    }

    [HttpPost("upload/games/csv")]
    public async Task<IActionResult> UploadGamesCsv(IFormFile file)
    {
        if(file == null || file.Length == 0)
        {
            TempData["Error"] = Messages.FileNotLoaded;
            return RedirectToAction("Index", "Admin");
        }

        CsvConfiguration config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = "\t",
            HasHeaderRecord = true,
            HeaderValidated = null,
            MissingFieldFound = null,
            Mode = CsvMode.Escape,
            Escape = '"',
            TrimOptions = TrimOptions.Trim
        };

        using var stream = new StreamReader(file.OpenReadStream());
        using var csv = new CsvReader(stream, config);

        try
        {
            csv.Context.RegisterClassMap<GameMap>();
            var games = csv.GetRecords<Game>().ToList();
            HashSet<string> existingTitles = _context.Games.Select(g => g.Title).ToHashSet(StringComparer.OrdinalIgnoreCase);
            List<Game> newGames = games.Where(g => !existingTitles.Contains(g.Title)).ToList();

            if (!newGames.Any())
            {
                TempData["Error"] = Messages.CsvParseError;
                return RedirectToAction("Index", "Admin");
            }

            _context.Games.AddRange(newGames);
            await _context.SaveChangesAsync();

            TempData["Success"] = string.Format(Messages.GamesUploaded, newGames.Count);
            return RedirectToAction("Index", "Admin");
        }
        catch(CsvHelperException ex)
        {
            TempData["Error"] = Messages.CsvFormatError + ex.Message;
            return RedirectToAction("Index", "Admin");
        }
        catch(Exception ex)
        {
            TempData["Error"] = Messages.GeneralError + ex.Message;
            return RedirectToAction("Index", "Admin");
        }
    }

    [HttpPost("upload/genres/json")]
    public async Task<IActionResult> UploadGenresJson(IFormFile file)
    {
        if(file == null || file.Length == 0)
        {
            TempData["Error"] = Messages.FileNotLoaded;
            return RedirectToAction("Index", "Admin");
        }

        using Stream stream = file.OpenReadStream();
        try
        {
            var genres = await JsonSerializer.DeserializeAsync<List<Genre>>(stream);

            HashSet<string> existedNames = _context.Genres.Select(g => g.Name).ToHashSet(StringComparer.OrdinalIgnoreCase);
            List<Genre> newGenres = genres.Where(g => !existedNames.Contains(g.Name)).ToList();

            if (!newGenres.Any())
            {
                TempData["Error"] = Messages.JsonParseError;
                return RedirectToAction("Index", "Admin");
            }

            _context.Genres.AddRange(newGenres);
            await _context.SaveChangesAsync();

            TempData["Success"] = string.Format(Messages.GenresUploaded, newGenres.Count);
            return RedirectToAction("Index", "Admin");
        }
        catch (JsonException ex)
        {
            TempData["Error"] = Messages.JsonFormatError + ex.Message;
            return RedirectToAction("Index", "Admin");
        }
    }

    [HttpPost("upload/genres/csv")]
    public async Task<IActionResult> UploadGenresCsv(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            TempData["Error"] = Messages.FileNotLoaded;
            return RedirectToAction("Index", "Admin");
        }

        CsvConfiguration config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = "\t",
            HasHeaderRecord = true,
            HeaderValidated = null,
            MissingFieldFound = null,
            Mode = CsvMode.Escape,
            Escape = '"',
            TrimOptions = TrimOptions.Trim
        };

        using StreamReader stream = new StreamReader(file.OpenReadStream());
        using var csv = new CsvReader(stream, config);

        try
        {
            csv.Context.RegisterClassMap<GameMap>();
            var genres = csv.GetRecords<Genre>().ToList();
            HashSet<string> existedNames = _context.Genres.Select(g => g.Name).ToHashSet(StringComparer.OrdinalIgnoreCase);
            List<Genre> newGenres = genres.Where(g => !existedNames.Contains(g.Name)).ToList();

            if (!newGenres.Any())
            {
                TempData["Error"] = Messages.CsvFormatError;
                return RedirectToAction("Index", "Admin");
            }

            _context.Genres.AddRange(newGenres);
            await _context.SaveChangesAsync();

            TempData["Success"] = string.Format(Messages.GenresUploaded, newGenres.Count);
            return RedirectToAction("Index", "Admin");
        }
        catch(CsvHelperException ex)
        {
            TempData["Error"] = Messages.CsvFormatError + ex.Message;
            return RedirectToAction("Index", "Admin");
        }
    }
}

public sealed class GameMap : ClassMap<Game>
{
    public GameMap()
    {
        Map(m => m.Title).Name("Title");
        Map(m => m.Description).Name("Description");
        Map(m => m.ReleaseDate).Name("ReleaseDate");
        Map(m => m.Id).Ignore();
    }
}