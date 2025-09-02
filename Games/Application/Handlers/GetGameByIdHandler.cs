using Games.Database;
using Games.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

public class GetGameByIdHandler : IRequestHandler<GetGameByIdQuery, Game?>
{
    private readonly AppDbContext _db;

    public GetGameByIdHandler(AppDbContext db)
    {
        _db = db;
    }

    public async Task<Game?> Handle(GetGameByIdQuery request, CancellationToken cancellationToken)
    {
        return await _db.Games
            .Include(g => g.GameGenres)
                .ThenInclude(gg => gg.Genre)
            .FirstOrDefaultAsync(g => g.Id == request.Id, cancellationToken);
    }
}