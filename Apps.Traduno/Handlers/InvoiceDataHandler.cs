using Apps.Traduno.Models.Dtos;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Traduno.Handlers;

public class InvoiceDataHandler(InvocationContext invocationContext) : TradunoDataSourceHandlerBase(invocationContext)
{
    protected override async Task<IEnumerable<DataSourceItem>> GetDataInternalAsync(CancellationToken cancellationToken)
    {
        var invoices = await Client.GetAllPaginatedAsync<InvoiceDto>("/invoices", cancellationToken: cancellationToken);
        return invoices.Select(x => new DataSourceItem(x.Id, $"{x.Name} ({x.Id})"));
    }
}
