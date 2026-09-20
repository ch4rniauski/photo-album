using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using photo_album.Application.Dto.User.Requests;
using photo_album.Application.Dto.User.Responses;
using photo_album.Application.Extensions;
using photo_album.Application.UseCases.Commands.User;
using photo_album.Application.UseCases.Queries.User;

namespace photo_album.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("register")]
    public async Task<ActionResult<CreateUserResponseDto>> Register(
        [FromBody] CreateUserRequestDto request,
        CancellationToken cancellationToken)
    {
        var command = new CreateUserCommand(request);

        var result = await _mediator.Send(command, cancellationToken);

        return result.Match(
            onSuccess: Ok,
            onFailure: err => Problem(
                detail: err.Description,
                statusCode: err.StatusCode));
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginUserResponseDto>> Login(
        [FromBody] LoginUserRequestDto request,
        CancellationToken cancellationToken)
    {
        var command = new LoginUserCommand(request);

        var result = await _mediator.Send(command, cancellationToken);

        return result.Match(
            onSuccess: Ok,
            onFailure: err => Problem(
                detail: err.Description,
                statusCode: err.StatusCode));
    }

    [Authorize]
    [HttpGet("is-auth")]
    public ActionResult IsAuth()
    {
        return Ok();
    }

    [HttpPost("access-token")]
    public async Task<ActionResult<UpdateAccessTokenResponseDto>> UpdateAccessToken(
        [FromBody] UpdateAccessTokenRequestDto request,
        CancellationToken cancellationToken)
    {
        var query = new UpdateAccessTokenQuery(request.UserId, request.RefreshToken);

        var result = await _mediator.Send(query, cancellationToken);

        return result.Match(
            onSuccess: Ok,
            onFailure: err => Problem(
                detail: err.Description,
                statusCode: err.StatusCode));
    }
}
