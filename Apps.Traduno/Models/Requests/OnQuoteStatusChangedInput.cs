using Apps.Traduno.Handlers.Static;
using Apps.Traduno.Models.Identifiers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.Traduno.Models.Requests;

public class OnQuoteStatusChangedInput : QuoteIdentifier
{
    [Display("Status")]
    [StaticDataSource(typeof(QuoteStatusDataHandler))]
    public string? Status { get; set; }
}
