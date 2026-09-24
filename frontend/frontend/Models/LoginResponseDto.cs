namespace frontend.Models;

public sealed record LoginResponseDto(
    string AccessToken,
    string RefreshToken,
    string UserId);
