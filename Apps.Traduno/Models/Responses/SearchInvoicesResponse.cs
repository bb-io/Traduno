using Apps.Traduno.Models.Dtos;
using Blackbird.Applications.Sdk.Common;

namespace Apps.Traduno.Models.Responses;

public class SearchInvoicesResponse
{
    public IEnumerable<InvoiceDto> Invoices { get; set; } = [];

    [Display("Invoice IDs")]
    public IEnumerable<string> InvoiceIds => Invoices.Select(x => x.Id);
}
