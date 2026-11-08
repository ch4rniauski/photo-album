using FluentValidation;
using photo_album.Application.Dto.Folder.Requests;

namespace photo_album.Application.Validators.Folder;

public sealed class CreateFolderRequestDtoValidator : AbstractValidator<CreateFolderRequestDto>
{
    public CreateFolderRequestDtoValidator()
    {
        RuleFor(request => request.Name)
            .NotEmpty()
            .MaximumLength(100);
    }
}
