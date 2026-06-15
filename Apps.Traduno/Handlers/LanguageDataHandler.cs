using Apps.Traduno.Models.Dtos;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Traduno.Handlers;

public class LanguageDataHandler(InvocationContext invocationContext) : TradunoDataSourceHandlerBase(invocationContext)
{
    protected override async Task<IEnumerable<DataSourceItem>> GetDataInternalAsync(CancellationToken cancellationToken)
    {
        var languages = await Client.GetAllPaginatedAsync<LanguageDto>("/languages", cancellationToken: cancellationToken);
        return languages
            .Where(x => !string.IsNullOrWhiteSpace(x.Code))
            .GroupBy(x => x.Code.Trim(), StringComparer.OrdinalIgnoreCase)
            .Select(group =>
            {
                var language = group.First();
                var code = language.Code.Trim();
                return new DataSourceItem(code, $"{language.Name} ({code})");
            })
            .OrderBy(x => x.DisplayName);
    }
}
