namespace Apps.Traduno.Models.Api;

public class PaginatedResponse<T>
{
    public int Page { get; set; }

    public int Limit { get; set; }

    public int Total { get; set; }

    public int TotalPages { get; set; }

    public IEnumerable<T>? Items { get; set; }
}
