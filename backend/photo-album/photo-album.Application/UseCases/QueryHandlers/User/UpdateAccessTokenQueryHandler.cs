using MediatR;
using photo_album.Application.Common.Errors;
using photo_album.Application.Common.Results;
using photo_album.Application.Contracts.Jwt;
using photo_album.Application.Contracts.Repositories;
using photo_album.Application.Dto.User.Responses;
using photo_album.Application.UseCases.Queries.User;

namespace photo_album.Application.UseCases.QueryHandlers.User;

internal sealed class UpdateAccessTokenQueryHandler
    : IRequestHandler<UpdateAccessTokenQuery, Result<UpdateAccessTokenResponseDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenProvider _tokenProvider;

    public UpdateAccessTokenQueryHandler(
        IUserRepository userRepository,
        ITokenProvider tokenProvider)
    {
        _userRepository = userRepository;
        _tokenProvider = tokenProvider;
    }

    public async Task<Result<UpdateAccessTokenResponseDto>> Handle(
        UpdateAccessTokenQuery request,
        CancellationToken cancellationToken)
    {
        if (request.UserId is null)
        {
            return Result<UpdateAccessTokenResponseDto>.Failure(
                Error.NotFound("ID was not found in the provided request")
            );
        }

        if (!Guid.TryParse(request.UserId, out var id))
        {
            return Result<UpdateAccessTokenResponseDto>.Failure(
                Error.IncorrectDataType("Provided ID does not match Guid format")
            );
        }

        var user = await _userRepository.GetByIdAsync(id, cancellationToken);

        if (user is null)
        {
            return Result<UpdateAccessTokenResponseDto>.Failure(
                Error.NotFound($"User with ID '{id}' was not found")
            );
        }

        if (user.RefreshToken != request.RefreshToken)
        {
            return Result<UpdateAccessTokenResponseDto>.Failure(
                Error.IncorrectToken($"Provided Refresh Token '{request.RefreshToken}' is invalid")
            );
        }

        var updatedAccessToken = _tokenProvider.GenerateAccessToken(user);
        var response = new UpdateAccessTokenResponseDto(updatedAccessToken);

        return Result<UpdateAccessTokenResponseDto>.Success(response);
    }
}
