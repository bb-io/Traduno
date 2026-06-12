using Apps.Traduno.Models.Dtos;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Traduno.Handlers;

public class TranslationAreaDataHandler(InvocationContext invocationContext) : TradunoDataSourceHandlerBase(invocationContext)
{
    protected override async Task<IEnumerable<DataSourceItem>> GetDataInternalAsync(CancellationToken cancellationToken)
    {
        var translationAreas = await Client.GetAllPaginatedAsync<TranslationAreaDto>("/translation_areas",
            cancellationToken: cancellationToken);
        return translationAreas.Select(x => new DataSourceItem(x.Id.ToString(), x.Name));
    }
}
