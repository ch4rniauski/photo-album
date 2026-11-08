using AutoMapper;
using MediatR;
using photo_album.Application.Common.Errors;
using photo_album.Application.Common.Results;
using photo_album.Application.Contracts.Repositories;
using photo_album.Application.Dto.Folder.Responses;
using photo_album.Application.UseCases.Queries.Folder;

namespace photo_album.Application.UseCases.QueryHandlers.Folder;

internal sealed class GetMyFoldersQueryHandler
    : IRequestHandler<GetMyFoldersQuery, Result<IReadOnlyList<GetFolderResponseDto>>>
{
    private readonly IFolderRepository _folderRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public GetMyFoldersQueryHandler(
        IFolderRepository folderRepository,
        IUserRepository userRepository,
        IMapper mapper)
    {
        _folderRepository = folderRepository;
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<Result<IReadOnlyList<GetFolderResponseDto>>> Handle(
        GetMyFoldersQuery request,
        CancellationToken cancellationToken)
    {
        var owner = await _userRepository.GetByIdAsync(request.OwnerId, cancellationToken);

        if (owner is null)
        {
            return Result<IReadOnlyList<GetFolderResponseDto>>.Failure(
                Error.NotFound($"User with id {request.OwnerId} does not exist")
            );
        }

        var folders = await _folderRepository.GetByOwnerIdAsync(
            request.OwnerId,
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
