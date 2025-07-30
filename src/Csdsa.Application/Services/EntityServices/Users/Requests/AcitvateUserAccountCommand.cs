using MediatR;

public record ActivateUserAccountCommand(Guid userId) : IRequest<bool>;
