using System.Text.RegularExpressions;
using AutoMapper;
using FluentValidation;
using MediatR;
using photo_album.Application.Common.Errors;
using photo_album.Application.Common.Results;
using photo_album.Application.Contracts.Repositories;
using photo_album.Application.Contracts.Storage;
using photo_album.Application.Dto.Photo.Requests;
using photo_album.Application.Dto.Photo.Responses;
using photo_album.Application.UseCases.Commands.Photo;
using photo_album.Domain.Entities;

namespace photo_album.Application.UseCases.CommandHandlers.Photo;

internal sealed class UploadPhotoCommandHandler : IRequestHandler<UploadPhotoCommand, Result<UploadPhotoResponseDto>>
{
    private static readonly Regex InvalidFileNameCharsRegex = new(
        $"[{Regex.Escape(new string(Path.GetInvalidFileNameChars()))}]+",
        RegexOptions.Compiled);

    private readonly IPhotoRepository _photoRepository;
    private readonly IUserRepository _userRepository;
    private readonly IImageStorage _imageStorage;
    private readonly IValidator<UploadPhotoRequestDto> _validator;
    private readonly IMapper _mapper;

    public UploadPhotoCommandHandler(
        IPhotoRepository photoRepository,
        IUserRepository userRepository,
        IImageStorage imageStorage,
        IValidator<UploadPhotoRequestDto> validator,
        IMapper mapper)
    {
        _photoRepository = photoRepository;
        _userRepository = userRepository;
        _imageStorage = imageStorage;
        _validator = validator;
        _mapper = mapper;
    }

    public async Task<Result<UploadPhotoResponseDto>> Handle(
        UploadPhotoCommand request,
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

        var owner = await _userRepository.GetByIdAsync(request.OwnerId, cancellationToken);

        if (owner is null)
        {
            return Result<UploadPhotoResponseDto>.Failure(
                Error.NotFound($"User with id {request.OwnerId} does not exist")
            );
        }

        var photoId = Guid.NewGuid();
        var storedFileName = BuildStoredFileName(
            request.Request.Name,
            request.Request.OriginalFileName,
            photoId);

        try
        {
            await _imageStorage.SaveAsync(request.Content, storedFileName, cancellationToken);
        }
        catch (InvalidDataException)
        {
            return Result<UploadPhotoResponseDto>.Failure(
                Error.IncorrectDataType("Uploaded file is not a valid image")
            );
        }
        catch (Exception)
        {
            return Result<UploadPhotoResponseDto>.Failure(
                Error.InternalError("Failed to store uploaded image")
            );
        }

        var photo = new PhotoEntity
        {
            Id = photoId,
            Name = request.Request.Name.Trim(),
            OriginalFileName = storedFileName,
            ThumbnailFileName = storedFileName,
            LikesCount = 0,
            DislikesCount = 0,
            OwnerId = request.OwnerId,
            CreatedAtUtc = DateTime.UtcNow
        };

        var isCreated = await _photoRepository.AddAsync(photo, cancellationToken);

        if (!isCreated)
        {
            return Result<UploadPhotoResponseDto>.Failure(
                Error.InternalError($"Photo with id {photo.Id} was not created")
            );
        }

        var response = _mapper.Map<UploadPhotoResponseDto>(photo);

        return Result<UploadPhotoResponseDto>.Success(response);
    }

    private static string BuildStoredFileName(string photoName, string originalFileName, Guid photoId)
    {
        var extension = Path.GetExtension(originalFileName).ToLowerInvariant();
        var sanitizedName = SanitizeFileName(photoName);

        return $"{sanitizedName}_{photoId:N}{extension}";
    }

    private static string SanitizeFileName(string photoName)
    {
        var trimmedName = photoName.Trim();
        var withoutInvalidChars = InvalidFileNameCharsRegex.Replace(trimmedName, "_");
        var normalized = withoutInvalidChars
            .Replace(' ', '_')
            .Trim('_');

        if (string.IsNullOrWhiteSpace(normalized))
        {
            return "photo";
        }

        const int maxBaseNameLength = 80;
        if (normalized.Length > maxBaseNameLength)
        {
            normalized = normalized[..maxBaseNameLength];
        }

        return normalized;
    }
}
