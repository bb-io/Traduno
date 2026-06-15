using Apps.Traduno.Models.Dtos;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Traduno.Handlers;

public class BillingEntityDataHandler(InvocationContext invocationContext) : TradunoDataSourceHandlerBase(invocationContext)
{
    protected override async Task<IEnumerable<DataSourceItem>> GetDataInternalAsync(CancellationToken cancellationToken)
    {
        var billingEntities = await Client.GetAllPaginatedAsync<BillingEntityDto>("/billing_entities",
            cancellationToken: cancellationToken);
        return billingEntities.Select(x =>
            new DataSourceItem(x.Id, x.Default ? $"{x.Name} (default)" : x.Name));
    }
}
