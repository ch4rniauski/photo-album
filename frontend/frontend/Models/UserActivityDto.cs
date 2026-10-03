namespace frontend.Models;

public sealed record UserActivityDto(
    Guid Id,
    Guid? UserId,
    string UserName,
    string Action,
    string? EntityType,
    Guid? EntityId,
    string? Details,
    DateTime CreatedAtUtc);
