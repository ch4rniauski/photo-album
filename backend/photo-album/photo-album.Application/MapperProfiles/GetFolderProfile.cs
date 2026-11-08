using AutoMapper;
using photo_album.Application.Dto.Folder.Responses;
using photo_album.Domain.Entities;

namespace photo_album.Application.MapperProfiles;

internal sealed class GetFolderProfile : Profile
{
    public GetFolderProfile()
    {
        CreateMap<FolderEntity, GetFolderResponseDto>()
            .ConstructUsing(src => new GetFolderResponseDto(
                src.Id,
                src.Name,
                src.OwnerId,
                src.Owner.UserName,
                0,
                src.CreatedAtUtc));
    }
}
