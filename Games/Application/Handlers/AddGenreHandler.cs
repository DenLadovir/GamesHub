using Games.Database;
using Games.Models;
using MediatR;

public class AddGenreHandler : IRequestHandler<AddGenreCommand, Genre>
{
    private readonly AppDbContext _db;

    public AddGenreHandler(AppDbContext db)
    {
        _db = db;
    }

    public async Task<Genre> Handle(AddGenreCommand request, CancellationToken cancellationToken)
    {
        _db.Genres.Add(request.Genre);
        await _db.SaveChangesAsync(cancellationToken);
        return request.Genre;
    }
}