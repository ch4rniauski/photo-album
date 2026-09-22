using AutoMapper;
using MediatR;
using photo_album.Application.Common.Results;
using photo_album.Application.Contracts.Repositories;
using photo_album.Application.Dto.Photo.Responses;
using photo_album.Application.UseCases.Queries.Photo;

namespace photo_album.Application.UseCases.QueryHandlers.Photo;

internal sealed class GetPhotosQueryHandler
    : IRequestHandler<GetPhotosQuery, Result<IReadOnlyList<GetPhotoResponseDto>>>
{
    private readonly IPhotoRepository _photoRepository;
    private readonly IMapper _mapper;

    public GetPhotosQueryHandler(
        IPhotoRepository photoRepository,
        IMapper mapper)
    {
        _photoRepository = photoRepository;
        _mapper = mapper;
    }

    public async Task<Result<IReadOnlyList<GetPhotoResponseDto>>> Handle(
        GetPhotosQuery request,
        CancellationToken cancellationToken)
    {
        var page = request.Page < 1
            ? 1
            : request.Page;
        var pageSize = request.PageSize < 1
            ? 20
            : request.PageSize;

        var photos = await _photoRepository.GetWithPaginationAsync(
            page,
            pageSize,
            cancellationToken);

        IReadOnlyList<GetPhotoResponseDto> response = _mapper.Map<List<GetPhotoResponseDto>>(photos);

        return Result<IReadOnlyList<GetPhotoResponseDto>>.Success(response);
    }
}
