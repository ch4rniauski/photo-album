namespace photo_album.Domain.Constants;

public static class UserActivityActions
{
    public const string Register = "Register";
    public const string Login = "Login";
    public const string LoginFailed = "LoginFailed";
    public const string UploadPhoto = "UploadPhoto";
    public const string LikePhoto = "LikePhoto";
    public const string DislikePhoto = "DislikePhoto";
    public const string RemoveLikePhoto = "RemoveLikePhoto";
    public const string RemoveDislikePhoto = "RemoveDislikePhoto";
    public const string RenamePhoto = "RenamePhoto";
    public const string DeletePhoto = "DeletePhoto";
    public const string CreateFolder = "CreateFolder";
    public const string DeleteFolder = "DeleteFolder";
    public const string MovePhotoToFolder = "MovePhotoToFolder";
}
