using System.Diagnostics;
using Games.Database;
using Games.Models;
using Microsoft.AspNetCore.Mvc;

using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Games.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _db;

        public HomeController(AppDbContext db)
        {
            _db = db;
        }
        public async Task<IActionResult> Index(string? title, int[]? selectedGenreIds, int? year, bool upcomingOnly)
        {
            var query = _db.Games
                .Include(g => g.GameGenres)
                    .ThenInclude(gg => gg.Genre)
                .AsQueryable();

            if (!string.IsNullOrEmpty(title))
            {
                string lowerTitle = title.ToLower();
                query = query.Where(g => g.Title.ToLower().Contains(lowerTitle));
            }

            if (selectedGenreIds != null && selectedGenreIds.Any())
            {
                query = query.Where(g => g.GameGenres.Any(gg => selectedGenreIds.Contains(gg.GenreId)));
            }

            if (year.HasValue)
            {
                query = query.Where(g => g.ReleaseDate.Year == year.Value);
            }

            if (upcomingOnly)
            {
                query = query.Where(g => g.ReleaseDate > DateTime.UtcNow);
            }

            var model = new GameFilterViewModel
            {
                Title = title,
                Year = year,
                SelectedGenreIds = selectedGenreIds?.ToList() ?? new List<int>(),
                UpcomingOnly = upcomingOnly,
                AllGenres = await _db.Genres.ToListAsync(),
                Games = await query.ToListAsync()
            };

            return View(model);
        }

        public IActionResult Feedback()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}