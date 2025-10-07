namespace Csdsa.Contracts.Dtos;

public record BaseEntityDto(
     Guid Id,
     DateTime CreatedAt,
     bool IsDeleted
);
