namespace Csdsa.Contracts.Dtos;

public record RegisterRequestDto(
    string Email,
    string Password,
    string UserName,
    string FirstName,
    string LastName
);
