using AutoMapper;
using MediatR;
using photo_album.Application.Common.Errors;
using photo_album.Application.Common.Results;
using photo_album.Application.Contracts.Repositories;
using photo_album.Application.Dto.Photo.Responses;
using photo_album.Application.UseCases.Queries.Folder;

namespace photo_album.Application.UseCases.QueryHandlers.Folder;

internal sealed class GetFolderPhotosQueryHandler
    : IRequestHandler<GetFolderPhotosQuery, Result<IReadOnlyList<GetPhotoResponseDto>>>
{
    private readonly IFolderRepository _folderRepository;
    private readonly IPhotoRepository _photoRepository;
    private readonly IMapper _mapper;

    public GetFolderPhotosQueryHandler(
        IFolderRepository folderRepository,
        IPhotoRepository photoRepository,
        IMapper mapper)
    {
        _folderRepository = folderRepository;
        _photoRepository = photoRepository;
        _mapper = mapper;
    }

    public async Task<Result<IReadOnlyList<GetPhotoResponseDto>>> Handle(
        GetFolderPhotosQuery request,
        CancellationToken cancellationToken)
    {
        var folder = await _folderRepository.GetByIdAsync(request.FolderId, cancellationToken);

        if (folder is null)
        {
            return Result<IReadOnlyList<GetPhotoResponseDto>>.Failure(
                Error.NotFound($"Folder with id {request.FolderId} does not exist")
            );
        }

        var page = request.Page < 1
            ? 1
            : request.Page;
        var pageSize = request.PageSize < 1
            ? 20
            : request.PageSize;

        var photos = await _photoRepository.GetByFolderIdWithPaginationAsync(
            request.FolderId,
            page,
            pageSize,
            cancellationToken);

        IReadOnlyList<GetPhotoResponseDto> response = _mapper.Map<List<GetPhotoResponseDto>>(photos);

        return Result<IReadOnlyList<GetPhotoResponseDto>>.Success(response);
    }
}
