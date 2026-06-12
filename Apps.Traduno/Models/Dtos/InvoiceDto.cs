using Blackbird.Applications.Sdk.Common;

namespace Apps.Traduno.Models.Dtos;

public class InvoiceDto
{
    [Display("Invoice ID")]
    public string Id { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    [Display("Issue date")]
    public DateTime IssueDate { get; set; }

    public MoneyDto Cost { get; set; } = new();

    public DateTime Deadline { get; set; }
}
