using Games.Models;
using MediatR;

public record AddGameCommand(Game Game, int[] SelectedGenres) : IRequest<Game>;