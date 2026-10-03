namespace photo_album.Application.Common.Errors;

public class ForbiddenError : Error
{
    private const int ForbiddenStatusCode = 403;

    public ForbiddenError(string message) : base(ForbiddenStatusCode, message)
    {
    }
}
