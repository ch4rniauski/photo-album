using AutoMapper;
using MediatR;
using photo_album.Application.Common.Errors;
using photo_album.Application.Common.Results;
using photo_album.Application.Contracts.Repositories;
using photo_album.Application.Dto.Folder.Responses;
using photo_album.Application.UseCases.Queries.Folder;

namespace photo_album.Application.UseCases.QueryHandlers.Folder;

internal sealed class GetFolderByIdQueryHandler
    : IRequestHandler<GetFolderByIdQuery, Result<GetFolderResponseDto>>
{
    private readonly IFolderRepository _folderRepository;
    private readonly IMapper _mapper;

    public GetFolderByIdQueryHandler(
        IFolderRepository folderRepository,
        IMapper mapper)
    {
        _folderRepository = folderRepository;
        _mapper = mapper;
    }

    public async Task<Result<GetFolderResponseDto>> Handle(
        GetFolderByIdQuery request,
        CancellationToken cancellationToken)
    {
        var folder = await _folderRepository.GetByIdWithOwnerAsync(
            request.FolderId,
            cancellationToken);

        if (folder is null)
        {
            return Result<GetFolderResponseDto>.Failure(
                Error.NotFound($"Folder with id {request.FolderId} does not exist")
            );
        }

        var photosCount = await _folderRepository.GetPhotosCountAsync(
            folder.Id,
            cancellationToken);

        var response = _mapper.Map<GetFolderResponseDto>(folder) with
        {
            PhotosCount = photosCount
        };

        return Result<GetFolderResponseDto>.Success(response);
    }
}
