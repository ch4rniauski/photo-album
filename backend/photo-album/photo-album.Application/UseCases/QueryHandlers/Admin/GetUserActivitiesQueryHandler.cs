using MediatR;
using photo_album.Application.Common.Errors;
using photo_album.Application.Common.Results;
using photo_album.Application.Contracts.Repositories;
using photo_album.Application.Dto.Admin.Responses;
using photo_album.Application.Dto.Common;
using photo_album.Application.UseCases.Queries.Admin;

namespace photo_album.Application.UseCases.QueryHandlers.Admin;

internal sealed class GetUserActivitiesQueryHandler
    : IRequestHandler<GetUserActivitiesQuery, Result<PagedResultDto<UserActivityResponseDto>>>
{
    private readonly IUserActivityRepository _userActivityRepository;

    public GetUserActivitiesQueryHandler(IUserActivityRepository userActivityRepository)
    {
        _userActivityRepository = userActivityRepository;
    }

    public async Task<Result<PagedResultDto<UserActivityResponseDto>>> Handle(
        GetUserActivitiesQuery request,
        CancellationToken cancellationToken)
    {
        if (request.Page < 1 ||
            request.PageSize < 1 ||
            request.PageSize > 100)
        {
            return Result<PagedResultDto<UserActivityResponseDto>>.Failure(
                Error.FailedValidation("Page must be >= 1 and pageSize must be between 1 and 100")
            );
        }

        var (items, totalCount) = await _userActivityRepository.GetPagedAsync(
            request.Page,
            request.PageSize,
            request.UserId,
            request.Action,
            cancellationToken);

        var responseItems = items
            .Select(activity => new UserActivityResponseDto(
                activity.Id,
                activity.UserId,
                activity.UserName,
                activity.Action,
                activity.EntityType,
                activity.EntityId,
                activity.Details,
                activity.CreatedAtUtc))
            .ToList();

        var response = new PagedResultDto<UserActivityResponseDto>(
            responseItems,
            request.Page,
            request.PageSize,
            totalCount);

        return Result<PagedResultDto<UserActivityResponseDto>>.Success(response);
    }
}
