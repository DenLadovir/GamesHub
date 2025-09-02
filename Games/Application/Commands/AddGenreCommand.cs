using Games.Models;
using MediatR;

public record AddGenreCommand(Genre Genre) : IRequest<Genre>;