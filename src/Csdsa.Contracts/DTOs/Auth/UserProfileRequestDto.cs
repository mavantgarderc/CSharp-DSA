namespace Csdsa.Contracts.Dtos;

public record UserProfileRequestDto(
    string FirstName,
    string LastName,
    string Email,
    string? PhoneNumber
);
