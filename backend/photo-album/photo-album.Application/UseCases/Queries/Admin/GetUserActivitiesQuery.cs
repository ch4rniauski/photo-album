using MediatR;
using photo_album.Application.Common.Results;
using photo_album.Application.Dto.Admin.Responses;
using photo_album.Application.Dto.Common;

namespace photo_album.Application.UseCases.Queries.Admin;

public sealed record GetUserActivitiesQuery(
    int Page,
    int PageSize,
    Guid? UserId,
    string? Action) : IRequest<Result<PagedResultDto<UserActivityResponseDto>>>;
