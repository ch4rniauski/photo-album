using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using photo_album.Application.Dto.Admin.Responses;
using photo_album.Application.Dto.Common;
using photo_album.Application.Extensions;
using photo_album.Application.UseCases.Queries.Admin;
using photo_album.Domain.Constants;

namespace photo_album.Api.Controllers;

[ApiController]
[Authorize(Roles = UserRoles.Admin)]
[Route("api/[controller]")]
public sealed class AdminController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("users")]
    public async Task<ActionResult<PagedResultDto<AdminUserResponseDto>>> GetUsers(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAdminUsersQuery(page, pageSize);

        var result = await _mediator.Send(query, cancellationToken);

        return result.Match(
            onSuccess: Ok,
            onFailure: err => Problem(
                detail: err.Description,
                statusCode: err.StatusCode));
    }

    [HttpGet("activities")]
    public async Task<ActionResult<PagedResultDto<UserActivityResponseDto>>> GetActivities(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        [FromQuery] Guid? userId = null,
        [FromQuery] string? action = null,
        CancellationToken cancellationToken = default)
    {
        var query = new GetUserActivitiesQuery(page, pageSize, userId, action);

        var result = await _mediator.Send(query, cancellationToken);

        return result.Match(
            onSuccess: Ok,
            onFailure: err => Problem(
                detail: err.Description,
                statusCode: err.StatusCode));
    }
}
