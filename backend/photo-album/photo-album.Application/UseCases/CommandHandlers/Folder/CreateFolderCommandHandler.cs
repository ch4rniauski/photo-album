using FluentValidation;
using MediatR;
using photo_album.Application.Common.Errors;
using photo_album.Application.Common.Results;
using photo_album.Application.Contracts.Activity;
using photo_album.Application.Contracts.Repositories;
using photo_album.Application.Dto.Folder.Requests;
using photo_album.Application.Dto.Folder.Responses;
using photo_album.Application.UseCases.Commands.Folder;
using photo_album.Domain.Constants;
using photo_album.Domain.Entities;

namespace photo_album.Application.UseCases.CommandHandlers.Folder;

internal sealed class CreateFolderCommandHandler
    : IRequestHandler<CreateFolderCommand, Result<GetFolderResponseDto>>
{
    private readonly IFolderRepository _folderRepository;
    private readonly IUserRepository _userRepository;
    private readonly IValidator<CreateFolderRequestDto> _validator;
    private readonly IUserActivityLogService _activityLogService;

    public CreateFolderCommandHandler(
        IFolderRepository folderRepository,
        IUserRepository userRepository,
        IValidator<CreateFolderRequestDto> validator,
        IUserActivityLogService activityLogService)
    {
        _folderRepository = folderRepository;
        _userRepository = userRepository;
        _validator = validator;
        _activityLogService = activityLogService;
    }

    public async Task<Result<GetFolderResponseDto>> Handle(
        CreateFolderCommand request,
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request.Request, cancellationToken);

        if (!validationResult.IsValid)
        {
            var message = string.Join("; ", validationResult.Errors.Select(error => error.ErrorMessage));

            return Result<GetFolderResponseDto>.Failure(
                Error.FailedValidation(message)
            );
        }

        var owner = await _userRepository.GetByIdAsync(request.OwnerId, cancellationToken);

        if (owner is null)
        {
            return Result<GetFolderResponseDto>.Failure(
                Error.NotFound($"User with id {request.OwnerId} does not exist")
            );
        }

        var folder = new FolderEntity
        {
            Id = Guid.NewGuid(),
            Name = request.Request.Name.Trim(),
            OwnerId = request.OwnerId,
            CreatedAtUtc = DateTime.UtcNow
        };

        var isCreated = await _folderRepository.AddAsync(folder, cancellationToken);

        if (!isCreated)
        {
            return Result<GetFolderResponseDto>.Failure(
                Error.InternalError($"Folder with id {folder.Id} was not created")
            );
        }

        await _activityLogService.LogAsync(
            owner.Id,
            owner.UserName,
            UserActivityActions.CreateFolder,
            entityType: "Folder",
            entityId: folder.Id,
            details: folder.Name,
            cancellationToken: cancellationToken);

        var response = new GetFolderResponseDto(
            folder.Id,
            folder.Name,
            folder.OwnerId,
            owner.UserName,
            0,
            folder.CreatedAtUtc);

        return Result<GetFolderResponseDto>.Success(response);
    }
}
