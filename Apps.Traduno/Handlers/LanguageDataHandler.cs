using Apps.Traduno.Models.Dtos;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Traduno.Handlers;

public class LanguageDataHandler(InvocationContext invocationContext) : TradunoDataSourceHandlerBase(invocationContext)
{
    protected override async Task<IEnumerable<DataSourceItem>> GetDataInternalAsync(CancellationToken cancellationToken)
    {
        var languages = await Client.GetAllPaginatedAsync<LanguageDto>("/languages", cancellationToken: cancellationToken);
        return languages.Select(x => new DataSourceItem(x.Code, $"{x.Name} ({x.Code})"));
    }
}
