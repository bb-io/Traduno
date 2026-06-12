using Apps.Traduno.Api;
using Apps.Traduno.Models.Dtos;
using Apps.Traduno.Models.Identifiers;
using Apps.Traduno.Models.Requests;
using Apps.Traduno.Models.Responses;
using Apps.Traduno.Utils;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.Traduno.Actions;

[ActionList("Invoices")]
public class InvoiceActions(InvocationContext invocationContext) : TradunoInvocable(invocationContext)
{
    [Action("Search invoices", Description = "Search invoices using optional filters.")]
    public async Task<SearchInvoicesResponse> SearchInvoices([ActionParameter] SearchInvoicesInput input)
    {
        var invoices = await Client.GetAllPaginatedAsync<InvoiceDto>("/invoices", TradunoRequestMapper.BuildInvoiceQuery(input));
        return new SearchInvoicesResponse
        {
            Invoices = invoices
        };
    }

    [Action("Get invoice", Description = "Get an invoice by ID.")]
    public async Task<InvoiceDto> GetInvoice([ActionParameter] InvoiceIdentifier input)
    {
        var request = new TradunoRequest($"/invoices/{input.InvoiceId}", Method.Get, Creds);
        return await Client.ExecuteWithErrorHandling<InvoiceDto>(request);
    }
}
