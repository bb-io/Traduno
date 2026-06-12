using Apps.Traduno.Models.Dtos;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Traduno.Handlers;

public class ProjectDataHandler(InvocationContext invocationContext) : TradunoDataSourceHandlerBase(invocationContext)
{
    protected override async Task<IEnumerable<DataSourceItem>> GetDataInternalAsync(CancellationToken cancellationToken)
    {
        var projects = await Client.GetAllPaginatedAsync<ProjectDto>("/projects", cancellationToken: cancellationToken);
        return projects.Select(x =>
            new DataSourceItem(x.Id, string.IsNullOrWhiteSpace(x.Name) ? x.Id : $"{x.Name} ({x.Id})"));
    }
}
