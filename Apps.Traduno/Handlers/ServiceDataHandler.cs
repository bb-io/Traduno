using Apps.Traduno.Models.Dtos;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Traduno.Handlers;

public class ServiceDataHandler(InvocationContext invocationContext) : TradunoDataSourceHandlerBase(invocationContext)
{
    protected override async Task<IEnumerable<DataSourceItem>> GetDataInternalAsync(CancellationToken cancellationToken)
    {
        var services = await Client.GetAllPaginatedAsync<ServiceDto>("/services", cancellationToken: cancellationToken);
        return services.Select(x => new DataSourceItem(x.Code, $"{x.Name} ({x.Code})"));
    }
}
