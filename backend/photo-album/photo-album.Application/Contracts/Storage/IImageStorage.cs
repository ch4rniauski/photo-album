namespace photo_album.Application.Contracts.Storage;

public interface IImageStorage
{
    Task SaveAsync(
        Stream content,
        string storedFileName,
        CancellationToken cancellationToken = default);
}
