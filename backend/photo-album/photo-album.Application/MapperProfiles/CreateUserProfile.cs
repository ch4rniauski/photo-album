using AutoMapper;
using photo_album.Application.Dto.User.Requests;
using photo_album.Application.Dto.User.Responses;
using photo_album.Domain.Entities;

namespace photo_album.Application.MapperProfiles;

internal sealed class CreateUserProfile : Profile
{
    public CreateUserProfile()
    {
        CreateMap<CreateUserRequestDto, UserEntity>()
            .ForMember(
                dest => dest.Id,
                opt => opt.MapFrom(_ => Guid.NewGuid())
            )
            .ForMember(
                dest => dest.UserName,
                opt => opt.MapFrom(src => src.UserName)
            )
            .ForMember(
                dest => dest.Email,
                opt => opt.MapFrom(src => src.Email)
            )
            .ForMember(
                dest => dest.PasswordHash,
                opt => opt.Ignore()
            )
            .ForMember(
                dest => dest.DisplayName,
                opt => opt.MapFrom(src => src.DisplayName)
            )
            .ForMember(
                dest => dest.Role,
                opt => opt.MapFrom(_ => "User")
            );

        CreateMap<UserEntity, CreateUserResponseDto>()
            .ForMember(
                dest => dest.Id,
                opt => opt.MapFrom(src => src.Id)
            )
            .ForMember(
                dest => dest.UserName,
                opt => opt.MapFrom(src => src.UserName)
            )
            .ForMember(
                dest => dest.Email,
                opt => opt.MapFrom(src => src.Email)
            )
            .ForMember(
                dest => dest.DisplayName,
                opt => opt.MapFrom(src => src.DisplayName)
            )
            .ForMember(
                dest => dest.Role,
                opt => opt.MapFrom(src => src.Role)
            );
    }
}
