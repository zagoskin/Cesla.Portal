using ErrorOr;

namespace Cesla.Portal.Application.Common;
internal static class GeneralErrors
{
    internal static Error NotFound<TIdType>(TIdType id, Type type) =>
        Error.NotFound(description: $"{type.Name} with id {id} not found");

    internal static Error Conflict(string message) =>
        Error.Conflict(description: message);

    internal static Error Unexpected(string message) =>
        Error.Unexpected(description: message);
}
