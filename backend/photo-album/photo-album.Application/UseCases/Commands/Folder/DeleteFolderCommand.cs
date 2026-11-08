using MediatR;
using photo_album.Application.Common.Results;

namespace photo_album.Application.UseCases.Commands.Folder;

public sealed record DeleteFolderCommand(
    Guid FolderId,
    Guid UserId) : IRequest<Result<Guid>>;
