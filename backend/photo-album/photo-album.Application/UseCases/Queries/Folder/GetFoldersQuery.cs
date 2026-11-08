using MediatR;
using photo_album.Application.Common.Results;
using photo_album.Application.Dto.Folder.Responses;

namespace photo_album.Application.UseCases.Queries.Folder;

public sealed record GetFoldersQuery(int Page, int PageSize)
    : IRequest<Result<IReadOnlyList<GetFolderResponseDto>>>;
