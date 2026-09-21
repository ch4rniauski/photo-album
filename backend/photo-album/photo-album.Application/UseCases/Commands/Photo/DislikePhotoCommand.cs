using MediatR;
using photo_album.Application.Common.Results;
using photo_album.Application.Dto.Photo.Responses;

namespace photo_album.Application.UseCases.Commands.Photo;

public sealed record DislikePhotoCommand(
    Guid PhotoId,
    Guid UserId) : IRequest<Result<UploadPhotoResponseDto>>;
