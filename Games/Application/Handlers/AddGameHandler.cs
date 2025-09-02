using Games.Database;
using Games.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

public class AddGameHandler : IRequestHandler<AddGameCommand, Game>
{
    private readonly AppDbContext _db;

    public AddGameHandler(AppDbContext db)
    {
        _db = db;
    }

    public async Task<Game> Handle(AddGameCommand request, CancellationToken cancellationToken)
    {
        var game = request.Game;
        game.ReleaseDate = DateTime.SpecifyKind(
            game.ReleaseDate == default ? DateTime.Today : game.ReleaseDate,
            DateTimeKind.Utc);

        _db.Games.Add(game);
        await _db.SaveChangesAsync(cancellationToken);

        if (request.SelectedGenres?.Any() == true)
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