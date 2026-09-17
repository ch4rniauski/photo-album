using photo_album.Application.Common.Errors;
using photo_album.Application.Common.Results;

namespace photo_album.Application.Extensions;

public static class ResultExtension
{
    public static TResult Match<TValue, TResult>(
        this Result<TValue> result,
        Func<TValue, TResult> onSuccess,
        Func<Error ,TResult> onFailure)
    {
        return result.IsSuccess
                ? onSuccess(result.Value!)
                : onFailure(result.Error!);
    }
}
