using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using photo_album.Application.Dto.Folder.Requests;
using photo_album.Application.Dto.Folder.Responses;
using photo_album.Application.Dto.Photo.Responses;
using photo_album.Application.Extensions;
using photo_album.Application.UseCases.Commands.Folder;
using photo_album.Application.UseCases.Queries.Folder;

namespace photo_album.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class FoldersController : ControllerBase
{
    private readonly IMediator _mediator;

    public FoldersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<GetFolderResponseDto>> CreateFolder(
        [FromBody] CreateFolderRequestDto request,
        CancellationToken cancellationToken)
    {
        if (!TryGetUserId(out var ownerId))
        {
            return Unauthorized();
        }

        var command = new CreateFolderCommand(ownerId, request);

        var result = await _mediator.Send(command, cancellationToken);

        return result.Match(
            onSuccess: Ok,
            onFailure: err => Problem(
                detail: err.Description,
                statusCode: err.StatusCode));
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<GetFolderResponseDto>>> GetFolders(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var query = new GetFoldersQuery(page, pageSize);

        var result = await _mediator.Send(query, cancellationToken);

        return result.Match(
            onSuccess: Ok,
            onFailure: err => Problem(
                detail: err.Description,
                statusCode: err.StatusCode));
    }

    [Authorize]
    [HttpGet("mine")]
    public async Task<ActionResult<IReadOnlyList<GetFolderResponseDto>>> GetMyFolders(
        CancellationToken cancellationToken)
    {
        if (!TryGetUserId(out var ownerId))
        {
            return Unauthorized();
        }

        var query = new GetMyFoldersQuery(ownerId);

        var result = await _mediator.Send(query, cancellationToken);

        return result.Match(
            onSuccess: Ok,
            onFailure: err => Problem(
                detail: err.Description,
                statusCode: err.StatusCode));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<GetFolderResponseDto>> GetFolderById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var query = new GetFolderByIdQuery(id);

        var result = await _mediator.Send(query, cancellationToken);

        return result.Match(
            onSuccess: Ok,
            onFailure: err => Problem(
                detail: err.Description,
                statusCode: err.StatusCode));
    }

    [HttpGet("{id:guid}/photos")]
    public async Task<ActionResult<IReadOnlyList<GetPhotoResponseDto>>> GetFolderPhotos(
        Guid id,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var query = new GetFolderPhotosQuery(id, page, pageSize);

        var result = await _mediator.Send(query, cancellationToken);

        return result.Match(
            onSuccess: Ok,
            onFailure: err => Problem(
                detail: err.Description,
                statusCode: err.StatusCode));
    }

    private bool TryGetUserId(out Guid userId)
    {
        var userIdClaim = User.FindFirstValue(JwtRegisteredClaimNames.Sub)
                          ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userIdClaim is null ||
            !Guid.TryParse(userIdClaim, out userId))
        {
            userId = Guid.Empty;
            return false;
        }

        return true;
    }
}
