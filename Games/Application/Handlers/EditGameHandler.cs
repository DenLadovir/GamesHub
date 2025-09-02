using Games.Database;
using Games.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

public class EditGameHandler : IRequestHandler<EditGameCommand, Game?>
{
    private readonly AppDbContext _db;

    public EditGameHandler(AppDbContext db)
    {
        _db = db;
    }

    public async Task<Game?> Handle(EditGameCommand request, CancellationToken cancellationToken)
    {
        var game = await _db.Games
            .Include(g => g.GameGenres)
            .FirstOrDefaultAsync(g => g.Id == request.Id, cancellationToken);

        if (game == null) return null;

        game.Title = request.UpdatedGame.Title;
        game.Description = request.UpdatedGame.Description;
        game.ReleaseDate = DateTime.SpecifyKind(
            request.UpdatedGame.ReleaseDate == default ? DateTime.Today : request.UpdatedGame.ReleaseDate,
            DateTimeKind.Utc);

        _db.Set<GameGenre>().RemoveRange(game.GameGenres);
        await _db.SaveChangesAsync(cancellationToken);

        if (request.SelectedGenres?.Length > 0)
        {
            var genres = await _db.Genres
                .Where(g => request.SelectedGenres.Contains(g.Id))
                .ToListAsync(cancellationToken);

            foreach (var genre in genres)
            {
                _db.Set<GameGenre>().Add(new GameGenre { GameId = game.Id, GenreId = genre.Id });
            }

            await _db.SaveChangesAsync(cancellationToken);
        }

        return game;
    }
}