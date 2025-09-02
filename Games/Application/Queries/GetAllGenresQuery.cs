using Games.Models;
using MediatR;
using System.Collections.Generic;

public record GetAllGenresQuery() : IRequest<List<Genre>>;