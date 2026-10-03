using MediatR;
using photo_album.Application.Common.Errors;
using photo_album.Application.Common.Results;
using photo_album.Application.Contracts.Repositories;
using photo_album.Application.Dto.Photo.Responses;
using photo_album.Application.UseCases.Queries.Photo;

namespace photo_album.Application.UseCases.QueryHandlers.Photo;

internal sealed class GetMyPhotoReactionQueryHandler
    : IRequestHandler<GetMyPhotoReactionQuery, Result<PhotoReactionResponseDto>>
{
    private readonly IPhotoRepository _photoRepository;
    private readonly IPhotoReactionRepository _photoReactionRepository;

    public GetMyPhotoReactionQueryHandler(
        IPhotoRepository photoRepository,
        IPhotoReactionRepository photoReactionRepository)
    {
        _photoRepository = photoRepository;
        _photoReactionRepository = photoReactionRepository;
    }

    public async Task<Result<PhotoReactionResponseDto>> Handle(
        GetMyPhotoReactionQuery request,
        CancellationToken cancellationToken)
    {
        var photo = await _photoRepository.GetByIdAsync(request.PhotoId, cancellationToken);

        if (photo is null)
        {
            return Result<PhotoReactionResponseDto>.Failure(
                Error.NotFound($"Photo with id {request.PhotoId} does not exist")
            );
        }

        var reaction = await _photoReactionRepository.GetByPhotoIdAndUserIdAsync(
            request.PhotoId,
            request.UserId,
            cancellationToken);

        return Result<PhotoReactionResponseDto>.Success(
            new PhotoReactionResponseDto(reaction?.IsLiked)
        );
    }
}
