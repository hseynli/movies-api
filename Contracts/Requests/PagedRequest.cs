namespace Contracts.Requests;

public class PagedRequest
{
    public const int DefaultPage = 1;
    public const int DefaultPageSize = 10;

    public required int? Page { get; init; } = DefaultPage;

    public required int? PageSize { get; init; } = DefaultPageSize;
}
