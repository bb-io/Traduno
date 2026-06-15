using Apps.Traduno.Models.Dtos;
using Apps.Traduno.Models.Requests;
using Apps.Traduno.Api;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Polling;
using RestSharp;

namespace Apps.Traduno.Polling;

[PollingEventList]
public class TradunoPollingList(InvocationContext invocationContext) : TradunoInvocable(invocationContext)
{
    [PollingEvent("On project status changed", "Triggered when the status of a specific project changes.")]
    public async Task<PollingEventResponse<TradunoStatusChangeMemory, ProjectDto>> OnProjectStatusChanged(
        PollingEventRequest<TradunoStatusChangeMemory> request,
        [PollingEventParameter] OnProjectStatusChangedInput input)
    {
        var project = await GetProjectAsync(input.ProjectId);
        return BuildStatusChangedResponse(request.Memory, input.ProjectId, project.Status, input.Status, project);
    }

    [PollingEvent("On quote status changed", "Triggered when the status of a specific quote changes.")]
    public async Task<PollingEventResponse<TradunoStatusChangeMemory, QuoteDto>> OnQuoteStatusChanged(
        PollingEventRequest<TradunoStatusChangeMemory> request,
        [PollingEventParameter] OnQuoteStatusChangedInput input)
    {
        var quote = await GetQuoteAsync(input.QuoteId);
        return BuildStatusChangedResponse(request.Memory, input.QuoteId, quote.Status, input.Status, quote);
    }

    private async Task<ProjectDto> GetProjectAsync(string projectId)
    {
        var request = new TradunoRequest($"/projects/{projectId}", Method.Get, Creds);
        return await Client.ExecuteWithErrorHandling<ProjectDto>(request, CancellationToken.None);
    }

    private async Task<QuoteDto> GetQuoteAsync(string quoteId)
    {
        var request = new TradunoRequest($"/quotes/{quoteId}", Method.Get, Creds);
        return await Client.ExecuteWithErrorHandling<QuoteDto>(request, CancellationToken.None);
    }

    private static PollingEventResponse<TradunoStatusChangeMemory, T> BuildStatusChangedResponse<T>(
        TradunoStatusChangeMemory? memory,
        string entityId,
        string currentStatus,
        string? statusFilter,
        T result)
    {
        var hasPreviousStatus = memory != null &&
                                string.Equals(memory.EntityId, entityId, StringComparison.OrdinalIgnoreCase) &&
                                !string.IsNullOrWhiteSpace(memory.LastStatus);
        var statusChanged = hasPreviousStatus &&
                            !string.Equals(memory!.LastStatus, currentStatus, StringComparison.OrdinalIgnoreCase);
        var matchesFilter = string.IsNullOrWhiteSpace(statusFilter) ||
                            string.Equals(currentStatus, statusFilter, StringComparison.OrdinalIgnoreCase);

        return new PollingEventResponse<TradunoStatusChangeMemory, T>
        {
            FlyBird = statusChanged && matchesFilter,
            Result = result,
            Memory = new TradunoStatusChangeMemory
            {
                LastCheckedAt = DateTime.UtcNow,
                EntityId = entityId,
                LastStatus = currentStatus
            }
        };
    }
}
