using MediatR;

namespace Csdsa.Application.Services.EntityServices.Users.Requests;

public record ActivateUserAccountCommand(Guid UserId) : IRequest<bool>;
