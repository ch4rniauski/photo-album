using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using photo_album.Application.Dto.Photo.Requests;
using photo_album.Application.Dto.Photo.Responses;
using photo_album.Application.Extensions;
using photo_album.Application.UseCases.Commands.Photo;
using photo_album.Application.UseCases.Queries.Photo;
using photo_album.Application.Validators.Photo;

namespace photo_album.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class PhotosController : ControllerBase
{
    private readonly IMediator _mediator;

    public PhotosController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize]
    [HttpPost]
    [RequestSizeLimit(UploadPhotoRequestDtoValidator.MaxFileSizeBytes)]
    [RequestFormLimits(MultipartBodyLengthLimit = UploadPhotoRequestDtoValidator.MaxFileSizeBytes)]
    public async Task<ActionResult<UploadPhotoResponseDto>> UploadPhoto(
        IFormFile? photo,
        [FromForm] string name,
        CancellationToken cancellationToken)
    {
        if (!TryGetUserId(out var ownerId))
        {
            return Unauthorized();
        }

        if (photo is null)
        {
            return Problem(
                detail: "Image file is required",
                statusCode: StatusCodes.Status400BadRequest);
        }

        await using var content = photo.OpenReadStream();

        var request = new UploadPhotoRequestDto(
            name,
            photo.ContentType,
            photo.FileName,
            photo.Length);

        var command = new UploadPhotoCommand(ownerId, request, content);

        var result = await _mediator.Send(command, cancellationToken);

        return result.Match(
            onSuccess: Ok,
            onFailure: err => Problem(
                detail: err.Description,
                statusCode: err.StatusCode));
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<GetPhotoResponseDto>>> GetPhotosWithPagination(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var query = new GetPhotosQuery(page, pageSize);

        var result = await _mediator.Send(query, cancellationToken);

        return result.Match(
            onSuccess: Ok,
            onFailure: err => Problem(
                detail: err.Description,
                statusCode: err.StatusCode));
    }

    [HttpGet("search")]
    public async Task<ActionResult<IReadOnlyList<SearchPhotoResponseDto>>> SearchPhotos(
        [FromQuery] string? search,
        CancellationToken cancellationToken)
    {
        var query = new SearchPhotosQuery(search ?? string.Empty);

        var result = await _mediator.Send(query, cancellationToken);

        return result.Match(
            onSuccess: Ok,
            onFailure: err => Problem(
                detail: err.Description,
                statusCode: err.StatusCode));
    }

    [HttpGet("{id:guid}/thumbnail")]
    public async Task<IActionResult> GetThumbnailPhoto(
        Guid id,
        CancellationToken cancellationToken)
    {
        var query = new GetThumbnailPhotoQuery(id);

        var result = await _mediator.Send(query, cancellationToken);

        return result.Match<GetThumbnailPhotoResponseDto, IActionResult>(
            onSuccess: file => File(file.Content, file.ContentType),
            onFailure: err => Problem(
                detail: err.Description,
                statusCode: err.StatusCode));
    }

    [HttpGet("{id:guid}/original")]
    public async Task<IActionResult> GetOriginalPhoto(
        Guid id,
        CancellationToken cancellationToken)
    {
        var query = new GetOriginalPhotoQuery(id);

        var result = await _mediator.Send(query, cancellationToken);

        return result.Match<GetOriginalPhotoResponseDto, IActionResult>(
            onSuccess: file => File(file.Content, file.ContentType),
            onFailure: err => Problem(
                detail: err.Description,
                statusCode: err.StatusCode));
    }

    [Authorize]
    [HttpPut("{id:guid}/likes")]
    public async Task<ActionResult<UploadPhotoResponseDto>> LikePhoto(
        Guid id,
        CancellationToken cancellationToken)
    {
        if (!TryGetUserId(out var userId))
        {
            return Unauthorized();
        }

        var command = new LikePhotoCommand(id, userId);

        var result = await _mediator.Send(command, cancellationToken);

        return result.Match(
            onSuccess: Ok,
            onFailure: err => Problem(
                detail: err.Description,
                statusCode: err.StatusCode));
    }

    [Authorize]
    [HttpPut("{id:guid}/dislikes")]
    public async Task<ActionResult<UploadPhotoResponseDto>> DislikePhoto(
        Guid id,
        CancellationToken cancellationToken)
    {
        if (!TryGetUserId(out var userId))
        {
            return Unauthorized();
        }

        var command = new DislikePhotoCommand(id, userId);

        var result = await _mediator.Send(command, cancellationToken);

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
