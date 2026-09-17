using FluentValidation;
using photo_album.Application.Dto.User.Requests;

namespace photo_album.Application.Validators.User;

public sealed class CreateUserRequestDtoValidator : AbstractValidator<CreateUserRequestDto>
{
    public CreateUserRequestDtoValidator()
    {
        RuleFor(user => user.UserName)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(user => user.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(256);

        RuleFor(user => user.Password)
            .NotEmpty()
            .MinimumLength(8)
            .MaximumLength(100);

        RuleFor(user => user.DisplayName)
            .NotEmpty()
            .MaximumLength(100);
    }
}
