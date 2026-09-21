using MediatR;
using photo_album.Application.Common.Results;
using photo_album.Application.Dto.Photo.Responses;

namespace photo_album.Application.UseCases.Queries.Photo;

public sealed record SearchPhotosQuery(string Search)
    : IRequest<Result<IReadOnlyList<SearchPhotoResponseDto>>>;
