using FluentValidation;
using photo_album.Application.Dto.Photo.Requests;

namespace photo_album.Application.Validators.Photo;

public sealed class RenamePhotoRequestDtoValidator : AbstractValidator<RenamePhotoRequestDto>
{
    public RenamePhotoRequestDtoValidator()
    {
        RuleFor(request => request.Name)
            .NotEmpty()
            .MaximumLength(100);
    }
}
