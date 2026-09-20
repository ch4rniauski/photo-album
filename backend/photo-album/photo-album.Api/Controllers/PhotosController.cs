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
        var ownerIdClaim = User.FindFirstValue(JwtRegisteredClaimNames.Sub) ??
                           User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (ownerIdClaim is null ||
            !Guid.TryParse(ownerIdClaim, out var ownerId))
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

    [Authorize]
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
}
