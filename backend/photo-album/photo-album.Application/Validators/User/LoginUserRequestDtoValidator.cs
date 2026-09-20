using FluentValidation;
using photo_album.Application.Dto.User.Requests;

namespace photo_album.Application.Validators.User;

public sealed class LoginUserRequestDtoValidator : AbstractValidator<LoginUserRequestDto>
{
    public LoginUserRequestDtoValidator()
    {
        RuleFor(user => user.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(256);

        RuleFor(user => user.Password)
            .NotEmpty()
            .MinimumLength(8)
            .MaximumLength(100);
    }
}
