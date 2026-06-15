using Blackbird.Applications.Sdk.Common;

namespace Apps.Traduno.Models.Dtos;

public class ProjectDeliverableDto
{
    [Display("Service codes")]
    public IEnumerable<string> ServiceCodes { get; set; } = [];

    [Display("Source language code")]
    public string? SourceLanguageCode { get; set; }

    [Display("Target language codes")]
    public IEnumerable<string> TargetLanguageCodes { get; set; } = [];

    public DateTime? Deadline { get; set; }

    public string? Description { get; set; }

    public QuantityDto? Quantity { get; set; }
}
