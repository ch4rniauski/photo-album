using MediatR;
using photo_album.Application.Common.Errors;
using photo_album.Application.Common.Results;
using photo_album.Application.Contracts.Activity;
using photo_album.Application.Contracts.Repositories;
using photo_album.Application.Contracts.Storage;
using photo_album.Application.UseCases.Commands.Photo;
using photo_album.Domain.Constants;

namespace photo_album.Application.UseCases.CommandHandlers.Photo;

internal sealed class DeletePhotoCommandHandler
    : IRequestHandler<DeletePhotoCommand, Result<Guid>>
{
    private readonly IPhotoRepository _photoRepository;
    private readonly IUserRepository _userRepository;
    private readonly IImageStorage _imageStorage;
    private readonly IUserActivityLogService _activityLogService;

    public DeletePhotoCommandHandler(
        IPhotoRepository photoRepository,
        IUserRepository userRepository,
        IImageStorage imageStorage,
        IUserActivityLogService activityLogService)
    {
        _photoRepository = photoRepository;
        _userRepository = userRepository;
        _imageStorage = imageStorage;
        _activityLogService = activityLogService;
    }

    public async Task<Result<Guid>> Handle(
        DeletePhotoCommand request,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

        if (user is null)
        {
            return Result<Guid>.Failure(
                Error.NotFound($"User with id {request.UserId} does not exist")
            );
        }

        var photo = await _photoRepository.GetByIdAsync(request.PhotoId, cancellationToken);

        if (photo is null)
        {
            return Result<Guid>.Failure(
                Error.NotFound($"Photo with id {request.PhotoId} does not exist")
            );
        }

        if (photo.OwnerId != request.UserId)
        {
            return Result<Guid>.Failure(
                Error.Forbidden("Only the photo owner can delete this photo")
            );
        }

        var photoName = photo.Name;
        var originalFileName = photo.OriginalFileName;
        var thumbnailFileName = photo.ThumbnailFileName;

        var deletedCount = await _photoRepository.DeleteRangeAsync(
            [photo],
            cancellationToken);

        if (deletedCount == 0)
        {
            return Result<Guid>.Failure(
                Error.InternalError($"Photo with id {photo.Id} was not deleted")
            );
        }

        _imageStorage.Delete(originalFileName, thumbnailFileName);

        await _activityLogService.LogAsync(
            user.Id,
            user.UserName,
            UserActivityActions.DeletePhoto,
            entityType: "Photo",
            entityId: photo.Id,
            details: photoName,
            cancellationToken: cancellationToken);

        return Result<Guid>.Success(photo.Id);
    }
}
