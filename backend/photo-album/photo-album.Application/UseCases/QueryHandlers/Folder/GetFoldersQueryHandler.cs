using AutoMapper;
using MediatR;
using photo_album.Application.Common.Results;
using photo_album.Application.Contracts.Repositories;
using photo_album.Application.Dto.Folder.Responses;
using photo_album.Application.UseCases.Queries.Folder;

namespace photo_album.Application.UseCases.QueryHandlers.Folder;

internal sealed class GetFoldersQueryHandler
    : IRequestHandler<GetFoldersQuery, Result<IReadOnlyList<GetFolderResponseDto>>>
{
    private readonly IFolderRepository _folderRepository;
    private readonly IMapper _mapper;

    public GetFoldersQueryHandler(
        IFolderRepository folderRepository,
        IMapper mapper)
    {
        _folderRepository = folderRepository;
        _mapper = mapper;
    }

    public async Task<Result<IReadOnlyList<GetFolderResponseDto>>> Handle(
        GetFoldersQuery request,
        CancellationToken cancellationToken)
    {
        var page = request.Page < 1
            ? 1
            : request.Page;
        var pageSize = request.PageSize < 1
            ? 20
            : request.PageSize;

        var folders = await _folderRepository.GetWithPaginationAsync(
            page,
            pageSize,
            cancellationToken);

        var response = new List<GetFolderResponseDto>(folders.Count);

        foreach (var folder in folders)
        {
            var photosCount = await _folderRepository.GetPhotosCountAsync(
                folder.Id,
                cancellationToken);

            response.Add(_mapper.Map<GetFolderResponseDto>(folder) with
            {
                PhotosCount = photosCount
            });
        }

        return Result<IReadOnlyList<GetFolderResponseDto>>.Success(response);
    }
}
