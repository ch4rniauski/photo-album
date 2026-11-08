using MediatR;
using photo_album.Application.Common.Results;

namespace photo_album.Application.UseCases.Commands.Photo;

public sealed record DeletePhotoCommand(
    Guid PhotoId,
    Guid UserId) : IRequest<Result<Guid>>;
