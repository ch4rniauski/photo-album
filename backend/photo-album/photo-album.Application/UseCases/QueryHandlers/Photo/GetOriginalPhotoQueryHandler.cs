using MediatR;
using photo_album.Application.Common.Errors;
using photo_album.Application.Common.Results;
using photo_album.Application.Contracts.Repositories;
using photo_album.Application.Contracts.Storage;
using photo_album.Application.Dto.Photo.Responses;
using photo_album.Application.UseCases.Queries.Photo;

namespace photo_album.Application.UseCases.QueryHandlers.Photo;

internal sealed class GetOriginalPhotoQueryHandler
    : IRequestHandler<GetOriginalPhotoQuery, Result<GetOriginalPhotoResponseDto>>
{
    private readonly IPhotoRepository _photoRepository;
    private readonly IImageStorage _imageStorage;

    public GetOriginalPhotoQueryHandler(
        IPhotoRepository photoRepository,
        IImageStorage imageStorage)
    {
        _photoRepository = photoRepository;
        _imageStorage = imageStorage;
    }

    public async Task<Result<GetOriginalPhotoResponseDto>> Handle(
        GetOriginalPhotoQuery request,
        CancellationToken cancellationToken)
    {
        var photo = await _photoRepository.GetByIdAsync(request.PhotoId, cancellationToken);

        if (photo is null)
        {
            return Result<GetOriginalPhotoResponseDto>.Failure(
                Error.NotFound($"Photo with id {request.PhotoId} does not exist")
            );
        }

        var content = _imageStorage.OpenOriginal(photo.OriginalFileName);

        if (content is null)
        {
            return Result<GetOriginalPhotoResponseDto>.Failure(
                Error.NotFound($"Original image file for photo {request.PhotoId} was not found")
            );
        }

        var response = new GetOriginalPhotoResponseDto(
            content,
            ResolveContentType(photo.OriginalFileName),
            photo.OriginalFileName);

        return Result<GetOriginalPhotoResponseDto>.Success(response);
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
