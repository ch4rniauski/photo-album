using MediatR;
using photo_album.Application.Common.Results;
using photo_album.Application.Dto.Folder.Requests;
using photo_album.Application.Dto.Folder.Responses;

namespace photo_album.Application.UseCases.Commands.Folder;

public sealed record CreateFolderCommand(
    Guid OwnerId,
    CreateFolderRequestDto Request) : IRequest<Result<GetFolderResponseDto>>;
