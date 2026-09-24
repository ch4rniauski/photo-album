namespace frontend.Models;

public sealed record LoginRequestDto(
    string Email,
    string Password);
