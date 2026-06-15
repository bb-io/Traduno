using Apps.Traduno.Api;
using Apps.Traduno.Models.Api;
using Apps.Traduno.Models.Dtos;
using Apps.Traduno.Models.Identifiers;
using Apps.Traduno.Models.Requests;
using Apps.Traduno.Models.Responses;
using Apps.Traduno.Utils;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Newtonsoft.Json;
using RestSharp;

namespace Apps.Traduno.Actions;

[ActionList("Projects")]
public class ProjectActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient)
    : TradunoInvocable(invocationContext)
{
    [Action("Search projects", Description = "Search projects using optional filters.")]
    public async Task<SearchProjectsResponse> SearchProjects([ActionParameter] SearchProjectsInput input)
    {
        var projects = await Client.GetAllPaginatedAsync<ProjectDto>("/projects", TradunoRequestMapper.BuildProjectQuery(input));
        return new SearchProjectsResponse
        {
            Projects = projects
        };
    }

    [Action("Create project", Description = "Create a project from source files and one or more deliverables.")]
    public async Task<ProjectDto> CreateProject([ActionParameter] CreateProjectInput input)
    {
        var stagedFileIds = await StageFilesAsync(input.SourceFiles, fileManagementClient);
        var requestBody = JsonConvert.SerializeObject(TradunoRequestMapper.BuildProjectRequest(input, stagedFileIds), JsonConfig.Settings);
        var request = new TradunoRequest("/projects", Method.Post, Creds)
            .AddStringBody(requestBody, ContentType.Json);

        var response = await Client.ExecuteWithErrorHandling(request);

        var createResponse = JsonConvert.DeserializeObject<CreateProjectResponse>(response.Content ?? string.Empty, JsonConfig.Settings)
            ?? throw new PluginApplicationException(
                $"Could not parse {response.Content} to {nameof(CreateProjectResponse)}");

        if (string.IsNullOrWhiteSpace(createResponse.ProjectId))
        {
            throw new PluginApplicationException("Project was created but no project ID was returned by Traduno.");
        }

        return await GetProject(new ProjectIdentifier { ProjectId = createResponse.ProjectId });
    }

    [Action("Get project", Description = "Get a project by ID.")]
    public async Task<ProjectDto> GetProject([ActionParameter] ProjectIdentifier input)
    {
        var request = new TradunoRequest($"/projects/{input.ProjectId}", Method.Get, Creds);
        return await Client.ExecuteWithErrorHandling<ProjectDto>(request);
    }
}
