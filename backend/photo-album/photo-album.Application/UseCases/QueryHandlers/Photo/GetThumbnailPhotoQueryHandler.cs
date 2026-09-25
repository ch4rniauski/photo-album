using MediatR;
using photo_album.Application.Common.Errors;
using photo_album.Application.Common.Results;
using photo_album.Application.Contracts.Repositories;
using photo_album.Application.Contracts.Storage;
using photo_album.Application.Dto.Photo.Responses;
using photo_album.Application.UseCases.Queries.Photo;

namespace photo_album.Application.UseCases.QueryHandlers.Photo;

internal sealed class GetThumbnailPhotoQueryHandler
    : IRequestHandler<GetThumbnailPhotoQuery, Result<GetThumbnailPhotoResponseDto>>
{
    private readonly IPhotoRepository _photoRepository;
    private readonly IImageStorage _imageStorage;

    public GetThumbnailPhotoQueryHandler(
        IPhotoRepository photoRepository,
        IImageStorage imageStorage)
    {
        _photoRepository = photoRepository;
        _imageStorage = imageStorage;
    }

    public async Task<Result<GetThumbnailPhotoResponseDto>> Handle(
        GetThumbnailPhotoQuery request,
        CancellationToken cancellationToken)
    {
        var photo = await _photoRepository.GetByIdAsync(request.PhotoId, cancellationToken);

        if (photo is null)
        {
            return Result<GetThumbnailPhotoResponseDto>.Failure(
                Error.NotFound($"Photo with id {request.PhotoId} does not exist")
            );
        }

        var content = _imageStorage.OpenThumbnail(photo.ThumbnailFileName);

        if (content is null)
        {
            return Result<GetThumbnailPhotoResponseDto>.Failure(
                Error.NotFound($"Thumbnail image file for photo {request.PhotoId} was not found")
            );
        }

        var response = new GetThumbnailPhotoResponseDto(
            content,
            ResolveContentType(photo.ThumbnailFileName),
            photo.ThumbnailFileName);

        return Result<GetThumbnailPhotoResponseDto>.Success(response);
    }

    private static string ResolveContentType(string fileName)
    {
        return Path.GetExtension(fileName).ToLowerInvariant() switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".webp" => "image/webp",
            _ => "application/octet-stream"
        };
    }
}
