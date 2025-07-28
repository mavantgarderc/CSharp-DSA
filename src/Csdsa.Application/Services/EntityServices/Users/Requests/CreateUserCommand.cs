using Csdsa.Domain.ViewModel.EntityViewModel.Users;
using FluentValidation;
using MediatR;

namespace Csdsa.Application.Services.EntityServices.Users.Requests;

public sealed class CreateUserCommandValidator : AbstractValidator<CreateUserCommandValidator>
{
    public CreateUserCommandValidator()
    {

    }
}

public record CreateUserCommand(CreateUserRequest Command) : IRequest<bool>;
