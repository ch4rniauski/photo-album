using MediatR;
using photo_album.Application.Common.Results;
using photo_album.Application.Dto.Photo.Responses;

namespace photo_album.Application.UseCases.Queries.Folder;

public sealed record GetFolderPhotosQuery(Guid FolderId, int Page, int PageSize)
    : IRequest<Result<IReadOnlyList<GetPhotoResponseDto>>>;
