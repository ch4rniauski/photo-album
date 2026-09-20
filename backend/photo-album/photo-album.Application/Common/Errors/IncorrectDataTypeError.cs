namespace photo_album.Application.Common.Errors;

public class IncorrectDataTypeError : Error
{
    private const int IncorrectDataTypeStatusCode = 400;

    public IncorrectDataTypeError(string message) : base(IncorrectDataTypeStatusCode, message)
    {
    }
}
