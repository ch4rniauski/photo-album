namespace frontend.Models;

public sealed record AdminUserDto(
    Guid Id,
    string UserName,
    string Email,
    string Role);
