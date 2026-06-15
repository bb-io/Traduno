using Blackbird.Applications.Sdk.Common;

namespace Apps.Traduno.Models.Dtos;

public class QuoteDeliverableDto
{
    [Display("Service codes")]
    public IEnumerable<string> ServiceCodes { get; set; } = [];

    [Display("Source language code")]
    public string? SourceLanguageCode { get; set; }

    [Display("Target language codes")]
    public IEnumerable<string> TargetLanguageCodes { get; set; } = [];

    public DateTime? Deadline { get; set; }

    [Display("Turnaround time")]
    public int? TurnaroundTime { get; set; }

    public string? Description { get; set; }

    public QuantityDto? Quantity { get; set; }
}
