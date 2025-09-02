using Games.Models;
using MediatR;

public record EditGameCommand(int Id, Game UpdatedGame, int[] SelectedGenres) : IRequest<Game?>;