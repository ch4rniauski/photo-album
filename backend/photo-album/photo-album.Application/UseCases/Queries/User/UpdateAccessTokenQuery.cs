using MediatR;
using photo_album.Application.Common.Results;
using photo_album.Application.Dto.User.Responses;

namespace photo_album.Application.UseCases.Queries.User;

public sealed record UpdateAccessTokenQuery(
    string? UserId,
    string RefreshToken) : IRequest<Result<UpdateAccessTokenResponseDto>>;
