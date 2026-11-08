using MediatR;
using photo_album.Application.Common.Results;
using photo_album.Application.Dto.Folder.Responses;

namespace photo_album.Application.UseCases.Queries.Folder;

public sealed record GetFolderByIdQuery(Guid FolderId)
    : IRequest<Result<GetFolderResponseDto>>;
