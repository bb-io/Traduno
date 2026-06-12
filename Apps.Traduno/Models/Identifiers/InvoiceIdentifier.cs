using Apps.Traduno.Handlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Traduno.Models.Identifiers;

public class InvoiceIdentifier
{
    [Display("Invoice ID")]
    [DataSource(typeof(InvoiceDataHandler))]
    public string InvoiceId { get; set; } = string.Empty;
}
