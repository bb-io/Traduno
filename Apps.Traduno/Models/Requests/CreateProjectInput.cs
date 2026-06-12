using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Files;
using Apps.Traduno.Handlers;
using Apps.Traduno.Handlers.Static;
using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Traduno.Models.Requests;

public class CreateProjectInput : ITradunoDeliverableInput
{
    public string? Name { get; set; }

    [Display("PO number")]
    public string? PoNumber { get; set; }

    public string? Notes { get; set; }

    [Display("Job instructions")]
    public string? JobInstructions { get; set; }

    [Display("Translation area")]
    [DataSource(typeof(TranslationAreaDataHandler))]
    public string TranslationAreaId { get; set; } = string.Empty;

    [Display("Currency")]
    [DataSource(typeof(CurrencyDataHandler))]
    public string CurrencyId { get; set; } = string.Empty;

    [Display("Billing entity")]
    [DataSource(typeof(BillingEntityDataHandler))]
    public string? BillingEntityId { get; set; }

    [Display("Delivery files format", Description = "Preferred format for delivered files, for example docx or same as source.")]
    public string DeliveryFilesFormat { get; set; } = string.Empty;

    [Display("Deliverable service code groups", Description = "One item per deliverable. Separate multiple service codes inside an item with commas, for example T,TEP.")]
    [DataSource(typeof(ServiceDataHandler))]
    public IEnumerable<string> DeliverableServiceCodeGroups { get; set; } = [];

    [Display("Deliverable source language codes", Description = "Optional, one item per deliverable.")]
    [DataSource(typeof(LanguageDataHandler))]
    public IEnumerable<string>? DeliverableSourceLanguageCodes { get; set; }

    [Display("Deliverable target language code groups", Description = "One item per deliverable. Separate multiple target language codes inside an item with commas.")]
    [DataSource(typeof(LanguageDataHandler))]
    public IEnumerable<string> DeliverableTargetLanguageCodeGroups { get; set; } = [];

    [Display("Deliverable scheduling modes", Description = "One item per deliverable. Use deadline for date-time values and turnaround for business-day integers.")]
    [StaticDataSource(typeof(DeliverableSchedulingModeDataHandler))]
    public IEnumerable<string> DeliverableSchedulingModes { get; set; } = [];

    [Display("Deliverable deadlines", Description = "One item per deliverable. Used only when the matching scheduling mode is deadline.")]
    public IEnumerable<DateTime?>? DeliverableDeadlines { get; set; }

    [Display("Deliverable turnaround times", Description = "One item per deliverable. Used only when the matching scheduling mode is turnaround.")]
    public IEnumerable<int?>? DeliverableTurnaroundTimes { get; set; }

    [Display("Deliverable descriptions", Description = "Optional, one item per deliverable.")]
    public IEnumerable<string>? DeliverableDescriptions { get; set; }

    [Display("Source files")]
    public IEnumerable<FileReference> SourceFiles { get; set; } = [];
}
