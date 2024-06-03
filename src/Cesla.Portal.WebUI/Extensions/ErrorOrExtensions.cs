using ErrorOr;

namespace Cesla.Portal.WebUI.Extensions;

internal static class ErrorOrExtensions
{
    internal static string GetErrorOrDefault<T> (this ErrorOr<T> errorOr, string defaultValue = "Something bad happened")
    {
        if (errorOr.ErrorsOrEmptyList.Count is 0)
        {
            return defaultValue;
        }
        
        return errorOr.ErrorsOrEmptyList
            .Aggregate(
                string.Empty,
                (fullError, error) =>
                {
                    if (string.IsNullOrWhiteSpace(fullError))
                    {
                        return error.Description;
                    }
                    return $"{fullError}, {error.Description}";
                });        
    }
}
