using AutoMapper;
using FluentValidation;
using MediatR;
using photo_album.Application.Common.Errors;
using photo_album.Application.Common.Results;
using photo_album.Application.Contracts.Activity;
using photo_album.Application.Contracts.Repositories;
using photo_album.Application.Dto.Photo.Requests;
using photo_album.Application.Dto.Photo.Responses;
using photo_album.Application.UseCases.Commands.Photo;
using photo_album.Domain.Constants;

namespace photo_album.Application.UseCases.CommandHandlers.Photo;

internal sealed class RenamePhotoCommandHandler
    : IRequestHandler<RenamePhotoCommand, Result<UploadPhotoResponseDto>>
{
    private readonly IPhotoRepository _photoRepository;
    private readonly IUserRepository _userRepository;
    private readonly IValidator<RenamePhotoRequestDto> _validator;
    private readonly IUserActivityLogService _activityLogService;
    private readonly IMapper _mapper;

    public RenamePhotoCommandHandler(
        IPhotoRepository photoRepository,
        IUserRepository userRepository,
        IValidator<RenamePhotoRequestDto> validator,
        IUserActivityLogService activityLogService,
        IMapper mapper)
    {
        _photoRepository = photoRepository;
        _userRepository = userRepository;
        _validator = validator;
        _activityLogService = activityLogService;
        _mapper = mapper;
    }

    public async Task<Result<UploadPhotoResponseDto>> Handle(
        RenamePhotoCommand request,
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request.Request, cancellationToken);

        if (!validationResult.IsValid)
        {
            var message = string.Join("; ", validationResult.Errors.Select(error => error.ErrorMessage));

            return Result<UploadPhotoResponseDto>.Failure(
                Error.FailedValidation(message)
            );
        }

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
                Error.Forbidden("Only the photo owner can rename this photo")
            );
        }

        var trimmedName = request.Request.Name.Trim();

        if (string.Equals(photo.Name, trimmedName, StringComparison.Ordinal))
        {
            return Result<UploadPhotoResponseDto>.Success(
                _mapper.Map<UploadPhotoResponseDto>(photo)
            );
        }

        var previousName = photo.Name;
        photo.Name = trimmedName;

        var isUpdated = await _photoRepository.UpdateAsync(photo, cancellationToken);

        if (!isUpdated)
        {
            return Result<UploadPhotoResponseDto>.Failure(
                Error.InternalError($"Photo with id {photo.Id} was not updated")
            );
        }

        await _activityLogService.LogAsync(
            user.Id,
            user.UserName,
            UserActivityActions.RenamePhoto,
            entityType: "Photo",
            entityId: photo.Id,
            details: $"{previousName} -> {photo.Name}",
            cancellationToken: cancellationToken);

        return Result<UploadPhotoResponseDto>.Success(
            _mapper.Map<UploadPhotoResponseDto>(photo)
        );
    }
}
