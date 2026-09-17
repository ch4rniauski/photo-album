namespace photo_album.Application.Common.Errors;

public class ValidationError : Error
{
    private const int FailedValidationStatusCode = 400;
    
    public ValidationError(string message) : base(FailedValidationStatusCode, message)
    {
    }
}