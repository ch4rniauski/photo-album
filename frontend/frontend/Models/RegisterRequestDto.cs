namespace frontend.Models;

public sealed record RegisterRequestDto(
    string UserName,
    string Email,
    string Password);
