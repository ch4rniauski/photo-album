using FluentValidation;
using photo_album.Application.Dto.Photo.Requests;

namespace photo_album.Application.Validators.Photo;

public sealed class UploadPhotoRequestDtoValidator : AbstractValidator<UploadPhotoRequestDto>
{
    public const long MaxFileSizeBytes = 5 * 1024 * 1024;

    private static readonly HashSet<string> AllowedContentTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "image/jpeg",
        "image/jpg",
        "image/png",
        "image/gif",
        "image/webp"
    };

    private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg",
        ".jpeg",
        ".png",
        ".gif",
        ".webp"
    };

    public UploadPhotoRequestDtoValidator()
    {
        RuleFor(request => request.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(request => request.Length)
            .GreaterThan(0)
                .WithMessage("Image file is required")
            .LessThanOrEqualTo(MaxFileSizeBytes)
                .WithMessage($"Image file size must not exceed {MaxFileSizeBytes / (1024 * 1024)} MB");

        RuleFor(request => request.ContentType)
            .NotEmpty()
            .Must(contentType => AllowedContentTypes.Contains(contentType))
                .WithMessage("Only JPEG, PNG, GIF and WebP images are allowed");

        RuleFor(request => request.OriginalFileName)
            .NotEmpty()
            .Must(fileName => AllowedExtensions.Contains(Path.GetExtension(fileName)))
                .WithMessage($"Only {string.Join(", ", AllowedExtensions)} images are allowed");
    }
}
