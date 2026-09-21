using AutoMapper;
using MediatR;
using photo_album.Application.Common.Results;
using photo_album.Application.Contracts.Repositories;
using photo_album.Application.Dto.Photo.Responses;
using photo_album.Application.UseCases.Queries.Photo;

namespace photo_album.Application.UseCases.QueryHandlers.Photo;

internal sealed class SearchPhotosQueryHandler
    : IRequestHandler<SearchPhotosQuery, Result<IReadOnlyList<SearchPhotoResponseDto>>>
{
    private readonly IPhotoRepository _photoRepository;
    private readonly IMapper _mapper;

    public SearchPhotosQueryHandler(
        IPhotoRepository photoRepository,
        IMapper mapper)
    {
        _photoRepository = photoRepository;
        _mapper = mapper;
    }

    public async Task<Result<IReadOnlyList<SearchPhotoResponseDto>>> Handle(
        SearchPhotosQuery request,
        CancellationToken cancellationToken)
    {
        var search = request.Search.Trim();

        if (string.IsNullOrWhiteSpace(search))
        {
            return Result<IReadOnlyList<SearchPhotoResponseDto>>.Success([]);
        }

        var photos = await _photoRepository.SearchByNameAsync(search, cancellationToken);

        IReadOnlyList<SearchPhotoResponseDto> response = _mapper.Map<List<SearchPhotoResponseDto>>(photos);

        return Result<IReadOnlyList<SearchPhotoResponseDto>>.Success(response);
    }
}
