using Apps.Traduno.Handlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Traduno.Models.Identifiers;

public class QuoteIdentifier
{
    [Display("Quote ID")]
    [DataSource(typeof(QuoteDataHandler))]
    public string QuoteId { get; set; } = string.Empty;
}
