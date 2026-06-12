using Apps.Traduno.Handlers;
using Apps.Traduno.Handlers.Static;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Traduno.Models.Requests;

public abstract class CreateTradunoEntityInputBase
{
    public string? Name { get; set; }

    [Display("PO number")]
    public string? PoNumber { get; set; }

    public string? Notes { get; set; }

    [Display("Job instructions")]
    public string? JobInstructions { get; set; }

    [Display("Translation area")]
    [DataSource(typeof(TranslationAreaDataHandler))]
    public string? TranslationAreaId { get; set; }

    [Display("Currency")]
    [DataSource(typeof(CurrencyDataHandler))]
    public string? CurrencyId { get; set; }

    [Display("Billing entity")]
    [DataSource(typeof(BillingEntityDataHandler))]
    public string? BillingEntityId { get; set; }

    [Display("Callback URL")]
    public string? CallbackUrl { get; set; }

    [Display("Delivery files format", Description = "Preferred format for delivered files, for example docx or same as source.")]
    public string? DeliveryFilesFormat { get; set; }

    [Display("Deliverable service code groups", Description = "One item per deliverable. Separate multiple service codes inside an item with commas, for example T,TEP.")]
    [DataSource(typeof(ServiceDataHandler))]
    public IEnumerable<string> DeliverableServiceCodeGroups { get; set; } = [];

    [Display("Deliverable source language codes", Description = "One item per deliverable. Leave empty for language-independent services.")]
    [DataSource(typeof(LanguageDataHandler))]
    public IEnumerable<string>? DeliverableSourceLanguageCodes { get; set; }

    [Display("Deliverable target language code groups", Description = "One item per deliverable. Separate multiple target language codes inside an item with commas.")]
    [DataSource(typeof(LanguageDataHandler))]
    public IEnumerable<string>? DeliverableTargetLanguageCodeGroups { get; set; }

    [Display("Deliverable scheduling modes", Description = "One item per deliverable. Use deadline for ISO date-time values and turnaround for business-day integers.")]
    [StaticDataSource(typeof(DeliverableSchedulingModeDataHandler))]
    public IEnumerable<string> DeliverableSchedulingModes { get; set; } = [];

    [Display("Deliverable scheduling values", Description = "One item per deliverable. Use ISO 8601 date-time values for deadline mode or integer values for turnaround mode.")]
    public IEnumerable<string> DeliverableSchedulingValues { get; set; } = [];

    [Display("Deliverable descriptions", Description = "Optional, one item per deliverable.")]
    public IEnumerable<string>? DeliverableDescriptions { get; set; }
}
