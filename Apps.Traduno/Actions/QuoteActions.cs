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
using Blackbird.Applications.Sdk.Common.Invocation;
using Newtonsoft.Json;
using RestSharp;

namespace Apps.Traduno.Actions;

[ActionList("Quotes")]
public class QuoteActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient)
    : TradunoInvocable(invocationContext)
{
    [Action("Search quotes", Description = "Search quotes using optional filters.")]
    public async Task<SearchQuotesResponse> SearchQuotes([ActionParameter] SearchQuotesInput input)
    {
        var quotes = await Client.GetAllPaginatedAsync<QuoteDto>("/quotes", TradunoRequestMapper.BuildQuoteQuery(input));
        return new SearchQuotesResponse
        {
            Quotes = quotes
        };
    }

    [Action("Create quote request", Description = "Create a quote request with optional source files and one or more deliverables.")]
    public async Task<QuoteDto> CreateQuote([ActionParameter] CreateQuoteInput input)
    {
        var stagedFileIds = input.SourceFiles == null
            ? null
            : await StageFilesAsync(input.SourceFiles, fileManagementClient);
        var request = new TradunoRequest("/quotes", Method.Post, Creds)
            .AddStringBody(JsonConvert.SerializeObject(TradunoRequestMapper.BuildQuoteRequest(input, stagedFileIds), JsonConfig.Settings),
                ContentType.Json);

        return await Client.ExecuteWithErrorHandling<QuoteDto>(request);
    }

    [Action("Get quote", Description = "Get a quote by ID.")]
    public async Task<QuoteDto> GetQuote([ActionParameter] QuoteIdentifier input)
    {
        var request = new TradunoRequest($"/quotes/{input.QuoteId}", Method.Get, Creds);
        return await Client.ExecuteWithErrorHandling<QuoteDto>(request);
    }

    [Action("Accept quote", Description = "Accept a quote.")]
    public async Task<QuoteDto> AcceptQuote([ActionParameter] QuoteIdentifier input)
    {
        var request = new TradunoRequest($"/quotes/{input.QuoteId}/acceptance", Method.Post, Creds);
        await Client.ExecuteWithErrorHandling(request);
        return await GetQuote(input);
    }

    [Action("Reject quote", Description = "Reject a quote with a reason.")]
    public async Task<QuoteDto> RejectQuote([ActionParameter] QuoteIdentifier quote,
        [ActionParameter] RejectQuoteInput input)
    {
        var request = new TradunoRequest($"/quotes/{quote.QuoteId}/reject", Method.Post, Creds)
            .AddStringBody(JsonConvert.SerializeObject(new RejectQuoteRequest
            {
                RejectMessage = input.RejectMessage
            }, JsonConfig.Settings), ContentType.Json);

        await Client.ExecuteWithErrorHandling(request);
        return await GetQuote(quote);
    }
}
