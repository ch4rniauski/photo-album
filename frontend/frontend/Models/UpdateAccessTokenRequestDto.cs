namespace frontend.Models;

public sealed record UpdateAccessTokenRequestDto(
    string UserId,
    string RefreshToken);
