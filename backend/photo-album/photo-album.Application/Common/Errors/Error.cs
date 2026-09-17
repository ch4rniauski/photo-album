namespace photo_album.Application.Common.Errors;

public abstract class Error
{
    public int StatusCode { get; set; }

    public string Description { get; set; }

    protected Error(int statusCode, string description)
    {
        StatusCode = statusCode;
        Description = description;
    }

    public static ValidationError FailedValidation(string message)
        => new ValidationError(message);
    
    public static InternalServerError InternalError(string message)
        => new(message);
    
    public static NotFoundError NotFound(string message)
        => new(message);
}
