using MediatR;

public record DeleteGameCommand(int Id) : IRequest<bool>;