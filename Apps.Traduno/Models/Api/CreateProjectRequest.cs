namespace Apps.Traduno.Models.Api;

public class CreateProjectRequest
{
    public string? Name { get; set; }

    public string? PoNumber { get; set; }

    public string? Notes { get; set; }

    public string? JobInstructions { get; set; }

    public int? TranslationAreaId { get; set; }

    public int? CurrencyId { get; set; }

    public string? BillingEntityId { get; set; }

    public string? DeliveryFilesFormat { get; set; }

    public IEnumerable<string> SourceFiles { get; set; } = [];

    public IEnumerable<CreateDeliverableRequest> Deliverables { get; set; } = [];
}
