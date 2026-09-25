namespace frontend.Models;

public sealed record OriginalPhotoContent(
    byte[] Bytes,
    string ContentType);
