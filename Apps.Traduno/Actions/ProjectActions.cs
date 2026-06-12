using Apps.Traduno.Api;
using Apps.Traduno.Models.Api;
using Apps.Traduno.Models.Dtos;
using Apps.Traduno.Models.Identifiers;
using Apps.Traduno.Models.Requests;
using Apps.Traduno.Models.Responses;
using Apps.Traduno.Utils;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Newtonsoft.Json;
using RestSharp;

namespace Apps.Traduno.Actions;

[ActionList("Projects")]
public class ProjectActions(InvocationContext invocationContext) : TradunoInvocable(invocationContext)
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

    [Action("Create project", Description = "Create a project from staged source files and one or more deliverables.")]
    public async Task<ProjectDto> CreateProject([ActionParameter] CreateProjectInput input)
    {
        var requestBody = JsonConvert.SerializeObject(TradunoRequestMapper.BuildProjectRequest(input), JsonConfig.Settings);
        var request = new TradunoRequest("/projects", Method.Post, Creds)
            .AddStringBody(requestBody, ContentType.Json);

        LogDebug($"CreateProject request body: {requestBody}");

        RestResponse response;
        try
        {
            response = await Client.ExecuteWithErrorHandling(request);
        }
        catch (Exception ex)
        {
            LogDebug($"CreateProject request failed: {ex.Message}");
            throw;
        }

        LogDebug($"CreateProject response status: {(int)response.StatusCode} {response.StatusCode}");
        LogDebug($"CreateProject response body: {response.Content ?? "<empty>"}");
        LogDebug($"CreateProject location header: {GetLocationHeader(response) ?? "<missing>"}");

        var createResponse = JsonConvert.DeserializeObject<CreateProjectResponse>(response.Content ?? string.Empty, JsonConfig.Settings)
            ?? throw new PluginApplicationException(
                $"Could not parse {response.Content} to {nameof(CreateProjectResponse)}");

        var projectIds = new List<string>();
        if (!string.IsNullOrWhiteSpace(createResponse.ProjectId))
        {
            projectIds.Add(createResponse.ProjectId);
        }

        var locationProjectId = ExtractProjectIdFromLocation(response);
        if (!string.IsNullOrWhiteSpace(locationProjectId) && !projectIds.Contains(locationProjectId))
        {
            projectIds.Add(locationProjectId);
        }

        LogDebug($"CreateProject candidate project IDs: {string.Join(", ", projectIds)}");

        if (!projectIds.Any())
        {
            throw new PluginApplicationException("Project was created but no project ID was returned by Traduno.");
        }

        foreach (var projectId in projectIds)
        {
            var project = await TryGetCreatedProject(projectId);
            if (project != null)
            {
                return project;
            }
        }

        var searchedProject = await TryFindCreatedProject(input);
        if (searchedProject != null)
        {
            return searchedProject;
        }

        return new ProjectDto
        {
            Id = projectIds.First(),
            Name = input.Name,
            PoNumber = input.PoNumber,
            Deliverables = []
        };
    }

    [Action("Get project", Description = "Get a project by ID.")]
    public async Task<ProjectDto> GetProject([ActionParameter] ProjectIdentifier input)
    {
        var request = new TradunoRequest($"/projects/{input.ProjectId}", Method.Get, Creds);
        return await Client.ExecuteWithErrorHandling<ProjectDto>(request);
    }

    private async Task<ProjectDto?> TryGetCreatedProject(string projectId)
    {
        const int maxAttempts = 6;

        for (var attempt = 1; attempt <= maxAttempts; attempt++)
        {
            try
            {
                LogDebug($"TryGetCreatedProject attempt {attempt}/{maxAttempts} for project ID: {projectId}");
                return await GetProject(new ProjectIdentifier { ProjectId = projectId });
            }
            catch (PluginApplicationException ex) when (IsProjectNotFound(ex) && attempt < maxAttempts)
            {
                LogDebug($"TryGetCreatedProject attempt {attempt} failed for project ID {projectId}: {ex.Message}");
                await Task.Delay(TimeSpan.FromSeconds(attempt));
            }
        }

        LogDebug($"TryGetCreatedProject exhausted retries for project ID: {projectId}");
        return null;
    }

    private async Task<ProjectDto?> TryFindCreatedProject(CreateProjectInput input)
    {
        const int maxAttempts = 4;

        for (var attempt = 1; attempt <= maxAttempts; attempt++)
        {
            var searchResult = await SearchProjects(new SearchProjectsInput
            {
                PoNumber = input.PoNumber,
                CreatedStart = DateTime.UtcNow.Date,
                ShowAll = true
            });

            var matchedProject = searchResult.Projects
                .Where(x => string.IsNullOrWhiteSpace(input.Name) ||
                            string.Equals(x.Name, input.Name, StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(x => x.CreatedAt)
                .FirstOrDefault();

            if (matchedProject != null)
            {
                return matchedProject;
            }

            if (attempt < maxAttempts)
            {
                await Task.Delay(TimeSpan.FromSeconds(attempt));
            }
        }

        return null;
    }

    private static bool IsProjectNotFound(PluginApplicationException ex) =>
        ex.Message.Contains("Project not found", StringComparison.OrdinalIgnoreCase) ||
        ex.Message.Contains("RESOURCE_NOT_FOUND", StringComparison.OrdinalIgnoreCase);

    private void LogDebug(string message)
    {
        Console.WriteLine($"[Traduno][CreateProject] {message}");
    }

    private static string? GetLocationHeader(RestResponse response) =>
        response.Headers?
            .FirstOrDefault(x => x.Name?.Equals("Location", StringComparison.OrdinalIgnoreCase) == true)
            ?.Value?.ToString();

    private static string? ExtractProjectIdFromLocation(RestResponse response)
    {
        var location = GetLocationHeader(response);

        if (string.IsNullOrWhiteSpace(location) || !Uri.TryCreate(location, UriKind.Absolute, out var uri))
        {
            return null;
        }

        return uri.Segments.LastOrDefault()?.Trim('/');
    }
}
