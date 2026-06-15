using Blackbird.Applications.Sdk.Common;

namespace Apps.Traduno.Models.Dtos;

public class MoneyDto
{
    public decimal Amount { get; set; }

    [Display("Currency")]
    public string Currency { get; set; } = string.Empty;
}
