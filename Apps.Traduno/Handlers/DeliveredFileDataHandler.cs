using Apps.Traduno.Models.Dtos;
using Apps.Traduno.Models.Identifiers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Traduno.Handlers;

public class DeliveredFileDataHandler(InvocationContext invocationContext, [ActionParameter] ProjectIdentifier project)
    : TradunoDataSourceHandlerBase(invocationContext)
{
    protected override async Task<IEnumerable<DataSourceItem>> GetDataInternalAsync(CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(project.ProjectId))
        {
            return [];
        }

        var projectDto = await Client.ExecuteWithErrorHandling<ProjectDto>(
            new Api.TradunoRequest($"/projects/{project.ProjectId}", RestSharp.Method.Get, Creds), cancellationToken);

        return projectDto.DeliveredFiles?.Select(x => new DataSourceItem(x.FileId, x.FileId)) ?? [];
    }
}
