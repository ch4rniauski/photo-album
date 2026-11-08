namespace photo_album.Application.Contracts.Storage;

public interface IImageStorage
{
    Task SaveAsync(
        Stream content,
        string storedFileName,
        CancellationToken cancellationToken = default);

    bool ExistsOriginal(string storedFileName);

    void Delete(string originalFileName, string thumbnailFileName);

    Stream? OpenOriginal(string storedFileName);

    Stream? OpenThumbnail(string storedFileName);
}
