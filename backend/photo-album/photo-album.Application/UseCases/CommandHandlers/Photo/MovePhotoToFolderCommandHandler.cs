using AutoMapper;
using MediatR;
using photo_album.Application.Common.Errors;
using photo_album.Application.Common.Results;
using photo_album.Application.Contracts.Activity;
using photo_album.Application.Contracts.Repositories;
using photo_album.Application.Dto.Photo.Responses;
using photo_album.Application.UseCases.Commands.Photo;
using photo_album.Domain.Constants;

namespace photo_album.Application.UseCases.CommandHandlers.Photo;

internal sealed class MovePhotoToFolderCommandHandler
    : IRequestHandler<MovePhotoToFolderCommand, Result<UploadPhotoResponseDto>>
{
    private readonly IPhotoRepository _photoRepository;
    private readonly IFolderRepository _folderRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUserActivityLogService _activityLogService;
    private readonly IMapper _mapper;

    public MovePhotoToFolderCommandHandler(
        IPhotoRepository photoRepository,
        IFolderRepository folderRepository,
        IUserRepository userRepository,
        IUserActivityLogService activityLogService,
        IMapper mapper)
    {
        _photoRepository = photoRepository;
        _folderRepository = folderRepository;
        _userRepository = userRepository;
        _activityLogService = activityLogService;
        _mapper = mapper;
    }

    public async Task<Result<UploadPhotoResponseDto>> Handle(
        MovePhotoToFolderCommand request,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

        if (user is null)
        {
            return Result<UploadPhotoResponseDto>.Failure(
                Error.NotFound($"User with id {request.UserId} does not exist")
            );
        }

        var photo = await _photoRepository.GetByIdAsync(request.PhotoId, cancellationToken);

        if (photo is null)
        {
            return Result<UploadPhotoResponseDto>.Failure(
                Error.NotFound($"Photo with id {request.PhotoId} does not exist")
            );
        }

        if (photo.OwnerId != request.UserId)
        {
            return Result<UploadPhotoResponseDto>.Failure(
                Error.Forbidden("Only the photo owner can move this photo")
            );
        }

        var targetFolderId = request.Request.FolderId;

        if (targetFolderId is not null)
        {
            var folder = await _folderRepository.GetByIdAsync(targetFolderId.Value, cancellationToken);

            if (folder is null)
            {
                return Result<UploadPhotoResponseDto>.Failure(
                    Error.NotFound($"Folder with id {targetFolderId.Value} does not exist")
                );
            }

            if (folder.OwnerId != request.UserId)
            {
                return Result<UploadPhotoResponseDto>.Failure(
                    Error.Forbidden("You can only move photos into your own folders")
                );
            }
        }

        if (photo.FolderId == targetFolderId)
        {
            return Result<UploadPhotoResponseDto>.Success(
                _mapper.Map<UploadPhotoResponseDto>(photo)
            );
        }

        var previousFolderId = photo.FolderId;
        photo.FolderId = targetFolderId;

        var isUpdated = await _photoRepository.UpdateAsync(photo, cancellationToken);

        if (!isUpdated)
        {
            return Result<UploadPhotoResponseDto>.Failure(
                Error.InternalError($"Photo with id {photo.Id} was not updated")
            );
        }

        var details = $"{previousFolderId?.ToString() ?? "none"} -> {targetFolderId?.ToString() ?? "none"}";

        await _activityLogService.LogAsync(
            user.Id,
            user.UserName,
            UserActivityActions.MovePhotoToFolder,
            entityType: "Photo",
            entityId: photo.Id,
            details: details,
            cancellationToken: cancellationToken);

        return Result<UploadPhotoResponseDto>.Success(
            _mapper.Map<UploadPhotoResponseDto>(photo)
        );
    }
}
