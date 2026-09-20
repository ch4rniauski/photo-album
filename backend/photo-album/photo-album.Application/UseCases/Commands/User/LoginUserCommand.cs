using MediatR;
using photo_album.Application.Common.Results;
using photo_album.Application.Dto.User.Requests;
using photo_album.Application.Dto.User.Responses;

namespace photo_album.Application.UseCases.Commands.User;

public sealed record LoginUserCommand(LoginUserRequestDto Request) : IRequest<Result<LoginUserResponseDto>>;
