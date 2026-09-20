namespace photo_album.Application.Common.Errors;

public class IncorrectTokenError : Error
{
    private const int IncorrectTokenStatusCode = 401;

    public IncorrectTokenError(string message) : base(IncorrectTokenStatusCode, message)
    {
    }
}
