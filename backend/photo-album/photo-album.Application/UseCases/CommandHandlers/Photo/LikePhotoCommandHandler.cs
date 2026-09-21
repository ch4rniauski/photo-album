using AutoMapper;
using MediatR;
using photo_album.Application.Common.Errors;
using photo_album.Application.Common.Results;
using photo_album.Application.Contracts.Repositories;
using photo_album.Application.Dto.Photo.Responses;
using photo_album.Application.UseCases.Commands.Photo;
using photo_album.Domain.Entities;

namespace photo_album.Application.UseCases.CommandHandlers.Photo;

internal sealed class LikePhotoCommandHandler
    : IRequestHandler<LikePhotoCommand, Result<UploadPhotoResponseDto>>
{
    private readonly IPhotoRepository _photoRepository;
    private readonly IPhotoReactionRepository _photoReactionRepository;
    private readonly IMapper _mapper;

    public LikePhotoCommandHandler(
        IPhotoRepository photoRepository,
        IPhotoReactionRepository photoReactionRepository,
        IMapper mapper)
    {
        _photoRepository = photoRepository;
        _photoReactionRepository = photoReactionRepository;
        _mapper = mapper;
    }

    public async Task<Result<UploadPhotoResponseDto>> Handle(
        LikePhotoCommand request,
        CancellationToken cancellationToken)
    {
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

        return Result<UploadPhotoResponseDto>.Success(
            _mapper.Map<UploadPhotoResponseDto>(photo)
        );
    }
}
