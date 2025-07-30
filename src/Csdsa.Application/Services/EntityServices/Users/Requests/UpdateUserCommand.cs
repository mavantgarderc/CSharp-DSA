
using MediatR;

public record UpdateUserCommand(Guid userId, string Email, string Username, bool IsActive) : IRequest<bool>;
