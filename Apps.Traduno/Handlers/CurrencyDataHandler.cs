using Apps.Traduno.Models.Dtos;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Traduno.Handlers;

public class CurrencyDataHandler(InvocationContext invocationContext) : TradunoDataSourceHandlerBase(invocationContext)
{
    protected override async Task<IEnumerable<DataSourceItem>> GetDataInternalAsync(CancellationToken cancellationToken)
    {
        var currencies = await Client.GetAllPaginatedAsync<CurrencyDto>("/currencies", cancellationToken: cancellationToken);
        return currencies.Select(x => new DataSourceItem(x.Id.ToString(), $"{x.Name} ({x.Code})"));
    }
}
