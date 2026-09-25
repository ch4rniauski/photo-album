using AutoMapper;
using photo_album.Application.Dto.Photo.Responses;
using photo_album.Domain.Entities;

namespace photo_album.Application.MapperProfiles;

internal sealed class GetPhotoProfile : Profile
{
    public GetPhotoProfile()
    {
        CreateMap<PhotoEntity, GetPhotoResponseDto>()
            .ConstructUsing(src => new GetPhotoResponseDto(
                src.Id,
                src.Name,
                src.LikesCount,
                src.DislikesCount,
                src.OwnerId,
                $"api/Photos/{src.Id}/thumbnail"));
    }
}
