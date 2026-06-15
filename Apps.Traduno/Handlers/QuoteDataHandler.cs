using Apps.Traduno.Models.Dtos;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Traduno.Handlers;

public class QuoteDataHandler(InvocationContext invocationContext) : TradunoDataSourceHandlerBase(invocationContext)
{
    protected override async Task<IEnumerable<DataSourceItem>> GetDataInternalAsync(CancellationToken cancellationToken)
    {
        var quotes = await Client.GetAllPaginatedAsync<QuoteDto>(
            "/quotes",
            new Dictionary<string, string?> { ["filter[show_all]"] = "true" },
            cancellationToken);
        return quotes.Select(x =>
            new DataSourceItem(x.Id, string.IsNullOrWhiteSpace(x.Name) ? x.Id : $"{x.Name} ({x.Id})"));
    }
}
