using MediatR;
using photo_album.Application.Common.Errors;
using photo_album.Application.Common.Results;
using photo_album.Application.Contracts.Repositories;
using photo_album.Application.Dto.Admin.Responses;
using photo_album.Application.Dto.Common;
using photo_album.Application.UseCases.Queries.Admin;

namespace photo_album.Application.UseCases.QueryHandlers.Admin;

internal sealed class GetAdminUsersQueryHandler
    : IRequestHandler<GetAdminUsersQuery, Result<PagedResultDto<AdminUserResponseDto>>>
{
    private readonly IUserRepository _userRepository;

    public GetAdminUsersQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Result<PagedResultDto<AdminUserResponseDto>>> Handle(
        GetAdminUsersQuery request,
        CancellationToken cancellationToken)
    {
        if (request.Page < 1 ||
            request.PageSize < 1 ||
            request.PageSize > 100)
        {
            return Result<PagedResultDto<AdminUserResponseDto>>.Failure(
                Error.FailedValidation("Page must be >= 1 and pageSize must be between 1 and 100")
            );
        }

        var (items, totalCount) = await _userRepository.GetPagedAsync(
            request.Page,
            request.PageSize,
            cancellationToken);

        var responseItems = items
            .Select(user => new AdminUserResponseDto(
                user.Id,
                user.UserName,
                user.Email,
                user.Role))
            .ToList();

        var response = new PagedResultDto<AdminUserResponseDto>(
            responseItems,
            request.Page,
            request.PageSize,
            totalCount);

        return Result<PagedResultDto<AdminUserResponseDto>>.Success(response);
    }
}
