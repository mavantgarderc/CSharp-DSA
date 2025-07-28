namespace Csdsa.Domain.ViewModel.EntityViewModel.Users;

public record CreateUserRequest
(
    string FirstName,
    string LastName,
    string UserName,
    string Password,
    string Email
);
