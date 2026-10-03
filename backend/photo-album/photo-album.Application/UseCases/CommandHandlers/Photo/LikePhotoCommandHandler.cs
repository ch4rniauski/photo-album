using AutoMapper;
using MediatR;
using photo_album.Application.Common.Errors;
using photo_album.Application.Common.Results;
using photo_album.Application.Contracts.Activity;
using photo_album.Application.Contracts.Repositories;
using photo_album.Application.Dto.Photo.Responses;
using photo_album.Application.UseCases.Commands.Photo;
using photo_album.Domain.Constants;
using photo_album.Domain.Entities;

namespace photo_album.Application.UseCases.CommandHandlers.Photo;

internal sealed class LikePhotoCommandHandler
    : IRequestHandler<LikePhotoCommand, Result<UploadPhotoResponseDto>>
{
    private readonly IPhotoRepository _photoRepository;
    private readonly IPhotoReactionRepository _photoReactionRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUserActivityLogService _activityLogService;
    private readonly IMapper _mapper;

    public LikePhotoCommandHandler(
        IPhotoRepository photoRepository,
        IPhotoReactionRepository photoReactionRepository,
        IUserRepository userRepository,
        IUserActivityLogService activityLogService,
        IMapper mapper)
    {
        _photoRepository = photoRepository;
        _photoReactionRepository = photoReactionRepository;
        _userRepository = userRepository;
        _activityLogService = activityLogService;
        _mapper = mapper;
    }

    public async Task<Result<UploadPhotoResponseDto>> Handle(
        LikePhotoCommand request,
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

        var reaction = await _photoReactionRepository.GetByPhotoIdAndUserIdAsync(
            request.PhotoId,
            request.UserId,
            cancellationToken);

        if (reaction is not null &&
            reaction.IsLiked)
        {
            if (photo.LikesCount > 0)
            {
                photo.LikesCount--;
            }

            var isDeleted = await _photoReactionRepository.DeleteAsync(reaction, cancellationToken);

            if (!isDeleted)
            {
                return Result<UploadPhotoResponseDto>.Failure(
                    Error.InternalError($"Photo with id {photo.Id} was not updated")
                );
            }

            await LogActivityAsync(
                user,
                photo,
                UserActivityActions.RemoveLikePhoto,
                cancellationToken);

            return Result<UploadPhotoResponseDto>.Success(
                _mapper.Map<UploadPhotoResponseDto>(photo)
            );
        }

        if (reaction is not null)
        {
            if (photo.DislikesCount > 0)
            {
                photo.DislikesCount--;
            }

            photo.LikesCount++;
            reaction.IsLiked = true;

            var isUpdated = await _photoReactionRepository.UpdateAsync(reaction, cancellationToken);

            if (!isUpdated)
            {
                return Result<UploadPhotoResponseDto>.Failure(
                    Error.InternalError($"Photo with id {photo.Id} was not updated")
                );
            }

            await LogActivityAsync(
                user,
                photo,
                UserActivityActions.LikePhoto,
                cancellationToken);

            return Result<UploadPhotoResponseDto>.Success(
                _mapper.Map<UploadPhotoResponseDto>(photo)
            );
        }

        photo.LikesCount++;

        var newReaction = new PhotoReactionEntity
        {
            PhotoId = request.PhotoId,
            UserId = request.UserId,
            IsLiked = true
        };

        var isCreated = await _photoReactionRepository.AddAsync(newReaction, cancellationToken);

        if (!isCreated)
        {
            return Result<UploadPhotoResponseDto>.Failure(
                Error.InternalError($"Photo with id {photo.Id} was not updated")
            );
        }

        await LogActivityAsync(
            user,
            photo,
            UserActivityActions.LikePhoto,
            cancellationToken);

        return Result<UploadPhotoResponseDto>.Success(
            _mapper.Map<UploadPhotoResponseDto>(photo)
        );
    }

    private Task LogActivityAsync(
        UserEntity user,
        PhotoEntity photo,
        string action,
        CancellationToken cancellationToken)
    {
        return _activityLogService.LogAsync(
            user.Id,
            user.UserName,
            action,
            entityType: "Photo",
            entityId: photo.Id,
            details: photo.Name,
            cancellationToken: cancellationToken);
    }
}
