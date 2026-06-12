using Apps.Traduno.Handlers;
using Apps.Traduno.Handlers.Static;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Traduno.Models.Requests;

public class SearchQuotesInput
{
    [Display("Status")]
    [StaticDataSource(typeof(QuoteStatusDataHandler))]
    public string? Status { get; set; }

    [Display("Service")]
    [DataSource(typeof(ServiceDataHandler))]
    public string? ServiceCode { get; set; }

    [Display("Translation area")]
    [DataSource(typeof(TranslationAreaDataHandler))]
    public string? TranslationAreaId { get; set; }

    [Display("PO number")]
    public string? PoNumber { get; set; }

    [Display("Created from")]
    public DateTime? CreatedStart { get; set; }

    [Display("Created to")]
    public DateTime? CreatedEnd { get; set; }

    [Display("Project deadline from")]
    public DateTime? ProjectDeadlineStart { get; set; }

    [Display("Project deadline to")]
    public DateTime? ProjectDeadlineEnd { get; set; }

    [Display("Show all accessible quotes")]
    public bool? ShowAll { get; set; } = true;
}
