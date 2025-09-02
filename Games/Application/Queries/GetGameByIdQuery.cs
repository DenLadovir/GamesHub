using Games.Models;
using MediatR;

public record GetGameByIdQuery(int Id) : IRequest<Game?>;