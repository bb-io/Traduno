using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Traduno.Handlers;

public abstract class TradunoDataSourceHandlerBase(InvocationContext invocationContext)
    : TradunoInvocable(invocationContext), IAsyncDataSourceItemHandler
{
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context,
        CancellationToken cancellationToken)
    {
        var items = await GetDataInternalAsync(cancellationToken);
        return items
            .Where(x => string.IsNullOrWhiteSpace(context.SearchString)
                        || x.DisplayName.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase)
                        || x.Value.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase))
            .Take(50);
    }

    protected abstract Task<IEnumerable<DataSourceItem>> GetDataInternalAsync(CancellationToken cancellationToken);
}
