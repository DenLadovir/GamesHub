using Games.Database;
using MediatR;
using Microsoft.EntityFrameworkCore;

public class DeleteGameHandler : IRequestHandler<DeleteGameCommand, bool>
{
    private readonly AppDbContext _db;
    private readonly ILogger<DeleteGameHandler> _logger;

    public DeleteGameHandler(AppDbContext db, ILogger<DeleteGameHandler> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<bool> Handle(DeleteGameCommand request, CancellationToken cancellationToken)
    {
        var game = await _db.Games.FirstOrDefaultAsync(g => g.Id == request.Id, cancellationToken);
        if (game == null)
        {
            _logger.LogWarning($"Игра с ID {request.Id} не найдена");
            return false;
        }

        _db.Games.Remove(game);
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }
}