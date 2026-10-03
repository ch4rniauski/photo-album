using MediatR;
using photo_album.Application.Common.Results;
using photo_album.Application.Dto.Admin.Responses;
using photo_album.Application.Dto.Common;

namespace photo_album.Application.UseCases.Queries.Admin;

public sealed record GetAdminUsersQuery(int Page, int PageSize)
    : IRequest<Result<PagedResultDto<AdminUserResponseDto>>>;
