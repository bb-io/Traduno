using Apps.Traduno.Handlers.Static;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.Traduno.Models.Requests;

public class SearchInvoicesInput
{
    [Display("Invoice name")]
    public string? Name { get; set; }

    [Display("Status")]
    [StaticDataSource(typeof(InvoiceStatusDataHandler))]
    public string? Status { get; set; }

    [Display("Issued from")]
    public DateTime? IssuedStart { get; set; }

    [Display("Issued to")]
    public DateTime? IssuedEnd { get; set; }

    [Display("Payment deadline from")]
    public DateTime? DeadlineStart { get; set; }

    [Display("Payment deadline to")]
    public DateTime? DeadlineEnd { get; set; }
}
