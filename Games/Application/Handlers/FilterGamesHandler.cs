using Games.Application.Queries;
using Games.Database;
using Games.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

public class FilterGamesHandler : IRequestHandler<FilterGamesQuery, GameFilterViewModel>
{
    private readonly AppDbContext _db;

    public FilterGamesHandler(AppDbContext db)
    {
        _db = db;
    }

    public async Task<GameFilterViewModel> Handle(FilterGamesQuery request, CancellationToken cancellationToken)
    {
        var filter = request.Filter;

        var query = _db.Games
            .Include(g => g.GameGenres)
                .ThenInclude(gg => gg.Genre)
            .AsQueryable();

        if (!string.IsNullOrEmpty(filter.Title))
        {
            var lowerTitle = filter.Title.ToLower();
            query = query.Where(g => g.Title.ToLower().Contains(lowerTitle));
        }

        if (filter.SelectedGenreIds != null && filter.SelectedGenreIds.Count > 0)
        {
            query = query.Where(g => g.GameGenres.Any(gg => filter.SelectedGenreIds.Contains(gg.GenreId)));
        }

        if (filter.Year.HasValue)
        {
            query = query.Where(g => g.ReleaseDate.Year == filter.Year.Value);
        }

        if (filter.UpcomingOnly)
        {
            query = query.Where(g => g.ReleaseDate > DateTime.UtcNow);
        }

        filter.Games = await query.ToListAsync(cancellationToken);
        filter.AllGenres = await _db.Genres.ToListAsync(cancellationToken);

        return filter;
    }
}