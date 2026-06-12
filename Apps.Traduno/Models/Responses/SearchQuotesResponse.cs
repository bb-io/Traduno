using Apps.Traduno.Models.Dtos;
using Blackbird.Applications.Sdk.Common;

namespace Apps.Traduno.Models.Responses;

public class SearchQuotesResponse
{
    public IEnumerable<QuoteDto> Quotes { get; set; } = [];

    [Display("Quote IDs")]
    public IEnumerable<string> QuoteIds => Quotes.Select(x => x.Id);
}
