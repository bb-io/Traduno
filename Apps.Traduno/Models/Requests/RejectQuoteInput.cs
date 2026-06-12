using Blackbird.Applications.Sdk.Common;

namespace Apps.Traduno.Models.Requests;

public class RejectQuoteInput
{
    [Display("Reject message")]
    public string RejectMessage { get; set; } = string.Empty;
}
