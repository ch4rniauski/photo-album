using MediatR;
using photo_album.Application.Common.Errors;
using photo_album.Application.Common.Results;
using photo_album.Application.Contracts.Activity;
using photo_album.Application.Contracts.Repositories;
using photo_album.Application.UseCases.Commands.Folder;
using photo_album.Domain.Constants;

namespace photo_album.Application.UseCases.CommandHandlers.Folder;

internal sealed class DeleteFolderCommandHandler
    : IRequestHandler<DeleteFolderCommand, Result<Guid>>
{
    private readonly IFolderRepository _folderRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUserActivityLogService _activityLogService;

    public DeleteFolderCommandHandler(
        IFolderRepository folderRepository,
        IUserRepository userRepository,
        IUserActivityLogService activityLogService)
    {
        _folderRepository = folderRepository;
        _userRepository = userRepository;
        _activityLogService = activityLogService;
    }

    public async Task<Result<Guid>> Handle(
        DeleteFolderCommand request,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

        if (user is null)
        {
            return Result<Guid>.Failure(
                Error.NotFound($"User with id {request.UserId} does not exist")
            );
        }

        var folder = await _folderRepository.GetByIdAsync(request.FolderId, cancellationToken);

        if (folder is null)
        {
            return Result<Guid>.Failure(
                Error.NotFound($"Folder with id {request.FolderId} does not exist")
            );
        }

        if (folder.OwnerId != request.UserId)
        {
            return Result<Guid>.Failure(
                Error.Forbidden("Only the folder owner can delete this folder")
            );
        }

        var folderName = folder.Name;

        var isDeleted = await _folderRepository.DeleteAsync(folder, cancellationToken);

        if (!isDeleted)
        {
            return Result<Guid>.Failure(
                Error.InternalError($"Folder with id {folder.Id} was not deleted")
            );
        }

        await _activityLogService.LogAsync(
            user.Id,
            user.UserName,
            UserActivityActions.DeleteFolder,
            entityType: "Folder",
            entityId: folder.Id,
            details: folderName,
            cancellationToken: cancellationToken);

        return Result<Guid>.Success(folder.Id);
    }
}
