using Games.Database;
using Games.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

public class GetAllGenresHandler : IRequestHandler<GetAllGenresQuery, List<Genre>>
{
    private readonly AppDbContext _db;

    public GetAllGenresHandler(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<Genre>> Handle(GetAllGenresQuery request, CancellationToken cancellationToken)
    {
        return await _db.Genres.ToListAsync(cancellationToken);
    }
}