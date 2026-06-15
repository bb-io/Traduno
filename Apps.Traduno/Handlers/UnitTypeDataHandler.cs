using Apps.Traduno.Models.Dtos;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Traduno.Handlers;

public class UnitTypeDataHandler(InvocationContext invocationContext) : TradunoDataSourceHandlerBase(invocationContext)
{
    protected override async Task<IEnumerable<DataSourceItem>> GetDataInternalAsync(CancellationToken cancellationToken)
    {
        var unitTypes = await Client.GetAllPaginatedAsync<UnitTypeDto>("/unit_types", cancellationToken: cancellationToken);
        return unitTypes.Select(x => new DataSourceItem(x.Id.ToString(), x.Name));
    }
}
