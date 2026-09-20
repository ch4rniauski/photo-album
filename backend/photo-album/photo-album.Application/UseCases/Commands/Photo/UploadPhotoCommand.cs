using MediatR;
using photo_album.Application.Common.Results;
using photo_album.Application.Dto.Photo.Requests;
using photo_album.Application.Dto.Photo.Responses;

namespace photo_album.Application.UseCases.Commands.Photo;

public sealed record UploadPhotoCommand(
    Guid OwnerId,
    UploadPhotoRequestDto Request,
    Stream Content) : IRequest<Result<UploadPhotoResponseDto>>;
