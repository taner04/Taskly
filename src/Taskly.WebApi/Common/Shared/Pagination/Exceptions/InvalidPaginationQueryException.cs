using Taskly.Shared.Pagination;

namespace Taskly.WebApi.Common.Shared.Pagination.Exceptions;

internal sealed class InvalidPaginationQueryException : TasklyException
{
    private InvalidPaginationQueryException(string title, string message, string errorCode) : base(title, message,
        errorCode,
        HttpStatusCode.BadRequest)
    {
    }

    public static void ThrowIfInvalidPaginationQuery(PaginationQuery paginationQuery)
    {
        if (paginationQuery is { PageIndex: < 0, PageSize: > PaginationService.MaxPageSize or < 1 })
        {
            throw new InvalidPaginationQueryException(
                "Invalid pagination query",
                $"Page index must be at least 1 and page size must be between 1 and {PaginationService.MaxPageSize}",
                "InvalidPaginationQuery");
        }
    }
}