using AutoMapper;
using photo_album.Application.Dto.Photo.Responses;
using photo_album.Domain.Entities;

namespace photo_album.Application.MapperProfiles;

internal sealed class UploadPhotoProfile : Profile
{
    public UploadPhotoProfile()
    {
        CreateMap<PhotoEntity, UploadPhotoResponseDto>()
            .ForMember(
                dest => dest.Id,
                opt => opt.MapFrom(src => src.Id)
            )
            .ForMember(
                dest => dest.Name,
                opt => opt.MapFrom(src => src.Name)
            )
            .ForMember(
                dest => dest.OriginalFileName,
                opt => opt.MapFrom(src => src.OriginalFileName)
            )
            .ForMember(
                dest => dest.ThumbnailFileName,
                opt => opt.MapFrom(src => src.ThumbnailFileName)
            )
            .ForMember(
                dest => dest.LikesCount,
                opt => opt.MapFrom(src => src.LikesCount)
            )
            .ForMember(
                dest => dest.DislikesCount,
                opt => opt.MapFrom(src => src.DislikesCount)
            )
            .ForMember(
                dest => dest.OwnerId,
                opt => opt.MapFrom(src => src.OwnerId)
            );
    }
}
