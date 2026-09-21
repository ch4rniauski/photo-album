using AutoMapper;
using photo_album.Application.Dto.Photo.Responses;
using photo_album.Domain.Entities;

namespace photo_album.Application.MapperProfiles;

internal sealed class SearchPhotoProfile : Profile
{
    public SearchPhotoProfile()
    {
        CreateMap<PhotoEntity, SearchPhotoResponseDto>()
            .ConstructUsing(src => new SearchPhotoResponseDto(
                src.Id,
                src.Name,
                src.LikesCount,
                src.DislikesCount,
                src.OwnerId,
                $"storage/images/thumbnails/{src.ThumbnailFileName}"));
    }
}
