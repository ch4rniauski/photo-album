namespace photo_album.Application.Common.Errors;

public class UnauthorizedError : Error
{
    private const int UnauthorizedStatusCode = 401;

    public UnauthorizedError(string message) : base(UnauthorizedStatusCode, message)
    {
    }
}
