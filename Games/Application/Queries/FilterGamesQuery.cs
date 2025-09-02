using Games.Models;
using MediatR;
using System.Collections.Generic;

namespace Games.Application.Queries
{
    public record FilterGamesQuery(GameFilterViewModel Filter) : IRequest<GameFilterViewModel>;
}