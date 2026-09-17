using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using photo_album.Application.Common.Errors;
using photo_album.Application.Common.Results;
using photo_album.Application.Contracts.Repositories;
using photo_album.Application.Dto.User.Requests;
using photo_album.Application.Dto.User.Responses;
using photo_album.Application.UseCases.Commands.User;
using photo_album.Domain.Entities;

namespace photo_album.Application.UseCases.CommandHandlers.User;

internal sealed class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result<CreateUserResponseDto>>
{
    private readonly IUserRepository _repository;
    private readonly IValidator<CreateUserRequestDto> _validator;
    private readonly IMapper _mapper;
    private readonly IPasswordHasher<UserEntity> _passwordHasher;

    public CreateUserCommandHandler(
        IUserRepository repository,
        IValidator<CreateUserRequestDto> validator,
        IMapper mapper,
        IPasswordHasher<UserEntity> passwordHasher)
    {
        _repository = repository;
        _validator = validator;
        _mapper = mapper;
        _passwordHasher = passwordHasher;
    }

    public async Task<Result<CreateUserResponseDto>> Handle(
        CreateUserCommand request,
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request.User, cancellationToken);

        if (!validationResult.IsValid)
        {
            var message = string.Join("; ", validationResult.Errors.Select(error => error.ErrorMessage));

            return Result<CreateUserResponseDto>.Failure(
                Error.FailedValidation(message)
            );
        }

        var emailExists = await _repository.ExistsByEmailAsync(request.User.Email, cancellationToken);

        if (emailExists)
        {
            return Result<CreateUserResponseDto>.Failure(
                Error.FailedValidation($"User with email {request.User.Email} already exists")
            );
        }

        var userNameExists = await _repository.ExistsByUserNameAsync(request.User.UserName, cancellationToken);

        if (userNameExists)
        {
            return Result<CreateUserResponseDto>.Failure(
                Error.FailedValidation($"User with user name {request.User.UserName} already exists")
            );
        }

        var user = _mapper.Map<UserEntity>(request.User);
        user.PasswordHash = _passwordHasher.HashPassword(user, request.User.Password);

        var isCreated = await _repository.AddAsync(user, cancellationToken);

        if (!isCreated)
        {
            return Result<CreateUserResponseDto>.Failure(
                Error.InternalError($"User with id {user.Id} was not created")
            );
        }

        var response = _mapper.Map<CreateUserResponseDto>(user);

        return Result<CreateUserResponseDto>.Success(response);
    }
}
