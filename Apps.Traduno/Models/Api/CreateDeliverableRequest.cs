namespace Apps.Traduno.Models.Api;

public class CreateDeliverableRequest
{
    public IEnumerable<string> ServiceCodes { get; set; } = [];

    public string? SourceLanguageCode { get; set; }

    public IEnumerable<string>? TargetLanguageCodes { get; set; }

    public DateTime? Deadline { get; set; }

    public int? TurnaroundTime { get; set; }

    public string? Description { get; set; }
}
