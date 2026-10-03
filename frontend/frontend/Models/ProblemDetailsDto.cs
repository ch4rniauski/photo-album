namespace frontend.Models;

public sealed record ProblemDetailsDto(
    string? Detail,
    string? Title,
    int? Status);
