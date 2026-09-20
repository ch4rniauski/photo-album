using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using photo_album.Application.Common.Errors;
using photo_album.Application.Common.Results;
using photo_album.Application.Contracts.Jwt;
using photo_album.Application.Contracts.Repositories;
using photo_album.Application.Dto.User.Requests;
using photo_album.Application.Dto.User.Responses;
using photo_album.Application.UseCases.Commands.User;
using photo_album.Domain.Entities;

namespace photo_album.Application.UseCases.CommandHandlers.User;

internal sealed class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, Result<LoginUserResponseDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenProvider _tokenProvider;
    private readonly IValidator<LoginUserRequestDto> _validator;
    private readonly IPasswordHasher<UserEntity> _passwordHasher;

    public LoginUserCommandHandler(
        IUserRepository userRepository,
        ITokenProvider tokenProvider,
        IValidator<LoginUserRequestDto> validator,
        IPasswordHasher<UserEntity> passwordHasher)
    {
        _userRepository = userRepository;
        _tokenProvider = tokenProvider;
        _validator = validator;
        _passwordHasher = passwordHasher;
    }

    public async Task<Result<LoginUserResponseDto>> Handle(
        LoginUserCommand request,
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request.Request, cancellationToken);

        if (!validationResult.IsValid)
        {
            var message = string.Join("; ", validationResult.Errors.Select(error => error.ErrorMessage));

            return Result<LoginUserResponseDto>.Failure(
                Error.FailedValidation(message)
            );
        }

        var user = await _userRepository.GetByEmailAsync(request.Request.Email, cancellationToken);

        if (user is null)
        {
            return Result<LoginUserResponseDto>.Failure(
                Error.NotFound($"User with Email {request.Request.Email} does not exist")
            );
        }

        var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(
            user,
            user.PasswordHash,
            request.Request.Password);

        if (passwordVerificationResult != PasswordVerificationResult.Success)
        {
            return Result<LoginUserResponseDto>.Failure(
                Error.Unauthorized("Invalid password")
            );
        }

        var accessToken = _tokenProvider.GenerateAccessToken(user);
        var refreshToken = _tokenProvider.GenerateRefreshToken();

        user.RefreshToken = refreshToken;

        var isUpdated = await _userRepository.UpdateAsync(user, cancellationToken);

        if (!isUpdated)
        {
            return Result<LoginUserResponseDto>.Failure(
                Error.InternalError("Exception was thrown while updating user refresh token")
            );
        }

        var response = new LoginUserResponseDto(
            accessToken,
            refreshToken,
            user.Id.ToString());

        return Result<LoginUserResponseDto>.Success(response);
    }
}
